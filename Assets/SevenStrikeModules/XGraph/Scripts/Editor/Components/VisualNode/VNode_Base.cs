namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UI;
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
        /// 构造器
        /// </summary>
        /// <param root_title="catergory_title"></param>
        /// <param root_title="type"></param>
        /// <param root_title="capacity"></param>
        public xGraph_NodePort(string name, Type type, Port.Capacity capacity)
        {
            Name = name;
            Type = type;
            Capacity = capacity;
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
        /// 视觉节点图标
        /// </summary>
        public Label IconLabel;
        /// <summary>
        /// 指定节点图标
        /// </summary>
        public string icon;

        #region 端口
        /// <summary>
        /// 输入端口
        /// </summary>
        public xGraph_NodePort Port_Input = new xGraph_NodePort();
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
        /// 节点携带的数据
        /// </summary>
        public ActionNode_Base ActionNode { get; set; }

        /// <summary>
        /// 初始化节点 - ActionNode_Base
        /// </summary>
        /// <param name="graphView"></param>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        public virtual void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            // 指定GraphView 组件
            this.graphView = graphView;

            extensionContainer.AddToClassList("ExtensionContainer");

            // 携带数据
            if (data != null)
                ActionNode = data;

            #region 基础参数设置
            this.icon = data.icon;
            this.viewDataKey = data != null ? data.guid : "";
            // 设置节点标题
            this.title = this.nodeTitle = data != null ? data.identifyName : "";
            #endregion

            // 设置节点的生成位置
            SetPosition(new Rect(pos, Vector2.zero));
        }

        /// <summary>
        /// 当拖动节点位置时，将位置数据传递给对应的目标数据节点位置变量
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            Undo.RecordObject(this.ActionNode, "SetPosition VisualNode");
            base.SetPosition(newPos);
            if (ActionNode != null)
            {
                ActionNode.nodeGraphPosition.x = newPos.xMin;
                ActionNode.nodeGraphPosition.y = newPos.yMin;
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

        public virtual VNode_Base SetPort_Input(xGraph_NodePort portInfo)
        {
            Port_Input = portInfo;
            return this;
        }
        public virtual VNode_Base SetPort_Output(List<xGraph_NodePort> portInfos)
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
        public virtual Port CreatePort(string name = "新端口", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single, Type type = null, Color nodeThemeColor = default)
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

        /// <summary>
        /// 绘制输出节点容器
        /// </summary>
        public virtual void Draw_Output()
        {
            Port_Outputs.ForEach(x =>
            {
                // 绘制端口 - 输出
                x.Port = CreatePort(x.Name, Orientation.Horizontal, Direction.Output, x.Capacity, x.Type, ActionNode.themeSolution == "M 默认" ? Color.white * 0.7f : ActionNode.themeColor);

                x.Port.Q<VisualElement>(className: "port").AddToClassList("Port_Out");
                x.Port.Q<Label>().AddToClassList("PortText_Out");

                AppendElement(GraphNodeContainerType.OutputContainer, x.Port);
            });
        }

        /// <summary>
        /// 绘制输入节点容器
        /// </summary>
        public virtual void Draw_Input()
        {
            Port_Input.Port = CreatePort(Port_Input.Name, Orientation.Horizontal, Direction.Input, Port_Input.Capacity, Port_Input.Type, ActionNode.themeSolution == "M 默认" ? Color.white * 0.7f : ActionNode.themeColor);

            // 样式指定
            Port_Input.Port.Q<VisualElement>(className: "port").AddToClassList("Port_In");
            Port_Input.Port.Q<Label>().AddToClassList("PortText_In");

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
            IconLabel = new Label("");
            IconLabel.AddToClassList("Title_Icon");
            IconLabel.style.backgroundImage = util_EditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{this.icon}.png");
            // 应用配置文件的颜色到节点的标识颜色
            graphView.ThemesList.Node.ForEach(colorData =>
            {
                if (colorData.solution == ActionNode.themeSolution)
                {
                    IconLabel.style.unityBackgroundImageTintColor = ActionNode.themeSolution == "M 默认" ? Color.white : ActionNode.themeColor;
                }
            });

            TextField input_title = new TextField()
            {
                multiline = false,
            };
            input_title.value = "节点标题";
            input_title.AddToClassList("Title_TextField");

            VisualElement input = input_title.Q<VisualElement>("unity-text-input");
            input.AddToClassList("Title_TextInput");

            TextElement textelement = input.Q<TextElement>();
            textelement.AddToClassList("Title_TextElement");

            VisualElement element = titleContainer.Q<VisualElement>("title-button-container");

            // 清空容器后重新按顺序添加
            titleContainer.Clear();
            AppendElement(GraphNodeContainerType.TitleContainer, IconLabel);
            AppendElement(GraphNodeContainerType.TitleContainer, input_title);
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
            titleContainer.style.borderBottomColor = ActionNode.themeColor;
        }
        /// <summary>
        /// 刷新节点配色
        /// </summary>
        public void UpdateMarkColor()
        {
            titleContainer.style.borderBottomColor = ActionNode.themeColor;
            titleContainer.style.borderBottomWidth = 1;
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
        protected void SetNodeStyle(string StyleName)
        {
            // 读取uss样式
            var uss_node = util_EditorUtility.AssetLoad<StyleSheet>($"{util_Dashboard.GetPath_GUI_Uss()}{StyleName}.uss");
            styleSheets.Add(uss_node);
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