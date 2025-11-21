namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public partial class xg_GraphView
    {
        /// <summary>
        /// 初始化编组并注册相关事件
        /// </summary>
        private Group CreateGroup(string title, Vector2 position, xGroupData groupData = null)
        {
            Group group = new Group
            {
                title = title
            };
            group.SetPosition(new Rect(position, Vector2.zero));

            group.AddToClassList("group_main");

            // 获取Group主体元素
            VisualElement group_element_header = group.Q<VisualElement>("headerContainer");
            group_element_header.pickingMode = PickingMode.Position;
            group_element_header.AddToClassList("headcontainer");

            #region Logo
            Label icon = new Label("");
            icon.name = "groupicon";
            icon.AddToClassList("Title_Icon");
            icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/group.png");
            icon.style.unityBackgroundImageTintColor = Color.black * 0.85f;
            icon.pickingMode = PickingMode.Ignore;
            #endregion

            group_element_header.Add(icon);
            icon.SendToBack();

            // 获取Group标题容器元素
            VisualElement group_element_titleContainer = group_element_header.Q<VisualElement>("titleContainer");
            group_element_titleContainer.AddToClassList("groupTitleContainer");

            // 获取Group内容容器元素
            VisualElement group_element_content = group.Q<VisualElement>("centralContainer");
            // 获取Group内容容器元素
            VisualElement group_element_contentContainerPlaceholder = group_element_content.Q<VisualElement>("contentContainerPlaceholder");
            group_element_contentContainerPlaceholder.AddToClassList("contentContainer");

            // 获取标题Label并注册改变内容事件
            Label group_element_title = group.Q<Label>("titleLabel");
            group_element_title.AddToClassList("group_title");

            // 应用配置文件的颜色到编组标题的文字颜色和背景颜色
            foreach (var colorData in NodeThemesList.Group)
            {
                if (colorData.solution == groupData.solution)
                {
                    group_element_header.style.backgroundColor = util_XGraphEditorUtility.Color_From_HexString(colorData.title_bg_color);
                    group_element_content.style.backgroundColor = util_XGraphEditorUtility.Color_From_HexString(colorData.content_bg_color);
                    group_element_title.style.color = util_XGraphEditorUtility.Color_From_HexString(colorData.title_text_color);
                    icon.style.unityBackgroundImageTintColor = util_XGraphEditorUtility.Color_From_HexString(colorData.logo_color);
                }
            }

            // 绑定 Group 右键菜单
            group_element_header.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendSeparator();
                // 菜单 - 主题色切换
                for (int i = 0; i < NodeThemesList.Group.Count; i++)
                {
                    ThemeData_Group dat = NodeThemesList.Group[i];
                    evt.menu.AppendAction($"T 编组配色/{dat.solution}", d =>
                    {
                        Undo.RecordObject(ActionTreeAsset, "Set Group Color");

                        if (CurrentSelectedGroups.Count > 1)
                        {
                            for (int i = 0; i < CurrentSelectedGroups.Count; i++)
                            {
                                Group gp = CurrentSelectedGroups[i];

                                // 同步修改行为树根节点中的对应的编组数据的配色方案
                                foreach (var data in ActionTreeAsset.Groups)
                                {
                                    if (data.group == gp)
                                    {
                                        data.solution = dat.solution;
                                    }
                                }
                                SetGroupTheme(
                                    gp, util_XGraphEditorUtility.Color_From_HexString(dat.title_bg_color),
                                    util_XGraphEditorUtility.Color_From_HexString(dat.content_bg_color),
                                    util_XGraphEditorUtility.Color_From_HexString(dat.title_text_color),
                                    util_XGraphEditorUtility.Color_From_HexString(dat.logo_color));
                            }
                        }
                        else
                        {
                            groupData.solution = dat.solution;
                            SetGroupTheme(
                                group_element_header, group_element_content, group_element_title, icon,
                                util_XGraphEditorUtility.Color_From_HexString(dat.title_bg_color),
                                util_XGraphEditorUtility.Color_From_HexString(dat.content_bg_color),
                                util_XGraphEditorUtility.Color_From_HexString(dat.title_text_color),
                                util_XGraphEditorUtility.Color_From_HexString(dat.logo_color));
                        }
                    });
                }
                evt.StopPropagation();
            }));

            group_element_title.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                ChangeGroupName(group, group_element_title.text);
                evt.StopPropagation();
            });

            // 初始化状态跟踪
            CurrentCreatedGroups[group] = new HashSet<object>();

            // 关联Group和NodeGroupData
            if (groupData != null)
            {
                groupData.group = group;
                groupData.groupcontainer = group.Q<VisualElement>("contentContainerPlaceholder");
            }

            // 刷新 BlackBoard 信息显示
            gv_GraphWindow.xw_BlackBoard_UpdateTitleInfo();

            VisualElement group_hignlighter = new VisualElement();
            group_hignlighter.name = "gp_highlighter";
            group_hignlighter.AddToClassList("group_highlighter");
            group_hignlighter.pickingMode = PickingMode.Ignore;
            util_XGraphEditorUtility.Element_Opacity_Set(group_hignlighter, 0);
            group.Add(group_hignlighter);

            return group;
        }
        /// <summary>
        /// 创建节点编组
        /// </summary>     
        private void MakeGroup(string title, Vector2 localMousePosition)
        {
            var selectedNodes = selection.OfType<Node>().ToList();

            // 所有选中的节点的Guid，用于收录到编组数据guids中
            List<string> nodes_guid = new List<string>();

            // 收集选中节点的GUID
            foreach (var n in selectedNodes)
            {
                if (n is xNode_Base node)
                {
                    nodes_guid.Add(node.ActionData.guid);
                }
                else if (n is xNode_Variable vare)
                {
                    nodes_guid.Add(vare.VariableData.guid);
                }
                else if (n is xNode_Stick stick)
                {
                    nodes_guid.Add(stick.StickData.guid);
                }
                else if (n is xNode_Label label)
                {
                    nodes_guid.Add(label.LabelData.guid);
                }
                else if (n is xNode_Decal decal)
                {
                    nodes_guid.Add(decal.DecalData.guid);
                }
            }

            // 创建编组数据
            xGroupData gp_data = new xGroupData(title, GUID.Generate().ToString(), localMousePosition, nodes_guid, "M 默认", null, null);

            // 初始化编组
            Group gp = CreateGroup(title, localMousePosition, gp_data);

            // 添加选中节点到编组
            foreach (var n in selectedNodes)
            {
                gp.AddElement(n);
                CheckHasAvatarNode(gp_data);
            }

            Undo.RecordObject(ActionTreeAsset, "Create Group");
            // 添加编组到行为树数据
            ActionTreeAsset.NodeGroup_Add(gp_data);

            // 添加编组到GraphView
            AddElement(gp);
        }
        /// <summary>
        /// 同步编组 - 名称
        /// </summary>
        /// <param solution="group"></param>
        /// <param solution="newName"></param>
        private void ChangeGroupName(Group group, string newName)
        {
            xGroupData groupData = ActionTreeAsset.Groups
                .FirstOrDefault(g => g.group == group);

            if (groupData != null && groupData.name != newName)
            {
#if UNITY_EDITOR
                Undo.RecordObject(ActionTreeAsset, "Rename Group");
#endif
                groupData.name = newName;

                //#if UNITY_EDITOR
                //                EditorUtility.SetDirty(ActionTreeAsset);
                //#endif
            }
        }
        /// <summary>
        /// 检查编组中是否存在节点
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool HasNodes(Group group)
        {
            return group.containedElements.OfType<Node>().Any();
        }
        /// <summary>
        /// 检查编组下的节点是否包含AvaterSet
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool HasMarkIconNodes(Group group)
        {
            IEnumerable<Node> nodes = group.containedElements.OfType<Node>();
            bool hasmark = false;
            foreach (var node in nodes)
            {
                if (node is xNode_Base bs)
                {
                    if (bs.ActionData.actionNodeType != "Relay" && bs.ActionData.HasAvatar)
                    {
                        hasmark = true;
                        break;
                    }
                }
            }
            return hasmark;
        }
        /// <summary>
        /// 清空所有视觉编组（保留组内节点）
        /// </summary>
        internal void Groups_Clear(bool ClearGroupData = true)
        {
            // 获取所有编组
            var groups = graphElements.OfType<Group>().ToList();

            if (groups.Count == 0) return;

            // 遍历所有编组
            foreach (var group in groups)
            {
                // 保存所有子节点位置
                var childPositions = group.containedElements
                    .OfType<GraphElement>()
                    .ToDictionary(e => e, e => e.GetPosition());

                // 解除所有父子关系（但不删除子元素）
                foreach (var child in group.containedElements.ToList())
                {
                    group.RemoveElement(child);
                }

                // 从GraphView移除编组（此时已是空组）
                RemoveElement(group);

                // 恢复子节点位置（防止Unity自动调整）
                foreach (var kvp in childPositions)
                {
                    kvp.Key.SetPosition(kvp.Value);
                }

                // 从状态跟踪字典中移除
                if (CurrentCreatedGroups.ContainsKey(group))
                {
                    CurrentCreatedGroups.Remove(group);
                }
            }

            // 清空行为树中的编组数据
            if (ClearGroupData && ActionTreeAsset != null)
            {
                ActionTreeAsset.Groups.Clear();
            }
        }
        /// <summary>
        /// 安全删除Group（保留内部节点）
        /// </summary>
        private void DeleteGroup(Group group)
        {
            if (group == null) return;

#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(ActionTreeAsset, "Delete Group");
#endif

            // 1. 保存所有子节点位置
            var childPositions = group.containedElements
                .OfType<GraphElement>()
                .ToDictionary(e => e, e => e.GetPosition());

            // 2. 解除所有父子关系（但不删除子元素）
            foreach (var child in group.containedElements.ToList())
            {
                group.RemoveElement(child);
            }

            // 3. 删除Group数据
            var groupData = ActionTreeAsset.Groups.FirstOrDefault(g => g.group == group);
            if (groupData != null)
            {
                ActionTreeAsset.NodeGroup_Remove(groupData);
            }

            // 4. 从GraphView移除Group（此时已是空组）
            RemoveElement(group);

            // 5. 恢复子节点位置（防止Unity自动调整）
            foreach (var kvp in childPositions)
            {
                kvp.Key.SetPosition(kvp.Value);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(ActionTreeAsset);
#endif
        }
        /// <summary>
        /// 收集所有编组的位置信息
        /// </summary>
        private void CollectGroupsPosition()
        {
            foreach (var g in ActionTreeAsset.Groups)
            {
                g.pos = g.group.GetPosition().position;
            }
        }
        /// <summary>
        /// 根据group寻找GroupData
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public xGroupData FindGroupData(Group group)
        {
            return ActionTreeAsset.Groups.FirstOrDefault(g => g.group == group);
        }
        /// <summary>
        /// 检查编组内是否存在设置了头像的节点
        /// </summary>
        /// <param name="groupData"></param>
        private void CheckHasAvatarNode(xGroupData groupData)
        {
            // 检查编组中是否存在设置了Avatar的节点
            groupData.hasAvatarNodes = HasMarkIconNodes(groupData.group);

            // 如果有Avatar的节点那么编组内边距为60，否则为15
            if (groupData.hasAvatarNodes)
            {
                groupData.groupcontainer.style.paddingTop = 60;
            }
            else
            {
                groupData.groupcontainer.style.paddingTop = 15;
            }
        }

        #region 节点放入或取出编组的回调
        /// <summary>
        /// 当节点拖入编组
        /// </summary>
        /// <param name="group"></param>
        /// <param name="nodes"></param>
        public void On_Group_AddedElements(Group group, IEnumerable<GraphElement> nodes)
        {
            // 查找对应的 ActionGroupData
            xGroupData groupData = FindGroupData(group);
            Undo.RecordObject(ActionTreeAsset, "Group AddedElements");

            // ---------------------处理  移入  的节点
            foreach (var item in nodes)
            {
                string guid = null;
                if (item is xNode_Base node)
                {
                    // 获取移入的节点的guid
                    guid = node.ActionData.guid;

                    // 注册委托 - 节点头像设置
                    node.ActionData.On_Node_AvatarChanged += ((n) =>
                    {
                        // 检查编组内是否存在设置了头像的节点
                        CheckHasAvatarNode(groupData);
                    });
                }
                else if (item is xNode_Variable vare)
                {
                    // 获取移入的节点的guid
                    guid = vare.VariableData.guid;
                }
                else if (item is xNode_Stick stick)
                {
                    // 获取移入的节点的guid
                    guid = stick.StickData.guid;
                }
                else if (item is xNode_Label label)
                {
                    // 获取移入的节点的guid
                    guid = label.LabelData.guid;
                }
                else if (item is xNode_Decal decal)
                {
                    // 获取移入的节点的guid
                    guid = decal.DecalData.guid;
                }
                // 不重复的节点Guids数据加入
                if (!string.IsNullOrEmpty(guid) && !groupData.guids.Contains(guid))
                {
                    groupData.guids.Add(guid);
                }
            }
            CheckHasAvatarNode(groupData);
        }
        /// <summary>
        /// 当节点移出编组
        /// </summary>
        /// <param name="group"></param>
        /// <param name="nodes"></param>
        public void On_Group_RemoveElements(Group group, IEnumerable<GraphElement> nodes)
        {
            // 查找对应的 ActionGroupData
            xGroupData groupData = FindGroupData(group);
            Undo.RecordObject(ActionTreeAsset, "Group RemovedElements");

            // ---------------------处理  移出  的节点
            foreach (var item in nodes)
            {
                string guid = null;
                if (item is xNode_Base node)
                {
                    // 获取移出的节点的guid
                    guid = node.ActionData.guid;

                    // 清空委托 - 节点头像移除
                    node.ActionData.On_Node_AvatarChanged = null;
                }
                else if (item is xNode_Variable vare)
                {
                    // 获取移出的节点的guid
                    guid = vare.VariableData.guid;
                }
                else if (item is xNode_Stick stick)
                {
                    // 获取移出的节点的guid
                    guid = stick.StickData.guid;
                }
                else if (item is xNode_Label label)
                {
                    // 获取移出的节点的guid
                    guid = label.LabelData.guid;
                }
                else if (item is xNode_Decal decal)
                {
                    // 获取移出的节点的guid
                    guid = decal.DecalData.guid;
                }
                // 包含节点Guids数据移除
                if (!string.IsNullOrEmpty(guid) && groupData.guids.Contains(guid))
                {
                    groupData.guids.Remove(guid);
                }
            }
            // 检查编组内是否存在设置了头像的节点
            CheckHasAvatarNode(groupData);
        }
        #endregion

        #region 设置编组主题配色
        public void SetGroupTheme(Group gp, Color title_bg_color, Color content_bg_color, Color title_text_color, Color logo_color)
        {
            gp.Q<VisualElement>("headerContainer").style.backgroundColor = title_bg_color;
            gp.Q<VisualElement>("centralContainer").style.backgroundColor = content_bg_color;
            gp.Q<Label>("titleLabel").style.color = title_text_color;
            gp.Q<Label>("groupicon").style.unityBackgroundImageTintColor = logo_color;
        }
        public void SetGroupTheme(VisualElement header, VisualElement content, VisualElement title, VisualElement icon, Color title_bg_color, Color content_bg_color, Color title_text_color, Color logo_color)
        {
            header.style.backgroundColor = title_bg_color;
            content.style.backgroundColor = content_bg_color;
            title.style.color = title_text_color;
            icon.style.unityBackgroundImageTintColor = logo_color;
        }
        #endregion

        #region 查找匹配的主题配色
        public ThemeData_Group FindGroupTheme(string solution)
        {
            ThemeData_Group t_gp = new ThemeData_Group();

            for (int i = 0; i < NodeThemesList.Group.Count; i++)
            {
                ThemeData_Group dat = NodeThemesList.Group[i];
                if (dat.solution == solution)
                {
                    t_gp = dat;
                    break;
                }
            }

            return t_gp;
        }
        #endregion

        #region 高亮编组
        /// <summary>
        /// 高亮编组显示
        /// </summary>        
        public void Group_Highlight(Group group, Color col)
        {
            VisualElement gp_highlighter = group.Q<VisualElement>("gp_highlighter");
            util_XGraphEditorUtility.Element_BorderColor_Set(gp_highlighter, col);
            util_XGraphEditorUtility.Element_Opacity_Set(gp_highlighter, 1);
        }
        /// <summary>
        /// 取消高亮编组显示
        /// </summary>
        public void Group_UnHighlight(Group group)
        {
            VisualElement gp_highlighter = group.Q<VisualElement>("gp_highlighter");
            util_XGraphEditorUtility.Element_Opacity_Set(gp_highlighter, 0);
        }
        #endregion
    }
}