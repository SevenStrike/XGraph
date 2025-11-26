namespace SevenStrikeModules.XGraph
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class util_XGraphNodeChildRemover : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset;

        [MenuItem("Assets/XGraph/C 子节点移除助手", false, 101)]
        public static void CreateActionGraphNode()
        {
            util_XGraphNodeChildRemover wnd = GetWindow<util_XGraphNodeChildRemover>();
            wnd.titleContent = new GUIContent("XGraphNodeChildRemover");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // 读取并克隆uxml布局到 root 布局
            var visual_window = util_XGraphEditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_XGraphNodeChildRemover.uxml");
            visual_window.CloneTree(root);
        }
    }
}