namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(ActionNode_Wait))]
    public class Editor_ActionNode_Wait : Editor_ActionNode_Base
    {
        private ActionNode_Wait actionScript;

        #region 序列化属性
        private SerializedProperty
            sp_Time;
        #endregion

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

            actionScript = target as ActionNode_Wait;
        }
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();

            #region 寻找序列化属性
            sp_Time = serializedObject.FindProperty("Time");
            #endregion
        }
        /// <summary>
        /// 子行为组件折叠容器
        /// </summary>
        /// <param name="fold"></param>
        public override Foldout Folder_ChildActions(VisualElement root)
        {
            Foldout fold = base.Folder_ChildActions(root);

            fold.text = $"{fold.text}（{actionScript.childNodes.Count}）";

            for (int i = 0; i < actionScript.childNodes.Count; i++)
            {
                ActionNode_Base child = actionScript.childNodes[i];
                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);

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
                util_XGraphInspectorGUI.GUI_Label(container_title, "行为", new string[] { "list_item_marktext" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>Guid：</b><color=#e1e1e1>{child.guid}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>行为类型：</b><color=#e1e1e1>{child.actionNodeType}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点类型：</b><color=#e1e1e1>{child.visualNodeType}</color>", new string[] { "list_item_label" });
            }

            return fold;
        }
        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="fold"></param>
        public override Foldout Folder_Extensions(VisualElement root)
        {
            Foldout fold = base.Folder_Extensions(root);

            #region 等待时间
            FloatField field_time = util_XGraphInspectorGUI.GUI_Field_Float(fold, "<b>时间： </b>", sp_Time.floatValue, new string[] { "field_float" });
            field_time.RegisterCallback<ChangeEvent<float>>((evt) =>
            {
                if (VariableBindConnectorsExist())
                {
                    serializedObject.Update();
                    field_time.value = sp_Time.floatValue;
                }
                else
                {
                    sp_Time.floatValue = field_time.value;
                    serializedObject.ApplyModifiedProperties();
                }
                actionScript.SetWaitTime("时间");
            });
            #endregion

            // 当根行为资源绑定变量时
            actionScript.On_Node_Variable_Binded += (var) =>
            {
                serializedObject.Update();
                field_time.value = sp_Time.floatValue;
            };

            // 克隆节点后刷新控件值为克隆后的最新值
            actionScript.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                ActionNode_Wait s_source = (ActionNode_Wait)source;
                // 克隆后的行为数据
                ActionNode_Wait s_clone = (ActionNode_Wait)clone;
                s_clone.Time = s_source.Time;

                GetProperties();

                serializedObject.Update();

                field_time.value = sp_Time.floatValue;
            };

            return fold;
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
    }
}