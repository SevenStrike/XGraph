namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Wait : xNode_Base
    {
        internal xAction_Wait wait;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口 - 输入
            List<xGraph_NodePort> port_in = new List<xGraph_NodePort>();
            port_in.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Single));// 加入行为端口（输入）
            port_in.Add(new xGraph_NodePort("时间", typeof(Variable_Float), Port.Capacity.Single));// 加入变量端口（输入）
            InputPort_Set(port_in);
            #endregion

            #region 端口 - 输出
            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            port_out.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Multi));// 加入行为端口（输出）
            OutputPort_Set(port_out);
            #endregion

            wait = data as xAction_Wait;
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

            wait.SetWaitTime("时间");
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
            fold.text = $"{fold.text}（{wait.childNodes.Count}）";

            for (int i = 0; i < wait.childNodes.Count; i++)
            {
                xAction_Base child = wait.BaseArgs.RootAsset.FindActionNode(wait.childNodes[i]);

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

            #region 等待时间
            FloatField field_time = util_XGraphInspectorGUI.GUI_Field_Float(fold, "时间", wait.Time, new string[] { "field_float" });
            field_time.RegisterCallback<ChangeEvent<float>>((evt) =>
            {
                if (isVariableBinded("时间"))
                {
                    field_time.value = wait.Time;
                    wait.SetWaitTime("时间");
                }
                else
                {
                    wait.SetWaitTime(field_time.value);
                }
            });
            #endregion

            // 当根行为资源绑定变量时
            wait.On_Node_Variable_Binded += (var) =>
            {
                field_time.value = wait.Time;
            };

            // 克隆节点后刷新控件值为克隆后的最新值
            wait.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                xAction_Wait s_source = (xAction_Wait)source;
                // 克隆后的行为数据
                xAction_Wait s_clone = (xAction_Wait)clone;
                s_clone.Time = s_source.Time;

                field_time.value = wait.Time;
            };

            return fold;
        }
        #endregion
    }
}