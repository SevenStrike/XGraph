namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ActionTreeController : MonoBehaviour
    {
        public ActionNode_Asset ActionAsset;
        private bool isRunning = false;
        private bool isPaused = false;
        [Header("Debug Settings")]
        public bool showLogs = true;

        /// <summary>
        /// 行为开始
        /// </summary>
        public void Action_Start()
        {
            if (!Action_Validate())
            {
                LogError("行为树启动失败");
                return;
            }

            if (isRunning)
            {
                LogWarning("行为树已在运行中");
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
            Log("行为树已停止");
        }

        /// <summary>
        /// 行为暂停
        /// </summary>
        public void Action_Pause()
        {
            if (isRunning)
            {
                isPaused = true;
                Log("行为树已暂停");
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
                Log("行为树已恢复");
            }
        }

        /// <summary>
        /// 行为流程
        /// </summary>
        IEnumerator Action_Flow()
        {
            isRunning = true;
            Log("----------------->> Actions Start");

            var startNode = ActionAsset.ActionNodes.Find(n => n.actionNodeType == "Start");
            yield return Action_Execute(startNode);

            isRunning = false;
            Log("----------------->> Actions Finished");
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
                LogWarning($"[暂停中] 节点: {action.identifyName}");

                // 每0.1秒检查一次，防止暂停时性能开销大
                yield return new WaitForSeconds(0.1f);
            }

            // 处理特殊节点类型
            if (action is ActionNode_Wait waitNode)
            {
                yield return HandlePausableWait(waitNode);
                yield break;
            }

            Log($"执行节点: {action.identifyName}");
            // 执行当前节点
            action.Execute();

            // 获取子节点
            var children = Action_GetChildrenNodes(action);
            if (children.Count == 0) yield break;

            // 根据模式执行子节点
            if (action.isConcurrentExecution)
            {
                yield return Action_Execute_Concurrent(children);
            }
            else
            {
                yield return Action_Execute_Sequential(children);
            }
        }
        private IEnumerator HandlePausableWait(ActionNode_Wait waitNode)
        {
            // 初始检查
            if (!isRunning || waitNode == null) yield break;

            // 执行等待前逻辑
            waitNode.Execute();
            Log($"⏳ 开始等待: {waitNode.identifyName} ({waitNode.Time}s)");

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
                Log($"⌛ 等待完成，执行 {children.Count} 个子节点");
                yield return waitNode.isConcurrentExecution ?
                    Action_Execute_Concurrent(children) :
                    Action_Execute_Sequential(children);
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
            else if (current is ActionNode_Debug debug && debug.childNode != null)
                list.Add(debug.childNode);
            else if (current is ActionNode_Relay relay)
                list.AddRange(relay.childNodes.FindAll(n => n != null));

            return list;
        }

        /// <summary>
        /// 验证行为树
        /// </summary>
        private bool Action_Validate()
        {
            if (ActionAsset == null || ActionAsset.ActionNodes.Count == 0)
            {
                LogError("没有行为树资源！");
                return false;
            }

            var start = ActionAsset.ActionNodes.Find(n => n.actionNodeType == "Start");
            if (start == null)
            {
                LogError("缺少 Start 节点！");
                return false;
            }

            return true;
        }

        #region 日志工具
        /// <summary>
        /// 消息
        /// </summary>
        /// <param name="message"></param>
        private void Log(string message)
        {
            if (showLogs) Debug.Log($"[ActionTree] {message}");
        }

        /// <summary>
        /// 警告消息
        /// </summary>
        /// <param name="message"></param>
        private void LogWarning(string message)
        {
            if (showLogs) Debug.LogWarning($"[ActionTree] {message}");
        }

        /// <summary>
        /// 错误消息
        /// </summary>
        /// <param name="message"></param>
        private void LogError(string message)
        {
            Debug.LogError($"[ActionTree] {message}");
        }

        private void LogWaitProgress(ActionNode_Wait node, float elapsed)
        {
            if (showLogs)
                Debug.Log($"[等待中] {node.identifyName} ({elapsed:F1}/{node.Time:F1}s)");
        }
        #endregion

    }
}