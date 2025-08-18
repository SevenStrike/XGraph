namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.Graphs;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.UIElements;

    public partial class xg_GraphView
    {
        /// <summary>
        /// 实现 GraphView 视图内的鼠标右键上下文菜单
        /// </summary>
        /// <param h_name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //base.BuildContextualMenu(evt);

            //Debug.Log(evt.sourceNode);

            bool isInGraphView = false;
            bool isInGraphNode = false;
            bool isInStickNode = false;
            bool isInGraphGroup = false;

            #region  确认当前有点点击的物体是否是Group
            if (evt.target is VisualElement ele)
            {
                var current = ele.parent;
                while (current != null)
                {
                    //Debug.Log(current);
                    if (current is Group group)
                    {
                        // 如果找到可以确认这个元素属于一个Group，那么以下菜单都不会显示了，确保独立显示Group专属菜单
                        isInGraphGroup = true;
                    }
                    current = current.parent; // 继续向上查找
                }
            }
            #endregion

            #region  确认当前有点点击的物体是否是GraphNode
            if (evt.target is VNode_Base nodebase)
            {
                // 菜单 - 自定主题色切换
                evt.menu.AppendAction($"T 节点配色/A 自定颜色", (action) =>
                {
                    var t = typeof(EditorWindow).Assembly.GetTypes().FirstOrDefault(ty => ty.Name == "ColorPicker");
                    var m = t?.GetMethod("Show", new[] { typeof(Action<Color>), typeof(Color), typeof(bool), typeof(bool) });
                    if (m == null)
                    {
                        Debug.LogWarning("Could not invoke Color Picker for ShaderGraph.");
                        return;
                    }

                    VNode_Base node = CurrentSelectedNodes.First();

                    var defaultColor = Color.gray;
                    defaultColor = node.ActionNode.themeColor;
                    defaultColor.a = 1.0f;

                    void ApplyColor(Color pickedColor)
                    {
                        foreach (var selectable in selection)
                        {
                            if (selectable is VNode_Base node)
                            {
                                Undo.RecordObject(node.ActionNode, "Change NodeColor");
                                node.ActionNode.themeSolution = "自定义";
                                node.ActionNode.themeColor = pickedColor;

                                // 改变图标颜色
                                node.TitleIconLabel.style.unityBackgroundImageTintColor = node.ActionNode.themeSolution == "M 默认" ? Color.white * 0.7f : node.ActionNode.themeColor;

                                // 改变连线颜色
                                if (node.Port_Input != null && node.Port_Input.Port != null)
                                {
                                    node.Port_Input.Port.portColor = node.ActionNode.themeSolution == "M 默认" ? Color.white * 0.7f : node.ActionNode.themeColor;

                                    var edges = node.Port_Input.Port.connections.ToList();
                                    // 遍历所有连线
                                    foreach (var edge in edges)
                                    {
                                        edge.edgeControl.inputColor = pickedColor;
                                    }
                                }
                                if (node.Port_Outputs != null)
                                {
                                    node.Port_Outputs.ForEach(x =>
                                    {
                                        x.Port.portColor = node.ActionNode.themeSolution == "M 默认" ? Color.white * 0.7f : node.ActionNode.themeColor;

                                        var edges = x.Port.connections.ToList();
                                        // 遍历所有连线
                                        foreach (var edge in edges)
                                        {
                                            edge.edgeControl.outputColor = pickedColor;
                                        }
                                    });
                                }
                                node.UpdateMarkColor();
                                if (gv_GaphWindow.xw_toggle_DisplayNodeColor.value)
                                    node.MarkColor_Dislay();
                            }
                        }
                    }
                    m.Invoke(null, new object[] { (Action<Color>)ApplyColor, Color.white, true, false });
                });

                // 菜单 - 主题色切换
                for (int i = 0; i < ThemesList.Node.Count; i++)
                {
                    ThemeData_Node dat = ThemesList.Node[i];
                    evt.menu.AppendAction($"T 节点配色/{dat.solution}", (action) =>
                    {
                        if (CurrentSelectedNodes.Count > 0)
                        {
                            for (int s = 0; s < CurrentSelectedNodes.Count; s++)
                            {
                                VNode_Base node = CurrentSelectedNodes[s];

                                Undo.RecordObject(node.ActionNode, "Change NodeColor");
                                node.ActionNode.themeSolution = dat.solution;
                                node.ActionNode.themeColor = util_EditorUtility.Color_From_HexString(dat.nodecolor);

                                // 改变图标颜色
                                node.IconLabel.style.unityBackgroundImageTintColor = node.ActionNode.themeSolution == "M 默认" ? Color.white * 0.7f : node.ActionNode.themeColor;

                                // 改变图标颜色
                                node.TitleIconLabel.style.unityBackgroundImageTintColor = node.ActionNode.themeSolution == "M 默认" ? Color.white * 0.7f : node.ActionNode.themeColor;

                                // 改变连线颜色
                                if (node.Port_Input != null && node.Port_Input.Port != null)
                                {
                                    node.Port_Input.Port.portColor = node.ActionNode.themeSolution == "M 默认" ? Color.white * 0.7f : node.ActionNode.themeColor;
                                    var edges = node.Port_Input.Port.connections.ToList();
                                    // 遍历所有连线
                                    foreach (var edge in edges)
                                    {
                                        edge.edgeControl.inputColor = node.ActionNode.themeColor;
                                    }
                                }
                                if (node.Port_Outputs != null)
                                {
                                    node.Port_Outputs.ForEach(x =>
                                    {
                                        x.Port.portColor = node.ActionNode.themeSolution == "M 默认" ? Color.white * 0.7f : node.ActionNode.themeColor;
                                        var edges = x.Port.connections.ToList();
                                        // 遍历所有连线
                                        foreach (var edge in edges)
                                        {
                                            edge.edgeControl.outputColor = node.ActionNode.themeColor;
                                        }
                                    });
                                }
                                node.UpdateMarkColor();
                                if (gv_GaphWindow.xw_toggle_DisplayNodeColor.value)
                                    node.MarkColor_Dislay();
                            }
                        }
                    });
                }

                // 执行模式切换
                evt.menu.AppendAction($"E 执行模式/S 顺序", (action) =>
                {
                    if (CurrentSelectedNodes.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes.Count; s++)
                        {
                            VNode_Base node = CurrentSelectedNodes[s];
                            node.ActionNode.isConcurrentExecution = false;
                            node.CheckExecutionModel();
                        }
                    }
                });
                evt.menu.AppendAction($"E 执行模式/C 并发", (action) =>
                {
                    if (CurrentSelectedNodes.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes.Count; s++)
                        {
                            VNode_Base node = CurrentSelectedNodes[s];
                            node.ActionNode.isConcurrentExecution = true;
                            node.CheckExecutionModel();
                        }
                    }
                });

                isInGraphNode = true;
                evt.menu.AppendSeparator();
            }
            #endregion

            #region  确认当前有点点击的物体是否是GraphView框架
            if (evt.target is xg_GraphView graphview)
            {
                isInGraphView = true;
                evt.menu.AppendSeparator();
                // 菜单 - 主题色切换
                for (int i = 0; i < ThemesList.GraphviewBgGrid.Count; i++)
                {
                    ThemeData_GraphViewBgGrid dat = ThemesList.GraphviewBgGrid[i];
                    evt.menu.AppendAction($"T 背景配色/{dat.solution}", (action) =>
                    {
                        SwitchGraphViewBgTheme(dat);
                    });
                }
            }
            #endregion

            #region  确认当前有点点击的物体是否是Stick节点
            if (evt.target is VNode_Stick stick)
            {
                isInStickNode = true;
            }
            #endregion

            if (isInGraphView)
            {
                #region 节点操作：添加节点
                evt.menu.AppendAction("A 添加节点", (action) =>
                {
                    Vector2 screenMousePosition = action.eventInfo.mousePosition + gv_GaphWindow.position.position;
                    nodeCreationRequest(new NodeCreationContext()
                    {
                        // 将当前鼠标的坐标传递给搜索框的坐标
                        screenMousePosition = screenMousePosition,
                        index = -1
                    });
                });
                #endregion

                #region 节点操作：清空节点
                if (nodes.Count() > 0)
                {
                    evt.menu.AppendAction("Z 清空节点", (action) =>
                    {
                        ClearGraphViewContents();
                    });
                }
                #endregion
            }

            evt.menu.AppendSeparator();

            #region 节点操作：粘贴节点
            if (gv_CopiedNodeList.Count > 0)
            {
                evt.menu.AppendAction("V 粘贴节点", param =>
                {
                    //Node_Paste(gv_NodeCreatedPosition);
                });
            }
            #endregion

            evt.menu.AppendSeparator();

            if (!isInGraphGroup && isInGraphNode || isInStickNode)
            {
                #region 节点操作：删除节点
                evt.menu.AppendAction("S 删除节点", param =>
                {
                    Node_Delete();
                });
                #endregion

                #region 节点操作：节点编组
                evt.menu.AppendAction("G 节点编组", param =>
                {
                    MakeGroup("节点编组", gv_NodeCreatedPosition);
                });
                #endregion

                #region 节点操作：克隆和复制
                if (selection.Count != 0)
                {
                    evt.menu.AppendAction("D 克隆节点", param =>
                    {
                        Node_Duplicate();
                    });
                    evt.menu.AppendAction("C 复制节点", param =>
                    {
                        //Node_Copy();
                    });
                }
                #endregion
            }
        }
    }
}