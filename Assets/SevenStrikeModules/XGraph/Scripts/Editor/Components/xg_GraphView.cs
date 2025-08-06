namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Plastic.Newtonsoft.Json;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;
    using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;
    using Color = UnityEngine.Color;
    using Edge = UnityEditor.Experimental.GraphView.Edge;

    [Serializable]
    public class ThemeData_Group
    {
        public string solution = "默认";
        public string title_bg_color = "#3C725D";
        public string title_text_color = "#FFFFFF";
        public string content_bg_color = "#DBDBDB1A";
        public string logo_color = "#ffffff";

        public ThemeData_Group() { }

        public ThemeData_Group(string solution, string bg_color, string text_color, string content_bg_color, string logo_color)
        {
            this.solution = solution;
            this.title_bg_color = bg_color;
            this.title_text_color = text_color;
            this.content_bg_color = content_bg_color;
            this.logo_color = logo_color;
        }
    }

    [Serializable]
    public class ThemeData_GraphViewBgGrid
    {
        public string solution = "default";
        public string grid_bg_color = "#747474";
        public string line_color = "#2f2f2f";
        public string thickLine_color = "#2f2f2f";
        public string spacing = "20";
        public string thicklines = "0";

        public ThemeData_GraphViewBgGrid() { }

        public ThemeData_GraphViewBgGrid(string solution, string grid_bg_color, string line_color, string thickLine_color, string spacing, string thicklines)
        {
            this.solution = solution;
            this.grid_bg_color = grid_bg_color;
            this.line_color = line_color;
            this.thickLine_color = thickLine_color;
            this.spacing = spacing;
            this.thicklines = thicklines;
        }
    }

    [Serializable]
    public class ThemeData_Node
    {
        public string solution = "默认";
        public string nodecolor = "#747474";

        public ThemeData_Node() { }

        public ThemeData_Node(string solution, string nodecolor)
        {
            this.solution = solution;
            this.nodecolor = nodecolor;
        }
    }

    [Serializable]
    public class ThemesList
    {
        /// <summary>
        /// Graphview Group 颜色集
        /// </summary>
        public List<ThemeData_Group> Group = new List<ThemeData_Group>();
        /// <summary>
        /// Graphview BgGrid 颜色集
        /// </summary>
        public List<ThemeData_GraphViewBgGrid> GraphviewBgGrid = new List<ThemeData_GraphViewBgGrid>();
        /// <summary>
        /// Graphview BgGrid 颜色集
        /// </summary>
        public List<ThemeData_Node> Node = new List<ThemeData_Node>();
    }

    /// <summary>
    /// XGraph的GraphView基础件，[UxmlElement]用于在UIBuilder中出现GraphView的控件
    /// </summary>
    [UxmlElement]
    public partial class xg_GraphView : GraphView
    {
        /// <summary>
        /// XGraph 主窗口
        /// </summary>
        public xg_Window gv_GaphWindow;
        /// <summary>
        /// 节点搜索框
        /// </summary>
        private xg_NodesSearchBox gv_NodesSearchBox;
        /// <summary>
        /// xw_graphView 内容缩放 - 最小
        /// </summary>
        private float gv_scaleGraph_Min = 0.2f;
        /// <summary>
        /// xw_graphView 内容缩放 - 最大
        /// </summary>
        private float gv_scaleGraph_Max = 3.5f;
        /// <summary>
        /// xw_graphView 创建节点的位置
        /// </summary>
        public Vector2 gv_NodeCreatedPosition;
        /// <summary>
        /// 用于存储复制的节点数据
        /// </summary>
        private List<xGraphNode_Base> gv_CopiedNodeList = new List<xGraphNode_Base>();
        /// <summary>
        /// 当节点被选中时的回调委托
        /// </summary>
        public Action<xGraphNode_Base> OnSelectedNode;
        /// <summary>
        /// 当节点被取消选中时的回调委托
        /// </summary>
        public Action<xGraphNode_Base> OnUnSelectedNode;
        /// <summary>
        /// 当节点被选中时的回调委托
        /// </summary>
        public Action<List<xGraphNode_Base>> OnSelectionNodes;
        /// <summary>
        /// 当节点被移除选中时的回调委托
        /// </summary>
        public Action<List<xGraphNode_Base>> OnRemoveSelectionNodes;
        /// <summary>
        /// 当前正在编辑的资源
        /// </summary>
        public ActionTree_Nodes_Asset ActionTreeAsset;
        /// <summary>
        /// 读取菜单结构列表内容
        /// </summary>
        public TextAsset SearchStructures_Json = null;
        /// <summary>
        /// 序列化解析到菜单结构列表类
        /// </summary>
        public searchBox_NodesRoot SearchStructures;
        /// <summary>
        /// 编组收集容器
        /// </summary>
        private Dictionary<Group, HashSet<object>> CurrentCreatedGroups = new Dictionary<Group, HashSet<object>>();
        /// <summary>
        /// GroupTitle 颜色集
        /// </summary>
        public ThemesList ThemesList = new ThemesList();
        /// <summary>
        /// 当前选中的所有节点
        /// </summary>
        private List<xGraphNode_Base> CurrentSelectedNodes = new List<xGraphNode_Base>();
        /// <summary>
        /// 当前选中的所有编组
        /// </summary>
        private List<Group> CurrentSelectedGroups = new List<Group>();
        /// <summary>
        /// 节点颜色标记开关
        /// </summary>
        private bool NodeColorDisplay = false;

        #region GraphView构造
        /// <summary>
        /// GraphView 组件实例化构造器
        /// </summary>
        public xg_GraphView()
        {
            // 读取菜单结构列表内容
            SearchStructures_Json = util_EditorUtility.AssetLoad<TextAsset>($"{util_Dashboard.GetPath_Config()}/NodesSearchStructure.json");
            // 序列化解析到类
            SearchStructures = JsonConvert.DeserializeObject<searchBox_NodesRoot>(SearchStructures_Json.text);

            #region 布局样式设定
            // 指定Flex布局的Grow撑满
            this.style.flexGrow = 1;

            // 插入网格背景            
            Insert(0, new GridBackground());

            var uss_GraphView = util_EditorUtility.AssetLoad<StyleSheet>($"{util_Dashboard.GetPath_GUI_Uss()}uss_GraphView.uss");
            styleSheets.Add(uss_GraphView);

            var uss_group = util_EditorUtility.AssetLoad<StyleSheet>($"{util_Dashboard.GetPath_GUI_Uss()}uss_Group.uss");
            styleSheets.Add(uss_group);

            #endregion

            #region 添加 GraphView 基础组件
            // 添加 xw_graphView 基础组件 - 内容拖动
            this.AddManipulator(new ContentDragger());
            // 设置缩放距离
            this.SetupZoom(gv_scaleGraph_Min, gv_scaleGraph_Max);
            // 添加 xw_graphView 基础组件 - 内容选择拖动
            this.AddManipulator(new SelectionDragger());
            // 添加 xw_graphView 基础组件 - 内容框选
            this.AddManipulator(new RectangleSelector());
            // 启用节点之间的连线功能
            this.AddManipulator(new EdgeManipulator());
            // 实例化节点搜索框的主体
            AddNodesSearchBox();
            #endregion

            #region 注册事件委托
            // 注册处理快捷键
            RegisterCallback<KeyDownEvent>(OnKeyDown);

            // 注册鼠标点击事件
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            #endregion

            // 读取Group主题色方案
            LoadThemes();

            // 注册双击连线的回调
            RegisterCallback<MouseDownEvent>(Node_AddRelay, TrickleDown.TrickleDown);
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 居中聚焦所有视觉节点
        /// </summary>
        public void SetFrameAll()
        {
            FrameAll();
        }
        /// <summary>
        /// 读取Group主题色方案
        /// </summary>
        private void LoadThemes()
        {
            string json_themes = util_EditorUtility.AssetLoad<TextAsset>($"{util_Dashboard.GetPath_Config()}/Themes.json").text;
            //Debug.Log(json_themes);
            ThemesList = JsonConvert.DeserializeObject<ThemesList>(json_themes);
        }
        /// <summary>
        /// 刷新目标节点主题配色
        /// </summary>
        /// <param name="node"></param>
        private void RefreshTheme_GraphNode(xGraphNode_Base node)
        {
            // 应用配置文件的颜色到节点的标识颜色
            ThemesList.Node.ForEach(colorData =>
            {
                if (colorData.solution == node.ActionTreeNode.nodeThemeSolution)
                {
                    node.ActionTreeNode.nodeThemeColor = util_EditorUtility.Color_From_HexString(colorData.nodecolor);
                }
            });

            if (!NodeColorDisplay)
            {
                node.MarkColor_Hidden();
            }
            else
            {
                node.MarkColor_Dislay();
            }
        }
        /// <summary>
        /// 清空GraphView的所有内容
        /// </summary>
        public void ClearGraphViewContents(bool DisplayActionTreeInspector = true)
        {
            // 清空克隆体的内容
            ActionTreeAsset.Clear();

            // 清空GraphView的所有节点
            Nodes_Clear();

            // 清空GraphView的所有连线
            Edges_Clear();

            // 清空GraphView的所有Group
            Groups_Clear();

            // 清空 Inspector 视图
            gv_GaphWindow.xw_InspectorView.ClearInspector();

            if (DisplayActionTreeInspector)
                // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                gv_GaphWindow.xw_InspectorView.UpdateSelection(ActionTreeAsset);
        }
        /// <summary>
        /// 切换Graphview背景主题
        /// </summary>
        /// <param name="dat"></param>
        private static void SwitchGraphViewBgTheme(ThemeData_GraphViewBgGrid dat)
        {
            xg_UssModifier.ModifyCssRule($"{util_Dashboard.GetPath_GUI_Uss()}uss_GraphView.uss", "GridBackground", "--grid-background-color", dat.grid_bg_color);
            xg_UssModifier.ModifyCssRule($"{util_Dashboard.GetPath_GUI_Uss()}uss_GraphView.uss", "GridBackground", "--line-color", dat.line_color);
            xg_UssModifier.ModifyCssRule($"{util_Dashboard.GetPath_GUI_Uss()}uss_GraphView.uss", "GridBackground", "--thick-line-color", dat.thickLine_color);
            xg_UssModifier.ModifyCssRule($"{util_Dashboard.GetPath_GUI_Uss()}uss_GraphView.uss", "GridBackground", "--spacing", dat.spacing);
            xg_UssModifier.ModifyCssRule($"{util_Dashboard.GetPath_GUI_Uss()}uss_GraphView.uss", "GridBackground", "--thick-lines", dat.thicklines);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        /// <summary>
        /// 根据数据节点的GUID来获取目标视觉节点
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private xGraphNode_Base FindNodeView(string guid)
        {
            return GetNodeByGuid(guid) as xGraphNode_Base;
        }
        /// <summary>
        /// 根据数据节点的GUID来获取目标视觉节点
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private Node FindNode(string guid)
        {
            return GetNodeByGuid(guid) as Node;
        }
        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        /// <param h_name="screenMousePosition"></param>
        /// <returns></returns>
        public Vector2 GetLocalMousePosition(Vector2 screenMousePosition)
        {
            // 将光标的屏幕坐标转换为光标在当前窗口内的坐标
            Vector2 window_mouse_pos = screenMousePosition - gv_GaphWindow.position.position;

            // 将光标在当前窗口内的坐标转换为光标在节点视图内的坐标
            Vector2 local_mouse_pos = contentViewContainer.WorldToLocal(window_mouse_pos);

            return local_mouse_pos;
        }
        /// <summary>
        /// 获取当前视口的滚动位置
        /// </summary>
        /// <returns></returns>
        public Vector2 GetCurrentViewPosition()
        {
            // viewTransform.matrix 包含平移和缩放信息
            Matrix4x4 matrix = viewTransform.matrix;

            // 提取平移部分（通常位于 matrix.m03 和 matrix.m13）
            float panX = matrix.m03;
            float panY = matrix.m13;

            return new Vector2(panX, panY);
        }
        /// <summary>
        /// 获取当前视口的缩放
        /// </summary>
        /// <returns></returns>
        public float GetCurrentZoomLevel()
        {
            return viewTransform.matrix.m00;
        }
        /// <summary>
        /// 设置视口位置
        /// </summary>
        /// <param solution="position"></param>
        /// <param solution="scale"></param>
        public void SetViewPosition(Vector2 position, float scale)
        {
            // 第二个参数是缩放，这里设为 scale,scale,1 表示缩放到scale，Z值保持1即可
            UpdateViewTransform(position, new Vector3(scale, scale, 1));
        }
        #endregion

        #region 右键菜单
        /// <summary>
        /// 实现 GraphView 视图内的鼠标右键上下文菜单
        /// </summary>
        /// <param h_name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //base.BuildContextualMenu(evt);

            //Debug.Log(evt.target);

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
            if (evt.target is xGraphNode_Base nodebase)
            {
                // 菜单 - 主题色切换
                for (int i = 0; i < ThemesList.Node.Count; i++)
                {
                    ThemeData_Node dat = ThemesList.Node[i];
                    evt.menu.AppendAction($"T 节点配色/{dat.solution}", (action) =>
                    {
                        //Undo.RecordObject(ActionTreeAsset, "Change NodeColor");
                        if (CurrentSelectedNodes.Count > 0)
                        {
                            for (int s = 0; s < CurrentSelectedNodes.Count; s++)
                            {
                                xGraphNode_Base node = CurrentSelectedNodes[s];
                                node.ActionTreeNode.nodeThemeSolution = dat.solution;
                                node.ActionTreeNode.nodeThemeColor = util_EditorUtility.Color_From_HexString(dat.nodecolor);
                                // 改变图标颜色
                                node.IconLabel.style.unityBackgroundImageTintColor = node.ActionTreeNode.nodeThemeSolution == "M 默认" ? Color.white * 0.7f : node.ActionTreeNode.nodeThemeColor;

                                // 改变连线颜色
                                if (node.Port_Input != null && node.Port_Input.Port != null)
                                    node.Port_Input.Port.portColor = node.ActionTreeNode.nodeThemeSolution == "M 默认" ? Color.white * 0.7f : node.ActionTreeNode.nodeThemeColor;
                                if (node.Port_Output != null && node.Port_Output.Port != null)
                                    node.Port_Output.Port.portColor = node.ActionTreeNode.nodeThemeSolution == "M 默认" ? Color.white * 0.7f : node.ActionTreeNode.nodeThemeColor;
                                node.SetMarkColor();
                                node.UpdateMarkColor();
                            }
                        }
                        else
                        {
                            nodebase.ActionTreeNode.nodeThemeSolution = dat.solution;
                            nodebase.ActionTreeNode.nodeThemeColor = util_EditorUtility.Color_From_HexString(dat.nodecolor);
                            // 改变图标颜色
                            nodebase.IconLabel.style.unityBackgroundImageTintColor = nodebase.ActionTreeNode.nodeThemeSolution == "M 默认" ? Color.white * 0.7f : nodebase.ActionTreeNode.nodeThemeColor;

                            // 改变连线颜色
                            if (nodebase.Port_Input != null && nodebase.Port_Input.Port != null)
                                nodebase.Port_Input.Port.portColor = nodebase.ActionTreeNode.nodeThemeSolution == "M 默认" ? Color.white * 0.7f : nodebase.ActionTreeNode.nodeThemeColor;
                            if (nodebase.Port_Output != null && nodebase.Port_Output.Port != null)
                                nodebase.Port_Output.Port.portColor = nodebase.ActionTreeNode.nodeThemeSolution == "M 默认" ? Color.white * 0.7f : nodebase.ActionTreeNode.nodeThemeColor;
                            nodebase.SetMarkColor();
                            nodebase.UpdateMarkColor();
                        }
                    });
                }
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
            if (evt.target is xGraphNode_StickNote stick)
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
                    Node_Paste(gv_NodeCreatedPosition);
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
                        Node_Copy();
                    });
                }
                #endregion
            }
        }
        #endregion

        #region 当GraphView内容发生改变时
        /// <summary>
        /// 当GraphView组件发生改变时
        /// </summary>
        /// <param assetName="graphViewChange"></param>
        /// <returns></returns>
        public GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            OnChanged_RemovedElement(graphViewChange);

            OnChanged_CreateEdge(graphViewChange);

            Debug.Log("Changed");
            return graphViewChange;
        }
        /// <summary>
        /// 当有连线被创建时
        /// </summary>
        /// <param assetName="graphViewChange"></param>
        private void OnChanged_CreateEdge(GraphViewChange graphViewChange)
        {
            // 当有连线被创建时
            if (graphViewChange.edgesToCreate != null)
            {
                // 如果创建了连线，e 为连线
                graphViewChange.edgesToCreate.ForEach(e =>
                {
                    #region 行为树节点连线逻辑
                    // e 连线的父级节点
                    xGraphNode_Base n_parent = e.output.node as xGraphNode_Base;
                    // e 连线的子级节点
                    xGraphNode_Base n_child = e.input.node as xGraphNode_Base;

                    if (n_parent != null && n_child != null)
                    {
                        // 将 "n_child" 放到 "n_parent" 的child成员变量中，这样就可以让父级数据节点知道自己和哪个子级数据节点相连接
                        ActionTreeAsset.AddNodeToChild(n_parent.ActionTreeNode, n_child.ActionTreeNode);
                    }
                    #endregion                   
                });
            }
        }
        /// <summary>
        /// 当有节点被移除时
        /// </summary>
        /// <param assetName="graphViewChange"></param>
        private void OnChanged_RemovedElement(GraphViewChange graphViewChange)
        {
            // 当有节点被移除时
            if (graphViewChange.elementsToRemove != null)
            {
                // 当有元素被移除的时候
                graphViewChange.elementsToRemove.ForEach(element =>
                {
                    #region 连线
                    Edge edge = element as Edge;
                    if (edge != null)
                    {
                        #region 判断连线源是否是行为树资源节点
                        // 连线的父级节点
                        xGraphNode_Base n_parent = edge.output.node as xGraphNode_Base;
                        // 连线的子级节点
                        xGraphNode_Base n_child = edge.input.node as xGraphNode_Base;

                        if (n_parent != null && n_child != null)
                        {
                            // 将 "n_child" 从 "n_parent" 的 "child" 数据节点变量中移除
                            ActionTreeAsset.RemoveChildNode(n_parent.ActionTreeNode, n_child.ActionTreeNode);
                        }
                        #endregion                        
                    }
                    #endregion

                    #region 行为节点
                    xGraphNode_Base nodeview = element as xGraphNode_Base;
                    if (nodeview != null)
                    {
                        // 从根节点中移除数据节点
                        ActionTreeAsset.Remove(nodeview.ActionTreeNode);
                    }
                    #endregion

                    #region 便签节点
                    xGraphNode_StickNote stickview = element as xGraphNode_StickNote;
                    if (stickview != null)
                    {
                        Undo.RecordObject(ActionTreeAsset, "Remove StickNote");
                        ActionTreeAsset.StickNote_Remove(stickview.stickNoteData);
                    }
                    #endregion

                    #region 编组
                    Group group = element as Group;
                    if (group != null)
                    {
                        Undo.RecordObject(ActionTreeAsset, "Remove Group");

                        // 查找对应的 NodeGroupData
                        NodeGroupData groupData = ActionTreeAsset.NodeGroupDatas.FirstOrDefault(g => g.group == group);
                        if (groupData != null)
                        {
                            // 从 NodeGroupDatas 中移除
                            ActionTreeAsset.NodeGroup_Remove(groupData);
                        }

                        // 清理状态跟踪数据
                        if (CurrentCreatedGroups.ContainsKey(group))
                        {
                            CurrentCreatedGroups.Remove(group);
                        }
                    }
                    #endregion
                });
            }
        }
        #endregion

        #region 视觉： 节点 / 便签 / 编组  -  重建
        /// <summary>
        /// 根据数据行为树根节点容器里的子资源来生成GraphView的视觉节点
        /// </summary>
        /// <param h_name="actiontree"></param>
        internal void Node_Make_With_ActionTreeData(ActionTree_Nodes_Asset actiontree)
        {
            // 获取到数据根节点
            ActionTreeAsset = actiontree;

            graphViewChanged -= OnGraphViewChanged;
            // 清空所有 NodeView
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            // 根据根节点的数据列表重建 NodeViews
            ActionTreeAsset.ActionTreeNodes.ForEach(data =>
            {
                Node_Make(data.nodeGraphPosition, data).Draw();
            });

            // 根据行为树根节点的数据列表重建 Edges
            ActionTreeAsset.ActionTreeNodes.ForEach(d =>
            {
                // 获取的目标数据节点的子数据节点
                var children = ActionTreeAsset.GetChildrenNodes(d);
                // c 为每一个子数据节点
                children.ForEach(c =>
                {
                    xGraphNode_Base n_parent = FindNodeView(d.nodeGUID);
                    xGraphNode_Base n_child = FindNodeView(c.nodeGUID);

                    Edge edge = n_parent.Port_Output.Port.ConnectTo(n_child.Port_Input.Port);
                    AddElement(edge);
                });
            });

            // 根据行为树根节点里的便签列表数据来生成GraphView的视觉便签节点
            Node_Make_With_StickData(actiontree.StickNoteDatas);

            // 重建编组
            Node_Make_With_GroupData(actiontree.NodeGroupDatas); // 新增调用
        }
        /// <summary>
        /// 根据行为树根节点里的便签列表数据来生成GraphView的视觉便签节点
        /// </summary>
        /// <param h_name="stickdata"></param>
        internal void Node_Make_With_StickData(List<StickNoteData> stickdata)
        {
            // 根据根节点的数据列表重建 NodeViews
            stickdata.ForEach(data =>
            {
                Node_Make_StickNote(data.position, data).Draw();
            });
        }
        /// <summary>
        /// 根据行为树根节点里的编组列表数据来生成GraphView的视觉编组
        /// </summary>
        internal void Node_Make_With_GroupData(List<NodeGroupData> groupDatas)
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
                        n is xGraphNode_Base baseNode && baseNode.ActionTreeNode.nodeGUID == guid);

                    if (node != null)
                    {
                        group.AddElement(node);
                        continue;
                    }

                    // 查找便签节点
                    var stickNote = nodes.ToList().FirstOrDefault(n =>
                        n is xGraphNode_StickNote stickNode && stickNode.stickNoteData.guid == guid);

                    if (stickNote != null)
                    {
                        group.AddElement(stickNote);
                    }
                }
            }
        }
        #endregion

        #region 内部重写方法
        /// <summary>
        /// 添加到选择集中
        /// </summary>
        /// <param solution="selectable"></param>
        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            List<xGraphNode_Base> gvs = new List<xGraphNode_Base>();
            List<Group> gps = new List<Group>();
            selection.ForEach(n =>
            {
                if (n is xGraphNode_Base node)
                    if (node.ActionTreeNode.actionNodeType != "StickNote")
                        gvs.Add(node);
                if (n is Group gp)
                {
                    gps.Add(gp);
                }
            });
            CurrentSelectedGroups = gps;
            CurrentSelectedNodes = gvs;

            if (OnSelectionNodes != null)
                OnSelectionNodes(gvs);
        }
        /// <summary>
        /// 从节点选择集中移除
        /// </summary>
        /// <param solution="selectable"></param>
        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            List<xGraphNode_Base> gvs = new List<xGraphNode_Base>();
            List<Group> gps = new List<Group>();
            selection.ForEach(n =>
            {
                if (n is xGraphNode_Base node)
                {
                    gvs.Add(node);
                }
                if (n is Group gp)
                {
                    gps.Add(gp);
                }
            });
            CurrentSelectedGroups = gps;
            CurrentSelectedNodes = gvs;

            if (OnRemoveSelectionNodes != null)
                OnRemoveSelectionNodes(gvs);
        }
        /// <summary>
        /// 重写删除方法，为了确保编组删除时保留内部节点
        /// </summary>
        /// <returns></returns>
        public override EventPropagation DeleteSelection()
        {
            // 1. 获取所有待删除元素
            var selectionCopy = selection.ToList();

            // 2. 单独处理Group的删除
            foreach (var item in selectionCopy.OfType<Group>().ToList())
            {
                DeleteGroup(item);
            }

            // 3. 处理其他元素的删除
            return base.DeleteSelection(); // 调用原始逻辑删除非Group元素
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
        #endregion

        #region 节点操作
        /// <summary>
        /// 选中所有视觉节点
        /// </summary>
        private void Node_Selectall()
        {
            // 清空当前选择
            ClearSelection();

            // 将所有节点加入当前选择
            foreach (var node in nodes)
            {
                AddToSelection(node);
            }
        }
        /// <summary>
        /// 实现视觉节点复制逻辑
        /// </summary>
        private void Node_Duplicate()
        {
            var selectedNodes = selection.OfType<Node>().ToList();
            if (selectedNodes.Count == 0) return;

            ClearSelection(); // 清空当前选择（可选）

            foreach (var original in selectedNodes)
            {
                if (original is xGraphNode_Base xg_base)
                {
                    // 待复制的源节点
                    ActionTree_Node_Base node = xg_base.ActionTreeNode;

                    // 待复制的源节点的命名空间
                    string prefix_namespace = node.GetType().Namespace;
                    // 待复制的源节点的类名
                    string class_name = node.GetType().Name;
                    // 待复制的源节点的行为节点名
                    string actiontype_name = node.actionNodeType.ToString();
                    // 待复制的源节点的类名的前缀
                    string prefix_class = class_name.Substring(0, class_name.Length - actiontype_name.Length);

                    //Debug.Log($"{prefix_namespace} - {class_name} - {actiontype_name} - {prefix_class}");

                    // 克隆出新的节点
                    Node_Create(node.nodeName, prefix_namespace, prefix_class, node.actionNodeType, node.icon, node.graphNodeType);
                }
                else if (original is xGraphNode_StickNote xg_sticknote)
                {
                    StickNoteData sickData = xg_sticknote.stickNoteData.Clone(true);
                    Undo.RecordObject(ActionTreeAsset, "Duplicate StickNoteData");
                    ActionTreeAsset.StickNoteDatas.Add(sickData);

                    Node_Make_StickNote(sickData.position + new Vector2(50, 20), sickData).Draw();
                }
            }
        }
        /// <summary>
        /// 复制视觉节点
        /// </summary>
        private void Node_Copy()
        {
            // 获取当前选中的节点
            gv_CopiedNodeList = selection.OfType<xGraphNode_Base>().ToList();
            if (gv_CopiedNodeList.Count == 0)
            {
                return;
            }
        }
        /// <summary>
        /// 粘贴视觉节点
        /// </summary>
        private void Node_Paste(Vector2 pos)
        {
            if (gv_CopiedNodeList.Count == 0)
            {
                Debug.LogWarning("剪贴板中没有节点数据！");
                return;
            }

            // 清空当前选择
            ClearSelection();

            // 粘贴节点
            foreach (var originalNode in gv_CopiedNodeList)
            {
                // 创建新节点
                var newNode = Node_Make(gv_NodeCreatedPosition, originalNode.ActionTreeNode);

                // 设置新节点位置（偏移到指定位置）
                Rect originalPos = originalNode.GetPosition();
                newNode.SetPosition(new Rect(pos, originalPos.size));

                // 复制端口数据
                newNode.ActionTreeNode = originalNode.ActionTreeNode;

                // 复制端口信息
                newNode.SetPortInfo(originalNode.Port_Input);
                newNode.SetPortInfo(originalNode.Port_Output);

                // 刷新节点
                newNode.Draw();
                newNode.RefreshExpandedState();
                newNode.RefreshPorts();

                // 选中新节点
                AddToSelection(newNode);
            }
        }
        /// <summary>
        /// 移除当前选择的所有节点及其相关的连线
        /// </summary>
        public void Node_Delete()
        {
            // 获取当前选择的所有节点
            var selectedNodes = selection.OfType<Node>().ToList();

            // 如果没有选中的节点，直接返回
            if (selectedNodes.Count == 0)
            {
                Debug.LogWarning("没有选中的节点！");
                return;
            }

            // 创建一个 GraphViewChange 对象用于调用GraphView的OnGraphViewChanged事件
            var graphViewChange = new GraphViewChange();
            graphViewChange.elementsToRemove = new List<GraphElement>();

            // 遍历所有选中的节点
            foreach (var node in selectedNodes)
            {
                // 移除节点的所有连线
                var edgesToRemove = edges.ToList()
                    .Where(edge => edge.input.node == node || edge.output.node == node)
                    .ToList();
                foreach (var edge in edgesToRemove)
                {
                    // 移除连线
                    RemoveElement(edge);
                    graphViewChange.elementsToRemove.Add(edge); // 添加到 GraphViewChange
                }

                // 移除节点本身
                RemoveElement(node);
                graphViewChange.elementsToRemove.Add(node); // 添加到 GraphViewChange

                // 与删除的节点的端口断开连接
                foreach (var edge in edgesToRemove)
                {
                    edge.input.Disconnect(edge);
                    edge.output.Disconnect(edge);
                }
            }

            // 清空当前选择
            ClearSelection();

            // 调用 OnGraphViewChanged 方法
            OnGraphViewChanged(graphViewChange);
        }
        /// <summary>
        /// 创建中继节点
        /// </summary>
        /// <param name="evt"></param>
        private void Node_AddRelay(MouseDownEvent evt)
        {
            if (evt.clickCount == 2 && evt.target is Edge edge)
            {
                // 在双击位置创建Relay Node
                Vector2 nodePosition = evt.localMousePosition;
                // 将鼠标位置从屏幕坐标转换为 xw_graphView 的局部坐标
                Vector2 localMousePosition = contentViewContainer.WorldToLocal(nodePosition);
                localMousePosition.x -= 50;

                // 获取连线两端
                Port outputPort = edge.output;
                Port inputPort = edge.input;

                #region 当双击两个行为节点中间的连线时，在中间创建一个中继节点
                // 获取为：上级节点
                xGraphNode_Base relay_in = outputPort.node as xGraphNode_Base;
                // 获取为：下级节点
                xGraphNode_Base relay_out = inputPort.node as xGraphNode_Base;


                if (relay_in != null && relay_out != null)
                {
                    //var relayNode = new xGraphNode_Relay(relay_in.Port_Output.Capacity);

                    //// 创建中继节点数据
                    //RelayNodeData relaydata = new RelayNodeData();
                    //relaydata.guid = UnityEditor.GUID.Generate().ToString();
                    //relaydata.position = localMousePosition;
                    //relaydata.inputGuid = relay_in.viewDataKey;
                    //relay_in.ActionTreeNode.relaynodeGUID = relaydata.guid;
                    //relaydata.outputGuids.Add(relay_out.viewDataKey);

                    //Undo.RecordObject(ActionTreeAsset, "Create RelayNode");
                    //// 加入到行为树根节点的中继列表中
                    //ActionTreeAsset.RelayData_Add(relaydata);

                    //// 初始化中继节点（指定Graph视图和位置以及数据）
                    //relayNode.Initialize(this, localMousePosition, relaydata);

                    //// 添加到GraphView
                    //AddElement(relayNode);

                    //// 断开原连线
                    //edge.input.Disconnect(edge);
                    //edge.output.Disconnect(edge);
                    //RemoveElement(edge);

                    //// 重新连接：原输出端口（父节点） -> Relay输入端口
                    //var newEdge_parent = outputPort.ConnectTo(relayNode.InputPort);
                    //AddElement(newEdge_parent);

                    //// 重新连接：Relay输出端口 -> 原输入端口（子节点）
                    //var newEdge_child = relayNode.OutputPort.ConnectTo(inputPort);
                    //AddElement(newEdge_child);
                }
                #endregion

                evt.StopPropagation();
            }
        }
        /// <summary>
        /// 清空视觉节点
        /// </summary>
        internal void Nodes_Clear()
        {
            // 删除所有节点
            foreach (var node in nodes.ToList())
            {
                RemoveElement(node);
            }
        }
        /// <summary>
        /// 创建视觉节点
        /// </summary>
        /// <param h_name="pos"></param>
        /// <param h_name="dat"></param>
        /// <returns></returns>
        public xGraphNode_Base Node_Make(Vector2 pos, ActionTree_Node_Base data = null)
        {
            if (data.graphNodeType == "None")
                return null;

            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.xGraphNode_{data.graphNodeType}");
            // 创建 NodeView 类型的实例为 visualNode 基类
            xGraphNode_Base node = Activator.CreateInstance(type_nodeview) as xGraphNode_Base;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            node.Initialize(this, pos, data);

            // 刷新节点主题配色
            RefreshTheme_GraphNode(node);

            #region GraphView 视图操作
            // 添加进当前主GraphView视图中
            this.AddElement(node);

            // 指定生成的节点点击事件委托，便于实现调用点击节点时调用
            node.OnSelectedNode = OnSelectedNode;
            node.OnUnSelectedNode = OnUnSelectedNode;
            // 刷新GraphView视图
            node.RefreshExpandedState();
            node.RefreshPorts();
            #endregion
            return node;
        }
        /// <summary>
        /// 创建视觉节点 - 便签
        /// </summary>
        /// <param h_name="pos"></param>
        /// <returns></returns>
        public xGraphNode_StickNote Node_Make_StickNote(Vector2 pos, StickNoteData data = null)
        {
            #region 根据枚举类型创建 NodeView
            // 根据枚举名称获取 NodeView 节点类
            Type type_nodeview = Type.GetType($"SevenStrikeModules.XGraph.xGraphNode_StickNote");
            // 创建 NodeView 类型的实例为 visualNode 基类
            xGraphNode_StickNote node = Activator.CreateInstance(type_nodeview) as xGraphNode_StickNote;
            #endregion

            // 初始化节点并将data数据容器赋值过来便于后面使用
            node.Initialize(this, pos, data);

            #region GraphView 视图操作
            // 添加进当前主GraphView视图中
            this.AddElement(node);

            // 刷新GraphView视图
            node.RefreshExpandedState();
            node.RefreshPorts();
            #endregion
            return node;
        }
        #endregion

        #region 连线逻辑
        /// <summary>
        /// GraphView 组件视图内的端口连线规则
        /// </summary>
        /// <param h_name="startPort"></param>
        /// <param h_name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            // 获取起始端口所属的节点
            var startNode = startPort.node;

            // 遍历所有端口
            ports.ForEach(port =>
            {
                // 确保不是同一个端口
                if (startPort == port)
                    return;

                // 确保方向相反（输入连输出，输出连输入）
                if (startPort.direction == port.direction)
                    return;

                // 确保不是同一个节点的端口（防止自连接）
                if (startNode == port.node)
                    return;

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
        /// <summary>
        /// 清空连线
        /// </summary>
        internal void Edges_Clear()
        {
            // 删除所有连线
            foreach (var edge in edges.ToList())
            {
                RemoveElement(edge);
            }
        }
        #endregion

        #region 创建编组
        /// <summary>
        /// 初始化编组并注册相关事件
        /// </summary>
        private Group CreateGroup(string title, Vector2 position, NodeGroupData groupData = null)
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
                if (n is xGraphNode_Base node)
                {
                    nodes_guid.Add(node.ActionTreeNode.nodeGUID);
                }
                else if (n is xGraphNode_StickNote stick)
                {
                    nodes_guid.Add(stick.stickNoteData.guid);
                }
            });

            // 创建编组数据
            NodeGroupData gp_data = new NodeGroupData(title, GUID.Generate().ToString(), localMousePosition, nodes_guid, "M 默认", null);

            // 初始化编组
            Group gp = CreateGroup(title, localMousePosition, gp_data);

            // 添加选中节点到编组
            selectedNodes.ForEach(n => gp.AddElement(n));

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
            NodeGroupData groupData = ActionTreeAsset.NodeGroupDatas
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
            NodeGroupData groupData = ActionTreeAsset.NodeGroupDatas
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

            // 查找对应的 NodeGroupData
            NodeGroupData groupData = ActionTreeAsset.NodeGroupDatas.FirstOrDefault(g => g.group == target);

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
                if (item is xGraphNode_Base node)
                {
                    guid = node.ActionTreeNode.nodeGUID;
                    //Debug.Log($"Group '{target.title}' 移入节点: {data.ActionTreeNode.nodeName}");
                }
                else if (item is xGraphNode_StickNote stick)
                {
                    guid = stick.stickNoteData.guid;
                    //Debug.Log($"Group '{target.title}' 移入便签: {stick.stickNoteData.solution}");
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
                if (item is xGraphNode_Base node)
                {
                    guid = node.ActionTreeNode.nodeGUID;
                    //Debug.Log($"Group '{target.title}' 移出节点: {data.ActionTreeNode.nodeName}");
                }
                else if (item is xGraphNode_StickNote stick)
                {
                    guid = stick.stickNoteData.guid;
                    //Debug.Log($"Group '{target.title}' 移出便签: {stick.stickNoteData.solution}");
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
        #endregion

        #region 行为树节点创建（由创建菜单调用）
        /// <summary>
        /// 创建行为树节点
        /// </summary>
        /// <param h_name="prefix_namespace"></param>
        /// <param h_name="prefix_class"></param>
        /// <param h_name="type"></param>
        /// <param h_name="action_nodeType"></param>
        /// <param h_name="visual_nodeType"></param>
        /// <param h_name="action_name"></param>
        /// <returns></returns>
        private ActionTree_Node_Base ActionTreeNodeCreate(string prefix_namespace, string prefix_class, Type type, string action_nodeType, string icon, string visual_nodeType, string action_name)
        {
            ActionTree_Node_Base data = ScriptableObject.CreateInstance(type) as ActionTree_Node_Base;
            data.name = type.Name;
            data.nodeGUID = GUID.Generate().ToString();
            data.actionNodeType = action_nodeType;
            data.icon = icon;
            data.graphNodeType = visual_nodeType;
            data.nodeName = action_name;
            data.nodeNameSpaceName = prefix_namespace;
            data.nodeClassName = prefix_class;
            ActionTreeAsset.Create(data);
            return data;
        }
        /// <summary>
        /// 创建包装节点
        /// </summary>
        /// <param name="visualName"></param>
        /// <param name="prefix_namespace"></param>
        /// <param name="prefix_class"></param>
        /// <param name="action_nodeType"></param>
        /// <param name="icon"></param>
        /// <param name="visual_nodeType"></param>
        public void Node_Create(string visualName, string prefix_namespace, string prefix_class, string action_nodeType, string icon, string visual_nodeType)
        {
            // 便签类是不需要加入行为树根资源中的，而是加入到行为树根资源的 StickNoteDatas 变量中
            if (action_nodeType == "StickNote")
            {
                Undo.RecordObject(ActionTreeAsset, "Create StickNote");
                // 新建行为树便签内容加入到行为树根资源的 StickNoteDatas 变量中
                StickNoteData stdata = new StickNoteData("便签", "点击此处更改内容", GUID.Generate().ToString(), gv_NodeCreatedPosition, new Vector2(100, 100));
                ActionTreeAsset.StickNote_Add(stdata);

                // 创建新的节点并指定资源数据项
                xGraphNode_StickNote visualstickNode = Node_Make_StickNote(gv_NodeCreatedPosition, stdata);

                // 刷新节点
                visualstickNode.Draw();
                visualstickNode.RefreshExpandedState();
                visualstickNode.RefreshPorts();
            }
            else
            {
                string asm = typeof(ActionTree_Node_Base).Assembly.FullName;
                // 目标类名字是拼接的，通过命名空间 + 类前缀 + 通用的 xg_ActionTreeType 枚举类型
                Type scriptType_Actiontree = Type.GetType($"{prefix_namespace}.{prefix_class}{action_nodeType},{asm}", true);

                // 在行为树根资源中加入新的数据项
                ActionTree_Node_Base database = ActionTreeNodeCreate(
                    prefix_namespace,
                    prefix_class,
                    scriptType_Actiontree,
                    action_nodeType,
                    icon,
                    visual_nodeType,
                    visualName);

                // 创建新的节点并指定资源数据项
                xGraphNode_Base visualNode = Node_Make(gv_NodeCreatedPosition, database);

                // 刷新节点
                visualNode.Draw();
                visualNode.RefreshExpandedState();
                visualNode.RefreshPorts();

                // 选中新节点
                AddToSelection(visualNode);
            }
        }
        #endregion

        #region 鼠标 & 键盘 & 委托
        /// <summary>
        /// 鼠标点击事件的回调函数
        /// </summary>
        /// <param assetName="e"></param>
        private void OnPointerDown(PointerDownEvent e)
        {
            // 获取鼠标点击位置
            Vector2 mousePosition = e.position;

            // 将鼠标位置从屏幕坐标转换为 xw_graphView 的局部坐标
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(mousePosition);

            gv_NodeCreatedPosition = localMousePosition;
        }
        /// <summary>
        /// 处理快捷键
        /// </summary>
        /// <param assetName="evt"></param>
        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.C && (evt.ctrlKey || evt.commandKey))
            {
                Node_Copy();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.V && (evt.ctrlKey || evt.commandKey))
            {
                Node_Paste(gv_NodeCreatedPosition);
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
                gv_GaphWindow.ActionTree_SaveAndReplace();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.O && (evt.ctrlKey || evt.commandKey))
            {
                gv_GaphWindow.ActionTree_Open();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.G && (evt.ctrlKey || evt.commandKey))
            {
                MakeGroup("节点编组", gv_NodeCreatedPosition);
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.R && (evt.ctrlKey || evt.commandKey))
            {
                // 刷新GraphView
                LoadThemes();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.C && ((evt.ctrlKey && evt.shiftKey) || evt.commandKey))
            {
                gv_GaphWindow.ActionTree_Clear();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.Escape)
            {
                gv_GaphWindow.Close();
                evt.StopPropagation();
            }
        }
        /// <summary>
        /// 注册XGraphWindow的 节点颜色标记开关状态委托
        /// </summary>
        public void RegisterNodeColorDisplayAction()
        {
            #region 注册XGraphWindow委托
            gv_GaphWindow.OnNodeColorToggleChanged += OnNodeColorToggleChanged;
            #endregion
        }
        /// <summary>
        /// 所有节点的颜色标记开关逻辑
        /// </summary>
        /// <param name="state"></param>
        private void OnNodeColorToggleChanged(bool state)
        {
            ActionTreeAsset.ActionTreeNodes.ForEach(data =>
            {
                nodes.ForEach(nodes =>
                {
                    if (data.nodeGUID == nodes.viewDataKey)
                    {
                        if (nodes is xGraphNode_Base bs)
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
        #endregion

        #region 节点列表搜索框实现
        /// <summary>
        /// 实例化节点搜索框的主体
        /// </summary>
        private void AddNodesSearchBox()
        {
            gv_NodesSearchBox = ScriptableObject.CreateInstance<xg_NodesSearchBox>();
            gv_NodesSearchBox.Init(this);

            // 当激活节点创建时使用实例化的节点搜索主体指定为搜索框的内容
            OpenNodeSearchBox();
        }
        /// <summary>
        /// 当激活节点创建时使用实例化的节点搜索主体指定为搜索框的内容
        /// </summary>
        private void OpenNodeSearchBox()
        {
            nodeCreationRequest = context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), gv_NodesSearchBox);
            };
        }
        #endregion
    }
}