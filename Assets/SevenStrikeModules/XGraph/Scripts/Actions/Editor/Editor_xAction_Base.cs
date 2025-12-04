namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(xAction_Base), true)]
    public class editor_xAction_Base : Editor
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private xAction_Base baseScript;

        /// <summary>
        /// 节点序列化属性
        /// </summary>
        private SerializedProperty
            sp_identifyName,
            sp_guid,
            sp_path,
            sp_actionNodeType,
            sp_isStartNode,
            sp_visualNodeType,
            sp_nodeGraphPosition,
            sp_nodeGraphSize,
            sp_themeSolution,
            sp_themeColor,
            sp_isConcurrentExecution,
            sp_HasAvatar,
            sp_TransparentNode,
            sp_Avatar,
            sp_NodeIcon;

        public virtual void OnEnable()
        {
            if (target == null)
                return;

            GetTargetScript();
            GetProperties();
        }

        /// <summary>
        /// 初始化目标脚本
        /// </summary>
        public virtual void GetTargetScript()
        {
            //baseScript = target as xAction_Base;
        }
        /// <summary>
        /// 寻找序列化属性
        /// </summary>
        public virtual void GetProperties()
        {
            #region 寻找序列化属性
            sp_identifyName = serializedObject.FindProperty("identifyName");
            sp_guid = serializedObject.FindProperty("guid");
            sp_path = serializedObject.FindProperty("path");
            sp_actionNodeType = serializedObject.FindProperty("actionNodeType");
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
            sp_isStartNode = serializedObject.FindProperty("isStartNode");
            #endregion
        }

        //------------------------------------------------------

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
            titlegroup.RegisterCallback<PointerDownEvent>((evt) =>
            {
                //EditorGUIUtility.PingObject(baseScript);
            });
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
            textField_guid.RegisterCallback<BlurEvent>((evt) =>
            {
                TextField field = evt.target as TextField;
                field.value = sp_guid.stringValue;
                sp_guid.serializedObject.ApplyModifiedProperties();
            });
            #endregion

            #region 节点路径
            TextField textField_path = util_XGraphInspectorGUI.GUI_Field_String(fo_node, "<b>资源路径： </b>", sp_path.stringValue, new string[1] { "field_text" });
            textField_path.RegisterCallback<BlurEvent>((evt) =>
            {
                TextField field = evt.target as TextField;
                field.value = sp_path.stringValue;
                sp_path.serializedObject.ApplyModifiedProperties();
            });
            #endregion

            #region 行为类型
            TextField textField_actionNode_type = util_XGraphInspectorGUI.GUI_Field_String(fo_node, "<b>行为类型： </b>", sp_actionNodeType.stringValue, new string[1] { "field_text" });
            textField_actionNode_type.RegisterCallback<BlurEvent>((evt) =>
            {
                TextField field = evt.target as TextField;
                field.value = sp_actionNodeType.stringValue;
                sp_actionNodeType.serializedObject.ApplyModifiedProperties();
            });
            #endregion

            #region 节点类型
            TextField textField_visualNode_type = util_XGraphInspectorGUI.GUI_Field_String(fo_node, "<b>节点类型： </b>", sp_visualNodeType.stringValue, new string[1] { "field_text" });
            textField_visualNode_type.RegisterCallback<BlurEvent>((evt) =>
            {
                TextField field = evt.target as TextField;
                field.value = sp_visualNodeType.stringValue;
                sp_visualNodeType.serializedObject.ApplyModifiedProperties();
            });
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

            #region 通透样式
            Toggle tog_transparentNode = util_XGraphInspectorGUI.GUI_Field_Bool(fo_node, "通透样式：", sp_TransparentNode.boolValue, new string[] { "field_bool" });
            tog_transparentNode.RegisterValueChangedCallback((value) =>
            {
                //Undo.RecordObject(baseScript, "Change TransparentNode");
                sp_TransparentNode.boolValue = value.newValue;

                serializedObject.ApplyModifiedProperties();

                if (baseScript.On_Node_TransparentChanged != null)
                    baseScript.On_Node_TransparentChanged(value.newValue);
            });
            baseScript.On_Node_TransparentChanged += (value) =>
            {
                serializedObject.Update();
                tog_transparentNode.value = value;
            };
            #endregion

            #region 节点头像
            ObjectField avatarobj = util_XGraphInspectorGUI.GUI_Object<Texture2D>(fo_node, "头像", sp_Avatar.objectReferenceValue, new string[] { "field_object" });
            avatarobj.RegisterValueChangedCallback(value =>
            {
                //Undo.RecordObject(baseScript, "Change Avatar");
                if (avatarobj.value != null)
                    sp_HasAvatar.boolValue = true;
                else
                    sp_HasAvatar.boolValue = false;
                Texture2D tex = value.newValue as Texture2D;
                sp_Avatar.objectReferenceValue = tex;
                serializedObject.ApplyModifiedProperties();

                // 调用创建头像组件方法
                xg_Window win = util_XGraphEditorUtility.GetGraphviewWindow();
                xNode_Base node = win.xw_graphView.FindNodeView(baseScript.BaseArgs.guid);
                node.CreateAvatarElement();

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
                //Undo.RecordObject(baseScript, "Change Avatar");
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
            label_size.RegisterCallback<BlurEvent>((evt) =>
            {
                Vector2Field field = evt.target as Vector2Field;
                field.value = sp_nodeGraphSize.vector2Value;
                sp_nodeGraphSize.serializedObject.ApplyModifiedProperties();
            });
            #endregion

            #region 节点位置
            Vector2Field label_pos = util_XGraphInspectorGUI.GUI_Field_Vector2(fo_node, "位置", sp_nodeGraphPosition.vector2Value, new string[] { "field_vector2" });
            baseScript.On_Node_Moved += (pos) =>
            {
                label_pos.value = pos;
            };
            label_pos.RegisterCallback<BlurEvent>((evt) =>
            {
                Vector2Field field = evt.target as Vector2Field;
                field.value = sp_nodeGraphPosition.vector2Value;
                sp_nodeGraphPosition.serializedObject.ApplyModifiedProperties();
            });
            #endregion

            #region 节点并发模式
            // 要忽略掉属性节点，因为属性节点不参与行为的流程执行逻辑
            if (baseScript is not xAction_Property)
            {
                Toggle tog_concurrent = util_XGraphInspectorGUI.GUI_Field_Bool(fo_node, "并发模式：", sp_isConcurrentExecution.boolValue, new string[] { "field_bool" });
                tog_concurrent.RegisterValueChangedCallback((value) =>
                {
                    //Undo.RecordObject(baseScript, "Change ConcurrentMode");
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
            }
            #endregion

            #region 设置起始节点
            // 要忽略掉属性节点，因为不能将属性节点作为起始节点
            if (baseScript is not xAction_Property)
            {
                Toggle tog_isStartNode = util_XGraphInspectorGUI.GUI_Field_Bool(fo_node, "起始节点：", sp_isStartNode.boolValue, new string[] { "field_bool" });
                tog_isStartNode.RegisterValueChangedCallback((value) =>
                        {
                            //Undo.RecordObject(baseScript, "Change IsStartNode");
                            sp_isStartNode.boolValue = value.newValue;

                            baseScript.BaseArgs.RootAsset.SetStartNode(baseScript);
                            serializedObject.ApplyModifiedProperties();
                        });
                baseScript.On_Node_IsStartNode += (value) =>
                {
                    serializedObject.Update();
                    tog_isStartNode.value = value;
                };
            }
            #endregion

            #region 父行为
            Folder_ParentNode(rootElement);
            #endregion

            #region 子节点折叠器
            Folder_ChildActions(rootElement);
            #endregion

            #region Variables 折叠器         
            Folder_BlackBoardVariable(rootElement);
            baseScript.On_Node_Variable_Binded += (value) =>
            {
                GetProperties();
                Folder_BlackBoardVariable(rootElement);
            };

            #endregion

            #region InternalVariables 折叠器           
            Folder_InternalVariable(rootElement);
            baseScript.On_Node_Variable_Binded += (value) =>
            {
                GetProperties();
                Folder_InternalVariable(rootElement);
            };
            #endregion

            #region 属性参数折叠器
            Folder_Propertys(rootElement);
            #endregion

            #region 节点绑定的属性记录折叠器
            Folder_BindedPropertys(rootElement);
            #endregion

            #region 自定义扩展 折叠器
            Folder_Extensions(rootElement);
            #endregion

            return rootElement;
        }
        #endregion

        #region 折叠器
        public virtual Foldout Folder_ParentNode(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, "父行为", "parent", new string[] { "foldout" });
            fold.Clear();

            return fold;
        }
        /// <summary>
        /// 黑板变量组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public virtual Foldout Folder_BlackBoardVariable(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, $"黑板变量（{baseScript.BaseArgs.VariableDatas.Count}）", "basetype-var", new string[] { "foldout" });
            fold.Clear();
            for (int i = 0; i < baseScript.BaseArgs.VariableDatas.Count; i++)
            {
                Binder_Varialble con = baseScript.BaseArgs.VariableDatas[i];

                Variable vare = con.variable;

                string var_value = "";

                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);
                // 高亮黑板变量节点
                container.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(con.VariableNodeGuid);
                    if (node is xNode_Variable n_vare)
                    {
                        n_vare.Highlight();
                    }
                });
                // 取消高亮黑板变量节点
                container.RegisterCallback<PointerLeaveEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(con.VariableNodeGuid);
                    if (node is xNode_Variable n_vare)
                    {
                        n_vare.UnHighlight();
                    }
                });

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
                    case xVariableType.String:
                        var_value = vare.GetValue<string>().ToString();
                        break;
                    case xVariableType.Float:
                        var_value = vare.GetValue<float>().ToString();
                        break;
                    case xVariableType.Int:
                        var_value = vare.GetValue<int>().ToString();
                        break;
                    case xVariableType.Bool:
                        var_value = vare.GetValue<bool>().ToString();
                        break;
                    case xVariableType.Vector2:
                        var_value = vare.GetValue<Vector2>().ToString();
                        break;
                    case xVariableType.Vector3:
                        var_value = vare.GetValue<Vector3>().ToString();
                        break;
                    case xVariableType.Vector4:
                        var_value = vare.GetValue<Vector4>().ToString();
                        break;
                    case xVariableType.Color:
                        var_value = vare.GetValue<Color>().ToString();
                        break;
                }
                util_XGraphInspectorGUI.GUI_Label(container, var_value.ToString(), new string[] { "list_item_themevalue" }).style.color = baseScript.BaseArgs.RootAsset.GraphviewGridBackgroundThemes.themecolor;

                util_XGraphInspectorGUI.GUI_Label(container, $"<b>端口：</b><color=#e1e1e1>{con.TargetPortName}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>说明：</b><color=#e1e1e1>{vare.description}</color>".ToString(), new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N：</b><color=#e1e1e1>{con.VariableNodeGuid}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-V：</b><color=#e1e1e1>{vare.guid}</color>", new string[] { "list_item_label" });

            }

            return fold;
        }
        /// <summary>
        /// 内部变量组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public virtual Foldout Folder_InternalVariable(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, $"内部变量（{baseScript.BaseArgs.InternalVariableDatas.Count}）", "basetype-intvar", new string[] { "foldout" });
            fold.Clear();
            for (int i = 0; i < baseScript.BaseArgs.InternalVariableDatas.Count; i++)
            {
                Binder_Varialble con = baseScript.BaseArgs.InternalVariableDatas[i];

                Variable vare = con.variable;

                string var_value = "";


                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);

                // 高亮内部变量节点
                container.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(con.VariableNodeGuid);
                    if (node is xNode_Base n_base)
                    {
                        n_base.Highlight();
                    }
                });
                // 取消高亮内部变量节点
                container.RegisterCallback<PointerLeaveEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(con.VariableNodeGuid);
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
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/variable.png");
                container_title.Add(container_icon);

                util_XGraphInspectorGUI.GUI_Label(container_title, $"变量：{vare.name}", new string[] { "labeltext", "list_item_title" });
                util_XGraphInspectorGUI.GUI_Label(container_title, vare.type.ToString(), new string[] { "list_item_marktext" });
                switch (vare.type)
                {
                    case xVariableType.String:
                        var_value = vare.GetValue<string>().ToString();
                        break;
                    case xVariableType.Float:
                        var_value = vare.GetValue<float>().ToString();
                        break;
                    case xVariableType.Int:
                        var_value = vare.GetValue<int>().ToString();
                        break;
                    case xVariableType.Bool:
                        var_value = vare.GetValue<bool>().ToString();
                        break;
                    case xVariableType.Vector2:
                        var_value = vare.GetValue<Vector2>().ToString();
                        break;
                    case xVariableType.Vector3:
                        var_value = vare.GetValue<Vector3>().ToString();
                        break;
                    case xVariableType.Vector4:
                        var_value = vare.GetValue<Vector4>().ToString();
                        break;
                    case xVariableType.Color:
                        var_value = vare.GetValue<Color>().ToString();
                        break;
                }
                util_XGraphInspectorGUI.GUI_Label(container, var_value.ToString(), new string[] { "list_item_themevalue" }).style.color = baseScript.BaseArgs.RootAsset.GraphviewGridBackgroundThemes.themecolor;

                util_XGraphInspectorGUI.GUI_Label(container, $"<b>端口：</b><color=#e1e1e1>{con.TargetPortName}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>说明：</b><color=#e1e1e1>{vare.description}</color>".ToString(), new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N：</b><color=#e1e1e1>{con.VariableNodeGuid}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-V：</b><color=#e1e1e1>{vare.guid}</color>", new string[] { "list_item_label" });
            }

            return fold;
        }
        /// <summary>
        /// 自定义派生类组件折叠容器
        /// </summary>
        /// <param name="fold"></param>
        public virtual Foldout Folder_Extensions(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, "扩展", "extension", new string[] { "foldout" });
            fold.Clear();
            return fold;
        }
        /// <summary>
        /// 子行为组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public virtual Foldout Folder_ChildActions(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, "子行为", "childs", new string[] { "foldout" });
            fold.Clear();
            return fold;
        }
        /// <summary>
        /// 属性记录折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public virtual Foldout Folder_BindedPropertys(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, "属性绑定", "binded_propertys", new string[] { "foldout" });
            fold.text = $"{fold.text}（{baseScript.BaseArgs.binded_propertys.Count}）";
            fold.Clear();

            for (int i = 0; i < baseScript.BaseArgs.binded_propertys.Count; i++)
            {
                Binder_Property prop = baseScript.BaseArgs.binded_propertys[i];

                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);

                // 高亮属性节点
                container.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(prop.Property_GUID);
                    if (node is xNode_Property n_prop)
                    {
                        n_prop.Highlight();
                    }
                });
                // 取消高亮属性节点
                container.RegisterCallback<PointerLeaveEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(prop.Property_GUID);
                    if (node is xNode_Property n_prop)
                    {
                        n_prop.UnHighlight();
                    }
                });

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/property.png");
                container_title.Add(container_icon);

                string type = prop.Property_PortType.ToString();
                string result_type = type.Substring(type.IndexOf("XGraph.") + "XGraph.".Length).Split(new char[] { '_' })[1];
                util_XGraphInspectorGUI.GUI_Label(container_title, result_type, new string[] { "list_item_marktext" });
                util_XGraphInspectorGUI.GUI_Label(container_title, $"{prop.Property_PortName}   >>>   {prop.Action_PortName}", new string[] { "labeltext", "list_item_title" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>属性节点：</b><color=#e1e1e1>{prop.Property_NodeName}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>目标属性：</b><color=#e1e1e1>{prop.Property_PortName}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>Guid：</b><color=#e1e1e1>{prop.Property_GUID}</color>", new string[] { "list_item_label" });
            }

            return fold;
        }
        /// <summary>
        /// 属性节点的属性项折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public virtual Foldout Folder_Propertys(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, "属性参数", "propertys", new string[] { "foldout" });
            fold.Clear();

            return fold;
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 是否存在变量绑定
        /// </summary>
        /// <returns></returns>
        internal bool isVariableBinded()
        {
            return baseScript.BaseArgs.VariableDatas.Count > 0 || baseScript.BaseArgs.InternalVariableDatas.Count > 0 || baseScript.BaseArgs.binded_propertys.Count > 0;
        }
        /// <summary>
        /// 检查是否存在目标名称的变量或属性绑定
        /// </summary>
        /// <param name="bindname">绑定的字段名称（如"名称"、"激活"）</param>
        /// <returns></returns>
        internal bool isVariableBinded(string bindname)
        {
            // 检查黑板变量
            foreach (var variableData in baseScript.BaseArgs.VariableDatas)
            {
                if (variableData.TargetPortName == bindname)
                    return true;
            }

            // 检查内部变量
            foreach (var internalVariableData in baseScript.BaseArgs.InternalVariableDatas)
            {
                if (internalVariableData.TargetPortName == bindname)
                    return true;
            }

            // 检查属性绑定
            foreach (var property in baseScript.BaseArgs.binded_propertys)
            {
                if (property.Action_PortName == bindname)
                    return true;
            }

            return false;
        }
        #endregion
    }
}