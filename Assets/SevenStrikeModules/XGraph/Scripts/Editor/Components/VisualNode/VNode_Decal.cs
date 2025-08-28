namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class VNode_Decal : Node
    {
        /// <summary>
        /// GraphView组件
        /// </summary>
        public xg_GraphView graphView;
        /// <summary>
        /// 视觉节点贴图尺寸控制图标
        /// </summary>
        public VisualElement ResizerIcon;
        /// <summary>
        /// 视觉节点贴图尺寸控制组件
        /// </summary>
        public VisualElement Resizer;
        /// <summary>
        /// 视觉节点贴图组件
        /// </summary>
        public VisualElement DecalTextureElement;
        /// <summary>
        /// 便签的最后一次尺寸
        /// </summary>
        private Vector2 m_LastSize;
        private bool monitoringObjectPicker = false;

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
        public decaldata decalData { get; set; }

        /// <summary>
        /// 初始化节点 - stickdata
        /// </summary>
        /// <param h_name="graphView"></param>
        /// <param h_name="pos"></param>
        /// <param h_name="data"></param>
        public virtual void Initialize(xg_GraphView graphView, Vector2 pos = default, decaldata data = null)
        {
            // 指定可调整大小
            capabilities |= Capabilities.Resizable;

            // 指定GraphView 组件
            this.graphView = graphView;
            // 设置节点的容器样式
            SetContainersStyle("uss_DecalNode");

            // 携带数据
            if (data != null)
                decalData = data;

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

            #region 快速选择头像专用隐藏式GUI
            var imguiContainer = new IMGUIContainer(OnGUI);
            imguiContainer.style.display = DisplayStyle.None; // 隐藏，只用于处理GUI事件
            AppendElement(GraphNodeContainerType.MainContainer, imguiContainer);
            #endregion
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

                decalData.size = newSize;
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

            if (decalData != null)
            {
                decalData.position.x = newPos.xMin;
                decalData.position.y = newPos.yMin;
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
        public VNode_Decal Draw()
        {
            // 绘制主容器
            Draw_Main();

            // 绘制标题容器
            Draw_Title();

            // 绘制顶部容器
            Draw_Top();

            // 检查贴图设置
            CheckDecalTextureChanged();
            return this;
        }

        /// <summary>
        /// 绘制顶部容器
        /// </summary>
        public virtual void Draw_Top()
        {
            topContainer.Clear();
        }

        /// <summary>
        /// 绘制标题容器
        /// </summary>
        public virtual void Draw_Title()
        {
            // 清空容器
            titleContainer.Clear();
        }

        /// <summary>
        /// 绘制主容器
        /// </summary>
        public virtual void Draw_Main()
        {
            // 只保留贴图组件，其余的都不要了
            mainContainer.Remove(mainContainer.Q<VisualElement>("contents"));
            mainContainer.Remove(mainContainer.Q<VisualElement>("title"));

            // 拖拽尺寸控件图标
            ResizerIcon = this.Q<VisualElement>(className: "resizer-icon");
            ResizerIcon.pickingMode = PickingMode.Ignore;
            ResizerIcon.style.opacity = 0f;

            // 拖拽尺寸控件
            Resizer = this.Q<VisualElement>(className: "resizer");
            Resizer.style.width = 30;
            Resizer.style.height = 30;
            Resizer.RegisterCallback<PointerEnterEvent>(Decal_DisplayResizer);
            Resizer.RegisterCallback<PointerLeaveEvent>(Decal_HideResizer);

            #region 创建贴图组件
            DecalTextureElement = new VisualElement();
            DecalTextureElement.name = "DecalTexture";
            DecalTextureElement.pickingMode = PickingMode.Position;
            if (decalData.DecalTexture != null)
                DecalTextureElement.style.backgroundImage = decalData.DecalTexture;
            else
                DecalTextureElement.style.backgroundImage = util_EditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Avatars/Missing.png"); ;
            DecalTextureElement.AddToClassList("DecalTexture");
            AppendElement(GraphNodeContainerType.MainContainer, DecalTextureElement);
            #endregion

            DecalTextureElement.RegisterCallback<PointerEnterEvent>(Decal_DisplayResizer);
            DecalTextureElement.RegisterCallback<PointerLeaveEvent>(Decal_HideResizer);

            RegisterDecalTextureClicked();
        }

        private void Decal_HideResizer(PointerLeaveEvent evt)
        {
            ResizerIcon.style.opacity = 0f;
        }

        private void Decal_DisplayResizer(PointerEnterEvent evt)
        {
            ResizerIcon.style.opacity = 1f;
        }
        #endregion

        #region 贴图设置
        public void RegisterDecalTextureClicked()
        {
            // 修改贴图点击回调
            DecalTextureElement.RegisterCallback<PointerDownEvent>((evt) =>
            {
                // 双击头像以更换贴图
                if (evt.clickCount == 2)
                {
                    OpenDecalTextureSelector();
                    evt.StopPropagation();
                }
            });
        }
        /// <summary>
        /// 打开贴图选择框
        /// </summary>
        public void OpenDecalTextureSelector()
        {
            EditorGUIUtility.ShowObjectPicker<Texture2D>(decalData.DecalTexture, false, "t:Texture2D", 0);
            monitoringObjectPicker = true;
        }
        /// <summary>
        /// 检查是否设置了贴图
        /// </summary>
        public void CheckDecalTextureChanged()
        {
            if (DecalTextureElement == null)
                return;

            // 如果该节点设置了贴图
            if (decalData.HasTexture)
            {
                NodeDecalTexture_IsSet();
            }
            else
            {
                NodeDecalTexture_IsNone();
            }
        }
        public void NodeDecalTexture_IsNone()
        {
            DecalTextureElement.style.backgroundImage = util_EditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Avatars/Missing.png");
            DecalTextureElement.style.borderTopWidth = 1;
            DecalTextureElement.style.borderBottomWidth = 1;
            DecalTextureElement.style.borderLeftWidth = 1;
            DecalTextureElement.style.borderRightWidth = 1;
        }

        public void NodeDecalTexture_IsSet()
        {
            if (decalData.DecalTexture != null)
            {
                DecalTextureElement.style.backgroundImage = decalData.DecalTexture;
                DecalTextureElement.style.borderTopWidth = 0;
                DecalTextureElement.style.borderBottomWidth = 0;
                DecalTextureElement.style.borderLeftWidth = 0;
                DecalTextureElement.style.borderRightWidth = 0;
            }
            else
                NodeDecalTexture_IsNone();
        }

        /// <summary>
        /// 设置贴图
        /// </summary>
        /// <param name="tex"></param>
        public void NodeDecalTexture_Set(Texture2D tex)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Set DecalData Texture");
            // 贴图状态开关 = 开
            decalData.HasTexture = true;
            // 贴图图像设置
            decalData.DecalTexture = tex;

            CheckDecalTextureChanged();
        }
        /// <summary>
        /// 移除贴图
        /// </summary>
        /// <param name="tex"></param>
        public void NodeDecalTexture_Remove()
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Remove DecalData Texture");
            // 贴图状态开关 = 开
            decalData.HasTexture = false;
            // 贴图图像设置
            decalData.DecalTexture = null;

            CheckDecalTextureChanged();
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 添加 OnGUI 方法，用于处理贴图选择
        /// </summary>
        private void OnGUI()
        {
            if (monitoringObjectPicker && Event.current != null)
            {
                if (Event.current.commandName == "ObjectSelectorClosed")
                {
                    var selectedTexture = EditorGUIUtility.GetObjectPickerObject() as Texture2D;
                    if (selectedTexture != null)
                    {
                        NodeDecalTexture_Set(selectedTexture);
                    }
                    monitoringObjectPicker = false;
                }
            }
        }
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