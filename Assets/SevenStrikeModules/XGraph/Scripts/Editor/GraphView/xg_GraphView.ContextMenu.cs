namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class xg_GraphView
    {
        /// <summary>
        /// 实现 GraphView 视图内的鼠标右键上下文菜单
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //base.BuildContextualMenu(evt);

            bool isInGraphView = false;
            bool isInGraphNode = false;
            bool isInVariableNode = false;
            bool isInStickNode = false;
            bool isInDecalNode = false;
            bool isInGraphGroup = false;

            #region  确认当前有点击的物体是否是Group
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

            #region  确认当前有点击的物体是否是ActionNode
            if (evt.target is VNode_Base nodebase)
            {

                // 菜单 - 自定主题色切换
                evt.menu.AppendAction($"T 节点配色/T 自定颜色", (action) =>
                {
                    if (Application.isPlaying)
                        return;
                    #region 打开颜色选择器
                    var t = typeof(EditorWindow).Assembly.GetTypes().FirstOrDefault(ty => ty.Name == "ColorPicker");
                    var m = t?.GetMethod("Show", new[] { typeof(Action<Color>), typeof(Color), typeof(bool), typeof(bool) });
                    if (m == null)
                    {
                        Debug.LogWarning("Could not invoke Color Picker for XGraph.");
                        return;
                    }
                    VNode_Base node = (VNode_Base)CurrentSelectedNodes_Base.First();
                    var defaultColor = Color.gray;
                    defaultColor = node.ActionData.themeColor;
                    defaultColor.a = 1.0f;
                    #endregion

                    void ApplyColor(Color pickedColor)
                    {
                        foreach (var selectable in selection)
                        {
                            if (selectable is VNode_Base node)
                            {
                                Undo.RecordObject(node.ActionData, "Change NodeColor");
                                node.ActionData.themeSolution = "自定义";
                                node.ActionData.themeColor = pickedColor;

                                // 改变分割图标颜色
                                node.SeperateIconLabel.style.unityBackgroundImageTintColor = pickedColor;


                                // 改变连线颜色
                                if (node.Port_Inputs != null)
                                {
                                    node.Port_Inputs.ForEach(x =>
                                    {
                                        x.Port.portColor = node.ActionData.themeColor;
                                        util_XGraphEditorUtility.Element_BoderColor_Set(x.PortDonut, node.ActionData.themeColor);
                                        var edges = x.Port.connections.ToList();
                                        // 遍历所有连线
                                        foreach (var edge in edges)
                                        {
                                            edge.edgeControl.inputColor = node.ActionData.themeColor;
                                        }
                                    });
                                }
                                if (node.Port_Outputs != null)
                                {
                                    node.Port_Outputs.ForEach(x =>
                                    {
                                        x.Port.portColor = node.ActionData.themeColor;
                                        util_XGraphEditorUtility.Element_BoderColor_Set(x.PortDonut, node.ActionData.themeColor);

                                        var edges = x.Port.connections.ToList();
                                        // 遍历所有连线
                                        foreach (var edge in edges)
                                        {
                                            edge.edgeControl.outputColor = pickedColor;
                                        }
                                    });
                                }
                                node.UpdateMarkColor();
                                if (gv_GraphWindow.xw_toggle_DisplayNodeColor.value)
                                    node.MarkColor_Dislay();
                            }
                        }
                    }
                    m.Invoke(null, new object[] { (Action<Color>)ApplyColor, defaultColor, true, false });
                });

                // 菜单 - 主题色切换
                for (int i = 0; i < ThemesList.Node.Count; i++)
                {
                    ThemeData_Node dat = ThemesList.Node[i];
                    evt.menu.AppendAction($"T 节点配色/{dat.solution}", (action) =>
                    {
                        if (CurrentSelectedNodes_Base.Count > 0)
                        {
                            for (int s = 0; s < CurrentSelectedNodes_Base.Count; s++)
                            {
                                VNode_Base node = (VNode_Base)CurrentSelectedNodes_Base[s];

                                Undo.RecordObject(node.ActionData, "Change NodeColor");
                                node.ActionData.themeSolution = dat.solution;
                                node.ActionData.themeColor = util_XGraphEditorUtility.Color_From_HexString(dat.nodecolor);

                                // 改变分割图标颜色
                                node.SeperateIconLabel.style.unityBackgroundImageTintColor = node.ActionData.themeColor;

                                // 改变连线颜色
                                if (node.Port_Inputs != null)
                                {
                                    node.Port_Inputs.ForEach(x =>
                                    {
                                        x.Port.portColor = node.ActionData.themeColor;
                                        util_XGraphEditorUtility.Element_BoderColor_Set(x.PortDonut, node.ActionData.themeColor);

                                        var edges = x.Port.connections.ToList();
                                        // 遍历所有连线
                                        foreach (var edge in edges)
                                        {
                                            edge.edgeControl.inputColor = node.ActionData.themeColor;

                                        }
                                    });
                                }
                                if (node.Port_Outputs != null)
                                {
                                    node.Port_Outputs.ForEach(x =>
                                    {
                                        x.Port.portColor = node.ActionData.themeColor;
                                        util_XGraphEditorUtility.Element_BoderColor_Set(x.PortDonut, node.ActionData.themeColor);

                                        var edges = x.Port.connections.ToList();
                                        // 遍历所有连线
                                        foreach (var edge in edges)
                                        {
                                            edge.edgeControl.outputColor = node.ActionData.themeColor;
                                        }
                                    });
                                }
                                node.UpdateMarkColor();
                                if (gv_GraphWindow.xw_toggle_DisplayNodeColor.value)
                                    node.MarkColor_Dislay();
                            }
                        }
                    });
                }

                evt.menu.AppendSeparator();

                // 执行模式切换
                evt.menu.AppendAction($"E 执行模式/E 顺序", (action) =>
                {
                    if (CurrentSelectedNodes_Base.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes_Base.Count; s++)
                        {
                            VNode_Base node = (VNode_Base)CurrentSelectedNodes_Base[s];
                            node.ActionData.isConcurrentExecution = false;
                            node.CheckExecutionModel();
                        }
                    }
                    evt.StopPropagation();
                });
                evt.menu.AppendAction($"E 执行模式/D 并发", (action) =>
                {
                    if (CurrentSelectedNodes_Base.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes_Base.Count; s++)
                        {
                            VNode_Base node = (VNode_Base)CurrentSelectedNodes_Base[s];
                            node.ActionData.isConcurrentExecution = true;
                            node.CheckExecutionModel();
                        }
                    }
                    evt.StopPropagation();
                });

                // 设置节点头像
                if (nodebase.ActionData.actionNodeType != "Relay")
                {
                    if (nodebase.ActionData.HasAvatar)
                    {
                        evt.menu.AppendSeparator();
                        evt.menu.AppendAction($"R 头像/R 清空头像", (action) =>
                        {
                            if (CurrentSelectedNodes_Base.Count > 0)
                            {
                                for (int s = 0; s < CurrentSelectedNodes_Base.Count; s++)
                                {
                                    VNode_Base node = (VNode_Base)CurrentSelectedNodes_Base[s];
                                    node.NodeAvatar_Remove();
                                    node.UnregisterAvatarClicked();
                                }
                            }
                            evt.StopPropagation();
                        });
                        evt.menu.AppendAction($"R 头像/W 替换头像", (action) =>
                        {
                            OpenObjectPickerForTextures("AvatarSet", "t:Texture2D", nodebase.ActionData.Avatar);
                            evt.StopPropagation();
                        });
                    }
                    else
                    {
                        evt.menu.AppendSeparator();
                        evt.menu.AppendAction($"R 头像/R 设置头像", (action) =>
                        {
                            OpenObjectPickerForTextures("AvatarSet", "t:Texture2D", nodebase.ActionData.Avatar);
                            evt.StopPropagation();
                        });
                    }
                    evt.menu.AppendSeparator();
                    evt.menu.AppendAction($"B 图标/B 恢复原生图标", (action) =>
                    {
                        if (CurrentSelectedNodes_Base.Count > 0)
                        {
                            for (int s = 0; s < CurrentSelectedNodes_Base.Count; s++)
                            {
                                VNode_Base node = (VNode_Base)CurrentSelectedNodes_Base[s];
                                Undo.RecordObject(node.ActionData, "Restore ActionNodeTitleIcon");
                                node.ActionData.NodeIcon = null;
                                node.NodeTitleIcon_Restore();
                            }
                        }
                        evt.StopPropagation();
                    });
                    evt.menu.AppendAction($"B 图标/V 设置标题图标", (action) =>
                    {
                        OpenObjectPickerForTextures("TitleIconSet", "t:Texture2D", nodebase.ActionData.NodeIcon);
                        evt.StopPropagation();
                    });
                }
                evt.menu.AppendSeparator();
                isInGraphNode = true;
            }
            #endregion

            #region  确认当前有点击的物体是否是 VariableNode
            if (evt.target is VNode_Variable nodevar)
            {
                evt.menu.AppendAction($"C 查看变量名称", (action) =>
                {
                    if (CurrentSelectedNodes_Variable.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes_Variable.Count; s++)
                        {
                            VNode_Variable node = (VNode_Variable)CurrentSelectedNodes_Variable[s];
                            Debug.Log(node.VariableData.name);
                        }
                    }
                    evt.StopPropagation();
                });
                evt.menu.AppendSeparator();
                isInVariableNode = true;
            }
            #endregion

            #region  确认当前有点击的物体是否是GraphView框架
            if (evt.target is xg_GraphView graphview)
            {
                evt.menu.AppendAction($"S 编辑器选项设置", (action) =>
                {
                    gv_GraphWindow.OptionsPanel_Display();
                    gv_GraphWindow.xw_OptionsPanel_ExpanderButton_Hide();
                    gv_GraphWindow.OptionsPanel_CloseButton_Display();
                    gv_GraphWindow.OptionsPanel_ToggleChange_WithoutNotify(true);
                    evt.StopPropagation();
                });
                isInGraphView = true;
            }
            #endregion

            #region  确认当前有点击的物体是否是Stick节点
            if (evt.target is VNode_Stick stick)
            {
                isInStickNode = true;
            }
            #endregion

            #region  确认当前有点击的物体是否是Decal节点
            if (evt.target is VNode_Decal decal)
            {
                if (decal.DecalData.HasTexture)
                {
                    evt.menu.AppendSeparator();
                    evt.menu.AppendAction($"R 清空贴图", (action) =>
                    {
                        if (CurrentSelectedNodes_Decal.Count > 0)
                        {
                            for (int s = 0; s < CurrentSelectedNodes_Decal.Count; s++)
                            {
                                VNode_Decal node = CurrentSelectedNodes_Decal[s];
                                node.NodeDecalTexture_Remove();
                            }
                        }
                    });
                    evt.menu.AppendAction($"W 替换贴图", (action) =>
                    {
                        OpenObjectPickerForTextures("DecalTexSet", "t:Texture2D", decal.DecalData.DecalTexture);
                        evt.StopPropagation();
                    });
                    evt.menu.AppendAction($"F 实际尺寸", (action) =>
                    {
                        if (CurrentSelectedNodes_Decal.Count > 0)
                        {
                            Undo.RecordObject(ActionTreeAsset, "Change Decals Size");

                            for (int i = 0; i < CurrentSelectedNodes_Decal.Count; i++)
                            {
                                CurrentSelectedNodes_Decal[i].SetNativeSize();
                            }
                        }
                        evt.StopPropagation();
                    });
                    evt.menu.AppendAction($"Q 水平翻转", (action) =>
                    {
                        decal.NodeDecalTexture_Flip_H();
                        evt.StopPropagation();
                    });
                    evt.menu.AppendAction($"E 垂直翻转", (action) =>
                    {
                        decal.NodeDecalTexture_Flip_V();
                        evt.StopPropagation();
                    });
                    evt.menu.AppendAction($"X 层级置顶", (action) =>
                    {
                        decal.VisualElementBringToFront();
                        evt.StopPropagation();
                    });
                    evt.menu.AppendAction($"Z 层级置底", (action) =>
                    {
                        decal.VisualElementSendToBack();
                        evt.StopPropagation();
                    });
                }
                else
                {
                    evt.menu.AppendAction($"R 设置贴图", (action) =>
                    {
                        OpenObjectPickerForTextures("DecalTexSet", "t:Texture2D", decal.DecalData.DecalTexture);
                        evt.StopPropagation();
                    });
                }

                evt.menu.AppendSeparator();
                isInDecalNode = true;
            }
            #endregion

            if (isInGraphView)
            {
                #region 节点操作：添加节点
                evt.menu.AppendAction("A 添加节点", (action) =>
                {
                    Vector2 screenMousePosition = action.eventInfo.mousePosition + gv_GraphWindow.position.position;
                    nodeCreationRequest(new NodeCreationContext()
                    {
                        // 将当前鼠标的坐标传递给搜索框的坐标
                        screenMousePosition = screenMousePosition,
                        index = -1
                    });
                    evt.StopPropagation();
                });
                #endregion

                #region 节点操作：清空节点
                if (nodes.Count() > 0)
                {
                    evt.menu.AppendAction("Z 清空节点", (action) =>
                    {
                        ClearGraphViewContents();
                        evt.StopPropagation();
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
                    Node_Paste();
                    evt.StopPropagation();
                });
            }
            #endregion

            evt.menu.AppendSeparator();

            if (!isInGraphGroup && isInGraphNode || isInStickNode || isInDecalNode || isInVariableNode)
            {
                #region 节点操作：删除节点
                evt.menu.AppendAction("S 删除节点", param =>
                {
                    Node_Delete();
                    evt.StopPropagation();
                });
                #endregion

                #region 节点操作：节点编组
                evt.menu.AppendAction("G 节点编组", param =>
                {
                    MakeGroup("节点编组", gv_NodeCreatedPosition);
                    evt.StopPropagation();
                });
                #endregion

                #region 节点操作：克隆和复制
                if (selection.Count != 0)
                {
                    evt.menu.AppendAction("D 克隆节点", param =>
                    {
                        Node_Duplicate();
                        evt.StopPropagation();
                    });
                    evt.menu.AppendAction("C 复制节点", param =>
                    {
                        Node_Copy();
                        evt.StopPropagation();
                    });
                }
                #endregion
            }

            // 节点视觉样式设定
            if (!isInGraphGroup && isInGraphNode || isInVariableNode)
            {
                evt.menu.AppendSeparator();
                evt.menu.AppendAction($"A 通透样式", (action) =>
                {
                    if (CurrentSelectedNodes_Base.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes_Base.Count; s++)
                        {
                            VNode_Base node_a = (VNode_Base)CurrentSelectedNodes_Base[s];
                            node_a.TransparentDisplay_Set(true);
                        }
                    }
                    if (CurrentSelectedNodes_Variable.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes_Variable.Count; s++)
                        {
                            VNode_Variable node_v = (VNode_Variable)CurrentSelectedNodes_Variable[s];
                            node_v.TransparentDisplay_Set(true);
                        }
                    }
                    evt.StopPropagation();
                });
                evt.menu.AppendAction($"Q 实体样式", (action) =>
                {
                    if (CurrentSelectedNodes_Base.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes_Base.Count; s++)
                        {
                            VNode_Base node = (VNode_Base)CurrentSelectedNodes_Base[s];
                            node.TransparentDisplay_Set(false);
                        }
                    }
                    if (CurrentSelectedNodes_Variable.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes_Variable.Count; s++)
                        {
                            VNode_Variable node_v = (VNode_Variable)CurrentSelectedNodes_Variable[s];
                            node_v.TransparentDisplay_Set(false);
                        }
                    }
                    gv_GraphWindow.xw_graphView.Restructure_Nodes(ActionTreeAsset);
                    evt.StopPropagation();
                });
                evt.menu.AppendSeparator();
            }

            evt.StopPropagation();
        }

        #region 弹出物体选择面板
        // 打开物体选择器的方法
        public void OpenObjectPickerForTextures(string mode, string typefilter, Texture2D tex)
        {
            SetTextureMode = mode;

            // 动态创建 IMGUIContainer
            if (m_ObjectPickerIMGUI == null)
            {
                m_ObjectPickerIMGUI = new IMGUIContainer(OnObjectPickerGUI);
                m_ObjectPickerIMGUI.name = "---------------GraphviewTexturePicker";
                m_ObjectPickerIMGUI.style.display = DisplayStyle.Flex;
                Add(m_ObjectPickerIMGUI);
            }

            EditorGUIUtility.ShowObjectPicker<Texture2D>(tex, false, typefilter, 0);
        }

        private void OnObjectPickerGUI()
        {
            // 只处理特定事件
            if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint)
            {
                if (Event.current != null && Event.current.commandName == "ObjectSelectorClosed")
                {
                    var selectedTexture = EditorGUIUtility.GetObjectPickerObject() as Texture2D;

                    if (selectedTexture != null)
                    {
                        ApplySelectedTexture(selectedTexture);
                    }

                    SetTextureMode = null;

                    // 使用延迟调用来处理选择结果，避免在当前 GUI 调用中修改层次结构
                    if (m_ObjectPickerIMGUI != null)
                    {
                        EditorApplication.delayCall += () =>
                        {
                            Remove(m_ObjectPickerIMGUI);
                            m_ObjectPickerIMGUI = null;
                        };
                    }

                }
            }

            MarkDirtyRepaint();
        }

        // 应用选择的贴图
        private void ApplySelectedTexture(Texture2D selectedTexture)
        {
            if (SetTextureMode == "DecalTexSet")
            {
                if (CurrentSelectedNodes_Decal.Count > 0)
                {
                    foreach (var decal in CurrentSelectedNodes_Decal)
                    {
                        if (decal != null)
                        {
                            decal.DecalData.HasTexture = true;
                            decal.DecalData.DecalTexture = selectedTexture;
                            decal.CheckDecalTextureChanged();
                        }
                    }
                }
            }
            else
            {
                if (CurrentSelectedNodes_Base.Count > 0)
                {
                    foreach (var node in CurrentSelectedNodes_Base)
                    {
                        if (node is VNode_Base n_base)
                        {
                            if (n_base.ActionData.actionNodeType != "Relay")
                            {
                                if (SetTextureMode == "AvatarSet")
                                {
                                    n_base.RegisterAvatarClicked();
                                    n_base.NodeAvatar_Set(selectedTexture);
                                }
                                else if (SetTextureMode == "TitleIconSet")
                                {
                                    n_base.NodeTitleIcon_Set(selectedTexture);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}