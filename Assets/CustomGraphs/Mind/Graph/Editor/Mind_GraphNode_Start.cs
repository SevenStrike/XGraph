namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class Mind_GraphNode_Start : VNode_Base
    {
        /// <summary>
        /// 视觉节点贴图尺寸控制图标
        /// </summary>
        public VisualElement ResizerIcon;
        /// <summary>
        /// 视觉节点贴图尺寸控制组件
        /// </summary>
        public VisualElement Resizer;
        /// <summary>
        /// 导图节点的行为节点数据
        /// </summary>
        private Mind_ActionNode_Start MindActionNode;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            // 指定可调整大小
            capabilities |= Capabilities.Resizable;

            // 重要步骤：从基础行为节点转型到思维导图行为节点，只有这一步才能让脚本使用基础行为节点没有的参数
            MindActionNode = data as Mind_ActionNode_Start;

            style.width = MindActionNode.nodeGraphSize.x;
            style.height = MindActionNode.nodeGraphSize.y;

            // 设置节点的容器样式
            util_XGraphEditorUtility.ElementStyle_Add(this, $"Assets/CustomGraphs/Mind/Uss/uss_MindNode.uss");


            AddToClassList("NodeCointainer");

            #region 端口设置
            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            port_out.Add(new xGraph_NodePort("out", typeof(bool), Port.Capacity.Multi));
            SetPort_Output(port_out);
            #endregion           
        }

        #region 节点绘制
        public override VNode_Base Draw()
        {
            // 绘制主容器
            Draw_Main();

            // 绘制标题容器
            Draw_Title();

            // 绘制标题按钮容器
            Draw_TitleButton();

            // 绘制顶部容器
            Draw_Top();

            //// 绘制输入节点容器
            //Draw_Input();

            // 绘制输出节点容器
            Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }

        public override void Draw_Extension()
        {
            Label subtitle = new Label(MindActionNode.Message);
            subtitle.AddToClassList("mind_label");
            subtitle.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    Debug.Log("222");
                    //VisualElementDisplay(stickContentlabel, false);
                    //VisualElementDisplay(stickContentInput, true);

                    //EditorApplication.delayCall += () =>
                    //{
                    //    stickContentInput.Focus();
                    //};
                    evt.StopPropagation();
                }
            });
            AppendElement(GraphNodeContainerType.ExtensionContainer, subtitle);
        }

        public override void Draw_Main()
        {
            base.Draw_Main();

            // 拖拽尺寸控件图标
            ResizerIcon = this.Q<VisualElement>(className: "resizer-icon");
            ResizerIcon.pickingMode = PickingMode.Ignore;

            // 拖拽尺寸控件
            Resizer = this.Q<VisualElement>(className: "resizer");
            Resizer.style.width = 30;
            Resizer.style.height = 30;
        }
        #endregion

        /// <summary>
        /// 节点被克隆时
        /// </summary>
        /// <param name="list"></param>
        public override void OnDuplicatedNode(List<DuplicateNodeData> list)
        {
            base.OnDuplicatedNode(list);

            list.ForEach(d =>
            {
                // 如果被克隆的节点是源于自身，就将 MindActionNode 的额外参数同步过去，使克隆的节点数据保持和克隆源一致
                if (d.SourceNodeGuid == ActionNode.guid)
                {
                    Mind_GraphNode_Start node = d.DuplicatedNode as Mind_GraphNode_Start;
                    if (node != null)
                    {
                        node.MindActionNode.Message = MindActionNode.Message;
                        node.MindActionNode.Names = MindActionNode.Names;
                    }
                }
            });
        }
    }
}