namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(xAction_Branch))]
    public class editor_xAction_Branch : editor_xAction_Base
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private xAction_Branch actionScript;

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

            //actionScript = target as xAction_Branch;
        }
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();
        }

        //------------------------------------------------------

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
        /// 属性节点的属性项折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout Folder_Propertys(VisualElement root)
        {
            return null;
        }

        #region 辅助
        /// <summary>
        /// 绘制分支节点信息面板
        /// </summary>
        /// <param name="element"></param>
        /// <param name="child"></param>
        private void DrawBranchAction(VisualElement element, string guid, string status)
        {
            xAction_Base child = actionScript.BaseArgs.RootAsset.FindActionNode(guid);

            VisualElement container = new VisualElement();
            container.AddToClassList("list_container");
            element.Add(container);

            // 高亮分支节点
            container.RegisterCallback<PointerEnterEvent>((evt) =>
            {
                xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                Node node = wnd.xw_graphView.FindNode(child.BaseArgs.guid);
                if (node is xNode_Base n_base)
                {
                    n_base.Highlight();
                }
            });
            // 取消高亮分支节点
            container.RegisterCallback<PointerLeaveEvent>((evt) =>
            {
                xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                Node node = wnd.xw_graphView.FindNode(child.BaseArgs.guid);
                if (node is xNode_Base n_base)
                {
                    n_base.UnHighlight();
                }
            });
            // 定位分支节点
            container.RegisterCallback<PointerDownEvent>((evt) =>
            {
                xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                if (child != null)
                {
                    //EditorGUIUtility.PingObject(child);
                }
            });

            VisualElement container_title = new VisualElement();
            container_title.AddToClassList("list_titlebg");
            container.Add(container_title);

            VisualElement container_icon = new VisualElement();
            container_icon.AddToClassList("list_item_icon");
            container_icon.style.backgroundImage = child.BaseArgs.NodeIcon == null ? util_XGraphEditorUtility.AssetLoad<Texture2D>(AssetDatabase.GUIDToAssetPath(child.BaseArgs.icon)) : child.BaseArgs.NodeIcon;
            container_title.Add(container_icon);

            util_XGraphInspectorGUI.GUI_Label(container_title, $"目标：{child.BaseArgs.identifyName}", new string[] { "labeltext", "list_item_title" });
            util_XGraphInspectorGUI.GUI_Label(container_title, status, new string[] { "list_item_marktext" });
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>Guid：</b><color=#e1e1e1>{child.BaseArgs.guid}</color>", new string[] { "list_item_label" });
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>行为类型：</b><color=#e1e1e1>{child.BaseArgs.actionNodeType}</color>", new string[] { "list_item_label" });
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点类型：</b><color=#e1e1e1>{child.BaseArgs.visualNodeType}</color>", new string[] { "list_item_label" });
        }
        #endregion
    }
}