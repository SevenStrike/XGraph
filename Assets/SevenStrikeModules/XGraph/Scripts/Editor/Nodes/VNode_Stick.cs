namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class VNode_Stick : Node
    {
        /// <summary>
        /// GraphView组件
        /// </summary>
        public xg_GraphView graphView;
        /// <summary>
        /// 便签标题组件
        /// </summary>
        public Label stickTitlelabel;
        /// <summary>
        /// 便签内容组件
        /// </summary>
        public Label stickContentlabel;
        /// <summary>
        /// 便签标题输入框组件
        /// </summary>
        public TextField stickTitleInput;
        /// <summary>
        /// 便签内容输入框组件
        /// </summary>
        public TextField stickContentInput;
        /// <summary>
        /// 便签内容组件
        /// </summary>
        public TextField stickContent;
        /// <summary>
        /// 视觉节点贴图尺寸控制图标
        /// </summary>
        public VisualElement ResizerIcon;
        /// <summary>
        /// 视觉节点贴图尺寸控制组件
        /// </summary>
        public VisualElement Resizer;
        /// <summary>
        /// 高亮面
        /// </summary>
        private VisualElement Highlighter;
        /// <summary>
        /// 便签的最后一次尺寸
        /// </summary>
        private Vector2 m_LastSize;
        /// <summary>
        /// 当选中节点时的委托事件
        /// </summary>
        public Action<VNode_Stick> OnSelectedNode;
        /// <summary>
        /// 当选中节点时的委托事件
        /// </summary>
        public Action<VNode_Stick> OnUnSelectedNode;
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
        public ActionStickData StickData { get; set; }

        /// <summary>
        /// 初始化节点 - ActionStickData
        /// </summary>
        /// <param name="graphView"></param>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        public virtual void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionStickData data = null)
        {
            // 指定可调整大小
            capabilities |= Capabilities.Resizable;

            // 指定GraphView 组件
            this.graphView = graphView;

            // 设置节点的容器样式
            util_XGraphEditorUtility.ElementStyle_Add(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_StickNote.uss");

            SetContainersStyle("uss_StickNote");

            // 携带数据
            if (data != null)
                StickData = data;

            style.width = data.size.x;
            style.height = data.size.y;

            #region 基础参数设置
            this.viewDataKey = data != null ? data.guid : "";
            // 设置节点标题
            this.title = this.nodeTitle = data != null ? data.name : "";
            #endregion

            // 设置节点的生成位置
            SetPosition(new Rect(pos, Vector2.zero));

            // 监听尺寸变化事件
            RegisterCallback<GeometryChangedEvent>(OnSizeChanged);
        }

        /// <summary>
        /// 当节点尺寸发生改变时
        /// </summary>
        /// <param name="evt"></param>
        private void OnSizeChanged(GeometryChangedEvent evt)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change Stick Size");

            Vector2 newSize = new Vector2(evt.newRect.width, evt.newRect.height);

            if (Vector2.Distance(m_LastSize, newSize) > 1f) // 1像素阈值
            {
                m_LastSize = newSize;

                StickData.size = newSize;
            }
        }

        /// <summary>
        /// 当拖动节点位置时，将位置数据传递给对应的目标数据节点位置变量
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change Stick Position");
            base.SetPosition(newPos);

            if (StickData != null)
            {
                StickData.position.x = newPos.xMin;
                StickData.position.y = newPos.yMin;
            }
        }

        /// <summary>
        /// 可改变尺寸
        /// </summary>
        /// <returns></returns>
        public override bool IsResizable()
        {
            return true;
        }

        #region 回调
        /// <summary>
        /// 当选择节点时
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();

            // 调用回调事件
            if (OnSelectedNode != null)
            {
                OnSelectedNode.Invoke(this);
            }
        }
        /// <summary>
        /// 取消选择时
        /// </summary>
        public override void OnUnselected()
        {
            base.OnUnselected();

            // 调用回调事件
            if (OnUnSelectedNode != null)
            {
                OnUnSelectedNode.Invoke(this);
            }
        }
        #endregion

        #region 节点绘制
        /// <summary>
        /// 绘制节点
        /// </summary>
        public VNode_Stick Draw()
        {
            // 绘制主容器
            Draw_Main();

            // 绘制标题容器
            Draw_Title();

            // 绘制顶部容器
            Draw_Top();

            return this;
        }

        /// <summary>
        /// 绘制顶部容器
        /// </summary>
        public virtual void Draw_Top()
        {
            #region 便签内容
            stickContentlabel = new Label(StickData.content);
            stickContentlabel.AddToClassList("Content_Label");
            stickContentlabel.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    VisualElementDisplay(stickContentlabel, false);
                    VisualElementDisplay(stickContentInput, true);

                    EditorApplication.delayCall += () =>
                    {
                        stickContentInput.Focus();
                    };
                    evt.StopPropagation();
                }
            });
            #endregion

            #region 便签内容输入框
            stickContentInput = new TextField();
            stickContentInput.value = StickData.content;
            StyleLength len_width = stickContentInput.style.width;

            Length len_w = len_width.value;
            len_w.unit = LengthUnit.Percent;
            len_w.value = 100;

            len_width.value = len_w;
            stickContentInput.multiline = true;
            stickContentInput.AddToClassList("Content_TextField");
            stickContentInput.RegisterCallback<BlurEvent>(OnStickContentInputBlur);

            AppendElement(GraphNodeContainerType.TopContainer, stickContentlabel);
            AppendElement(GraphNodeContainerType.TopContainer, stickContentInput);
            #endregion
        }

        /// <summary>
        /// 绘制标题容器
        /// </summary>
        public virtual void Draw_Title()
        {
            #region Logo
            Label icon = new Label("");
            icon.AddToClassList("Title_Icon");
            icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/stick.png");
            icon.style.unityBackgroundImageTintColor = Color.black * 0.85f;
            #endregion

            #region 便签标题
            stickTitlelabel = new Label(StickData.name);
            stickTitlelabel.AddToClassList("Title_Label");
            stickTitlelabel.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    VisualElementDisplay(stickTitlelabel, false);
                    VisualElementDisplay(stickTitleInput, true);

                    EditorApplication.delayCall += () =>
                    {
                        stickTitleInput.Focus();
                    };
                    evt.StopPropagation();
                }
            });
            #endregion

            #region 便签标题输入框
            stickTitleInput = new TextField();
            stickTitleInput.value = StickData.name;
            stickTitleInput.AddToClassList("Title_Input");
            stickTitleInput.RegisterCallback<BlurEvent>(OnStickTitleInputBlur);

            VisualElement input = stickTitleInput.Q<VisualElement>("unity-text-input");
            input.AddToClassList("Title_InputElement");

            TextElement inputtext = input.Q<TextElement>();
            inputtext.AddToClassList("Title_InputElement_Text");
            #endregion

            VisualElement element = titleContainer.Q<VisualElement>("title-button-container");

            // 清空容器后重新按顺序添加
            titleContainer.Clear();

            AppendElement(GraphNodeContainerType.TitleContainer, icon);
            AppendElement(GraphNodeContainerType.TitleContainer, stickTitlelabel);
            AppendElement(GraphNodeContainerType.TitleContainer, stickTitleInput);
            AppendElement(GraphNodeContainerType.TitleContainer, element);
        }

        /// <summary>
        /// 绘制主容器
        /// </summary>
        public virtual void Draw_Main()
        {
            // 拖拽尺寸控件图标
            ResizerIcon = this.Q<VisualElement>(className: "resizer-icon");
            ResizerIcon.pickingMode = PickingMode.Ignore;

            // 拖拽尺寸控件
            Resizer = this.Q<VisualElement>(className: "resizer");
            Resizer.style.width = 30;
            Resizer.style.height = 30;

            #region 高亮面
            Highlighter = new VisualElement();
            Highlighter.pickingMode = PickingMode.Ignore;
            Highlighter.name = "HighlighterVisualler";
            Highlighter.AddToClassList("highlighter");
            util_XGraphEditorUtility.Element_BackgroundColor_Set(Highlighter, Color.white);
            UnHighlight();
            AppendElement(GraphNodeContainerType.MainContainer, Highlighter);
            Highlighter.BringToFront();
            #endregion
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 当内容框输入完成时
        /// </summary>
        /// <param name="evt"></param>
        private void OnStickContentInputBlur(BlurEvent evt)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change StickNode Content");
            StickData.content = stickContentlabel.text = stickContentInput.value;
            VisualElementDisplay(stickContentlabel, true);
            VisualElementDisplay(stickContentInput, false);
        }
        /// <summary>
        /// 便签标题输入完成时
        /// </summary>
        /// <param name="evt"></param>
        private void OnStickTitleInputBlur(BlurEvent evt)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change StickNode Name");
            StickData.name = stickTitlelabel.text = stickTitleInput.value;
            VisualElementDisplay(stickTitlelabel, true);
            VisualElementDisplay(stickTitleInput, false);
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
        protected void SetContainersStyle(string StyleName)
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
        #endregion

        #region 高亮
        /// <summary>
        /// 高亮显示节点
        /// </summary>        
        public void Highlight()
        {
            util_XGraphEditorUtility.Element_Opacity_Set(Highlighter, 0.65f);
        }
        /// <summary>
        /// 取消高亮显示节点
        /// </summary>
        public void UnHighlight()
        {
            util_XGraphEditorUtility.Element_Opacity_Set(Highlighter, 0);
        }
        #endregion
    }
}