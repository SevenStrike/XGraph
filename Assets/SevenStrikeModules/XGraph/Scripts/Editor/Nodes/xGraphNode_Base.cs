namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xGraph_NodePort
    {
        /// <summary>
        /// 端口
        /// </summary>
        public Port Port;
        /// <summary>
        /// 端口名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 端口类型
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// 连线方式
        /// </summary>
        public Port.Capacity Capacity { get; set; }
        /// <summary>
        /// 端口方向
        /// </summary>
        public Direction Direction { get; set; }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param root_title="catergory_title"></param>
        /// <param root_title="type"></param>
        /// <param root_title="capacity"></param>
        public xGraph_NodePort(string name, Type type, Direction direction, Port.Capacity capacity)
        {
            Name = name;
            Type = type;
            Capacity = capacity;
            Direction = direction;
        }

        public xGraph_NodePort()
        {

        }
    }

    /// <summary>
    /// GraphView 容器类型
    /// </summary>
    public enum GraphNodeContainerType
    {
        /// <summary>
        /// 主容器
        /// </summary>
        MainContainer = 0,
        /// <summary>
        /// 标题
        /// </summary>
        TitleContainer = 1,
        /// <summary>
        /// 标题按钮
        /// </summary>
        TitleButtonContainer = 2,
        /// <summary>
        /// 顶部容器
        /// </summary>
        TopContainer = 3,
        /// <summary>
        /// 输入端口容器
        /// </summary>
        InputContainer = 4,
        /// <summary>
        /// 输出端口容器
        /// </summary>
        OutputContainer = 5,
        /// <summary>
        /// 扩展容器
        /// </summary>
        ExtensionContainer = 6,
    }

    public class xGraphNode_Base : Node
    {
        /// <summary>
        /// GraphView组件
        /// </summary>
        public xg_GraphView graphView;
        /// <summary>
        /// 当选中节点时的委托事件
        /// </summary>
        public Action<xGraphNode_Base> OnSelectedNode;
        /// <summary>
        /// 当选中节点时的委托事件
        /// </summary>
        public Action<xGraphNode_Base> OnUnSelectedNode;
        /// <summary>
        /// 指定节点图标
        /// </summary>
        public string icon;
        /// <summary>
        /// 节点类型
        /// </summary>
        public xg_GraphViewNode nodeType { get; set; } = xg_GraphViewNode.None;

        #region 端口
        /// <summary>
        /// 输入端口
        /// </summary>
        public xGraph_NodePort Port_Input = new xGraph_NodePort();
        /// <summary>
        /// 输出端口
        /// </summary>
        public xGraph_NodePort Port_Output = new xGraph_NodePort();
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
        /// 节点携带的数据
        /// </summary>
        public ActionTree_Node_Base ActionTreeNode { get; set; }

        /// <summary>
        /// 初始化节点 - ActionTree_Node_Base
        /// </summary>
        /// <param h_name="graphView"></param>
        /// <param h_name="pos"></param>
        /// <param h_name="data"></param>
        public virtual void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionTree_Node_Base data = null)
        {
            // 指定GraphView 组件
            this.graphView = graphView;
            // 设置节点的容器样式
            SetContainersStyle("uss_Node");

            // 携带数据
            if (data != null)
                ActionTreeNode = data;

            #region 基础参数设置
            this.icon = data.icon;
            this.viewDataKey = data != null ? data.nodeGUID : "";
            // 设置节点标题
            this.title = this.nodeTitle = data != null ? data.nodeName : "";
            #endregion

            // 设置节点的生成位置
            SetPosition(new Rect(pos, Vector2.zero));
        }

        /// <summary>
        /// 当拖动节点位置时，将位置数据传递给对应的目标数据节点位置变量
        /// </summary>
        /// <param h_name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(this.ActionTreeNode, "Add Node");
            base.SetPosition(newPos);
            if (ActionTreeNode != null)
            {
                ActionTreeNode.nodeGraphPosition.x = newPos.xMin;
                ActionTreeNode.nodeGraphPosition.y = newPos.yMin;
            }
        }

        /// <summary>
        /// 当点击节点时
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

        public override void OnUnselected()
        {
            base.OnUnselected();

            // 调用回调事件
            if (OnUnSelectedNode != null)
            {
                OnUnSelectedNode.Invoke(this);
            }
        }

        #region 端口设置
        /// <summary>
        /// 根据传入的端口信息中的Direction值设置节点的端口参数信息
        /// </summary>
        /// <param h_name="portInfo"></param>
        /// <returns></returns>
        public virtual xGraphNode_Base SetPortInfo(xGraph_NodePort portInfo)
        {
            if (portInfo.Direction == Direction.Input)
                Port_Input = portInfo;
            else
                Port_Output = portInfo;

            return this;
        }
        #endregion

        #region 节点绘制
        /// <summary>
        /// 绘制节点
        /// </summary>
        public virtual xGraphNode_Base Draw()
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

        /// <summary>
        /// 绘制输出节点容器
        /// </summary>
        public virtual void Draw_Output()
        {
            // 绘制端口 - 输出
            Port_Output.Port = CreatePort(Port_Output.Name, Orientation.Horizontal, Port_Output.Direction, Port_Output.Capacity, Port_Output.Type);
            AppendElement(GraphNodeContainerType.OutputContainer, Port_Output.Port);
        }

        /// <summary>
        /// 绘制输入节点容器
        /// </summary>
        public virtual void Draw_Input()
        {
            // 绘制端口 - 输入
            Port_Input.Port = CreatePort(Port_Input.Name, Orientation.Horizontal, Port_Input.Direction, Port_Input.Capacity, Port_Input.Type);
            AppendElement(GraphNodeContainerType.InputContainer, Port_Input.Port);
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
            Label icon = new Label("");
            icon.AddToClassList("Title_Icon");
            icon.style.backgroundImage = util_EditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{this.icon}.png");
            icon.style.unityBackgroundImageTintColor = util_Dashboard.Theme_Primary;

            Label label = titleContainer.Q<Label>();
            label.AddToClassList("Title_Label");

            VisualElement element = titleContainer.Q<VisualElement>("title-button-container");

            // 清空容器后重新按顺序添加
            titleContainer.Clear();
            AppendElement(GraphNodeContainerType.TitleContainer, icon);
            AppendElement(GraphNodeContainerType.TitleContainer, label);
            AppendElement(GraphNodeContainerType.TitleContainer, element);

            //UpdateMarkColor();
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
        /// 设置节点配色
        /// </summary>
        public void SetMarkColor()
        {
            titleContainer.style.borderBottomColor = ActionTreeNode.nodeThemeColor;
        }
        /// <summary>
        /// 刷新节点配色
        /// </summary>
        public void UpdateMarkColor()
        {
            titleContainer.style.borderBottomColor = ActionTreeNode.nodeThemeColor;
            titleContainer.style.borderBottomWidth = 4;
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
            UpdateMarkColor();
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

        /// <summary>
        /// 创建端口
        /// </summary>
        /// <param h_name="solution"></param>
        /// <param h_name="orientation"></param>
        /// <param h_name="direction"></param>
        /// <param h_name="capacity"></param>
        /// <param h_name="type"></param>
        /// <returns></returns>
        protected Port CreatePort(string name = "新端口", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single, Type type = null)
        {
            Port port = InstantiatePort(orientation, direction, capacity, type);
            port.portName = name;
            return port;
        }
        #endregion
    }
}