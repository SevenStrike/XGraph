namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class visualnode_s_in_1port_n_out : visualnode_base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, actionnode_base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口设置
            xGraph_NodePort port_in = new xGraph_NodePort("in", typeof(bool), Port.Capacity.Single);
            SetPort_Input(port_in);
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
                    // 如果移除的元素为：节点
                    Remove_Node(element);
                });
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
                    Undo.RecordObject(graphView.ActionTreeAsset, "Remove_End_Node");
                    graphView.ActionTreeAsset.Remove(ActionNode);
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

            //// 绘制输出节点容器
            //Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }
        #endregion
    }
}