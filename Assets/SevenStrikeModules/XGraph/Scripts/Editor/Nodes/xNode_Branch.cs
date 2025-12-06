namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Branch : xNode_Base
    {
        internal xAction_Branch branch;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口设置
            // 加入行为端口
            Port_Inputs.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Single));
            Port_Inputs.Add(new xGraph_NodePort("条件", typeof(Variable_Bool), Port.Capacity.Single));
            Port_Outputs.Add(new xGraph_NodePort("开", typeof(xAction_Base), Port.Capacity.Single));
            Port_Outputs.Add(new xGraph_NodePort("关", typeof(xAction_Base), Port.Capacity.Single));
            #endregion          

            branch = data as xAction_Branch;
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

        public override void Draw_Output()
        {
            outputContainer.AddToClassList("branchOutputOffset");
            base.Draw_Output();
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

        #region 重写 - 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            branch.SetPredicateState("条件");
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
            fold.text = $"{fold.text}";

            xAction_Base child_true = branch.BaseArgs.RootAsset.FindActionNode(branch.childNode_true);

            VisualElement container = new VisualElement();
            container.AddToClassList("list_container");
            fold.Add(container);

            container.RegisterCallback<PointerEnterEvent>((evt) =>
            {
                xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                Node node = wnd.xw_graphView.FindNode(child_true.BaseArgs.guid);
                if (node is xNode_Base n_base)
                {
                    n_base.Highlight();
                }
            });

            container.RegisterCallback<PointerLeaveEvent>((evt) =>
            {
                xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                Node node = wnd.xw_graphView.FindNode(child_true.BaseArgs.guid);
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
            container_icon.style.backgroundImage = child_true.BaseArgs.NodeIcon == null ? util_XGraphEditorUtility.AssetLoad<Texture2D>(AssetDatabase.GUIDToAssetPath(child_true.BaseArgs.icon)) : child_true.BaseArgs.NodeIcon;
            container_title.Add(container_icon);

            util_XGraphInspectorGUI.GUI_Label(container_title, $"目标：{child_true.identifyName}", new string[] { "labeltext", "list_item_title" });
            util_XGraphInspectorGUI.GUI_Label(container_title, "行为", new string[] { "list_item_marktext" });
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>Guid：</b><color=#e1e1e1>{child_true.BaseArgs.guid}</color>", new string[] { "list_item_label" });
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>行为类型：</b><color=#e1e1e1>{child_true.BaseArgs.actionNodeType}</color>", new string[] { "list_item_label" });
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点类型：</b><color=#e1e1e1>{child_true.BaseArgs.visualNodeType}</color>", new string[] { "list_item_label" });

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
            return null;
        }
        #endregion
    }
}