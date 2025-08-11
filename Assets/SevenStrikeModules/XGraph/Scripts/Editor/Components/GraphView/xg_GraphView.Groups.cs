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
        private Group CreateGroup(string title, Vector2 position, groupdata groupData = null)
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
            icon.style.backgroundImage = util_EditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/stick.png");
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

            group_element_title.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                OnGroupChangedName(group, group_element_title.text);
            });

            // 应用配置文件的颜色到编组标题的文字颜色和背景颜色
            ThemesList.Group.ForEach(colorData =>
            {
                if (colorData.solution == groupData.solution)
                {
                    group_element_header.style.backgroundColor = util_EditorUtility.Color_From_HexString(colorData.title_bg_color);
                    group_element_content.style.backgroundColor = util_EditorUtility.Color_From_HexString(colorData.content_bg_color);
                    group_element_title.style.color = util_EditorUtility.Color_From_HexString(colorData.title_text_color);
                    icon.style.unityBackgroundImageTintColor = util_EditorUtility.Color_From_HexString(colorData.logo_color);
                }
            });

            // 绑定 Group 右键菜单
            group_element_header.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendSeparator();
                // 菜单 - 主题色切换
                for (int i = 0; i < ThemesList.Group.Count; i++)
                {
                    ThemeData_Group dat = ThemesList.Group[i];
                    evt.menu.AppendAction($"T 编组配色/{dat.solution}", d =>
                    {
                        Undo.RecordObject(ActionTreeAsset, "Set Group Color");

                        if (CurrentSelectedGroups.Count > 1)
                        {
                            for (int i = 0; i < CurrentSelectedGroups.Count; i++)
                            {
                                Group gp = CurrentSelectedGroups[i];

                                // 同步修改行为树根节点中的对应的编组数据的配色方案
                                ActionTreeAsset.NodeGroupDatas.ForEach(data => { if (data.group == gp) { data.solution = dat.solution; } });

                                gp.Q<VisualElement>("headerContainer").style.backgroundColor = util_EditorUtility.Color_From_HexString(dat.title_bg_color);
                                gp.Q<VisualElement>("centralContainer").style.backgroundColor = util_EditorUtility.Color_From_HexString(dat.content_bg_color);
                                gp.Q<Label>("titleLabel").style.color = util_EditorUtility.Color_From_HexString(dat.title_text_color);
                                gp.Q<Label>("groupicon").style.unityBackgroundImageTintColor = util_EditorUtility.Color_From_HexString(dat.logo_color);
                            }
                        }
                        else
                        {
                            groupData.solution = dat.solution;

                            group_element_header.style.backgroundColor = util_EditorUtility.Color_From_HexString(dat.title_bg_color);
                            group_element_content.style.backgroundColor = util_EditorUtility.Color_From_HexString(dat.content_bg_color);
                            group_element_title.style.color = util_EditorUtility.Color_From_HexString(dat.title_text_color);
                            icon.style.unityBackgroundImageTintColor = util_EditorUtility.Color_From_HexString(dat.logo_color);
                        }
                    });
                }
            }));

            // 注册编组位置改变事件
            group.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (evt.oldRect.position != evt.newRect.position)
                {
                    OnGroupChangedPosition(group, group.GetPosition().position);
                }
            });

            // 注册编组变化事件
            group.RegisterCallback<GeometryChangedEvent>(OnGroupChangedState);

            // 初始化状态跟踪
            CurrentCreatedGroups[group] = new HashSet<object>();

            // 关联Group和NodeGroupData
            if (groupData != null)
            {
                groupData.group = group;
            }

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
            selectedNodes.ForEach(n =>
            {
                if (n is VNode_Base node)
                {
                    nodes_guid.Add(node.ActionNode.guid);
                }
                else if (n is VNode_Stick stick)
                {
                    nodes_guid.Add(stick.stickNoteData.guid);
                }
            });

            // 创建编组数据
            groupdata gp_data = new groupdata(title, GUID.Generate().ToString(), localMousePosition, nodes_guid, "M 默认", null);

            // 初始化编组
            Group gp = CreateGroup(title, localMousePosition, gp_data);

            // 添加选中节点到编组
            selectedNodes.ForEach(n => gp.AddElement(n));

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
        private void OnGroupChangedName(Group group, string newName)
        {
            groupdata groupData = ActionTreeAsset.NodeGroupDatas
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
        /// 同步编组 - 位置
        /// </summary>
        /// <param solution="group"></param>
        /// <param solution="newpos"></param>
        private void OnGroupChangedPosition(Group group, Vector2 newpos)
        {
            groupdata groupData = ActionTreeAsset.NodeGroupDatas
                .FirstOrDefault(g => g.group == group);

            if (groupData != null && groupData.pos != newpos)
            {
#if UNITY_EDITOR
                Undo.RecordObject(ActionTreeAsset, "Reposition Group");
#endif
                groupData.pos = newpos;

                //#if UNITY_EDITOR
                //                EditorUtility.SetDirty(ActionTreeAsset);
                //#endif
            }
        }
        /// <summary>
        /// Group 变化事件处理
        /// </summary>
        private void OnGroupChangedState(GeometryChangedEvent evt)
        {
            Group target = evt.target as Group;
            if (target == null || !CurrentCreatedGroups.ContainsKey(target)) return;

            // 查找对应的 groupdata
            groupdata groupData = ActionTreeAsset.NodeGroupDatas.FirstOrDefault(g => g.group == target);
            Undo.RecordObject(ActionTreeAsset, "Group Guids Changed");

            var currentElements = new HashSet<object>(target.containedElements);
            var previousElements = CurrentCreatedGroups[target];

            var addedElements = new HashSet<object>(currentElements);
            addedElements.ExceptWith(previousElements);

            var removedElements = new HashSet<object>(previousElements);
            removedElements.ExceptWith(currentElements);

            // 处理移入的节点
            foreach (var item in addedElements)
            {
                string guid = null;
                if (item is VNode_Base node)
                {
                    guid = node.ActionNode.guid;
                    //Debug.Log($"Group '{sourceNode.title}' 移入节点: {data.RelayData.identifyName}");
                }
                else if (item is VNode_Stick stick)
                {
                    guid = stick.stickNoteData.guid;
                    //Debug.Log($"Group '{sourceNode.title}' 移入便签: {stick.stickNoteData.solution}");
                }

                if (!string.IsNullOrEmpty(guid) && !groupData.guids.Contains(guid))
                {
                    groupData.guids.Add(guid);
                }
            }

            // 处理移出的节点
            foreach (var item in removedElements)
            {
                string guid = null;
                if (item is VNode_Base node)
                {
                    guid = node.ActionNode.guid;
                    //Debug.Log($"Group '{sourceNode.title}' 移出节点: {data.RelayData.identifyName}");
                }
                else if (item is VNode_Stick stick)
                {
                    guid = stick.stickNoteData.guid;
                    //Debug.Log($"Group '{sourceNode.title}' 移出便签: {stick.stickNoteData.solution}");
                }

                if (!string.IsNullOrEmpty(guid))
                {
                    groupData.guids.Remove(guid);
                }
            }

            // 更新状态跟踪
            CurrentCreatedGroups[target] = currentElements;
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
                ActionTreeAsset.NodeGroupDatas.Clear();
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
            var groupData = ActionTreeAsset.NodeGroupDatas.FirstOrDefault(g => g.group == group);
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
    }
}