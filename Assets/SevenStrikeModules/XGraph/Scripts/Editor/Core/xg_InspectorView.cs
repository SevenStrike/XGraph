namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// XGraph的GraphView Inspector基础件，[UxmlElement]用于在UIBuilder中出现Inspector的控件
    /// </summary>
    [UxmlElement]
    public partial class xg_InspectorView : VisualElement
    {
        public xg_Window graphwindow;
        /// <summary>
        /// 编辑器主体
        /// </summary>
        private Editor editor;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            util_XGraphInspectorGUI.InitializeStyle(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_Inspector.uss");
        }
        /// <summary>
        /// 清空面板内容
        /// </summary>
        internal void ClearInspector()
        {
            Clear();
            editor = null;
        }

        #region 绘制对象属性面板
        /// <summary>
        /// 绘制节点的属性界面
        /// </summary>
        /// <param root_title="nodesasset"></param>
        internal void InspectorViewer(Node nodeview)
        {
            #region 清空面板内容
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);
            #endregion

            // 如果选中的节点是 VNode_Base
            if (nodeview is xNode_Base n_base)
            {
                // 如果选中的节点类型是：VNode_Base 同时也是：VNode_Variable_Internal 
                if (nodeview is xNode_Variable_Internal n_internalvar)
                    GUI_InternalVariableNode(n_internalvar);
                // 如果选中的节点类型是：VNode_Base 但并不是：VNode_Variable_Internal 
                else
                    GUI_ActionNode(n_base);
            }

            // 如果选中的节点是 VNode_Decal
            if (nodeview is xNode_Decal n_decal)
            {
                GUI_DecalNode(n_decal);
            }

            // 如果选中的节点是 VNode_Label
            if (nodeview is xNode_Label n_label)
            {
                GUI_LabelNode(n_label);
            }

            // 如果选中的节点是 VNode_Variable
            if (nodeview is xNode_Variable n_variable)
            {
                GUI_VariableNode(n_variable);
            }
        }
        /// <summary>
        /// 绘制连线的属性界面
        /// </summary>
        /// <param root_title="nodesasset"></param>
        internal void InspectorViewer(util_AnimatedEdge edge)
        {
            #region 清空面板内容
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);
            #endregion

            // 如果选中的节点是 VNode_Base
            if (edge is util_AnimatedEdge n_edge)
            {
                GUI_Edge(n_edge);
            }
        }
        /// <summary>
        /// 绘制行为根资源的属性界面
        /// </summary>
        /// <param root_title="nodesasset"></param>
        internal void InspectorViewer(xAction_Asset nodesasset)
        {
            #region 清空面板内容
            Clear();
            if (editor != null)
            {
                UnityEngine.Object.DestroyImmediate(editor);
                editor = null;
            }
            #endregion

            GUI_ActionAsset(nodesasset);
        }
        /// <summary>
        /// 绘制黑板变量项的属性界面
        /// </summary>
        /// <param root_title="nodesasset"></param>
        internal void InspectorViewer(Variable vare)
        {
            #region 清空面板内容
            Clear();
            if (editor != null)
            {
                UnityEngine.Object.DestroyImmediate(editor);
                editor = null;
            }
            #endregion

            GUI_BlackBoardVariable(vare);
        }
        #endregion

        #region 创建面板GUI界面
        /// <summary>
        /// 创建行为节点的属性面板
        /// </summary>
        /// <param name="target"></param>
        private void GUI_ActionNode(xNode_Base n_base)
        {
            var target = n_base.ActionData;

            if (target == null)
                return;

            #region  尝试查找是否有自定义 Editor
            string asm = "Assembly-CSharp-Editor";
            string name = target.GetType().Name;
            string result = util_XGraphEditorUtility.HasString(name, "xAction") ? name : util_XGraphEditorUtility.ExtractString(name, 7);
            var editorType = Type.GetType($"SevenStrikeModules.XGraph.editor_{result}, {asm}");
            #endregion

            #region 布局容器
            // 创建布局容器
            VisualElement container = util_XGraphInspectorGUI.GUI_Container(this, new string[] { "container" });
            Add(container);
            #endregion

            // 调用节点的InspectorGUI，以此支持在内置的Inspector面板上绘制节点属性
            container.Add(n_base.InspectorGUI());
        }
        /// <summary>
        /// 创建行为根资源的属性面板
        /// </summary>
        /// <param name="target"></param>
        private void GUI_ActionAsset(xAction_Asset nodesasset)
        {
            var target = nodesasset;

            if (target == null)
                return;

            #region  尝试查找是否有自定义 Editor
            string asm = "Assembly-CSharp-Editor";
            var editorType = Type.GetType($"SevenStrikeModules.XGraph.editor_{target.GetType().Name}, {asm}");
            #endregion

            #region 布局容器
            // 创建布局容器
            VisualElement container = util_XGraphInspectorGUI.GUI_Container(this, new string[] { "container" });
            Add(container);
            #endregion

            #region 行为节点自定义属性面板
            bool isCustomEditor = (editorType != null && typeof(Editor).IsAssignableFrom(editorType)) ? true : false;
            switch (isCustomEditor)
            {
                // 存在Editor解释文件，使用自定义界面样式
                case true:
                    editor = Editor.CreateEditor(target, editorType);
                    if (editor is editor_xAction_Asset actionEditor)
                        container.Add(actionEditor.CreateGraphviewInespector());
                    break;
                // 不存在Editor解释文件，使用内置界面样式
                case false:
                    // 回退到默认编辑器
                    editor = Editor.CreateEditor(target);
                    IMGUIContainer imguiContainer = new IMGUIContainer(() =>
                    {
                        editor.OnInspectorGUI();
                    });
                    container.Add(imguiContainer);
                    break;
            }
            #endregion
        }
        /// <summary>
        /// 创建内部变量节点的属性面板
        /// </summary>
        /// <param name="n_var_internal"></param>
        private void GUI_InternalVariableNode(xNode_Variable_Internal n_var_internal)
        {
            var data = n_var_internal.VariableData;

            if (data == null)
                return;

            // 创建布局容器
            VisualElement container = util_XGraphInspectorGUI.GUI_Container(this, new string[1] { "container" });

            // 标题
            VisualElement titlegroup = util_XGraphInspectorGUI.GUI_Title(container, n_var_internal.ActionData, data.identifyName, new string[] { "titlegroup" }, new string[] { "titleicon" }, new string[] { "titlename" });

            // 标题附加 - 变量类型标签
            string[] styles_sub = new string[] { "type" };
            Label lab_sub = new Label(data.variable.type.ToString());
            lab_sub.name = "sub";
            for (int i = 0; i < styles_sub.Length; i++)
            {
                lab_sub.AddToClassList(styles_sub[i]);
            }
            titlegroup.Add(lab_sub);

            // node_guid
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N： </b><color=#e1e1e1>{data.guid}</color>", new string[1] { "labeltext" });

            // node_size
            Label label_size = util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点尺寸： </b> <color=#e1e1e1>X：{data.nodeGraphSize.x.ToString()}    Y：{data.nodeGraphSize.y.ToString()}</color>", new string[1] { "labeltext" });
            n_var_internal.ActionData.On_Node_SizeChanged += (size) =>
            {
                label_size.text = $"<b>节点尺寸： </b> <color=#e1e1e1>X：{size.x}    Y：{size.y}</color>";
            };

            // node_pos
            Label label_pos = util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点位置： </b> <color=#e1e1e1>X：{data.nodeGraphPosition.x.ToString()}    Y：{data.nodeGraphPosition.y.ToString()}</color>", new string[1] { "labeltext" });
            n_var_internal.ActionData.On_Node_Moved += (pos) =>
            {
                label_pos.text = $"<b>节点位置： </b> <color=#e1e1e1>X：{pos.x}    Y：{pos.y}</color>";
            };

            #region 值
            switch (data.variable.type)
            {
                case xVariableType.String:
                    TextField field_string = util_XGraphInspectorGUI.GUI_Field_String(container, "变量值：", data.variable.GetValue<string>(), new string[1] { "field_text" });
                    field_string.RegisterCallback<BlurEvent>((value) =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_string.value = data.variable.GetValue<string>();
                        }
                        else
                        {
                            data.variable.SetValue(field_string.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_string.value}");
                    });
                    field_string.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_string.value = data.variable.GetValue<string>();
                        }
                        else
                        {
                            data.variable.SetValue(field_string.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_string.value}");
                    });
                    n_var_internal.VariableData.On_InternalVariableValue_Changed += (() =>
                    {
                        field_string.value = data.variable.GetValue<string>();
                    });
                    break;
                case xVariableType.Float:
                    FloatField field_float = util_XGraphInspectorGUI.GUI_Field_Float(container, "变量值：", data.variable.GetValue<float>(), new string[1] { "field_float" });
                    field_float.RegisterCallback<BlurEvent>((value) =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_float.value = data.variable.GetValue<float>();
                        }
                        else
                        {
                            data.variable.SetValue(field_float.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_float.value}");
                    });
                    field_float.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_float.value = data.variable.GetValue<float>();
                        }
                        else
                        {
                            data.variable.SetValue(field_float.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_float.value}");
                    });
                    n_var_internal.VariableData.On_InternalVariableValue_Changed += (() =>
                    {
                        field_float.value = data.variable.GetValue<float>();
                    });
                    break;
                case xVariableType.Int:
                    IntegerField field_int = util_XGraphInspectorGUI.GUI_Field_Int(container, "变量值：", data.variable.GetValue<int>(), new string[1] { "field_int" });
                    field_int.RegisterCallback<BlurEvent>((value) =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_int.value = data.variable.GetValue<int>();
                        }
                        else
                        {
                            data.variable.SetValue(field_int.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_int.value}");
                    });
                    field_int.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_int.value = data.variable.GetValue<int>();
                        }
                        else
                        {
                            data.variable.SetValue(field_int.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_int.value}");
                    });
                    n_var_internal.VariableData.On_InternalVariableValue_Changed += (() =>
                    {
                        field_int.value = data.variable.GetValue<int>();
                    });
                    break;
                case xVariableType.Bool:
                    Toggle field_bool = util_XGraphInspectorGUI.GUI_Field_Bool(container, "变量值：", data.variable.GetValue<bool>(), new string[1] { "field_bool" });
                    n_var_internal.VariableData.On_InternalVariableValue_Changed += (() =>
                    {
                        field_bool.value = data.variable.GetValue<bool>();
                    });
                    field_bool.RegisterValueChangedCallback((value) =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_bool.value = data.variable.GetValue<bool>();
                        }
                        else
                        {
                            data.variable.SetValue(field_bool.value);

                            if (n_var_internal != null)
                            {
                                n_var_internal.Toggle_Check(field_bool.value);
                            }
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_bool.value}");
                    });
                    field_bool.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_bool.value = data.variable.GetValue<bool>();
                        }
                        else
                        {
                            data.variable.SetValue(field_bool.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_bool.value}");
                    });
                    break;
                case xVariableType.Vector2:
                    Vector2Field field_vector2 = util_XGraphInspectorGUI.GUI_Field_Vector2(container, "变量值：", data.variable.GetValue<Vector2>(), new string[1] { "field_vector2" });
                    field_vector2.RegisterCallback<BlurEvent>((value) =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_vector2.value = data.variable.GetValue<Vector2>();
                        }
                        else
                        {
                            data.variable.SetValue(field_vector2.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector2.value}");
                    });
                    field_vector2.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_vector2.value = data.variable.GetValue<Vector2>();
                        }
                        else
                        {
                            data.variable.SetValue(field_vector2.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector2.value}");
                    });
                    n_var_internal.VariableData.On_InternalVariableValue_Changed += (() =>
                    {
                        field_vector2.value = data.variable.GetValue<Vector2>();
                    });
                    break;
                case xVariableType.Vector3:
                    Vector3Field field_vector3 = util_XGraphInspectorGUI.GUI_Field_Vector3(container, "变量值：", data.variable.GetValue<Vector3>(), new string[1] { "field_vecto3" });
                    field_vector3.RegisterCallback<BlurEvent>((value) =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_vector3.value = data.variable.GetValue<Vector3>();
                        }
                        else
                        {
                            data.variable.SetValue(field_vector3.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector3.value}");
                    });
                    field_vector3.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_vector3.value = data.variable.GetValue<Vector3>();
                        }
                        else
                        {
                            data.variable.SetValue(field_vector3.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector3.value}");
                    });
                    n_var_internal.VariableData.On_InternalVariableValue_Changed += (() =>
                    {
                        field_vector3.value = data.variable.GetValue<Vector3>();
                    });
                    break;
                case xVariableType.Vector4:
                    Vector4Field field_vector4 = util_XGraphInspectorGUI.GUI_Field_Vector4(container, "变量值：", data.variable.GetValue<Vector4>(), new string[1] { "field_vector4" });
                    field_vector4.RegisterCallback<BlurEvent>((value) =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_vector4.value = data.variable.GetValue<Vector4>();
                        }
                        else
                        {
                            data.variable.SetValue(field_vector4.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector4.value}");
                    });
                    field_vector4.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_vector4.value = data.variable.GetValue<Vector4>();
                        }
                        else
                        {
                            data.variable.SetValue(field_vector4.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector4.value}");
                    });
                    n_var_internal.VariableData.On_InternalVariableValue_Changed += (() =>
                    {
                        field_vector4.value = data.variable.GetValue<Vector4>();
                    });
                    break;
                case xVariableType.Color:
                    ColorField field_color = util_XGraphInspectorGUI.GUI_Field_Color(container, "变量值：", data.variable.GetValue<Color>(), new string[1] { "field_color" });
                    field_color.RegisterCallback<BlurEvent>((value) =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_color.value = data.variable.GetValue<Color>();
                        }
                        else
                        {
                            data.variable.SetValue(field_color.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_color.value}");
                    });
                    field_color.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        #region 如果该内部变量有链接黑板变量则不会更改黑板变量，而是用黑板变量覆盖当前属性框的值
                        if (data.VariableDatas != null && data.VariableDatas.Count > 0)
                        {
                            field_color.value = data.variable.GetValue<Color>();
                        }
                        else
                        {
                            data.variable.SetValue(field_color.value);
                        }
                        #endregion

                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_color.value}");
                    });
                    n_var_internal.VariableData.On_InternalVariableValue_Changed += (() =>
                    {
                        field_color.value = data.variable.GetValue<Color>();
                    });
                    break;
            }
            #endregion
        }
        /// <summary>
        /// 创建黑板变量节点的属性面板
        /// </summary>
        /// <param name="n_variable"></param>
        private void GUI_VariableNode(xNode_Variable n_variable)
        {
            var data = n_variable.VariableData;

            if (data == null)
                return;

            // 创建布局容器
            VisualElement container = util_XGraphInspectorGUI.GUI_Container(this, new string[1] { "container" });

            // 标题
            Color themeColor = Color.clear;
            foreach (var theme in graphwindow.xw_BlackBoardView.VariableThemes.VariableThemes)
            {
                if (theme.type == data.type.ToString())
                {
                    themeColor = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            }
            // 标题
            VisualElement titlegroup = util_XGraphInspectorGUI.GUI_Title(container, util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/blackboardvariable.png"), data.name, new string[] { "titlegroup" }, new string[] { "titleicon" }, new string[] { "titlename" });
            titlegroup.Q<VisualElement>(name: "icon").style.unityBackgroundImageTintColor = themeColor;

            // 标题附加 - 变量类型标签
            string[] styles_sub = new string[] { "type" };
            Label lab_sub = new Label(data.variable.type.ToString());
            lab_sub.name = "sub";
            for (int i = 0; i < styles_sub.Length; i++)
            {
                lab_sub.AddToClassList(styles_sub[i]);
            }
            titlegroup.Add(lab_sub);

            // 解释
            util_XGraphInspectorGUI.GUI_Label(container, data.description, new string[1] { "description" });

            // node_guid
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N： </b><color=#e1e1e1>{data.guid_n}</color>", new string[1] { "labeltext" });

            // var_guid
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-V： </b><color=#e1e1e1>{data.guid_v}</color>", new string[1] { "labeltext" });

            // node_pos
            Label label_size = util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点尺寸： </b> <color=#e1e1e1>X：{data.size.x.ToString()}    Y：{data.size.y.ToString()}</color>", new string[1] { "labeltext" });
            n_variable.On_Node_SizeChanged += (size) =>
            {
                label_size.text = $"<b>节点尺寸： </b> <color=#e1e1e1>X：{size.x}    Y：{size.y}</color>";
            };

            // node_size
            Label label_pos = util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点位置： </b> <color=#e1e1e1>X：{data.position.x.ToString()}    Y：{data.position.y.ToString()}</color>", new string[1] { "labeltext" });
            n_variable.On_Node_Moved += (pos) =>
            {
                label_pos.text = $"<b>节点位置： </b> <color=#e1e1e1>X：{pos.x}    Y：{pos.y}</color>";
            };

            #region 值
            switch (data.type)
            {
                case xVariableType.String:
                    TextField field_string = util_XGraphInspectorGUI.GUI_Field_String(container, "变量值：", data.variable.GetValue<string>(), new string[1] { "field_text" });
                    field_string.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_string.value);
                        SetBlackBoardVariableValue(field_string.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_string.value}");
                    });
                    field_string.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_string.value);
                        SetBlackBoardVariableValue(field_string.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_string.value}");
                    });
                    break;
                case xVariableType.Float:
                    FloatField field_float = util_XGraphInspectorGUI.GUI_Field_Float(container, "变量值：", data.variable.GetValue<float>(), new string[1] { "field_float" });
                    field_float.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_float.value);
                        SetBlackBoardVariableValue(field_float.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_float.value}");
                    });
                    field_float.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_float.value);
                        SetBlackBoardVariableValue(field_float.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_float.value}");
                    });
                    break;
                case xVariableType.Int:
                    IntegerField field_int = util_XGraphInspectorGUI.GUI_Field_Int(container, "变量值：", data.variable.GetValue<int>(), new string[1] { "field_int" });
                    field_int.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_int.value);
                        SetBlackBoardVariableValue(field_int.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_int.value}");
                    });
                    field_int.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_int.value);
                        SetBlackBoardVariableValue(field_int.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_int.value}");
                    });
                    break;
                case xVariableType.Bool:
                    Toggle field_bool = util_XGraphInspectorGUI.GUI_Field_Bool(container, "变量值：", data.variable.GetValue<bool>(), new string[1] { "field_bool" });
                    field_bool.RegisterValueChangedCallback((value) =>
                    {
                        data.variable.SetValue(field_bool.value);
                        SetBlackBoardVariableValue(field_bool.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_bool.value}");
                    });
                    field_bool.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_bool.value);
                        SetBlackBoardVariableValue(field_bool.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_bool.value}");
                    });
                    break;
                case xVariableType.Vector2:
                    Vector2Field field_vector2 = util_XGraphInspectorGUI.GUI_Field_Vector2(container, "变量值：", data.variable.GetValue<Vector2>(), new string[1] { "field_vector2" });
                    field_vector2.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_vector2.value);
                        SetBlackBoardVariableValue(field_vector2.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector2.value}");
                    });
                    field_vector2.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_vector2.value);
                        SetBlackBoardVariableValue(field_vector2.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector2.value}");
                    });
                    break;
                case xVariableType.Vector3:
                    Vector3Field field_vector3 = util_XGraphInspectorGUI.GUI_Field_Vector3(container, "变量值：", data.variable.GetValue<Vector3>(), new string[1] { "field_vecto3" });
                    field_vector3.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_vector3.value);
                        SetBlackBoardVariableValue(field_vector3.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector3.value}");
                    });
                    field_vector3.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_vector3.value);
                        SetBlackBoardVariableValue(field_vector3.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector3.value}");
                    });
                    break;
                case xVariableType.Vector4:
                    Vector4Field field_vector4 = util_XGraphInspectorGUI.GUI_Field_Vector4(container, "变量值：", data.variable.GetValue<Vector4>(), new string[1] { "field_vector4" });
                    field_vector4.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_vector4.value);
                        SetBlackBoardVariableValue(field_vector4.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector4.value}");
                    });
                    field_vector4.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_vector4.value);
                        SetBlackBoardVariableValue(field_vector4.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector4.value}");
                    });
                    break;
                case xVariableType.Color:
                    ColorField field_color = util_XGraphInspectorGUI.GUI_Field_Color(container, "变量值：", data.variable.GetValue<Color>(), new string[1] { "field_color" });
                    field_color.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_color.value);
                        SetBlackBoardVariableValue(field_color.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_color.value}");
                    });
                    field_color.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_color.value);
                        SetBlackBoardVariableValue(field_color.value, data.guid_v);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_color.value}");
                    });
                    break;
            }
            #endregion
        }
        /// <summary>
        /// 创建贴图节点的属性面板
        /// </summary>
        /// <param name="n_decal"></param>
        private void GUI_DecalNode(xNode_Decal n_decal)
        {
            var data = n_decal.DecalData;

            if (data == null)
                return;

            // 创建布局容器
            VisualElement container = util_XGraphInspectorGUI.GUI_Container(this, new string[1] { "container" });

            // 标题
            VisualElement titlegroup = util_XGraphInspectorGUI.GUI_Title(container, util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/decal.png"), data.texture_decal ? data.texture_decal.name : "Unset", new string[] { "titlegroup" }, new string[] { "titleicon" }, new string[] { "titlename" });

            // 标题附加 - 贴图链接
            Button btn_decaltex_ping = util_XGraphInspectorGUI.GUI_Button(titlegroup, null, new string[] { "iconbutton" });
            btn_decaltex_ping.clicked += (() =>
            {
                EditorGUIUtility.PingObject(data.texture_decal);
            });

            // node_guid
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N： </b><color=#e1e1e1>{data.guid}</color>", new string[] { "labeltext" });

            // node_size
            Label label_size = util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点尺寸： </b> <color=#e1e1e1>X：{data.size.x.ToString()}    Y：{data.size.y.ToString()}</color>", new string[] { "labeltext" });
            n_decal.On_Node_SizeChanged += (size) =>
            {
                label_size.text = $"<b>节点尺寸： </b> <color=#e1e1e1>X：{size.x}    Y：{size.y}</color>";
            };

            // node_pos
            Label label_pos = util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点位置： </b> <color=#e1e1e1>X：{data.position.x.ToString()}    Y：{data.position.y.ToString()}</color>", new string[] { "labeltext" });
            n_decal.On_Node_Moved += (pos) =>
            {
                label_pos.text = $"<b>节点位置： </b> <color=#e1e1e1>X：{pos.x}    Y：{pos.y}</color>";
            };

            // node_scale
            Label label_scale = util_XGraphInspectorGUI.GUI_Label(container, $"<b>缩放： </b> <color=#e1e1e1>X：{data.scale.x.ToString()}    Y：{data.scale.y.ToString()}</color>", new string[] { "labeltext" });
            n_decal.On_Node_DecalTexScaleChanged += (scale) =>
            {
                label_scale.text = $"<b>缩放： </b> <color=#e1e1e1>X：{scale.x}    Y：{scale.y}</color>";
            };

            // node_realsize
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>实际尺寸： </b> <color=#e1e1e1>X：{(data.texture_decal ? data.texture_decal.width.ToString() : " - ")}    Y：{(data.texture_decal ? data.texture_decal.height.ToString() : "-")}</color>", new string[] { "labeltext" });

            // texture_bg
            VisualElement tex_bg = util_XGraphInspectorGUI.GUI_Texture(container, util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/decal_bg.png"), new string[] { "texture_bg" });

            // texture
            VisualElement tex = util_XGraphInspectorGUI.GUI_Texture(container, data.texture_decal ? data.texture_decal : null, new string[] { "texture" });
            // 延迟播放动画
            EditorApplication.delayCall += () =>
            {
                tex.AddToClassList("texture_floated");
            };

            tex.RegisterCallback<MouseDownEvent>((evt) =>
            {
                EditorGUIUtility.PingObject(data.texture_decal ? data.texture_decal : null);
            });
            tex_bg.Add(tex);
        }
        /// <summary>
        /// 创建标签节点的属性面板
        /// </summary>
        /// <param name="n_label"></param>
        private void GUI_LabelNode(xNode_Label n_label)
        {
            var data = n_label.LabelData;

            if (data == null)
                return;

            // 创建布局容器
            VisualElement container = util_XGraphInspectorGUI.GUI_Container(this, new string[] { "container" });

            // 标题
            VisualElement titlegroup = util_XGraphInspectorGUI.GUI_Title(container, util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/label.png"), "标签", new string[] { "titlegroup" }, new string[] { "titleicon" }, new string[] { "titlename" });

            // 标题附加 - 字体颜色
            ColorField colField = util_XGraphInspectorGUI.GUI_Field_Color(titlegroup, $"<b>颜色： </b>", data.color, new string[] { "labelnode_field_color" });
            colField.Q<Label>(className: "unity-base-field__label").AddToClassList("labelnode_field_color_labelinterval");
            colField.RegisterValueChangedCallback((v) =>
            {
                Undo.RecordObject(graphwindow.CloneTree, "Change LabelColor");
                data.color = v.newValue;
                util_XGraphEditorUtility.Element_Color_Set(n_label.labelContentlabel, v.newValue);
            });
            n_label.On_FontColorValueChanged += (value) =>
            {
                colField.value = value;
            };

            // node_guid
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N： </b><color=#e1e1e1>{data.guid}</color>", new string[] { "labeltext" });

            // node_size
            Label label_size = util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点尺寸： </b> <color=#e1e1e1>X：{data.size.x.ToString()}    Y：{data.size.y.ToString()}</color>", new string[] { "labeltext" });
            n_label.On_Node_SizeChanged += (size) =>
            {
                label_size.text = $"<b>节点尺寸： </b> <color=#e1e1e1>X：{size.x}    Y：{size.y}</color>";
            };

            // node_pos
            Label label_pos = util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点位置： </b> <color=#e1e1e1>X：{data.position.x.ToString()}    Y：{data.position.y.ToString()}</color>", new string[] { "labeltext" });
            n_label.On_Node_Moved += (pos) =>
            {
                label_pos.text = $"<b>节点位置： </b> <color=#e1e1e1>X：{pos.x}    Y：{pos.y}</color>";
            };

            // Bold
            Toggle field_bold = util_XGraphInspectorGUI.GUI_Field_Bool(container, "粗体：", data.bold, new string[] { "field_bool" });
            field_bold.RegisterValueChangedCallback((value) =>
            {
                Undo.RecordObject(graphwindow.CloneTree, "Change LabelBold");
                data.bold = value.newValue;
                n_label.LabelFontStyleSet();
            });
            n_label.On_BoldValueChanged += (value) =>
            {
                field_bold.value = value;
            };


            // Italic
            Toggle field_italic = util_XGraphInspectorGUI.GUI_Field_Bool(container, "斜体：", data.italic, new string[] { "field_bool" });
            field_italic.RegisterValueChangedCallback((value) =>
            {
                Undo.RecordObject(graphwindow.CloneTree, "Change LabelItalic");
                data.italic = value.newValue;
                n_label.LabelFontStyleSet();
            });
            n_label.On_ItalicValueChanged += (value) =>
            {
                field_italic.value = value;
            };

            // font
            ObjectField objectField = util_XGraphInspectorGUI.GUI_Object<Font>(container, $"<b>字体： </b>", data.font, new string[] { "field_object" });
            objectField.RegisterValueChangedCallback(value =>
            {
                Undo.RecordObject(graphwindow.CloneTree, "Change LabelFont");
                Font font = value.newValue as Font;
                data.font = font;
                util_XGraphEditorUtility.Element_Label_FontSet(n_label.labelContentlabel, font);
            });

            // size
            IntegerField fontsizefield = util_XGraphInspectorGUI.GUI_Field_Int(container, "字体尺寸：", data.fontSize, new string[] { "field_int" });
            fontsizefield.RegisterValueChangedCallback<int>(value =>
            {
                Undo.RecordObject(graphwindow.CloneTree, "Change LabelFontSize");
                data.fontSize = value.newValue;
                util_XGraphEditorUtility.Element_IntegerField_ValueSet(n_label.FontSizeInput, value.newValue);
            });
            fontsizefield.RegisterCallback<BlurEvent>((evt) =>
            {
                IntegerField tf = evt.target as IntegerField;
                Undo.RecordObject(graphwindow.CloneTree, "Change LabelFontSize");
                data.fontSize = tf.value;
                util_XGraphEditorUtility.Element_IntegerField_ValueSet(n_label.FontSizeInput, tf.value);
            });
            fontsizefield.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                IntegerField tf = evt.target as IntegerField;
                Undo.RecordObject(graphwindow.CloneTree, "Change LabelFontSize");
                data.fontSize = tf.value;
                util_XGraphEditorUtility.Element_IntegerField_ValueSet(n_label.FontSizeInput, tf.value);
            });
            n_label.On_FontSizeValueChanged += (value) =>
            {
                fontsizefield.value = value;
            };

            // 值
            TextField textfield = util_XGraphInspectorGUI.GUI_Field_String(container, "内容：", data.content, new string[] { "field_text" });
            textfield.RegisterCallback<BlurEvent>((evt) =>
            {
                TextField tf = evt.target as TextField;
                Undo.RecordObject(graphwindow.CloneTree, "Change LabelValue");
                data.content = tf.value;
                util_XGraphEditorUtility.Element_Label_ValueSet(n_label.labelContentlabel, tf.value);
            });
            textfield.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                TextField tf = evt.target as TextField;
                Undo.RecordObject(graphwindow.CloneTree, "Change LabelValue");
                data.content = tf.value;
                util_XGraphEditorUtility.Element_Label_ValueSet(n_label.labelContentlabel, tf.value);
            });
            n_label.On_ContentValueChanged += (value) =>
            {
                textfield.value = value;
            };
        }
        /// <summary>
        /// 创建黑板变量项的属性面板
        /// </summary>
        /// <param name="vare"></param>
        private void GUI_BlackBoardVariable(Variable vare)
        {
            if (vare == null)
                return;

            // 创建布局容器
            VisualElement container = util_XGraphInspectorGUI.GUI_Container(this, new string[] { "container" });

            // 标题
            Color themeColor = Color.clear;
            foreach (var theme in graphwindow.xw_BlackBoardView.VariableThemes.VariableThemes)
            {
                if (theme.type == vare.type.ToString())
                {
                    themeColor = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            }
            // 标题
            VisualElement titlegroup = util_XGraphInspectorGUI.GUI_Title(container, util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/blackboardvariable.png"), vare.name, new string[] { "titlegroup" }, new string[] { "titleicon" }, new string[] { "titlename" });
            titlegroup.Q<VisualElement>(name: "icon").style.unityBackgroundImageTintColor = themeColor;

            // 标题附加 - 变量类型标签
            string[] styles_sub = new string[] { "type" };
            Label lab_sub = new Label(vare.type.ToString());
            lab_sub.name = "sub";
            for (int i = 0; i < styles_sub.Length; i++)
            {
                lab_sub.AddToClassList(styles_sub[i]);
            }
            titlegroup.Add(lab_sub);

            // 解释
            util_XGraphInspectorGUI.GUI_Label(container, vare.description, new string[] { "description" });

            // GUID-V
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-V： </b> <color=#e1e1e1>{vare.guid}</color>", new string[] { "labeltext" });

            #region 值
            switch (vare.type)
            {
                case xVariableType.String:
                    TextField field_string = util_XGraphInspectorGUI.GUI_Field_String(container, "变量值：", vare.GetValue<string>(), new string[] { "field_text" });
                    field_string.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(field_string.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    field_string.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(field_string.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case xVariableType.Float:
                    FloatField field_float = util_XGraphInspectorGUI.GUI_Field_Float(container, "变量值：", vare.GetValue<float>(), new string[] { "field_float" });
                    field_float.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(field_float.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    field_float.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(field_float.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case xVariableType.Int:
                    IntegerField field_int = util_XGraphInspectorGUI.GUI_Field_Int(container, "变量值：", vare.GetValue<int>(), new string[] { "field_int" });
                    field_int.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(field_int.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    field_int.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(field_int.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case xVariableType.Bool:
                    Toggle field_bool = util_XGraphInspectorGUI.GUI_Field_Bool(container, "变量值：", vare.GetValue<bool>(), new string[] { "field_bool" });
                    field_bool.RegisterValueChangedCallback((value) =>
                    {
                        vare.SetValue(field_bool.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    field_bool.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(field_bool.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case xVariableType.Vector2:
                    Vector2Field field_vector2 = util_XGraphInspectorGUI.GUI_Field_Vector2(container, "变量值：", vare.GetValue<Vector2>(), new string[] { "field_vector2" });
                    field_vector2.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(field_vector2.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    field_vector2.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(field_vector2.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case xVariableType.Vector3:
                    Vector3Field field_vector3 = util_XGraphInspectorGUI.GUI_Field_Vector3(container, "变量值：", vare.GetValue<Vector3>(), new string[] { "field_vecto3" });
                    field_vector3.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(field_vector3.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    field_vector3.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(field_vector3.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case xVariableType.Vector4:
                    Vector4Field field_vector4 = util_XGraphInspectorGUI.GUI_Field_Vector4(container, "变量值：", vare.GetValue<Vector4>(), new string[] { "field_vector4" });
                    field_vector4.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(field_vector4.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    field_vector4.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(field_vector4.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case xVariableType.Color:
                    ColorField field_color = util_XGraphInspectorGUI.GUI_Field_Color(container, "变量值：", vare.GetValue<Color>(), new string[] { "field_color" });
                    field_color.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(field_color.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    field_color.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(field_color.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
            }
            #endregion
        }
        /// <summary>
        /// 创建连线的属性面板
        /// </summary>
        /// <param name="n_edge"></param>
        private void GUI_Edge(util_AnimatedEdge n_edge)
        {
            if (n_edge == null)
                return;

            // 创建布局容器
            VisualElement container = util_XGraphInspectorGUI.GUI_Container(this, new string[1] { "container" });

            // 标题
            VisualElement titlegroup = util_XGraphInspectorGUI.GUI_Title(container, util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/Edge.png"), "连线信息", new string[] { "titlegroup" }, new string[] { "titleicon" }, new string[] { "titlename" });

            // node_guid
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-Edge： </b><color=#e1e1e1>{n_edge.viewDataKey}</color>", new string[] { "labeltext" });

            // 创建布局容器
            VisualElement connector_group = util_XGraphInspectorGUI.GUI_Container(container, new string[] { "connector_group" });


            Node n_out = n_edge.output.node;
            Node n_in = n_edge.input.node;

            Color themeColor = Color.white;
            if (n_out is xNode_Variable n_var)
            {
                foreach (var theme in graphwindow.xw_BlackBoardView.VariableThemes.VariableThemes)
                {
                    if (theme.type == n_var.VariableData.type.ToString())
                    {
                        themeColor = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                    }
                }
            }

            #region 输出节点
            VisualElement label_out = util_XGraphInspectorGUI.GUI_Container(connector_group, new string[] { "connector_node" });
            // 类型图标
            util_XGraphInspectorGUI.GUI_Texture(label_out, GetNodeIcon(n_out), new string[] { "connector_node_icon" }).style.unityBackgroundImageTintColor = themeColor;
            // 类型文字
            util_XGraphInspectorGUI.GUI_Label(label_out, GetNodeName(n_out), new string[] { "connector_node_name" });
            // 类型文字
            util_XGraphInspectorGUI.GUI_Label(label_out, GetNodeType(n_out), new string[] { "connector_node_type" });
            // 配色标记
            util_XGraphInspectorGUI.GUI_Container(label_out, new string[] { "connector_col_dot" }).style.backgroundColor = GetNodeThemeColor(n_out);
            #endregion

            util_XGraphInspectorGUI.GUI_Texture(connector_group, util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/Edge.png"), new string[] { "connector_icon" });

            #region 输入节点
            VisualElement label_in = util_XGraphInspectorGUI.GUI_Container(connector_group, new string[] { "connector_node" });
            // 类型图标
            util_XGraphInspectorGUI.GUI_Texture(label_in, GetNodeIcon(n_in), new string[] { "connector_node_icon" });
            // 类型文字
            util_XGraphInspectorGUI.GUI_Label(label_in, GetNodeName(n_in), new string[] { "connector_node_name" });
            // 类型文字
            util_XGraphInspectorGUI.GUI_Label(label_in, GetNodeType(n_in), new string[] { "connector_node_type" });
            // 配色标记
            util_XGraphInspectorGUI.GUI_Container(label_in, new string[] { "connector_col_dot" }).style.backgroundColor = GetNodeThemeColor(n_in);
            #endregion
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 设置黑板变量值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="guid"></param>
        private void SetBlackBoardVariableValue<T>(T value, string guid)
        {
            foreach (var v in graphwindow.CloneTree.BlackboardVariable)
            {
                if (v.guid == guid)
                {
                    Undo.RecordObject(graphwindow.CloneTree, "Change BlackBoardVaiableValue");
                    v.SetValue(value);
                }
            }
        }
        /// <summary>
        /// 根据节点类型获取特殊名称
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetNodeType(Node node)
        {
            string text = string.Empty;
            if (node is xNode_Base out_base)
            {
                text = "行为节点";
            }
            if (node is xNode_Variable out_var)
            {
                text = "黑板变量";
            }
            if (node is xNode_Variable_Internal out_internalvar)
            {
                text = "内部变量";
            }
            if (node is xNode_Property out_property)
            {
                text = "属性节点";
            }
            return text;
        }
        private string GetNodeName(Node node)
        {
            string text = string.Empty;
            if (node is xNode_Base out_base)
            {
                text = out_base.ActionData.identifyName;
            }
            if (node is xNode_Variable out_var)
            {
                text = out_var.VariableData.name;
            }
            return text;
        }
        /// <summary>
        /// 获取节点配色
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Color GetNodeThemeColor(Node node)
        {
            Color col = Color.white;
            if (node is xNode_Base out_base)
            {
                col = out_base.ActionData.themeColor;
            }
            return col;
        }
        /// <summary>
        /// 根据节点类型获取特殊图标
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Texture2D GetNodeIcon(Node node)
        {
            Texture2D tex = null;
            if (node is xNode_Base out_base)
            {
                if (out_base.ActionData.NodeIcon == null)
                    tex = util_XGraphEditorUtility.AssetLoad<Texture2D>(AssetDatabase.GUIDToAssetPath(out_base.ActionData.icon));
                else
                    tex = out_base.ActionData.NodeIcon;
            }
            if (node is xNode_Variable out_var)
            {
                tex = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/blackboardvariable.png");
            }

            return tex;
        }
        #endregion
    }
}