namespace SevenStrikeModules.XGraph
{
    using Codice.CM.Common.Tree;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class VNode_Base : Node
    {
        /// <summary>
        /// GraphView组件
        /// </summary>
        public xg_GraphView graphView;
        /// <summary>
        /// 当选中节点时的委托事件
        /// </summary>
        public Action<VNode_Base> OnSelectedNode;
        /// <summary>
        /// 当选中节点时的委托事件
        /// </summary>
        public Action<VNode_Base> OnUnSelectedNode;
        /// <summary>
        /// 当为节点设置Avatar时的委托事件
        /// </summary>
        public Action<VNode_Base> OnNodeAvatar_Set;
        /// <summary>
        /// 当节点清空Avatar时的委托事件
        /// </summary>
        public Action<VNode_Base> On_NodeAvatar_Clear;
        /// <summary>
        /// 视觉节点标题图标
        /// </summary>
        public Label NodeTitleIconLabel;
        /// <summary>
        /// 视觉节点头像组件
        /// </summary>
        public VisualElement AvatarIcon;
        /// <summary>
        /// 视觉节点图标
        /// </summary>
        public Label SeperateIconLabel;
        /// <summary>
        /// 视觉节点标题
        /// </summary>
        public Label TitleLabel;
        /// <summary>
        /// 视觉节点输入框
        /// </summary>
        public TextField TitleInputField;
        /// <summary>
        /// 指定节点图标
        /// </summary>
        public string icon;
        /// <summary>
        /// 指示物体选择器是否已经打开
        /// </summary>
        private bool monitoringObjectPicker = false;
        private IMGUIContainer m_ObjectPickerIMGUI;
        /// <summary>
        /// 用于打开贴图选择器后选择贴图应用的模式
        /// </summary>
        public string SetTextureMode;

        /// <summary>
        /// 导图节点的最后一次尺寸
        /// </summary>
        private Vector2 m_LastSize;

        public Texture2D tex_logo_dir_sequential;
        public Texture2D tex_logo_dir_concurrent;

        /// <summary>
        /// 节点携带的数据
        /// </summary>
        public ActionNode_Base ActionData { get; set; }

        #region 端口
        /// <summary>
        /// 输入端口
        /// </summary>
        public List<xGraph_NodePort> Port_Inputs = new List<xGraph_NodePort>();
        /// <summary>
        /// 输出端口
        /// </summary>
        public List<xGraph_NodePort> Port_Outputs = new List<xGraph_NodePort>();
        #endregion

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
        /// 初始化节点 - ActionNode_Base
        /// </summary>
        /// <param name="graphView"></param>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        public virtual void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            // 设置节点的容器样式
            util_XGraphEditorUtility.ElementStyle_Add(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_Node.uss");

            tex_logo_dir_sequential = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/sepline.png");
            tex_logo_dir_concurrent = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/concurrent.png");

            // 指定GraphView 组件
            this.graphView = graphView;

            extensionContainer.AddToClassList("ExtensionContainer");

            // 携带数据
            if (data != null)
                ActionData = data;

            #region 基础参数设置
            this.icon = data.icon;
            this.viewDataKey = data != null ? data.guid : "";
            // 设置节点标题
            this.title = this.nodeTitle = data != null ? data.identifyName : "";
            #endregion

            // 设置节点的生成位置
            SetPosition(new Rect(pos, Vector2.zero));

            // 关键：在节点监听拖拽事件
            RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            RegisterCallback<DragPerformEvent>(OnDragPerform);
            RegisterCallback<DragExitedEvent>(OnDragExit);

            // 监听尺寸变化事件
            RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

            DuplicateAction_Add();

            // 更新变量值
            //RefreshVariableValue();
        }

        #region 订阅 Graphview 克隆动作
        /// <summary>
        /// 注册Graphview的克隆动作
        /// </summary>
        public void DuplicateAction_Add()
        {
            graphView.OnDuplicateNodes += OnDuplicatedNode;
        }

        /// <summary>
        /// 注销Graphview的克隆动作
        /// </summary>
        public void DuplicateAction_Remove()
        {
            graphView.OnDuplicateNodes -= OnDuplicatedNode;
        }
        #endregion

        /// <summary>
        /// 当拖动节点位置时，将位置数据传递给对应的目标数据节点位置变量
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(this.ActionData, "SetPosition VisualNode");
            base.SetPosition(newPos);
            if (ActionData != null)
            {
                ActionData.nodeGraphPosition.x = newPos.xMin;
                ActionData.nodeGraphPosition.y = newPos.yMin;
            }

            VisualElementDisplay(TitleLabel, true);
            VisualElementDisplay(TitleInputField, false);
        }

        #region 拖拽素材到节点
        /// <summary>
        /// 拖拽贴图到节点时
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            if (ActionData.actionNodeType == "Relay")
                return;

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
            if (ActionData.actionNodeType == "Relay")
                return;
            var tex = DragAndDrop.objectReferences[0] as Texture2D;
            if (tex == null) return;

            NodeAvatar_Set(tex);
            RegisterAvatarClicked();

            evt.StopPropagation();
        }
        #endregion

        #region 端口设置
        public virtual VNode_Base InputPort_Set(List<xGraph_NodePort> portInfos)
        {
            Port_Inputs = portInfos;
            return this;
        }

        public virtual VNode_Base OutputPort_Set(List<xGraph_NodePort> portInfos)
        {
            Port_Outputs = portInfos;
            return this;
        }

        /// <summary>
        /// 创建端口
        /// </summary>
        /// <param name="name"></param>
        /// <param name="orientation"></param>
        /// <param name="direction"></param>
        /// <param name="capacity"></param>
        /// <param name="type"></param>
        /// <param name="nodeThemeColor"></param>
        /// <returns></returns>
        public virtual Port Port_Create(string name = "新端口", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single, Type type = null, Color nodeThemeColor = default)
        {
            Port port = InstantiatePort(orientation, direction, capacity, type);
            port.portName = name;
            port.portColor = nodeThemeColor;
            return port;
        }
        #endregion

        #region 节点绘制
        /// <summary>
        /// 绘制节点
        /// </summary>
        public virtual VNode_Base Draw()
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

        /// <summary>
        /// 绘制扩展容器
        /// </summary>
        public virtual void Draw_Extension()
        {
            Label label = new Label(nodeNote);
            label.AddToClassList("Extension_Label");
            AppendElement(GraphNodeContainerType.ExtensionContainer, label);
        }

        public virtual void Draw_Input()
        {
            Port_Inputs.ForEach(p =>
            {
                p.Port = Port_Create(p.Name, Orientation.Horizontal, Direction.Input, p.Capacity, p.Type, ActionData.themeSolution == "M 默认" ? Color.white * 0.7f : ActionData.themeColor);
                p.PortDonut = p.Port.Q<VisualElement>("connector");
                SetPortStyle(p, PortStyleType.In);
                AppendElement(GraphNodeContainerType.InputContainer, p.Port);
            });
        }

        /// <summary>
        /// 绘制输出节点容器
        /// </summary>
        public virtual void Draw_Output()
        {
            Port_Outputs.ForEach(p =>
            {
                // 绘制端口 - 输出
                p.Port = Port_Create(p.Name, Orientation.Horizontal, Direction.Output, p.Capacity, p.Type, ActionData.themeSolution == "M 默认" ? Color.white * 0.7f : ActionData.themeColor);
                p.PortDonut = p.Port.Q<VisualElement>("connector");
                SetPortStyle(p, PortStyleType.Out);
                AppendElement(GraphNodeContainerType.OutputContainer, p.Port);
            });
        }

        /// <summary>
        /// 绘制顶部容器
        /// </summary>
        public virtual void Draw_Top()
        {

        }

        /// <summary>
        /// 绘制标题按钮容器
        /// </summary>
        public virtual void Draw_TitleButton()
        {

        }

        /// <summary>
        /// 绘制标题容器
        /// </summary>
        public virtual void Draw_Title()
        {
            // 节点标题图标
            NodeTitleIconLabel = new Label("");
            NodeTitleIconLabel.AddToClassList("Title_Icon");

            // 如果指定了图标就不用根据名称指定的图标了
            if (ActionData.NodeIcon != null)
                NodeTitleIcon_Set(ActionData.NodeIcon);
            else
                NodeTitleIcon_Restore();
            NodeTitleIconLabel.RegisterCallback<PointerDownEvent>(ChangeTitleIcon);

            // 用于显示节点名称
            TitleLabel = new Label(ActionData.identifyName);
            TitleLabel.AddToClassList("Title_Label");
            TitleLabel.RegisterCallback<PointerDownEvent>((evt) =>
            {
                if (evt.clickCount == 2)
                {
                    VisualElementDisplay(TitleLabel, false);
                    VisualElementDisplay(TitleInputField, true);
                    EditorApplication.delayCall += () =>
                    {
                        TitleInputField.Focus();
                    };
                    evt.StopPropagation();
                }
            });

            // 用于编辑节点名称
            TitleInputField = new TextField()
            {
                multiline = false
            };
            TitleInputField.value = ActionData.identifyName;
            TitleInputField.AddToClassList("Title_TextField");
            TitleInputField.RegisterCallback<BlurEvent>(OnTitleInputFieldBlur);
            VisualElement input = TitleInputField.Q<VisualElement>("unity-text-input");
            input.AddToClassList("Title_TextInput");
            TextElement textelement = input.Q<TextElement>();
            textelement.AddToClassList("Title_TextElement");

            // 节点折叠 / 展开按钮
            VisualElement titlebuttoncontainer = titleContainer.Q<VisualElement>("title-button-container");

            titlebuttoncontainer.style.borderRightWidth = 0;
            SeperateIconLabel = new Label("");
            SeperateIconLabel.AddToClassList("Seperate_Icon");
            CheckExecutionModel();

            #region 应用配置文件的颜色到节点的标识颜色
            graphView.ThemesList.Node.ForEach(colorData =>
            {
                if (colorData.solution == ActionData.themeSolution)
                {
                    SeperateIconLabel.style.unityBackgroundImageTintColor = ActionData.themeSolution == "M 默认" ? Color.white : ActionData.themeColor;
                }
            });
            #endregion

            titlebuttoncontainer.Add(SeperateIconLabel);

            // 清空容器后重新按顺序添加
            titleContainer.Clear();
            AppendElement(GraphNodeContainerType.TitleContainer, NodeTitleIconLabel);
            AppendElement(GraphNodeContainerType.TitleContainer, TitleInputField);
            AppendElement(GraphNodeContainerType.TitleContainer, TitleLabel);
            AppendElement(GraphNodeContainerType.TitleContainer, titlebuttoncontainer);
        }

        /// <summary>
        /// 绘制主容器
        /// </summary>
        public virtual void Draw_Main()
        {
            mainContainer.style.overflow = new StyleEnum<Overflow>(Overflow.Visible);
            #region 头像组件
            if (ActionData.HasAvatar)
            {
                RegisterAvatarClicked();
            }
            #endregion
        }
        #endregion

        #region 标题回调事件
        /// <summary>
        /// 改变节点图标
        /// </summary>
        /// <param name="evt"></param>
        private void ChangeTitleIcon(PointerDownEvent evt)
        {
            // 只响应左键
            if (evt.button != (int)MouseButton.LeftMouse)
                return;

            if (evt.clickCount == 2)
            {
                OpenObjectPickerForTextures("TitleIconSet", "t:Texture2D", ActionData.NodeIcon);
            }

            evt.StopPropagation();
        }

        /// <summary>
        /// 节点名称设置
        /// </summary>
        /// <param name="evt"></param>
        private void OnTitleInputFieldBlur(BlurEvent evt)
        {
            Undo.RecordObject(ActionData, "Change ActionNode Name");

            if (TitleInputField.value != ActionData.name && TitleInputField.value != ActionData.identifyName)
                TitleLabel.text = ActionData.name = ActionData.identifyName = TitleInputField.value;

            ActionData.path = Regex.Replace(ActionData.path, @" > .*?\.asset$", $" > {TitleInputField.value}.asset");

            VisualElementDisplay(TitleLabel, true);
            VisualElementDisplay(TitleInputField, false);
        }
        #endregion

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

            VisualElementDisplay(TitleLabel, true);
            VisualElementDisplay(TitleInputField, false);
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

            VisualElementDisplay(TitleLabel, true);
            VisualElementDisplay(TitleInputField, false);
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

                ActionData.nodeGraphSize = newSize;
            }
        }
        /// <summary>
        /// 当Graphview克隆节点时调用
        /// </summary>
        /// <param name="list"></param>
        public virtual void OnDuplicatedNode(List<DuplicateNodeData> list)
        {

        }
        #endregion

        #region 头像设置
        /// <summary>
        /// 注册头像双击委托
        /// </summary>
        public void RegisterAvatarClicked()
        {
            if (AvatarIcon == null)
            {
                AvatarIcon = new VisualElement();
                AvatarIcon.name = "AvatarIcon";
                AvatarIcon.pickingMode = PickingMode.Position;
                AvatarIcon.style.backgroundImage = ActionData.Avatar;
                AvatarIcon.AddToClassList("Avatar_Icon");
                AppendElement(GraphNodeContainerType.MainContainer, AvatarIcon);
            }
            // 修改头像点击回调
            AvatarIcon.RegisterCallback<PointerDownEvent>((evt) =>
            {
                // 双击头像以更换头像
                if (evt.clickCount == 2)
                {
                    OpenObjectPickerForTextures("AvatarSet", "t:Texture2D", ActionData.Avatar);
                    evt.StopPropagation();
                }
            });
        }
        /// <summary>
        /// 注销头像双击委托
        /// </summary>
        public void UnregisterAvatarClicked()
        {
            if (AvatarIcon != null)
            {
                mainContainer.Remove(AvatarIcon);
                AvatarIcon = null;
            }
        }
        /// <summary>
        /// 检查是否设置了头像
        /// </summary>
        public void CheckAvatarChanged()
        {
            if (AvatarIcon == null)
                return;

            // 如果该节点设置了头像
            if (ActionData.HasAvatar)
            {
                // 头像组件的图片设置
                if (ActionData.Avatar != null)
                    AvatarIcon.style.backgroundImage = ActionData.Avatar;
                else
                    AvatarIcon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Avatars/Missing.png");
                // 标题组件的缩进内边距
                TitleLabel.style.paddingRight = 40;
                TitleInputField.style.paddingRight = 37;
            }
            else
            {
                // 头像组件的图片设置
                AvatarIcon.style.backgroundImage = null;
                // 标题组件的缩进内边距
                TitleLabel.style.paddingRight = 0;
                TitleInputField.style.paddingRight = 0;
            }
        }
        /// <summary>
        /// 设置Avatar
        /// </summary>
        /// <param name="tex"></param>
        public void NodeAvatar_Set(Texture2D tex)
        {
            Undo.RecordObject(ActionData, "Set ActionNode Avatar");
            // 头像状态开关 = 开
            ActionData.HasAvatar = true;
            // 头像图像设置
            ActionData.Avatar = tex;

            CheckAvatarChanged();

            // 调用视觉节点自身的移除头像的委托（可导致所在编组的内边距的扩展）
            if (OnNodeAvatar_Set != null)
                OnNodeAvatar_Set(this);
        }
        /// <summary>
        /// 移除Avatar
        /// </summary>
        /// <param name="tex"></param>
        public void NodeAvatar_Remove()
        {
            Undo.RecordObject(ActionData, "Remove ActionNode Avatar");
            // 头像状态开关 = 关
            ActionData.HasAvatar = false;
            // 头像图像移除
            ActionData.Avatar = null;

            CheckAvatarChanged();

            // 调用视觉节点自身的移除头像的委托（可导致所在编组的内边距的缩进）
            if (On_NodeAvatar_Clear != null)
                On_NodeAvatar_Clear(this);
        }
        #endregion

        #region 节点透明背景
        /// <summary>
        /// 检查节点背景透明化
        /// </summary>
        public void CheckTransparentDisplay(bool state)
        {
            if (state)
            {
                mainContainer.style.backgroundColor = Color.clear;
                mainContainer.style.borderTopColor = Color.clear;
                mainContainer.style.borderBottomColor = Color.clear;
                mainContainer.style.borderLeftColor = Color.clear;
                mainContainer.style.borderRightColor = Color.clear;
                extensionContainer.style.backgroundColor = Color.clear;
            }
        }

        /// <summary>
        /// 设置节点背景透明化
        /// </summary>
        /// <param name="state"></param>
        public void TransparentDisplay_Set(bool state)
        {
            Undo.RecordObject(ActionData, $"Set NodeTransparentMode - {state}");
            ActionData.TransparentNode = state;
            CheckTransparentDisplay(state);
        }
        #endregion

        #region 标题Icon设置
        /// <summary>
        /// 设置 标题Icon
        /// </summary>
        /// <param name="tex"></param>
        public void NodeTitleIcon_Set(Texture2D tex)
        {
            Undo.RecordObject(ActionData, "Set ActionNode TitleIcon");

            ActionData.NodeIcon = tex;
            NodeTitleIconLabel.style.backgroundImage = tex;
        }

        public void NodeTitleIcon_Restore()
        {
            NodeTitleIconLabel.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{this.icon}.png");
        }
        #endregion

        #region 辅助
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
        /// 设置节点配色
        /// </summary>
        public void UpdateMarkColor()
        {
            titleContainer.style.borderBottomColor = ActionData.themeColor;
        }
        /// <summary>
        /// 节点配色 - 隐藏
        /// </summary>
        public void MarkColor_Hidden()
        {
            titleContainer.style.borderBottomWidth = 0;
        }
        /// <summary>
        /// 节点配色 - 显示
        /// </summary>
        public void MarkColor_Dislay()
        {
            titleContainer.style.borderBottomColor = ActionData.themeColor;
            titleContainer.style.borderBottomWidth = 1;
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
        /// 执行模式
        /// </summary>
        public void CheckExecutionModel()
        {
            if (ActionData.isConcurrentExecution)
                SetConcurrent();
            else
                SetSequential();
        }
        /// <summary>
        /// 设置为并发模式
        /// </summary>
        public void SetConcurrent()
        {
            SeperateIconLabel.style.backgroundImage = tex_logo_dir_concurrent;
        }
        /// <summary>
        /// 设置为顺序模式
        /// </summary>
        public void SetSequential()
        {
            SeperateIconLabel.style.backgroundImage = tex_logo_dir_sequential;
        }
        /// <summary>
        /// 设置端口样式 - In
        /// </summary>
        /// <param name="nodeport"></param>
        public void SetPortStyle(xGraph_NodePort nodeport, PortStyleType type)
        {
            nodeport.Port.Q<VisualElement>(className: "port").AddToClassList(type == PortStyleType.In ? "Port_In" : "Port_Out");
            nodeport.Port.Q<Label>().AddToClassList(type == PortStyleType.In ? "PortText_In" : "PortText_Out");
        }
        /// <summary>
        /// 从输入端口获取特定的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Port GetVariablePort(Type type, string portName)
        {
            Port port = null;
            foreach (var item in Port_Inputs)
            {
                if (item.Type == type && item.Port.portName == portName)
                {
                    port = item.Port;
                }
            }

            return port;
        }
        #endregion

        #region 弹出物体选择面板
        public void OpenObjectPickerForTextures(string mode, string typefilter, Texture2D tex)
        {
            if (monitoringObjectPicker) return;

            monitoringObjectPicker = true;
            SetTextureMode = mode;

            // 动态创建 IMGUIContainer
            if (m_ObjectPickerIMGUI == null)
            {
                m_ObjectPickerIMGUI = new IMGUIContainer(OnObjectPickerGUI);
                m_ObjectPickerIMGUI.name = "---------------VNodeTexturePicker";
                m_ObjectPickerIMGUI.style.display = DisplayStyle.Flex;
                Add(m_ObjectPickerIMGUI);
            }
            EditorGUIUtility.ShowObjectPicker<Texture2D>(ActionData.Avatar, false, typefilter, 0);
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
            if (SetTextureMode == "TitleIconSet")
                NodeTitleIcon_Set(selectedTexture);
            if (SetTextureMode == "AvatarSet")
                NodeAvatar_Set(selectedTexture);
        }

        /// <summary>
        /// 根据VraiableCategory的变量列表源来更新在Actions列表 & Variables列表中所有用到这些变量的值
        /// </summary>
        internal void RefreshVariableValue()
        {
            // 遍历VariableDatas，为每一项数据重新匹配到ActionTreeAsset.VariableCategory中对应的变量项
            foreach (var v in graphView.ActionTreeAsset.VariableCategory)
            {
                foreach (var item in ActionData.VariableDatas)
                {
                    if (item.variable.guid == v.guid)
                    {
                        switch (item.variable.type)
                        {
                            case VariableType.String:
                                item.variable.SetValue<string>(v.GetValue<string>());
                                break;
                            case VariableType.Float:
                                item.variable.SetValue<float>(v.GetValue<float>());
                                break;
                            case VariableType.Int:
                                item.variable.SetValue<int>(v.GetValue<int>());
                                break;
                            case VariableType.Bool:
                                item.variable.SetValue<bool>(v.GetValue<bool>());
                                break;
                            case VariableType.Vector2:
                                item.variable.SetValue<Vector2>(v.GetValue<Vector2>());
                                break;
                            case VariableType.Vector3:
                                item.variable.SetValue<Vector3>(v.GetValue<Vector3>());
                                break;
                            case VariableType.Vector4:
                                item.variable.SetValue<Vector4>(v.GetValue<Vector4>());
                                break;
                            case VariableType.Color:
                                item.variable.SetValue<Color>(v.GetValue<Color>());
                                break;
                        }
                    }
                }
            }
        }
        #endregion


    }
}