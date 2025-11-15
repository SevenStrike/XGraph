namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Collections;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;

    public partial class xg_GraphView
    {
        /// <summary>
        /// 根据数据行为树根节点容器里的子资源来重建GraphView的视觉节点
        /// </summary>
        /// <param name="actiontree"></param>
        public void Restructure_Graph(ActionNode_Asset actiontree)
        {
            // 获取到数据根节点
            ActionTreeAsset = actiontree;

            graphViewChanged -= OnGraphViewChanged;

            // 清空数值更新回调
            ActionTreeAsset.On_VariablesValue_Changed = null;

            // 清空所有 NodeView
            DeleteElements(graphElements);

            graphViewChanged += OnGraphViewChanged;

            // 根据行为树根节点里的  -贴图-  列表数据来重建GraphView的视觉  -贴图-  节点
            Restructure_Decals(ActionTreeAsset.Decals);
            // 根据行为树根节点里的  -便签-  列表数据来重建GraphView的视觉  -便签-  节点
            Restructure_Sticks(ActionTreeAsset.Sticks);
            // 根据行为树根节点里的  -标签-  列表数据来重建GraphView的视觉  -标签-  节点
            Restructure_Labels(ActionTreeAsset.Labels);
            // 根据行为树根节点里的  -变量-  列表数据来重建GraphView的视觉  -变量-  节点
            Restructure_Variable(ActionTreeAsset.Variables);
            // 根据行为树根节点里的  -行为-  列表数据来重建GraphView的视觉  -行为-  节点
            Restructure_Actions(ActionTreeAsset.Actions);
            // 重建编组
            Restructure_Groups(ActionTreeAsset.Groups);
            // 根据行为节点里的  -黑板变量数据列表-  来重建变量与行为节点指定的连线  -“Variable类型端口”
            Restructure_VariableConnector(ActionTreeAsset.Actions);
            // 根据行为节点里的  -内部变量数据列表-  来重建内部变量与行为节点指定的连线  -“Variable类型端口”
            Restructure_InternalVariableConnector(ActionTreeAsset.Actions);
            // 更新变量值数据
            ActionTreeAsset.Variables_Refresh();

            #region 编辑器主面板UI逻辑

            // 计算并显示行为资源的保存时间差
            gv_GraphWindow.xw_GraphInfo_LastSaveDateTime_Set(ActionTreeAsset.LastSaveDateTime);
            // 行为资源的保存时间差文本颜色同步为主题色
            gv_GraphWindow.xw_GraphInfo_LastSaveLag_ColorSyncUpdate();
            // 编辑器图标的着色颜色同步为主题色
            gv_GraphWindow.xw_GraphInfo_GraphViewIcon_ColorSyncUpdate();

            // 显示行为资源路径
            gv_GraphWindow.xw_GraphInfo_PathContent_Set(AssetDatabase.GetAssetPath(gv_GraphWindow.SourceTree));

            // OptionsPanel_GraphView背景颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_Bg, ActionTreeAsset.GraphviewGridBackgroundThemes.bgcolor);

            // OptionsPanel_GraphView属性面板背景颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_bg_inspector, ActionTreeAsset.GraphviewGridBackgroundThemes.inspector_bgcolor);

            // OptionsPanel_GraphView黑板面板背景颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_bg_blackboard, ActionTreeAsset.GraphviewGridBackgroundThemes.blackboard_bgcolor);

            // OptionsPanel_GraphView网格颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_Grid, ActionTreeAsset.GraphviewGridBackgroundThemes.gridcolor);

            // OptionsPanel_GraphView分界线颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_Thickline, ActionTreeAsset.GraphviewGridBackgroundThemes.thickLinecolor);

            // OptionsPanel_GraphView背景图像颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_CustomImage, ActionTreeAsset.GraphviewGridBackgroundThemes.customimagecolor);

            // OptionsPanel_GraphView主题颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_ThemeColor, ActionTreeAsset.GraphviewGridBackgroundThemes.themecolor);

            // OptionsPanel_GraphView网格间距值设置
            util_XGraphEditorUtility.Element_FloatField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Floatfield_GridSpace, ActionTreeAsset.GraphviewGridBackgroundThemes.spacing);

            // OptionsPanel_GraphView网格分界线值设置
            util_XGraphEditorUtility.Element_IntegerField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Integerfield_ThicklineCount, ActionTreeAsset.GraphviewGridBackgroundThemes.thicklines);

            // OptionsPanel_GraphView背景图像值设置
            util_XGraphEditorUtility.Element_ObjectField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Objectfield_CustomImage, ActionTreeAsset.GraphviewGridBackgroundThemes.customimage);

            // OptionsPanel_GraphView 选择框坐标显示开关值设置
            util_XGraphEditorUtility.Element_ToggleField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Toggle_Invert_Bg_Gradient, ActionTreeAsset.GraphviewGridBackgroundThemes.InvertBgGradient);

            // GraphviewGridBackground 网格背景主题改变
            GridBackgroundThemeUpdate();

            // GraphviewGridBackground 检测是否有节点
            RecheckNodesIsExist();

            // OptionsPanel_GraphView 选择框线分段值设置
            util_XGraphEditorUtility.Element_IntegerField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Integerfield_SelectorLineSegment, ActionTreeAsset.GraphviewRectangleSelectorThemes.segments);

            // OptionsPanel_GraphView 选择框线颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_RectangleSelector, ActionTreeAsset.GraphviewRectangleSelectorThemes.rectangleSelectorLineColor);

            // GraphviewRectangleSelector 选择框主题改变
            RectangleSelectorThemeUpdate(ActionTreeAsset.GraphviewRectangleSelectorThemes);

            #endregion

            gv_GraphWindow.NodeEditorIsReady = true;
        }

        /// <summary>
        /// 根据行为树根节点里的行为列表数据来重建GraphView的视觉  -  行为节点
        /// </summary>
        /// <param name="actions"></param>
        private void Restructure_Actions(List<ActionNode_Base> actions)
        {
            // 根据根节点的数据列表  -  重建  -  内部变量节点
            // 内部变量节点优先重建 - 为了确保后续的行为节点能正常注册 On_InternalVariable_ValueChanged 回调
            foreach (var data in actions)
            {
                if (data is ActionNode_Variable)
                {
                    data.SetActionAssetRoot(ActionTreeAsset);

                    VNode_Base vNode_Base = Node_MakeAction(data.nodeGraphPosition, data);
                    vNode_Base.RefreshExpandedState();

                    // 检查头像设置情况
                    vNode_Base.CheckAvatarChanged();
                    vNode_Base.CheckTransparentDisplay(vNode_Base.ActionData.TransparentNode);
                }
            }
            // 根据根节点的数据列表  -  重建  -  行为节点
            foreach (var data in actions)
            {
                if (!(data is ActionNode_Variable))
                {
                    data.SetActionAssetRoot(ActionTreeAsset);

                    if (data.actionNodeType == "Relay")
                    {
                        VNode_Relay vNode_Relay = Node_MakeRelay(data.nodeGraphPosition, data);
                        vNode_Relay.Draw();
                        vNode_Relay.CheckTransparentDisplay(vNode_Relay.ActionData.TransparentNode);
                        vNode_Relay.RefreshExpandedState();
                    }
                    else
                    {
                        VNode_Base vNode_Base = Node_MakeAction(data.nodeGraphPosition, data);
                        vNode_Base.RefreshExpandedState();

                        // 检查头像设置情况
                        vNode_Base.CheckAvatarChanged();
                        vNode_Base.CheckTransparentDisplay(vNode_Base.ActionData.TransparentNode);
                    }
                }
            }
            // 根据行为树根节点的数据列表  -  重建  -  行为连线
            foreach (var data in actions)
            {
                if (data is ActionNode_Branch branch_node)
                {
                    VNode_Base n_branch = FindNodeView(branch_node.guid);

                    foreach (var port in n_branch.Port_Outputs)
                    {
                        if (port.Name == "开")
                        {
                            if (branch_node.childNode_true != null)
                            {
                                VNode_Base n_true = FindNodeView(branch_node.childNode_true.guid);
                                util_AnimatedEdge edge = port.Port.ConnectTo<util_AnimatedEdge>(util_XGraphEditorUtility.GetPort_WithType_OfPortList<ActionNode_Base>(n_true.Port_Inputs));
                                edge.OnUnSelectedEdge += OnUnSelectedEdge;
                                AddElement(edge);
                            }
                        }
                        else if (port.Name == "关")
                        {
                            if (branch_node.childNode_false != null)
                            {
                                VNode_Base n_false = FindNodeView(branch_node.childNode_false.guid);
                                util_AnimatedEdge edge = port.Port.ConnectTo<util_AnimatedEdge>(util_XGraphEditorUtility.GetPort_WithType_OfPortList<ActionNode_Base>(n_false.Port_Inputs));
                                edge.OnUnSelectedEdge += OnUnSelectedEdge;
                                AddElement(edge);
                            }
                        }
                    }
                }
                else
                {
                    // 获取的目标数据节点的子数据节点
                    var children = ActionTreeAsset.GetChildrenNodes(data);

                    // child 为每一个子数据节点
                    foreach (var child in children)
                    {
                        VNode_Base n_parent = FindNodeView(data.guid);
                        VNode_Base n_child = FindNodeView(child.guid);

                        foreach (var p in n_parent.Port_Outputs)
                        {
                            // 确保输出端口为 ActionNode_Base 同时不是继承于 ActionNode_Base 的 VNode_Variable_Internal 的节点类型
                            // 此处这样写是为了区分VNode_Base的行为输出端口和自定义扩展的输出端口
                            if (p.Port.portType == typeof(ActionNode_Base) && p.Port.node.GetType() != typeof(VNode_Variable_Internal))
                            {
                                Port port_start = p.Port;
                                Port port_end = util_XGraphEditorUtility.GetPort_WithType_OfPortList<ActionNode_Base>(n_child.Port_Inputs);

                                util_AnimatedEdge edge = ConnectNode(port_start, port_end);
                                edge.OnUnSelectedEdge += OnUnSelectedEdge;
                                AddElement(edge);
                            }
                        }
                    }
                }
            }
            // 根据行为树根节点的数据列表  -  检查并重建  -  行为延展
            foreach (var data in actions)
            {
                // 获取延展节点
                VNode_Base n_parent = FindNodeView(data.guid);
                // 如果是延展节点那么需要执行输入端口是否为空的检查以切换节点中的图标显示
                if (n_parent is VNode_Relay relay)
                {
                    relay.CheckConnected();
                }
            }
            // 根据行为树根节点的数据列表  -  检查并重建  -  调用重建回调
            foreach (var data in actions)
            {
                if (data.On_Node_Restructure != null)
                {
                    data.On_Node_Restructure();
                }
            }
        }

        /// <summary>
        /// 根据行为树根节点里的便签列表数据来重建GraphView的视觉  -  便签节点
        /// </summary>
        /// <param name="datas_stick"></param>
        public void Restructure_Sticks(List<ActionStickData> datas_stick)
        {
            foreach (var data in datas_stick)
            {
                Node_MakeStick(data.position, data).Draw();
            }
        }

        /// <summary>
        /// 根据行为树根节点里的便签列表数据来重建GraphView的视觉  -  标签节点
        /// </summary>
        /// <param name="datas_label"></param>
        public void Restructure_Labels(List<ActionLabelData> datas_label)
        {
            foreach (var data in datas_label)
            {
                Node_MakeLabel(data.position, data).Draw();
            }
        }

        /// <summary>
        /// 根据行为树根节点里的贴图列表数据来重建GraphView的视觉  -  贴图节点
        /// </summary>
        /// <param name="ActionDecalData"></param>
        public void Restructure_Decals(List<ActionDecalData> datas_decal)
        {
            foreach (var data in datas_decal)
            {
                VNode_Decal vNode_Decal = Node_MakeDecal(data.position, data).Draw();
                // 检查头像设置情况
                vNode_Decal.CheckDecalTextureChanged();
            }
        }

        /// <summary>
        /// 根据行为树根节点里的变量列表数据来重建GraphView的视觉  -  黑板变量节点
        /// </summary>
        /// <param name="datas_var"></param>
        public void Restructure_Variable(List<ActionVariableData> datas_var)
        {
            foreach (var data in datas_var)
            {
                data.name = ActionTreeAsset.Variable_GetVarSource(data.varguid).name;
                VNode_Variable vNode_Variable = Node_MakeVariable(data.position, data);
                vNode_Variable.Draw();
                vNode_Variable.CheckTransparentDisplay(vNode_Variable.VariableData.TransparentNode);
                vNode_Variable.RefreshExpandedState();
            }
        }

        /// <summary>
        /// 根据行为树根节点里的编组列表数据来重建GraphView的视觉  -  编组
        /// </summary>
        /// <param name="datas_group"></param>
        public void Restructure_Groups(List<ActionGroupData> datas_group)
        {
            if (datas_group == null || datas_group.Count == 0) return;

            foreach (var groupData in datas_group)
            {
                // 初始化编组
                Group group = CreateGroup(groupData.name, groupData.pos, groupData);

                // 添加编组到GraphView
                AddElement(group);

                // 遍历编组中的节点GUID，找到对应的节点并添加到编组中
                foreach (string guid in groupData.guids)
                {
                    // 查找 - 行为节点
                    var action = nodes.ToList().FirstOrDefault(n => n is VNode_Base node_action && node_action.ActionData.guid == guid);
                    if (action != null)
                    {
                        group.AddElement(action);
                        continue;
                    }

                    // 查找 - 黑板变量节点
                    var stick = nodes.ToList().FirstOrDefault(n => n is VNode_Stick node_stick && node_stick.StickData.guid == guid);
                    if (stick != null)
                    {
                        group.AddElement(stick);
                    }

                    // 查找 - 标签节点
                    var label = nodes.ToList().FirstOrDefault(n => n is VNode_Label node_label && node_label.LabelData.guid == guid);
                    if (label != null)
                    {
                        group.AddElement(label);
                    }

                    // 查找 - 贴图节点
                    var decal = nodes.ToList().FirstOrDefault(n => n is VNode_Decal node_decal && node_decal.DecalData.guid == guid);
                    if (decal != null)
                    {
                        group.AddElement(decal);
                    }

                    // 查找 - 黑板变量节点
                    var vare = nodes.ToList().FirstOrDefault(n => n is VNode_Variable node_var && node_var.VariableData.guid == guid);
                    if (vare != null)
                    {
                        group.AddElement(vare);
                    }
                }
            }
        }

        /// <summary>
        /// 根据所有行为节点的 “黑板变量数据列表”来重建 “黑板变量节点”与行为节点的连线
        /// </summary>
        /// <param name="datas_action"></param>
        public void Restructure_VariableConnector(List<ActionNode_Base> datas_action)
        {
            // 根据行为树根节点的数据列表  -  重建与每个行为数据中指定的 VariableGuid 所对应的Variable节点连线
            foreach (var action in datas_action)
            {
                if (action.VariableDatas != null && action.VariableDatas.Count > 0)
                {
                    // 父节点
                    VNode_Base n_parent = FindNodeView(action.guid);

                    for (int i = 0; i < action.VariableDatas.Count; i++)
                    {
                        var item = action.VariableDatas[i];

                        // 在节点图内找到目标变量节点与行为节点的匹配端口连接起来
                        VNode_Variable n_var = FindNode(item.VariableNodeGuid) as VNode_Variable;
                        if (n_var != null)
                        {
                            // 获取变量节点的变量类型
                            Type type = n_var.VariableData.variable.GetType();

                            // 父节点存在的变量端口
                            Port port_parent = n_parent.GetPort(type, item.TargetPortName, PortStyleType.In);

                            if (n_var != null && port_parent != null)
                            {
                                util_AnimatedEdge edge = n_var.OutputPort.Port.ConnectTo<util_AnimatedEdge>(port_parent);
                                edge.OnUnSelectedEdge += OnUnSelectedEdge;

                                AddElement(edge);
                            }
                        }
                        else
                        {
                            action.VariableDatas.Remove(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据所有行为节点的 “内部变量数据列表”来重建 “内部变量节点”与行为节点的连线
        /// </summary>
        /// <param name="datas_action"></param>
        public void Restructure_InternalVariableConnector(List<ActionNode_Base> datas_action)
        {
            foreach (var action in datas_action)
            {
                // 首先确保内部变量列表不为空
                if (action.InternalVariableDatas != null && action.InternalVariableDatas.Count > 0)
                {
                    // 获得父节点
                    VNode_Base n_parent = FindNodeView(action.guid);

                    // 循环遍历内部变量列表
                    for (int i = 0; i < action.InternalVariableDatas.Count; i++)
                    {
                        // 获取列表其中一项
                        var item = action.InternalVariableDatas[i];

                        // 在节点图内找到目标  -  内部变量节点  -  与  -  行为节点  -  的  -  匹配端口  -  连接
                        VNode_Variable_Internal n_var = FindNode(item.VariableNodeGuid) as VNode_Variable_Internal;
                        if (n_var != null)
                        {
                            n_var.VariableData.variable = item.variable.Clone(false);
                            n_var.VariableData.variable.name = "";
                            n_var.VariableNodeFieldValueUpdate();

                            // 获取变量节点的变量类型
                            Type type = n_var.VariableData.variable.GetType();

                            // 父节点存在的变量端口
                            Port port_parent = n_parent.GetPort(type, item.TargetPortName, PortStyleType.In);

                            if (n_var != null && port_parent != null)
                            {
                                util_AnimatedEdge edge = n_var.Port_Outputs.First().Port.ConnectTo<util_AnimatedEdge>(port_parent);
                                edge.OnUnSelectedEdge += OnUnSelectedEdge;
                                AddElement(edge);
                            }
                        }
                        else
                        {
                            action.InternalVariableDatas.Remove(item);
                        }
                    }
                }
            }
        }
    }
}