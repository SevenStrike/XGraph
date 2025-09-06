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

        /// <summary>
        /// 指示物体选择器是否已经打开
        /// </summary>
        private bool monitoringObjectPicker = false;
        private IMGUIContainer m_ObjectPickerIMGUI;
        /// <summary>
        /// 用于打开贴图选择器后选择贴图应用的模式
        /// </summary>
        public string SetTextureMode;

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
            util_XGraphEditorUtility.ElementStyle_Add(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_DecalNode.uss");
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

            // 关键：在节点监听拖拽事件
            RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            RegisterCallback<DragPerformEvent>(OnDragPerform);
            RegisterCallback<DragExitedEvent>(OnDragExit);
        }

        /// <summary>
        /// 当节点尺寸发生改变时
        /// </summary>
        /// <param h_name="evt"></param>
        private void OnSizeChanged(GeometryChangedEvent evt)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change Decal Size");

            Vector2 newSize = new Vector2(evt.newRect.width, evt.newRect.height);

            if (Vector2.Distance(m_LastSize, newSize) > 1f) // 1像素阈值
            {
                m_LastSize = newSize;

                decalData.size = newSize;
            }
        }

        /// <summary>
        /// 设置节点尺寸为图片原生尺寸
        /// </summary>
        public void SetNativeSize()
        {
            decalData.size = new Vector2(decalData.DecalTexture.width, decalData.DecalTexture.height);
            style.width = decalData.size.x;
            style.height = decalData.size.y;
        }

        /// <summary>
        /// 当拖动节点位置时，将位置数据传递给对应的目标数据节点位置变量
        /// </summary>
        /// <param h_name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change Decal Position");
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

        public override void OnSelected()
        {
            //base.OnSelected();
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
            DecalTextureElement.style.opacity = decalData.opacity;
            DecalTextureElement.style.scale = new StyleScale(decalData.scale);
            DecalTextureElement.pickingMode = PickingMode.Position;
            if (decalData.DecalTexture != null)
                DecalTextureElement.style.backgroundImage = decalData.DecalTexture;
            else
                DecalTextureElement.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Avatars/Missing.png"); ;
            DecalTextureElement.AddToClassList("DecalTexture");
            AppendElement(GraphNodeContainerType.MainContainer, DecalTextureElement);
            #endregion

            DecalTextureElement.RegisterCallback<PointerEnterEvent>(Decal_DisplayResizer);
            DecalTextureElement.RegisterCallback<PointerLeaveEvent>(Decal_HideResizer);

            DecalTextureElement.RegisterCallback<PointerEnterEvent>(OnDecalPointerEnter);
            DecalTextureElement.RegisterCallback<PointerLeaveEvent>(OnDecalPointerLeave);

            DecalTextureElement.RegisterCallback<PointerDownEvent>(OnDecalPointerDown);
            RegisterDecalTextureClicked();
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
            DecalTextureElement.RegisterCallback(wheelHandler);

            leaveHandler = OnDecalPointerLeave;
            DecalTextureElement.RegisterCallback(leaveHandler);
        }

        // 鼠标离开：摘掉滚轮监听
        private void OnDecalPointerLeave(PointerLeaveEvent evt)
        {
            if (wheelHandler != null)
            {
                DecalTextureElement.UnregisterCallback(wheelHandler);
                wheelHandler = null;
            }

            if (leaveHandler != null)
            {
                DecalTextureElement.UnregisterCallback(leaveHandler);
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

            float delta = imDelta.x * 0.02f;   // 滚一格 ≈ ±3
            float newOpacity = DecalTextureElement.resolvedStyle.opacity - delta;
            newOpacity = Mathf.Clamp01(newOpacity);

            DecalTextureElement.style.opacity = newOpacity;
            decalData.opacity = newOpacity;
            evt.StopPropagation();
        }

        /// <summary>
        /// 按住Shift点击鼠标滚轮键恢复透明度
        /// </summary>
        /// <param name="evt"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnDecalPointerDown(PointerDownEvent evt)
        {
            if (evt.button == (int)MouseButton.MiddleMouse)
            {
                DecalTextureElement.style.opacity = 1;
                decalData.opacity = 1;
            }
        }
        #endregion

        #region 贴图设置
        /// <summary>
        /// 注册贴图双击事件
        /// </summary>
        public void RegisterDecalTextureClicked()
        {
            // 修改贴图点击回调
            DecalTextureElement.RegisterCallback<PointerDownEvent>((evt) =>
            {
                // 只响应左键
                if (evt.button != (int)MouseButton.LeftMouse)
                    return;

                // 双击头像以更换贴图
                if (evt.clickCount == 2)
                {
                    OpenTexturesSelector();
                    evt.StopPropagation();
                }
            });
        }

        /// <summary>
        /// 打开贴图选择框
        /// </summary>
        public void OpenTexturesSelector()
        {
            OpenObjectPickerForTextures("t:Texture2D");
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

        /// <summary>
        /// 贴图为空的时候的样式设置
        /// </summary>
        public void NodeDecalTexture_IsNone()
        {
            DecalTextureElement.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Avatars/Missing.png");
            DecalTextureElement.style.borderTopWidth = 1;
            DecalTextureElement.style.borderBottomWidth = 1;
            DecalTextureElement.style.borderLeftWidth = 1;
            DecalTextureElement.style.borderRightWidth = 1;
        }

        /// <summary>
        /// 贴图不为空的时候的样式设置
        /// </summary>
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

        /// <summary>
        /// 贴图水平翻转
        /// </summary>
        public void NodeDecalTexture_Flip_H()
        {
            VisualElementScale(DecalTextureElement, "h");
        }

        /// <summary>
        /// 贴图垂直翻转
        /// </summary>
        public void NodeDecalTexture_Flip_V()
        {
            VisualElementScale(DecalTextureElement, "v");
        }
        #endregion

        #region 拖拽到节点
        /// <summary>
        /// 拖拽贴图到节点时
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            // 只关心贴图
            if (DragAndDrop.objectReferences.Length > 0 &&
                DragAndDrop.objectReferences[0] is Texture2D)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }

            evt.StopPropagation();   // 防止 GraphView 也处理
        }

        /// <summary>
        /// 当拖拽离开节点时
        /// </summary>
        /// <param name="evt"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnDragExit(DragExitedEvent evt)
        {

        }

        /// <summary>
        /// 松开鼠标赋值贴图到ActioNode的AvatarIcon
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragPerform(DragPerformEvent evt)
        {
            var tex = DragAndDrop.objectReferences[0] as Texture2D;
            if (tex == null) return;

            NodeDecalTexture_Set(tex);
            evt.StopPropagation();
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 鼠标移出时隐藏角点拖拽显示
        /// </summary>
        /// <param name="evt"></param>
        private void Decal_HideResizer(PointerLeaveEvent evt)
        {
            ResizerIcon.style.opacity = 0f;
        }
        /// <summary>
        /// 鼠标进入时显示角点拖拽显示
        /// </summary>
        /// <param name="evt"></param>
        private void Decal_DisplayResizer(PointerEnterEvent evt)
        {
            ResizerIcon.style.opacity = 1f;
        }
        /// <summary>
        /// 元素的视觉布局翻转
        /// </summary>
        /// <param name="element"></param>
        /// <param name="scale"></param>
        private void VisualElementScale(VisualElement element, string flip)
        {
            if (element == null)
                return;
            Undo.RecordObject(graphView.ActionTreeAsset, "Filp Decal");

            Vector2 flipedValue = new Vector2(element.style.scale.value.value.x, element.style.scale.value.value.y);

            if (flip == "h")
            {
                flipedValue.x = -flipedValue.x;
            }
            else if (flip == "v")
            {
                flipedValue.y = -flipedValue.y;
            }

            element.style.scale = new StyleScale(flipedValue);
            decalData.scale = flipedValue;
        }
        /// <summary>
        /// 将节点置顶显示（最上层显示级别）
        /// </summary>
        public void VisualElementBringToFront()
        {
            decaldata data = null;

            for (int i = 0; i < graphView.ActionTreeAsset.DecalDatas.Count; i++)
            {
                if (graphView.ActionTreeAsset.DecalDatas[i].guid == decalData.guid)
                {
                    data = graphView.ActionTreeAsset.DecalDatas[i].Clone(false);
                    graphView.ActionTreeAsset.DecalDatas.RemoveAt(i);
                    break;
                }
            }
            graphView.ActionTreeAsset.DecalDatas.Add(data);
            BringToFront();
        }
        /// <summary>
        /// 将节点置底显示（最下层显示级别）
        /// </summary>
        public void VisualElementSendToBack()
        {
            decaldata data = null;

            for (int i = 0; i < graphView.ActionTreeAsset.DecalDatas.Count; i++)
            {
                if (graphView.ActionTreeAsset.DecalDatas[i].guid == decalData.guid)
                {
                    data = graphView.ActionTreeAsset.DecalDatas[i].Clone(false);
                    graphView.ActionTreeAsset.DecalDatas.RemoveAt(i);
                    break;
                }
            }
            graphView.ActionTreeAsset.DecalDatas.Insert(0, data);
            SendToBack();
        }
        /// <summary>
        /// 设置节点的样式应用
        /// </summary>
        /// <param h_name="StyleName"></param>
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

        #region 弹出物体选择面板
        // 打开物体选择器的方法
        public void OpenObjectPickerForTextures(string typefilter)
        {
            if (monitoringObjectPicker) return;

            monitoringObjectPicker = true;

            // 动态创建 IMGUIContainer
            if (m_ObjectPickerIMGUI == null)
            {
                m_ObjectPickerIMGUI = new IMGUIContainer(OnObjectPickerGUI);
                m_ObjectPickerIMGUI.name = "---------------VNodeTexturePicker";
                m_ObjectPickerIMGUI.style.display = DisplayStyle.Flex;
                Add(m_ObjectPickerIMGUI);
            }

            EditorGUIUtility.ShowObjectPicker<Texture2D>(decalData.DecalTexture, false, typefilter, 0);
        }

        private void OnObjectPickerGUI()
        {
            if (!monitoringObjectPicker) return;

            // 只处理特定事件
            if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint)
            {
                if (Event.current != null && Event.current.commandName == "ObjectSelectorClosed")
                {
                    var selectedTexture = EditorGUIUtility.GetObjectPickerObject() as Texture2D;

                    if (selectedTexture != null)
                    {
                        ApplySelectedTexture(selectedTexture);
                    }

                    monitoringObjectPicker = false;
                    SetTextureMode = null;

                    if (m_ObjectPickerIMGUI != null)
                    {
                        // 使用延迟调用来处理选择结果，避免在当前 GUI 调用中修改层次结构
                        EditorApplication.delayCall += () =>
                        {
                            Remove(m_ObjectPickerIMGUI);
                            m_ObjectPickerIMGUI = null;
                        };
                    }
                }
            }
            MarkDirtyRepaint();
        }

        // 应用选择的贴图
        private void ApplySelectedTexture(Texture2D selectedTexture)
        {
            NodeDecalTexture_Set(selectedTexture);
        }
        #endregion
    }
}