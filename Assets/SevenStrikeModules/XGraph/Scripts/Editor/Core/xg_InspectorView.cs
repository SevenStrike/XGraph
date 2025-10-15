namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Linq;
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
        /// 初始化样式
        /// </summary>
        public void InitializeStyle()
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
            if (nodeview is VNode_Base n_base)
            {
                // 如果选中的节点类型是：VNode_Base 同时也是：VNode_Variable_Internal 
                if (nodeview is VNode_Variable_Internal n_internalvar)
                    GUI_InternalVariableNode(n_internalvar);
                // 如果选中的节点类型是：VNode_Base 但并不是：VNode_Variable_Internal 
                else
                    GUI_ActionNode(n_base);
            }

            // 如果选中的节点是 VNode_Variable
            if (nodeview is VNode_Variable n_variable)
            {
                GUI_VariableNode(n_variable);
            }
        }

        /// <summary>
        /// 绘制行为根资源的属性界面
        /// </summary>
        /// <param root_title="nodesasset"></param>
        internal void InspectorViewer(ActionNode_Asset nodesasset)
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
        private void GUI_ActionNode(VNode_Base n_base)
        {
            var target = n_base.ActionData;

            if (target == null)
                return;

            #region  尝试查找是否有自定义 Editor
            string asm = "Assembly-CSharp-Editor";
            var editorType = Type.GetType($"SevenStrikeModules.XGraph.Editor_{target.GetType().Name}, {asm}");
            #endregion

            // 存在Editor解释文件，使用自定义界面样式
            if (editorType != null && typeof(Editor).IsAssignableFrom(editorType))
            {
                editor = Editor.CreateEditor(target, editorType);
            }
            // 不存在Editor解释文件，使用内置界面样式
            else
            {
                // 回退到默认编辑器
                editor = Editor.CreateEditor(target);
            }

            // 如果编辑器对象不为空则显示并绘制属性面板
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
        /// 创建行为根资源的属性面板
        /// </summary>
        /// <param name="target"></param>
        private void GUI_ActionAsset(ActionNode_Asset nodesasset)
        {
            var target = nodesasset;

            if (target == null)
                return;

            #region 尝试查找是否有自定义 Editor
            string asm = "Assembly-CSharp-Editor";
            var editorType = Type.GetType($"SevenStrikeModules.XGraph.Editor_{target.GetType().Name}, {asm}");
            #endregion

            // 如果是自带自定义编辑器类
            if (editorType != null && typeof(Editor).IsAssignableFrom(editorType))
            {
                // 如果定义了自定义Inspector界面
                editor = Editor.CreateEditor(target, editorType);
            }
            // 原生编辑器类
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
        /// 创建内部变量节点的属性面板
        /// </summary>
        /// <param name="n_var_internal"></param>
        private void GUI_InternalVariableNode(VNode_Variable_Internal n_var_internal)
        {
            var data = n_var_internal.VariableData;

            if (data == null)
                return;

            // 创建布局容器
            VisualElement container = util_XGraphInspectorGUI.GUI_Container(this, new string[1] { "container" });

            // 标题
            string[] styles_group = new string[1] { "titlegroup" };
            string[] styles_icon = new string[1] { "titleicon" };
            string[] styles_title = new string[1] { "titlename" };
            string[] styles_sub = new string[1] { "type" };
            util_XGraphInspectorGUI.GUI_IconTitle(container, n_var_internal.ActionData, data.name, data.variable.type.ToString(), styles_group, styles_icon, styles_title, styles_sub);

            // node_guid
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N： </b><color=#b1b1b1>{data.guid}</color>", new string[1] { "labeltext" });

            // node_pos
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>尺寸： </b> <color=#b1b1b1>X：{data.nodeGraphSize.x.ToString()}    Y：{data.nodeGraphSize.y.ToString()}</color>", new string[1] { "labeltext" });

            // node_size
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>位置： </b> <color=#b1b1b1>X：{data.nodeGraphPosition.x.ToString()}    Y：{data.nodeGraphPosition.y.ToString()}</color>", new string[1] { "labeltext" });

            #region 值
            switch (data.variable.type)
            {
                case VariableType.String:
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
                    n_var_internal.On_InternalVariableValue_Changed += (() =>
                    {
                        field_string.value = data.variable.GetValue<string>();
                    });
                    break;
                case VariableType.Float:
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
                    n_var_internal.On_InternalVariableValue_Changed += (() =>
                    {
                        field_float.value = data.variable.GetValue<float>();
                    });
                    break;
                case VariableType.Int:
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
                    n_var_internal.On_InternalVariableValue_Changed += (() =>
                    {
                        field_int.value = data.variable.GetValue<int>();
                    });
                    break;
                case VariableType.Bool:
                    Toggle field_bool = util_XGraphInspectorGUI.GUI_Field_Bool(container, "变量值：", data.variable.GetValue<bool>(), new string[1] { "field_bool" });
                    n_var_internal.On_InternalVariableValue_Changed += (() =>
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
                case VariableType.Vector2:
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
                    n_var_internal.On_InternalVariableValue_Changed += (() =>
                    {
                        field_vector2.value = data.variable.GetValue<Vector2>();
                    });
                    break;
                case VariableType.Vector3:
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
                    n_var_internal.On_InternalVariableValue_Changed += (() =>
                    {
                        field_vector3.value = data.variable.GetValue<Vector3>();
                    });
                    break;
                case VariableType.Vector4:
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
                    n_var_internal.On_InternalVariableValue_Changed += (() =>
                    {
                        field_vector4.value = data.variable.GetValue<Vector4>();
                    });
                    break;
                case VariableType.Color:
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
                    n_var_internal.On_InternalVariableValue_Changed += (() =>
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
        private void GUI_VariableNode(VNode_Variable n_variable)
        {
            var data = n_variable.VariableData;

            if (data == null)
                return;

            // 创建布局容器
            VisualElement container = util_XGraphInspectorGUI.GUI_Container(this, new string[1] { "container" });

            // 标题
            Color themeColor = Color.clear;
            foreach (var theme in graphwindow.xw_BlackBoardView.VariableThemeList.VariableThemes)
            {
                if (theme.type == data.type.ToString())
                {
                    themeColor = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            }
            string[] styles_group = new string[1] { "titlegroup" };
            string[] styles_mark = new string[1] { "titlemark" };
            string[] styles_title = new string[1] { "titlename" };
            string[] styles_sub = new string[1] { "type" };
            util_XGraphInspectorGUI.GUI_Title(container, themeColor, data.name, data.variable.type.ToString(), styles_group, styles_mark, styles_title, styles_sub);

            // 解释
            util_XGraphInspectorGUI.GUI_Label(container, data.description, new string[1] { "description" });

            // var_guid
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-V： </b><color=#b1b1b1>{data.varguid}</color>", new string[1] { "labeltext" });

            // node_guid
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N： </b><color=#b1b1b1>{data.guid}</color>", new string[1] { "labeltext" });

            // node_pos
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>尺寸： </b> <color=#b1b1b1>X：{data.size.x.ToString()}    Y：{data.size.y.ToString()}</color>", new string[1] { "labeltext" });

            // node_size
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>位置： </b> <color=#b1b1b1>X：{data.position.x.ToString()}    Y：{data.position.y.ToString()}</color>", new string[1] { "labeltext" });

            #region 值
            switch (data.type)
            {
                case VariableType.String:
                    TextField field_string = util_XGraphInspectorGUI.GUI_Field_String(container, "变量值：", data.variable.GetValue<string>(), new string[1] { "field_text" });
                    field_string.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_string.value);
                        SetBlackBoardVariableValue(field_string.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_string.value}");
                    });
                    field_string.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_string.value);
                        SetBlackBoardVariableValue(field_string.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_string.value}");
                    });
                    break;
                case VariableType.Float:
                    FloatField field_float = util_XGraphInspectorGUI.GUI_Field_Float(container, "变量值：", data.variable.GetValue<float>(), new string[1] { "field_float" });
                    field_float.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_float.value);
                        SetBlackBoardVariableValue(field_float.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_float.value}");
                    });
                    field_float.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_float.value);
                        SetBlackBoardVariableValue(field_float.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_float.value}");
                    });
                    break;
                case VariableType.Int:
                    IntegerField field_int = util_XGraphInspectorGUI.GUI_Field_Int(container, "变量值：", data.variable.GetValue<int>(), new string[1] { "field_int" });
                    field_int.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_int.value);
                        SetBlackBoardVariableValue(field_int.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_int.value}");
                    });
                    field_int.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_int.value);
                        SetBlackBoardVariableValue(field_int.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_int.value}");
                    });
                    break;
                case VariableType.Bool:
                    Toggle field_bool = util_XGraphInspectorGUI.GUI_Field_Bool(container, "变量值：", data.variable.GetValue<bool>(), new string[1] { "field_bool" });
                    field_bool.RegisterValueChangedCallback((value) =>
                    {
                        data.variable.SetValue(field_bool.value);
                        SetBlackBoardVariableValue(field_bool.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_bool.value}");
                    });
                    field_bool.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_bool.value);
                        SetBlackBoardVariableValue(field_bool.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_bool.value}");
                    });
                    break;
                case VariableType.Vector2:
                    Vector2Field field_vector2 = util_XGraphInspectorGUI.GUI_Field_Vector2(container, "变量值：", data.variable.GetValue<Vector2>(), new string[1] { "field_vector2" });
                    field_vector2.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_vector2.value);
                        SetBlackBoardVariableValue(field_vector2.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector2.value}");
                    });
                    field_vector2.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_vector2.value);
                        SetBlackBoardVariableValue(field_vector2.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector2.value}");
                    });
                    break;
                case VariableType.Vector3:
                    Vector3Field field_vector3 = util_XGraphInspectorGUI.GUI_Field_Vector3(container, "变量值：", data.variable.GetValue<Vector3>(), new string[1] { "field_vecto3" });
                    field_vector3.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_vector3.value);
                        SetBlackBoardVariableValue(field_vector3.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector3.value}");
                    });
                    field_vector3.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_vector3.value);
                        SetBlackBoardVariableValue(field_vector3.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector3.value}");
                    });
                    break;
                case VariableType.Vector4:
                    Vector4Field field_vector4 = util_XGraphInspectorGUI.GUI_Field_Vector4(container, "变量值：", data.variable.GetValue<Vector4>(), new string[1] { "field_vector4" });
                    field_vector4.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_vector4.value);
                        SetBlackBoardVariableValue(field_vector4.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector4.value}");
                    });
                    field_vector4.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_vector4.value);
                        SetBlackBoardVariableValue(field_vector4.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_vector4.value}");
                    });
                    break;
                case VariableType.Color:
                    ColorField field_color = util_XGraphInspectorGUI.GUI_Field_Color(container, "变量值：", data.variable.GetValue<Color>(), new string[1] { "field_color" });
                    field_color.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(field_color.value);
                        SetBlackBoardVariableValue(field_color.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_color.value}");
                    });
                    field_color.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(field_color.value);
                        SetBlackBoardVariableValue(field_color.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                        graphwindow.xw_SetNodeInfo_Footer($"{data.variable.GetActiveType()}  /  {field_color.value}");
                    });
                    break;
            }
            #endregion
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
            VisualElement container = util_XGraphInspectorGUI.GUI_Container(this, new string[1] { "container" });

            Color themeColor = Color.clear;
            foreach (var theme in graphwindow.xw_BlackBoardView.VariableThemeList.VariableThemes)
            {
                if (theme.type == vare.type.ToString())
                {
                    themeColor = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            }

            // 标题
            string[] styles_group = new string[1] { "titlegroup" };
            string[] styles_mark = new string[1] { "titlemark" };
            string[] styles_title = new string[1] { "titlename" };
            string[] styles_sub = new string[1] { "type" };
            util_XGraphInspectorGUI.GUI_Title(container, themeColor, vare.name, vare.type.ToString(), styles_group, styles_mark, styles_title, styles_sub);

            // 解释
            util_XGraphInspectorGUI.GUI_Label(container, vare.description, new string[1] { "description" });

            // GUID-V
            util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-V： </b> <color=#b1b1b1>{vare.guid}</color>", new string[1] { "labeltext" });

            #region 值
            switch (vare.type)
            {
                case VariableType.String:
                    TextField field_string = util_XGraphInspectorGUI.GUI_Field_String(container, "变量值：", vare.GetValue<string>(), new string[1] { "field_text" });
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
                case VariableType.Float:
                    FloatField field_float = util_XGraphInspectorGUI.GUI_Field_Float(container, "变量值：", vare.GetValue<float>(), new string[1] { "field_float" });
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
                case VariableType.Int:
                    IntegerField field_int = util_XGraphInspectorGUI.GUI_Field_Int(container, "变量值：", vare.GetValue<int>(), new string[1] { "field_int" });
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
                case VariableType.Bool:
                    Toggle field_bool = util_XGraphInspectorGUI.GUI_Field_Bool(container, "变量值：", vare.GetValue<bool>(), new string[1] { "field_bool" });
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
                case VariableType.Vector2:
                    Vector2Field field_vector2 = util_XGraphInspectorGUI.GUI_Field_Vector2(container, "变量值：", vare.GetValue<Vector2>(), new string[1] { "field_vector2" });
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
                case VariableType.Vector3:
                    Vector3Field field_vector3 = util_XGraphInspectorGUI.GUI_Field_Vector3(container, "变量值：", vare.GetValue<Vector3>(), new string[1] { "field_vecto3" });
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
                case VariableType.Vector4:
                    Vector4Field field_vector4 = util_XGraphInspectorGUI.GUI_Field_Vector4(container, "变量值：", vare.GetValue<Vector4>(), new string[1] { "field_vector4" });
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
                case VariableType.Color:
                    ColorField field_color = util_XGraphInspectorGUI.GUI_Field_Color(container, "变量值：", vare.GetValue<Color>(), new string[1] { "field_color" });
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
        #endregion
    }
}