namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class VNode_Relay : VNode_Base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            // 设置节点的容器样式
            SetNodeStyle("uss_RelayNode");

            #region 端口设置
            xGraph_NodePort port_in = new xGraph_NodePort("Ri", typeof(bool), Port.Capacity.Single);
            SetPort_Input(port_in);

            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            port_out.Add(new xGraph_NodePort("Ro", typeof(bool), Port.Capacity.Multi));
            SetPort_Output(port_out);
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
            //Draw_Extension();

            return this;
        }

        public override void Draw_Top()
        {
            VisualElement divider = topContainer.Q<VisualElement>("divider");
            IconLabel = new Label("");
            IconLabel.AddToClassList("Title_Icon");
            IconLabel.style.backgroundImage = util_EditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{this.icon}.png");
            // 应用配置文件的颜色到节点的标识颜色
            graphView.ThemesList.Node.ForEach(colorData =>
            {
                if (colorData.solution == ActionNode.themeSolution)
                {
                    IconLabel.style.unityBackgroundImageTintColor = ActionNode.themeSolution == "M 默认" ? Color.white : ActionNode.themeColor;
                }
            });
            divider.Add(IconLabel);

            base.Draw_Top();
        }
        #endregion
    }
}