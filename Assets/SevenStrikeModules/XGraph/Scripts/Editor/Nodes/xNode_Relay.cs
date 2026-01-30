/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGraph 行为流程图插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
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

            CreateIsStartNodeMark();
        }
        #endregion
    }
}