namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class xg_GraphView
    {
        private void Action_MouseMove(PointerMoveEvent evt)
        {
            gv_GraphWindow.xw_GraphInfo_GraphMousePos_Set(GetGraphMousePosition_With_PointerEventMousePosition(evt.position));
        }
        /// <summary>
        /// 鼠标点击事件的回调函数
        /// </summary>
        /// <param assetName="evt"></param>
        private void Action_PointerDown(PointerDownEvent evt)
        {
            if (gv_GraphWindow == null)
                return;

            Vector2 graphMouse_pos = GetGraphMousePosition_With_PointerEventMousePosition(evt.position);
            gv_NodeCreatedPosition = graphMouse_pos;

            #region 双击两个节点之间的连线时
            if (evt.clickCount == 2 && evt.target is Edge edge)
            {
                // 先存储边和端口信息
                var p_child = edge.input;
                var p_parent = edge.output;

                // 断开并移除边
                p_parent.Disconnect(edge);
                p_child.Disconnect(edge);
                RemoveElement(edge);

                string asm = typeof(ActionNode_Base).Assembly.FullName;
                Type scriptType_Actiontree = Type.GetType($"SevenStrikeModules.XGraph.ActionNode_Relay,{asm}", true);

                // 在行为树根资源中加入新的数据项
                ActionNode_Base data = InstantiateActionNode(
                    prefix_namespace: "SevenStrikeModules.XGraph",
                    prefix_class: "ActionNode_",
                    type: scriptType_Actiontree,
                    action_nodeType: "Relay",
                    icon: "relay",
                    titleicon: null,
                    visual_nodeType: "VNode_Relay",
                    action_name: "延展",
                    hasAvatar: false,
                    avatar: null,
                    themeSolution: null,
                    themeColor: Color.white,
                    transparentNode: false,
                    content: null,
                    nodeGraphSize: Vector2.one * 100);

                VNode_Relay relaynode = Node_MakeRelay(graphMouse_pos, data);
                relaynode.Draw();
                relaynode.expanded = true;
                relaynode.RefreshExpandedState();
                relaynode.RefreshPorts();

                Edge edge1 = relaynode.Port_Input.Port.ConnectTo(p_parent);
                Edge edge2 = relaynode.Port_Outputs.First().Port.ConnectTo(p_child);

                AddElement(edge1);
                AddElement(edge2);

                // 手动触发 GraphViewChanged 事件
                var edgesToRemove = new List<GraphElement> { edge };
                var edgesToCreate = new List<Edge> { edge1, edge2 };
                var changes = new GraphViewChange
                {
                    elementsToRemove = edgesToRemove,
                    movedElements = new List<GraphElement>(),
                    edgesToCreate = edgesToCreate
                };
                // 调用 OnGraphViewChanged
                OnGraphViewChanged(changes);
            }
            #endregion

            // 隐藏选项面板
            if (evt.button == (int)MouseButton.LeftMouse)
            {
                gv_GraphWindow.OptionsPanel_Hide();
                gv_GraphWindow.OptionsPanel_CloseButton_Hide();
                gv_GraphWindow.OptionsPanel_ExpanderButton_Display();
                gv_GraphWindow.OptionsPanel_ToggleChange_WithoutNotify(false);
            }

            // 清空 VariableList 选中状态
            gv_GraphWindow.xw_BlackBoardView.VariableList.ClearSelection();
        }
        /// <summary>
        /// 处理快捷键
        /// </summary>
        /// <param assetName="evt"></param>
        private void Action_KeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.C && (evt.ctrlKey || evt.commandKey))
            {
                //Node_Copy();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.V && (evt.ctrlKey || evt.commandKey))
            {
                //Node_Paste(gv_NodeCreatedPosition);
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.D && (evt.ctrlKey || evt.commandKey))
            {
                Node_Duplicate();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.A && (evt.ctrlKey || evt.commandKey))
            {
                Node_Selectall();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.S && (evt.ctrlKey || evt.commandKey))
            {
                CollectGroupsPosition();
                gv_GraphWindow.ActionTree_SaveAndReplace();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.O && (evt.ctrlKey || evt.commandKey))
            {
                gv_GraphWindow.ActionTree_Open();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.G && (evt.ctrlKey || evt.commandKey))
            {
                MakeGroup("节点编组", gv_NodeCreatedPosition);
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.C && ((evt.ctrlKey && evt.shiftKey) || evt.commandKey))
            {
                gv_GraphWindow.ActionTree_Clear();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.Escape)
            {
                gv_GraphWindow.Close();
                evt.StopPropagation();
            }

            //evt.StopPropagation();
        }
        /// <summary>
        /// 注册XGraphWindow的 节点颜色标记开关状态委托
        /// </summary>
        public void Action_Register_NodeColorDisplayer()
        {
            #region 注册XGraphWindow委托
            gv_GraphWindow.OnNodeColorToggleChanged += Action_On_NodeColorDisplayer_Changed;
            #endregion
        }
        /// <summary>
        /// 注销XGraphWindow的 节点颜色标记开关状态委托
        /// </summary>
        public void Action_Unregister_NodeColorDisplayer()
        {
            #region 注册XGraphWindow委托
            gv_GraphWindow.OnNodeColorToggleChanged -= Action_On_NodeColorDisplayer_Changed;
            #endregion
        }
        /// <summary>
        /// 所有节点的颜色标记开关逻辑
        /// </summary>
        /// <param name="state"></param>
        private void Action_On_NodeColorDisplayer_Changed(bool state)
        {
            ActionTreeAsset.ActionNodes.ForEach(data =>
            {
                nodes.ForEach(nodes =>
                {
                    if (data.guid == nodes.viewDataKey)
                    {
                        if (nodes is VNode_Base bs)
                        {
                            if (!state)
                                bs.MarkColor_Hidden();
                            else
                                bs.MarkColor_Dislay();
                        }
                    }
                });
            });
            NodeColorDisplay = state;
        }
    }
}