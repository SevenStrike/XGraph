namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class VNode_Label : Node
    {
        /// <summary>
        /// GraphView组件
        /// </summary>
        public xg_GraphView graphView;
        /// <summary>
        /// 标签内容组件
        /// </summary>
        public Label labelContentlabel;
        /// <summary>
        /// 字体大小按钮 - 增加尺寸
        /// </summary>
        public Button btn_fontsize_plus;
        /// <summary>
        /// 字体大小按钮 - 减小尺寸
        /// </summary>
        public Button btn_fontsize_minus;
        /// <summary>
        /// 字体粗细按钮
        /// </summary>
        public Button btn_font_bold;
        /// <summary>
        /// 字体粗细指示器
        /// </summary>
        public VisualElement font_bold_mark;
        /// <summary>
        /// 字体斜体按钮
        /// </summary>
        public Button btn_font_italic;
        /// <summary>
        /// 字体斜体指示器
        /// </summary>
        public VisualElement font_italic_mark;
        /// <summary>
        /// 字体尺寸调整按钮容器
        /// </summary>
        public VisualElement fontsizerContainer;
        /// <summary>
        /// 标签内容输入框组件
        /// </summary>
        public TextField labelContentInput;
        /// <summary>
        /// 标签内容尺寸大小输入框组件
        /// </summary>
        public IntegerField FontSizeInput;
        /// <summary>
        /// 视觉节点贴图尺寸控制图标
        /// </summary>
        public VisualElement ResizerIcon;
        /// <summary>
        /// 视觉节点贴图尺寸控制组件
        /// </summary>
        public VisualElement Resizer;
        /// <summary>
        /// 标签的最后一次尺寸
        /// </summary>
        private Vector2 m_LastSize;

        #region 节点信息
        /// <summary>
        /// 节点标题
        /// </summary>
        public string nodeTitle { get; set; }
        /// <summary>
        /// 节点解释内容
        /// </summary>
        public string nodeNote { get; set; }
        #endregion

        /// <summary>
        /// 节点携带的数据
        /// </summary>
        public ActionLabelData LabelData { get; set; }

        /// <summary>
        /// 初始化节点 - ActionStickData
        /// </summary>
        /// <param name="graphView"></param>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        public virtual void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionLabelData data = null)
        {
            // 指定可调整大小
            capabilities |= Capabilities.Resizable;

            // 指定GraphView 组件
            this.graphView = graphView;

            // 设置节点的容器样式
            util_XGraphEditorUtility.ElementStyle_Add(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_Label.uss");

            // 携带数据
            if (data != null)
                LabelData = data;

            style.width = data.size.x;
            style.height = data.size.y;

            #region 基础参数设置
            this.viewDataKey = data != null ? data.guid : "";
            // 设置节点标题
            this.title = this.nodeTitle = "";
            #endregion

            // 设置节点的生成位置
            SetPosition(new Rect(pos, Vector2.zero));

            // 监听尺寸变化事件
            RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

            // 当Graphview编辑器的主题色改变时
            graphView.gv_GraphWindow.OnThemeColorChanged += OnGraphViewEditorThemeColorChanged;
        }

        /// <summary>
        /// 当节点尺寸发生改变时
        /// </summary>
        /// <param name="evt"></param>
        private void OnSizeChanged(GeometryChangedEvent evt)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change Label Size");

            Vector2 newSize = new Vector2(evt.newRect.width, evt.newRect.height);

            if (Vector2.Distance(m_LastSize, newSize) > 1f) // 1像素阈值
            {
                m_LastSize = newSize;

                LabelData.size = newSize;
            }
        }

        /// <summary>
        /// 当拖动节点位置时，将位置数据传递给对应的目标数据节点位置变量
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change Label Position");
            base.SetPosition(newPos);

            if (LabelData != null)
            {
                LabelData.position.x = newPos.xMin;
                LabelData.position.y = newPos.yMin;
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();

            FontControlPanelDisplayer(true);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();

            FontControlPanelDisplayer(false);
        }

        /// <summary>
        /// 可改变尺寸
        /// </summary>
        /// <returns></returns>
        public override bool IsResizable()
        {
            return true;
        }

        #region 节点绘制
        /// <summary>
        /// 绘制节点
        /// </summary>
        public VNode_Label Draw()
        {
            // 绘制主容器
            Draw_Main();

            // 绘制标题容器
            Draw_Title();

            // 绘制顶部容器
            Draw_Top();

            return this;
        }

        public virtual void Draw_Title()
        {
            titleContainer.Clear();
        }

        /// <summary>
        /// 绘制顶部容器
        /// </summary>
        public virtual void Draw_Top()
        {
            #region 标签内容
            labelContentlabel = new Label(LabelData.content);
            labelContentlabel.enableRichText = true;

            // 读取数据中的文字特性
            util_XGraphEditorUtility.Element_Opacity_Set(labelContentlabel, LabelData.opacity);
            util_XGraphEditorUtility.Element_Label_SizeSet(labelContentlabel, LabelData.fontSize);
            util_XGraphEditorUtility.Element_Label_ItalicSet(labelContentlabel, LabelData.italic);
            util_XGraphEditorUtility.Element_Label_BoldSet(labelContentlabel, LabelData.bold);

            if (LabelData.italic)
                util_XGraphEditorUtility.Element_Opacity_Set(font_italic_mark, 1);
            else
                util_XGraphEditorUtility.Element_Opacity_Set(font_italic_mark, 0);
            if (LabelData.bold)
                util_XGraphEditorUtility.Element_Opacity_Set(font_bold_mark, 1);
            else
                util_XGraphEditorUtility.Element_Opacity_Set(font_bold_mark, 0);

            util_XGraphEditorUtility.Element_BackgroundColor_Set(font_bold_mark, graphView.ActionTreeAsset.GraphviewGridBackgroundThemes.themecolor);
            util_XGraphEditorUtility.Element_BackgroundColor_Set(font_italic_mark, graphView.ActionTreeAsset.GraphviewGridBackgroundThemes.themecolor);

            labelContentlabel.AddToClassList("Content_Label");
            labelContentlabel.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    VisualElementDisplay(labelContentlabel, false);
                    VisualElementDisplay(labelContentInput, true);

                    EditorApplication.delayCall += () =>
                    {
                        labelContentInput.Focus();
                    };
                    evt.StopPropagation();
                }
            });

            labelContentlabel.RegisterCallback<PointerEnterEvent>(OnDecalPointerEnter);
            labelContentlabel.RegisterCallback<PointerLeaveEvent>(OnDecalPointerLeave);
            labelContentlabel.RegisterCallback<PointerDownEvent>(OnDecalPointerDown);
            #endregion

            #region 标签内容输入框
            labelContentInput = new TextField();
            labelContentInput.value = LabelData.content;
            StyleLength len_width = labelContentInput.style.width;

            Length len_w = len_width.value;
            len_w.unit = LengthUnit.Percent;
            len_w.value = 100;

            len_width.value = len_w;
            labelContentInput.multiline = true;
            labelContentInput.AddToClassList("Content_TextField");
            labelContentInput.Q<VisualElement>(className: "unity-base-text-field__multiline-container").AddToClassList("sizefieldTextmultilinecontainer");
            labelContentInput.RegisterCallback<BlurEvent>(OnStickContentInputBlur);

            AppendElement(GraphNodeContainerType.TopContainer, labelContentlabel);
            AppendElement(GraphNodeContainerType.TopContainer, labelContentInput);
            #endregion
        }

        /// <summary>
        /// 绘制主容器
        /// </summary>
        public virtual void Draw_Main()
        {
            #region 按钮 - 字体尺寸增加
            btn_fontsize_plus = new Button();
            btn_fontsize_plus.AddToClassList("button");
            btn_fontsize_plus.text = "";
            btn_fontsize_plus.clicked += Btn_font_size_add_clicked;
            btn_fontsize_plus.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/fontsizeAdd.png");
            #endregion

            #region 按钮 - 字体尺寸减少
            btn_fontsize_minus = new Button();
            btn_fontsize_minus.AddToClassList("button");
            btn_fontsize_minus.text = "";
            btn_fontsize_minus.clicked += Btn_font_size_minus_clicked;
            btn_fontsize_minus.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/fontsizeMinus.png");
            #endregion

            #region 按钮 - 字体粗细
            btn_font_bold = new Button();
            btn_font_bold.AddToClassList("button");
            btn_font_bold.text = "";
            btn_font_bold.clicked += Btn_font_bold_clicked;
            btn_font_bold.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/fontbold.png");

            font_bold_mark = new VisualElement();
            font_bold_mark.AddToClassList("button_mark");
            btn_font_bold.Add(font_bold_mark);
            #endregion

            #region 按钮 - 字体斜体
            btn_font_italic = new Button();
            btn_font_italic.AddToClassList("button");
            btn_font_italic.text = "";
            btn_font_italic.clicked += Btn_font_italic_clicked;
            btn_font_italic.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/fontitalic.png");

            font_italic_mark = new VisualElement();
            font_italic_mark.AddToClassList("button_mark");
            btn_font_italic.Add(font_italic_mark);
            #endregion

            #region 内容尺寸大小输入框
            FontSizeInput = new IntegerField();
            FontSizeInput.AddToClassList("sizefield");
            FontSizeInput.value = LabelData.fontSize;
            FontSizeInput.Q<TextElement>().AddToClassList("sizefieldText");
            FontSizeInput.Q<VisualElement>(name: "unity-text-input").AddToClassList("sizefieldInput");
            FontSizeInput.RegisterValueChangedCallback(Field_font_size_changed);
            #endregion

            #region 按钮容器
            fontsizerContainer = new VisualElement();
            fontsizerContainer.name = "FontSizerContainer";
            fontsizerContainer.Add(btn_fontsize_plus);
            fontsizerContainer.Add(btn_fontsize_minus);
            fontsizerContainer.Add(FontSizeInput);
            fontsizerContainer.Add(btn_font_bold);
            fontsizerContainer.Add(btn_font_italic);

            FontControlPanelDisplayer(false);

            Add(fontsizerContainer);
            #endregion

            #region 拖拽控件
            // 拖拽尺寸控件图标
            ResizerIcon = this.Q<VisualElement>(className: "resizer-icon");
            ResizerIcon.pickingMode = PickingMode.Ignore;
            ResizerIcon.style.opacity = 0f;

            // 拖拽尺寸控件
            Resizer = this.Q<VisualElement>(className: "resizer");
            Resizer.style.width = 30;
            Resizer.style.height = 30;
            Resizer.RegisterCallback<PointerEnterEvent>(Label_DisplayResizer);
            Resizer.RegisterCallback<PointerLeaveEvent>(Label_HideResizer);
            #endregion
        }

        #endregion

        #region Shift调整透明度
        // 这两个字段用来缓存回调，方便注销
        private EventCallback<WheelEvent> wheelHandler;
        private EventCallback<PointerLeaveEvent> leaveHandler;

        // 鼠标进入：挂滚轮监听
        private void OnDecalPointerEnter(PointerEnterEvent evt)
        {
            // 如果已经挂过就不再重复挂
            if (wheelHandler != null) return;

            wheelHandler = OnDecalWheel;
            labelContentlabel.RegisterCallback(wheelHandler);

            leaveHandler = OnDecalPointerLeave;
            labelContentlabel.RegisterCallback(leaveHandler);
        }

        // 鼠标离开：摘掉滚轮监听
        private void OnDecalPointerLeave(PointerLeaveEvent evt)
        {
            if (wheelHandler != null)
            {
                labelContentlabel.UnregisterCallback(wheelHandler);
                wheelHandler = null;
            }

            if (leaveHandler != null)
            {
                labelContentlabel.UnregisterCallback(leaveHandler);
                leaveHandler = null;
            }
        }

        // 滚轮回调：按住 Shift 时调透明度
        private void OnDecalWheel(WheelEvent evt)
        {
            if (!evt.shiftKey) return;          // 没按 Shift 直接忽略

            // --- 关键：同时读 IMGUI 事件，拿到真正的滚轮增量 ---
            Vector2 imDelta = Vector2.zero;
            if (Event.current != null && Event.current.type == EventType.ScrollWheel)
                imDelta = Event.current.delta;

            float delta = imDelta.x * 0.014f;   // 滚一格 ≈ ±3
            float newOpacity = labelContentlabel.resolvedStyle.opacity - delta;
            newOpacity = Mathf.Clamp01(newOpacity);

            labelContentlabel.style.opacity = newOpacity;
            LabelData.opacity = newOpacity;
            evt.StopPropagation();
        }

        /// <summary>
        /// 按住Shift点击鼠标滚轮键恢复透明度
        /// </summary>
        /// <param name="evt"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnDecalPointerDown(PointerDownEvent evt)
        {
            if (evt.button == (int)MouseButton.MiddleMouse && evt.shiftKey)
            {
                labelContentlabel.style.opacity = 1;
                LabelData.opacity = 1;
            }
        }
        #endregion

        #region 控件回调
        /// <summary>
        /// 标签文字尺寸 - 增加
        /// </summary>
        private void Btn_font_size_minus_clicked()
        {
            StyleLength fontSize = labelContentlabel.style.fontSize;
            Length original_value = fontSize.value;

            original_value.value--;
            fontSize.value = original_value.value;
            util_XGraphEditorUtility.Element_Label_SizeSet(labelContentlabel, (int)fontSize.value.value);

            Undo.RecordObject(graphView.ActionTreeAsset, "Label FontSize Add");
            LabelData.fontSize = (int)fontSize.value.value;
            FontSizeInput.value = LabelData.fontSize;
        }
        /// <summary>
        /// 标签文字尺寸 - 减少
        /// </summary>
        private void Btn_font_size_add_clicked()
        {
            StyleLength fontSize = labelContentlabel.style.fontSize;
            Length original_value = fontSize.value;

            original_value.value++;
            fontSize.value = original_value.value;
            util_XGraphEditorUtility.Element_Label_SizeSet(labelContentlabel, (int)fontSize.value.value);

            Undo.RecordObject(graphView.ActionTreeAsset, "Label FontSize Minus");
            LabelData.fontSize = (int)fontSize.value.value;
            FontSizeInput.value = LabelData.fontSize;
        }
        /// <summary>
        /// 文字大小值改变时
        /// </summary>
        /// <param name="evt"></param>
        private void Field_font_size_changed(ChangeEvent<int> evt)
        {
            IntegerField integerField = evt.target as IntegerField;

            util_XGraphEditorUtility.Element_Label_SizeSet(labelContentlabel, evt.newValue);
            Undo.RecordObject(graphView.ActionTreeAsset, "Label FontSize Set");
            LabelData.fontSize = evt.newValue;
        }
        /// <summary>
        /// 切换斜体时
        /// </summary>
        private void Btn_font_italic_clicked()
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Label Font Italic");
            LabelData.italic = !LabelData.italic;
            util_XGraphEditorUtility.Element_Label_ItalicSet(labelContentlabel, LabelData.italic);

            if (LabelData.italic)
                util_XGraphEditorUtility.Element_Opacity_Set(font_italic_mark, 1);
            else
                util_XGraphEditorUtility.Element_Opacity_Set(font_italic_mark, 0);
        }
        /// <summary>
        /// 切换粗细时
        /// </summary>
        private void Btn_font_bold_clicked()
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Label Font Bold");
            LabelData.bold = !LabelData.bold;
            util_XGraphEditorUtility.Element_Label_BoldSet(labelContentlabel, LabelData.bold);

            if (LabelData.bold)
                util_XGraphEditorUtility.Element_Opacity_Set(font_bold_mark, 1);
            else
                util_XGraphEditorUtility.Element_Opacity_Set(font_bold_mark, 0);
        }
        /// <summary>
        /// 当Graphview编辑器的主题色改变时
        /// </summary>
        /// <param name="color"></param>
        private void OnGraphViewEditorThemeColorChanged(Color color)
        {
            util_XGraphEditorUtility.Element_BackgroundColor_Set(font_bold_mark, graphView.ActionTreeAsset.GraphviewGridBackgroundThemes.themecolor);
            util_XGraphEditorUtility.Element_BackgroundColor_Set(font_italic_mark, graphView.ActionTreeAsset.GraphviewGridBackgroundThemes.themecolor);
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 标签文字控件显示 & 隐藏
        /// </summary>
        /// <param name="state"></param>
        private void FontControlPanelDisplayer(bool state)
        {
            if (state)
            {
                fontsizerContainer.style.visibility = new StyleEnum<Visibility>(Visibility.Visible);
            }
            else
            {
                fontsizerContainer.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
            }
        }
        /// <summary>
        /// 当内容框输入完成时
        /// </summary>
        /// <param name="evt"></param>
        private void OnStickContentInputBlur(BlurEvent evt)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change StickNode Content");
            LabelData.content = labelContentlabel.text = labelContentInput.value;
            VisualElementDisplay(labelContentlabel, true);
            VisualElementDisplay(labelContentInput, false);
        }
        /// <summary>
        /// 元素的视觉布局样式
        /// </summary>
        /// <param name="element"></param>
        /// <param name="display"></param>
        private void VisualElementDisplay(VisualElement element, bool display)
        {
            if (element == null)
                return;
            element.style.display = display ? new StyleEnum<DisplayStyle>(DisplayStyle.Flex) : new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }
        /// <summary>
        /// 设置节点的样式应用
        /// </summary>
        /// <param name="StyleName"></param>
        protected void SetContainersStyle()
        {
            contentContainer.AddToClassList("ContentContainer");
            mainContainer.AddToClassList("MainContainer");
            titleContainer.AddToClassList("TitleContainer");
            titleButtonContainer.AddToClassList("TitleButtonContainer");
            topContainer.AddToClassList("TopContainer");
            inputContainer.AddToClassList("InputContainer");
            outputContainer.AddToClassList("OutputContainer");
            extensionContainer.AddToClassList("ExtensionContainer");
        }
        /// <summary>
        /// 添加元素到指定类型的容器中
        /// </summary>
        /// <param name="type"></param>
        /// <param name="element"></param>
        protected void AppendElement(GraphNodeContainerType type, VisualElement element)
        {
            switch (type)
            {
                case GraphNodeContainerType.MainContainer:
                    mainContainer.Add(element);
                    break;
                case GraphNodeContainerType.TitleContainer:
                    titleContainer.Add(element);
                    break;
                case GraphNodeContainerType.TitleButtonContainer:
                    titleButtonContainer.Add(element);
                    break;
                case GraphNodeContainerType.TopContainer:
                    topContainer.Add(element);
                    break;
                case GraphNodeContainerType.InputContainer:
                    inputContainer.Add(element);
                    break;
                case GraphNodeContainerType.OutputContainer:
                    outputContainer.Add(element);
                    break;
                case GraphNodeContainerType.ExtensionContainer:
                    extensionContainer.Add(element);
                    RefreshExpandedState();
                    break;
            }
        }
        /// <summary>
        /// 鼠标移出时隐藏角点拖拽显示
        /// </summary>
        /// <param name="evt"></param>
        private void Label_HideResizer(PointerLeaveEvent evt)
        {
            ResizerIcon.style.opacity = 0f;
        }
        /// <summary>
        /// 鼠标进入时显示角点拖拽显示
        /// </summary>
        /// <param name="evt"></param>
        private void Label_DisplayResizer(PointerEnterEvent evt)
        {
            ResizerIcon.style.opacity = 1f;
        }
        #endregion
    }
}