namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class VNode_Branch : VNode_Base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            ActionNode_Branch actionvbranch = data as ActionNode_Branch;

            #region 端口设置
            // 加入行为端口
            Port_Inputs.Add(new xGraph_NodePort("", typeof(ActionNode_Base), Port.Capacity.Single));
            Port_Inputs.Add(new xGraph_NodePort("条件", typeof(Variable), Port.Capacity.Single));
            Port_Outputs.Add(new xGraph_NodePort("开", typeof(ActionNode_Base), Port.Capacity.Single));
            Port_Outputs.Add(new xGraph_NodePort("关", typeof(ActionNode_Base), Port.Capacity.Single));
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

        public override void Draw_Main()
        {
            base.Draw_Main();
        }

        public override void Draw_Extension()
        {
            //base.Draw_Extension();
        }

        public override void Draw_Title()
        {
            base.Draw_Title();
        }
        #endregion     

        #region 辅助

        #endregion       
    }
}