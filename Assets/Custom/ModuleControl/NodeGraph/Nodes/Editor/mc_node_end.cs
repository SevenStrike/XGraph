namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class mc_node_end : xNode_Base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口设置
            List<xGraph_NodePort> port_in = new List<xGraph_NodePort>();
            // 加入行为端口
            port_in.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Single));
            InputPort_Set(port_in);
            #endregion
        }

        #region 节点绘制
        public override xNode_Base Draw()
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

            //// 绘制输出节点容器
            //Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }
        #endregion

        #region 重写
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();
        }
        /// <summary>
        /// 当克隆节点时
        /// </summary>
        /// <param name="list"></param>
        public override void On_Nodes_Duplicated(List<DuplicateNodeData> list)
        {
            base.On_Nodes_Duplicated(list);

            foreach (var node in list)
            {
                if (node.DuplicatedNode is mc_node_end cur)
                {
                    // 找到克隆的父物体行为节点
                    mc_node_end source = graphView.FindNode(node.SourceNodeGuid) as mc_node_end;

                    // 调用行为数据脚本中的 On_Node_Duplicated 事件以便于行为数据Editor界面下的控件获取克隆父物体的特定变量数据
                    if (cur.ActionData.On_Node_Duplicated != null)
                        cur.ActionData.On_Node_Duplicated(cur.ActionData, source.ActionData);
                }
            }
        }
        /// <summary>
        /// 当节点重建时
        /// </summary>
        public override void On_Node_Restructure()
        {
            base.On_Node_Restructure();
        }
        /// <summary>
        /// 当节点连线时
        /// </summary>
        /// <param name="edge"></param>
        public override void On_Node_CreateEdge(Edge edge)
        {
            base.On_Node_CreateEdge(edge);
        }
        /// <summary>
        /// 当节点移除连线时
        /// </summary>
        /// <param name="edge"></param>
        public override void On_Node_RemovedEdge(Edge edge)
        {
            base.On_Node_RemovedEdge(edge);
        }
        /// <summary>
        /// 当节点头像改变时
        /// </summary>
        /// <param name="tex"></param>
        public override void On_Node_AvatarChanged(Texture2D tex)
        {
            base.On_Node_AvatarChanged(tex);
        }
        /// <summary>
        /// 当节点执行模式改变时
        /// </summary>
        /// <param name="state"></param>
        public override void On_Node_ConcurrentChanged(bool state)
        {
            base.On_Node_ConcurrentChanged(state);
        }
        /// <summary>
        /// 当改变节点图标时
        /// </summary>
        /// <param name="tex"></param>
        public override void On_Node_IconChanged(Texture2D tex)
        {
            base.On_Node_IconChanged(tex);
        }
        /// <summary>
        /// 当改变节点颜色主题时
        /// </summary>
        public override void On_Node_ThemeColorChanged()
        {
            base.On_Node_ThemeColorChanged();
        }
        /// <summary>
        /// 当改变节点通透模式时
        /// </summary>
        /// <param name="state"></param>
        public override void On_Node_TransparentChanged(bool state)
        {
            base.On_Node_TransparentChanged(state);
        }
        /// <summary>
        /// 当改变节点尺寸时
        /// </summary>
        /// <param name="evt"></param>
        public override void OnSizeChanged(GeometryChangedEvent evt)
        {
            base.OnSizeChanged(evt);
        }
        /// <summary>
        /// 当选中节点时
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
        }
        /// <summary>
        /// 当取消选中节点时
        /// </summary>
        public override void OnUnselected()
        {
            base.OnUnselected();
        }
        #endregion
    }
}