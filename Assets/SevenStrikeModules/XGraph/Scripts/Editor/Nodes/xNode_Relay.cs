namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Relay : xNode_Base
    {
        public Texture2D tex_logo_disconnected;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            tex_logo_disconnected = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/emptyrelay.png");

            // 设置节点的容器样式
            util_XGraphEditorUtility.ElementStyle_Add(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_RelayNode.uss");

            VisualElement nodeborder = this.Q<VisualElement>("node-border");
            nodeborder.AddToClassList("node_nodeborder");

            VisualElement selectionborder = this.Q<VisualElement>("selection-border");
            selectionborder.AddToClassList("node_selectionborder");

            #region 端口设置
            List<xGraph_NodePort> port_in = new List<xGraph_NodePort>();
            // 加入行为端口
            port_in.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Single));
            InputPort_Set(port_in);

            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            // 加入行为端口
            port_out.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Multi));
            OutputPort_Set(port_out);
            #endregion
        }

        /// <summary>
        /// 检查端口连线状态
        /// </summary>
        public virtual void CheckConnected()
        {
            Port pi = util_XGraphEditorUtility.GetPort_WithType_OfPortList<xAction_Base>(Port_Inputs);
            Port po = util_XGraphEditorUtility.GetPort_WithType_OfPortList<xAction_Base>(Port_Outputs);

            if (pi.connected && po.connected)
                Connected();
            else
                Disconnected();
        }

        public virtual void Connected()
        {
            CheckExecutionModel();
        }

        public virtual void Disconnected()
        {
            ExecutionIcon.style.backgroundImage = tex_logo_disconnected;
        }

        #region 节点绘制
        public override xNode_Base Draw()
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
            ExecutionIcon = new Label("");
            ExecutionIcon.AddToClassList("Title_Icon");
            ExecutionIcon.style.backgroundImage = tex_logo_dir_sequential;

            divider.Add(ExecutionIcon);
        }

        public override void Draw_Main()
        {
            mainContainer.style.overflow = new StyleEnum<Overflow>(Overflow.Visible);

            CreateHighlighter();
        }
        #endregion
    }
}