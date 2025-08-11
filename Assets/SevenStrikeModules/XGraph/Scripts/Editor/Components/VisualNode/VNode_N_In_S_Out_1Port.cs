namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class VNode_N_In_S_Out_1Port : VNode_Base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            // 设置节点的容器样式
            SetNodeStyle("uss_Node");

            #region 端口设置
            List<xGraph_NodePort> ports_out = new List<xGraph_NodePort>();

            xGraph_NodePort port_out = new xGraph_NodePort("out", typeof(bool), Port.Capacity.Single);

            ports_out.Add(port_out);

            SetPort_Output(ports_out);
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