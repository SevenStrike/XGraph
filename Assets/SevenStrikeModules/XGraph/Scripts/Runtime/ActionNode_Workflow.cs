namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class ActionNode_Workflow : MonoBehaviour
    {
        /// <summary>
        /// 原始行为资源
        /// </summary>
        public ActionNode_Asset ActionAsset;
        /// <summary>
        /// 运行时使用的克隆行为资源
        /// </summary>
        public ActionNode_Asset ActionAssetClone;
        /// <summary>
        /// 流程运行中
        /// </summary>
        private bool isRunning = false;
        /// <summary>
        /// 流程暂停中
        /// </summary>
        private bool isPaused = false;
        /// <summary>
        /// 编辑器内运行时保存参数的改动
        /// </summary>
        public bool SaveValueChangesInRuntime = true;
        /// <summary>
        /// 显示调试信息
        /// </summary>
        public bool showLogs = true;

        private void Start()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
            if (SaveValueChangesInRuntime)
                return;

            if (obj == PlayModeStateChange.EnteredPlayMode)
            {
                // 创建运行时克隆
                if (!CreateRuntimeClone())
                {
                    Log(MessageType.Error, $"无法创建克隆资源！", "");
                    return;
                }
            }
            if (obj == PlayModeStateChange.ExitingPlayMode)
            {
                EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
                // 恢复原始资源
                RestoreOriginalAsset();
            }
        }
#endif

        /// <summary>
        /// 行为开始
        /// </summary>
        public void Action_Start()
        {
            if (!Action_Validate())
            {
                Log(MessageType.Error, $"行为流程启动失败！", "");
                return;
            }

            if (isRunning)
            {
                Log(MessageType.Error, $"行为流程已经在执行中！", "");
                return;
            }

            StopAllCoroutines();
            StartCoroutine(Action_Flow());
        }

        /// <summary>
        /// 行为停止
        /// </summary>
        public void Action_Stop()
        {
            isRunning = false;
            // 强制解除暂停
            isPaused = false;
            Log(MessageType.Info, $"流程停止！", "");
        }

        /// <summary>
        /// 行为暂停
        /// </summary>
        public void Action_Pause()
        {
            if (isRunning)
            {
                isPaused = true;
                Log(MessageType.Info, $"流程暂停！", "");
            }
        }

        /// <summary>
        /// 行为恢复
        /// </summary>
        public void Action_Resume()
        {
            if (isRunning && isPaused)
            {
                isPaused = false;
                Log(MessageType.Info, $"流程继续！", "");
            }
        }

        /// <summary>
        /// 行为流程
        /// </summary>
        IEnumerator Action_Flow()
        {
            isRunning = true;
            Log(MessageType.Warning, $"开始执行流程：", ActionAsset.name);

            var startNode = ActionAsset.Actions.Find(n => n.actionNodeType == "Start");
            yield return Action_Execute(startNode);

            isRunning = false;
            Log(MessageType.Warning, $"流程执行完成：", ActionAsset.name);
        }

        /// <summary>
        /// 行为执行
        /// </summary>
        IEnumerator Action_Execute(ActionNode_Base action)
        {
            // 检查执行条件
            if (action == null || !isRunning) yield break;

            // 处理暂停状态（双重检查）
            while (isPaused && isRunning)
            {
                Log(MessageType.Warning, $"执行暂停中：", $"{action.identifyName}  （{(action.isConcurrentExecution ? "并发" : "顺序")}）");

                // 每0.1秒检查一次，防止暂停时性能开销大
                yield return new WaitForSeconds(0.1f);
            }

            // 处理特殊节点类型
            if (action is ActionNode_Wait waitNode)
            {
                yield return HandlePausableWait(waitNode);
                yield break;
            }

            Log(MessageType.Info, $"---> ：", $"{action.identifyName}  （{(action.isConcurrentExecution ? "并发" : "顺序")}）");
            // 执行当前节点
            action.Execute();

            // 获取子节点
            var childrens = Action_GetChildrenNodes(action);

            if (childrens.Count == 0)
                yield break;

            bool v = action is ActionNode_Branch;

            // 根据模式执行子节点
            if (action.isConcurrentExecution && !v)
            {
                yield return Action_Execute_Concurrent(childrens);
            }
            else
            {
                yield return Action_Execute_Sequential(childrens);
            }
        }

        private IEnumerator HandlePausableWait(ActionNode_Wait waitNode)
        {
            // 初始检查
            if (!isRunning || waitNode == null) yield break;

            // 执行等待前逻辑
            waitNode.Execute();
            Log(MessageType.Info, $"---> ：", $"{waitNode.identifyName}  {waitNode.Time}s  （{(waitNode.isConcurrentExecution ? "并发" : "顺序")}）");

            // 可中断的等待实现
            float elapsed = 0;
            while (elapsed < waitNode.Time && isRunning) // 持续检查运行状态
            {
                // 处理暂停
                while (isPaused && isRunning)
                {
                    yield return null;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // 最终状态检查
            if (!isRunning) yield break;

            // 执行子节点（必须包含！）
            var children = Action_GetChildrenNodes(waitNode);
            if (children.Count > 0)
            {
                yield return waitNode.isConcurrentExecution ? Action_Execute_Concurrent(children) : Action_Execute_Sequential(children);
            }
        }

        /// <summary>
        /// 顺序执行子节点
        /// </summary>
        private IEnumerator Action_Execute_Sequential(List<ActionNode_Base> children)
        {
            foreach (var child in children)
            {
                if (!isRunning) yield break;
                yield return Action_Execute(child);
            }
        }

        /// <summary>
        /// 并发执行子节点
        /// </summary>
        private IEnumerator Action_Execute_Concurrent(List<ActionNode_Base> children)
        {
            int completedCount = 0;
            var runningCoroutines = new Coroutine[children.Count];

            // 启动所有协程
            for (int i = 0; i < children.Count; i++)
            {
                runningCoroutines[i] = StartCoroutine(
                    Action_ExecuteWithCallback(children[i], () => completedCount++)
                );
            }

            // 等待所有完成
            while (completedCount < children.Count && isRunning)
            {
                yield return null;
            }

            // 清理未完成的协程
            if (completedCount < children.Count)
            {
                foreach (var coroutine in runningCoroutines)
                {
                    if (coroutine != null) StopCoroutine(coroutine);
                }
            }
        }

        /// <summary>
        /// 带回调的执行方法
        /// </summary>
        private IEnumerator Action_ExecuteWithCallback(ActionNode_Base node, Action callback)
        {
            yield return Action_Execute(node);
            callback?.Invoke();
        }

        /// <summary>
        /// 返回当前节点下的子节点列表
        /// </summary>
        private List<ActionNode_Base> Action_GetChildrenNodes(ActionNode_Base current)
        {
            var list = new List<ActionNode_Base>();

            if (current is ActionNode_Start start && start.childNode != null)
                list.Add(start.childNode);
            else if (current is ActionNode_Wait wait)
                list.AddRange(wait.childNodes.FindAll(n => n != null));
            else if (current is ActionNode_Composite composite)
                list.AddRange(composite.childNodes.FindAll(n => n != null));
            else if (current is ActionNode_Debug debug)
                list.AddRange(debug.childNodes.FindAll(n => n != null));
            else if (current is ActionNode_Relay relay)
                list.AddRange(relay.childNodes.FindAll(n => n != null));
            else if (current is ActionNode_Branch branch)
            {
                if (branch.PredicateState)
                {
                    list.Add(branch.childNode_true);
                }
                else
                {
                    list.Add(branch.childNode_false);
                }
            }
            return list;
        }

        /// <summary>
        /// 验证行为树
        /// </summary>
        private bool Action_Validate()
        {
            if (ActionAsset == null || ActionAsset.Actions.Count == 0)
            {
                Log(MessageType.Error, $"行为资源列表是空的！", "");
                return false;
            }

            var start = ActionAsset.Actions.Find(n => n.actionNodeType == "Start");
            if (start == null)
            {
                Log(MessageType.Warning, $"未能找到  Start  节点！", "");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 日志工具
        /// </summary>
        /// <param name="type"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        private void Log(MessageType type, string title, string message)
        {
            string hexcolor = "";
            string mark = "";
            switch (type)
            {
                case MessageType.Info:
                    hexcolor = "DFDFDF";
                    mark = "   ┠─ ■";
                    break;
                case MessageType.Warning:
                    hexcolor = "FFC320";
                    mark = "▲";
                    break;
                case MessageType.Error:
                    hexcolor = "FF5050";
                    mark = "●";
                    break;
            }
            if (showLogs)
                Debug.Log($"<color=#{hexcolor}>{mark}  {title}</color> {message}");
        }

        #region 运行时克隆
        /// <summary>
        /// 创建运行时克隆
        /// </summary>
        private bool CreateRuntimeClone()
        {
            if (ActionAsset == null)
            {
                Log(MessageType.Error, $"无法创建克隆资源！", "");
                return false;
            }

            ActionAssetClone = ActionAsset.Clone("", false);
            return true;
        }

        /// <summary>
        /// 恢复原始资源
        /// </summary>
        private void RestoreOriginalAsset()
        {
            if (ActionAssetClone != null)
            {
                ActionAsset.Replace(ActionAssetClone);
                Log(MessageType.Warning, $"已恢复原始行为资源！", "");
                ActionAssetClone = null;
            }
        }
        #endregion
    }
}