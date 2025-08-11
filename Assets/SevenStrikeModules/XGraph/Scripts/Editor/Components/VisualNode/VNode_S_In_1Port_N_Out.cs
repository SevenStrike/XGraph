namespace SevenStrikeModules.XGraph
{
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class VNode_S_In_1Port_N_Out : VNode_Base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            // 设置节点的容器样式
            SetNodeStyle("uss_Node");

            #region 端口设置
            xGraph_NodePort port_in = new xGraph_NodePort("in", typeof(bool), Port.Capacity.Single);
            SetPort_Input(port_in);
            #endregion
        }

        #region 节点绘制
        public override VNode_Base Draw()
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