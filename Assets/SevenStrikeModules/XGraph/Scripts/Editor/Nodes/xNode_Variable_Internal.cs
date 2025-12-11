namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Linq;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Variable_Internal : xNode_Base
    {
        /// <summary>
        /// 视觉节点尺寸控制图标
        /// </summary>
        public VisualElement ResizerIcon;
        /// <summary>
        /// 视觉节点尺寸控制组件
        /// </summary>
        public VisualElement Resizer;
        /// <summary>
        /// 视觉开关组件
        /// </summary>
        public VisualElement VisualToggle;
        /// <summary>
        /// 开关背景
        /// </summary>
        public VisualElement tog_pill;
        /// <summary>
        /// 开关控制柄根物体（控制柄阴影）
        /// </summary>
        public VisualElement tog_handleRoot;
        /// <summary>
        /// 开关点击器
        /// </summary>
        public VisualElement tog_clicker;
        /// <summary>
        /// 数据输入框
        /// </summary>
        public VisualElement VariableField;

        public xAction_Variable VariableData;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            // 设置节点的容器样式
            util_XGraphEditorUtility.ElementStyle_Add(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_VariableInternalNode.uss");

            xAction_Variable actionvar = VariableData = data as xAction_Variable;

            if (VariableData.variable.type == xVariableType.Bool)
            {
                // 获取视觉开关的元素模版
                util_XGraphEditorUtility.ElementStyle_Add(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_VisualToggle.uss");

                var visual_toggle = util_XGraphEditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_VisualToggle.uxml");
                VisualToggle = visual_toggle.CloneTree().Q<VisualElement>(name: "visualtoggle");
                AppendElement(xNodeContainerType.ExtensionContainer, VisualToggle);

                tog_pill = VisualToggle.Q<VisualElement>(name: "pill");
                tog_handleRoot = tog_pill.Q<VisualElement>(name: "handleRoot");
                tog_clicker = VisualToggle.Q<VisualElement>(name: "clicker");

                Toggle_Check(VariableData.variable.GetValue<bool>());

                tog_clicker.RegisterCallback<PointerDownEvent>(ToggleClicker);

                ActionData.BaseArgs.RootAsset.On_VariablesValue_Changed += () =>
                {
                    Toggle_Check(VariableData.variable.GetValue<bool>());
                };
            }

            if (VariableData.variable.type == xVariableType.String)
            {
                AddToClassList("rootcontainer");

                // 指定可调整大小
                capabilities |= Capabilities.Resizable;

                style.width = data.BaseArgs.nodeGraphSize.x;
                style.height = data.BaseArgs.nodeGraphSize.y;
            }

            // 动态获取变量类型
            string asm = "Assembly-CSharp";
            Type var_type = Type.GetType($"SevenStrikeModules.XGraph.Variable_{actionvar.variable.type}, {asm}");

            #region 端口设置
            // 加入端口：单输入，类型：根据传入 variable.type 参数解析
            Port_Inputs.Add(new xGraph_NodePort("", var_type, Port.Capacity.Single));
            // 加入端口：多输出，类型：根据传入 variable.type 参数解析
            Port_Outputs.Add(new xGraph_NodePort("", var_type, Port.Capacity.Multi));
            #endregion

            // 当Graphview编辑器的主题色改变时
            graphView.gv_GraphWindow.OnThemeColorChanged += OnGraphViewEditorThemeColorChanged;

            // 每次初始化时先清空，避免重复注册
            VariableData.On_InternalVariableValue_Changed = null;
        }

        /// <summary>
        /// 可改变尺寸
        /// </summary>
        /// <returns></returns>
        public override bool IsResizable()
        {
            if (VariableData.variable.type == xVariableType.String)
                return true;
            else
                return false;
        }

        #region 节点绘制
        public override xNode_Base Draw()
        {
            // 绘制主容器
            Draw_Main();

            // 绘制标题容器
            Draw_Title();

            // 绘制标题按钮容器
            Draw_TitleButton();

            // 绘制顶部容器
            Draw_Top();

            // 绘制输入节点容器
            Draw_Input();

            // 绘制输出节点容器
            Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }

        public override void Draw_Main()
        {
            base.Draw_Main();

            if (VariableData.variable.type == xVariableType.String)
            {
                // 拖拽尺寸控件图标
                ResizerIcon = this.Q<VisualElement>(className: "resizer-icon");
                ResizerIcon.pickingMode = PickingMode.Ignore;
                ResizerIcon.style.opacity = 0f;

                // 拖拽尺寸控件
                Resizer = this.Q<VisualElement>(className: "resizer");
                Resizer.style.width = 30;
                Resizer.style.height = 30;
                Resizer.RegisterCallback<PointerEnterEvent>(DisplayResizer);
                Resizer.RegisterCallback<PointerLeaveEvent>(HideResizer);
            }
        }

        public override void Draw_Extension()
        {
            //base.Draw_Extension();

            // 创建控件
            VariableField = CreateField();

            if (VariableField != null)
                AppendElement(xNodeContainerType.ExtensionContainer, VariableField);
        }

        /// <summary>
        /// 创建相对应节点的变量类型的控件
        /// </summary>
        /// <returns></returns>
        private VisualElement CreateField()
        {
            VisualElement element = null;

            switch (VariableData.variable.type)
            {
                case xVariableType.String:
                    TextField f_string = new TextField();
                    f_string.name = "Field_String";
                    //f_string.multiline = true;
                    f_string.value = VariableData.variable.GetValue<string>();
                    f_string.RegisterCallback<BlurEvent>(VariableDataChanged_String);
                    f_string.AddToClassList("value_field_string");
                    f_string.Q(name: "unity-text-input").AddToClassList("value_field_string_input");
                    f_string.Q(className: "unity-text-element").AddToClassList("value_field_string_text-element");
                    element = f_string;
                    break;
                case xVariableType.Float:
                    FloatField f_float = new FloatField();
                    f_float.name = "Field_Float";
                    f_float.value = VariableData.variable.GetValue<float>();
                    f_float.RegisterCallback<BlurEvent>(VariableDataChanged_Float);
                    TextElement float_ele = f_float.Q<TextElement>();
                    float_ele.AddToClassList("value_field_text");
                    float_ele.AddToClassList("value_field_center_text");
                    element = f_float;
                    element.AddToClassList("value_field");
                    break;
                case xVariableType.Int:
                    IntegerField f_int = new IntegerField();
                    f_int.name = "Field_Int";
                    f_int.value = VariableData.variable.GetValue<int>();
                    f_int.RegisterCallback<BlurEvent>(VariableDataChanged_Int);
                    TextElement int_ele = f_int.Q<TextElement>();
                    int_ele.AddToClassList("value_field_text");
                    int_ele.AddToClassList("value_field_center_text");
                    element = f_int;
                    element.AddToClassList("value_field");
                    break;
                case xVariableType.Bool:
                    break;
                case xVariableType.Vector2:
                    Vector2Field f_vec2 = new Vector2Field();
                    f_vec2.name = "Field_Vector2";
                    f_vec2.value = VariableData.variable.GetValue<Vector2>();
                    f_vec2.RegisterCallback<BlurEvent>(VariableDataChanged_Vector2);

                    FloatField v2_field_input_x = f_vec2.Q<FloatField>("unity-x-input");
                    FloatField v2_field_input_y = f_vec2.Q<FloatField>("unity-y-input");

                    v2_field_input_x.AddToClassList("value_field_text");
                    v2_field_input_x.Q<Label>().AddToClassList("value_field_vector_labelColor_x");

                    v2_field_input_y.AddToClassList("value_field_text");
                    v2_field_input_y.Q<Label>().AddToClassList("value_field_vector_labelColor_y");

                    element = f_vec2;
                    element.AddToClassList("value_field");
                    break;
                case xVariableType.Vector3:
                    Vector3Field f_vec3 = new Vector3Field();
                    f_vec3.name = "Field_Vector3";
                    f_vec3.AddToClassList("value_field_vector3");
                    f_vec3.value = VariableData.variable.GetValue<Vector3>();
                    f_vec3.RegisterCallback<BlurEvent>(VariableDataChanged_Vector3);

                    FloatField v3_field_input_x = f_vec3.Q<FloatField>("unity-x-input");
                    FloatField v3_field_input_y = f_vec3.Q<FloatField>("unity-y-input");
                    FloatField v3_field_input_z = f_vec3.Q<FloatField>("unity-z-input");

                    v3_field_input_x.AddToClassList("value_field_text");
                    v3_field_input_x.Q<Label>().AddToClassList("value_field_vector_labelColor_x");

                    v3_field_input_y.AddToClassList("value_field_text");
                    v3_field_input_y.Q<Label>().AddToClassList("value_field_vector_labelColor_y");

                    v3_field_input_z.AddToClassList("value_field_text");
                    v3_field_input_z.Q<Label>().AddToClassList("value_field_vector_labelColor_z");

                    element = f_vec3;
                    element.AddToClassList("value_field");
                    break;
                case xVariableType.Vector4:
                    Vector4Field f_vec4 = new Vector4Field();
                    f_vec4.name = "Field_Vector4";
                    f_vec4.AddToClassList("value_field_vector4");
                    f_vec4.value = VariableData.variable.GetValue<Vector4>();
                    f_vec4.RegisterCallback<BlurEvent>(VariableDataChanged_Vector4);

                    FloatField v4_field_input_x = f_vec4.Q<FloatField>("unity-x-input");
                    FloatField v4_field_input_y = f_vec4.Q<FloatField>("unity-y-input");
                    FloatField v4_field_input_z = f_vec4.Q<FloatField>("unity-z-input");
                    FloatField v4_field_input_w = f_vec4.Q<FloatField>("unity-w-input");

                    v4_field_input_x.AddToClassList("value_field_text");
                    v4_field_input_x.Q<Label>().AddToClassList("value_field_vector_labelColor_x");

                    v4_field_input_y.AddToClassList("value_field_text");
                    v4_field_input_y.Q<Label>().AddToClassList("value_field_vector_labelColor_y");

                    v4_field_input_z.AddToClassList("value_field_text");
                    v4_field_input_z.Q<Label>().AddToClassList("value_field_vector_labelColor_z");

                    v4_field_input_w.AddToClassList("value_field_text");
                    v4_field_input_w.Q<Label>().AddToClassList("value_field_vector_labelColor_w");

                    element = f_vec4;
                    element.AddToClassList("value_field");
                    break;
                case xVariableType.Color:
                    ColorField f_color = new ColorField();
                    f_color.name = "Field_Color";
                    f_color.AddToClassList("value_field");
                    f_color.value = VariableData.variable.GetValue<Color>();
                    f_color.RegisterValueChangedCallback(VariableDataChanged_Color);
                    element = f_color;
                    break;
            }

            return element;
        }

        public override void Draw_Title()
        {
            base.Draw_Title();

            TitleInputField.RegisterCallback<BlurEvent>(SyncChangeVariableName);

            // 类型
            Label lab_sub = new Label(VariableData.variable.type.ToString().ToLower());
            lab_sub.name = "var_type";
            //lab_sub.style.color = ActionData.RootAsset.GraphviewGridBackgroundThemes.themecolor;
            lab_sub.AddToClassList("variable_type");
            titleContainer.Add(lab_sub);
        }
        #endregion

        #region 视觉开关
        /// <summary>
        /// 开关打开
        /// </summary>
        public void Toggle_On()
        {
            util_XGraphEditorUtility.Element_BackgroundColorTint_Set(tog_pill, graphView.ActionTreeAsset.GraphviewGridBackgroundThemes.themecolor);
            tog_handleRoot.style.left = 15;
        }

        /// <summary>
        /// 开关关闭
        /// </summary>
        public void Toggle_Off()
        {
            util_XGraphEditorUtility.Element_BackgroundColorTint_Set(tog_pill, Color.gray);
            tog_handleRoot.style.left = -11;
        }

        /// <summary>
        /// 检查开关
        /// </summary>
        /// <param name="state"></param>
        public void Toggle_Check(bool state)
        {
            if (state)
            {
                Toggle_On();
            }
            else
            {
                Toggle_Off();
            }
        }
        #endregion

        #region 改变节点值时的回调
        /// <summary>
        /// 当字符串类型的变量节点内容改变时
        /// </summary>
        /// <param name="evt"></param>
        private void VariableDataChanged_String(BlurEvent evt)
        {
            TextField field = evt.target as TextField;
            if (VariableData.BaseArgs.VariableDatas.Count > 0)
            {
                field.value = VariableData.BaseArgs.VariableDatas.First().variable.GetValue<string>();
                return;
            }

            //Undo.RecordObject(VariableData, "Change NodeData String Variable");
            VariableData.variable.SetValue<string>(field.text);
            // 更新变量值数据
            graphView.ActionTreeAsset.Variables_Refresh();
            // 通知注册了变量数值改变的回调
            if (VariableData.On_InternalVariableValue_Changed != null)
            {
                VariableData.On_InternalVariableValue_Changed();
            }
            // 更新节点信息内容显示
            VariableNodeInfoRefresh();
        }
        /// <summary>
        /// 当 Float 类型的变量节点内容改变时
        /// </summary>
        /// <param name="evt"></param>
        private void VariableDataChanged_Float(BlurEvent evt)
        {
            FloatField field = evt.target as FloatField;
            if (VariableData.BaseArgs.VariableDatas.Count > 0)
            {
                field.value = VariableData.BaseArgs.VariableDatas.First().variable.GetValue<float>();
                return;
            }
            //Undo.RecordObject(VariableData, "Change NodeData Float Variable");
            VariableData.variable.SetValue<float>(field.value);
            // 更新变量值数据
            graphView.ActionTreeAsset.Variables_Refresh();
            // 通知注册了变量数值改变的回调
            if (VariableData.On_InternalVariableValue_Changed != null)
            {
                VariableData.On_InternalVariableValue_Changed();
            }
            // 更新节点信息内容显示
            VariableNodeInfoRefresh();
        }
        /// <summary>
        /// 当 Int 类型的变量节点内容改变时
        /// </summary>
        /// <param name="evt"></param>
        private void VariableDataChanged_Int(BlurEvent evt)
        {
            IntegerField field = evt.target as IntegerField;
            if (VariableData.BaseArgs.VariableDatas.Count > 0)
            {
                field.value = VariableData.BaseArgs.VariableDatas.First().variable.GetValue<int>();
                return;
            }
            //Undo.RecordObject(VariableData, "Change NodeData Int Variable");
            VariableData.variable.SetValue<int>(field.value);
            // 更新变量值数据
            graphView.ActionTreeAsset.Variables_Refresh();
            // 通知注册了变量数值改变的回调
            if (VariableData.On_InternalVariableValue_Changed != null)
            {
                VariableData.On_InternalVariableValue_Changed();
            }
            // 更新节点信息内容显示
            VariableNodeInfoRefresh();
        }
        /// <summary>
        /// 当 Vector2 类型的变量节点内容改变时
        /// </summary>
        /// <param name="evt"></param>
        private void VariableDataChanged_Vector2(BlurEvent evt)
        {
            Vector2Field field = evt.target as Vector2Field;
            if (VariableData.BaseArgs.VariableDatas.Count > 0)
            {
                field.value = VariableData.BaseArgs.VariableDatas.First().variable.GetValue<Vector2>();
                return;
            }
            //Undo.RecordObject(VariableData, "Change NodeData Vector2 Variable");
            VariableData.variable.SetValue<Vector2>(field.value);
            // 更新变量值数据
            graphView.ActionTreeAsset.Variables_Refresh();
            // 通知注册了变量数值改变的回调
            if (VariableData.On_InternalVariableValue_Changed != null)
            {
                VariableData.On_InternalVariableValue_Changed();
            }
            // 更新节点信息内容显示
            VariableNodeInfoRefresh();
        }
        /// <summary>
        /// 当 Vector3 类型的变量节点内容改变时
        /// </summary>
        /// <param name="evt"></param>
        private void VariableDataChanged_Vector3(BlurEvent evt)
        {
            Vector3Field field = evt.target as Vector3Field;
            if (VariableData.BaseArgs.VariableDatas.Count > 0)
            {
                field.value = VariableData.BaseArgs.VariableDatas.First().variable.GetValue<Vector3>();
                return;
            }
            //Undo.RecordObject(VariableData, "Change NodeData Vector3 Variable");
            VariableData.variable.SetValue<Vector3>(field.value);
            // 更新变量值数据
            graphView.ActionTreeAsset.Variables_Refresh();
            // 通知注册了变量数值改变的回调
            if (VariableData.On_InternalVariableValue_Changed != null)
            {
                VariableData.On_InternalVariableValue_Changed();
            }
            // 更新节点信息内容显示
            VariableNodeInfoRefresh();
        }
        /// <summary>
        /// 当 Vector4 类型的变量节点内容改变时
        /// </summary>
        /// <param name="evt"></param>
        private void VariableDataChanged_Vector4(BlurEvent evt)
        {
            Vector4Field field = evt.target as Vector4Field;
            if (VariableData.BaseArgs.VariableDatas.Count > 0)
            {
                field.value = VariableData.BaseArgs.VariableDatas.First().variable.GetValue<Vector4>();
                return;
            }
            //Undo.RecordObject(VariableData, "Change NodeData Vector4 Variable");
            VariableData.variable.SetValue<Vector4>(field.value);
            // 更新变量值数据
            graphView.ActionTreeAsset.Variables_Refresh();
            // 通知注册了变量数值改变的回调
            if (VariableData.On_InternalVariableValue_Changed != null)
            {
                VariableData.On_InternalVariableValue_Changed();
            }
            // 更新节点信息内容显示
            VariableNodeInfoRefresh();
        }
        /// <summary>
        /// 当 Color 类型的变量节点内容改变时
        /// </summary>
        /// <param name="evt"></param>
        private void VariableDataChanged_Color(ChangeEvent<Color> evt)
        {
            ColorField field = evt.target as ColorField;
            if (VariableData.BaseArgs.VariableDatas.Count > 0)
            {
                field.value = VariableData.BaseArgs.VariableDatas[0].variable.GetValue<Color>();
                return;
            }
            //Undo.RecordObject(VariableData, "Change NodeData Color Variable");
            VariableData.variable.SetValue<Color>(field.value);
            // 更新变量值数据
            graphView.ActionTreeAsset.Variables_Refresh();
            // 通知注册了变量数值改变的回调
            if (VariableData.On_InternalVariableValue_Changed != null)
            {
                VariableData.On_InternalVariableValue_Changed();
            }
            // 更新节点信息内容显示
            VariableNodeInfoRefresh();
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 更新节点信息内容显示
        /// </summary>
        private void VariableNodeInfoRefresh()
        {
            // 显示当前选中的节点的类型信息
            graphView.gv_GraphWindow.xw_SetNodeInfos($"{VariableData.variable.name}  /  {VariableData.variable.GetActiveType()}  /  {VariableData.variable.guid}", $"{VariableData.variable.description}  /  {VariableData.variable.GetValue()}");
        }
        #endregion

        #region 回调
        /// <summary>
        /// 当Graphview编辑器的主题色改变时
        /// </summary>
        /// <param name="color"></param>
        private void OnGraphViewEditorThemeColorChanged(Color color)
        {
            if (tog_pill != null && VariableData.variable.GetValue<bool>())
                util_XGraphEditorUtility.Element_BackgroundColorTint_Set(tog_pill, graphView.ActionTreeAsset.GraphviewGridBackgroundThemes.themecolor);
        }
        #endregion

        #region 其他
        /// <summary>
        /// 改变节点名称的同时同步修改变量类的名称
        /// </summary>
        /// <param name="evt"></param>
        private void SyncChangeVariableName(BlurEvent evt)
        {
            VariableData.variable.name = VariableData.identifyName;

            // Inspector 面板显示属性
            graphView.gv_GraphWindow.xw_InspectorView.InspectorViewer(this);
        }
        /// <summary>
        /// 鼠标移出时隐藏角点拖拽显示
        /// </summary>
        /// <param name="evt"></param>
        private void HideResizer(PointerLeaveEvent evt)
        {
            ResizerIcon.style.opacity = 0f;
        }
        /// <summary>
        /// 鼠标进入时显示角点拖拽显示
        /// </summary>
        /// <param name="evt"></param>
        private void DisplayResizer(PointerEnterEvent evt)
        {
            ResizerIcon.style.opacity = 1f;
        }
        /// <summary>
        /// 开关点击事件
        /// </summary>
        /// <param name="evt"></param>
        private void ToggleClicker(PointerDownEvent evt)
        {
            if (VariableData.BaseArgs.VariableDatas.Count > 0)
            {
                Toggle_Check(VariableData.BaseArgs.VariableDatas.First().variable.GetValue<bool>());
                return;
            }
            //Undo.RecordObject(VariableData, "Changed Toggle State");

            bool sw = VariableData.variable.GetValue<bool>();
            sw = !sw;
            VariableData.variable.SetValue<bool>(sw);
            Toggle_Check(sw);

            // 更新变量值数据
            graphView.ActionTreeAsset.Variables_Refresh();
            // 通知注册了变量数值改变的回调
            if (VariableData.On_InternalVariableValue_Changed != null)
            {
                VariableData.On_InternalVariableValue_Changed();
            }

            // 更新节点信息内容显示
            VariableNodeInfoRefresh();
        }
        /// <summary>
        /// 刷新控件值
        /// </summary>
        public void UpdateFieldValue()
        {
            // 获取控件
            VisualElement valuefield = extensionContainer.Q<VisualElement>(name: $"Field_{VariableData.variable.type}");
            // 根据自身变量类型刷新控件值
            switch (VariableData.variable.type)
            {
                //字符串
                case xVariableType.String:
                    // 目标控件
                    TextField value_String = valuefield as TextField;
                    if (value_String != null)
                    {
                        // 从变量数据中获取值给控件
                        value_String.value = VariableData.variable.GetValue<string>();
                    }
                    break;
                //浮点
                case xVariableType.Float:
                    // 目标控件
                    FloatField value_Float = valuefield as FloatField;
                    if (value_Float != null)
                    {
                        // 从变量数据中获取值给控件
                        value_Float.value = VariableData.variable.GetValue<float>();
                    }
                    break;
                //整型
                case xVariableType.Int:
                    // 目标控件
                    IntegerField value_Int = valuefield as IntegerField;
                    if (value_Int != null)
                    {
                        // 从变量数据中获取值给控件
                        value_Int.value = VariableData.variable.GetValue<int>();
                    }
                    break;
                //布尔
                case xVariableType.Bool:
                    // 目标控件
                    Toggle value_Bool = valuefield as Toggle;
                    if (value_Bool != null)
                    {
                        // 从变量数据中获取值给控件
                        value_Bool.value = VariableData.variable.GetValue<bool>();
                    }
                    break;
                //2维向量
                case xVariableType.Vector2:
                    // 目标控件
                    Vector2Field value_Vector2 = valuefield as Vector2Field;
                    if (value_Vector2 != null)
                    {
                        // 从变量数据中获取值给控件
                        value_Vector2.value = VariableData.variable.GetValue<Vector2>();
                    }
                    break;
                //3维向量
                case xVariableType.Vector3:
                    // 目标控件
                    Vector3Field value_Vector3 = valuefield as Vector3Field;
                    if (value_Vector3 != null)
                    {
                        // 从变量数据中获取值给控件
                        value_Vector3.value = VariableData.variable.GetValue<Vector3>();
                    }
                    break;
                //4维向量
                case xVariableType.Vector4:
                    // 目标控件
                    Vector4Field value_Vector4 = valuefield as Vector4Field;
                    if (value_Vector4 != null)
                    {
                        // 从变量数据中获取值给控件
                        value_Vector4.value = VariableData.variable.GetValue<Vector4>();
                    }
                    break;
                //颜色
                case xVariableType.Color:
                    // 目标控件
                    ColorField value_Color = valuefield as ColorField;
                    if (value_Color != null)
                    {
                        // 从变量数据中获取值给控件
                        value_Color.value = VariableData.variable.GetValue<Color>();
                    }
                    break;
            }
        }
        #endregion

        #region 重写
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();
            UpdateFieldValue();
        }
        #endregion
    }
}