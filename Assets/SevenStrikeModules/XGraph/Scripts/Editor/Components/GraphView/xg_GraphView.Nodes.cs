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
        }
        /// <summary>
        /// 实现视觉节点复制逻辑
        /// </summary>
        public void Node_Duplicate()
        {
            var selectedNodes = selection.OfType<Node>().ToList();
            if (selectedNodes.Count == 0) return;

            ClearSelection(); // 清空当前选择（可选）

            foreach (var original in selectedNodes)
            {
                if (original is VNode_Base xg_base)
                {
                    // 待复制的源节点
                    ActionNode_Base node = xg_base.ActionNode;

                    // 待复制的源节点的命名空间
                    string prefix_namespace = node.GetType().Namespace;
                    // 待复制的源节点的类名
                    string class_name = node.GetType().Name;
                    // 待复制的源节点的行为节点名
                    string actiontype_name = node.actionNodeType.ToString();
                    // 待复制的源节点的类名的前缀
                    string prefix_class = class_name.Substring(0, class_name.Length - actiontype_name.Length);

                    //Debug.Log($"{prefix_namespace} - {class_name} - {actiontype_name} - {prefix_class}");

                    // 克隆出新的节点
                    Node_Create(node.identifyName, prefix_namespace, prefix_class, node.actionNodeType, node.icon, node.visualNodeType);
                }
                else if (original is VNode_Stick xg_sticknote)
                {
                    stickdata sickData = xg_sticknote.stickNoteData.Clone(true);
                    Undo.RecordObject(ActionTreeAsset, "Duplicate StickNoteData");
                    ActionTreeAsset.StickNoteDatas.Add(sickData);

                    Node_MakeStick(sickData.position + new Vector2(50, 20), sickData).Draw();
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
                RemoveElement(node);
            }
        }
        /// <summary>
        /// 创建视觉节点
        /// </summary>
        /// <param h_name="pos"></param>
        /// <param h_name="dat"></param>
        /// <returns></returns>
        public VNode_Base Node_Make(Vector2 pos, ActionNode_Base data = null)
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
            return node;
        }
        /// <summary>
        /// 创建视觉节点 - 便签
        /// </summary>
        /// <param h_name="pos"></param>
        /// <returns></returns>
        public VNode_Stick Node_MakeStick(Vector2 pos, stickdata data = null)
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
            return node;
        }
        /// <summary>
        /// 创建中继节点
        /// </summary>
        /// <param name="node_base"></param>
        public VNode_Relay Node_MakeRelay(Vector2 pos, ActionNode_Base data)
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

            return relay;
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
        /// 创建行为树节点
        /// </summary>
        /// <param h_name="prefix_namespace"></param>
        /// <param h_name="prefix_class"></param>
        /// <param h_name="type"></param>
        /// <param h_name="action_nodeType"></param>
        /// <param h_name="visual_nodeType"></param>
        /// <param h_name="action_name"></param>
        /// <returns></returns>
        public ActionNode_Base InstantiateActionNode(string prefix_namespace, string prefix_class, Type type, string action_nodeType, string icon, string visual_nodeType, string action_name)
        {
            ActionNode_Base data = ScriptableObject.CreateInstance(type) as ActionNode_Base;
            data.name = type.Name;
            data.guid = GUID.Generate().ToString();
            data.actionNodeType = action_nodeType;
            data.icon = icon;
            data.visualNodeType = visual_nodeType;
            data.identifyName = action_name;
            data.namespaces = prefix_namespace;
            data.classes = prefix_class;
            ActionTreeAsset.Create(data);
            return data;
        }

        /// <summary>
        /// 创建包装节点
        /// </summary>
        /// <param name="visualName"></param>
        /// <param name="prefix_namespace"></param>
        /// <param name="prefix_class"></param>
        /// <param name="action_nodeType"></param>
        /// <param name="icon"></param>
        /// <param name="visual_nodeType"></param>
        public void Node_Create(string visualName, string prefix_namespace, string prefix_class, string action_nodeType, string icon, string visual_nodeType)
        {
            // 便签类是不需要加入行为树根资源中的，而是加入到行为树根资源的 StickNoteDatas 变量中
            if (action_nodeType == "Stick")
            {
                Undo.RecordObject(ActionTreeAsset, "Create StickNote");
                // 新建行为树便签内容加入到行为树根资源的 StickNoteDatas 变量中
                stickdata stdata = new stickdata("便签", "点击此处更改内容", GUID.Generate().ToString(), gv_NodeCreatedPosition, new Vector2(100, 100));
                ActionTreeAsset.StickNote_Add(stdata);

                // 创建新的节点并指定资源数据项
                VNode_Stick visualstickNode = Node_MakeStick(gv_NodeCreatedPosition, stdata);

                // 刷新节点
                visualstickNode.Draw();
                visualstickNode.RefreshExpandedState();
                visualstickNode.RefreshPorts();
            }
            else
            {
                string asm = typeof(ActionNode_Base).Assembly.FullName;
                // 目标类名字是拼接的，通过命名空间 + 类前缀 + 通用的 xg_ActionTreeType 枚举类型
                // asm: 作用域
                // ex:  SevenStrikeModules.XGraph + ActionTree_ + Start  = SevenStrikeModules.XGraph.ActionTree_Start.cs

                Type scriptType_Actiontree = Type.GetType($"{prefix_namespace}.{prefix_class}{action_nodeType},{asm}", true);

                // 在行为树根资源中加入新的数据项
                ActionNode_Base database = InstantiateActionNode(
                    prefix_namespace,
                    prefix_class,
                    scriptType_Actiontree,
                    action_nodeType,
                    icon,
                    visual_nodeType,
                    visualName);

                // 创建新的节点并指定资源数据项
                VNode_Base visualNode = Node_Make(gv_NodeCreatedPosition, database);

                // 刷新节点
                visualNode.Draw();
                visualNode.RefreshExpandedState();
                visualNode.RefreshPorts();

                // 选中新节点
                AddToSelection(visualNode);
            }
        }
    }
}