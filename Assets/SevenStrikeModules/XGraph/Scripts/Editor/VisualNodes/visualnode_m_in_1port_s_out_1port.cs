namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class visualnode_m_in_1port_s_out_1port : visualnode_base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, actionnode_base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口设置
            xGraph_NodePort port_in = new xGraph_NodePort("in", typeof(bool), Port.Capacity.Multi);
            SetPort_Input(port_in);

            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            port_out.Add(new xGraph_NodePort("out", typeof(bool), Port.Capacity.Single));
            SetPort_Output(port_out);
            #endregion
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