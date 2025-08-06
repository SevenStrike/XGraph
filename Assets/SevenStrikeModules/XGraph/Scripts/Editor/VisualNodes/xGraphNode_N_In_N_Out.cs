namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class xGraphNode_N_In_N_Out : xGraphNode_Base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionTree_Node_Base data = null)
        {
            base.Initialize(graphView, pos, data);
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

            //// 绘制顶部容器
            //Draw_Top();

            //// 绘制输入节点容器
            //Draw_Input();

            //// 绘制输出节点容器
            //Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }
        #endregion
    }
}