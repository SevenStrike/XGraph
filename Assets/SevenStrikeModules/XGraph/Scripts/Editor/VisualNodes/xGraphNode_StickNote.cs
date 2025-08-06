namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xGraphNode_StickNote : Node
    {
        /// <summary>
        /// GraphView组件
        /// </summary>
        public xg_GraphView graphView;
        /// <summary>
        /// 便签的最后一次尺寸
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
        public StickNoteData stickNoteData { get; set; }

        /// <summary>
        /// 初始化节点 - StickNoteData
        /// </summary>
        /// <param h_name="graphView"></param>
        /// <param h_name="pos"></param>
        /// <param h_name="data"></param>
        public virtual void Initialize(xg_GraphView graphView, Vector2 pos = default, StickNoteData data = null)
        {
            // 指定可调整大小
            capabilities |= Capabilities.Resizable;

            // 指定GraphView 组件
            this.graphView = graphView;
            // 设置节点的容器样式
            SetContainersStyle("uss_StickNote");

            // 携带数据
            if (data != null)
                stickNoteData = data;

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
        /// <param h_name="evt"></param>
        private void OnSizeChanged(GeometryChangedEvent evt)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change Stick Size");

            Vector2 newSize = new Vector2(evt.newRect.width, evt.newRect.height);

            if (Vector2.Distance(m_LastSize, newSize) > 1f) // 1像素阈值
            {
                m_LastSize = newSize;

                stickNoteData.size = newSize;
            }
        }

        /// <summary>
        /// 当拖动节点位置时，将位置数据传递给对应的目标数据节点位置变量
        /// </summary>
        /// <param h_name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change Stick Position");
            base.SetPosition(newPos);

            if (stickNoteData != null)
            {
                stickNoteData.position.x = newPos.xMin;
                stickNoteData.position.y = newPos.yMin;
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

        #region 节点绘制
        /// <summary>
        /// 绘制节点
        /// </summary>
        public xGraphNode_StickNote Draw()
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
            #region 便签内容输入框
            TextField field_content = new TextField();
            field_content.value = stickNoteData.content;
            StyleLength len_width = field_content.style.width;

            Length len_w = len_width.value;
            len_w.unit = LengthUnit.Percent;
            len_w.value = 100;

            len_width.value = len_w;
            field_content.multiline = true;
            field_content.AddToClassList("StickNoteTextField_Title");

            field_content.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                stickNoteData.content = evt.newValue;
            });

            AppendElement(GraphNodeContainerType.TopContainer, field_content);
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
            icon.style.backgroundImage = util_EditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/stick.png");
            icon.style.unityBackgroundImageTintColor = Color.black * 0.85f;
            #endregion

            #region 便签标题输入框
            TextField field_title = new TextField();
            field_title.value = stickNoteData.name;
            field_title.AddToClassList("Title_Input");

            field_title.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                stickNoteData.name = evt.newValue;
            });

            VisualElement input = field_title.Q<VisualElement>("unity-text-input");
            input.AddToClassList("TitleInput");

            TextElement inputtext = input.Q<TextElement>();
            inputtext.AddToClassList("TitleInputText");
            #endregion

            VisualElement element = titleContainer.Q<VisualElement>("title-button-container");

            // 清空容器后重新按顺序添加
            titleContainer.Clear();
            AppendElement(GraphNodeContainerType.TitleContainer, icon);
            AppendElement(GraphNodeContainerType.TitleContainer, field_title);
            AppendElement(GraphNodeContainerType.TitleContainer, element);
        }

        /// <summary>
        /// 绘制主容器
        /// </summary>
        public virtual void Draw_Main()
        {

        }
        #endregion

        #region 辅助
        /// <summary>
        /// 设置节点的样式应用
        /// </summary>
        /// <param h_name="StyleName"></param>
        protected void SetContainersStyle(string StyleName)
        {
            // 读取uss样式
            var uss_node = util_EditorUtility.AssetLoad<StyleSheet>($"{util_Dashboard.GetPath_GUI_Uss()}{StyleName}.uss");
            styleSheets.Add(uss_node);
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
        /// <param h_name="type"></param>
        /// <param h_name="element"></param>
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
    }
}