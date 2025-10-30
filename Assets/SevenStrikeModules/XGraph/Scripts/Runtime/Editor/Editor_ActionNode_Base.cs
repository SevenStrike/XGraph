namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(ActionNode_Base), true)]
    public class Editor_ActionNode_Base : Editor
    {
        private ActionNode_Base baseScript;

        #region 序列化属性
        private SerializedProperty
            sp_identifyName,
            sp_content,
            sp_guid,
            sp_namespaces,
            sp_classes,
            sp_path,
            sp_actionNodeType,
            sp_icon,
            sp_visualNodeType,
            sp_nodeGraphPosition,
            sp_nodeGraphSize,
            sp_themeSolution,
            sp_themeColor,
            sp_isConcurrentExecution,
            sp_HasAvatar,
            sp_TransparentNode,
            sp_Avatar,
            sp_NodeIcon,
            sp_VariableDatas,
            sp_InternalVariableDatas,
            sp_ParentNode,
            sp_RootAsset;
        #endregion

        public virtual void OnEnable()
        {
            if (target == null)
                return;

            GetTargetScript();
            GetProperties();
        }

        #region 绘制界面
        /// <summary>
        /// 绘制节点编辑器内的Inspector界面
        /// </summary>
        /// <returns></returns>
        public VisualElement CreateGraphviewInespector()
        {
            VisualElement rootElement = new VisualElement();

            #region 标题
            VisualElement titlegroup = util_XGraphInspectorGUI.GUI_Title(rootElement, baseScript, sp_identifyName.stringValue, new string[] { "titlegroup" }, new string[] { "titleicon" }, new string[] { "titlename" });
            #endregion

            #region 标题附加 - 变量类型标签
            string[] styles_sub = new string[] { "type" };
            Label lab_sub = new Label("行为节点");
            lab_sub.name = "sub";
            for (int i = 0; i < styles_sub.Length; i++)
            {
                lab_sub.AddToClassList(styles_sub[i]);
            }
            titlegroup.Add(lab_sub);
            #endregion

            #region 同步节点名称改变
            Label titlename = titlegroup.Q<Label>(name: "title");
            baseScript.On_Node_TitleChanged += (name) =>
            {
                titlename.text = name;
            };
            #endregion

            #region 同步节点图标改变
            VisualElement titleIcon = titlegroup.Q<VisualElement>(name: "icon");
            baseScript.On_Node_IconChanged += (tex) =>
            {
                titleIcon.style.backgroundImage = tex;
            };
            #endregion

            #region 节点基础属性折叠器
            Foldout fo_node = util_XGraphInspectorGUI.GUI_Foldout(rootElement, "节点基础属性", "basetype-base", new string[] { "foldout" });
            #endregion

            #region 节点GUID
            TextField textField_guid = util_XGraphInspectorGUI.GUI_Field_String(fo_node, "<b>GUID： </b>", sp_guid.stringValue, new string[1] { "field_text" });
            #endregion

            #region 节点路径
            TextField textField_path = util_XGraphInspectorGUI.GUI_Field_String(fo_node, "<b>资源路径： </b>", sp_path.stringValue, new string[1] { "field_text" });
            #endregion

            #region 节点颜色
            ColorField themecolor = util_XGraphInspectorGUI.GUI_Field_Color(fo_node, "标记色", sp_themeColor.colorValue, new string[] { "field_color" });
            themecolor.RegisterValueChangedCallback(value =>
            {
                sp_themeSolution.stringValue = "自定义";
                sp_themeColor.colorValue = themecolor.value;
                serializedObject.ApplyModifiedProperties();

                if (baseScript.On_Node_ThemeColorChanged != null)
                    baseScript.On_Node_ThemeColorChanged();
            });
            // 根据节点视图右键菜单更改主题颜色的操作被动更新颜色框值
            baseScript.On_Node_ThemeColorChanged += () =>
            {
                serializedObject.Update();
                themecolor.value = sp_themeColor.colorValue;
            };
            #endregion

            #region 节点头像
            ObjectField avatarobj = util_XGraphInspectorGUI.GUI_Object<Texture2D>(fo_node, "头像", sp_Avatar.objectReferenceValue, new string[] { "field_object" });
            avatarobj.RegisterValueChangedCallback(value =>
            {
                Undo.RecordObject(baseScript, "Change Avatar");
                Texture2D tex = value.newValue as Texture2D;
                sp_Avatar.objectReferenceValue = tex;
                serializedObject.ApplyModifiedProperties();

                if (baseScript.On_Node_AvatarChanged != null)
                    baseScript.On_Node_AvatarChanged(tex);
            });
            baseScript.On_Node_AvatarChanged += (tex) =>
            {
                serializedObject.Update();
                avatarobj.value = tex;
            };
            #endregion

            #region 节点图标
            ObjectField iconobj = util_XGraphInspectorGUI.GUI_Object<Texture2D>(fo_node, "图标", sp_NodeIcon.objectReferenceValue, new string[] { "field_object" });
            iconobj.RegisterValueChangedCallback(value =>
            {
                Undo.RecordObject(baseScript, "Change Avatar");
                Texture2D tex = value.newValue as Texture2D;
                sp_NodeIcon.objectReferenceValue = tex;
                serializedObject.ApplyModifiedProperties();

                if (baseScript.On_Node_IconChanged != null)
                    baseScript.On_Node_IconChanged(tex);
            });
            baseScript.On_Node_IconChanged += (tex) =>
            {
                serializedObject.Update();
                iconobj.value = tex;
            };
            #endregion

            #region 节点尺寸
            Vector2Field label_size = util_XGraphInspectorGUI.GUI_Field_Vector2(fo_node, "尺寸", sp_nodeGraphSize.vector2Value, new string[] { "field_vector2" });
            baseScript.On_Node_SizeChanged += (size) =>
            {
                label_size.value = size;
            };
            #endregion

            #region 节点位置
            Vector2Field label_pos = util_XGraphInspectorGUI.GUI_Field_Vector2(fo_node, "位置", sp_nodeGraphPosition.vector2Value, new string[] { "field_vector2" });
            baseScript.On_Node_Moved += (pos) =>
            {
                label_pos.value = pos;
            };
            #endregion

            #region 节点并发模式
            Toggle tog_concurrent = util_XGraphInspectorGUI.GUI_Field_Bool(fo_node, "并发模式：", sp_isConcurrentExecution.boolValue, new string[] { "field_bool" });
            tog_concurrent.RegisterValueChangedCallback((value) =>
            {
                Undo.RecordObject(baseScript, "Change ConcurrentMode");
                sp_isConcurrentExecution.boolValue = value.newValue;

                serializedObject.ApplyModifiedProperties();

                if (baseScript.On_Node_ConcurrentChanged != null)
                    baseScript.On_Node_ConcurrentChanged(value.newValue);
            });
            baseScript.On_Node_ConcurrentChanged += (value) =>
            {
                serializedObject.Update();
                tog_concurrent.value = value;
            };
            #endregion

            #region 子节点折叠器
            Foldout fo_childs = util_XGraphInspectorGUI.GUI_Foldout(rootElement, "子行为", "childs", new string[] { "foldout" });
            ChildActionFolder_ItemDisplay(fo_childs);
            #endregion

            #region Variables 折叠器
            Foldout fo_var = util_XGraphInspectorGUI.GUI_Foldout(rootElement, $"黑板变量（{baseScript.VariableDatas.Count}）", "basetype-var", new string[] { "foldout" });
            BlackBoardVariableConnectorsFolder_ItemDisplay(fo_var);
            baseScript.On_Node_VariableBinded += (value) =>
            {
                BlackBoardVariableConnectorsFolder_ItemDisplay(fo_var);
            };

            #endregion

            #region InternalVariables 折叠器
            Foldout fo_intvar = util_XGraphInspectorGUI.GUI_Foldout(rootElement, $"内部变量（{baseScript.InternalVariableDatas.Count}）", "basetype-intvar", new string[] { "foldout" });
            InternalVariableConnectorsFolder_ItemDisplay(fo_intvar);
            baseScript.On_Node_VariableBinded += (value) =>
            {
                InternalVariableConnectorsFolder_ItemDisplay(fo_intvar);
            };
            #endregion

            #region 自定义扩展 折叠器
            Foldout fo_custom = util_XGraphInspectorGUI.GUI_Foldout(rootElement, "扩展", "extension", new string[] { "foldout" });
            ExtensionFolder_ItemDisplay(fo_custom);
            #endregion

            return rootElement;
        }
        /// <summary>
        /// 显示黑板变量关系到折叠
        /// </summary>
        /// <param name="fold"></param>
        private void BlackBoardVariableConnectorsFolder_ItemDisplay(Foldout fold)
        {
            fold.Clear();
            for (int i = 0; i < baseScript.VariableDatas.Count; i++)
            {
                VarialbleGuidConnector con = baseScript.VariableDatas[i];

                Variable vare = con.variable;

                string var_value = "";

                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/variable.png");
                container_title.Add(container_icon);

                util_XGraphInspectorGUI.GUI_Label(container_title, $"变量：{vare.name}", new string[] { "labeltext", "list_item_title" });
                util_XGraphInspectorGUI.GUI_Label(container_title, vare.type.ToString(), new string[] { "list_item_marktext" });
                switch (vare.type)
                {
                    case VariableType.String:
                        var_value = vare.GetValue<string>().ToString();
                        break;
                    case VariableType.Float:
                        var_value = vare.GetValue<float>().ToString();
                        break;
                    case VariableType.Int:
                        var_value = vare.GetValue<int>().ToString();
                        break;
                    case VariableType.Bool:
                        var_value = vare.GetValue<bool>().ToString();
                        break;
                    case VariableType.Vector2:
                        var_value = vare.GetValue<Vector2>().ToString();
                        break;
                    case VariableType.Vector3:
                        var_value = vare.GetValue<Vector3>().ToString();
                        break;
                    case VariableType.Vector4:
                        var_value = vare.GetValue<Vector4>().ToString();
                        break;
                    case VariableType.Color:
                        var_value = vare.GetValue<Color>().ToString();
                        break;
                }
                util_XGraphInspectorGUI.GUI_Label(container, var_value.ToString(), new string[] { "list_item_themevalue" }).style.color = baseScript.RootAsset.GraphviewGridBackgroundThemes.themecolor;

                util_XGraphInspectorGUI.GUI_Label(container, $"<b>端口：</b><color=#b9b9b9>{con.TargetPortName}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>说明：</b><color=#b9b9b9>{vare.description}</color>".ToString(), new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N：</b><color=#b9b9b9>{con.VariableNodeGuid}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-V：</b><color=#b9b9b9>{vare.guid}</color>", new string[] { "list_item_label" });

            }
        }
        /// <summary>
        /// 显示内部变量关系到折叠
        /// </summary>
        /// <param name="fold"></param>
        private void InternalVariableConnectorsFolder_ItemDisplay(Foldout fold)
        {
            fold.Clear();
            for (int i = 0; i < baseScript.InternalVariableDatas.Count; i++)
            {
                VarialbleInternalGuidConnector con = baseScript.InternalVariableDatas[i];

                Variable vare = con.variable;

                string var_value = "";


                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/variable.png");
                container_title.Add(container_icon);

                util_XGraphInspectorGUI.GUI_Label(container_title, $"变量：{vare.name}", new string[] { "labeltext", "list_item_title" });
                util_XGraphInspectorGUI.GUI_Label(container_title, vare.type.ToString(), new string[] { "list_item_marktext" });
                switch (vare.type)
                {
                    case VariableType.String:
                        var_value = vare.GetValue<string>().ToString();
                        break;
                    case VariableType.Float:
                        var_value = vare.GetValue<float>().ToString();
                        break;
                    case VariableType.Int:
                        var_value = vare.GetValue<int>().ToString();
                        break;
                    case VariableType.Bool:
                        var_value = vare.GetValue<bool>().ToString();
                        break;
                    case VariableType.Vector2:
                        var_value = vare.GetValue<Vector2>().ToString();
                        break;
                    case VariableType.Vector3:
                        var_value = vare.GetValue<Vector3>().ToString();
                        break;
                    case VariableType.Vector4:
                        var_value = vare.GetValue<Vector4>().ToString();
                        break;
                    case VariableType.Color:
                        var_value = vare.GetValue<Color>().ToString();
                        break;
                }
                util_XGraphInspectorGUI.GUI_Label(container, var_value.ToString(), new string[] { "list_item_themevalue" }).style.color = baseScript.RootAsset.GraphviewGridBackgroundThemes.themecolor;

                util_XGraphInspectorGUI.GUI_Label(container, $"<b>端口：</b><color=#b9b9b9>{con.TargetPortName}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>说明：</b><color=#b9b9b9>{vare.description}</color>".ToString(), new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N：</b><color=#b9b9b9>{con.VariableNodeGuid}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-V：</b><color=#b9b9b9>{vare.guid}</color>", new string[] { "list_item_label" });
            }
        }
        /// <summary>
        /// 显示自定义到折叠
        /// </summary>
        /// <param name="fold"></param>
        public virtual void ExtensionFolder_ItemDisplay(Foldout fold)
        {
            fold.Clear();
        }
        /// <summary>
        /// 显示子行为到折叠
        /// </summary>
        /// <param name="fold"></param>
        public virtual void ChildActionFolder_ItemDisplay(Foldout fold)
        {
            fold.Clear();
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 初始化目标脚本
        /// </summary>
        public virtual void GetTargetScript()
        {
            baseScript = target as ActionNode_Base;
        }
        /// <summary>
        /// 寻找序列化属性
        /// </summary>
        public virtual void GetProperties()
        {
            #region 寻找序列化属性
            sp_identifyName = serializedObject.FindProperty("identifyName");
            sp_content = serializedObject.FindProperty("content");
            sp_guid = serializedObject.FindProperty("guid");
            sp_namespaces = serializedObject.FindProperty("namespaces");
            sp_classes = serializedObject.FindProperty("classes");
            sp_path = serializedObject.FindProperty("path");
            sp_actionNodeType = serializedObject.FindProperty("actionNodeType");
            sp_icon = serializedObject.FindProperty("icon");
            sp_visualNodeType = serializedObject.FindProperty("visualNodeType");
            sp_nodeGraphPosition = serializedObject.FindProperty("nodeGraphPosition");
            sp_nodeGraphSize = serializedObject.FindProperty("nodeGraphSize");
            sp_themeSolution = serializedObject.FindProperty("themeSolution");
            sp_themeColor = serializedObject.FindProperty("themeColor");
            sp_isConcurrentExecution = serializedObject.FindProperty("isConcurrentExecution");
            sp_HasAvatar = serializedObject.FindProperty("HasAvatar");
            sp_TransparentNode = serializedObject.FindProperty("TransparentNode");
            sp_Avatar = serializedObject.FindProperty("Avatar");
            sp_NodeIcon = serializedObject.FindProperty("NodeIcon");
            sp_VariableDatas = serializedObject.FindProperty("VariableDatas");
            sp_InternalVariableDatas = serializedObject.FindProperty("InternalVariableDatas");
            sp_ParentNode = serializedObject.FindProperty("ParentNode");
            sp_RootAsset = serializedObject.FindProperty("RootAsset");
            #endregion
        }
        /// <summary>
        /// 是否存在变量绑定
        /// </summary>
        /// <returns></returns>
        internal bool VariableBindConnectorsExist()
        {
            if (baseScript.VariableDatas.Count > 0 || baseScript.InternalVariableDatas.Count > 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}