namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public partial class xg_GraphView
    {
        /// <summary>
        /// 选中所有视觉节点
        /// </summary>
        public void Node_Selectall()
        {
            // 清空当前选择
            ClearSelection();

            // 将所有节点加入当前选择
            foreach (var node in nodes)
            {
                AddToSelection(node);
            }
            foreach (var node in ActionTreeAsset.Groups)
            {
                AddToSelection(node.group);
            }

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();
        }
        /// <summary>
        /// 实现视觉节点克隆逻辑
        /// </summary>
        public void Node_Duplicate()
        {
            // ---------------------------Linq 方法 （不推荐）
            //var selectedNodes = selection.OfType<Node>().ToList();
            //if (selectedNodes.Count == 0) return;

            // 收集所有选中的 Node 类型元素
            List<Node> selectedNodes = new List<Node>();
            foreach (var element in selection)
            {
                if (element is Node node)
                {
                    selectedNodes.Add(node);
                }
            }

            if (selectedNodes.Count == 0) return;

            ClearSelection(); // 清空当前选择（可选）

            List<DuplicateNodeData> dupDataList = new List<DuplicateNodeData>();

            foreach (var original in selectedNodes)
            {
                DuplicateNodeData dupData = new DuplicateNodeData();
                dupData.SourceNodeGuid = original.viewDataKey;

                // 如果克隆的节点为： VNode_Base
                if (original is xNode_Base target_base)
                {
                    // 待复制的源节点
                    xAction_Base action = target_base.ActionData;

                    // 克隆出新的节点
                    NodeCreateArgs_Action args = new NodeCreateArgs_Action();
                    args.visualName = action.identifyName;
                    args.prefixNamespace = action.BaseArgs.namespaces;
                    args.prefixClass = action.BaseArgs.classes;
                    args.actionNodeType = action.BaseArgs.actionNodeType;
                    args.iconName = action.BaseArgs.icon;
                    args.nodeIcon = action.BaseArgs.NodeIcon;
                    args.visualNodeType = action.BaseArgs.visualNodeType;
                    args.hasAvatar = action.BaseArgs.HasAvatar;
                    args.avatar = action.BaseArgs.Avatar;
                    args.themeSolution = action.BaseArgs.themeSolution;
                    args.themeColor = action.BaseArgs.themeColor;
                    args.transparentNode = action.BaseArgs.TransparentNode;
                    args.content = action.BaseArgs.content;
                    args.position = action.BaseArgs.nodeGraphPosition + new Vector2(81, 46.5f);
                    args.size = action.BaseArgs.nodeGraphSize;
                    args.isConcurrentExecution = action.BaseArgs.isConcurrentExecution;

                    // 为变量类型节点数据特化处理，需要初始化类型 Variable
                    if (action is xAction_Variable avnode)
                    {
                        switch (avnode.variable.type)
                        {
                            case xVariableType.String:
                                string val_String = avnode.variable.GetValue<string>();
                                args.variable = new Variable_String(avnode.identifyName, val_String);
                                break;
                            case xVariableType.Float:
                                float val_Float = avnode.variable.GetValue<float>();
                                args.variable = new Variable_Float(avnode.identifyName, val_Float);
                                break;
                            case xVariableType.Int:
                                int val_Int = avnode.variable.GetValue<int>();
                                args.variable = new Variable_Int(avnode.identifyName, val_Int);
                                break;
                            case xVariableType.Bool:
                                bool val_Bool = avnode.variable.GetValue<bool>();
                                args.variable = new Variable_Bool(avnode.identifyName, val_Bool);
                                break;
                            case xVariableType.Vector2:
                                Vector2 val_Vector2 = avnode.variable.GetValue<Vector2>();
                                args.variable = new Variable_Vector2(avnode.identifyName, val_Vector2);
                                break;
                            case xVariableType.Vector3:
                                Vector3 val_Vector3 = avnode.variable.GetValue<Vector3>();
                                args.variable = new Variable_Vector3(avnode.identifyName, val_Vector3);
                                break;
                            case xVariableType.Vector4:
                                Vector4 val_Vector4 = avnode.variable.GetValue<Vector4>();
                                args.variable = new Variable_Vector4(avnode.identifyName, val_Vector4);
                                break;
                            case xVariableType.Color:
                                Color val_Color = avnode.variable.GetValue<Color>();
                                args.variable = new Variable_Color(avnode.identifyName, val_Color);
                                break;
                        }
                    }
                    xNode_Base node = CreateNode(args) as xNode_Base;

                    // 调用克隆出来的节点的克隆回调
                    if (node.ActionData.On_Node_Duplicated != null)
                        node.ActionData.On_Node_Duplicated(node.ActionData, target_base.ActionData);

                    dupData.DuplicatedNode = node;
                }
                // 如果克隆的节点为： VNode_Stick
                else if (original is xNode_Stick target_stick)
                {
                    xStickData data = target_stick.StickData.Clone(true);

                    NodeCreateArgs_Stick args = new NodeCreateArgs_Stick();
                    args.stickName = data.name;
                    args.stickContent = data.content;
                    args.position = data.position + new Vector2(data.size.x / 2, data.size.y / 2);
                    args.size = data.size;

                    dupData.DuplicatedNode = CreateNode(args);
                }
                // 如果克隆的节点为： VNode_Stick
                else if (original is xNode_Label target_label)
                {
                    xLabelData data = target_label.LabelData.Clone(true);

                    NodeCreateArgs_Label args = new NodeCreateArgs_Label();
                    args.content = data.content;
                    args.italic = data.italic;
                    args.bold = data.bold;
                    args.opacity = data.opacity;
                    args.color = data.color;
                    args.font = data.font;
                    args.fontSize = data.fontSize;
                    args.position = data.position + new Vector2(data.size.x / 2, data.size.y / 2);
                    args.size = data.size;

                    dupData.DuplicatedNode = CreateNode(args);
                }
                // 如果克隆的节点为： VNode_Decal
                else if (original is xNode_Decal target_decal)
                {
                    xDecalData data = target_decal.DecalData.Clone(true);

                    NodeCreateArgs_Decal args = new NodeCreateArgs_Decal();
                    args.position = data.position + new Vector2(data.size.x / 2, data.size.y / 2);
                    args.size = data.size;
                    args.opacity = data.opacity;
                    args.hasTexture = data.texture_exist;
                    args.color = data.color;
                    args.decalTexture = data.texture_decal;
                    args.scale = data.scale;
                    Node decalNode = CreateNode(args);

                    dupData.DuplicatedNode = decalNode as xNode_Decal;
                }
                // 如果克隆的节点为： VNode_Variable
                else if (original is xNode_Variable target_vare)
                {
                    xVariableData data = target_vare.VariableData.Clone(true);

                    NodeCreateArgs_Variable args = new NodeCreateArgs_Variable();
                    args.name = data.name;
                    args.description = data.description;
                    args.type = data.type;
                    args.position = data.position + new Vector2(data.size.x / 2, data.size.y / 2);
                    args.varguid = data.guid_v;
                    args.size = data.size;
                    args.variable = data.variable;

                    Node vareNode = CreateNode(args);

                    dupData.DuplicatedNode = vareNode as xNode_Variable;
                }

                dupDataList.Add(dupData);

                AddToSelection(dupData.DuplicatedNode as Node);
            }

            if (OnDuplicateNodes != null)
                OnDuplicateNodes(dupDataList);

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();
        }
        /// <summary>
        /// 将节点复制到缓冲区
        /// </summary>
        public void Node_Copy()
        {
            // 先清空拷贝缓冲列表
            gv_CopiedNodeList.Clear();

            // ---------------------------Linq 方法 （不推荐）
            //var selectedNodes = selection.OfType<Node>().ToList();

            // 获取所有选中的 Node 类型元素
            List<Node> selectedNodes = new List<Node>();
            foreach (var element in selection)
            {
                if (element is Node node)
                {
                    selectedNodes.Add(node);
                }
            }

            if (selectedNodes.Count == 0) return;

            // 特化处理 - Action
            foreach (var node in CurrentSelectedNodes_Base)
            {
                // 将选中的节点拷贝进缓冲区
                gv_CopiedNodeList.Add(node);
            }
            // 特化处理 - Decal
            foreach (var node in CurrentSelectedNodes_Decal)
            {
                // 将选中的节点拷贝进缓冲区
                gv_CopiedNodeList.Add(node);
            }
            // 特化处理 - Variable
            foreach (var node in CurrentSelectedNodes_Variable)
            {
                // 将选中的节点拷贝进缓冲区
                gv_CopiedNodeList.Add(node);
            }
            // 特化处理 - Stick
            foreach (var node in CurrentSelectedNodes_Stick)
            {
                // 将选中的节点拷贝进缓冲区
                gv_CopiedNodeList.Add(node);
            }
            // 特化处理 - Label
            foreach (var node in CurrentSelectedNodes_Label)
            {
                // 将选中的节点拷贝进缓冲区
                gv_CopiedNodeList.Add(node);
            }
        }
        /// <summary>
        /// 粘贴拷贝到缓冲区的节点
        /// </summary>
        public void Node_Paste()
        {
            if (gv_CopiedNodeList.Count <= 0)
                return;

            // 清空当前选择
            ClearSelection();

            Vector2 pos_mouse = GetNodeCreatedMousePosition();
            Vector2 realpos = pos_mouse;

            foreach (var node in gv_CopiedNodeList)
            {
                if (node is Node nd)
                {
                    Vector2 nd_size = nd.GetPosition().size;

                    if (node is xNode_Base node_base)
                    {
                        xAction_Base action = node_base.ActionData;

                        if (action.BaseArgs.actionNodeType == "Relay")
                            continue;

                        // 从拷贝的节点中创建出新的节点数据
                        NodeCreateArgs_Action args = new NodeCreateArgs_Action();
                        args.visualName = action.identifyName;
                        args.prefixNamespace = action.BaseArgs.namespaces;
                        args.prefixClass = action.BaseArgs.classes;
                        args.actionNodeType = action.BaseArgs.actionNodeType;
                        args.iconName = action.BaseArgs.icon;
                        args.nodeIcon = action.BaseArgs.NodeIcon;
                        args.visualNodeType = action.BaseArgs.visualNodeType;
                        args.hasAvatar = action.BaseArgs.HasAvatar;
                        args.avatar = action.BaseArgs.Avatar;
                        args.themeSolution = action.BaseArgs.themeSolution;
                        args.themeColor = action.BaseArgs.themeColor;
                        args.transparentNode = action.BaseArgs.TransparentNode;
                        args.content = action.BaseArgs.content;
                        args.position = realpos;
                        args.size = action.BaseArgs.nodeGraphSize;

                        // 为变量类型节点数据特化处理，需要初始化类型 Variable
                        if (action is xAction_Variable avnode)
                        {
                            switch (avnode.variable.type)
                            {
                                case xVariableType.String:
                                    string val_String = avnode.variable.GetValue<string>();
                                    args.variable = new Variable_String(avnode.identifyName, val_String);
                                    break;
                                case xVariableType.Float:
                                    float val_Float = avnode.variable.GetValue<float>();
                                    args.variable = new Variable_Float(avnode.identifyName, val_Float);
                                    break;
                                case xVariableType.Int:
                                    int val_Int = avnode.variable.GetValue<int>();
                                    args.variable = new Variable_Int(avnode.identifyName, val_Int);
                                    break;
                                case xVariableType.Bool:
                                    bool val_Bool = avnode.variable.GetValue<bool>();
                                    args.variable = new Variable_Bool(avnode.identifyName, val_Bool);
                                    break;
                                case xVariableType.Vector2:
                                    Vector2 val_Vector2 = avnode.variable.GetValue<Vector2>();
                                    args.variable = new Variable_Vector2(avnode.identifyName, val_Vector2);
                                    break;
                                case xVariableType.Vector3:
                                    Vector3 val_Vector3 = avnode.variable.GetValue<Vector3>();
                                    args.variable = new Variable_Vector3(avnode.identifyName, val_Vector3);
                                    break;
                                case xVariableType.Vector4:
                                    Vector4 val_Vector4 = avnode.variable.GetValue<Vector4>();
                                    args.variable = new Variable_Vector4(avnode.identifyName, val_Vector4);
                                    break;
                                case xVariableType.Color:
                                    Color val_Color = avnode.variable.GetValue<Color>();
                                    args.variable = new Variable_Color(avnode.identifyName, val_Color);
                                    break;
                            }
                        }

                        AddToSelection(CreateNode(args));
                    }
                    if (node is xNode_Decal node_decal)
                    {
                        xDecalData data = node_decal.DecalData;

                        NodeCreateArgs_Decal args = new NodeCreateArgs_Decal();
                        args.size = data.size;
                        args.opacity = data.opacity;
                        args.color = data.color;
                        args.hasTexture = data.texture_exist;
                        args.decalTexture = data.texture_decal;
                        args.position = realpos;
                        args.scale = data.scale;
                        AddToSelection(CreateNode(args));
                    }
                    if (node is xNode_Variable node_vare)
                    {
                        xVariableData data = node_vare.VariableData;

                        NodeCreateArgs_Variable args = new NodeCreateArgs_Variable();
                        args.name = data.name;
                        args.description = data.description;
                        args.type = data.type;
                        args.position = realpos;
                        args.varguid = data.guid_v;
                        args.size = data.size;
                        args.variable = data.variable;
                        AddToSelection(CreateNode(args));
                    }
                    if (node is xNode_Stick node_stick)
                    {
                        xStickData data = node_stick.StickData;

                        NodeCreateArgs_Stick args = new NodeCreateArgs_Stick();
                        args.size = data.size;
                        args.stickContent = data.content;
                        args.stickName = data.name;
                        args.position = realpos;

                        AddToSelection(CreateNode(args));
                    }
                    if (node is xNode_Label node_label)
                    {
                        xLabelData data = node_label.LabelData;

                        NodeCreateArgs_Label args = new NodeCreateArgs_Label();
                        args.size = data.size;
                        args.content = data.content;
                        args.opacity = data.opacity;
                        args.position = realpos;
                        args.font = data.font;
                        args.color = data.color;
                        args.bold = data.bold;
                        args.italic = data.italic;
                        args.fontSize = data.fontSize;

                        AddToSelection(CreateNode(args));
                    }
                    // 递增间距
                    realpos.y += nd_size.y + 5;
                }
            }
        }
        /// <summary>
        /// 移除当前选择的所有节点及其相关的连线
        /// </summary>
        public void Node_Delete()
        {
            // ---------------------------Linq 方法（不推荐）
            //// 获取当前选择的所有节点
            //var selectedNodes = selection.OfType<Node>().ToList();

            //// 如果没有选中的节点，直接返回
            //if (selectedNodes.Count == 0)
            //{
            //    Debug.LogWarning("没有选中的节点！");
            //    return;
            //}

            // 获取当前选择的所有节点
            List<Node> selectedNodes = new List<Node>();
            foreach (var element in selection)
            {
                if (element is Node node)
                {
                    selectedNodes.Add(node);
                }
            }

            // 如果没有选中的节点，直接返回
            if (selectedNodes.Count == 0)
            {
                Debug.LogWarning("没有选中的节点！");
                return;
            }

            // 创建一个 GraphViewChange 对象用于调用GraphView的OnGraphViewChanged事件
            var graphViewChange = new GraphViewChange();
            graphViewChange.elementsToRemove = new List<GraphElement>();

            // 遍历所有选中的节点
            foreach (var node in selectedNodes)
            {
                // ---------------------------Linq 方法（不推荐）
                //// 移除节点的所有连线
                //var edgesToRemove = edges.ToList()
                //    .Where(edge => edge.input.node == node || edge.output.node == node)
                //    .ToList();

                // 移除节点的所有连线
                List<Edge> edgesToRemove = new List<Edge>();
                foreach (Edge edge in edges)
                {
                    if (edge.input.node == node || edge.output.node == node)
                    {
                        edgesToRemove.Add(edge);
                    }
                }

                foreach (var edge in edgesToRemove)
                {
                    // 移除连线
                    RemoveElement(edge);
                    graphViewChange.elementsToRemove.Add(edge); // 添加到 GraphViewChange
                }

                // 移除节点本身
                RemoveElement(node);
                graphViewChange.elementsToRemove.Add(node); // 添加到 GraphViewChange

                // 与删除的节点的端口断开连接
                foreach (var edge in edgesToRemove)
                {
                    edge.input.Disconnect(edge);
                    edge.output.Disconnect(edge);
                }
            }

            // 清空当前选择
            ClearSelection();

            // 调用 OnGraphViewChanged 方法
            OnGraphViewChanged(graphViewChange);
        }
        /// <summary>
        /// 移除指定的节点及其相关的连线
        /// </summary>
        public void Node_Delete(Node node)
        {
            // 如果没有节点，直接返回
            if (node == null)
            {
                Debug.LogWarning("无法删除无效的节点！");
                return;
            }

            // 创建一个 GraphViewChange 对象用于调用GraphView的OnGraphViewChanged事件
            var graphViewChange = new GraphViewChange();
            graphViewChange.elementsToRemove = new List<GraphElement>();

            // ---------------------------Linq 方法（不推荐）
            //// 移除节点的所有连线
            //var edgesToRemove = edges.ToList()
            //    .Where(edge => edge.input.node == node || edge.output.node == node)
            //    .ToList();

            // 移除节点的所有连线
            List<Edge> edgesToRemove = new List<Edge>();
            foreach (Edge edge in edges)
            {
                if (edge.input.node == node || edge.output.node == node)
                {
                    edgesToRemove.Add(edge);
                }
            }

            foreach (var edge in edgesToRemove)
            {
                // 移除连线
                RemoveElement(edge);
                graphViewChange.elementsToRemove.Add(edge); // 添加到 GraphViewChange
            }

            // 移除节点本身
            RemoveElement(node);
            graphViewChange.elementsToRemove.Add(node); // 添加到 GraphViewChange

            // 与删除的节点的端口断开连接
            foreach (var edge in edgesToRemove)
            {
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
            }

            // 调用 OnGraphViewChanged 方法
            OnGraphViewChanged(graphViewChange);
        }
        /// <summary>
        /// 清空视觉节点
        /// </summary>
        public void Node_Clear()
        {
            // ---------------------------Linq 方法（不推荐）
            //// 删除所有节点
            //foreach (var node in nodes.ToList())
            //{
            //    if (node is xNode_Base b)
            //        b.DuplicateAction_Remove();
            //    if (node is xNode_Variable v)
            //        v.DuplicateAction_Remove();
            //    RemoveElement(node);
            //}

            // 删除所有节点
            List<Node> nodesList = new List<Node>();
            foreach (var element in nodes)
            {
                nodesList.Add(element);
            }

            foreach (var node in nodesList)
            {
                if (node is xNode_Base b)
                    b.DuplicateAction_Remove();
                if (node is xNode_Variable v)
                    v.DuplicateAction_Remove();
                RemoveElement(node);
            }

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();
        }
        /// <summary>
        /// 清空连线
        /// </summary>
        public void EdgesClear()
        {
            // ---------------------------Linq 方法（不推荐）
            //// 删除所有连线
            //foreach (var edge in edges.ToList())
            //{
            //    RemoveElement(edge);
            //}

            // 删除所有连线
            List<Edge> edgesList = new List<Edge>();
            foreach (var edge in edges)
            {
                edgesList.Add(edge);
            }

            foreach (var edge in edgesList)
            {
                RemoveElement(edge);
            }
        }

        /// <summary>
        /// 创建视觉节点
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public xNode_Base Node_MakeAction(Vector2 pos, xAction_Base data = null)
        {
            if (data.BaseArgs.visualNodeType == "None")
                return null;

            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.{data.BaseArgs.visualNodeType}");
            // 创建 NodeView 类型的实例为 visualNode 基类
            xNode_Base node = Activator.CreateInstance(type_nodeview) as xNode_Base;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            node.Initialize(this, pos, data);


            #region GraphView 视图操作
            // 添加进当前主GraphView视图中
            this.AddElement(node);

            // 指定生成的节点点击事件委托，便于实现调用点击节点时调用
            node.On_SelectedNode = OnSelectedNode;
            node.On_UnSelectedNode = OnUnSelectedNode;
            // 刷新GraphView视图
            node.RefreshExpandedState();
            node.RefreshPorts();
            #endregion

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();

            node.Draw();

            // 刷新节点主题配色
            RefreshTheme_GraphNode(node);

            return node;
        }
        /// <summary>
        /// 创建视觉节点 - 贴纸
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public xNode_Decal Node_MakeDecal(Vector2 pos, xDecalData data = null)
        {
            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.xNode_Decal");
            // 创建 NodeView 类型的实例为 visualNode 基类
            xNode_Decal node = Activator.CreateInstance(type_nodeview) as xNode_Decal;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            node.Initialize(this, pos, data);

            #region GraphView 视图操作
            // 添加进当前主GraphView视图中
            this.AddElement(node);

            // 指定生成的节点点击事件委托，便于实现调用点击节点时调用
            node.OnSelectedNode = OnSelectedNode;
            node.OnUnSelectedNode = OnUnSelectedNode;

            // 刷新GraphView视图
            node.RefreshExpandedState();
            node.RefreshPorts();
            #endregion

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();

            return node;
        }
        /// <summary>
        /// 创建视觉节点 - 变量
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public xNode_Variable Node_MakeVariable(Vector2 pos, xVariableData data = null)
        {
            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.xNode_Variable");
            // 创建 NodeView 类型的实例为 visualNode 基类
            xNode_Variable node = Activator.CreateInstance(type_nodeview) as xNode_Variable;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            node.Initialize(this, pos, data);

            #region GraphView 视图操作
            // 添加进当前主GraphView视图中
            this.AddElement(node);

            // 指定生成的节点点击事件委托，便于实现调用点击节点时调用
            node.OnSelectedNode = OnSelectedNode;
            node.OnUnSelectedNode = OnUnSelectedNode;

            // 刷新GraphView视图
            node.RefreshExpandedState();
            node.RefreshPorts();
            #endregion

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();

            return node;
        }
        /// <summary>
        /// 创建视觉节点 - 便签
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public xNode_Stick Node_MakeStick(Vector2 pos, xStickData data = null)
        {
            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.xNode_Stick");
            // 创建 NodeView 类型的实例为 visualNode 基类
            xNode_Stick node = Activator.CreateInstance(type_nodeview) as xNode_Stick;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            node.Initialize(this, pos, data);

            #region GraphView 视图操作
            // 添加进当前主GraphView视图中
            this.AddElement(node);

            // 指定生成的节点点击事件委托，便于实现调用点击节点时调用
            node.OnSelectedNode = OnSelectedNode;
            node.OnUnSelectedNode = OnUnSelectedNode;

            // 刷新GraphView视图
            node.RefreshExpandedState();
            node.RefreshPorts();
            #endregion

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();

            return node;
        }
        /// <summary>
        /// 创建视觉节点 - 标签
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public xNode_Label Node_MakeLabel(Vector2 pos, xLabelData data = null)
        {
            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.xNode_Label");
            // 创建 NodeView 类型的实例为 visualNode 基类
            xNode_Label node = Activator.CreateInstance(type_nodeview) as xNode_Label;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            node.Initialize(this, pos, data);

            #region GraphView 视图操作
            // 添加进当前主GraphView视图中
            this.AddElement(node);

            // 指定生成的节点点击事件委托，便于实现调用点击节点时调用
            node.OnSelectedNode = OnSelectedNode;
            node.OnUnSelectedNode = OnUnSelectedNode;

            // 刷新GraphView视图
            node.RefreshExpandedState();
            node.RefreshPorts();
            #endregion

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();

            return node;
        }
        /// <summary>
        /// 创建延展节点
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public xNode_Relay Node_MakeRelay(Vector2 pos, xAction_Base data = null)
        {
            if (data.BaseArgs.visualNodeType == "None")
                return null;

            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.{data.BaseArgs.visualNodeType}");
            // 创建 NodeView 类型的实例为 visualNode 基类
            xNode_Relay relay = Activator.CreateInstance(type_nodeview) as xNode_Relay;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            relay.Initialize(this, pos, data);


            // 刷新节点主题配色
            RefreshTheme_GraphNode(relay);

            #region GraphView 视图操作
            // 添加进当前主GraphView视图中
            this.AddElement(relay);

            // 指定生成的节点点击事件委托，便于实现调用点击节点时调用
            relay.On_SelectedNode = OnSelectedNode;
            relay.On_UnSelectedNode = OnUnSelectedNode;
            #endregion

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();

            return relay;
        }

        /// <summary>
        ///  创建节点 - 行为
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Node CreateNode(NodeCreateArgs_Action args)
        {
            // 创建新的节点并指定资源数据项
            xNode_Base visualNode = Node_MakeAction(args.position, ActionTreeAsset.Create(args));

            // 刷新节点
            visualNode.RefreshExpandedState();
            visualNode.RefreshPorts();
            visualNode.CheckTransparentDisplay(args.transparentNode);
            visualNode.CheckAvatarChanged();
            visualNode.CheckExecutionModel();
            return visualNode;
        }
        /// <summary>
        /// 创建节点 - 便签
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Node CreateNode(NodeCreateArgs_Stick args)
        {
            // 便签节点创建，便签类是不需要加入行为树根资源中的，而是加入到行为树根资源的 Sticks 变量中
            Undo.RecordObject(ActionTreeAsset, "Create StickNode");
            // 新建行为树便签内容加入到行为树根资源的 Sticks 变量中
            xStickData stickdata = new xStickData(args.stickName, args.stickContent, GUID.Generate().ToString(), args.position, args.size);
            ActionTreeAsset.StickNote_Add(stickdata);

            // 创建新的节点并指定资源数据项
            xNode_Stick stickNode = Node_MakeStick(args.position, stickdata);

            // 刷新节点
            stickNode.Draw();
            stickNode.RefreshExpandedState();
            stickNode.RefreshPorts();

            return stickNode;
        }
        /// <summary>
        /// 创建节点 - 标签
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Node CreateNode(NodeCreateArgs_Label args)
        {
            // 便签节点创建，便签类是不需要加入行为树根资源中的，而是加入到行为树根资源的 Sticks 变量中
            Undo.RecordObject(ActionTreeAsset, "Create LabelNode");
            // 新建行为树便签内容加入到行为树根资源的 Sticks 变量中
            xLabelData labeldata = new xLabelData(args.content, GUID.Generate().ToString(), args.position, args.size, args.color, args.font, args.opacity, args.fontSize, args.bold, args.italic);
            ActionTreeAsset.Label_Add(labeldata);

            // 创建新的节点并指定资源数据项
            xNode_Label labelNode = Node_MakeLabel(args.position, labeldata);

            // 刷新节点
            labelNode.Draw();
            labelNode.RefreshExpandedState();
            labelNode.RefreshPorts();

            return labelNode;
        }
        /// <summary>
        /// 创建节点 - 贴图
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Node CreateNode(NodeCreateArgs_Decal args)
        {
            // 贴图节点创建，贴图类是不需要加入行为树根资源中的，而是加入到行为树根资源的 Decals 变量中
            Undo.RecordObject(ActionTreeAsset, "Create DecalNode");
            // 新建行为树贴图内容加入到行为树根资源的 Decals 变量中
            xDecalData decaldata = new xDecalData(GUID.Generate().ToString(), args.position, args.size, args.scale, args.color, args.opacity, args.hasTexture, args.decalTexture);
            ActionTreeAsset.Decal_Add(decaldata);

            // 创建新的节点并指定资源数据项
            xNode_Decal decalNode = Node_MakeDecal(args.position, decaldata);

            // 刷新节点
            decalNode.Draw();
            decalNode.RefreshExpandedState();
            decalNode.RefreshPorts();

            return decalNode;
        }
        /// <summary>
        /// 创建节点 - 变量
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Node CreateNode(NodeCreateArgs_Variable args)
        {
            // 贴图节点创建，贴图类是不需要加入行为树根资源中的，而是加入到行为树根资源的 Variables 变量中
            Undo.RecordObject(ActionTreeAsset, "Create VariablelNode");
            // 新建行为树贴图内容加入到行为树根资源的 Variables 变量中
            xVariableData vardata = new xVariableData(args.name, args.description, args.type, GUID.Generate().ToString(), args.position, args.size, args.varguid, args.variable, args.transparentNode);
            ActionTreeAsset.Variable_Add(vardata);

            // 创建新的节点并指定资源数据项
            xNode_Variable decalNode = Node_MakeVariable(args.position, vardata);

            // 刷新节点
            decalNode.Draw();
            decalNode.RefreshExpandedState();
            decalNode.RefreshPorts();

            return decalNode;
        }
    }
}