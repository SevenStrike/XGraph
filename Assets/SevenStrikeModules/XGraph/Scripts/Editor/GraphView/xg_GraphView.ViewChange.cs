namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

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

            // 更新变量赋值数据
            ActionTreeAsset.Variables_Refresh();

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
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    CreateEdge(edge.output.node, edge.input.node, edge);
                }
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
            // 移除原有的连线
            if (edge != null)
            {
                RemoveElement(edge);
            }

            // 创建新的动画连线
            util_AnimatedEdge animatedEdge = null;

            #region 处理行为节点 / 行为节点 <---> 行为节点
            VNode_Base node_parent = n_parent as VNode_Base;
            VNode_Base node_child = n_child as VNode_Base;
            if (node_parent != null && node_child != null)
            {
                // 找到对应的端口
                Port outputPort = null;
                Port inputPort = null;

                // 查找输出端口
                foreach (var port in node_parent.Port_Outputs)
                {
                    if (port.Port == edge?.output)
                    {
                        outputPort = port.Port;
                        break;
                    }
                }

                // 查找输入端口
                foreach (var port in node_child.Port_Inputs)
                {
                    if (port.Port == edge?.input)
                    {
                        inputPort = port.Port;
                        break;
                    }
                }

                if (outputPort != null && inputPort != null)
                {
                    animatedEdge = outputPort.ConnectTo<util_AnimatedEdge>(inputPort);
                    AddElement(animatedEdge);
                }

                // 将 "n_child" 放到 "n_parent" 的child成员变量中，这样就可以让父级数据节点知道自己和哪个子级数据节点相连接
                ActionTreeAsset.ChildNode_Add(node_parent.ActionData, node_child.ActionData);
            }
            #endregion

            #region 处理行为节点 / 分支节点 <---> 行为节点
            VNode_Branch node_branch = n_parent as VNode_Branch;
            if (node_branch != null && node_child != null)
            {
                // 找到对应的端口
                Port outputPort = null;
                Port inputPort = null;

                // 查找输出端口
                foreach (var port in node_branch.Port_Outputs)
                {
                    if (port.Port == edge?.output && port.Name == edge?.output.portName)
                    {
                        outputPort = port.Port;
                        break;
                    }
                }

                // 查找输入端口
                foreach (var port in node_child.Port_Inputs)
                {
                    if (port.Port == edge?.input)
                    {
                        inputPort = port.Port;
                        break;
                    }
                }

                if (outputPort != null && inputPort != null)
                {
                    animatedEdge = outputPort.ConnectTo<util_AnimatedEdge>(inputPort);
                    AddElement(animatedEdge);
                }

                // 将 "n_child" 放到 "n_parent" 的相对应True和False的child成员变量中
                ActionTreeAsset.ChildNode_Add(node_branch.ActionData, node_child.ActionData, edge?.output.portName);
            }
            #endregion

            #region 黑板变量节点 <---> 内部变量节点
            VNode_Variable node_var = n_parent as VNode_Variable;
            VNode_Variable_Internal node_internalvar = node_child as VNode_Variable_Internal;

            if (node_var != null && node_internalvar != null)
            {
                Port outputPort = node_var.OutputPort?.Port;
                Port inputPort = null;

                // 查找输入端口
                foreach (var port in node_internalvar.Port_Inputs)
                {
                    if (port.Port == edge?.input && port.Name == edge?.input.portName)
                    {
                        inputPort = port.Port;
                        break;
                    }
                }

                if (outputPort != null && inputPort != null)
                {
                    animatedEdge = outputPort.ConnectTo<util_AnimatedEdge>(inputPort);
                    AddElement(animatedEdge);
                }

                Undo.RecordObject(node_child.ActionData, "Assigned Variable Guid");
                string portName = edge?.input.portName;
                // 加入行为节点数据中的变量列表中
                node_child.ActionData.VariableData_Bind(node_var.VariableData, portName);

                // 两个变量类型一致时
                if (node_internalvar.VariableData.variable.type == node_var.VariableData.type)
                {
                    node_internalvar.VariableNodeFieldValueUpdate();
                }
            }
            #endregion

            #region 内部变量节点 <---> 行为节点
            VNode_Variable_Internal node_varInternal = n_parent as VNode_Variable_Internal;
            if (node_varInternal != null && node_child != null)
            {
                Port outputPort = node_varInternal.Port_Outputs.FirstOrDefault()?.Port;
                Port inputPort = null;

                // 查找输入端口
                foreach (var port in node_child.Port_Inputs)
                {
                    if (port.Port == edge?.input && port.Name == edge?.input.portName)
                    {
                        inputPort = port.Port;
                        break;
                    }
                }

                if (outputPort != null && inputPort != null)
                {
                    animatedEdge = outputPort.ConnectTo<util_AnimatedEdge>(inputPort);
                    AddElement(animatedEdge);
                }

                Undo.RecordObject(node_child.ActionData, "Assigned InternalVariable Guid To InternalVariableAction");
                string portName = edge?.input.portName;
                node_child.ActionData.InternalVariableData_Bind(node_varInternal.VariableData, portName);
            }
            #endregion

            #region 黑板变量节点 <---> 行为节点
            VNode_Variable node_bb_var = n_parent as VNode_Variable;
            if (node_bb_var != null && node_child != null)
            {
                Port outputPort = node_bb_var.OutputPort?.Port;
                Port inputPort = null;

                // 查找输入端口
                foreach (var port in node_child.Port_Inputs)
                {
                    if (port.Port == edge?.input && port.Name == edge?.input.portName)
                    {
                        inputPort = port.Port;
                        break;
                    }
                }

                if (outputPort != null && inputPort != null)
                {
                    animatedEdge = outputPort.ConnectTo<util_AnimatedEdge>(inputPort);
                    AddElement(animatedEdge);
                }

                Undo.RecordObject(node_child.ActionData, "Assigned Variable Guid To BaseAction");
                string portName = edge?.input.portName;
                // 加入行为节点数据中的变量列表中
                node_child.ActionData.VariableData_Bind(node_bb_var.VariableData, portName);
            }
            #endregion    

            #region 特化处理延展节点
            VNode_Relay node_relay = edge?.input.node as VNode_Relay;
            if (node_relay != null)
            {
                node_relay.Connected();
            }
            #endregion
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
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    Removed_Node(element);
                    Removed_Stick(element);
                    Removed_Label(element);
                    Removed_Decal(element);
                    Removed_Variable(element);
                    Removed_InternalVariable(element);
                    Removed_Edge(element);
                    Removed_Group(element);
                }
                RecheckNodesIsExist();
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

                // 移除：起点是 ”行为节点“ 终点是 ”行为节点“ 的连线时
                if (node_parent != null && node_child != null)
                {
                    // 将 "n_child" 从 "n_parent" 的 "port" 数据节点变量中移除
                    ActionTreeAsset.ChildNode_Remove(node_parent.ActionData, node_child.ActionData);
                }

                // 移除：起点是 ”行为节点“ 终点是 ”延展节点“ 的连线时
                VNode_Relay relay_child = edge.input.node as VNode_Relay;
                if (relay_child != null)
                {
                    Undo.RecordObject(relay_child.ActionData, "Remove RelayConnector");
                    relay_child.Disconnected();
                }

                // 移除：起点是 “黑板变量节点” 终点是 带有 "Variable类型" 端口的 "行为节点" 的连线时
                VNode_Variable node_var = edge.output.node as VNode_Variable;
                if (node_var != null)
                {
                    string portName = edge.input.portName;
                    // 获取变量节点的变量类型
                    Type type = node_var.VariableData.variable.GetType();
                    // 拿到行为节点上的对应的变量类型的端口
                    Port port = node_child.GetVariablePort(type, portName);
                    // 断开端口与连线的连接
                    port.Disconnect(edge);

                    // 从行为节点解绑 ”黑板变量数据“
                    node_child.ActionData.VariableData_Unbind(node_var.VariableData.guid, portName);
                }

                // 移除：起点是“内部变量节点”  终点是 带有 "Variable类型" 端口的 "行为节点" 的连线时
                VNode_Variable_Internal node_var_internal = edge.output.node as VNode_Variable_Internal;
                if (node_var_internal != null)
                {
                    string portName = edge.input.portName;
                    // 获取变量节点的变量类型
                    Type type = node_var_internal.VariableData.variable.GetType();
                    // 拿到行为节点上的对应的变量类型的端口
                    Port port = node_child.GetVariablePort(type, portName);
                    // 断开端口与连线的连接
                    port.Disconnect(edge);

                    // 从行为节点解绑 ”内部变量节点数据“
                    node_child.ActionData.InternalVariableData_Unbind(node_var_internal.VariableData.guid, portName);
                }

                // 移除：起点是“分支节点”  终点是 "行为节点" 的连线时
                VNode_Branch node_branch = node_parent as VNode_Branch;
                if (node_branch != null && node_child != null)
                {
                    // 将 "n_child" 从 "n_parent" 的 "port" 数据节点变量中移除
                    ActionTreeAsset.ChildNode_Remove(node_parent.ActionData, node_child.ActionData, edge.output.portName);
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
        /// 当移除标签时
        /// </summary>
        /// <param name="element"></param>
        private void Removed_Label(GraphElement element)
        {
            VNode_Label labelview = element as VNode_Label;
            if (labelview != null)
            {
                Undo.RecordObject(ActionTreeAsset, "Remove Label");
                ActionTreeAsset.Label_Remove(labelview.LabelData);
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
        /// 当移除内部变量时
        /// </summary>
        /// <param name="element"></param>
        private void Removed_InternalVariable(GraphElement element)
        {
            VNode_Variable_Internal vare = element as VNode_Variable_Internal;
            if (vare != null)
            {
                Undo.RecordObject(ActionTreeAsset, "Remove InternalVariable");
                ActionTreeAsset.Remove(vare.VariableData);
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