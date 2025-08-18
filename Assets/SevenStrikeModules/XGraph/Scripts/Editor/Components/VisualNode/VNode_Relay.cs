namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class VNode_Relay : VNode_Base
    {
        public Texture2D tex_logo_connected;
        public Texture2D tex_logo_disconnected;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            tex_logo_connected = util_EditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{this.icon}.png");
            tex_logo_disconnected = util_EditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/emptyrelay.png");

            // 设置节点的容器样式
            SetNodeStyle("uss_RelayNode");

            VisualElement nodeborder = this.Q<VisualElement>("node-border");
            nodeborder.AddToClassList("node_nodeborder");

            VisualElement selectionborder = this.Q<VisualElement>("selection-border");
            selectionborder.AddToClassList("node_selectionborder");

            #region 端口设置
            xGraph_NodePort port_in = new xGraph_NodePort("", typeof(bool), Port.Capacity.Single);
            SetPort_Input(port_in);

            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            port_out.Add(new xGraph_NodePort("", typeof(bool), Port.Capacity.Multi));
            SetPort_Output(port_out);
            #endregion
        }

        public void CheckConnected()
        {
            if (Port_Input.Port.connected)
                Connected();
            else
                Disconnected();
        }

        public void Connected()
        {
            IconLabel.style.backgroundImage = tex_logo_connected;
        }

        public void Disconnected()
        {
            IconLabel.style.backgroundImage = tex_logo_disconnected;
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
            IconLabel.style.backgroundImage = tex_logo_connected;

            divider.Add(IconLabel);
        }
        #endregion
    }
}