namespace SevenStrikeModules.XGraph
{
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class xGraphNode_M_In_S_Out : xGraphNode_Base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionTree_Node_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 类型指定
            nodeType = xg_GraphViewNode.M_In_S_Out;
            #endregion

            #region 端口设置
            xGraph_NodePort port_info_in = new xGraph_NodePort("输入端", typeof(bool), Direction.Input, Port.Capacity.Multi);
            xGraph_NodePort port_info_out = new xGraph_NodePort("输出端", typeof(bool), Direction.Output, Port.Capacity.Single);
            SetPortInfo(port_info_in);
            SetPortInfo(port_info_out);
            #endregion
        }

        #region 节点绘制
        public override xGraphNode_Base Draw()
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