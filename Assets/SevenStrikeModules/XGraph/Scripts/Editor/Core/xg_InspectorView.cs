namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// XGraph的GraphView Inspector基础件，[UxmlElement]用于在UIBuilder中出现Inspector的控件
    /// </summary>
    [UxmlElement]
    public partial class xg_InspectorView : VisualElement
    {
        /// <summary>
        /// 编辑器主体
        /// </summary>
        private Editor editor;

        /// <summary>
        /// 显示目标节点的属性控件
        /// </summary>
        /// <param root_title="nodesasset"></param>
        internal void UpdateSelection(Node nodeview)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);

            VNode_Base n_base = nodeview as VNode_Base;
            VNode_Variable n_var = nodeview as VNode_Variable;

            // 如果选中的节点时 VNode_Base
            if (n_base != null)
            {
                var target = n_base.ActionData;
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
            }

            // 如果选中的节点时 VNode_Variable
            if (n_var != null)
            {
                var target = n_var.VariableData;
                if (target == null) return;
                //UpdateSelection(target);
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
            if (editor != null)
            {
                UnityEngine.Object.DestroyImmediate(editor);
                editor = null;
            }

            var target = nodesasset;
            if (target == null)
                return;
            // 尝试查找是否有自定义 Editor
            string asm = "Assembly-CSharp-Editor";
            var editorType = Type.GetType($"SevenStrikeModules.XGraph.Editor_{target.GetType().Name}, {asm}");
            if (editorType != null && typeof(Editor).IsAssignableFrom(editorType))
            {
                // 如果定义了自定义Inspector界面
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
        /// 显示目标黑板变量的属性控件
        /// </summary>
        /// <param root_title="nodesasset"></param>
        internal void UpdateSelection(Variable vare)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);

            Label lab = new Label();
            lab.style.fontSize = 25;
            lab.text = vare.name;
            Add(lab);

            Label val = new Label();
            val.style.fontSize = 25;
            val.text = vare.GetValue().ToString();
            Add(val);
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