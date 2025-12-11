namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public partial class xg_GraphView
    {
        /// <summary>
        /// 当GraphView组件发生改变时
        /// </summary>
        /// <param assetName="graphViewChange"></param>
        /// <returns></returns>
        public virtual GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
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
        /// <param name="graphViewChange"></param>
        private void On_CreateEdge(GraphViewChange graphViewChange)
        {
            // 当有连线被创建时
            if (graphViewChange.edgesToCreate != null && graphViewChange.edgesToCreate.Count > 0)
            {
                // 清空系统自动创建的连线列表，完全由我们自己处理
                var edgesToCreate = graphViewChange.edgesToCreate.ToList();
                graphViewChange.edgesToCreate.Clear();

                // 如果创建了连线，edge 为连线
                foreach (var edge in edgesToCreate)
                {
                    // 输出端->Parent    |    输入端->Child
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
            // 连线的输出端->Parent    |    连线的输入端->Child

            // 移除原有的连线
            if (edge != null)
            {
                RemoveElement(edge);
            }

            // 创建新的动画连线
            util_AnimatedEdge animatedEdge = null;

            #region 行为节点 ---> 行为节点
            xNode_Base node_parent = n_parent as xNode_Base;
            xNode_Base node_child = n_child as xNode_Base;

            if (node_parent != null && node_child != null)
            {
                // 确保子节点类型不是  VNode_Variable_Internal 类型同时还保证是 ActionNodeBase 类型
                if (!(node_child is xNode_Variable_Internal) && edge.input.portType == typeof(xAction_Base))
                {
                    // 找到对应的端口
                    Port port_start = null;
                    Port port_end = null;

                    // 查找输出端口
                    foreach (var port in node_parent.Port_Outputs)
                    {
                        if (port.Port == edge?.output)
                        {
                            port_start = port.Port;
                            break;
                        }
                    }

                    // 查找输入端口
                    foreach (var port in node_child.Port_Inputs)
                    {
                        if (port.Port == edge?.input)
                        {
                            port_end = port.Port;
                            break;
                        }
                    }

                    if (port_start != null && port_end != null)
                    {
                        animatedEdge = ConnectNode(port_start, port_end);
                        AddElement(animatedEdge);
                    }

                    Undo.RecordObject(ActionTreeAsset, "Add ChildNode");
                    // 将 "n_child" 放到 "n_parent" 的child成员变量中，这样就可以让父级数据节点知道自己和哪个子级数据节点相连接
                    ActionTreeAsset.ChildNode_Add(node_parent.ActionData, node_child.ActionData);
                }
            }
            #endregion

            #region 分支节点 ---> 行为节点
            xNode_Branch node_branch = n_parent as xNode_Branch;
            if (node_branch != null && node_child != null)
            {
                // 找到对应的端口
                Port port_start = null;
                Port port_end = null;

                // 查找输出端口
                foreach (var port in node_branch.Port_Outputs)
                {
                    if (port.Port == edge?.output && port.Name == edge?.output.portName)
                    {
                        port_start = port.Port;
                        break;
                    }
                }

                // 查找输入端口
                foreach (var port in node_child.Port_Inputs)
                {
                    if (port.Port == edge?.input)
                    {
                        port_end = port.Port;
                        break;
                    }
                }

                if (port_start != null && port_end != null)
                {
                    animatedEdge = ConnectNode(port_start, port_end);
                    AddElement(animatedEdge);
                }

                Undo.RecordObject(ActionTreeAsset, "Add BranchChildNode");
                // 将 "n_child" 放到 "n_parent" 的相对应True和False的child成员变量中
                ActionTreeAsset.ChildNode_Add(node_branch.ActionData, node_child.ActionData, edge?.output.portName);

                Debug.Log($"{node_branch.branch.childNode_true}  / {node_branch.branch.childNode_false}");
            }
            #endregion

            #region 黑板变量节点 ---> 内部变量节点
            xNode_Variable node_var = n_parent as xNode_Variable;
            xNode_Variable_Internal node_internalvar = node_child as xNode_Variable_Internal;
            if (node_var != null && node_internalvar != null)
            {
                Port port_start = node_var.OutputPort?.Port;
                Port port_end = null;

                // 查找输入端口
                foreach (var port in node_internalvar.Port_Inputs)
                {
                    if (port.Port == edge?.input && port.Name == edge?.input.portName)
                    {
                        port_end = port.Port;
                        break;
                    }
                }

                if (port_start != null && port_end != null)
                {
                    animatedEdge = ConnectNode(port_start, port_end);
                    AddElement(animatedEdge);
                }

                Undo.RecordObject(node_child.ActionData.BaseArgs.RootAsset, "Bind Variable  To Action");
                string portName = edge?.input.portName;
                // 加入行为节点数据中的变量列表中
                node_child.ActionData.VariableData_Bind(node_var.VariableData, portName);

                // 两个变量类型一致时
                if (node_internalvar.VariableData.variable.type == node_var.VariableData.type)
                {
                    node_internalvar.UpdateFieldValue();
                }
            }
            #endregion

            #region 内部变量节点 ---> 行为节点
            xNode_Variable_Internal node_varInternal = n_parent as xNode_Variable_Internal;
            if (node_varInternal != null && node_child != null && node_child is not xNode_Variable_Internal)
            {
                Port port_start = node_varInternal.Port_Outputs.FirstOrDefault()?.Port;
                Port port_end = null;

                // 查找输入端口
                foreach (var port in node_child.Port_Inputs)
                {
                    if (port.Port == edge?.input && port.Name == edge?.input.portName)
                    {
                        port_end = port.Port;
                        break;
                    }
                }

                if (port_start != null && port_end != null)
                {
                    animatedEdge = ConnectNode(port_start, port_end);
                    AddElement(animatedEdge);
                }

                Undo.RecordObject(node_child.ActionData.BaseArgs.RootAsset, "Bind InternalVariableAction To Action");
                string portName = edge?.input.portName;
                node_child.ActionData.InternalVariableData_Bind(node_varInternal.VariableData, portName);
            }
            #endregion

            #region 黑板变量节点 ---> 行为节点
            xNode_Variable node_bb_var = n_parent as xNode_Variable;
            if (node_bb_var != null && node_child != null)
            {
                Port port_start = node_bb_var.OutputPort?.Port;
                Port port_end = null;

                // 查找输入端口
                foreach (var port in node_child.Port_Inputs)
                {
                    if (port.Port == edge?.input && port.Name == edge?.input.portName)
                    {
                        port_end = port.Port;
                        break;
                    }
                }

                if (port_start != null && port_end != null)
                {
                    animatedEdge = ConnectNode(port_start, port_end);
                    AddElement(animatedEdge);
                }

                Undo.RecordObject(node_child.ActionData.BaseArgs.RootAsset, "Bind Variable To Action");
                string portName = edge?.input.portName;
                // 加入行为节点数据中的变量列表中
                node_child.ActionData.VariableData_Bind(node_bb_var.VariableData, portName);
            }
            #endregion

            #region 属性节点 ---> 行为节点
            xNode_Property node_property = n_parent as xNode_Property;
            if (node_property != null && node_child != null)
            {
                // 保证输入 / 输出端口类型不是 xAction_Base 类型
                if (edge.input.portType != typeof(xAction_Base) && edge.output.portType != typeof(xAction_Base))
                {
                    // 找到对应的端口
                    Port port_property = null;
                    Port port_action = null;

                    // 查找输出端口
                    foreach (var port in node_property.Port_Outputs)
                    {
                        if (port.Port == edge?.output)
                        {
                            port_property = port.Port;
                            break;
                        }
                    }

                    // 查找输入端口
                    foreach (var port in node_child.Port_Inputs)
                    {
                        if (port.Port == edge?.input)
                        {
                            port_action = port.Port;
                            break;
                        }
                    }

                    if (port_property != null && port_action != null)
                    {
                        animatedEdge = ConnectNode(port_property, port_action);
                        AddElement(animatedEdge);
                    }

                    Undo.RecordObject(node_property.property.BaseArgs.RootAsset, "Bind Property To Action");

                    // node_property.PropertyData : 目标属性节点的属性数据
                    xAction_Property propertyData = node_property.property;
                    // "属性节点" 端口的 -- 名称
                    string property_port_name = port_property.portName;
                    // "属性节点" 端口的 -- 类型
                    string property_port_type = port_property.portType.ToString();
                    // "行为节点" 端口的 -- 名称
                    string action_port_name = port_action.portName;
                    // "行为节点" 端口的 -- 类型
                    string action_port_type = port_action.portType.ToString();

                    // 将属性节点的目标端口绑定到行为节点的 "属性列表记录" 中
                    node_child.ActionData.Property_Bind(
                        propertyData,
                        property_port_name,
                        property_port_type,
                        action_port_name,
                        action_port_type);
                }
            }

            #endregion

            #region 特化处理延展节点
            xNode_Relay node_relay = edge?.input.node as xNode_Relay;
            if (node_relay != null)
            {
                node_relay.Connected();
            }
            #endregion

            #region 调用节点的创建连线的回调
            if (node_parent != null && node_child != null)
            {
                if (node_parent.ActionData.On_Node_CreateEdge != null)
                    node_parent.ActionData.On_Node_CreateEdge(edge);

                if (node_child.ActionData.On_Node_CreateEdge != null)
                    node_child.ActionData.On_Node_CreateEdge(edge);
            }
            #endregion

            #region 创建连线后更新Inspector面板
            // 如果 xw_currentSelectedVisualNode  不为空则 Inspector 面板显示当前选中的节点的属性
            if (gv_GraphWindow.xw_currentSelectedVisualNode != null)
                gv_GraphWindow.xw_InspectorView.InspectorViewer(gv_GraphWindow.xw_currentSelectedVisualNode);
            #endregion

            if (animatedEdge != null)
                animatedEdge.OnUnSelectedEdge += OnUnSelectedEdge;
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
        /// 当移除连线时
        /// </summary>
        /// <param name="element"></param>
        private void Removed_Edge(GraphElement element)
        {
            util_AnimatedEdge edge = element as util_AnimatedEdge;
            if (edge != null)
            {
                xNode_Base node_parent = edge.output.node as xNode_Base;
                xNode_Base node_child = edge.input.node as xNode_Base;

                #region 移除：起点是 ”行为节点“ 终点是 ”行为节点“ 的连线时
                if (node_parent != null && node_child != null)
                {
                    // 确保子节点类型不是  VNode_Variable_Internal 类型同时还保证是 ActionNodeBase 类型
                    if (!(node_child is xNode_Variable_Internal) && edge.input.portType == typeof(xAction_Base))
                    {
                        Undo.RecordObject(ActionTreeAsset, "Remove ChildNode");
                        // 将 "n_child" 从 "n_parent" 的 "port" 数据节点变量中移除
                        ActionTreeAsset.ChildNode_Remove(node_parent.ActionData, node_child.ActionData);
                    }
                }
                #endregion

                #region 移除：起点是 ”行为节点“ 终点是 ”延展节点“ 的连线时
                xNode_Relay relay_child = edge.input.node as xNode_Relay;
                if (relay_child != null)
                {
                    //Undo.RecordObject(relay_child.ActionData, "Remove RelayConnector");
                    relay_child.Disconnected();
                }
                #endregion

                #region 移除：起点是 “黑板变量节点” 终点是 带有 "Variable类型" 端口的 "行为节点" 的连线时
                xNode_Variable node_var = edge.output.node as xNode_Variable;
                if (node_var != null)
                {
                    string portName = edge.input.portName;
                    // 获取变量节点的变量类型
                    Type type = node_var.VariableData.variable.GetType();
                    // 拿到行为节点上的对应的变量类型的端口
                    //Port port = node_child.GetPort(type, portName, xPortType.In);
                    Port port = null;
                    if (node_child is xNode_Debug)
                        port = node_child.GetPort(portName, xPortType.In);
                    else
                        port = node_child.GetPort(type, portName, xPortType.In);

                    // 断开端口与连线的连接
                    port.Disconnect(edge);

                    // 从行为节点解绑 ”黑板变量数据“
                    node_child.ActionData.VariableData_Unbind(node_var.VariableData.guid_n, portName);
                }
                #endregion

                #region 移除：起点是“内部变量节点”  终点是 带有 "Variable类型" 端口的 "行为节点" 的连线时
                xNode_Variable_Internal node_var_internal = edge.output.node as xNode_Variable_Internal;
                if (node_var_internal != null)
                {
                    string portName = edge.input.portName;
                    // 获取变量节点的变量类型
                    Type type = node_var_internal.VariableData.variable.GetType();

                    // 拿到行为节点上的对应的变量类型的端口
                    Port port = null;
                    if (node_child is xNode_Debug)
                        port = node_child.GetPort(portName, xPortType.In);
                    else
                        port = node_child.GetPort(type, portName, xPortType.In);

                    // 断开端口与连线的连接
                    port.Disconnect(edge);

                    // 从行为节点解绑 ”内部变量节点数据“
                    node_child.ActionData.InternalVariableData_Unbind(node_var_internal.VariableData.BaseArgs.guid, portName);
                }
                #endregion

                #region 移除：起点是“分支节点”  终点是 "行为节点" 的连线时
                xNode_Branch node_branch = node_parent as xNode_Branch;
                if (node_branch != null && node_child != null)
                {
                    Undo.RecordObject(ActionTreeAsset, "Remove ChildNode");
                    // 将 "n_child" 从 "n_parent" 的 "port" 数据节点变量中移除
                    ActionTreeAsset.ChildNode_Remove(node_parent.ActionData, node_child.ActionData, edge.output.portName);
                }
                #endregion

                #region 移除：起点是 ”属性节点“ 终点是 ”行为节点“ 的连线时
                xNode_Property node_Property = node_parent as xNode_Property;
                if (node_parent != null && node_child != null)
                {
                    // 确保子节点类型是  xNode_Base 类型同时还保证 "输入 / 输出" 端口类型不是 xAction_Base 类型
                    if (node_child is xNode_Base && node_parent is not xNode_Variable_Internal && edge.input.portType != typeof(xAction_Base) && edge.output.portType != typeof(xAction_Base))
                    {
                        // node_property.PropertyData : 目标属性节点的属性数据
                        string property_guid = node_Property.property.BaseArgs.guid;
                        // "属性节点" 端口的 -- 名称
                        string property_port_name = edge.output.portName;
                        // "属性节点" 端口的 -- 类型
                        string property_port_type = edge.output.portType.ToString();
                        // "行为节点" 端口的 -- 名称
                        string action_port_name = edge.input.portName;
                        // "行为节点" 端口的 -- 类型
                        string action_port_type = edge.input.portType.ToString();

                        // 从行为节点的 "属性列表记录" 中解绑
                        node_child.ActionData.Property_Unbind(
                            property_guid,
                            property_port_name,
                            property_port_type,
                            action_port_name,
                            action_port_type);
                    }
                }
                #endregion

                if (node_parent != null && node_child != null)
                {
                    if (node_parent.ActionData.On_Node_RemovedEdge != null)
                        node_parent.ActionData.On_Node_RemovedEdge(edge);

                    if (node_child.ActionData.On_Node_RemovedEdge != null)
                        node_child.ActionData.On_Node_RemovedEdge(edge);
                }
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
                xGroupData groupData = ActionTreeAsset.Groups.FirstOrDefault(g => g.group == group);
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
        /// 当移除便签时
        /// </summary>
        /// <param name="element"></param>
        private void Removed_Stick(GraphElement element)
        {
            xNode_Stick stickview = element as xNode_Stick;
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
            xNode_Label labelview = element as xNode_Label;
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
            xNode_Decal decalview = element as xNode_Decal;
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
            xNode_Variable vare = element as xNode_Variable;
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
            xNode_Variable_Internal vare = element as xNode_Variable_Internal;
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
            xNode_Base nodeview = element as xNode_Base;
            if (nodeview != null)
            {
                Undo.RecordObject(ActionTreeAsset, "Remove ActionNode");
                // 从根节点中移除数据节点
                ActionTreeAsset.Remove(nodeview.ActionData);
            }
        }
        #endregion
    }
}