namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class Base_GraphNode_Debug : VNode_Base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口设置
            List<xGraph_NodePort> port_in = new List<xGraph_NodePort>();
            // 加入行为端口
            port_in.Add(new xGraph_NodePort("in", typeof(ActionNode_Base), Port.Capacity.Single));
            // 加入变量端口
            port_in.Add(new xGraph_NodePort("string-s", typeof(Variable_String), Port.Capacity.Single));
            // 加入变量端口
            port_in.Add(new xGraph_NodePort("string-t", typeof(Variable_String), Port.Capacity.Single));
            // 加入变量端口
            port_in.Add(new xGraph_NodePort("bool", typeof(Variable_Bool), Port.Capacity.Single));
            // 加入变量端口
            port_in.Add(new xGraph_NodePort("color", typeof(Variable_Color), Port.Capacity.Single));
            // 加入变量端口
            port_in.Add(new xGraph_NodePort("float", typeof(Variable_Float), Port.Capacity.Single));
            InputPort_Set(port_in);

            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            // 加入行为端口
            port_out.Add(new xGraph_NodePort("out", typeof(ActionNode_Base), Port.Capacity.Single));
            OutputPort_Set(port_out);
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

            // 绘制输出节点容器
            Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }

        public override void Draw_Input()
        {
            base.Draw_Input();
        }
        #endregion
    }
}