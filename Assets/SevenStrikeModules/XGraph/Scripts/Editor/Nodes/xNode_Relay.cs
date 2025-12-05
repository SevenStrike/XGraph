namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Relay : xNode_Base
    {
        internal xAction_Relay relay;
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

            #region 端口 - 输入
            List<xGraph_NodePort> port_in = new List<xGraph_NodePort>();
            port_in.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Single));// 加入行为端口（输入）
            InputPort_Set(port_in);
            #endregion

            #region 端口 - 输出
            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            port_out.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Multi));// 加入行为端口（输出）
            OutputPort_Set(port_out);
            #endregion

            relay = ActionData as xAction_Relay;
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

        #region 重写 - 绘制Inspector
        /// <summary>
        /// 子行为折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout ins_Folder_ChildActions(VisualElement root)
        {
            Foldout fold = base.ins_Folder_ChildActions(root);
            fold.text = $"{fold.text}（{relay.childNodes.Count}）";

            for (int i = 0; i < relay.childNodes.Count; i++)
            {
                xAction_Base child = relay.BaseArgs.RootAsset.FindActionNode(relay.childNodes[i]);

                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);

                container.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(child.BaseArgs.guid);
                    if (node is xNode_Base n_base)
                    {
                        n_base.Highlight();
                    }
                });

                container.RegisterCallback<PointerLeaveEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(child.BaseArgs.guid);
                    if (node is xNode_Base n_base)
                    {
                        n_base.UnHighlight();
                    }
                });

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = child.BaseArgs.NodeIcon == null ? util_XGraphEditorUtility.AssetLoad<Texture2D>(AssetDatabase.GUIDToAssetPath(child.BaseArgs.icon)) : child.BaseArgs.NodeIcon;
                container_title.Add(container_icon);

                util_XGraphInspectorGUI.GUI_Label(container_title, $"目标：{child.identifyName}", new string[] { "labeltext", "list_item_title" });
                util_XGraphInspectorGUI.GUI_Label(container_title, "行为", new string[] { "list_item_marktext" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>Guid：</b><color=#e1e1e1>{child.BaseArgs.guid}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>行为类型：</b><color=#e1e1e1>{child.BaseArgs.actionNodeType}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点类型：</b><color=#e1e1e1>{child.BaseArgs.visualNodeType}</color>", new string[] { "list_item_label" });
            }

            return fold;
        }
        /// <summary>
        /// 属性节点的属性项折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout ins_Folder_Propertys(VisualElement root)
        {
            return null;
        }
        /// <summary>
        /// 属性记录折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout ins_Folder_BindedPropertys(VisualElement root)
        {
            return null;
        }
        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout ins_Folder_Extensions(VisualElement root)
        {
            return null;
        }
        /// <summary>
        /// 内部变量组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout ins_Folder_InternalVariable(VisualElement root)
        {
            return null;
        }
        /// <summary>
        /// 黑板变量组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout ins_Folder_BlackBoardVariable(VisualElement root)
        {
            return null;
        }
        #endregion
    }
}