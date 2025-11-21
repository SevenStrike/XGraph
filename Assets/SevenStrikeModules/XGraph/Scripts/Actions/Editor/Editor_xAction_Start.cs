namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(xAction_Start), true)]
    public class Editor_xAction_Start : Editor_xAction_Base
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private xAction_Start actionScript;

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

            actionScript = target as xAction_Start;
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
        /// 子行为折叠容器
        /// </summary>
        /// <param name="fold"></param>
        public override Foldout Folder_ChildActions(VisualElement root)
        {
            Foldout fold = base.Folder_ChildActions(root);
            fold.text = $"{fold.text}（{actionScript.childNodes.Count}）";

            for (int i = 0; i < actionScript.childNodes.Count; i++)
            {
                xAction_Base child = actionScript.childNodes[i];

                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);

                container.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(child.guid);
                    if (node is xNode_Base n_base)
                    {
                        n_base.Highlight();
                    }
                });

                container.RegisterCallback<PointerLeaveEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(child.guid);
                    if (node is xNode_Base n_base)
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
                util_XGraphInspectorGUI.GUI_Label(container_title, "行为", new string[] { "list_item_marktext" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>Guid：</b><color=#e1e1e1>{child.guid}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>行为类型：</b><color=#e1e1e1>{child.actionNodeType}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点类型：</b><color=#e1e1e1>{child.visualNodeType}</color>", new string[] { "list_item_label" });
            }

            return fold;
        }
        /// <summary>
        /// 节点父行为容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout Folder_ParentNode(VisualElement root)
        {
            return null;
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
    }
}