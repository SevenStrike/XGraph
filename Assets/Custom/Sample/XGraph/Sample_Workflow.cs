using SevenStrikeModules.XGraph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sample_Workflow : MonoBehaviour
{
    /// <summary>
    /// 原始行为资源
    /// </summary>
    public Sample_GraphAsset SampleAsset;
    /// <summary>
    /// 运行时使用的克隆行为资源
    /// </summary>
    private Sample_GraphAsset SampleAssetClone;
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
    public bool Logs = true;
    public string StartNodeType = "Module_Initialize";

    /// <summary>
    /// Sample 场景控制器，用于赋值给行为资源中的目标脚本对象
    /// </summary>
    public Sample_Module_Controller Sample_Controller;

    private void Start()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#endif
        // 为每一个行为注册运行时变量值改变回调
        RegisterVariableChangeWithAction();

        SampleAsset.Sample_Controller = Sample_Controller;
    }

    private void OnDisable()
    {
        // 注销每一个行为运行时变量值改变回调
        UnregisterVariableChangeWithAction();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            Action_Start();
        if (Input.GetKeyDown(KeyCode.K))
            Action_Kill();
        if (Input.GetKeyDown(KeyCode.P))
            Action_Pause();
        if (Input.GetKeyDown(KeyCode.R))
            Action_Resume();
    }

    #region 编辑器内运行时保存参数的监听事件
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
                util_Dashboard.LogMsg(util_Dashboard.MsgType.错误, $"无法创建克隆资源！", "", Logs);
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
    #endregion

    #region WorkFlow
    /// <summary>
    /// 行为开始
    /// </summary>
    public void Action_Start()
    {
        if (!Action_Validate())
        {
            util_Dashboard.LogMsg(util_Dashboard.MsgType.错误, $"行为流程启动失败！", "", Logs);
            return;
        }

        if (isRunning)
        {
            util_Dashboard.LogMsg(util_Dashboard.MsgType.错误, $"行为流程已经在执行中！", "", Logs);
            return;
        }

        StopAllCoroutines();
        StartCoroutine(Action_Flow());
    }

    /// <summary>
    /// 行为停止
    /// </summary>
    public void Action_Kill()
    {
        isRunning = false;
        // 强制解除暂停
        isPaused = false;

        // 注销每一个行为运行时变量值改变回调
        UnregisterVariableChangeWithAction();

        util_Dashboard.LogMsg(util_Dashboard.MsgType.信息, $"流程停止！", "", Logs);
    }

    /// <summary>
    /// 行为暂停
    /// </summary>
    public void Action_Pause()
    {
        if (isRunning)
        {
            isPaused = true;
            util_Dashboard.LogMsg(util_Dashboard.MsgType.信息, $"流程暂停！", "", Logs);
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
            util_Dashboard.LogMsg(util_Dashboard.MsgType.信息, $"流程继续！", "", Logs);
        }
    }

    /// <summary>
    /// 行为流程
    /// </summary>
    IEnumerator Action_Flow()
    {
        isRunning = true;
        util_Dashboard.LogMsg(util_Dashboard.MsgType.警告, $"开始执行流程：", SampleAsset.name, "00ff9d", Logs);

        var startNode = SampleAsset.Actions.Find(n => n.actionNodeType == StartNodeType);
        yield return Action_Execute(startNode);

        isRunning = false;
        util_Dashboard.LogMsg(util_Dashboard.MsgType.警告, $"流程执行完成：", SampleAsset.name, "00ff9d", Logs);
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
            util_Dashboard.LogMsg(util_Dashboard.MsgType.警告, $"执行暂停中：", $"{action.identifyName}  （{(action.isConcurrentExecution ? "并发" : "顺序")}）", Logs);

            // 每0.1秒检查一次，防止暂停时性能开销大
            yield return new WaitForSeconds(0.1f);
        }

        // 处理特殊节点类型
        if (action is ActionNode_Wait waitNode)
        {
            yield return HandlePausableWait(waitNode);
            yield break;
        }

        util_Dashboard.LogMsg(util_Dashboard.MsgType.信息, $"---> ：", $"{action.identifyName}  （{(action.isConcurrentExecution ? "并发" : "顺序")}）", Logs);
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
        util_Dashboard.LogMsg(util_Dashboard.MsgType.信息, $"---> ：", $"{waitNode.identifyName}  {waitNode.Time}s  （{(waitNode.isConcurrentExecution ? "并发" : "顺序")}）", Logs);

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

        if (current is ActionNode_Start start && start.childNodes != null)
            list.AddRange(start.childNodes.FindAll(n => n != null));
        else if (current is ActionNode_Wait wait)
            list.AddRange(wait.childNodes.FindAll(n => n != null));
        else if (current is ActionNode_Composite composite)
            list.AddRange(composite.childNodes.FindAll(n => n != null));
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
        if (SampleAsset == null || SampleAsset.Actions.Count == 0)
        {
            util_Dashboard.LogMsg(util_Dashboard.MsgType.错误, $"行为资源列表是空的！", "", Logs);
            return false;
        }

        var start = SampleAsset.Actions.Find(n => n.actionNodeType == StartNodeType);
        if (start == null)
        {
            util_Dashboard.LogMsg(util_Dashboard.MsgType.警告, $"未能找到  {StartNodeType}  类型节点！", "", Logs);
            return false;
        }

        return true;
    }

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

    #region 运行时克隆
    /// <summary>
    /// 创建运行时克隆
    /// </summary>
    private bool CreateRuntimeClone()
    {
        if (SampleAsset == null)
        {
            util_Dashboard.LogMsg(util_Dashboard.MsgType.错误, $"无法创建克隆资源！", "", Logs);
            return false;
        }

        SampleAssetClone = (Sample_GraphAsset)SampleAsset.Clone("", false);
        return true;
    }

    /// <summary>
    /// 恢复原始资源
    /// </summary>
    private void RestoreOriginalAsset()
    {
        if (SampleAssetClone != null)
        {
            SampleAsset.Replace(SampleAssetClone);
            util_Dashboard.LogMsg(util_Dashboard.MsgType.警告, $"已恢复原始行为资源！", "", Logs);
            SampleAssetClone = null;
        }
    }
    #endregion

    /// <summary>
    /// 设置节点流程资源
    /// </summary>
    /// <param name="asset"></param>
    public void SetActionAsset(ActionNode_Asset asset)
    {
        SampleAsset = asset as Sample_GraphAsset;
        SampleAsset.Sample_Controller = Sample_Controller;
    }
    #endregion
}