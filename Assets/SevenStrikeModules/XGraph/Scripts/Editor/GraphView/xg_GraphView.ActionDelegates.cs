namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class xg_GraphView
    {
        /// <summary>
        /// 鼠标移动时的回调
        /// </summary>
        /// <param name="evt"></param>
        private void Action_PointerMove(PointerMoveEvent evt)
        {
            //gv_GraphWindow.SourceTree.LastGraphViewPosition = GetCurrentViewPosition();
            //gv_GraphWindow.SourceTree.LastGraphViewZoom = GetCurrentZoomLevel();
            gv_GraphWindow.xw_GraphInfo_GraphMousePos_Set(GetGraphMousePosition_With_PointerEventMousePosition(evt.position));
        }
        /// <summary>
        /// 鼠标点击事件的回调
        /// </summary>
        /// <param name="evt"></param>
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
                var p_parent = edge.output;
                var p_child = edge.input;

                // 如果是从 "行为节点"  --->  "行为节点"
                if (!(p_parent.node is xNode_Base) && !(p_child.node is xNode_Base))
                    return;
                // 如果是从 "内部变量节点"  --->  "行为节点"
                if ((p_parent.node is xNode_Variable_Internal) && (p_child.node is xNode_Base))
                    return;
                // 如果是从 "变量节点"  --->  "行为节点"
                if ((p_parent.node is xNode_Variable) && (p_child.node is xNode_Base))
                    return;
                // 如果是从 "属性节点"  --->  "行为节点"
                if ((p_parent.node is xNode_Property) && (p_child.node is xNode_Base))
                    return;

                //断开并移除边
                p_parent.Disconnect(edge);
                p_child.Disconnect(edge);
                RemoveElement(edge);

                // 克隆出新的节点
                NodeCreateArgs_Action args = new NodeCreateArgs_Action();
                args.visualName = "延展";
                args.prefixNamespace = "SevenStrikeModules.XGraph";
                args.prefixClass = "xAction_";
                args.actionNodeType = "Relay";
                args.iconName = "relay";
                args.nodeIcon = null;
                args.visualNodeType = "xNode_Relay";
                args.hasAvatar = false;
                args.avatar = null;
                args.themeSolution = null;
                args.themeColor = Color.white;
                args.transparentNode = false;
                args.content = null;
                args.position = graphMouse_pos;
                args.size = Vector2.one * 100;

                xNode_Relay relaynode = Node_MakeRelay(graphMouse_pos, ActionTreeAsset.Create(args));
                relaynode.Draw();
                relaynode.expanded = true;
                relaynode.RefreshExpandedState();
                relaynode.RefreshPorts();

                Edge edge1 = util_XGraphEditorUtility.GetPort_WithType_OfPortList<xAction_Base>(relaynode.Port_Inputs).ConnectTo(p_parent);
                Edge edge2 = util_XGraphEditorUtility.GetPort_WithType_OfPortList<xAction_Base>(relaynode.Port_Outputs).ConnectTo(p_child);

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

            // 清空 BlackboardVariable 选中状态
            gv_GraphWindow.xw_BlackBoardView.VariableList.ClearSelection();


        }
        /// <summary>
        /// 处理快捷键
        /// </summary>
        /// <param name="evt"></param>
        private void Action_KeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.C && (evt.ctrlKey || evt.commandKey))
            {
                Node_Copy();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.V && (evt.ctrlKey || evt.commandKey))
            {
                Node_Paste();
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
                gv_GraphWindow.RestructureGraphViews();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.O && (evt.ctrlKey || evt.commandKey))
            {
                gv_GraphWindow.ActionTree_Open();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.R && (evt.ctrlKey || evt.commandKey))
            {
                gv_GraphWindow.RestructureGraphViews();
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
            foreach (var data in ActionTreeAsset.Actions)
            {
                foreach (var nodes in nodes)
                {
                    if (data.guid == nodes.viewDataKey)
                    {
                        if (nodes is xNode_Base bs)
                        {
                            if (!state)
                                bs.MarkColor_Hidden();
                            else
                                bs.MarkColor_Dislay();
                        }
                    }
                }
            }
            NodeColorDisplay = state;
        }
    }
}