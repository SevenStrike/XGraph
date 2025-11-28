namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewNodeGraph", menuName = "XGraph/GraphAssets/Default")]
    public class xAction_Asset : ScriptableObject
    {
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
        #endregion

        #region 节点数据列表
        /// <summary>
        /// 行为节点列表
        /// </summary>
        [SerializeReference] public List<xAction_Base> Actions = new List<xAction_Base>();
        /// <summary>
        /// 变量节点列表
        /// </summary>
        [SerializeField] public List<xVariableData> Variables = new List<xVariableData>();
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
#if UNITY_EDITOR
        public xAction_Base Create(NodeCreateArgs_Action args)
        {
            UnityEditor.Undo.RecordObject(this, "Added ActionTree Asset");
            // 解析得到行为基础类
            string asm = typeof(xAction_Base).Assembly.FullName;
            // 拼接字符串得到行为类
            Type type = Type.GetType($"{args.prefixNamespace}.{args.prefixClass}{args.actionNodeType},{asm}", true);

            // 创建行为资源
            xAction_Base actionData = ScriptableObject.CreateInstance(type) as xAction_Base;
            actionData.name = args.visualName;
            actionData.guid = UnityEditor.GUID.Generate().ToString();
            actionData.actionNodeType = args.actionNodeType;
            actionData.icon = args.iconName;
            actionData.NodeIcon = args.nodeIcon;
            actionData.visualNodeType = args.visualNodeType;
            actionData.identifyName = args.visualName;
            actionData.namespaces = args.prefixNamespace;
            actionData.classes = args.prefixClass;
            actionData.HasAvatar = args.hasAvatar;
            actionData.Avatar = args.avatar;
            actionData.themeSolution = args.themeSolution;
            actionData.themeColor = args.themeColor;
            actionData.TransparentNode = args.transparentNode;
            actionData.content = args.content;
            actionData.nodeGraphSize = args.size;
            actionData.isConcurrentExecution = args.isConcurrentExecution;

            actionData.SetActionAssetRoot(this);

            // 为变量类型节点数据特化处理，需要初始化类型 Variable
            if (actionData is xAction_Variable avnode)
            {
                avnode.variable = avnode.Initialized(args.visualName, args.variable.type);
                if (args.variable != null)
                {
                    switch (args.variable.type)
                    {
                        case xVariableType.String:
                            if (args.variable is Variable_String v_string)
                                avnode.variable.SetValue(v_string.GetValue<string>());
                            break;
                        case xVariableType.Float:
                            if (args.variable is Variable_Float v_float)
                                avnode.variable.SetValue(v_float.GetValue<float>());
                            break;
                        case xVariableType.Int:
                            if (args.variable is Variable_Int v_int)
                                avnode.variable.SetValue(v_int.GetValue<int>());
                            break;
                        case xVariableType.Bool:
                            if (args.variable is Variable_Bool v_bool)
                                avnode.variable.SetValue(v_bool.GetValue<bool>());
                            break;
                        case xVariableType.Vector2:
                            if (args.variable is Variable_Vector2 v_Vector2)
                                avnode.variable.SetValue(v_Vector2.GetValue<Vector2>());
                            break;
                        case xVariableType.Vector3:
                            if (args.variable is Variable_Vector3 v_Vector3)
                                avnode.variable.SetValue(v_Vector3.GetValue<Vector3>());
                            break;
                        case xVariableType.Vector4:
                            if (args.variable is Variable_Vector4 v_Vector4)
                                avnode.variable.SetValue(v_Vector4.GetValue<Vector4>());
                            break;
                        case xVariableType.Color:
                            if (args.variable is Variable_Color v_Color)
                                avnode.variable.SetValue(v_Color.GetValue<Color>());
                            break;
                    }
                }
            }
            // 如果创建的是属性节点，那么需要为属性节点初始化属性变量项，为后期其他节点获取数值做好准备工作
            if (actionData is xAction_Property property)
            {
                property.Propertys_Initialize();
            }
            // 添加到列表中
            Actions.Add(actionData);

            // 添加到资源文件下
            UnityEditor.AssetDatabase.AddObjectToAsset(actionData, this);
            //AssetDatabase.SaveAssets();

            // 创建后获取该行为树节点相对行为树资源根节点的路径
            string re_path = Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(Actions[^1]));
            string opt_path = re_path.Replace("Temp", $"{this.name}");
            string combine_path = $"{opt_path}   >   {Actions[^1].name}.asset";
            Actions[^1].path = combine_path;

            return actionData;
        }
#endif
        /// <summary>
        /// 从列表中移除一个数据节点
        /// </summary>
        /// <param name="node"></param>
        public void Remove(xAction_Base node)
        {
            if (node == null) return;

#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Removed ChildAction");
            Actions.Remove(node);
            UnityEditor.Undo.DestroyObjectImmediate(node);
            //AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
        /// <summary>
        /// 从列表中清空所有数据节点
        /// </summary>
        public void Clear()
        {
#if UNITY_EDITOR
            // 移除子级的所有资源
            foreach (var node in Actions)
            {
                UnityEditor.AssetDatabase.RemoveObjectFromAsset(node);
                DestroyImmediate(node, true);
            }

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

            // 刷新资源状态
            UnityEditor.EditorUtility.SetDirty(this);
            //AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
        /// <summary>
        /// 使用目标资源替换当前资源
        /// </summary>
        /// <param name="targetAsset"></param>
        public void Replace(xAction_Asset targetAsset)
        {
            if (targetAsset == null) return;

#if UNITY_EDITOR
            // 清空当前原始资源的所有子节点
            Clear();

            // 更新所有使用到的变量值（当前资源 - 更新）
            this.Variables_Refresh();
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);

            // 更新所有使用到的变量值（目标资源 - 更新）
            targetAsset.Variables_Refresh();
            UnityEditor.AssetDatabase.SaveAssetIfDirty(targetAsset);

            LastSaveDateTime = targetAsset.LastSaveDateTime = DateTime.Now.ToString("yyyy-MM-dd  -  HH:mm:ss");

            GraphviewGridBackgroundThemes = targetAsset.GraphviewGridBackgroundThemes.Clone();

            XGraph_BlackBoardViewDisplay = targetAsset.XGraph_BlackBoardViewDisplay;
            XGraph_InspectorViewDisplay = targetAsset.XGraph_InspectorViewDisplay;
            XGraph_DisplayNodeColor = targetAsset.XGraph_DisplayNodeColor;
            XGraph_DisplayNodeFlow = targetAsset.XGraph_DisplayNodeFlow;

            GraphNodesStruct = targetAsset.GraphNodesStruct;

            Last_GraphView_BlackboardPanel_TransformData = targetAsset.Last_GraphView_BlackboardPanel_TransformData.Clone();
            Last_GraphView_InspectorPanel_TransformData = targetAsset.Last_GraphView_InspectorPanel_TransformData.Clone();

            GraphviewRectangleSelectorThemes = targetAsset.GraphviewRectangleSelectorThemes.Clone();

            // 覆盖原有的 Decals 数据列表
            Decals = new List<xDecalData>();
            foreach (var decal in targetAsset.Decals)
            {
                Decals.Add(decal.Clone(false));
            }

            // 覆盖原有的 Sticks 数据列表
            Sticks = new List<xStickData>();
            foreach (var stick in targetAsset.Sticks)
            {
                Sticks.Add(stick.Clone(false));
            }

            // 覆盖原有的 Labels  数据列表
            Labels = new List<xLabelData>();
            foreach (var label in targetAsset.Labels)
            {
                Labels.Add(label.Clone(false));
            }

            // 覆盖原有的 Groups 数据列表
            Groups = new List<xGroupData>();
            foreach (var group in targetAsset.Groups)
            {
                Groups.Add(group.Clone(false));
            }

            // 覆盖原有的 Variables 数据列表
            Variables = new List<xVariableData>();
            foreach (var vare in targetAsset.Variables)
            {
                Variables.Add(vare.Clone(false));
            }

            // 覆盖原有的 BlackboardVariable 数据列表
            BlackboardVariable = new List<Variable>();
            foreach (var vare in targetAsset.BlackboardVariable)
            {
                BlackboardVariable.Add(vare.Clone(false));
            }

            // 创建新节点副本并添加到原始资源中
            Dictionary<xAction_Base, xAction_Base> dictionary = new Dictionary<xAction_Base, xAction_Base>();
            foreach (var sourceNode in targetAsset.Actions)
            {
                var newNode = Instantiate(sourceNode);
                newNode.name = sourceNode.name;
                newNode.hideFlags = HideFlags.None;
                newNode.SetActionAssetRoot(this);
                Actions.Add(newNode);
                UnityEditor.AssetDatabase.AddObjectToAsset(newNode, this);
                dictionary[sourceNode] = newNode;
            }
            // 重建父子引用关系
            foreach (var action in targetAsset.Actions)
            {
                if (action is xAction_Start s && s.childNodes != null)
                {
                    var newStart = dictionary[action] as xAction_Start;
                    newStart.childNodes.Clear();
                    foreach (var node in s.childNodes)
                    {
                        newStart.childNodes.Add(dictionary[node]);
                        // 设置父节点关系
                        dictionary[node].SetParentNode(newStart);
                    }
                }

                if (action is xAction_Wait w && w.childNodes != null)
                {
                    var newWait = dictionary[action] as xAction_Wait;
                    newWait.childNodes.Clear();
                    foreach (var node in w.childNodes)
                    {
                        newWait.childNodes.Add(dictionary[node]);
                        // 设置父节点关系
                        dictionary[node].SetParentNode(newWait);
                    }
                }

                if (action is xAction_Composite c && c.childNodes != null)
                {
                    var newComposite = dictionary[action] as xAction_Composite;
                    newComposite.childNodes.Clear();
                    foreach (var node in c.childNodes)
                    {
                        newComposite.childNodes.Add(dictionary[node]);
                        // 设置父节点关系
                        dictionary[node].SetParentNode(newComposite);
                    }
                }

                // 添加 ActionNode_Relay 的处理
                if (action is xAction_Relay r && r.childNodes != null)
                {
                    var newRelay = dictionary[action] as xAction_Relay;
                    newRelay.childNodes.Clear();
                    foreach (var node in r.childNodes)
                    {
                        newRelay.childNodes.Add(dictionary[node]);
                        // 设置父节点关系
                        dictionary[node].SetParentNode(newRelay);
                    }
                }

                if (action is xAction_Branch b)
                {
                    var newBranch = dictionary[action] as xAction_Branch;

                    if (b.childNode_true != null)
                        if (dictionary.TryGetValue(b.childNode_true, out var n_true))
                        {
                            newBranch.childNode_true = n_true;
                            // 设置父节点关系
                            n_true.SetParentNode(newBranch);
                        }
                    if (b.childNode_false != null)
                        if (dictionary.TryGetValue(b.childNode_false, out var n_false))
                        {
                            newBranch.childNode_false = n_false;
                            // 设置父节点关系
                            n_false.SetParentNode(newBranch);
                        }
                }
            }

            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 创建当前流程设计的克隆体（仅编辑器下）
        /// </summary>
        /// <returns></returns>
        public xAction_Asset Clone(string clonepath = "", bool saveAsset = true)
        {
            // 创建新的 ActionNode_Asset
            xAction_Asset newActionAsset = ScriptableObject.CreateInstance<xAction_Asset>();

            newActionAsset.LastSaveDateTime = LastSaveDateTime;

            newActionAsset.XGraph_BlackBoardViewDisplay = XGraph_BlackBoardViewDisplay;
            newActionAsset.XGraph_InspectorViewDisplay = XGraph_InspectorViewDisplay;
            newActionAsset.XGraph_DisplayNodeColor = XGraph_DisplayNodeColor;
            newActionAsset.XGraph_DisplayNodeFlow = XGraph_DisplayNodeFlow;

            newActionAsset.GraphNodesStruct = GraphNodesStruct;

            newActionAsset.Last_GraphView_BlackboardPanel_TransformData = Last_GraphView_BlackboardPanel_TransformData.Clone();
            newActionAsset.Last_GraphView_InspectorPanel_TransformData = Last_GraphView_InspectorPanel_TransformData.Clone();

            newActionAsset.GraphviewGridBackgroundThemes = GraphviewGridBackgroundThemes.Clone();

            newActionAsset.GraphviewRectangleSelectorThemes = GraphviewRectangleSelectorThemes.Clone();

            // 实例化新的 Sticks 列表，并从原始资源复制项
            newActionAsset.Sticks = new List<xStickData>();
            foreach (var item in Sticks)
            {
                newActionAsset.Sticks.Add(item.Clone(false));
            }

            // 实例化新的 Labels 列表，并从原始资源复制项
            newActionAsset.Labels = new List<xLabelData>();
            foreach (var item in Labels)
            {
                newActionAsset.Labels.Add(item.Clone(false));
            }

            // 实例化新的 Decals 列表，并从原始资源复制项
            newActionAsset.Decals = new List<xDecalData>();
            foreach (var item in Decals)
            {
                newActionAsset.Decals.Add(item.Clone(false));
            }

            // 实例化新的 Variables 列表，并从原始资源复制项
            newActionAsset.Variables = new List<xVariableData>();
            foreach (var item in Variables)
            {
                newActionAsset.Variables.Add(item.Clone(false));
            }

            // 实例化新的 Groups 列表，并从原始资源复制项
            newActionAsset.Groups = new List<xGroupData>();
            foreach (var item in Groups)
            {
#if UNITY_EDITOR
                newActionAsset.Groups.Add(item.Clone(false));
#endif
            }

            // 实例化新的 BlackboardVariable 列表，并从原始资源复制项
            newActionAsset.BlackboardVariable = new List<Variable>();
            foreach (var bbv in BlackboardVariable)
            {
#if UNITY_EDITOR
                newActionAsset.BlackboardVariable.Add(bbv.Clone(false));
#endif
            }

            newActionAsset.name = this.name;
            newActionAsset.LastGraphWindowSize = this.LastGraphWindowSize;
            newActionAsset.LastGraphViewPosition = this.LastGraphViewPosition;
            newActionAsset.LastGraphViewZoom = this.LastGraphViewZoom;

            // 用于映射原始节点到新节点（不包含分支逻辑类）
            Dictionary<xAction_Base, xAction_Base> originalRootDic = new Dictionary<xAction_Base, xAction_Base>();

            // 复制所有节点（不包含分支逻辑类）
            foreach (var node in this.Actions)
            {
                xAction_Base actionNode = Instantiate(node);
                actionNode.name = node.name;
                actionNode.hideFlags = HideFlags.None;

                // 关键修复点：初始化时清空所有子引用
                if (actionNode is xAction_Start newStart)
                    newStart.childNodes.Clear();
                else if (actionNode is xAction_Wait newWait)
                    newWait.childNodes.Clear();
                else if (actionNode is xAction_Composite newComp)
                    newComp.childNodes.Clear();
                else if (actionNode is xAction_Branch newBranch)
                {
                    newBranch.childNode_true = null;
                    newBranch.childNode_false = null;
                }

                newActionAsset.Actions.Add(actionNode);
                originalRootDic[node] = actionNode;
            }

            // 重建父子关系
            foreach (var node in this.Actions)
            {
                xAction_Base newParentNode = originalRootDic[node];

                // 处理 ActionNode_Start
                if (node is xAction_Start originalStart)
                {
                    var newStart = newParentNode as xAction_Start;
                    foreach (var originalChild in originalStart.childNodes)
                    {
                        if (originalRootDic.TryGetValue(originalChild, out var newChild))
                        {
                            newStart.childNodes.Add(newChild);
                            // 设置父节点关系
                            newChild.SetParentNode(newStart);
                        }
                    }
                }

                // 处理 ActionNode_Wait
                else if (node is xAction_Wait originalWait)
                {
                    var newWait = newParentNode as xAction_Wait;
                    foreach (var originalChild in originalWait.childNodes)
                    {
                        if (originalRootDic.TryGetValue(originalChild, out var newChild))
                        {
                            newWait.childNodes.Add(newChild);
                            // 设置父节点关系
                            newChild.SetParentNode(newWait);
                        }
                    }
                }

                // 处理 ActionNode_Composite
                else if (node is xAction_Composite originalComposite)
                {
                    var newComposite = newParentNode as xAction_Composite;
                    foreach (var originalChild in originalComposite.childNodes)
                    {
                        if (originalRootDic.TryGetValue(originalChild, out var newChild))
                        {
                            newComposite.childNodes.Add(newChild);
                            // 设置父节点关系
                            newChild.SetParentNode(newComposite);
                        }
                    }
                }

                // 处理 ActionNode_Branch
                else if (node is xAction_Branch originalBranch)
                {
                    var newBranch = newParentNode as xAction_Branch;

                    if (originalBranch.childNode_true != null)
                        if (originalRootDic.TryGetValue(originalBranch.childNode_true, out var new_true))
                        {
                            newBranch.childNode_true = new_true;
                            // 设置父节点关系
                            new_true.SetParentNode(newBranch);
                        }
                    if (originalBranch.childNode_false != null)
                        if (originalRootDic.TryGetValue(originalBranch.childNode_false, out var new_false))
                        {
                            newBranch.childNode_false = new_false;
                            // 设置父节点关系
                            new_false.SetParentNode(newBranch);
                        }
                }

                // 处理 ActionNode_Relay（如果存在）
                else if (node is xAction_Relay originalRelay)
                {
                    var newRelay = newParentNode as xAction_Relay;
                    foreach (var originalChild in originalRelay.childNodes)
                    {
                        if (originalRootDic.TryGetValue(originalChild, out var newChild))
                        {
                            newRelay.childNodes.Add(newChild);
                            // 设置父节点关系
                            newChild.SetParentNode(newRelay);
                        }
                    }
                }
            }

            if (saveAsset)
            {
                SaveNodeRootAsset(newActionAsset, string.IsNullOrEmpty(clonepath) ? $"{util_Dashboard.GetPath_Temp()}/GraphNodeTempAsset.asset" : clonepath);

                // 更新变量赋值数据
                newActionAsset.Variables_Refresh();

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.SaveAssetIfDirty(newActionAsset);
#endif
            }

            return newActionAsset;
        }
        /// <summary>
        /// 保存为Tree资源到目标路径下
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        public void SaveNodeRootAsset(xAction_Asset root, string path)
        {
#if UNITY_EDITOR
            // 提取根路径整理 - 去掉尾部的 /
            string path_root = util_Dashboard.GetPath_Root();
            path_root = path_root.Substring(0, path_root.Length - 1);

            // 目标路径整理 - 去掉尾部的 /
            string path_target = $"{util_Dashboard.GetPath_Temp()}";
            path_target = path_target.Substring(0, path_target.Length - 1);

            // 判断是否存在目标路径，如果不存在就创建该路径的文件夹
            if (!UnityEditor.AssetDatabase.AssetPathExists(path_target))
            {
                UnityEditor.AssetDatabase.CreateFolder(path_root, "Temp");
                //AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }

            // 保存为临时.asset 文件，供Unity资源系统进行操作
            UnityEditor.AssetDatabase.CreateAsset(root, path);
            foreach (var treenode in root.Actions)
            {
                UnityEditor.AssetDatabase.AddObjectToAsset(treenode, root);
            }

            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 获取指定数据节点的子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<xAction_Base> GetChildrenNodes(xAction_Base parent)
        {
            List<xAction_Base> nodes = new List<xAction_Base>();

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

            // 如果是 "ActionNode_Branch" 节点，那么就收集 "ActionNode_Branch" 节点下的 "childNodes"
            xAction_Branch branch = parent as xAction_Branch;
            if (branch != null)
            {
                if (branch.childNode_true != null)
                    nodes.Add(branch.childNode_true);
                if (branch.childNode_false != null)
                    nodes.Add(branch.childNode_false);
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
            child.SetParentNode(parent);

            #region 特化处理 - Start
            xAction_Start start = parent as xAction_Start;
            if (start)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(start, "Connect_StartNode");
#endif
                bool existChild = false;
                foreach (var c in start.childNodes)
                {
                    if (child.guid == c.guid)
                        existChild = true;
                }
                if (existChild)
                {
                    Debug.Log("start 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                start.childNodes.Add(child);
            }
            #endregion

            #region 特化处理 - Wait
            xAction_Wait wait = parent as xAction_Wait;
            if (wait)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(wait, "Connect_WaitNode");
#endif
                bool existChild = false;
                foreach (var c in wait.childNodes)
                {
                    if (child.guid == c.guid)
                        existChild = true;
                }

                if (existChild)
                {
                    Debug.Log("wait 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                wait.childNodes.Add(child);
            }
            #endregion

            #region 特化处理 - Composite
            xAction_Composite comp = parent as xAction_Composite;
            if (comp)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(comp, "Connect_CompositeNode");
#endif
                bool existChild = false;
                foreach (var c in comp.childNodes)
                {
                    if (child.guid == c.guid)
                        existChild = true;
                }
                if (existChild)
                {
                    Debug.Log("comp 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                comp.childNodes.Add(child);
            }
            #endregion

            #region 特化处理 - Relay
            xAction_Relay relay = parent as xAction_Relay;
            if (relay)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(relay, "Connect_RelayNode");
#endif
                bool existChild = false;
                foreach (var c in relay.childNodes)
                {
                    if (child.guid == c.guid)
                        existChild = true;
                }
                if (existChild)
                {
                    //Debug.Log("comp 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                relay.childNodes.Add(child);
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
            child.SetParentNode(parent);

            #region 特化处理 - Branch
            xAction_Branch branch = parent as xAction_Branch;
            if (branch)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(branch, "Connect_BranchNode");
#endif
                if (childmode == "开")
                {
                    if (branch.childNode_true != null)
                    {
                        if (child.guid == branch.childNode_true.guid)
                        {
                            Debug.Log("branch 节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                            return;
                        }
                    }
                    else
                    {
                        branch.childNode_true = child;
                    }
                }
                else if (childmode == "关")
                {
                    if (branch.childNode_false != null)
                    {
                        if (child.guid == branch.childNode_false.guid)
                        {
                            Debug.Log("branch 节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                            return;
                        }
                    }
                    else
                    {
                        branch.childNode_false = child;
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
            xAction_Start start = parent as xAction_Start;
            if (start)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(start, "RemoveConnect_StartNode");
#endif
                start.childNodes.Remove(child);
            }
            #endregion

            #region 特化处理 - Wait
            xAction_Wait wait = parent as xAction_Wait;
            if (wait)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(wait, "RemoveConnect_WaitNode");
#endif
                wait.childNodes.Remove(child);
            }
            #endregion

            #region 特化处理 - Composite
            xAction_Composite comp = parent as xAction_Composite;
            if (comp)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(comp, "RemoveConnect_CompositeNode");
#endif
                comp.childNodes.Remove(child);
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
        public void ChildNode_Remove(xAction_Base parent, xAction_Base child, string childmode)
        {
            //Debug.Log($"{parent.identifyName}       |  断开链接  ×  |      {child.identifyName}");

            #region 特化处理 - Branch
            xAction_Branch branch = parent as xAction_Branch;
            if (branch)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(branch, "RemoveConnect_BranchNode");
#endif
                if (childmode == "开")
                    branch.childNode_true = null;
                else if (childmode == "关")
                    branch.childNode_false = null;
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
                if (Actions[i].guid == guid)
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
                if (action.guid == Actions[i].guid)
                    Actions[i].isStartNode = true;
                else
                    Actions[i].isStartNode = false;

                if (Actions[i].On_Node_IsStartNode != null)
                    Actions[i].On_Node_IsStartNode(Actions[i].isStartNode);
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
                    if (vare.varguid == bb_variable.guid)
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
                foreach (var data in action.VariableDatas)
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
                foreach (var data in action.InternalVariableDatas)
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