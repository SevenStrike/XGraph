namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Debug : xNode_Base
    {
        internal xAction_Debug debug;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口 - 输入
            List<xGraph_NodePort> port_in = new List<xGraph_NodePort>();
            port_in.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Single));// 加入行为端口（输入）
            port_in.Add(new xGraph_NodePort("前缀", typeof(Variable), Port.Capacity.Single));// 加入变量端口（输入）
            port_in.Add(new xGraph_NodePort("对象", typeof(Variable), Port.Capacity.Single));// 加入变量端口（输入）
            InputPort_Set(port_in);
            #endregion

            #region 端口 - 输出
            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            port_out.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Multi));// 加入行为端口（输出）
            OutputPort_Set(port_out);
            #endregion

            debug = data as xAction_Debug;
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
            Draw_Extension();

            return this;
        }
        #endregion

        #region 重写 - 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            debug.DebugMessage_withPort("对象");
            debug.SetPrefix_withPort("前缀");
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
            fold.text = $"{fold.text}（{debug.childNodes.Count}）";

            for (int i = 0; i < debug.childNodes.Count; i++)
            {
                xAction_Base child = debug.BaseArgs.RootAsset.FindActionNode(debug.childNodes[i]);

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
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout ins_Folder_Extensions(VisualElement root)
        {
            Foldout fold = base.ins_Folder_Extensions(root);

            #region 调试前缀
            TextField field_prefix = util_XGraphInspectorGUI.GUI_Field_String(fold, "前缀", debug.Prefix, new string[] { "field_text" });
            field_prefix.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                // 如果该节点已经存在绑定的变量，则将当前序列化值给到控件值，因为该序列化值已受变量控制
                if (isVariableBinded("前缀"))
                {
                    field_prefix.value = debug.Prefix;
                    debug.SetPrefix_withPort("前缀");
                }
                // 否则就代表没有任何变量节点接入，而使用控件值给到序列化属性值
                else
                {
                    debug.SetPrefix(field_prefix.value);
                }
            });
            #endregion

            #region 调试内容
            Label label_msg = util_XGraphInspectorGUI.GUI_Label(fold, $"对象内容：{debug.Msg}", new string[] { "field_text" });
            #endregion

            // 当节点绑定变量时，将变量值同步到控件值
            debug.On_Node_Variable_Binded += (vare) =>
            {
                label_msg.text = debug.Msg;
                field_prefix.value = debug.Prefix;
            };

            // 克隆节点后刷新控件值为克隆后的最新值
            debug.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                xAction_Debug s_source = (xAction_Debug)source;
                // 克隆后的行为数据
                xAction_Debug s_clone = (xAction_Debug)clone;

                s_clone.Msg = s_source.Msg;
                s_clone.Prefix = s_source.Prefix;

                label_msg.text = debug.Msg;
                field_prefix.value = debug.Prefix;
            };

            return fold;
        }
        #endregion
    }
}