namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class visualnode_s_in_1port_s_out_1port : visualnode_base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, actionnode_base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口设置
            xGraph_NodePort port_in = new xGraph_NodePort("in", typeof(bool), Port.Capacity.Single);
            SetPort_Input(port_in);

            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            port_out.Add(new xGraph_NodePort("out", typeof(bool), Port.Capacity.Single));
            SetPort_Output(port_out);
            #endregion

            #region 注册GraphView的变化状态
            RegisterGraphViewChanged(graphView);
            #endregion
        }

        #region GraphView视图操作有改变时
        /// <summary>
        /// 注册GraphView的变化状态
        /// </summary>
        /// <param name="graphView"></param>
        private void RegisterGraphViewChanged(xg_GraphView graphView)
        {
            graphView.graphViewChanged -= OnGraphViewChanged;
            graphView.graphViewChanged += OnGraphViewChanged;
        }
        /// <summary>
        /// 当GraphView视图内有变化时
        /// </summary>
        /// <param name="graphViewChange"></param>
        public GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            On_RemovedElement(graphViewChange);
            On_CreateEdge(graphViewChange);
            return graphViewChange;
        }
        /// <summary>
        /// 当GraphView视图内有变化  -  移除了元素时（含所有元素类型：节点、组、连线等）
        /// </summary>
        /// <param name="graphViewChange"></param>
        private void On_RemovedElement(GraphViewChange graphViewChange)
        {
            // 当有元素被移除时
            if (graphViewChange.elementsToRemove != null)
            {
                // 遍历被移除的元素
                graphViewChange.elementsToRemove.ForEach(element =>
                {
                    // 如果移除的元素为：连线
                    Remove_Edge(element);

                    // 如果移除的元素为：节点
                    Remove_Node(element);
                });
            }
        }
        /// <summary>
        /// 当GraphView视图内有变化  -  创建了连线时
        /// </summary>
        /// <param name="graphViewChange"></param>
        private void On_CreateEdge(GraphViewChange graphViewChange)
        {
            if (graphViewChange.edgesToCreate != null)
            {
                // 如果创建了连线
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    // 如果该视觉节点的行为数据类型为： debug
                    if (ActionNode is actionnode_debug debug)
                    {
                        visualnode_base n_parent = edge.output.node as visualnode_base;
                        // edge 连线的子级节点
                        visualnode_base n_child = edge.input.node as visualnode_base;

                        if (n_parent.ActionNode.guid == debug.guid && n_child != null)
                        {
                            Undo.RecordObject(graphView.ActionTreeAsset, "Create_Debug_ChildNode");
                            // 将连线的 input 端赋值给 debug 的子节点
                            debug.ChildNode = n_child.ActionNode;
                        }
                    }
                });
            }
        }
        /// <summary>
        /// GraphView 移除连线时
        /// </summary>
        /// <param name="element"></param>
        private void Remove_Edge(GraphElement element)
        {
            Edge edge = element as Edge;
            if (edge != null)
            {
                #region 判断连线源是否是行为树资源节点
                // 如果该视觉节点的行为数据类型为： debug
                if (ActionNode is actionnode_debug debug)
                {
                    // 连线的父级节点
                    visualnode_base n_parent = edge.output.node as visualnode_base;

                    if (n_parent != null)
                    {
                        // 如果删除的连线的 output 端的节点的行为 Guid 等于 debug 的Guid
                        if (n_parent.ActionNode.guid == debug.guid)
                        {
                            Undo.RecordObject(debug.ChildNode, "Remove_Debug_ChildNode");
                            // 清空 debug 的子节点
                            debug.ChildNode = null;
                        }
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// GraphView 移除节点时
        /// </summary>
        /// <param name="element"></param>
        private void Remove_Node(GraphElement element)
        {
            visualnode_base remove_node = element as visualnode_base;
            if (remove_node != null)
            {
                // 如果删除的节点 Guid 等于 自身的Guid就从行为树根资源中移除该节点资源
                if (remove_node.ActionNode.guid == ActionNode.guid)
                {
                    Undo.RecordObject(graphView.ActionTreeAsset, "Remove_Debug_Node");
                    graphView.ActionTreeAsset.Remove(ActionNode);
                    return;
                }

                // 如果该视觉节点的行为数据类型为： debug
                if (ActionNode is actionnode_debug debug)
                {
                    // 如果删除的节点 Guid 等于 debug 的子节点的Guid
                    if (debug.ChildNode != null && remove_node.ActionNode.guid == debug.ChildNode.guid)
                    {
                        Undo.RecordObject(debug.ChildNode, "Remove_Debug_ChildNode");
                        // 清空 debug 的子节点
                        debug.ChildNode = null;
                    }
                }
            }
        }
        #endregion

        #region 节点绘制
        public override visualnode_base Draw()
        {
            // 绘制主容器
            Draw_Main();

            // 绘制标题容器
            Draw_Title();

            // 绘制标题按钮容器
            Draw_TitleButton();

            // 绘制顶部容器
            Draw_Top();

            // 绘制输入节点容器
            Draw_Input();

            // 绘制输出节点容器
            Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }
        #endregion
    }
}