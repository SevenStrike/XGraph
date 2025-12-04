namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Variable : Node
    {
        #region 组件
        /// <summary>
        /// GraphView组件
        /// </summary>
        public xg_GraphView graphView;
        #endregion

        #region 回调
        /// <summary>
        /// 当选中节点时的委托事件
        /// </summary>
        public Action<xNode_Variable> OnSelectedNode;
        /// <summary>
        /// 当选中节点时的委托事件
        /// </summary>
        public Action<xNode_Variable> OnUnSelectedNode;
        /// <summary>
        /// 当移动节点位置时的委托事件
        /// </summary>
        public Action<Vector2> On_Node_Moved;
        /// <summary>
        /// 当改变节点尺寸时的委托事件
        /// </summary>
        public Action<Vector2> On_Node_SizeChanged;
        #endregion

        #region 参数
        /// <summary>
        /// 变量节点的最后一次尺寸
        /// </summary>
        private Vector2 m_LastSize;
        #endregion

        #region 控件
        public Label TitleIcon;
        public Label TitleLabel;
        private VisualElement Highlighter;
        #endregion

        /// <summary>
        /// 输出端口
        /// </summary>
        public xGraph_NodePort OutputPort;

        /// <summary>
        /// 节点标题
        /// </summary>
        public string nodeTitle { get; set; }

        /// <summary>
        /// 节点携带的数据
        /// </summary>
        public xVariableData VariableData { get; set; }

        /// <summary>
        /// 初始化节点 - ActionVariableData
        /// </summary>
        /// <param name="graphView"></param>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        public virtual void Initialize(xg_GraphView graphView, Vector2 pos = default, xVariableData data = null)
        {
            // 指定GraphView 组件
            this.graphView = graphView;

            // 设置节点的容器样式
            util_XGraphEditorUtility.ElementStyle_Add(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_VariableNode.uss");

            SetContainersStyle();

            // 携带数据
            if (data != null)
                VariableData = data;

            #region 基础参数设置
            this.viewDataKey = data != null ? data.guid_n : "";
            // 设置节点标题
            this.title = this.nodeTitle = data.name;
            #endregion

            #region 端口设置
            Port_Set();
            #endregion

            // 设置节点的生成位置
            SetPosition(new Rect(pos, Vector2.zero));

            // 监听尺寸变化事件
            RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

            this.name = VariableData.name;
        }

        /// <summary>
        /// 当拖动节点位置时，将位置数据传递给对应的目标数据节点位置变量
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change Variable Position");
            base.SetPosition(newPos);

            if (VariableData != null)
            {
                VariableData.position.x = newPos.xMin;
                VariableData.position.y = newPos.yMin;

                if (On_Node_Moved != null)
                    On_Node_Moved(VariableData.position);
            }
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

        #region 数据流效果控制
        private void SetConnectedEdgesFlow(bool enable)
        {
            var edges = new HashSet<util_AnimatedEdge>();

            foreach (var con in OutputPort.Port.connections)
            {
                if (con is util_AnimatedEdge aedge)
                {
                    edges.Add(aedge);
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

            // 数据流效果  -  开启
            if (graphView.gv_GraphWindow.DisplayNodeFlow)
                SetConnectedEdgesFlow(true);

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

            // 数据流效果  -  关闭
            SetConnectedEdgesFlow(false);

            // 调用回调事件
            if (OnUnSelectedNode != null)
            {
                OnUnSelectedNode.Invoke(this);
            }

            On_Node_Moved = null;
            On_Node_SizeChanged = null;

        }
        /// <summary>
        /// 当节点尺寸发生改变时
        /// </summary>
        /// <param name="evt"></param>
        private void OnSizeChanged(GeometryChangedEvent evt)
        {
            Undo.RecordObject(graphView.ActionTreeAsset, "Change Variable Size");

            Vector2 newSize = new Vector2(evt.newRect.width, evt.newRect.height);

            if (Vector2.Distance(m_LastSize, newSize) > 1f) // 1像素阈值
            {
                m_LastSize = newSize;

                VariableData.size = newSize;

                if (On_Node_SizeChanged != null)
                    On_Node_SizeChanged(VariableData.size);
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

        #region 端口设置
        /// <summary>
        /// 设置变量输出端口
        /// </summary>
        /// <returns></returns>
        public virtual xNode_Variable Port_Set()
        {
            // 动态获取变量类型
            string asm = "Assembly-CSharp";
            Type var_type = Type.GetType($"SevenStrikeModules.XGraph.Variable_{VariableData.type}, {asm}");

            OutputPort = new xGraph_NodePort("out", var_type, Port.Capacity.Multi);
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
            //Port port = Port.Create<util_AnimatedEdge>(orientation, direction, capacity, type);
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
        public virtual xNode_Variable Draw()
        {
            // 绘制主容器
            Draw_Main();

            // 绘制标题容器
            Draw_Title();

            // 绘制输入节点容器
            Draw_Input();

            // 绘制输出节点容器
            Draw_Output();
            return this;
        }

        public virtual void Draw_Input()
        {
            VisualElement title_element = new VisualElement();
            title_element.AddToClassList("Title_Group");

            // 节点标题图标
            TitleIcon = new Label("");
            TitleIcon.AddToClassList("Title_Icon");
            TitleIcon.style.backgroundColor = GetVariableThemeColor(VariableData.type);

            // 用于显示节点名称
            TitleLabel = new Label(VariableData.name);
            TitleLabel.AddToClassList("Title_Label");

            title_element.Add(TitleIcon);
            title_element.Add(TitleLabel);
            AppendElement(xNodeContainerType.InputContainer, title_element);
        }

        /// <summary>
        /// 绘制输出节点容器
        /// </summary>
        public virtual void Draw_Output()
        {
            // 绘制端口 - 输出
            OutputPort.Port = Port_Create(OutputPort.Name, Orientation.Horizontal, Direction.Output, OutputPort.Capacity, OutputPort.Type, Color.white * 0.7f);
            OutputPort.PortDonut = OutputPort.Port.Q<VisualElement>("connector");
            SetPortStyle(OutputPort);
            AppendElement(xNodeContainerType.OutputContainer, OutputPort.Port);
        }

        /// <summary>
        /// 绘制标题容器
        /// </summary>
        public virtual void Draw_Title()
        {

        }

        /// <summary>
        /// 绘制主容器
        /// </summary>
        public virtual void Draw_Main()
        {
            mainContainer.style.overflow = new StyleEnum<Overflow>(Overflow.Visible);

            Highlighter = new VisualElement();
            Highlighter.pickingMode = PickingMode.Ignore;
            Highlighter.name = "HighlighterVisualler";
            Highlighter.AddToClassList("Variable_highlighter");
            util_XGraphEditorUtility.Element_BackgroundColor_Set(Highlighter, graphView.gv_GraphWindow.xw_BlackBoardView.GetVariableThemeColor(VariableData.type));
            UnHighlight();
            AppendElement(xNodeContainerType.MainContainer, Highlighter);
            Highlighter.SendToBack();
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
            Undo.RecordObject(graphView.ActionTreeAsset, $"Set NodeTransparentMode - {state}");
            VariableData.transparent = state;
            CheckTransparentDisplay(state);
        }
        #endregion        

        #region 辅助
        /// <summary>
        /// 修改节点标题显示名称
        /// </summary>
        /// <param name="name"></param>
        public void SetNode_TitleLabel(string name)
        {
            VariableData.name = name;
            util_XGraphEditorUtility.Element_Label_ValueSet(TitleLabel, VariableData.name);
        }
        /// <summary>
        /// 修改节点解释文字
        /// </summary>
        /// <param name="des"></param>
        public void SetNode_Description(string des)
        {
            VariableData.description = des;
            //util_XGraphEditorUtility.Element_Label_ValueSet(TitleLabel, variable.name);
        }
        /// <summary>
        /// 设置端口样式
        /// </summary>
        /// <param name="nodeport"></param>
        public void SetPortStyle(xGraph_NodePort nodeport)
        {
            nodeport.Port.Q<VisualElement>(className: "port").AddToClassList("PortOut");
            nodeport.Port.Q<Label>().AddToClassList("PortTextOut");
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
        /// 获取黑板变量的专属主题色
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public Color GetVariableThemeColor(xVariableType type)
        {
            Color node_color = Color.white;
            foreach (var theme in graphView.gv_GraphWindow.xw_BlackBoardView.VariableThemes.VariableThemes)
            {
                if (theme.type == type.ToString())
                {
                    node_color = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            }
            return node_color;
        }
        /// <summary>
        /// 高亮显示节点
        /// </summary>        
        public void Highlight()
        {
            util_XGraphEditorUtility.Element_Opacity_Set(Highlighter, 0.7f);
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