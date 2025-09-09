namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public partial class xg_GraphView
    {
        /// <summary>
        /// 根据数据行为树根节点容器里的子资源来重建GraphView的视觉节点
        /// </summary>
        /// <param name="actiontree"></param>
        public void Restructure_Nodes(ActionNode_Asset actiontree)
        {
            // 获取到数据根节点
            ActionTreeAsset = actiontree;

            graphViewChanged -= OnGraphViewChanged;

            // 清空所有 NodeView
            DeleteElements(graphElements);

            graphViewChanged += OnGraphViewChanged;

            // 根据根节点的数据列表  -  重建 Nodes
            ActionTreeAsset.ActionNodes.ForEach(data =>
            {
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
                    vNode_Base.Draw();
                    vNode_Base.RefreshExpandedState();

                    // 检查头像设置情况
                    vNode_Base.CheckAvatarChanged();
                    vNode_Base.CheckTransparentDisplay(vNode_Base.ActionData.TransparentNode);
                }
            });

            // 根据行为树根节点的数据列表  -  重建 Edges
            ActionTreeAsset.ActionNodes.ForEach(d =>
            {
                // 获取的目标数据节点的子数据节点
                var children = ActionTreeAsset.GetChildrenNodes(d);
                // c 为每一个子数据节点
                children.ForEach(c =>
                {
                    VNode_Base n_parent = FindNodeView(d.guid);
                    VNode_Base n_child = FindNodeView(c.guid);

                    n_parent.Port_Outputs.ForEach(p =>
                    {
                        Edge edge = p.Port.ConnectTo(n_child.Port_Input.Port);
                        AddElement(edge);
                    });
                });
            });

            // 根据行为树根节点的数据列表  -  检查 所有Relay的连线状态
            ActionTreeAsset.ActionNodes.ForEach(d =>
            {
                // 获取延展节点
                VNode_Base n_parent = FindNodeView(d.guid);
                // 如果是延展节点那么需要执行输入端口是否为空的检查以切换节点中的图标显示
                if (n_parent is VNode_Relay relay)
                {
                    relay.CheckConnected();
                }
            });

            // 根据行为树根节点里的便签列表数据来重建GraphView的视觉便签节点
            Restructure_Sticks(ActionTreeAsset.StickNoteDatas);

            // 根据行为树根节点里的贴图列表数据来重建GraphView的视觉贴图节点
            Restructure_Decals(ActionTreeAsset.DecalDatas);

            // 重建编组
            Restructure_Groups(ActionTreeAsset.NodeGroupDatas);

            #region 编辑器主面板UI逻辑

            // 计算并显示行为资源的保存时间差
            gv_GraphWindow.xw_GraphInfo_LastSaveDateTime_Set(ActionTreeAsset.LastSaveDateTime);

            // 显示行为资源路径
            gv_GraphWindow.xw_GraphInfo_PathContent_Set(AssetDatabase.GetAssetPath(gv_GraphWindow.SourceTree));

            // OptionsPanel_GraphView背景颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_Bg, ActionTreeAsset.GraphviewGridBackgroundThemes.bgcolor);

            // OptionsPanel_GraphView网格颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_Grid, ActionTreeAsset.GraphviewGridBackgroundThemes.gridcolor);

            // OptionsPanel_GraphView分界线颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_Thickline, ActionTreeAsset.GraphviewGridBackgroundThemes.thickLinecolor);

            // OptionsPanel_GraphView背景图像颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_CustomImage, ActionTreeAsset.GraphviewGridBackgroundThemes.customimagecolor);

            // OptionsPanel_GraphView网格间距值设置
            util_XGraphEditorUtility.Element_FloatField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Floatfield_GridSpace, ActionTreeAsset.GraphviewGridBackgroundThemes.spacing);

            // OptionsPanel_GraphView网格分界线值设置
            util_XGraphEditorUtility.Element_IntegerField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Integerfield_ThicklineCount, ActionTreeAsset.GraphviewGridBackgroundThemes.thicklines);

            // OptionsPanel_GraphView背景图像值设置
            util_XGraphEditorUtility.Element_ObjectField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Objectfield_CustomImage, ActionTreeAsset.GraphviewGridBackgroundThemes.customimage);

            // GraphviewGridBackground 网格背景主题改变
            GridBackgroundThemeUpdate();

            // OptionsPanel_GraphView 选择框坐标显示开关值设置
            util_XGraphEditorUtility.Element_ToggleField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Toggle_DisplaySelectorCoordinate, ActionTreeAsset.GraphviewRectangleSelectorThemes.displayCoordinate);

            // OptionsPanel_GraphView 选择框线分段值设置
            util_XGraphEditorUtility.Element_IntegerField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Integerfield_SelectorLineSegment, ActionTreeAsset.GraphviewRectangleSelectorThemes.segments);

            // OptionsPanel_GraphView 选择框线颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(gv_GraphWindow.xw_OptionsPanel_Colorfield_RectangleSelector, ActionTreeAsset.GraphviewRectangleSelectorThemes.rectangleSelectorLineColor);

            // GraphviewRectangleSelector 选择框主题改变
            RectangleSelectorThemeUpdate(ActionTreeAsset.GraphviewRectangleSelectorThemes);

            #endregion
        }

        /// <summary>
        /// 根据行为树根节点里的便签列表数据来重建GraphView的视觉便签节点
        /// </summary>
        /// <param name="ActionDecalData"></param>
        public void Restructure_Sticks(List<ActionStickData> stickdata)
        {
            // 根据根节点的数据列表重建 NodeViews
            stickdata.ForEach(data =>
            {
                Node_MakeStick(data.position, data).Draw();
            });
        }

        /// <summary>
        /// 根据行为树根节点里的贴图列表数据来重建GraphView的视觉贴图节点
        /// </summary>
        /// <param name="ActionDecalData"></param>
        public void Restructure_Decals(List<ActionDecalData> decaldata)
        {
            // 根据根节点的数据列表重建 NodeViews
            decaldata.ForEach(data =>
            {
                VNode_Decal vNode_Decal = Node_MakeDecal(data.position, data).Draw();
                // 检查头像设置情况
                vNode_Decal.CheckDecalTextureChanged();
            });
        }

        /// <summary>
        /// 根据行为树根节点里的编组列表数据来重建GraphView的视觉编组
        /// </summary>
        public void Restructure_Groups(List<groupdata> groupDatas)
        {
            if (groupDatas == null || groupDatas.Count == 0) return;

            foreach (var groupData in groupDatas)
            {
                // 初始化编组
                Group group = CreateGroup(groupData.name, groupData.pos, groupData);

                // 添加编组到GraphView
                AddElement(group);

                // 遍历编组中的节点GUID，找到对应的节点并添加到编组中
                foreach (string guid in groupData.guids)
                {
                    // 查找 - 行为节点
                    var node = nodes.ToList().FirstOrDefault(n =>
                        n is VNode_Base baseNode && baseNode.ActionData.guid == guid);

                    if (node != null)
                    {
                        group.AddElement(node);
                        continue;
                    }

                    // 查找 - 便签节点
                    var stickNote = nodes.ToList().FirstOrDefault(n =>
                        n is VNode_Stick stickNode && stickNode.StickData.guid == guid);

                    if (stickNote != null)
                    {
                        group.AddElement(stickNote);
                    }

                    // 查找 - 贴图节点
                    var decalNote = nodes.ToList().FirstOrDefault(n =>
                        n is VNode_Decal decalNote && decalNote.DecalData.guid == guid);

                    if (decalNote != null)
                    {
                        group.AddElement(decalNote);
                    }
                }
            }
        }
    }
}