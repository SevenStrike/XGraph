namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(ActionNode_Branch))]
    public class Editor_ActionNode_Branch : Editor_ActionNode_Base
    {
        private ActionNode_Branch actionScript;

        public override void OnEnable()
        {
            base.OnEnable();
        }
        /// <summary>
        /// 获取脚本
        /// </summary>
        public override void GetTargetScript()
        {
            base.GetTargetScript();

            actionScript = target as ActionNode_Branch;
        }
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();
        }
        /// <summary>
        /// 子行为组件折叠容器
        /// </summary>
        /// <param name="fold"></param>
        public override Foldout Folder_ChildActions(VisualElement root)
        {
            Foldout fold = base.Folder_ChildActions(root);

            if (actionScript.childNode_true != null)
                DrawBranchAction(fold, actionScript.childNode_true, "开路行为");
            if (actionScript.childNode_false != null)
                DrawBranchAction(fold, actionScript.childNode_false, "闭路行为");

            return fold;
        }
        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout Folder_Extensions(VisualElement root)
        {
            return base.Folder_Extensions(root);
        }
        /// <summary>
        /// 黑板变量组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout Folder_BlackBoardVariable(VisualElement root)
        {
            return base.Folder_BlackBoardVariable(root);
        }
        /// <summary>
        /// 内部变量组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout Folder_InternalVariable(VisualElement root)
        {
            return base.Folder_InternalVariable(root);
        }

        #region 辅助
        /// <summary>
        /// 绘制分支节点信息面板
        /// </summary>
        /// <param name="element"></param>
        /// <param name="child"></param>
        private void DrawBranchAction(VisualElement element, ActionNode_Base child, string status)
        {
            VisualElement container = new VisualElement();
            container.AddToClassList("list_container");
            element.Add(container);

            container.RegisterCallback<PointerEnterEvent>((evt) =>
            {
                xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                Node node = wnd.xw_graphView.FindNode(child.guid);
                if (node is VNode_Base n_base)
                {
                    n_base.Highlight();
                }
            });

            container.RegisterCallback<PointerLeaveEvent>((evt) =>
            {
                xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                Node node = wnd.xw_graphView.FindNode(child.guid);
                if (node is VNode_Base n_base)
                {
                    n_base.UnHighlight();
                }
            });

            container.RegisterCallback<PointerDownEvent>((evt) =>
            {
                xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                if (child != null)
                {
                    EditorGUIUtility.PingObject(child);
                }
            });

            VisualElement container_title = new VisualElement();
            container_title.AddToClassList("list_titlebg");
            container.Add(container_title);

            VisualElement container_icon = new VisualElement();
            container_icon.AddToClassList("list_item_icon");
            container_icon.style.backgroundImage = child.NodeIcon == null ? util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{child.icon}.png") : child.NodeIcon;
            container_title.Add(container_icon);

            util_XGraphInspectorGUI.GUI_Label(container_title, $"目标：{child.identifyName}", new string[] { "labeltext", "list_item_title" });
            util_XGraphInspectorGUI.GUI_Label(container_title, status, new string[] { "list_item_marktext" });
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>Guid：</b><color=#e1e1e1>{child.guid}</color>", new string[] { "list_item_label" });
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>行为类型：</b><color=#e1e1e1>{child.actionNodeType}</color>", new string[] { "list_item_label" });
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点类型：</b><color=#e1e1e1>{child.visualNodeType}</color>", new string[] { "list_item_label" });
        }
        #endregion
    }
}