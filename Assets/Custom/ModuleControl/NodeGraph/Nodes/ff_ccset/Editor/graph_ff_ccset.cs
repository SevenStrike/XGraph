namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class graph_ff_ccset : xNode_Property
    {
        action_ff_ccset ccset;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

              #region 端口设置
                List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
                // 加入行为端口
                port_out.Add(new xGraph_NodePort("属性", typeof(Variable_Float), Port.Capacity.Multi));
                OutputPort_Set(port_out);
                #endregion     

            ccset = ActionData as action_ff_ccset;
        }

        #region 重写
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();
        }
        /// <summary>
        /// 当克隆节点时
        /// </summary>
        /// <param name="list"></param>
        public override void On_Nodes_Duplicated(List<DuplicateNodeData> list)
        {
            base.On_Nodes_Duplicated(list);
        }
        /// <summary>
        /// 当节点重建时
        /// </summary>
        public override void On_Node_Restructure()
        {
            base.On_Node_Restructure();
        }
        /// <summary>
        /// 当节点连线时
        /// </summary>
        /// <param name="edge"></param>
        public override void On_Node_CreateEdge(Edge edge)
        {
            base.On_Node_CreateEdge(edge);
        }
        /// <summary>
        /// 当节点移除连线时
        /// </summary>
        /// <param name="edge"></param>
        public override void On_Node_RemovedEdge(Edge edge)
        {
            base.On_Node_RemovedEdge(edge);
        }
        /// <summary>
        /// 当节点头像改变时
        /// </summary>
        /// <param name="tex"></param>
        public override void On_Node_AvatarChanged(Texture2D tex)
        {
            base.On_Node_AvatarChanged(tex);
        }
        /// <summary>
        /// 当节点执行模式改变时
        /// </summary>
        /// <param name="state"></param>
        public override void On_Node_ConcurrentChanged(bool state)
        {
            base.On_Node_ConcurrentChanged(state);
        }
        /// <summary>
        /// 当改变节点图标时
        /// </summary>
        /// <param name="tex"></param>
        public override void On_Node_IconChanged(Texture2D tex)
        {
            base.On_Node_IconChanged(tex);
        }
        /// <summary>
        /// 当改变节点颜色主题时
        /// </summary>
        public override void On_Node_ThemeColorChanged()
        {
            base.On_Node_ThemeColorChanged();
        }
        /// <summary>
        /// 当改变节点通透模式时
        /// </summary>
        /// <param name="state"></param>
        public override void On_Node_TransparentChanged(bool state)
        {
            base.On_Node_TransparentChanged(state);
        }
        /// <summary>
        /// 当改变节点尺寸时
        /// </summary>
        /// <param name="evt"></param>
        public override void OnSizeChanged(GeometryChangedEvent evt)
        {
            base.OnSizeChanged(evt);
        }
        /// <summary>
        /// 当选中节点时
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
        }
        /// <summary>
        /// 当取消选中节点时
        /// </summary>
        public override void OnUnselected()
        {
            base.OnUnselected();
        }
        #endregion
    }
}