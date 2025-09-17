namespace SevenStrikeModules.XGraph
{
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;

    public partial class xg_GraphView
    {
        /// <summary>
        /// 当GraphView组件发生改变时
        /// </summary>
        /// <param assetName="graphViewChange"></param>
        /// <returns></returns>
        public GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            On_RemovedElement(graphViewChange);

            On_CreateEdge(graphViewChange);

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();

            return graphViewChange;
        }

        #region 创建连线时
        /// <summary>
        /// 当有连线被创建时
        /// </summary>
        /// <param assetName="graphViewChange"></param>
        private void On_CreateEdge(GraphViewChange graphViewChange)
        {
            // 当有连线被创建时
            if (graphViewChange.edgesToCreate != null)
            {
                // 如果创建了连线，edge 为连线
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    CreateEdge(edge.output.node, edge.input.node, edge);
                });
            }
        }
        /// <summary>
        /// 创建连线时
        /// </summary>
        /// <param name="n_parent"></param>
        /// <param name="n_child"></param>
        /// <param name="edge"></param>
        private void CreateEdge(Node n_parent, Node n_child, Edge edge)
        {
            VNode_Base vbs_parent = n_parent as VNode_Base;
            VNode_Base vbs_child = n_child as VNode_Base;

            if (vbs_parent != null && vbs_child != null)
            {
                // 将 "n_child" 放到 "n_parent" 的child成员变量中，这样就可以让父级数据节点知道自己和哪个子级数据节点相连接
                ActionTreeAsset.ChildNode_Add(vbs_parent.ActionData, vbs_child.ActionData);
            }

            VNode_Relay relay_child = edge.input.node as VNode_Relay;
            if (relay_child != null)
            {
                relay_child.Connected();
            }
        }
        #endregion

        #region 移除元素时
        /// <summary>
        /// 当有节点被移除时
        /// </summary>
        /// <param assetName="graphViewChange"></param>
        private void On_RemovedElement(GraphViewChange graphViewChange)
        {
            // 当有节点被移除时
            if (graphViewChange.elementsToRemove != null)
            {
                // 当有元素被移除的时候
                graphViewChange.elementsToRemove.ForEach(element =>
                {
                    Removed_Node(element);
                    Removed_Stick(element);
                    Removed_Decal(element);
                    Removed_Variable(element);
                    Removed_Edge(element);
                    Removed_Group(element);
                });
            }
        }
        /// <summary>
        /// 当移除编组时
        /// </summary>
        /// <param name="element"></param>
        private void Removed_Group(GraphElement element)
        {
            Group group = element as Group;
            if (group != null)
            {
                Undo.RecordObject(ActionTreeAsset, "Remove Group");

                // 查找对应的 NodeGroupData
                ActionGroupData groupData = ActionTreeAsset.Groups.FirstOrDefault(g => g.group == group);
                if (groupData != null)
                {
                    // 从 Groups 中移除
                    ActionTreeAsset.NodeGroup_Remove(groupData);
                }

                // 清理状态跟踪数据
                if (CurrentCreatedGroups.ContainsKey(group))
                {
                    CurrentCreatedGroups.Remove(group);
                }
            }
        }
        /// <summary>
        /// 当移除连线时
        /// </summary>
        /// <param name="element"></param>
        private void Removed_Edge(GraphElement element)
        {
            Edge edge = element as Edge;
            if (edge != null)
            {
                VNode_Base node_parent = edge.output.node as VNode_Base;
                VNode_Base node_child = edge.input.node as VNode_Base;

                // 连线的起点是 v-Base 终点是 v-Base
                if (node_parent != null && node_child != null)
                {
                    // 将 "n_child" 从 "n_parent" 的 "port" 数据节点变量中移除
                    ActionTreeAsset.ChildNode_Remove(node_parent.ActionData, node_child.ActionData);
                }

                VNode_Relay relay_child = edge.input.node as VNode_Relay;
                if (relay_child != null)
                {
                    Undo.RecordObject(relay_child.ActionData, "Remove RelayConnector");
                    relay_child.Disconnected();
                }
            }
        }
        /// <summary>
        /// 当移除便签时
        /// </summary>
        /// <param name="element"></param>
        private void Removed_Stick(GraphElement element)
        {
            VNode_Stick stickview = element as VNode_Stick;
            if (stickview != null)
            {
                Undo.RecordObject(ActionTreeAsset, "Remove StickNote");
                ActionTreeAsset.StickNote_Remove(stickview.StickData);
            }
        }
        /// <summary>
        /// 当移除贴图时
        /// </summary>
        /// <param name="element"></param>
        private void Removed_Decal(GraphElement element)
        {
            VNode_Decal decalview = element as VNode_Decal;
            if (decalview != null)
            {
                Undo.RecordObject(ActionTreeAsset, "Remove Decal");
                ActionTreeAsset.Decal_Remove(decalview.DecalData);
            }
        }
        /// <summary>
        /// 当移除变量时
        /// </summary>
        /// <param name="element"></param>
        private void Removed_Variable(GraphElement element)
        {
            VNode_Variable vare = element as VNode_Variable;
            if (vare != null)
            {
                Undo.RecordObject(ActionTreeAsset, "Remove Variable");
                ActionTreeAsset.Variable_Remove(vare.VariableData);
            }
        }
        /// <summary>
        /// 当移除节点时
        /// </summary>
        /// <param name="element"></param>
        private void Removed_Node(GraphElement element)
        {
            #region 延展节点
            VNode_Base nodeview = element as VNode_Base;
            if (nodeview != null)
            {
                // 从根节点中移除数据节点
                ActionTreeAsset.Remove(nodeview.ActionData);
            }
            #endregion          
        }
        #endregion
    }
}