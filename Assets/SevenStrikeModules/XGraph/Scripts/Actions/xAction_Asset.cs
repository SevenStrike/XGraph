namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.VersionControl;
    using UnityEngine;

    public class xAction_Asset : ScriptableObject
    {
        #region 行为节点资源快照
        /// <summary>
        /// 行为节点资源快照
        /// </summary>
        public List<class_ActionAssetCaptureData> ActionAssetCaptureData = new List<class_ActionAssetCaptureData>();
        #endregion

        #region Graphview 基础参数
        /// <summary>
        /// 记录的节点编辑器最后一次的窗口尺寸
        /// </summary>
        [SerializeField] public Vector2Int LastGraphWindowSize;
        /// <summary>
        /// 记录的节点编辑器最后一次的视图内位置
        /// </summary>
        [SerializeField] public Vector2 LastGraphViewPosition = Vector2.zero;
        /// <summary>
        /// 记录的节点编辑器最后一次的视图内缩放
        /// </summary>
        [SerializeField] public float LastGraphViewZoom = 1;
        /// <summary>
        /// 最后一次保存时间
        /// </summary>
        [SerializeField] public string LastSaveDateTime = DateTime.Now.ToString("yyyy-MM-dd  -  HH:mm:ss");
        /// <summary>
        /// 专属节点配置表
        /// </summary>
        [SerializeField] public TextAsset GraphNodesStruct;
        #endregion

        #region 主题配置
        /// <summary>
        /// 节点编辑器的背景参数
        /// </summary>
        [SerializeField] public GraphviewGridBackgroundThemes GraphviewGridBackgroundThemes;
        /// <summary>
        /// 节点编辑器的选择框主题参数
        /// </summary>
        [SerializeField] public GraphviewRectangleSelectorThemes GraphviewRectangleSelectorThemes;
        #endregion

        #region 面板元素变换数据
        /// <summary>
        /// 记录的节点编辑器最后一次的黑板变量面板变换数据
        /// </summary>
        [SerializeField]
        public VisualElementTransformData Last_GraphView_BlackboardPanel_TransformData = new VisualElementTransformData()
        {
            top = 10,
            bottom = 0,
            left = 13,
            right = 0,
            anc_Top = true,
            anc_Bottom = false,
            anc_Left = true,
            anc_Right = false,
            size = new Vector2(250, 400)
        };
        /// <summary>
        /// 记录的节点编辑器最后一次的属性面板变换数据
        /// </summary>
        [SerializeField]
        public VisualElementTransformData Last_GraphView_InspectorPanel_TransformData = new VisualElementTransformData()
        {
            top = 10,
            bottom = 0,
            left = 0,
            right = 13,
            anc_Top = true,
            anc_Bottom = false,
            anc_Left = false,
            anc_Right = true,
            size = new Vector2(250, 400)
        };
        #endregion

        #region Graphview功能开关状态
        /// <summary>
        /// 记录的节点编辑器 Inspector 视图开关
        /// </summary>
        [SerializeField] public bool XGraph_InspectorViewDisplay;
        /// <summary>
        /// 记录的节点编辑器 BlackBoard 视图开关
        /// </summary>
        [SerializeField] public bool XGraph_BlackBoardViewDisplay;
        /// <summary>
        /// 记录的节点编辑器 节点颜色标记开关
        /// </summary>
        [SerializeField] public bool XGraph_DisplayNodeColor;
        /// <summary>
        /// 记录的节点编辑器 节点连线数据流效果开关
        /// </summary>
        [SerializeField] public bool XGraph_DisplayNodeFlow;
        /// <summary>
        /// 节点执行日志打印开关
        /// </summary>
        [SerializeField] public bool LogEnabled;
        #endregion

        #region 节点数据列表
        /// <summary>
        /// 行为节点列表
        /// </summary>
        [SerializeReference] public List<xAction_Base> Actions = new List<xAction_Base>();
        /// <summary>
        /// 变量节点列表
        /// </summary>
        [SerializeReference] public List<xVariableData> Variables = new List<xVariableData>();
        /// <summary>
        /// 便签列表
        /// </summary>
        [SerializeField] public List<xStickData> Sticks = new List<xStickData>();
        /// <summary>
        /// 标签列表
        /// </summary>
        [SerializeField] public List<xLabelData> Labels = new List<xLabelData>();
        /// <summary>
        /// 贴纸列表
        /// </summary>
        [SerializeField] public List<xDecalData> Decals = new List<xDecalData>();
        /// <summary>
        /// 编组列表
        /// </summary>
        [SerializeField] public List<xGroupData> Groups = new List<xGroupData>();
        /// <summary>
        /// 黑板变量列表
        /// </summary>
        [SerializeReference] public List<Variable> BlackboardVariable = new List<Variable>();
        #endregion

        #region 回调
        /// <summary>
        /// 黑板变量数值更新后的回调
        /// </summary>
        public Action On_VariablesValue_Changed;
        /// <summary>
        /// Graphview窗口尺寸变化的回调
        /// </summary>
        public Action<Vector2> On_GraphviewSize_Changed;
        /// <summary>
        /// Graphview视口位置化的回调
        /// </summary>
        public Action<Vector2> On_GraphviewPos_Changed;
        /// <summary>
        /// Graphview视口缩放化的回调
        /// </summary>
        public Action<float> On_GraphviewZoom_Changed;
        /// <summary>
        /// Graphview最后一次保存的回调
        /// </summary>
        public Action<string> On_GraphviewLastSave_Changed;
        #endregion

        #region 资源操作
        /// <summary>
        /// 创建数据节点到列表中
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public xAction_Base Create(NodeCreateArgs_Action args)
        {
            xAction_Base action = null;

            // 根据命名空间和类名动态创建特定类型
            if (!string.IsNullOrEmpty(args.prefixNamespace) &&
                !string.IsNullOrEmpty(args.prefixClass) &&
                !string.IsNullOrEmpty(args.actionNodeType))
            {
                string fullTypeName = $"{args.prefixNamespace}.{args.prefixClass}{args.actionNodeType}";

                try
                {
                    // 尝试通过反射创建特定类型
                    Type actionType = Type.GetType(fullTypeName);
                    if (actionType != null && typeof(xAction_Base).IsAssignableFrom(actionType))
                    {
                        action = Activator.CreateInstance(actionType) as xAction_Base;
                        action.BaseArgs = new class_ActionBaseArgs();
                        //Debug.Log($"成功创建特定类型: {fullTypeName}");
                    }
                    else
                    {
                        Debug.LogWarning($"无法创建类型 {fullTypeName}，将使用基类 xAction_Base");
                        action = new xAction_Base();
                        action.BaseArgs = new class_ActionBaseArgs();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"创建类型 {fullTypeName} 时出错: {ex.Message}");
                    action = new xAction_Base();
                    action.BaseArgs = new class_ActionBaseArgs();
                }
            }
            else
            {
                // 如果没有指定特定类型，使用基类
                action = new xAction_Base();
                action.BaseArgs = new class_ActionBaseArgs();
            }

#if UNITY_EDITOR
            action.BaseArgs.guid = UnityEditor.GUID.Generate().ToString();
#endif
            action.BaseArgs.actionNodeType = args.actionNodeType;
            action.BaseArgs.icon = args.iconName;
            action.BaseArgs.NodeIcon = args.nodeIcon;
            action.BaseArgs.visualNodeType = args.visualNodeType;
            action.identifyName = args.visualName;
            action.BaseArgs.namespaces = args.prefixNamespace;
            action.BaseArgs.classes = args.prefixClass;
            action.BaseArgs.HasAvatar = args.hasAvatar;
            action.BaseArgs.Avatar = args.avatar;
            action.BaseArgs.themeSolution = args.themeSolution;
            action.BaseArgs.themeColor = args.themeColor;
            action.BaseArgs.TransparentNode = args.transparentNode;
            action.BaseArgs.content = args.content;
            action.BaseArgs.nodeGraphSize = args.size;
            action.BaseArgs.isConcurrentExecution = args.isConcurrentExecution;

            // 设置行为数据的目标根资源为当前资源类
            action.SetActionAssetRoot(this);

            // 为变量类型节点数据特化处理，需要初始化类型 Variable
            if (action is xAction_Variable v)
            {
                v.variable = v.Initialized(args.visualName, args.variable.type);
                if (args.variable != null)
                {
                    switch (args.variable.type)
                    {
                        case xVariableType.String:
                            if (args.variable is Variable_String v_string)
                                v.variable.SetValue(v_string.GetValue<string>());
                            break;
                        case xVariableType.Float:
                            if (args.variable is Variable_Float v_float)
                                v.variable.SetValue(v_float.GetValue<float>());
                            break;
                        case xVariableType.Int:
                            if (args.variable is Variable_Int v_int)
                                v.variable.SetValue(v_int.GetValue<int>());
                            break;
                        case xVariableType.Bool:
                            if (args.variable is Variable_Bool v_bool)
                                v.variable.SetValue(v_bool.GetValue<bool>());
                            break;
                        case xVariableType.Vector2:
                            if (args.variable is Variable_Vector2 v_Vector2)
                                v.variable.SetValue(v_Vector2.GetValue<Vector2>());
                            break;
                        case xVariableType.Vector3:
                            if (args.variable is Variable_Vector3 v_Vector3)
                                v.variable.SetValue(v_Vector3.GetValue<Vector3>());
                            break;
                        case xVariableType.Vector4:
                            if (args.variable is Variable_Vector4 v_Vector4)
                                v.variable.SetValue(v_Vector4.GetValue<Vector4>());
                            break;
                        case xVariableType.Color:
                            if (args.variable is Variable_Color v_Color)
                                v.variable.SetValue(v_Color.GetValue<Color>());
                            break;
                    }
                }
            }

            // 如果创建的是属性节点，那么需要为属性节点初始化属性变量项，为后期其他节点获取数值做好准备工作
            if (action is xAction_Property p)
            {
                p.Propertys_Initialize();
            }

            // 添加到列表中
            Actions.Add(action);

            return action;
        }
        /// <summary>
        /// 从列表中移除一个数据节点
        /// </summary>
        /// <param name="node"></param>
        public void Remove(xAction_Base node)
        {
            if (node == null) return;
            Actions.Remove(node);
        }
        /// <summary>
        /// 从列表中清空所有数据节点
        /// </summary>
        public void ClearDatas()
        {
            // 清空资源列表
            Actions.Clear();
            // 清空便签列表
            StickNote_Clear();
            // 清空标签
            Label_Clear();
            // 清空贴图列表
            Decal_Clear();
            // 清空变量列表
            Variable_Clear();
            // 清空编组列表
            NodeGroup_Clear();
        }
        /// <summary>
        /// 使用目标资源替换当前资源
        /// </summary>
        /// <param name="target"></param>
        public void Replace(xAction_Asset target)
        {
            if (target == null) return;

            // 清空当前原始资源的所有子节点
            ClearDatas();

            // 更新所有使用到的变量值（target 目标资源 - 更新）
            target.Variables_Refresh();

            #region Graphs
            // 对象名称
            name = target.name;
            // 最后一次的窗口尺寸
            LastGraphWindowSize = target.LastGraphWindowSize;
            // 最后一次的视口位置
            LastGraphViewPosition = target.LastGraphViewPosition;
            // 最后一次的视口缩放
            LastGraphViewZoom = target.LastGraphViewZoom;
            // 最后一次保存日期与时间
            LastSaveDateTime = target.LastSaveDateTime = DateTime.Now.ToString("yyyy-MM-dd  -  HH:mm:ss");
            // 节点执行日志打印开关
            LogEnabled = target.LogEnabled;
            // 背景主题配置参数（脱离引用克隆）
            GraphviewGridBackgroundThemes = target.GraphviewGridBackgroundThemes.Clone();
            // 选择框主题配置参数（脱离引用克隆）
            GraphviewRectangleSelectorThemes = target.GraphviewRectangleSelectorThemes.Clone();
            // 浮动面板数据 - 黑板（脱离引用克隆）
            Last_GraphView_BlackboardPanel_TransformData = target.Last_GraphView_BlackboardPanel_TransformData.Clone();
            // 浮动面板数据 - 属性（脱离引用克隆）
            Last_GraphView_InspectorPanel_TransformData = target.Last_GraphView_InspectorPanel_TransformData.Clone();
            // 开关参数 - 黑板
            XGraph_BlackBoardViewDisplay = target.XGraph_BlackBoardViewDisplay;
            // 开关参数 - 属性
            XGraph_InspectorViewDisplay = target.XGraph_InspectorViewDisplay;
            // 开关参数 - 节点颜色标记
            XGraph_DisplayNodeColor = target.XGraph_DisplayNodeColor;
            // 开关参数 - 节点数据流
            XGraph_DisplayNodeFlow = target.XGraph_DisplayNodeFlow;
            // 自定义扩展节点列表配置文件
            GraphNodesStruct = target.GraphNodesStruct;

            ActionAssetCaptureData = new List<class_ActionAssetCaptureData>();
            ActionAssetCaptureData.Clear();
            foreach (var data in target.ActionAssetCaptureData)
            {
                ActionAssetCaptureData.Add(data.Clone());
            }
            #endregion

            #region Nodes
            // 用 targetAsset 中的 Actions 覆盖当前 Actions 数据列表
            Actions = new List<xAction_Base>();
            // 先克隆所有节点
            Dictionary<string, xAction_Base> guidToNodeMap = new Dictionary<string, xAction_Base>();
            foreach (var action in target.Actions)
            {
                // 脱离引用克隆
                xAction_Base clone = action.Clone();
                // 设置根资源
                clone.SetActionAssetRoot(this);

                // 存储GUID映射
                guidToNodeMap[action.BaseArgs.guid] = clone;

                Actions.Add(clone);
            }
            // 重建子节点关系
            foreach (var a in target.Actions)
            {
                var clone = guidToNodeMap[a.BaseArgs.guid];

                if (a is xAction_Start s && s.childNodes != null && s.childNodes.Count > 0)
                {
                    var cloneStart = clone as xAction_Start;
                    if (cloneStart != null)
                    {
                        cloneStart.childNodes = new List<string>();
                        foreach (var child in s.childNodes)
                        {
                            if (guidToNodeMap.TryGetValue(child, out var childClone))
                            {
                                cloneStart.childNodes.Add(childClone.BaseArgs.guid);
                                // 设置父节点关系
                                childClone.SetParentNode(cloneStart.BaseArgs.guid);
                            }
                        }
                    }
                }
                if (a is xAction_Wait w && w.childNodes != null && w.childNodes.Count > 0)
                {
                    var cloneWait = clone as xAction_Wait;
                    if (cloneWait != null)
                    {
                        cloneWait.childNodes = new List<string>();
                        foreach (var child in w.childNodes)
                        {
                            if (guidToNodeMap.TryGetValue(child, out var childClone))
                            {
                                cloneWait.childNodes.Add(childClone.BaseArgs.guid);
                                // 设置父节点关系
                                childClone.SetParentNode(cloneWait.BaseArgs.guid);
                            }
                        }
                    }
                }
                if (a is xAction_Composite c && c.childNodes != null && c.childNodes.Count > 0)
                {
                    var cloneComposite = clone as xAction_Composite;
                    if (cloneComposite != null)
                    {
                        cloneComposite.childNodes = new List<string>();
                        foreach (var child in c.childNodes)
                        {
                            if (guidToNodeMap.TryGetValue(child, out var childClone))
                            {
                                cloneComposite.childNodes.Add(childClone.BaseArgs.guid);
                                // 设置父节点关系
                                childClone.SetParentNode(cloneComposite.BaseArgs.guid);
                            }
                        }
                    }
                }
                if (a is xAction_Debug d && d.childNodes != null && d.childNodes.Count > 0)
                {
                    var cloneDebug = clone as xAction_Debug;
                    if (cloneDebug != null)
                    {
                        cloneDebug.childNodes = new List<string>();
                        foreach (var child in d.childNodes)
                        {
                            if (guidToNodeMap.TryGetValue(child, out var childClone))
                            {
                                cloneDebug.childNodes.Add(childClone.BaseArgs.guid);
                                // 设置父节点关系
                                childClone.SetParentNode(cloneDebug.BaseArgs.guid);
                            }
                        }
                    }
                }
                if (a is xAction_Relay r && r.childNodes != null && r.childNodes.Count > 0)
                {
                    var cloneRelay = clone as xAction_Relay;
                    if (cloneRelay != null)
                    {
                        cloneRelay.childNodes = new List<string>();
                        foreach (var child in r.childNodes)
                        {
                            if (guidToNodeMap.TryGetValue(child, out var childClone))
                            {
                                cloneRelay.childNodes.Add(childClone.BaseArgs.guid);
                                // 设置父节点关系
                                childClone.SetParentNode(cloneRelay.BaseArgs.guid);
                            }
                        }
                    }
                }
                if (a is xAction_Branch b)
                {
                    var cloneBranch = clone as xAction_Branch;
                    if (cloneBranch != null)
                    {
                        if (b.childNode_true != null && guidToNodeMap.TryGetValue(b.childNode_true, out var trueChildClone))
                        {
                            cloneBranch.childNode_true = trueChildClone.BaseArgs.guid;
                            // 设置父节点关系
                            trueChildClone.SetParentNode(cloneBranch.BaseArgs.guid);
                        }

                        if (b.childNode_false != null && guidToNodeMap.TryGetValue(b.childNode_false, out var falseChildClone))
                        {
                            cloneBranch.childNode_false = falseChildClone.BaseArgs.guid;
                            // 设置父节点关系
                            falseChildClone.SetParentNode(cloneBranch.BaseArgs.guid);
                        }
                    }
                }
            }
            // 用 targetAsset 中的 Decals 覆盖当前 Decals 数据列表
            Decals = new List<xDecalData>();
            foreach (var decal in target.Decals)
            {
                Decals.Add(decal.Clone(false));
            }
            // 用 targetAsset 中的 Sticks 覆盖当前 Sticks 数据列表
            Sticks = new List<xStickData>();
            foreach (var stick in target.Sticks)
            {
                Sticks.Add(stick.Clone(false));
            }
            // 用 targetAsset 中的 Labels 覆盖当前 Labels 数据列表
            Labels = new List<xLabelData>();
            foreach (var label in target.Labels)
            {
                Labels.Add(label.Clone(false));
            }
#if UNITY_EDITOR
            // 用 targetAsset 中的 Groups 覆盖当前 Groups 数据列表
            Groups = new List<xGroupData>();
            foreach (var group in target.Groups)
            {
                Groups.Add(group.Clone(false));
            }
#endif
            // 用 targetAsset 中的 Variables 覆盖当前 Variables 数据列表
            Variables = new List<xVariableData>();
            foreach (var vare in target.Variables)
            {
                Variables.Add(vare.Clone(false));
            }
            // 用 targetAsset 中的 ActBlackboardVariable 覆盖当前 BlackboardVariable 数据列表
            BlackboardVariable = new List<Variable>();
            foreach (var vare in target.BlackboardVariable)
            {
                BlackboardVariable.Add(vare.Clone(false));
            }
            #endregion

            GUIUtility.systemCopyBuffer = JsonUtility.ToJson(target);
        }
        /// <summary>
        /// 创建当前流程设计的克隆体（仅编辑器下）
        /// </summary>
        /// <returns></returns>
        public xAction_Asset Clone()
        {
            // 创建新的 ActionNode_Asset
            xAction_Asset asset = CreateInstance<xAction_Asset>();

            #region Graphs
            // 对象名称
            asset.name = name;
            // 最后一次的窗口尺寸
            asset.LastGraphWindowSize = LastGraphWindowSize;
            // 最后一次的视口位置
            asset.LastGraphViewPosition = LastGraphViewPosition;
            // 最后一次的视口缩放
            asset.LastGraphViewZoom = LastGraphViewZoom;
            // 最后一次保存日期与时间
            asset.LastSaveDateTime = LastSaveDateTime;
            // 节点执行日志打印开关
            asset.LogEnabled = LogEnabled;
            // 开关参数 - 黑板
            asset.XGraph_BlackBoardViewDisplay = XGraph_BlackBoardViewDisplay;
            // 开关参数 - 属性
            asset.XGraph_InspectorViewDisplay = XGraph_InspectorViewDisplay;
            // 开关参数 - 节点颜色标记
            asset.XGraph_DisplayNodeColor = XGraph_DisplayNodeColor;
            // 开关参数 - 节点数据流
            asset.XGraph_DisplayNodeFlow = XGraph_DisplayNodeFlow;
            // 自定义扩展节点列表配置文件
            asset.GraphNodesStruct = GraphNodesStruct;
            // 浮动面板数据 - 黑板（脱离引用克隆）
            asset.Last_GraphView_BlackboardPanel_TransformData = Last_GraphView_BlackboardPanel_TransformData.Clone();
            // 浮动面板数据 - 属性（脱离引用克隆）
            asset.Last_GraphView_InspectorPanel_TransformData = Last_GraphView_InspectorPanel_TransformData.Clone();
            // 背景主题配置参数（脱离引用克隆）
            asset.GraphviewGridBackgroundThemes = GraphviewGridBackgroundThemes.Clone();
            // 选择框主题配置参数（脱离引用克隆）
            asset.GraphviewRectangleSelectorThemes = GraphviewRectangleSelectorThemes.Clone();

            asset.ActionAssetCaptureData = new List<class_ActionAssetCaptureData>();
            foreach (var data in ActionAssetCaptureData)
            {
                asset.ActionAssetCaptureData.Add(data.Clone());
            }
            #endregion

            #region Nodes
            // 将当前的 Actions 列表克隆到 asset.Actions
            asset.Actions = new List<xAction_Base>();
            // 先克隆所有节点，建立GUID映射
            Dictionary<string, xAction_Base> guidToNodeMap = new Dictionary<string, xAction_Base>();
            foreach (var a in Actions)
            {
                // 脱离引用克隆
                xAction_Base clone = a.Clone();
                // 设置根资源
                clone.SetActionAssetRoot(asset);

                // 存储GUID映射
                guidToNodeMap[a.BaseArgs.guid] = clone;

                asset.Actions.Add(clone);
            }

            // 重建子节点关系
            foreach (var a in Actions)
            {
                var clone = guidToNodeMap[a.BaseArgs.guid];

                if (a is xAction_Start s && s.childNodes != null && s.childNodes.Count > 0)
                {
                    var cloneStart = clone as xAction_Start;
                    if (cloneStart != null)
                    {
                        cloneStart.childNodes = new List<string>();
                        foreach (var child in s.childNodes)
                        {
                            if (guidToNodeMap.TryGetValue(child, out var childClone))
                            {
                                cloneStart.childNodes.Add(childClone.BaseArgs.guid);
                                // 设置父节点关系
                                childClone.SetParentNode(cloneStart.BaseArgs.guid);
                            }
                        }
                    }
                }
                if (a is xAction_Wait w && w.childNodes != null && w.childNodes.Count > 0)
                {
                    var cloneWait = clone as xAction_Wait;
                    if (cloneWait != null)
                    {
                        cloneWait.childNodes = new List<string>();
                        foreach (var child in w.childNodes)
                        {
                            if (guidToNodeMap.TryGetValue(child, out var childClone))
                            {
                                cloneWait.childNodes.Add(childClone.BaseArgs.guid);
                                // 设置父节点关系
                                childClone.SetParentNode(cloneWait.BaseArgs.guid);
                            }
                        }
                    }
                }
                if (a is xAction_Composite c && c.childNodes != null && c.childNodes.Count > 0)
                {
                    var cloneComposite = clone as xAction_Composite;
                    if (cloneComposite != null)
                    {
                        cloneComposite.childNodes = new List<string>();
                        foreach (var child in c.childNodes)
                        {
                            if (guidToNodeMap.TryGetValue(child, out var childClone))
                            {
                                cloneComposite.childNodes.Add(childClone.BaseArgs.guid);
                                // 设置父节点关系
                                childClone.SetParentNode(cloneComposite.BaseArgs.guid);
                            }
                        }
                    }
                }
                if (a is xAction_Debug d && d.childNodes != null && d.childNodes.Count > 0)
                {
                    var cloneDebug = clone as xAction_Debug;
                    if (cloneDebug != null)
                    {
                        cloneDebug.childNodes = new List<string>();
                        foreach (var child in d.childNodes)
                        {
                            if (guidToNodeMap.TryGetValue(child, out var childClone))
                            {
                                cloneDebug.childNodes.Add(childClone.BaseArgs.guid);
                                // 设置父节点关系
                                childClone.SetParentNode(cloneDebug.BaseArgs.guid);
                            }
                        }
                    }
                }
                if (a is xAction_Relay r && r.childNodes != null && r.childNodes.Count > 0)
                {
                    var cloneRelay = clone as xAction_Relay;
                    if (cloneRelay != null)
                    {
                        cloneRelay.childNodes = new List<string>();
                        foreach (var child in r.childNodes)
                        {
                            if (guidToNodeMap.TryGetValue(child, out var childClone))
                            {
                                cloneRelay.childNodes.Add(childClone.BaseArgs.guid);
                                // 设置父节点关系
                                childClone.SetParentNode(cloneRelay.BaseArgs.guid);
                            }
                        }
                    }
                }
                if (a is xAction_Branch b)
                {
                    var cloneBranch = clone as xAction_Branch;
                    if (cloneBranch != null)
                    {
                        if (b.childNode_true != null && guidToNodeMap.TryGetValue(b.childNode_true, out var trueChildClone))
                        {
                            cloneBranch.childNode_true = trueChildClone.BaseArgs.guid;
                            // 设置父节点关系
                            trueChildClone.SetParentNode(cloneBranch.BaseArgs.guid);
                        }

                        if (b.childNode_false != null && guidToNodeMap.TryGetValue(b.childNode_false, out var falseChildClone))
                        {
                            cloneBranch.childNode_false = falseChildClone.BaseArgs.guid;
                            // 设置父节点关系
                            falseChildClone.SetParentNode(cloneBranch.BaseArgs.guid);
                        }
                    }
                }
            }

            // 将当前的 Sticks 列表克隆到 asset.Sticks
            asset.Sticks = new List<xStickData>();
            foreach (var s in Sticks)
            {
                asset.Sticks.Add(s.Clone(false));
            }

            // 将当前的 Labels 列表克隆到 asset.Labels
            asset.Labels = new List<xLabelData>();
            foreach (var l in Labels)
            {
                asset.Labels.Add(l.Clone(false));
            }

            // 将当前的 Decals 列表克隆到 asset.Decals
            asset.Decals = new List<xDecalData>();
            foreach (var d in Decals)
            {
                asset.Decals.Add(d.Clone(false));
            }

            // 将当前的 Variables 列表克隆到 asset.Variables
            asset.Variables = new List<xVariableData>();
            foreach (var v in Variables)
            {
                asset.Variables.Add(v.Clone(false));
            }

            // 将当前的 Groups 列表克隆到 asset.Groups
            asset.Groups = new List<xGroupData>();
            foreach (var g in Groups)
            {
#if UNITY_EDITOR
                asset.Groups.Add(g.Clone(false));
#endif
            }

            // 将当前的 BlackboardVariable 列表克隆到 asset.BlackboardVariable
            asset.BlackboardVariable = new List<Variable>();
            foreach (var v in BlackboardVariable)
            {
                asset.BlackboardVariable.Add(v.Clone(false));
            }
            #endregion

            return asset;
        }
        /// <summary>
        /// 保存为Tree资源到目标路径下
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        public void SaveNodeRootAsset(xAction_Asset root, string path)
        {
#if UNITY_EDITOR
            //// 提取根路径整理 - 去掉尾部的 /
            //string path_root = util_Dashboard.GetPath_Root();
            //path_root = path_root.Substring(0, path_root.Length - 1);

            //// 目标路径整理 - 去掉尾部的 /
            //string path_target = $"{util_Dashboard.GetPath_Temp()}";
            //path_target = path_target.Substring(0, path_target.Length - 1);

            //// 判断是否存在目标路径，如果不存在就创建该路径的文件夹
            //if (!UnityEditor.AssetDatabase.AssetPathExists(path_target))
            //{
            //    UnityEditor.AssetDatabase.CreateFolder(path_root, "Temp");
            //    //AssetDatabase.SaveAssets();
            //    UnityEditor.AssetDatabase.Refresh();
            //}

            //// 保存为临时.asset 文件，供Unity资源系统进行操作
            //UnityEditor.AssetDatabase.CreateAsset(root, path);
            //foreach (var treenode in root.Actions)
            //{
            //UnityEditor.AssetDatabase.AddObjectToAsset(treenode, root);
            //}

            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 获取指定数据节点的子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<string> GetChildrenNodes(xAction_Base parent)
        {
            List<string> nodes = new List<string>();

            // 如果是 "ActionNode_Start" 节点，那么就收集 "ActionNode_Start" 节点下的 "childNodes"
            xAction_Start start = parent as xAction_Start;
            if (start != null && start.childNodes != null)
            {
                nodes = start.childNodes;
            }

            // 如果是 "ActionNode_Wait" 节点，那么就收集 "ActionNode_Wait" 节点下的 "childNodes"
            xAction_Wait wait = parent as xAction_Wait;
            if (wait != null && wait.childNodes != null)
            {
                nodes = wait.childNodes;
            }

            // 如果是 "ActionNode_Composite" 节点，那么就收集 "ActionNode_Composite" 节点下的 "childNodes"
            xAction_Composite comp = parent as xAction_Composite;
            if (comp != null && comp.childNodes != null)
            {
                nodes = comp.childNodes;
            }

            // 如果是 "ActionNode_Debug" 节点，那么就收集 "ActionNode_Debug" 节点下的 "childNodes"
            xAction_Debug debug = parent as xAction_Debug;
            if (debug != null && debug.childNodes != null)
            {
                nodes = debug.childNodes;
            }

            // 如果是 "ActionNode_Branch" 节点，那么就收集 "ActionNode_Branch" 节点下的 "childNodes"
            xAction_Branch branch = parent as xAction_Branch;
            if (branch != null)
            {
                if (branch.childNode_true != null)
                    nodes.Add(branch.childNode_true);
                if (branch.childNode_false != null)
                    nodes.Add(branch.childNode_false);
            }

            // 如果是 "ActionNode_Relay" 节点，那么就收集 "ActionNode_Relay" 节点下的 "childNodes"
            xAction_Relay relay = parent as xAction_Relay;
            if (relay != null && relay.childNodes != null)
            {
                nodes = relay.childNodes;
            }

            // 返回的列表就是 "GraphView组件" 那边需要根据这子资源列表才能知道跟哪些子节点重建节点之间的连线
            return nodes;
        }
        /// <summary>
        /// 为资源指定子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void ChildNode_Add(xAction_Base parent, xAction_Base child)
        {
            //Debug.Log($"{parent.identifyName}       |  建立链接  √  |      {child.identifyName}");

            // 设置父节点关系
            child.SetParentNode(parent.BaseArgs.guid);

            #region 特化处理 - Start
            if (parent is xAction_Start s)
            {
                bool exist = false;
                foreach (var item in s.childNodes)
                {
                    if (child.BaseArgs.guid == item)
                        exist = true;
                }
                if (exist)
                {
                    Debug.Log("start 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                s.childNodes.Add(child.BaseArgs.guid);
            }
            #endregion

            #region 特化处理 - Wait
            if (parent is xAction_Wait w)
            {
                bool exist = false;
                foreach (var item in w.childNodes)
                {
                    if (child.BaseArgs.guid == item)
                        exist = true;
                }

                if (exist)
                {
                    Debug.Log("wait 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                w.childNodes.Add(child.BaseArgs.guid);
            }
            #endregion

            #region 特化处理 - Composite
            if (parent is xAction_Composite c)
            {
                bool exist = false;
                foreach (var item in c.childNodes)
                {
                    if (child.BaseArgs.guid == item)
                        exist = true;
                }
                if (exist)
                {
                    Debug.Log("comp 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                c.childNodes.Add(child.BaseArgs.guid);
            }
            #endregion

            #region 特化处理 - Debug
            if (parent is xAction_Debug d)
            {
                bool exist = false;
                foreach (var item in d.childNodes)
                {
                    if (child.BaseArgs.guid == item)
                        exist = true;
                }
                if (exist)
                {
                    Debug.Log("comp 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                d.childNodes.Add(child.BaseArgs.guid);
            }
            #endregion

            #region 特化处理 - Relay
            if (parent is xAction_Relay r)
            {
                bool exist = false;
                foreach (var item in r.childNodes)
                {
                    if (child.BaseArgs.guid == item)
                        exist = true;
                }
                if (exist)
                {
                    //Debug.Log("comp 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                r.childNodes.Add(child.BaseArgs.guid);
            }
            #endregion
        }
        /// <summary>
        /// 为资源指定子资源（分支节点专用）
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void ChildNode_Add(xAction_Base parent, xAction_Base child, string childmode)
        {
            //Debug.Log($"{parent.identifyName}       |  建立链接  √  |      {child.identifyName}");

            // 设置父节点关系
            child.SetParentNode(parent.BaseArgs.guid);

            #region 特化处理 - Branch
            if (parent is xAction_Branch b)
            {
                if (childmode == "开")
                {
                    if (b.childNode_true != null)
                    {
                        if (child.BaseArgs.guid == b.childNode_true)
                        {
                            Debug.Log("branch 节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                            return;
                        }
                    }
                    else
                    {
                        b.childNode_true = child.BaseArgs.guid;
                    }
                }
                else if (childmode == "关")
                {
                    if (b.childNode_false != null)
                    {
                        if (child.BaseArgs.guid == b.childNode_false)
                        {
                            Debug.Log("branch 节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                            return;
                        }
                    }
                    else
                    {
                        b.childNode_false = child.BaseArgs.guid;
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// 从指定的父资源中移除子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void ChildNode_Remove(xAction_Base parent, xAction_Base child)
        {
            //Debug.Log($"{parent.identifyName}       |  断开链接  ×  |      {c.identifyName}");

            #region 特化处理 - Start
            if (parent is xAction_Start s)
            {
                s.childNodes.Remove(child.BaseArgs.guid);
            }
            #endregion

            #region 特化处理 - Wait
            if (parent is xAction_Wait w)
            {
                w.childNodes.Remove(child.BaseArgs.guid);
            }
            #endregion

            #region 特化处理 - Composite
            if (parent is xAction_Composite c)
            {
                c.childNodes.Remove(child.BaseArgs.guid);
            }
            #endregion

            #region 特化处理 - Debug
            if (parent is xAction_Debug d)
            {
                d.childNodes.Remove(child.BaseArgs.guid);
            }
            #endregion

            #region 特化处理 - Relay
            if (parent is xAction_Relay r)
            {
                r.childNodes.Remove(child.BaseArgs.guid);
            }
            #endregion

            // 清理父节点关系
            child.SetParentNode(null);
        }
        /// <summary>
        /// 从指定的父资源中移除子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void ChildNode_Remove(xAction_Base parent, xAction_Base child, string state)
        {
            //Debug.Log($"{parent.identifyName}       |  断开链接  ×  |      {child.identifyName}");

            #region 特化处理 - Branch
            if (parent is xAction_Branch b)
            {
                if (state == "开")
                    b.childNode_true = null;
                else if (state == "关")
                    b.childNode_false = null;
            }
            #endregion

            // 清理父节点关系
            child.SetParentNode(null);
        }
        /// <summary>
        /// 寻找匹配guid的行为数据节点
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public xAction_Base FindActionNode(string guid)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i].BaseArgs.guid == guid)
                {
                    return Actions[i];
                }
            }

            return null;
        }
        /// <summary>
        /// 设置逻辑起始节点
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        public void SetStartNode(xAction_Base action)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                if (action.BaseArgs.guid == Actions[i].BaseArgs.guid)
                    Actions[i].BaseArgs.isStartNode = true;
                else
                    Actions[i].BaseArgs.isStartNode = false;

                if (Actions[i].On_Node_IsStartNode != null)
                    Actions[i].On_Node_IsStartNode(Actions[i].BaseArgs.isStartNode);
            }
        }
        #endregion

        #region 便签操作
        /// <summary>
        /// 添加便签数据
        /// </summary>
        /// <param name="data"></param>
        public void StickNote_Add(xStickData data)
        {
            Sticks.Add(data);
        }
        /// <summary>
        /// 清空便签数据列表
        /// </summary>
        public void StickNote_Clear()
        {
            Sticks.Clear();
#if UNITY_EDITOR
            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标便签数据
        /// </summary>
        /// <param name="data"></param>
        public void StickNote_Remove(xStickData data)
        {
            Sticks.Remove(data);
        }
        #endregion

        #region 标签操作
        /// <summary>
        /// 添加标签数据
        /// </summary>
        /// <param name="data"></param>
        public void Label_Add(xLabelData data)
        {
            Labels.Add(data);
        }
        /// <summary>
        /// 清空标签数据列表
        /// </summary>
        public void Label_Clear()
        {
            Labels.Clear();
#if UNITY_EDITOR
            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标标签数据
        /// </summary>
        /// <param name="data"></param>
        public void Label_Remove(xLabelData data)
        {
            Labels.Remove(data);
        }
        #endregion

        #region 贴图操作
        /// <summary>
        /// 添加贴图数据
        /// </summary>
        /// <param name="data"></param>
        public void Decal_Add(xDecalData data)
        {
            Decals.Add(data);
        }
        /// <summary>
        /// 清空贴图数据列表
        /// </summary>
        public void Decal_Clear()
        {
            Decals.Clear();
#if UNITY_EDITOR
            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标贴图数据
        /// </summary>
        /// <param name="data"></param>
        public void Decal_Remove(xDecalData data)
        {
            Decals.Remove(data);
        }
        #endregion

        #region 变量操作
        /// <summary>
        /// 添加变量数据
        /// </summary>
        /// <param name="data"></param>
        public void Variable_Add(xVariableData data)
        {
            Variables.Add(data);
        }
        /// <summary>
        /// 清空变量数据列表
        /// </summary>
        public void Variable_Clear()
        {
            Variables.Clear();
#if UNITY_EDITOR
            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标变量数据
        /// </summary>
        /// <param name="data"></param>
        public void Variable_Remove(xVariableData data)
        {
            Variables.Remove(data);
        }
        /// <summary>
        /// 获取目标变量源头数据
        /// </summary>
        /// <param name="data"></param>
        public Variable Variable_GetVarSource(string varguid)
        {
            Variable vare = null;
            foreach (Variable n in BlackboardVariable)
            {
                if (varguid == n.guid)
                {
                    vare = n;
                }
            }

            return vare;
        }
        /// <summary>
        /// 设置变量值
        /// </summary>
        public void Variable_SetValue<T>(string name, T value)
        {
            foreach (var item in BlackboardVariable)
            {
                if (name == item.name)
                {
                    item.SetValue(value);

                    // 更新变量赋值数据
                    Variables_Refresh();
                }
            }
        }
        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Variable_GetValue<T>(string name)
        {
            foreach (var item in BlackboardVariable)
            {
                if (name == item.name)
                {
                    return item.GetValue<T>();
                }
            }

            return default(T);
        }
        /// <summary>
        /// 根据VraiableCategory的变量列表源来更新在Actions列表 & Variables列表中所有用到这些变量的值
        /// </summary>
        public void Variables_Refresh()
        {
            // 更新 “黑板变量数据列表”的变量信息
            foreach (var vare in Variables)
            {
                foreach (var bb_variable in BlackboardVariable)
                {
                    // 匹配黑板变量
                    if (vare.guid_v == bb_variable.guid)
                    {
                        vare.variable.name = bb_variable.name;
                        vare.description = bb_variable.description;
                        vare.variable.description = bb_variable.description;
                        switch (vare.variable.type)
                        {
                            case xVariableType.String:
                                vare.variable.SetValue(bb_variable.GetValue<string>());
                                break;
                            case xVariableType.Float:
                                vare.variable.SetValue(bb_variable.GetValue<float>());
                                break;
                            case xVariableType.Int:
                                vare.variable.SetValue(bb_variable.GetValue<int>());
                                break;
                            case xVariableType.Bool:
                                vare.variable.SetValue(bb_variable.GetValue<bool>());
                                break;
                            case xVariableType.Vector2:
                                vare.variable.SetValue(bb_variable.GetValue<Vector2>());
                                break;
                            case xVariableType.Vector3:
                                vare.variable.SetValue(bb_variable.GetValue<Vector3>());
                                break;
                            case xVariableType.Vector4:
                                vare.variable.SetValue(bb_variable.GetValue<Vector4>());
                                break;
                            case xVariableType.Color:
                                vare.variable.SetValue(bb_variable.GetValue<Color>());
                                break;
                        }
                    }
                }
            }

            // 更新行为节点中的 “黑板变量数据列表” 的变量信息
            foreach (var action in Actions)
            {
                foreach (var data in action.BaseArgs.VariableDatas)
                {
                    foreach (var variable in BlackboardVariable)
                    {
                        // 匹配黑板变量
                        if (data.variable.guid == variable.guid)
                        {
                            data.variable.name = variable.name;
                            data.variable.description = variable.description;
                            switch (data.variable.type)
                            {
                                case xVariableType.String:
                                    data.variable.SetValue<string>(variable.GetValue<string>());
                                    break;
                                case xVariableType.Float:
                                    data.variable.SetValue<float>(variable.GetValue<float>());
                                    break;
                                case xVariableType.Int:
                                    data.variable.SetValue<int>(variable.GetValue<int>());
                                    break;
                                case xVariableType.Bool:
                                    data.variable.SetValue<bool>(variable.GetValue<bool>());
                                    break;
                                case xVariableType.Vector2:
                                    data.variable.SetValue<Vector2>(variable.GetValue<Vector2>());
                                    break;
                                case xVariableType.Vector3:
                                    data.variable.SetValue<Vector3>(variable.GetValue<Vector3>());
                                    break;
                                case xVariableType.Vector4:
                                    data.variable.SetValue<Vector4>(variable.GetValue<Vector4>());
                                    break;
                                case xVariableType.Color:
                                    data.variable.SetValue<Color>(variable.GetValue<Color>());
                                    break;
                            }

                            if (action is xAction_Variable vare)
                            {
                                string originalGUID = vare.variable.guid;
                                vare.variable = data.variable.Clone(false);
                                vare.variable.guid = originalGUID;
                                vare.variable.name = action.identifyName;
                            }
                        }
                    }
                }
            }

            // 更新行为节点中的 “内部变量数据列表” 的变量信息
            foreach (var action in Actions)
            {
                // 刷新内部变量数据数值
                foreach (var data in action.BaseArgs.InternalVariableDatas)
                {
                    xAction_Variable internalVar = FindActionNode(data.VariableNodeGuid) as xAction_Variable;
                    internalVar.variable.name = internalVar.identifyName;
                    data.variable.name = internalVar.identifyName;
                    switch (data.variable.type)
                    {
                        case xVariableType.String:
                            data.variable.SetValue(internalVar.variable.GetValue<string>());
                            break;
                        case xVariableType.Float:
                            data.variable.SetValue(internalVar.variable.GetValue<float>());
                            break;
                        case xVariableType.Int:
                            data.variable.SetValue(internalVar.variable.GetValue<int>());
                            break;
                        case xVariableType.Bool:
                            data.variable.SetValue(internalVar.variable.GetValue<bool>());
                            break;
                        case xVariableType.Vector2:
                            data.variable.SetValue(internalVar.variable.GetValue<Vector2>());
                            break;
                        case xVariableType.Vector3:
                            data.variable.SetValue(internalVar.variable.GetValue<Vector3>());
                            break;
                        case xVariableType.Vector4:
                            data.variable.SetValue(internalVar.variable.GetValue<Vector4>());
                            break;
                        case xVariableType.Color:
                            data.variable.SetValue(internalVar.variable.GetValue<Color>());
                            break;
                    }
                }
            }

            // 数值更新后的回调
            if (On_VariablesValue_Changed != null)
                On_VariablesValue_Changed();
        }
        #endregion

        #region 编组操作
        /// <summary>
        /// 添加编组数据
        /// </summary>
        /// <param name="data"></param>
        public void NodeGroup_Add(xGroupData data)
        {
            Groups.Add(data);
        }
        /// <summary>
        /// 清空编组数据列表
        /// </summary>
        public void NodeGroup_Clear()
        {
            BlackboardVariable.Clear();
#if UNITY_EDITOR
            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标编组数据
        /// </summary>
        /// <param name="data"></param>
        public void NodeGroup_Remove(xGroupData data)
        {
            Groups.Remove(data);
        }
        #endregion
    }
}