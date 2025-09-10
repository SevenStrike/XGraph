namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            foreach (var node in ActionTreeAsset.NodeGroupDatas)
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
            var selectedNodes = selection.OfType<Node>().ToList();
            if (selectedNodes.Count == 0) return;

            ClearSelection(); // 清空当前选择（可选）

            List<DuplicateNodeData> dupDataList = new List<DuplicateNodeData>();

            foreach (var original in selectedNodes)
            {
                DuplicateNodeData dupData = new DuplicateNodeData();
                dupData.SourceNodeGuid = original.viewDataKey;

                if (original is VNode_Base target_base)
                {
                    // 待复制的源节点
                    ActionNode_Base actionnode = target_base.ActionData;

                    // 克隆出新的节点
                    NodeCreateArgs_Action args = new NodeCreateArgs_Action();
                    args.visualName = actionnode.identifyName;
                    args.prefixNamespace = actionnode.namespaces;
                    args.prefixClass = actionnode.classes;
                    args.actionNodeType = actionnode.actionNodeType;
                    args.iconName = actionnode.icon;
                    args.nodeIcon = actionnode.NodeIcon;
                    args.visualNodeType = actionnode.visualNodeType;
                    args.hasAvatar = actionnode.HasAvatar;
                    args.avatar = actionnode.Avatar;
                    args.themeSolution = actionnode.themeSolution;
                    args.themeColor = actionnode.themeColor;
                    args.transparentNode = actionnode.TransparentNode;
                    args.content = actionnode.content;
                    args.position = actionnode.nodeGraphPosition + new Vector2(81, 46.5f);
                    args.size = actionnode.nodeGraphSize;

                    dupData.DuplicatedNode = CreateNode(args);
                }
                else if (original is VNode_Stick target_stick)
                {
                    ActionStickData data = target_stick.StickData.Clone(true);

                    NodeCreateArgs_Stick args = new NodeCreateArgs_Stick();
                    args.stickName = data.name;
                    args.stickContent = data.content;
                    args.position = data.position + new Vector2(data.size.x / 2, data.size.y / 2);
                    args.size = data.size;

                    dupData.DuplicatedNode = CreateNode(args);
                }
                else if (original is VNode_Decal target_decal)
                {
                    ActionDecalData data = target_decal.DecalData.Clone(true);

                    NodeCreateArgs_Decal args = new NodeCreateArgs_Decal();
                    args.position = data.position + new Vector2(data.size.x / 2, data.size.y / 2);
                    args.size = data.size;
                    args.opacity = data.opacity;
                    args.hasTexture = data.HasTexture;
                    args.decalTexture = data.DecalTexture;
                    object decalNode = CreateNode(args);

                    dupData.DuplicatedNode = decalNode as VNode_Decal;
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

            var selectedNodes = selection.OfType<Node>().ToList();
            if (selectedNodes.Count == 0) return;

            // 特化处理 - ActionData
            foreach (var node in CurrentSelectedNodes_Base)
            {
                // 将选中的节点拷贝进缓冲区
                gv_CopiedNodeList.Add(node);
            }

            // 特化处理 - DecalData
            foreach (var node in CurrentSelectedNodes_Decal)
            {
                // 将选中的节点拷贝进缓冲区
                gv_CopiedNodeList.Add(node);
            }

            // 特化处理 - StickNoteData
            foreach (var node in CurrentSelectedNodes_Stick)
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

                    if (node is VNode_Base node_base)
                    {
                        ActionNode_Base data = node_base.ActionData;

                        if (data.actionNodeType == "Relay")
                            continue;

                        // 从拷贝的节点中创建出新的节点数据
                        NodeCreateArgs_Action args = new NodeCreateArgs_Action();
                        args.visualName = data.identifyName;
                        args.prefixNamespace = data.namespaces;
                        args.prefixClass = data.classes;
                        args.actionNodeType = data.actionNodeType;
                        args.iconName = data.icon;
                        args.nodeIcon = data.NodeIcon;
                        args.visualNodeType = data.visualNodeType;
                        args.hasAvatar = data.HasAvatar;
                        args.avatar = data.Avatar;
                        args.themeSolution = data.themeSolution;
                        args.themeColor = data.themeColor;
                        args.transparentNode = data.TransparentNode;
                        args.content = data.content;
                        args.position = realpos;
                        args.size = data.nodeGraphSize;

                        AddToSelection(CreateNode(args));
                    }
                    if (node is VNode_Decal node_decal)
                    {
                        ActionDecalData data = node_decal.DecalData;

                        NodeCreateArgs_Decal args = new NodeCreateArgs_Decal();
                        args.size = data.size;
                        args.opacity = data.opacity;
                        args.hasTexture = data.HasTexture;
                        args.decalTexture = data.DecalTexture;
                        args.position = realpos;
                        args.scale = data.scale;
                        AddToSelection(CreateNode(args));
                    }
                    if (node is VNode_Stick node_stick)
                    {
                        ActionStickData data = node_stick.StickData;

                        NodeCreateArgs_Stick args = new NodeCreateArgs_Stick();
                        args.size = data.size;
                        args.stickContent = data.content;
                        args.stickName = data.name;
                        args.position = realpos;

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
            // 获取当前选择的所有节点
            var selectedNodes = selection.OfType<Node>().ToList();

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
                // 移除节点的所有连线
                var edgesToRemove = edges.ToList()
                    .Where(edge => edge.input.node == node || edge.output.node == node)
                    .ToList();
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
        /// 清空视觉节点
        /// </summary>
        public void Node_Clear()
        {
            // 删除所有节点
            foreach (var node in nodes.ToList())
            {
                if (node is VNode_Base b)
                    b.DuplicateAction_Remove();
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
            // 删除所有连线
            foreach (var edge in edges.ToList())
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
        public VNode_Base Node_MakeAction(Vector2 pos, ActionNode_Base data = null)
        {
            if (data.visualNodeType == "None")
                return null;

            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.{data.visualNodeType}");
            // 创建 NodeView 类型的实例为 visualNode 基类
            VNode_Base node = Activator.CreateInstance(type_nodeview) as VNode_Base;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            node.Initialize(this, pos, data);

            // 刷新节点主题配色
            RefreshTheme_GraphNode(node);

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
        /// 创建视觉节点 - 贴纸
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public VNode_Decal Node_MakeDecal(Vector2 pos, ActionDecalData data = null)
        {
            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.VNode_Decal");
            // 创建 NodeView 类型的实例为 visualNode 基类
            VNode_Decal node = Activator.CreateInstance(type_nodeview) as VNode_Decal;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            node.Initialize(this, pos, data);

            #region GraphView 视图操作
            // 添加进当前主GraphView视图中
            this.AddElement(node);

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
        public VNode_Stick Node_MakeStick(Vector2 pos, ActionStickData data = null)
        {
            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.VNode_Stick");
            // 创建 NodeView 类型的实例为 visualNode 基类
            VNode_Stick node = Activator.CreateInstance(type_nodeview) as VNode_Stick;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            node.Initialize(this, pos, data);

            #region GraphView 视图操作
            // 添加进当前主GraphView视图中
            this.AddElement(node);

            // 刷新GraphView视图
            node.RefreshExpandedState();
            node.RefreshPorts();
            #endregion

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();

            return node;
        }
        /// <summary>
        /// 创建中继节点
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public VNode_Relay Node_MakeRelay(Vector2 pos, ActionNode_Base data = null)
        {
            if (data.visualNodeType == "None")
                return null;

            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.{data.visualNodeType}");
            // 创建 NodeView 类型的实例为 visualNode 基类
            VNode_Relay relay = Activator.CreateInstance(type_nodeview) as VNode_Relay;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            relay.Initialize(this, pos, data);


            // 刷新节点主题配色
            RefreshTheme_GraphNode(relay);

            #region GraphView 视图操作
            // 添加进当前主GraphView视图中
            this.AddElement(relay);

            // 指定生成的节点点击事件委托，便于实现调用点击节点时调用
            relay.OnSelectedNode = OnSelectedNode;
            relay.OnUnSelectedNode = OnUnSelectedNode;
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
            VNode_Base visualNode = Node_MakeAction(args.position, ActionTreeAsset.Create(args));

            // 刷新节点
            visualNode.Draw();
            visualNode.RefreshExpandedState();
            visualNode.RefreshPorts();
            visualNode.CheckTransparentDisplay(args.transparentNode);
            visualNode.CheckAvatarChanged();

            return visualNode;
        }

        /// <summary>
        /// 创建节点 - 便签
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Node CreateNode(NodeCreateArgs_Stick args)
        {
            // 便签节点创建，便签类是不需要加入行为树根资源中的，而是加入到行为树根资源的 StickNoteDatas 变量中
            Undo.RecordObject(ActionTreeAsset, "Create StickNode");
            // 新建行为树便签内容加入到行为树根资源的 StickNoteDatas 变量中
            ActionStickData stickdata = new ActionStickData(args.stickName, args.stickContent, GUID.Generate().ToString(), args.position, args.size);
            ActionTreeAsset.StickNote_Add(stickdata);

            // 创建新的节点并指定资源数据项
            VNode_Stick stickNode = Node_MakeStick(args.position, stickdata);

            // 刷新节点
            stickNode.Draw();
            stickNode.RefreshExpandedState();
            stickNode.RefreshPorts();

            return stickNode;
        }

        /// <summary>
        /// 创建节点 - 贴图
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Node CreateNode(NodeCreateArgs_Decal args)
        {
            // 贴图节点创建，贴图类是不需要加入行为树根资源中的，而是加入到行为树根资源的 DecalDatas 变量中
            Undo.RecordObject(ActionTreeAsset, "Create DecalNode");
            // 新建行为树贴图内容加入到行为树根资源的 DecalDatas 变量中
            ActionDecalData decaldata = new ActionDecalData(GUID.Generate().ToString(), args.position, args.size, args.scale, args.opacity, args.hasTexture, args.decalTexture);
            ActionTreeAsset.Decal_Add(decaldata);

            // 创建新的节点并指定资源数据项
            VNode_Decal decalNode = Node_MakeDecal(args.position, decaldata);

            // 刷新节点
            decalNode.Draw();
            decalNode.RefreshExpandedState();
            decalNode.RefreshPorts();

            return decalNode;
        }
    }
}