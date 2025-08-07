namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class visualnode_n_in_s_out_1port : visualnode_base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, actionnode_base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口设置
            List<xGraph_NodePort> ports_out = new List<xGraph_NodePort>();

            xGraph_NodePort port_out = new xGraph_NodePort("out", typeof(bool), Port.Capacity.Single);

            ports_out.Add(port_out);

            SetPort_Output(ports_out);
            #endregion

            #region 注册GraphView的变化状态
            graphView.graphViewChanged -= OnGraphViewChanged;
            graphView.graphViewChanged += OnGraphViewChanged;
            #endregion
        }

        /// <summary>
        /// 当GraphView视图内有变化时
        /// </summary>
        /// <param name="graphViewChange"></param>
        public GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            Debug.Log("ChangedXSD");
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
        /// GraphView 移除连线时
        /// </summary>
        /// <param name="element"></param>
        private void Remove_Edge(GraphElement element)
        {
            Edge edge = element as Edge;
            if (edge != null)
            {
                #region 判断连线源是否是行为树资源节点
                // 连线的父级节点
                visualnode_base n_parent = edge.output.node as visualnode_base;

                if (n_parent != null)
                {
                    // 如果该视觉节点的行为数据类型为： start
                    if (ActionNode is actionnode_start start)
                    {
                        // 如果删除的连线的 output 端的节点的行为 Guid 等于 start 的Guid
                        if (n_parent.ActionNode.nodeGUID == start.nodeGUID)
                        {
                            // 清空 start 的子节点
                            start.ChildNode = null;
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
            visualnode_base visualnode = element as visualnode_base;
            if (visualnode != null)
            {
                // 如果该视觉节点的行为数据类型为： start
                if (ActionNode is actionnode_start start)
                {
                    // 如果删除的节点 Guid 等于 start 的子节点的Guid
                    if (start.ChildNode != null && visualnode.ActionNode.nodeGUID == start.ChildNode.nodeGUID)
                    {
                        // 清空 start 的子节点
                        start.ChildNode = null;
                    }
                }
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
                // 如果创建了连线，edge 为连线
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    // edge 连线的子级节点
                    visualnode_base n_child = edge.input.node as visualnode_base;

                    if (n_child != null)
                    {
                        // 如果该视觉节点的行为数据类型为： start
                        if (ActionNode is actionnode_start start)
                        {
                            // 将连线的 input 端赋值给 start 的子节点
                            start.ChildNode = n_child.ActionNode;
                        }
                    }
                });
            }
        }

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

            //// 绘制输入节点容器
            //Draw_Input();

            // 绘制输出节点容器
            Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }
        #endregion
    }
}