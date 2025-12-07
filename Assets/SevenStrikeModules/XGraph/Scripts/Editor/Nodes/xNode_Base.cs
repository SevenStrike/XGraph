namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Base : Node
    {
        #region 组件
        /// <summary>
        /// GraphView组件
        /// </summary>
        public xg_GraphView graphView;
        #endregion

        #region 控件
        /// <summary>
        /// 视觉节点标题图标
        /// </summary>
        public Label NodeTitleIconLabel;
        /// <summary>
        /// 视觉节点头像组件
        /// </summary>
        public VisualElement AvatarIcon;
        /// <summary>
        /// 视觉节点标记组件
        /// </summary>
        public VisualElement nodeMark;
        /// <summary>
        /// 高亮面
        /// </summary>
        private VisualElement Highlighter;
        /// <summary>
        /// 设为起始节点的边框
        /// </summary>
        private VisualElement IsStartNodeBorder;
        /// <summary>
        /// 设为起始节点的标记
        /// </summary>
        private VisualElement IsStartNodeMark;
        /// <summary>
        /// 设为起始节点的文字
        /// </summary>
        public Label IsStartNodeText;
        /// <summary>
        /// 视觉节点图标
        /// </summary>
        public Label ExecutionIcon;
        /// <summary>
        /// 视觉节点标题
        /// </summary>
        public Label TitleLabel;
        /// <summary>
        /// 视觉节点输入框
        /// </summary>
        public TextField TitleInputField;
        /// <summary>
        /// IMGUI容器
        /// </summary>
        private IMGUIContainer m_ObjectPickerIMGUI;
        #endregion

        #region 参数
        /// <summary>
        /// 指定节点图标
        /// </summary>
        public string icon;
        /// <summary>
        /// 指示物体选择器是否已经打开
        /// </summary>
        private bool monitoringObjectPicker = false;
        /// <summary>
        /// 用于打开贴图选择器后选择贴图应用的模式
        /// </summary>
        public string SetTextureMode;
        /// <summary>
        /// 导图节点的最后一次尺寸
        /// </summary>
        private Vector2 m_LastSize;
        /// <summary>
        /// 顺序执行标记图标
        /// </summary>
        public Texture2D tex_logo_dir_sequential;
        /// <summary>
        /// 并发执行标记图标
        /// </summary>
        public Texture2D tex_logo_dir_concurrent;
        #endregion

        #region 回调
        /// <summary>
        /// 当选中节点时的委托事件
        /// </summary>
        public Action<xNode_Base> On_SelectedNode;
        /// <summary>
        /// 当选中节点时的委托事件
        /// </summary>
        public Action<xNode_Base> On_UnSelectedNode;
        #endregion

        #region 节点数据
        /// <summary>
        /// 节点携带的数据
        /// </summary>
        public xAction_Base ActionData;
        #endregion

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
        public virtual void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
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
            this.icon = data.BaseArgs.icon;
            this.viewDataKey = data != null ? data.BaseArgs.guid : "";
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

            // 注册黑板变量数值变化回调
            ActionData.BaseArgs.RootAsset.On_VariablesValue_Changed += On_VariablesValue_Changed;

            // 注册节点重建的回调
            ActionData.On_Node_Restructure = null;
            ActionData.On_Node_Restructure += On_Node_Restructure;

            // 注册节点创建连线的回调
            ActionData.On_Node_CreateEdge = null;
            ActionData.On_Node_CreateEdge += On_Node_CreateEdge;

            // 注册节点创建连线的回调
            ActionData.On_Node_RemovedEdge = null;
            ActionData.On_Node_RemovedEdge += On_Node_RemovedEdge;

            // 注册节点被指定为起始节点时
            ActionData.On_Node_IsStartNode = null;
            ActionData.On_Node_IsStartNode += On_Node_IsStartNode;

            // 当Graphview编辑器的主题色改变时
            graphView.gv_GraphWindow.OnThemeColorChanged += OnGraphViewEditorThemeColorChanged;
        }

        /// <summary>
        /// 刷新节点标记主题颜色
        /// </summary>
        public void UpdateNodeThemeColor()
        {
            if (ExecutionIcon != null)
                // 改变分割图标颜色
                ExecutionIcon.style.unityBackgroundImageTintColor = ActionData.BaseArgs.themeColor;

            // 更新节点标记颜色
            UpdateMarkColor(ActionData.BaseArgs.themeColor);

            // 改变输入端连线颜色
            if (Port_Inputs != null)
            {
                foreach (var x in Port_Inputs)
                {
                    x.Port.portColor = ActionData.BaseArgs.themeColor;
                    util_XGraphEditorUtility.Element_BorderColor_Set(x.PortDonut, ActionData.BaseArgs.themeColor);
                    var edges = x.Port.connections.ToList();
                    // 遍历所有连线
                    foreach (var edge in edges)
                    {
                        edge.edgeControl.inputColor = ActionData.BaseArgs.themeColor;
                    }
                }
            }

            // 改变输出端连线颜色
            if (Port_Outputs != null)
            {
                foreach (var x in Port_Outputs)
                {
                    x.Port.portColor = ActionData.BaseArgs.themeColor;
                    util_XGraphEditorUtility.Element_BorderColor_Set(x.PortDonut, ActionData.BaseArgs.themeColor);

                    var edges = x.Port.connections.ToList();
                    // 遍历所有连线
                    foreach (var edge in edges)
                    {
                        edge.edgeControl.outputColor = ActionData.BaseArgs.themeColor;
                    }
                }
            }

            // 如果节点标记开关被打开那个就显示标记
            if (graphView.gv_GraphWindow.xw_toggle_DisplayNodeColor.value)
                MarkColor_Dislay();
        }

        #region 订阅 Graphview 克隆动作
        /// <summary>
        /// 注册Graphview的克隆动作
        /// </summary>
        public void DuplicateAction_Add()
        {
            graphView.OnDuplicateNodes += On_Nodes_Duplicated;
        }

        /// <summary>
        /// 注销Graphview的克隆动作
        /// </summary>
        public void DuplicateAction_Remove()
        {
            graphView.OnDuplicateNodes -= On_Nodes_Duplicated;
        }
        #endregion

        /// <summary>
        /// 当拖动节点位置时，将位置数据传递给对应的目标数据节点位置变量
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(ActionData.BaseArgs.RootAsset, "SetPosition VisualNode");
            base.SetPosition(newPos);
            if (ActionData != null)
            {
                ActionData.BaseArgs.nodeGraphPosition.x = newPos.xMin;
                ActionData.BaseArgs.nodeGraphPosition.y = newPos.yMin;

                if (ActionData.On_Node_Moved != null)
                    ActionData.On_Node_Moved(ActionData.BaseArgs.nodeGraphPosition);
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
            if (ActionData.BaseArgs.actionNodeType == "Relay")
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
            if (ActionData.BaseArgs.actionNodeType == "Relay")
                return;
            var tex = DragAndDrop.objectReferences[0] as Texture2D;
            if (tex == null) return;

            NodeAvatar_Set(tex);
            RegisterAvatarClicked();

            evt.StopPropagation();
        }
        #endregion

        #region 端口设置
        public virtual xNode_Base InputPort_Set(List<xGraph_NodePort> portInfos)
        {
            Port_Inputs = portInfos;
            return this;
        }

        /// <summary>
        /// 增加输入端口
        /// </summary>
        /// <param name="port"></param>
        public void InputPort_Add(xGraph_NodePort port)
        {
            Port_Inputs.Add(port);
        }

        public virtual xNode_Base OutputPort_Set(List<xGraph_NodePort> portInfos)
        {
            Port_Outputs = portInfos;
            return this;
        }

        /// <summary>
        /// 增加输出端口
        /// </summary>
        /// <param name="port"></param>
        public void OutputPort_Add(xGraph_NodePort port)
        {
            Port_Outputs.Add(port);
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
            Port port = Port.Create<util_AnimatedEdge>(orientation, direction, capacity, type);
            //Port port = InstantiatePort(orientation, direction, capacity, type);
            port.portName = name;
            port.portColor = nodeThemeColor;
            return port;
        }
        #endregion

        #region 节点绘制
        /// <summary>
        /// 绘制节点
        /// </summary>
        public virtual xNode_Base Draw()
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

        }

        /// <summary>
        /// 绘制输入端口
        /// </summary>
        public virtual void Draw_Input()
        {
            foreach (var p in Port_Inputs)
            {
                p.Port = Port_Create(p.Name, Orientation.Horizontal, Direction.Input, p.Capacity, p.Type, ActionData.BaseArgs.themeSolution == "M 默认" ? Color.white * 0.7f : ActionData.BaseArgs.themeColor);
                p.PortDonut = p.Port.Q<VisualElement>("connector");
                SetPortStyle(p, xPortType.In);
                AppendElement(xNodeContainerType.InputContainer, p.Port);
            }
        }

        /// <summary>
        /// 绘制输出节点容器
        /// </summary>
        public virtual void Draw_Output()
        {
            foreach (var p in Port_Outputs)
            {
                // 绘制端口 - 输出
                p.Port = Port_Create(p.Name, Orientation.Horizontal, Direction.Output, p.Capacity, p.Type, ActionData.BaseArgs.themeSolution == "M 默认" ? Color.white * 0.7f : ActionData.BaseArgs.themeColor);
                p.PortDonut = p.Port.Q<VisualElement>("connector");
                SetPortStyle(p, xPortType.Out);
                AppendElement(xNodeContainerType.OutputContainer, p.Port);
            }
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
            #region 节点标题图标
            NodeTitleIconLabel = new Label("");
            NodeTitleIconLabel.AddToClassList("Title_Icon");
            #endregion

            #region 如果指定了图标就不用根据名称指定的图标了
            if (ActionData.BaseArgs.NodeIcon != null)
                NodeTitleIcon_Set(ActionData.BaseArgs.NodeIcon);
            else
                NodeTitleIcon_Restore();
            NodeTitleIconLabel.RegisterCallback<PointerDownEvent>(ChangeTitleIcon);
            #endregion

            #region 用于显示节点名称
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
            #endregion

            #region 用于编辑节点名称
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
            #endregion

            #region 节点折叠 / 展开按钮
            VisualElement titlebuttoncontainer = titleContainer.Q<VisualElement>("title-button-container");
            #endregion

            #region 节点标记
            CreateNodeMark();
            #endregion

            titlebuttoncontainer.style.borderRightWidth = 0;

            #region 执行模式图标
            ExecutionModeMark();
            #endregion

            #region 应用配置文件的颜色到节点的标识颜色
            foreach (var colorData in graphView.NodeThemesList.Node)
            {
                if (colorData.solution == ActionData.BaseArgs.themeSolution)
                {
                    ExecutionIcon.style.unityBackgroundImageTintColor = ActionData.BaseArgs.themeSolution == "M 默认" ? Color.white : ActionData.BaseArgs.themeColor;
                }
            }
            #endregion

            titlebuttoncontainer.Add(ExecutionIcon);

            // 清空容器后重新按顺序添加
            titleContainer.Clear();
            AppendElement(xNodeContainerType.TitleContainer, NodeTitleIconLabel);
            AppendElement(xNodeContainerType.TitleContainer, TitleInputField);
            AppendElement(xNodeContainerType.TitleContainer, TitleLabel);
            AppendElement(xNodeContainerType.TitleContainer, titlebuttoncontainer);
            AppendElement(xNodeContainerType.TitleContainer, nodeMark);
        }

        /// <summary>
        /// 绘制主容器
        /// </summary>
        public virtual void Draw_Main()
        {
            mainContainer.style.overflow = new StyleEnum<Overflow>(Overflow.Visible);
            CreateHighlighter();
            CreateIsStartNodeMark();

            #region 头像组件
            if (ActionData.BaseArgs.HasAvatar)
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
                OpenObjectPickerForTextures("TitleIconSet", "t:Texture2D", ActionData.BaseArgs.NodeIcon);
            }

            evt.StopPropagation();
        }

        /// <summary>
        /// 节点名称设置
        /// </summary>
        /// <param name="evt"></param>
        private void OnTitleInputFieldBlur(BlurEvent evt)
        {
            //Undo.RecordObject(ActionData, "Change ActionNode Name");

            if (TitleInputField.value != ActionData.identifyName)
                TitleLabel.text = ActionData.identifyName = TitleInputField.value;

            VisualElementDisplay(TitleLabel, true);
            VisualElementDisplay(TitleInputField, false);

            // 更新变量赋值数据
            graphView.ActionTreeAsset.Variables_Refresh();

            if (ActionData.On_Node_TitleChanged != null)
                ActionData.On_Node_TitleChanged(TitleLabel.text);
        }
        #endregion

        #region 数据流效果控制
        public void SetConnectedEdgesFlow(bool enable)
        {
            var edges = new HashSet<util_AnimatedEdge>();

            // 收集所有连接的边缘
            foreach (var port in Port_Inputs)
            {
                foreach (var con in port.Port.connections)
                {
                    if (con is util_AnimatedEdge edge)
                    {
                        edges.Add(edge);
                    }
                }
            }
            foreach (var port in Port_Outputs)
            {
                foreach (var con in port.Port.connections)
                {
                    if (con is util_AnimatedEdge edge)
                    {
                        if (con.output.node is xNode_Branch branch)
                        {
                            if (branch.ActionData is xAction_Branch bra)
                            {
                                if (!enable)
                                {
                                    edges.Add(edge);
                                }
                                else
                                {
                                    if (bra.PredicateState)
                                    {
                                        if (con.output.portName == "开")
                                        {
                                            edges.Add(edge);
                                        }
                                    }
                                    else
                                    {
                                        if (con.output.portName == "关")
                                        {
                                            edges.Add(edge);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            edges.Add(edge);
                        }
                    }
                }
            }

            // 批量设置
            foreach (var edge in edges)
            {
                edge.EnableFlow = enable;
            }
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
            if (On_SelectedNode != null)
            {
                On_SelectedNode.Invoke(this);
            }

            DuplicateAction_Add();

            // 数据流效果  -  开启
            if (graphView.gv_GraphWindow.DisplayNodeFlow)
                SetConnectedEdgesFlow(true);

            VisualElementDisplay(TitleLabel, true);
            VisualElementDisplay(TitleInputField, false);

            // 注册节点主题颜色变化回调
            ActionData.On_Node_ThemeColorChanged += On_Node_ThemeColorChanged;
            ActionData.On_Node_AvatarChanged += On_Node_AvatarChanged;
            ActionData.On_Node_IconChanged += On_Node_IconChanged;
            ActionData.On_Node_ConcurrentChanged += On_Node_ConcurrentChanged;
            ActionData.On_Node_TransparentChanged += On_Node_TransparentChanged;
        }
        /// <summary>
        /// 取消选择时
        /// </summary>
        public override void OnUnselected()
        {
            base.OnUnselected();

            // 调用回调事件
            if (On_UnSelectedNode != null)
            {
                On_UnSelectedNode.Invoke(this);
            }

            // 数据流效果  -  关闭
            SetConnectedEdgesFlow(false);

            VisualElementDisplay(TitleLabel, true);
            VisualElementDisplay(TitleInputField, false);

            // 清空回调
            ActionData.On_Node_Moved = null;
            ActionData.On_Node_SizeChanged = null;
            ActionData.On_Node_IconChanged = null;
            ActionData.On_Node_AvatarChanged = null;
            ActionData.On_Node_TitleChanged = null;
            ActionData.On_Node_ThemeColorChanged = null;
            ActionData.On_Node_ConcurrentChanged = null;
            ActionData.On_Node_TransparentChanged = null;

            DuplicateAction_Remove();
        }
        /// <summary>
        /// 当节点尺寸发生改变时
        /// </summary>
        /// <param name="evt"></param>
        public virtual void OnSizeChanged(GeometryChangedEvent evt)
        {
            Vector2 newSize = new Vector2(evt.newRect.width, evt.newRect.height);

            if (Vector2.Distance(m_LastSize, newSize) > 1f) // 1像素阈值
            {
                m_LastSize = newSize;

                // 修复：正确的撤销目标应该是 ActionData
                if (ActionData != null)
                {
                    //Undo.RecordObject(ActionData, "Change Node Size");
                    ActionData.BaseArgs.nodeGraphSize = newSize;

                    // 如果需要，也可以标记资产为脏
                    if (graphView?.ActionTreeAsset != null)
                    {
                        EditorUtility.SetDirty(graphView.ActionTreeAsset);
                    }

                    if (ActionData.On_Node_SizeChanged != null)
                        ActionData.On_Node_SizeChanged(ActionData.BaseArgs.nodeGraphSize);
                }
            }
        }
        /// <summary>
        /// 当Graphview克隆节点时调用
        /// </summary>
        /// <param name="list"></param>
        public virtual void On_Nodes_Duplicated(List<DuplicateNodeData> list)
        {
            foreach (var node in list)
            {
                xNode_Base n_base = node.DuplicatedNode as xNode_Base;
                // 找到克隆的父物体行为节点
                xNode_Base source = graphView.FindNodeView(node.SourceNodeGuid);

                // 调用行为数据脚本中的 On_Node_Duplicated 事件以便于行为数据Editor界面下的控件获取克隆父物体的特定变量数据
                if (n_base.ActionData.On_Node_Duplicated != null)
                    n_base.ActionData.On_Node_Duplicated(n_base.ActionData, source.ActionData);
            }
        }
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public virtual void On_VariablesValue_Changed()
        {

        }
        /// <summary>
        /// 当该节点被重建时的回调
        /// </summary>
        public virtual void On_Node_Restructure()
        {

        }
        /// <summary>
        /// 当该节点创建连线时
        /// </summary>
        /// <param name="edge"></param>
        public virtual void On_Node_CreateEdge(Edge edge)
        {

        }
        /// <summary>
        /// 当该节点移除连线时
        /// </summary>
        /// <param name="edge"></param>
        public virtual void On_Node_RemovedEdge(Edge edge)
        {

        }
        /// <summary>
        /// 当改变节点通透样式时
        /// </summary>
        /// <param name="state"></param>
        public virtual void On_Node_TransparentChanged(bool state)
        {
            TransparentDisplay_Set(state);
        }
        /// <summary>
        /// 当节点主题色改变时
        /// </summary>
        public virtual void On_Node_ThemeColorChanged()
        {
            UpdateNodeThemeColor();
        }
        /// <summary>
        /// 改变头像时
        /// </summary>
        /// <param name="tex"></param>
        public virtual void On_Node_AvatarChanged(Texture2D tex)
        {
            CheckAvatarChanged();
        }
        /// <summary>
        /// 改变图标时
        /// </summary>
        /// <param name="tex"></param>
        public virtual void On_Node_IconChanged(Texture2D tex)
        {
            NodeTitileICon_Check();
        }
        /// <summary>
        /// 改变执行模式
        /// </summary>
        /// <param name="obj"></param>
        public virtual void On_Node_ConcurrentChanged(bool obj)
        {
            CheckExecutionModel();
        }
        /// <summary>
        /// 设为起始节点时
        /// </summary>
        /// <param name="state"></param>
        private void On_Node_IsStartNode(bool state)
        {
            StartNodeMark_Displayer(state);
        }
        private void OnGraphViewEditorThemeColorChanged(Color color)
        {
            StartNodeMark_Displayer(ActionData.BaseArgs.isStartNode);
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
                CreateAvatarElement();
            }
            // 修改头像点击回调
            AvatarIcon.RegisterCallback<PointerDownEvent>((evt) =>
            {
                // 双击头像以更换头像
                if (evt.clickCount == 2)
                {
                    OpenObjectPickerForTextures("AvatarSet", "t:Texture2D", ActionData.BaseArgs.Avatar);
                    evt.StopPropagation();
                }
            });
        }

        /// <summary>
        /// 创建头像组件
        /// </summary>
        public void CreateAvatarElement()
        {
            if (AvatarIcon != null)
                return;
            AvatarIcon = new VisualElement();
            AvatarIcon.name = "AvatarIcon";
            AvatarIcon.pickingMode = PickingMode.Position;
            AvatarIcon.style.backgroundImage = ActionData.BaseArgs.Avatar;
            AvatarIcon.AddToClassList("Avatar_Icon");
            AppendElement(xNodeContainerType.MainContainer, AvatarIcon);
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
            if (ActionData.BaseArgs.HasAvatar)
            {
                // 头像组件的图片设置
                if (ActionData.BaseArgs.Avatar != null)
                    AvatarIcon.style.backgroundImage = ActionData.BaseArgs.Avatar;
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
            //Undo.RecordObject(ActionData, "Set ActionNode Avatar");
            // 头像状态开关 = 开
            ActionData.BaseArgs.HasAvatar = true;
            // 头像图像设置
            ActionData.BaseArgs.Avatar = tex;

            CheckAvatarChanged();

            if (ActionData.On_Node_AvatarChanged != null)
                ActionData.On_Node_AvatarChanged(tex);
        }
        /// <summary>
        /// 移除Avatar
        /// </summary>
        /// <param name="tex"></param>
        public void NodeAvatar_Remove()
        {
            //Undo.RecordObject(ActionData, "Remove ActionNode Avatar");
            // 头像状态开关 = 关
            ActionData.BaseArgs.HasAvatar = false;
            // 头像图像移除
            ActionData.BaseArgs.Avatar = null;

            CheckAvatarChanged();

            if (ActionData.On_Node_AvatarChanged != null)
                ActionData.On_Node_AvatarChanged(null);
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
            //Undo.RecordObject(ActionData, $"Set NodeTransparentMode - {state}");
            ActionData.BaseArgs.TransparentNode = state;
            CheckTransparentDisplay(state);

            graphView.Restructure_Graph(ActionData.BaseArgs.RootAsset);
        }
        #endregion

        #region 标题Icon设置
        /// <summary>
        /// 标题图标检查
        /// </summary>
        public void NodeTitileICon_Check()
        {
            if (ActionData.BaseArgs.NodeIcon != null)
                NodeTitleIcon_Set(ActionData.BaseArgs.NodeIcon);
            else
                NodeTitleIcon_Restore();
        }
        /// <summary>
        /// 设置 标题Icon
        /// </summary>
        /// <param name="tex"></param>
        public void NodeTitleIcon_Set(Texture2D tex)
        {
            //Undo.RecordObject(ActionData, "Set ActionNode TitleIcon");

            ActionData.BaseArgs.NodeIcon = tex;
            NodeTitleIconLabel.style.backgroundImage = tex;
        }

        public void NodeTitleIcon_Restore()
        {
            //NodeTitleIconLabel.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{this.icon}.png");
            NodeTitleIconLabel.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>(AssetDatabase.GUIDToAssetPath(icon));
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 创建节点颜色标记
        /// </summary>
        private void CreateNodeMark()
        {
            nodeMark = new VisualElement();
            nodeMark.AddToClassList("nodeMark");
        }
        /// <summary>
        /// 执行模式图标创建
        /// </summary>
        public virtual void ExecutionModeMark()
        {
            ExecutionIcon = new Label("");
            ExecutionIcon.AddToClassList("ExecutionIcon");
            CheckExecutionModel();
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
        /// 设置节点配色
        /// </summary>
        public void UpdateMarkColor(Color color)
        {
            nodeMark.style.backgroundColor = color;
        }
        /// <summary>
        /// 节点配色 - 隐藏
        /// </summary>
        public void MarkColor_Hidden()
        {
            if (nodeMark == null)
                return;
            nodeMark.style.opacity = 0;
        }
        /// <summary>
        /// 节点配色 - 显示
        /// </summary>
        public void MarkColor_Dislay()
        {
            if (nodeMark == null)
                return;
            nodeMark.style.backgroundColor = ActionData.BaseArgs.themeColor;

            nodeMark.style.opacity = 1;
        }
        /// <summary>
        /// 添加元素到指定类型的容器中
        /// </summary>
        /// <param name="type"></param>
        /// <param name="element"></param>
        protected void AppendElement(xNodeContainerType type, VisualElement element)
        {
            switch (type)
            {
                case xNodeContainerType.MainContainer:
                    mainContainer.Add(element);
                    break;
                case xNodeContainerType.TitleContainer:
                    titleContainer.Add(element);
                    break;
                case xNodeContainerType.TitleButtonContainer:
                    titleButtonContainer.Add(element);
                    break;
                case xNodeContainerType.TopContainer:
                    topContainer.Add(element);
                    break;
                case xNodeContainerType.InputContainer:
                    inputContainer.Add(element);
                    break;
                case xNodeContainerType.OutputContainer:
                    outputContainer.Add(element);
                    break;
                case xNodeContainerType.ExtensionContainer:
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
            if (ActionData.BaseArgs.isConcurrentExecution)
                SetConcurrent();
            else
                SetSequential();
        }
        /// <summary>
        /// 设置为并发模式
        /// </summary>
        public void SetConcurrent()
        {
            if (ExecutionIcon != null)
                ExecutionIcon.style.backgroundImage = tex_logo_dir_concurrent;
        }
        /// <summary>
        /// 设置为顺序模式
        /// </summary>
        public void SetSequential()
        {
            if (ExecutionIcon != null)
                ExecutionIcon.style.backgroundImage = tex_logo_dir_sequential;
        }
        /// <summary>
        /// 设置端口样式 - In
        /// </summary>
        /// <param name="nodeport"></param>
        public void SetPortStyle(xGraph_NodePort nodeport, xPortType type)
        {
            nodeport.Port.Q<VisualElement>(className: "port").AddToClassList(type == xPortType.In ? "Port_In" : "Port_Out");
            nodeport.Port.Q<Label>().AddToClassList(type == xPortType.In ? "PortText_In" : "PortText_Out");
        }
        /// <summary>
        /// 根据特定的类型和指定的端口名称获取输入或是输出端口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="portName"></param>
        /// <param name="portType"></param>
        /// <returns></returns>
        public Port GetPort(Type type, string portName, xPortType portType)
        {
            Port x_port = null;

            switch (portType)
            {
                case xPortType.In:
                    foreach (var port in Port_Inputs)
                    {
                        if (port.Port.portName == portName && port.Type == type)
                        {
                            x_port = port.Port;
                        }
                    }
                    break;
                case xPortType.Out:
                    foreach (var port in Port_Outputs)
                    {
                        if (port.Port.portName == portName && port.Type == type)
                        {
                            x_port = port.Port;
                        }
                    }
                    break;
            }

            return x_port;
        }
        /// <summary>
        /// 根据指定的端口名称获取输入或是输出端口
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="portType"></param>
        /// <returns></returns>
        public Port GetPort(string portName, xPortType portType)
        {
            Port x_port = null;

            switch (portType)
            {
                case xPortType.In:
                    foreach (var port in Port_Inputs)
                    {
                        if (port.Port.portName == portName)
                        {
                            x_port = port.Port;
                        }
                    }
                    break;
                case xPortType.Out:
                    foreach (var port in Port_Outputs)
                    {
                        if (port.Port.portName == portName)
                        {
                            x_port = port.Port;
                        }
                    }
                    break;
            }

            return x_port;
        }
        /// <summary>
        /// 获取第一个输入或是输出端口
        /// </summary>
        /// <param name="portType"></param>
        /// <returns></returns>
        public Port GetFirstPort(xPortType portType)
        {
            Port x_port = null;

            switch (portType)
            {
                case xPortType.In:
                    x_port = Port_Inputs.First().Port;
                    break;
                case xPortType.Out:
                    x_port = Port_Outputs.First().Port;
                    break;
            }

            return x_port;
        }
        #endregion

        #region 高亮
        /// <summary>
        /// 创建高亮面
        /// </summary>
        public void CreateHighlighter()
        {
            #region 高亮面
            Highlighter = new VisualElement();
            Highlighter.pickingMode = PickingMode.Ignore;
            Highlighter.name = "HighlighterVisualler";
            Highlighter.AddToClassList("node_highlighter");
            util_XGraphEditorUtility.Element_BackgroundColor_Set(Highlighter, ActionData.BaseArgs.themeColor);
            UnHighlight();
            AppendElement(xNodeContainerType.MainContainer, Highlighter);
            Highlighter.BringToFront();
            #endregion
        }
        /// <summary>
        /// 高亮显示节点
        /// </summary>        
        public virtual void Highlight()
        {
            if (ActionData is xAction_Relay relay)
            {
                Highlighter.style.borderBottomLeftRadius = 13;
                Highlighter.style.borderBottomRightRadius = 13;
                Highlighter.style.borderTopLeftRadius = 13;
                Highlighter.style.borderTopRightRadius = 13;
            }
            util_XGraphEditorUtility.Element_BackgroundColor_Set(Highlighter, ActionData.BaseArgs.themeColor);
            util_XGraphEditorUtility.Element_Opacity_Set(Highlighter, 0.5f);
        }
        /// <summary>
        /// 取消高亮显示节点
        /// </summary>
        public virtual void UnHighlight()
        {
            util_XGraphEditorUtility.Element_Opacity_Set(Highlighter, 0);
        }
        #endregion

        #region 创建标记为起始节点的元素
        /// <summary>
        /// 创建标记为起始节点的元素
        /// </summary>
        public void CreateIsStartNodeMark()
        {
            #region 边
            IsStartNodeBorder = new VisualElement();
            IsStartNodeBorder.pickingMode = PickingMode.Ignore;
            IsStartNodeBorder.name = "StartNodeBorder";
            IsStartNodeBorder.AddToClassList("startnode_border");
            AppendElement(xNodeContainerType.MainContainer, IsStartNodeBorder);
            IsStartNodeBorder.BringToFront();
            #endregion

            #region 标记
            IsStartNodeMark = new VisualElement();
            IsStartNodeMark.pickingMode = PickingMode.Ignore;
            IsStartNodeMark.name = "StartNodeMark";
            IsStartNodeMark.AddToClassList("startnode_mark");
            IsStartNodeBorder.Add(IsStartNodeMark);
            IsStartNodeMark.BringToFront();
            #endregion

            #region 文字
            IsStartNodeText = new Label();
            IsStartNodeText.pickingMode = PickingMode.Ignore;
            IsStartNodeText.name = "StartNodeText";
            IsStartNodeText.text = "起始节点";
            IsStartNodeText.AddToClassList("startnode_text");
            IsStartNodeBorder.Add(IsStartNodeText);
            IsStartNodeText.BringToFront();
            #endregion

            StartNodeMark_Displayer(ActionData.BaseArgs.isStartNode);
        }

        /// <summary>
        /// 起始节点标记可见性控制
        /// </summary>
        /// <param name="state"></param>
        public void StartNodeMark_Displayer(bool state)
        {
            bool isrelay = this is xNode_Relay;
            util_XGraphEditorUtility.Element_BorderRadius_Set(IsStartNodeBorder, isrelay ? 13 : 5, isrelay ? 13 : 5, isrelay ? 13 : 5, isrelay ? 13 : 5);
            util_XGraphEditorUtility.Element_BorderColor_Set(IsStartNodeBorder, state ? graphView.ActionTreeAsset.GraphviewGridBackgroundThemes.themecolor : Color.clear);
            util_XGraphEditorUtility.Element_BackgroundColor_Set(IsStartNodeMark, state ? graphView.ActionTreeAsset.GraphviewGridBackgroundThemes.themecolor : Color.clear);
            util_XGraphEditorUtility.Element_Color_Set(IsStartNodeText, state ? Color.white : Color.clear);
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
            EditorGUIUtility.ShowObjectPicker<Texture2D>(ActionData.BaseArgs.Avatar, false, typefilter, 0);
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
            foreach (var v in graphView.ActionTreeAsset.BlackboardVariable)
            {
                foreach (var item in ActionData.BaseArgs.VariableDatas)
                {
                    if (item.variable.guid == v.guid)
                    {
                        switch (item.variable.type)
                        {
                            case xVariableType.String:
                                item.variable.SetValue<string>(v.GetValue<string>());
                                break;
                            case xVariableType.Float:
                                item.variable.SetValue<float>(v.GetValue<float>());
                                break;
                            case xVariableType.Int:
                                item.variable.SetValue<int>(v.GetValue<int>());
                                break;
                            case xVariableType.Bool:
                                item.variable.SetValue<bool>(v.GetValue<bool>());
                                break;
                            case xVariableType.Vector2:
                                item.variable.SetValue<Vector2>(v.GetValue<Vector2>());
                                break;
                            case xVariableType.Vector3:
                                item.variable.SetValue<Vector3>(v.GetValue<Vector3>());
                                break;
                            case xVariableType.Vector4:
                                item.variable.SetValue<Vector4>(v.GetValue<Vector4>());
                                break;
                            case xVariableType.Color:
                                item.variable.SetValue<Color>(v.GetValue<Color>());
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        //------------------------------------------------- inspector 界面

        #region 绘制界面
        /// <summary>
        /// 绘制节点编辑器内的Inspector界面
        /// </summary>
        /// <returns></returns>
        public virtual VisualElement DrawInspectorGUI()
        {
            VisualElement rootElement = new VisualElement();

            #region 标题
            VisualElement titlegroup = util_XGraphInspectorGUI.GUI_Title(rootElement, ActionData, ActionData.identifyName, new string[] { "titlegroup" }, new string[] { "titleicon" }, new string[] { "titlename" });
            #endregion

            #region 标题附加 - 变量类型标签
            string[] styles_sub = new string[] { "type" };
            Label lab_sub = new Label("行为节点");
            lab_sub.name = "sub";
            for (int i = 0; i < styles_sub.Length; i++)
            {
                lab_sub.AddToClassList(styles_sub[i]);
            }
            titlegroup.Add(lab_sub);
            #endregion

            #region 同步节点名称改变
            Label titlename = titlegroup.Q<Label>(name: "title");
            ActionData.On_Node_TitleChanged += (name) =>
            {
                titlename.text = name;
            };
            #endregion

            #region 同步节点图标改变
            VisualElement titleIcon = titlegroup.Q<VisualElement>(name: "icon");
            ActionData.On_Node_IconChanged += (tex) =>
            {
                titleIcon.style.backgroundImage = tex;
            };
            #endregion

            #region 节点基础属性折叠器
            Foldout fo_node = util_XGraphInspectorGUI.GUI_Foldout(rootElement, "节点基础属性", "basetype-base", new string[] { "foldout" });
            #endregion

            #region 节点GUID
            TextField textField_guid = util_XGraphInspectorGUI.GUI_Field_String(fo_node, "<b>GUID： </b>", ActionData.BaseArgs.guid, new string[1] { "field_text" });
            textField_guid.RegisterCallback<BlurEvent>((evt) =>
            {
                TextField field = evt.target as TextField;
                field.value = ActionData.BaseArgs.guid;
            });
            #endregion

            #region 行为类型
            TextField textField_actionNode_type = util_XGraphInspectorGUI.GUI_Field_String(fo_node, "<b>行为类型： </b>", ActionData.BaseArgs.actionNodeType, new string[1] { "field_text" });
            textField_actionNode_type.RegisterCallback<BlurEvent>((evt) =>
            {
                TextField field = evt.target as TextField;
                field.value = ActionData.BaseArgs.actionNodeType;
            });
            #endregion

            #region 节点类型
            TextField textField_visualNode_type = util_XGraphInspectorGUI.GUI_Field_String(fo_node, "<b>节点类型： </b>", ActionData.BaseArgs.visualNodeType, new string[1] { "field_text" });
            textField_visualNode_type.RegisterCallback<BlurEvent>((evt) =>
            {
                TextField field = evt.target as TextField;
                field.value = ActionData.BaseArgs.visualNodeType;
            });
            #endregion

            #region 节点颜色
            ColorField themecolor = util_XGraphInspectorGUI.GUI_Field_Color(fo_node, "标记色", ActionData.BaseArgs.themeColor, new string[] { "field_color" });
            themecolor.RegisterValueChangedCallback(value =>
            {
                ActionData.BaseArgs.themeSolution = "自定义";
                ActionData.BaseArgs.themeColor = themecolor.value;

                if (ActionData.On_Node_ThemeColorChanged != null)
                    ActionData.On_Node_ThemeColorChanged();
            });
            // 根据节点视图右键菜单更改主题颜色的操作被动更新颜色框值
            ActionData.On_Node_ThemeColorChanged += () =>
            {
                themecolor.value = ActionData.BaseArgs.themeColor;
            };
            #endregion

            #region 通透样式
            Toggle tog_transparentNode = util_XGraphInspectorGUI.GUI_Field_Bool(fo_node, "通透样式：", ActionData.BaseArgs.TransparentNode, new string[] { "field_bool" });
            tog_transparentNode.RegisterValueChangedCallback((value) =>
            {
                //Undo.RecordObject(baseScript, "Change TransparentNode");
                ActionData.BaseArgs.TransparentNode = value.newValue;

                if (ActionData.On_Node_TransparentChanged != null)
                    ActionData.On_Node_TransparentChanged(value.newValue);
            });
            ActionData.On_Node_TransparentChanged += (value) =>
            {
                tog_transparentNode.value = value;
            };
            #endregion

            #region 节点头像
            ObjectField avatarobj = util_XGraphInspectorGUI.GUI_Object<Texture2D>(fo_node, "头像", ActionData.BaseArgs.Avatar, new string[] { "field_object" });
            avatarobj.RegisterValueChangedCallback(value =>
            {
                //Undo.RecordObject(baseScript, "Change Avatar");
                if (avatarobj.value != null)
                    ActionData.BaseArgs.HasAvatar = true;
                else
                    ActionData.BaseArgs.HasAvatar = false;
                Texture2D tex = value.newValue as Texture2D;
                ActionData.BaseArgs.Avatar = tex;

                // 调用创建头像组件方法
                xg_Window win = util_XGraphEditorUtility.GetGraphviewWindow();
                xNode_Base node = win.xw_graphView.FindNodeView(ActionData.BaseArgs.guid);
                node.CreateAvatarElement();

                if (ActionData.On_Node_AvatarChanged != null)
                    ActionData.On_Node_AvatarChanged(tex);
            });
            ActionData.On_Node_AvatarChanged += (tex) =>
            {
                avatarobj.value = tex;
            };
            #endregion

            #region 节点图标
            ObjectField iconobj = util_XGraphInspectorGUI.GUI_Object<Texture2D>(fo_node, "图标", ActionData.BaseArgs.NodeIcon, new string[] { "field_object" });
            iconobj.RegisterValueChangedCallback(value =>
            {
                //Undo.RecordObject(baseScript, "Change Avatar");
                Texture2D tex = value.newValue as Texture2D;
                ActionData.BaseArgs.NodeIcon = tex;

                if (ActionData.On_Node_IconChanged != null)
                    ActionData.On_Node_IconChanged(tex);
            });
            ActionData.On_Node_IconChanged += (tex) =>
            {
                iconobj.value = tex;
            };
            #endregion

            #region 节点尺寸
            Vector2Field label_size = util_XGraphInspectorGUI.GUI_Field_Vector2(fo_node, "尺寸", ActionData.BaseArgs.nodeGraphSize, new string[] { "field_vector2" });
            ActionData.On_Node_SizeChanged += (size) =>
            {
                label_size.value = size;
            };
            label_size.RegisterCallback<BlurEvent>((evt) =>
            {
                Vector2Field field = evt.target as Vector2Field;
                field.value = ActionData.BaseArgs.nodeGraphSize;
            });
            #endregion

            #region 节点位置
            Vector2Field label_pos = util_XGraphInspectorGUI.GUI_Field_Vector2(fo_node, "位置", ActionData.BaseArgs.nodeGraphPosition, new string[] { "field_vector2" });
            ActionData.On_Node_Moved += (pos) =>
            {
                label_pos.value = pos;
            };
            label_pos.RegisterCallback<BlurEvent>((evt) =>
            {
                Vector2Field field = evt.target as Vector2Field;
                field.value = ActionData.BaseArgs.nodeGraphPosition;
            });
            #endregion

            #region 节点并发模式
            // 要忽略掉属性节点，因为属性节点不参与行为的流程执行逻辑
            if (ActionData is not xAction_Property)
            {
                Toggle tog_concurrent = util_XGraphInspectorGUI.GUI_Field_Bool(fo_node, "并发模式：", ActionData.BaseArgs.isConcurrentExecution, new string[] { "field_bool" });
                tog_concurrent.RegisterValueChangedCallback((value) =>
                {
                    //Undo.RecordObject(ActionData, "Change ConcurrentMode");
                    ActionData.BaseArgs.isConcurrentExecution = value.newValue;

                    if (ActionData.On_Node_ConcurrentChanged != null)
                        ActionData.On_Node_ConcurrentChanged(value.newValue);
                });
                ActionData.On_Node_ConcurrentChanged += (value) =>
                {
                    tog_concurrent.value = value;
                };
            }
            #endregion

            #region 设置起始节点
            // 要忽略掉属性节点，因为不能将属性节点作为起始节点
            if (ActionData is not xAction_Property)
            {
                Toggle tog_isStartNode = util_XGraphInspectorGUI.GUI_Field_Bool(fo_node, "起始节点：", ActionData.BaseArgs.isStartNode, new string[] { "field_bool" });
                tog_isStartNode.RegisterValueChangedCallback((value) =>
                {
                    //Undo.RecordObject(baseScript, "Change IsStartNode");
                    ActionData.BaseArgs.isStartNode = value.newValue;

                    ActionData.BaseArgs.RootAsset.SetStartNode(ActionData);
                });
                ActionData.On_Node_IsStartNode += (value) =>
                {
                    tog_isStartNode.value = value;
                };
            }
            #endregion

            #region 父行为
            ins_Folder_ParentNode(rootElement);
            #endregion

            #region 子节点折叠器
            ins_Folder_ChildActions(rootElement);
            #endregion

            #region Variables 折叠器         
            ins_Folder_BlackBoardVariable(rootElement);
            ActionData.On_Node_Variable_Binded += (value) =>
            {
                ins_Folder_BlackBoardVariable(rootElement);
            };

            #endregion

            #region InternalVariables 折叠器           
            ins_Folder_InternalVariable(rootElement);
            ActionData.On_Node_Variable_Binded += (value) =>
            {
                ins_Folder_InternalVariable(rootElement);
            };
            #endregion

            #region 属性参数折叠器
            ins_Folder_Propertys(rootElement);
            #endregion

            #region 节点绑定的属性记录折叠器
            ins_Folder_BindedPropertys(rootElement);
            #endregion

            #region 自定义扩展 折叠器
            ins_Folder_Extensions(rootElement);
            #endregion

            return rootElement;
        }
        #endregion

        #region 折叠器
        public virtual Foldout ins_Folder_ParentNode(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, "父行为", "parent", new string[] { "foldout" });
            fold.Clear();

            if (!string.IsNullOrEmpty(ActionData.BaseArgs.ParentNodeGuid))
            {
                xAction_Base parent = graphView.FindNodeView(ActionData.BaseArgs.ParentNodeGuid).ActionData;
                if (parent != null)
                {
                    VisualElement container = new VisualElement();
                    container.AddToClassList("list_container");
                    fold.Add(container);

                    // 高亮父节点
                    container.RegisterCallback<PointerEnterEvent>((evt) =>
                    {
                        xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                        Node node = wnd.xw_graphView.FindNode(parent.BaseArgs.guid);
                        if (node is xNode_Base n_base)
                        {
                            n_base.Highlight();
                        }
                    });
                    // 取消高亮父节点
                    container.RegisterCallback<PointerLeaveEvent>((evt) =>
                    {
                        xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                        Node node = wnd.xw_graphView.FindNode(parent.BaseArgs.guid);
                        if (node is xNode_Base n_base)
                        {
                            n_base.UnHighlight();
                        }
                    });

                    VisualElement container_title = new VisualElement();
                    container_title.AddToClassList("list_titlebg");
                    container.Add(container_title);

                    VisualElement container_icon = new VisualElement();
                    container_icon.AddToClassList("list_item_icon");
                    container_icon.style.backgroundImage = parent.BaseArgs.NodeIcon == null ? util_XGraphEditorUtility.AssetLoad<Texture2D>(AssetDatabase.GUIDToAssetPath(parent.BaseArgs.icon)) : parent.BaseArgs.NodeIcon;
                    container_title.Add(container_icon);

                    util_XGraphInspectorGUI.GUI_Label(container_title, $"目标：{parent.identifyName}", new string[] { "labeltext", "list_item_title" });
                    util_XGraphInspectorGUI.GUI_Label(container_title, "行为", new string[] { "list_item_marktext" });
                    util_XGraphInspectorGUI.GUI_Label(container, $"<b>Guid：</b><color=#e1e1e1>{parent.BaseArgs.guid}</color>", new string[] { "list_item_label" });
                    util_XGraphInspectorGUI.GUI_Label(container, $"<b>行为类型：</b><color=#e1e1e1>{parent.BaseArgs.actionNodeType}</color>", new string[] { "list_item_label" });
                    util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点类型：</b><color=#e1e1e1>{parent.BaseArgs.visualNodeType}</color>", new string[] { "list_item_label" });
                }
            }
            return fold;
        }
        /// <summary>
        /// 黑板变量组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public virtual Foldout ins_Folder_BlackBoardVariable(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, $"黑板变量（{ActionData.BaseArgs.VariableDatas.Count}）", "basetype-var", new string[] { "foldout" });
            fold.Clear();
            for (int i = 0; i < ActionData.BaseArgs.VariableDatas.Count; i++)
            {
                Binder_Varialble con = ActionData.BaseArgs.VariableDatas[i];

                Variable vare = con.variable;

                string var_value = "";

                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);
                // 高亮黑板变量节点
                container.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(con.VariableNodeGuid);
                    if (node is xNode_Variable n_vare)
                    {
                        n_vare.Highlight();
                    }
                });
                // 取消高亮黑板变量节点
                container.RegisterCallback<PointerLeaveEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(con.VariableNodeGuid);
                    if (node is xNode_Variable n_vare)
                    {
                        n_vare.UnHighlight();
                    }
                });

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/variable.png");
                container_title.Add(container_icon);

                util_XGraphInspectorGUI.GUI_Label(container_title, $"变量：{vare.name}", new string[] { "labeltext", "list_item_title" });
                util_XGraphInspectorGUI.GUI_Label(container_title, vare.type.ToString(), new string[] { "list_item_marktext" });
                switch (vare.type)
                {
                    case xVariableType.String:
                        var_value = vare.GetValue<string>().ToString();
                        break;
                    case xVariableType.Float:
                        var_value = vare.GetValue<float>().ToString();
                        break;
                    case xVariableType.Int:
                        var_value = vare.GetValue<int>().ToString();
                        break;
                    case xVariableType.Bool:
                        var_value = vare.GetValue<bool>().ToString();
                        break;
                    case xVariableType.Vector2:
                        var_value = vare.GetValue<Vector2>().ToString();
                        break;
                    case xVariableType.Vector3:
                        var_value = vare.GetValue<Vector3>().ToString();
                        break;
                    case xVariableType.Vector4:
                        var_value = vare.GetValue<Vector4>().ToString();
                        break;
                    case xVariableType.Color:
                        var_value = vare.GetValue<Color>().ToString();
                        break;
                }
                util_XGraphInspectorGUI.GUI_Label(container, var_value.ToString(), new string[] { "list_item_themevalue" }).style.color = ActionData.BaseArgs.RootAsset.GraphviewGridBackgroundThemes.themecolor;

                util_XGraphInspectorGUI.GUI_Label(container, $"<b>端口：</b><color=#e1e1e1>{con.TargetPortName}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>说明：</b><color=#e1e1e1>{vare.description}</color>".ToString(), new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N：</b><color=#e1e1e1>{con.VariableNodeGuid}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-V：</b><color=#e1e1e1>{vare.guid}</color>", new string[] { "list_item_label" });

            }

            return fold;
        }
        /// <summary>
        /// 内部变量组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public virtual Foldout ins_Folder_InternalVariable(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, $"内部变量（{ActionData.BaseArgs.InternalVariableDatas.Count}）", "basetype-intvar", new string[] { "foldout" });
            fold.Clear();
            for (int i = 0; i < ActionData.BaseArgs.InternalVariableDatas.Count; i++)
            {
                Binder_Varialble con = ActionData.BaseArgs.InternalVariableDatas[i];

                Variable vare = con.variable;

                string var_value = "";


                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);

                // 高亮内部变量节点
                container.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(con.VariableNodeGuid);
                    if (node is xNode_Base n_base)
                    {
                        n_base.Highlight();
                    }
                });
                // 取消高亮内部变量节点
                container.RegisterCallback<PointerLeaveEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(con.VariableNodeGuid);
                    if (node is xNode_Base n_base)
                    {
                        n_base.UnHighlight();
                    }
                });

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/variable.png");
                container_title.Add(container_icon);

                util_XGraphInspectorGUI.GUI_Label(container_title, $"变量：{vare.name}", new string[] { "labeltext", "list_item_title" });
                util_XGraphInspectorGUI.GUI_Label(container_title, vare.type.ToString(), new string[] { "list_item_marktext" });
                switch (vare.type)
                {
                    case xVariableType.String:
                        var_value = vare.GetValue<string>().ToString();
                        break;
                    case xVariableType.Float:
                        var_value = vare.GetValue<float>().ToString();
                        break;
                    case xVariableType.Int:
                        var_value = vare.GetValue<int>().ToString();
                        break;
                    case xVariableType.Bool:
                        var_value = vare.GetValue<bool>().ToString();
                        break;
                    case xVariableType.Vector2:
                        var_value = vare.GetValue<Vector2>().ToString();
                        break;
                    case xVariableType.Vector3:
                        var_value = vare.GetValue<Vector3>().ToString();
                        break;
                    case xVariableType.Vector4:
                        var_value = vare.GetValue<Vector4>().ToString();
                        break;
                    case xVariableType.Color:
                        var_value = vare.GetValue<Color>().ToString();
                        break;
                }
                util_XGraphInspectorGUI.GUI_Label(container, var_value.ToString(), new string[] { "list_item_themevalue" }).style.color = ActionData.BaseArgs.RootAsset.GraphviewGridBackgroundThemes.themecolor;

                util_XGraphInspectorGUI.GUI_Label(container, $"<b>端口：</b><color=#e1e1e1>{con.TargetPortName}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>说明：</b><color=#e1e1e1>{vare.description}</color>".ToString(), new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-N：</b><color=#e1e1e1>{con.VariableNodeGuid}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>GUID-V：</b><color=#e1e1e1>{vare.guid}</color>", new string[] { "list_item_label" });
            }

            return fold;
        }
        /// <summary>
        /// 自定义派生类组件折叠容器
        /// </summary>
        /// <param name="fold"></param>
        public virtual Foldout ins_Folder_Extensions(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, "扩展", "extension", new string[] { "foldout" });
            fold.Clear();
            return fold;
        }
        /// <summary>
        /// 子行为组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public virtual Foldout ins_Folder_ChildActions(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, "子行为", "childs", new string[] { "foldout" });
            fold.Clear();
            return fold;
        }
        /// <summary>
        /// 属性记录折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public virtual Foldout ins_Folder_BindedPropertys(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, "属性绑定", "binded_propertys", new string[] { "foldout" });
            fold.text = $"{fold.text}（{ActionData.BaseArgs.binded_propertys.Count}）";
            fold.Clear();

            for (int i = 0; i < ActionData.BaseArgs.binded_propertys.Count; i++)
            {
                Binder_Property prop = ActionData.BaseArgs.binded_propertys[i];

                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);

                // 高亮属性节点
                container.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(prop.Property_GUID);
                    if (node is xNode_Property n_prop)
                    {
                        n_prop.Highlight();
                    }
                });
                // 取消高亮属性节点
                container.RegisterCallback<PointerLeaveEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(prop.Property_GUID);
                    if (node is xNode_Property n_prop)
                    {
                        n_prop.UnHighlight();
                    }
                });

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/property.png");
                container_title.Add(container_icon);

                string type = prop.Property_PortType.ToString();
                string result_type = type.Substring(type.IndexOf("XGraph.") + "XGraph.".Length).Split(new char[] { '_' })[1];
                util_XGraphInspectorGUI.GUI_Label(container_title, result_type, new string[] { "list_item_marktext" });
                util_XGraphInspectorGUI.GUI_Label(container_title, $"{prop.Property_PortName}   >>>   {prop.Action_PortName}", new string[] { "labeltext", "list_item_title" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>属性节点：</b><color=#e1e1e1>{prop.Property_NodeName}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>目标属性：</b><color=#e1e1e1>{prop.Property_PortName}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>Guid：</b><color=#e1e1e1>{prop.Property_GUID}</color>", new string[] { "list_item_label" });
            }

            return fold;
        }
        /// <summary>
        /// 属性节点的属性项折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public virtual Foldout ins_Folder_Propertys(VisualElement root)
        {
            Foldout fold = util_XGraphInspectorGUI.GUI_Foldout(root, "属性参数", "propertys", new string[] { "foldout" });
            fold.Clear();

            return fold;
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 是否存在变量绑定
        /// </summary>
        /// <returns></returns>
        internal bool isVariableBinded()
        {
            return ActionData.BaseArgs.VariableDatas.Count > 0 || ActionData.BaseArgs.InternalVariableDatas.Count > 0 || ActionData.BaseArgs.binded_propertys.Count > 0;
        }
        /// <summary>
        /// 检查是否存在目标名称的变量或属性绑定
        /// </summary>
        /// <param name="bindname">绑定的字段名称（如"名称"、"激活"）</param>
        /// <returns></returns>
        internal bool isVariableBinded(string bindname)
        {
            // 检查黑板变量
            foreach (var variableData in ActionData.BaseArgs.VariableDatas)
            {
                if (variableData.TargetPortName == bindname)
                    return true;
            }

            // 检查内部变量
            foreach (var internalVariableData in ActionData.BaseArgs.InternalVariableDatas)
            {
                if (internalVariableData.TargetPortName == bindname)
                    return true;
            }

            // 检查属性绑定
            foreach (var property in ActionData.BaseArgs.binded_propertys)
            {
                if (property.Action_PortName == bindname)
                    return true;
            }

            return false;
        }
        #endregion
    }
}