namespace SevenStrikeModules.XGraph
{
    using System;
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
            // 指定样式
            util_XGraphEditorUtility.ElementStyle_Add(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_Inspector.uss");
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
        /// 创建黑板变量节点的属性面板
        /// </summary>
        /// <param name="target"></param>
        private void GUI_VariableNode(VNode_Variable n_variable)
        {
            var data = n_variable.VariableData;

            if (data == null)
                return;

            VisualElement container = new VisualElement();
            container.name = $"Inspector_{data.name}";
            container.AddToClassList("container");
            this.Add(container);

            #region 标题
            // 标题组
            VisualElement titlegroup = new VisualElement();
            titlegroup.name = "titlegroup";
            titlegroup.AddToClassList("titlegroup");
            container.Add(titlegroup);
            // 标记
            VisualElement titlemark = new VisualElement();
            titlemark.name = "titlemark";
            titlemark.AddToClassList("titlemark");
            foreach (var theme in graphwindow.xw_BlackBoardView.VariableThemeList.VariableThemes)
            {
                if (theme.type == data.type.ToString())
                {
                    titlemark.style.backgroundColor = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            }
            titlegroup.Add(titlemark);
            // 标题
            Label varname = new Label($"{data.name}");
            varname.AddToClassList("titlename");
            titlegroup.Add(varname);
            // 类型
            Label vartype = new Label($"{data.variable.type}");
            vartype.AddToClassList("type");
            titlegroup.Add(vartype);
            #endregion

            #region 参数
            // 解释
            Label vardescription = new Label($"{data.description}");
            vardescription.AddToClassList("description");
            container.Add(vardescription);

            // var_guid
            Label varguid = new Label();
            varguid.text = $"<b>GUID-V： </b><color=#b1b1b1>{data.varguid}</color>";
            varguid.AddToClassList("labeltext");
            container.Add(varguid);

            // node_guid
            Label nodeguid = new Label();
            nodeguid.text = $"<b>GUID-N： </b><color=#b1b1b1>{data.guid}</color>";
            nodeguid.AddToClassList("labeltext");
            container.Add(nodeguid);

            // 尺寸
            Label size = new Label();
            size.text = $"<b>尺寸： </b> <color=#b1b1b1>X：{data.size.x.ToString()}    Y：{data.size.y.ToString()}</color>";
            size.AddToClassList("labeltext");
            container.Add(size);

            // 位置
            Label pos = new Label("位置：");
            pos.text = $"<b>位置： </b> <color=#b1b1b1>X：{data.position.x.ToString()}    Y：{data.position.y.ToString()}</color>";
            pos.AddToClassList("labeltext");
            container.Add(pos);
            #endregion

            #region 值
            switch (data.type)
            {
                case VariableType.String:
                    // node_guid
                    TextField value_string = new TextField("变量值：");
                    value_string.value = data.variable.GetValue<string>();
                    value_string.AddToClassList("field_text");
                    container.Add(value_string);
                    value_string.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(value_string.value);
                        SetBlackBoardVariableValue(value_string.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_string.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(value_string.value);
                        SetBlackBoardVariableValue(value_string.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Float:
                    FloatField value_float = new FloatField("变量值：");
                    value_float.value = data.variable.GetValue<float>();
                    value_float.AddToClassList("field_float");
                    container.Add(value_float);
                    value_float.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(value_float.value);
                        SetBlackBoardVariableValue(value_float.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_float.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(value_float.value);
                        SetBlackBoardVariableValue(value_float.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Int:
                    IntegerField value_int = new IntegerField("变量值：");
                    value_int.value = data.variable.GetValue<int>();
                    value_int.AddToClassList("field_float");
                    container.Add(value_int);
                    value_int.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(value_int.value);
                        SetBlackBoardVariableValue(value_int.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_int.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(value_int.value);
                        SetBlackBoardVariableValue(value_int.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Bool:
                    Toggle value_bool = new Toggle("变量值：");
                    value_bool.value = data.variable.GetValue<bool>();
                    value_bool.AddToClassList("field_bool");
                    container.Add(value_bool);
                    value_bool.RegisterValueChangedCallback((value) =>
                    {
                        data.variable.SetValue(value_bool.value);
                        SetBlackBoardVariableValue(value_bool.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_bool.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(value_bool.value);
                        SetBlackBoardVariableValue(value_bool.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Vector2:
                    Vector2Field value_v2 = new Vector2Field("变量值：");
                    value_v2.value = data.variable.GetValue<Vector2>();
                    value_v2.AddToClassList("field_vector2");
                    container.Add(value_v2);
                    value_v2.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(value_v2.value);
                        SetBlackBoardVariableValue(value_v2.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_v2.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(value_v2.value);
                        SetBlackBoardVariableValue(value_v2.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Vector3:
                    Vector3Field value_v3 = new Vector3Field("变量值：");
                    value_v3.value = data.variable.GetValue<Vector3>();
                    value_v3.AddToClassList("field_vector3");
                    container.Add(value_v3);
                    value_v3.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(value_v3.value);
                        SetBlackBoardVariableValue(value_v3.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_v3.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(value_v3.value);
                        SetBlackBoardVariableValue(value_v3.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Vector4:
                    Vector4Field value_v4 = new Vector4Field("变量值：");
                    value_v4.value = data.variable.GetValue<Vector4>();
                    value_v4.AddToClassList("field_vector4");
                    container.Add(value_v4);
                    value_v4.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(value_v4.value);
                        SetBlackBoardVariableValue(value_v4.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_v4.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(value_v4.value);
                        SetBlackBoardVariableValue(value_v4.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Color:
                    ColorField value_color = new ColorField("变量值：");
                    value_color.value = data.variable.GetValue<Color>();
                    value_color.AddToClassList("field_color");
                    container.Add(value_color);
                    value_color.RegisterCallback<BlurEvent>((value) =>
                    {
                        data.variable.SetValue(value_color.value);
                        SetBlackBoardVariableValue(value_color.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_color.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        data.variable.SetValue(value_color.value);
                        SetBlackBoardVariableValue(value_color.value, data.varguid);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
            }
            #endregion
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
        /// 创建黑板变量项的属性面板
        /// </summary>
        /// <param name="vare"></param>
        private void GUI_BlackBoardVariable(Variable vare)
        {
            if (vare == null)
                return;

            VisualElement container = new VisualElement();
            container.name = $"Inspector_{vare.name}";
            container.AddToClassList("container");
            this.Add(container);

            #region 标题
            // 标题组
            VisualElement titlegroup = new VisualElement();
            titlegroup.name = "titlegroup";
            titlegroup.AddToClassList("titlegroup");
            container.Add(titlegroup);
            // 标记
            VisualElement titlemark = new VisualElement();
            titlemark.name = "titlemark";
            titlemark.AddToClassList("titlemark");
            foreach (var theme in graphwindow.xw_BlackBoardView.VariableThemeList.VariableThemes)
            {
                if (theme.type == vare.type.ToString())
                {
                    titlemark.style.backgroundColor = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            }
            titlegroup.Add(titlemark);
            // 标题
            Label varname = new Label($"{vare.name}");
            varname.AddToClassList("titlename");
            titlegroup.Add(varname);
            // 类型
            Label vartype = new Label($"{vare.type}");
            vartype.AddToClassList("type");
            titlegroup.Add(vartype);
            #endregion

            #region 参数
            // 解释
            Label vardescription = new Label($"{vare.description}");
            vardescription.AddToClassList("description");
            container.Add(vardescription);

            // var_guid
            Label varguid = new Label();
            varguid.text = $"<b>GUID-V： </b> <color=#b1b1b1>{vare.guid}</color>";
            varguid.AddToClassList("labeltext");
            container.Add(varguid);
            #endregion

            #region 值
            switch (vare.type)
            {
                case VariableType.String:
                    // node_guid
                    TextField value_string = new TextField("变量值：");
                    value_string.value = vare.GetValue<string>();
                    value_string.AddToClassList("field_text");
                    container.Add(value_string);
                    value_string.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(value_string.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_string.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(value_string.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Float:
                    FloatField value_float = new FloatField("变量值：");
                    value_float.value = vare.GetValue<float>();
                    value_float.AddToClassList("field_float");
                    container.Add(value_float);
                    value_float.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(value_float.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_float.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(value_float.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Int:
                    IntegerField value_int = new IntegerField("变量值：");
                    value_int.value = vare.GetValue<int>();
                    value_int.AddToClassList("field_float");
                    container.Add(value_int);
                    value_int.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(value_int.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_int.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(value_int.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Bool:
                    Toggle value_bool = new Toggle("变量值：");
                    value_bool.value = vare.GetValue<bool>();
                    value_bool.AddToClassList("field_bool");
                    container.Add(value_bool);
                    value_bool.RegisterValueChangedCallback((value) =>
                    {
                        vare.SetValue(value_bool.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_bool.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(value_bool.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Vector2:
                    Vector2Field value_v2 = new Vector2Field("变量值：");
                    value_v2.value = vare.GetValue<Vector2>();
                    value_v2.AddToClassList("field_vector2");
                    container.Add(value_v2);
                    value_v2.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(value_v2.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_v2.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(value_v2.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Vector3:
                    Vector3Field value_v3 = new Vector3Field("变量值：");
                    value_v3.value = vare.GetValue<Vector3>();
                    value_v3.AddToClassList("field_vector3");
                    container.Add(value_v3);
                    value_v3.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(value_v3.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_v3.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(value_v3.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Vector4:
                    Vector4Field value_v4 = new Vector4Field("变量值：");
                    value_v4.value = vare.GetValue<Vector4>();
                    value_v4.AddToClassList("field_vector4");
                    container.Add(value_v4);
                    value_v4.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(value_v4.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_v4.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(value_v4.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
                case VariableType.Color:
                    ColorField value_color = new ColorField("变量值：");
                    value_color.value = vare.GetValue<Color>();
                    value_color.AddToClassList("field_color");
                    container.Add(value_color);
                    value_color.RegisterCallback<BlurEvent>((value) =>
                    {
                        vare.SetValue(value_color.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    // 注册元素从面板分离时的事件（相当于销毁）
                    value_color.RegisterCallback<DetachFromPanelEvent>(evt =>
                    {
                        vare.SetValue(value_color.value);
                        graphwindow.CloneTree.Variables_Refresh();
                    });
                    break;
            }
            #endregion
        }

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