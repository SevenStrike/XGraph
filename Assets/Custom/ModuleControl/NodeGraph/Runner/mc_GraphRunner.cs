namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class mc_GraphRunner : MonoBehaviour
    {
        /// <summary>
        /// 原始行为资源
        /// </summary>
        public mc_GraphAsset SampleAsset;
        /// <summary>
        /// 原始行为资源（克隆）
        /// </summary>
        public mc_GraphAsset SampleAssetClone; // 修改类型为 mc_GraphAsset
        /// <summary>
        /// 流程运行中
        /// </summary>
        private bool isRunning = false;
        /// <summary>
        /// 流程暂停中
        /// </summary>
        private bool isPaused = false;
        /// <summary>
        /// 运行时修改
        /// </summary>
        public bool RuntimeSave = false;

        /// <summary>
        /// 自定义的目标脚本，用于赋值给行为资源中的目标脚本对象，利用此脚本可达成行为节点对目标脚本功能的调用
        /// </summary>
        public ModuleController TargetScript;

        #region 手动执行
        /// <summary>
        /// 手动执行模式开关
        /// </summary>
        public bool ManualExecutionMode = false;
        /// <summary>
        /// 当前正在执行的节点（手动模式下）
        /// </summary>
        private string currentManualNode;
        /// <summary>
        /// 手动执行模式下是否正在等待延时节点
        /// </summary>
        private bool isWaitingForDelay = false;
        /// <summary>
        /// 手动执行模式下的节点执行队列
        /// </summary>
        private Queue<string> manualExecutionQueue = new Queue<string>();
        /// <summary>
        /// 手动执行完成回调
        /// </summary>
        public Action OnManual_StepComplete;
        /// <summary>
        /// 手动执行等待结束回调
        /// </summary>
        public Action OnManual_WaitComplete;
        #endregion

        private void Start()
        {
            Asset_Clone();

            // 为每一个行为注册运行时变量值改变回调
            RegisterVariableChangeWithAction();

            SampleAsset.ModuleController = TargetScript;
        }

        /// <summary>
        /// 克隆资源副本
        /// </summary>
        private void Asset_Clone()
        {
            if (!Application.isEditor)
                return;
            if (!RuntimeSave)
                return;

            if (SampleAsset == null)
            {
                Debug.LogError("SampleAsset 为 null，无法克隆");
                return;
            }

            // 克隆为正确的类型
            SampleAssetClone = Instantiate(SampleAsset);
            SampleAssetClone.name = SampleAsset.name;

            Debug.Log($"已克隆资源: {SampleAsset.name} -> {SampleAssetClone.name}");
        }

        /// <summary>
        /// 还原资源
        /// </summary>
        private void Asset_Restore()
        {
            if (!Application.isEditor)
                return;
            if (!RuntimeSave)
                return;

            if (SampleAsset == null || SampleAssetClone == null)
            {
                Debug.LogError("无法恢复资源: SampleAsset 或 SampleAssetClone 为 null");
                return;
            }

            try
            {
                // 使用 Replace 方法恢复数据
                SampleAsset.Replace(SampleAssetClone);

                Debug.Log($"已恢复资源: {SampleAsset.name}");

                // 清理克隆资源
                if (Application.isEditor && !Application.isPlaying)
                {
                    DestroyImmediate(SampleAssetClone, true);
                }
                else
                {
                    Destroy(SampleAssetClone);
                }

                SampleAssetClone = null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"恢复资源时出错: {ex.Message}");
            }
        }

        private void OnDisable()
        {
            // 注销每一个行为运行时变量值改变回调
            UnregisterVariableChangeWithAction();

            // 清理手动执行模式
            Manual_CleanupMode();

            // 仅在运行时且克隆存在时恢复
            if (Application.isPlaying && SampleAssetClone != null)
            {
                Asset_Restore();
            }
        }

        void Update()
        {
            // 手动模式下检查等待状态
            if (ManualExecutionMode && isWaitingForDelay)
            {
                // 可以在这里添加等待状态的UI提示等
            }
        }

        private void OnDestroy()
        {
            // 确保在对象销毁时也恢复资源
            if (SampleAssetClone != null)
            {
                Asset_Restore();
            }
        }

        #region Runner 手动模式
        /// <summary>
        /// 手动执行下一步（仅在手动模式下有效）
        /// </summary>
        public void Manual_Action_Execution()
        {
            if (!ManualExecutionMode || !isRunning)
            {
                util_Dashboard.LogMsg(xMessageType.警告, $"手动执行模式未启用或流程未运行", "", SampleAsset.LogEnabled);
                return;
            }

            if (isWaitingForDelay)
            {
                util_Dashboard.LogMsg(xMessageType.警告, $"正在等待延时节点完成，请稍后再试", "", SampleAsset.LogEnabled);
                return;
            }

            if (manualExecutionQueue.Count == 0)
            {
                util_Dashboard.LogMsg(xMessageType.信息, $"流程执行完成", "", SampleAsset.LogEnabled);
                isRunning = false;
                OnManual_StepComplete?.Invoke();
                return;
            }

            currentManualNode = manualExecutionQueue.Dequeue();
            StartCoroutine(Manual_ExecuteAction(currentManualNode));
        }
        /// <summary>
        /// 初始化手动执行模式
        /// </summary>
        private void Manual_InitializeMode()
        {
            Manual_CleanupMode();

            var startNode = SampleAsset.Actions.Find(n => n.BaseArgs.isStartNode);
            if (startNode != null)
            {
                isRunning = true;
                manualExecutionQueue.Enqueue(startNode.BaseArgs.guid);
                util_Dashboard.LogMsg(xMessageType.信息, $"手动执行模式已初始化，准备执行第一个节点", "", SampleAsset.LogEnabled);
            }
        }
        /// <summary>
        /// 执行手动模式下的单个节点
        /// </summary>
        private IEnumerator Manual_ExecuteAction(string guid)
        {
            xAction_Base node = SampleAsset.FindActionNode(guid);

            if (node == null || !isRunning) yield break;

            util_Dashboard.LogMsg(xMessageType.信息, $"[手动] ---> ：", $"{node.identifyName}  （{(node.BaseArgs.isConcurrentExecution ? "并发" : "顺序")}）", SampleAsset.LogEnabled);

            // 特殊处理等待节点
            if (node is xAction_Wait waitNode)
            {
                yield return Manual_HandleWait(waitNode);
            }
            else
            {
                // 执行普通节点
                node.Execute();

                // 获取子节点并加入队列
                var children = Action_GetChildrenActions(node.BaseArgs.guid);
                foreach (var child in children)
                {
                    if (child != null)
                        manualExecutionQueue.Enqueue(child);
                }

                OnManual_StepComplete?.Invoke();
            }
        }
        /// <summary>
        /// 手动模式下的等待节点处理
        /// </summary>
        private IEnumerator Manual_HandleWait(xAction_Wait waitNode)
        {
            isWaitingForDelay = true;
            util_Dashboard.LogMsg(xMessageType.信息, $"[手动] ---> 等待节点：", $"{waitNode.identifyName}  {waitNode.Time}s", SampleAsset.LogEnabled);

            // 执行等待节点
            waitNode.Execute();

            float elapsed = 0;
            while (elapsed < waitNode.Time && isRunning)
            {
                // 处理暂停
                while (isPaused && isRunning)
                {
                    yield return null;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!isRunning)
            {
                isWaitingForDelay = false;
                yield break;
            }

            // 等待完成后获取子节点
            var children = Action_GetChildrenActions(waitNode.BaseArgs.guid);
            foreach (var child in children)
            {
                if (child != null)
                    manualExecutionQueue.Enqueue(child);
            }

            isWaitingForDelay = false;
            OnManual_WaitComplete?.Invoke();
            util_Dashboard.LogMsg(xMessageType.信息, $"[手动] 等待节点完成", "", SampleAsset.LogEnabled);
        }
        /// <summary>
        /// 清理手动执行模式
        /// </summary>
        private void Manual_CleanupMode()
        {
            manualExecutionQueue.Clear();
            currentManualNode = null;
            isWaitingForDelay = false;
            StopAllCoroutines();
        }
        #endregion

        #region Runner 流程执行控制
        /// <summary>
        /// 验证是否存在起始节点
        /// </summary>
        private bool Runner_StartNodeValidate()
        {
            if (SampleAsset == null || SampleAsset.Actions.Count == 0)
            {
                util_Dashboard.LogMsg(xMessageType.错误, $"行为资源列表是空的！", "", SampleAsset.LogEnabled);
                return false;
            }

            var start = SampleAsset.Actions.Find(n => n.BaseArgs.isStartNode);
            if (start == null)
            {
                util_Dashboard.LogMsg(xMessageType.警告, $"未能找到指定的起始节点！", "", SampleAsset.LogEnabled);
                return false;
            }

            return true;
        }
        /// <summary>
        /// 行为开始
        /// </summary>
        public void Runner_Start()
        {
            if (!Runner_StartNodeValidate())
            {
                util_Dashboard.LogMsg(xMessageType.错误, $"行为流程启动失败！", "", SampleAsset.LogEnabled);
                return;
            }

            if (isRunning)
            {
                util_Dashboard.LogMsg(xMessageType.错误, $"行为流程已经在执行中！", "", SampleAsset.LogEnabled);
                return;
            }

            // 手动执行模式初始化
            if (ManualExecutionMode)
            {
                Manual_InitializeMode();
                util_Dashboard.LogMsg(xMessageType.警告, $"手动执行模式已启动，等待手动执行指令", "", SampleAsset.LogEnabled);
                return;
            }

            StopAllCoroutines();
            StartCoroutine(Action_Flow());
        }
        /// <summary>
        /// 行为停止
        /// </summary>
        public void Runner_Kill()
        {
            isRunning = false;
            // 强制解除暂停
            isPaused = false;

            // 清理手动模式
            if (ManualExecutionMode)
            {
                Manual_CleanupMode();
            }

            // 注销每一个行为运行时变量值改变回调
            UnregisterVariableChangeWithAction();

            util_Dashboard.LogMsg(xMessageType.信息, $"流程停止！", "", SampleAsset.LogEnabled);
        }
        /// <summary>
        /// 行为暂停
        /// </summary>
        public void Runner_Pause()
        {
            if (isRunning)
            {
                isPaused = true;
                util_Dashboard.LogMsg(xMessageType.信息, $"流程暂停！", "", SampleAsset.LogEnabled);
            }
        }
        /// <summary>
        /// 行为恢复
        /// </summary>
        public void Runner_Resume()
        {
            if (isRunning && isPaused)
            {
                isPaused = false;
                util_Dashboard.LogMsg(xMessageType.信息, $"流程继续！", "", SampleAsset.LogEnabled);
            }
        }
        #endregion

        #region Runner 行为逻辑
        /// <summary>
        /// 行为流程
        /// </summary>
        IEnumerator Action_Flow()
        {
            isRunning = true;
            util_Dashboard.LogMsg(xMessageType.警告, $"开始执行流程：", SampleAsset.name, "00ff9d", SampleAsset.LogEnabled);

            var startNode = SampleAsset.Actions.Find(n => n.BaseArgs.isStartNode);
            yield return Action_Execute(startNode.BaseArgs.guid);

            isRunning = false;
            util_Dashboard.LogMsg(xMessageType.警告, $"流程执行完成：", SampleAsset.name, "00ff9d", SampleAsset.LogEnabled);
        }
        /// <summary>
        /// 行为执行
        /// </summary>
        IEnumerator Action_Execute(string guid)
        {
            xAction_Base action = SampleAsset.FindActionNode(guid);

            // 检查执行条件
            if (action == null || !isRunning) yield break;

            // 处理暂停状态（双重检查）
            while (isPaused && isRunning)
            {
                util_Dashboard.LogMsg(xMessageType.警告, $"执行暂停中：", $"{action.identifyName}  （{(action.BaseArgs.isConcurrentExecution ? "并发" : "顺序")}）", SampleAsset.LogEnabled);

                // 每0.1秒检查一次，防止暂停时性能开销大
                yield return new WaitForSeconds(0.1f);
            }

            // 处理特殊节点类型
            if (action is xAction_Wait waitNode)
            {
                yield return Action_HandlePausableWait(waitNode);
                yield break;
            }

            util_Dashboard.LogMsg(xMessageType.信息, $"---> ：", $"{action.identifyName}  （{(action.BaseArgs.isConcurrentExecution ? "并发" : "顺序")}）", SampleAsset.LogEnabled);
            // 执行当前节点
            action.Execute();

            // 获取子节点
            var childrens = Action_GetChildrenActions(action.BaseArgs.guid);

            if (childrens.Count == 0)
                yield break;

            bool v = action is xAction_Branch;

            // 根据模式执行子节点
            if (action.BaseArgs.isConcurrentExecution && !v)
            {
                yield return Action_Execute_Concurrent(childrens);
            }
            else
            {
                yield return Action_Execute_Sequential(childrens);
            }
        }
        private IEnumerator Action_HandlePausableWait(xAction_Wait waitNode)
        {
            // 初始检查
            if (!isRunning || waitNode == null) yield break;

            // 执行等待前逻辑
            waitNode.Execute();
            util_Dashboard.LogMsg(xMessageType.信息, $"---> ：", $"{waitNode.identifyName}  {waitNode.Time}s  （{(waitNode.BaseArgs.isConcurrentExecution ? "并发" : "顺序")}）", SampleAsset.LogEnabled);

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
            var children = Action_GetChildrenActions(waitNode.BaseArgs.guid);
            if (children.Count > 0)
            {
                yield return waitNode.BaseArgs.isConcurrentExecution ? Action_Execute_Concurrent(children) : Action_Execute_Sequential(children);
            }
        }
        /// <summary>
        /// 顺序执行子节点
        /// </summary>
        private IEnumerator Action_Execute_Sequential(List<string> children)
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
        private IEnumerator Action_Execute_Concurrent(List<string> children)
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
        private IEnumerator Action_ExecuteWithCallback(string guid, Action callback)
        {
            yield return Action_Execute(guid);
            callback?.Invoke();
        }
        /// <summary>
        /// 返回当前节点下的子节点列表
        /// </summary>
        private List<string> Action_GetChildrenActions(string guid)
        {
            xAction_Base current = SampleAsset.FindActionNode(guid);

            var list = new List<string>();

            if (current is xAction_Start start && start.childNodes != null)
                list.AddRange(start.childNodes.FindAll(n => n != null));
            else if (current is xAction_Wait wait)
                list.AddRange(wait.childNodes.FindAll(n => n != null));
            else if (current is xAction_Composite composite)
                list.AddRange(composite.childNodes.FindAll(n => n != null));
            else if (current is xAction_Debug debug)
                list.AddRange(debug.childNodes.FindAll(n => n != null));
            else if (current is xAction_Relay relay)
                list.AddRange(relay.childNodes.FindAll(n => n != null));
            else if (current is xAction_Branch branch)
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
        #endregion

        #region 辅助
        /// <summary>
        /// 设置节点资源
        /// </summary>
        /// <param name="asset"></param>
        public void SetActionAsset(xAction_Asset asset)
        {
            // 转换后赋值
            SampleAsset = asset as mc_GraphAsset;
            // 将目标控制脚本赋值
            SampleAsset.ModuleController = TargetScript;
        }
        #endregion

        #region 为每一个继承ActionBase的对象注册变量数值变化回调
        /// <summary>
        /// 为每一个行为注册运行时变量值改变回调
        /// </summary>
        public void RegisterVariableChangeWithAction()
        {
            foreach (var action in SampleAsset.Actions)
            {
                action.RegisterVariableValueChanged();
            }
        }
        /// <summary>
        /// 注销每一个行为运行时变量值改变回调
        /// </summary>
        public void UnregisterVariableChangeWithAction()
        {
            foreach (var action in SampleAsset.Actions)
            {
                action.UnregisterVariableValueChanged();
            }
        }
        #endregion
    }
}