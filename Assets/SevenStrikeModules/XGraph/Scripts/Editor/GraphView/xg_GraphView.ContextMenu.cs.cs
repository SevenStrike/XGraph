/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGraph 行为流程图插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
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
            if (evt.target is xNode_Base nodebase)
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
                    xNode_Base node = (xNode_Base)CurrentSelectedNodes_Base.First();
                    var defaultColor = Color.gray;
                    defaultColor = node.ActionData.themeColor;
                    defaultColor.a = 1.0f;
                    #endregion

                    void ApplyColor(Color pickedColor)
                    {
                        foreach (var selectable in selection)
                        {
                            if (selectable is xNode_Base node)
                            {
                                Undo.RecordObject(node.ActionData, "Change NodeThemeColor");

                                // 将主题方案名称赋值
                                node.ActionData.themeSolution = "自定义";

                                // 将主题方案名称赋值
                                node.ActionData.themeColor = pickedColor;

                                // 调用节点主题色改变回调
                                if (node.ActionData.On_Node_ThemeColorChanged != null)
                                    node.ActionData.On_Node_ThemeColorChanged();
                            }
                        }
                    }
                    m.Invoke(null, new object[] { (Action<Color>)ApplyColor, defaultColor, true, false });
                });

                // 菜单 - 主题色切换
                for (int i = 0; i < NodeThemesList.Node.Count; i++)
                {
                    ThemeData_Node dat = NodeThemesList.Node[i];
                    evt.menu.AppendAction($"T 节点配色/{dat.solution}", (action) =>
                    {
                        if (CurrentSelectedNodes_Base.Count > 0)
                        {
                            for (int s = 0; s < CurrentSelectedNodes_Base.Count; s++)
                            {
                                xNode_Base node = (xNode_Base)CurrentSelectedNodes_Base[s];

                                Undo.RecordObject(node.ActionData, "Change NodeThemeColor");

                                // 将主题方案名称赋值
                                node.ActionData.themeSolution = dat.solution;

                                // 根据主题方案名称的对应颜色Hex转换为Color赋值给主题色变量
                                node.ActionData.themeColor = util_XGraphEditorUtility.Color_From_HexString(dat.nodecolor);

                                // 调用节点主题色改变回调
                                if (node.ActionData.On_Node_ThemeColorChanged != null)
                                    node.ActionData.On_Node_ThemeColorChanged();
                            }
                        }
                    });
                }

                evt.menu.AppendSeparator();

                if (evt.target is not xNode_Property)
                {
                    // 执行模式切换
                    evt.menu.AppendAction($"E 执行模式/E 顺序", (action) =>
                    {
                        if (CurrentSelectedNodes_Base.Count > 0)
                        {
                            for (int s = 0; s < CurrentSelectedNodes_Base.Count; s++)
                            {
                                xNode_Base node = (xNode_Base)CurrentSelectedNodes_Base[s];
                                node.ActionData.isConcurrentExecution = false;

                                if (node.ActionData.On_Node_ConcurrentChanged != null)
                                    node.ActionData.On_Node_ConcurrentChanged(false);
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
                                xNode_Base node = (xNode_Base)CurrentSelectedNodes_Base[s];
                                node.ActionData.isConcurrentExecution = true;

                                if (node.ActionData.On_Node_ConcurrentChanged != null)
                                    node.ActionData.On_Node_ConcurrentChanged(true);
                            }
                        }
                        evt.StopPropagation();
                    });
                }

                if (nodebase is not xNode_Variable_Internal && evt.target is not xNode_Property)
                {
                    if (CurrentSelectedNodes_Base.Count == 1)
                    {
                        evt.menu.AppendAction($"X 设为起始节点", (action) =>
                        {
                            xNode_Base node = (xNode_Base)CurrentSelectedNodes_Base[0];

                            ActionTreeAsset.SetStartNode(node.ActionData);

                            evt.StopPropagation();
                        });
                    }
                }

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
                                    xNode_Base node = (xNode_Base)CurrentSelectedNodes_Base[s];
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
                    evt.menu.AppendAction($"B 图标/B 设置标题图标", (action) =>
                    {
                        OpenObjectPickerForTextures("TitleIconSet", "t:Texture2D", nodebase.ActionData.NodeIcon);
                        evt.StopPropagation();
                    });
                    evt.menu.AppendAction($"B 图标/W 恢复原生图标", (action) =>
                    {
                        if (CurrentSelectedNodes_Base.Count > 0)
                        {
                            for (int s = 0; s < CurrentSelectedNodes_Base.Count; s++)
                            {
                                xNode_Base node = (xNode_Base)CurrentSelectedNodes_Base[s];
                                Undo.RecordObject(node.ActionData, "Restore ActionNodeTitleIcon");
                                node.ActionData.NodeIcon = null;

                                if (node.ActionData.On_Node_IconChanged != null)
                                    node.ActionData.On_Node_IconChanged(null);
                            }
                        }
                        evt.StopPropagation();
                    });
                }
                evt.menu.AppendSeparator();
                isInGraphNode = true;
            }
            #endregion

            #region  确认当前有点击的物体是否是 VariableNode
            if (evt.target is xNode_Variable nodevar)
            {
                evt.menu.AppendAction($"C 查看变量名称", (action) =>
                {
                    if (CurrentSelectedNodes_Variable.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes_Variable.Count; s++)
                        {
                            xNode_Variable node = (xNode_Variable)CurrentSelectedNodes_Variable[s];
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
            if (evt.target is xNode_Stick stick)
            {
                isInStickNode = true;
            }
            #endregion

            #region  确认当前有点击的物体是否是Decal节点
            if (evt.target is xNode_Decal decal)
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
                                xNode_Decal node = CurrentSelectedNodes_Decal[s];
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
                            xNode_Base node_a = (xNode_Base)CurrentSelectedNodes_Base[s];
                            node_a.TransparentDisplay_Set(true);
                        }
                    }
                    if (CurrentSelectedNodes_Variable.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes_Variable.Count; s++)
                        {
                            xNode_Variable node_v = (xNode_Variable)CurrentSelectedNodes_Variable[s];
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
                            xNode_Base node = (xNode_Base)CurrentSelectedNodes_Base[s];
                            node.TransparentDisplay_Set(false);
                        }
                    }
                    if (CurrentSelectedNodes_Variable.Count > 0)
                    {
                        for (int s = 0; s < CurrentSelectedNodes_Variable.Count; s++)
                        {
                            xNode_Variable node_v = (xNode_Variable)CurrentSelectedNodes_Variable[s];
                            node_v.TransparentDisplay_Set(false);
                        }
                    }
                    gv_GraphWindow.xw_graphView.Restructure_Graph(ActionTreeAsset);
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
                        if (node is xNode_Base n_base)
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
                                    if (n_base.ActionData.On_Node_IconChanged != null)
                                        n_base.ActionData.On_Node_IconChanged(selectedTexture);
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