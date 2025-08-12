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
        /// 根据数据行为树根节点容器里的子资源来生成GraphView的视觉节点
        /// </summary>
        /// <param h_name="actiontree"></param>
        public void Restructure_VisualNodes(ActionNode_Asset actiontree)
        {
            // 获取到数据根节点
            ActionTreeAsset = actiontree;

            graphViewChanged -= OnGraphViewChanged;
            // 清空所有 NodeView
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            // 根据根节点的数据列表重建 NodeViews
            ActionTreeAsset.ActionNodes.ForEach(data =>
            {
                if (data.actionNodeType == "Relay")
                    Node_MakeRelay(data.nodeGraphPosition, data).Draw().RefreshExpandedState();
                else
                    Node_Make(data.nodeGraphPosition, data).Draw().RefreshExpandedState();
            });

            // 根据行为树根节点的数据列表重建 Edges
            ActionTreeAsset.ActionNodes.ForEach(d =>
            {
                // 获取的目标数据节点的子数据节点
                var children = ActionTreeAsset.GetChildrenNodes(d);
                // c 为每一个子数据节点
                children.ForEach(c =>
                {
                    VNode_Base n_parent = FindNodeView(d.guid);
                    VNode_Base n_child = FindNodeView(c.guid);

                    if (n_parent is VNode_Relay relay)
                    {
                        Debug.Log(relay.Port_Input.Port.connected);
                        relay.CheckConnected();
                    }

                    n_parent.Port_Outputs.ForEach(p =>
                    {
                        Edge edge = p.Port.ConnectTo(n_child.Port_Input.Port);
                        AddElement(edge);
                    });
                });
            });

            // 根据行为树根节点里的便签列表数据来生成GraphView的视觉便签节点
            Restructure_Sticks(actiontree.StickNoteDatas);

            // 重建编组
            Restructure_Groups(actiontree.NodeGroupDatas); // 新增调用
        }
        /// <summary>
        /// 根据行为树根节点里的便签列表数据来生成GraphView的视觉便签节点
        /// </summary>
        /// <param h_name="stickdata"></param>
        public void Restructure_Sticks(List<stickdata> stickdata)
        {
            // 根据根节点的数据列表重建 NodeViews
            stickdata.ForEach(data =>
            {
                Node_MakeStick(data.position, data).Draw();
            });
        }
        /// <summary>
        /// 根据行为树根节点里的编组列表数据来生成GraphView的视觉编组
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
                    // 查找普通节点
                    var node = nodes.ToList().FirstOrDefault(n =>
                        n is VNode_Base baseNode && baseNode.ActionNode.guid == guid);

                    if (node != null)
                    {
                        group.AddElement(node);
                        continue;
                    }

                    // 查找便签节点
                    var stickNote = nodes.ToList().FirstOrDefault(n =>
                        n is VNode_Stick stickNode && stickNode.stickNoteData.guid == guid);

                    if (stickNote != null)
                    {
                        group.AddElement(stickNote);
                    }
                }
            }
        }
    }
}