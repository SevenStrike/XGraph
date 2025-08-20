namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEditor;
    using UnityEngine.UIElements;

    /// <summary>
    /// XGraph的GraphView BlackBoard基础件，[UxmlElement]用于在UIBuilder中出现BlackBoardView的控件
    /// </summary>
    [UxmlElement]
    public partial class xg_BlackBoardView : VisualElement
    {
        private ListView ListView { get; set; }



        /// <summary>
        /// 编辑器主体
        /// </summary>
        private Editor editor;

        /// <summary>
        /// 显示目标节点的属性控件
        /// </summary>
        /// <param root_title="nodesasset"></param>
        internal void UpdateSelection(VNode_Base nodeview)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);

            var target = nodeview.ActionNode;
            if (target == null) return;

            // 尝试查找是否有自定义 Editor
            string asm = "Assembly-CSharp-Editor";
            var editorType = Type.GetType($"SevenStrikeModules.XGraph.Editor_{target.GetType().Name}, {asm}");
            if (editorType != null && typeof(Editor).IsAssignableFrom(editorType))
            {
                editor = Editor.CreateEditor(target, editorType);
            }
            else
            {
                // 回退到默认编辑器
                editor = Editor.CreateEditor(target);
            }

            if (editor != null)
            {
                IMGUIContainer container = new IMGUIContainer(() =>
                {
                    editor.OnInspectorGUI();
                });
                Add(container);
            }
        }

        /// <summary>
        /// 显示目标节点的属性控件
        /// </summary>
        /// <param root_title="nodesasset"></param>
        internal void UpdateSelection(ActionNode_Asset nodesasset)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);

            var target = nodesasset;
            if (target == null)
                return;
            // 尝试查找是否有自定义 Editor
            string asm = "Assembly-CSharp-Editor";
            var editorType = Type.GetType($"SevenStrikeModules.XGraph.Editor_{target.GetType().Name}, {asm}");
            if (editorType != null && typeof(Editor).IsAssignableFrom(editorType))
            {
                editor = Editor.CreateEditor(target, editorType);
            }
            else
            {
                // 回退到默认编辑器
                editor = Editor.CreateEditor(target);
            }

            if (editor != null)
            {
                IMGUIContainer container = new IMGUIContainer(() =>
                {
                    editor.OnInspectorGUI();
                });
                Add(container);
            }
        }

        /// <summary>
        /// 清空面板内容
        /// </summary>
        internal void ClearInspector()
        {
            Clear();
            editor = null;
        }
    }
}