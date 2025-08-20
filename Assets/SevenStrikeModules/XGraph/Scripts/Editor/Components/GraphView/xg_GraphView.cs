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
    using Color = UnityEngine.Color;
    using Edge = UnityEditor.Experimental.GraphView.Edge;

    #region 主题配置文件类结构
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
    #endregion

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
        private List<VNode_Base> gv_CopiedNodeList = new List<VNode_Base>();
        /// <summary>
        /// 当节点被选中时的回调委托
        /// </summary>
        public Action<VNode_Base> OnSelectedNode;
        /// <summary>
        /// 当节点被取消选中时的回调委托
        /// </summary>
        public Action<VNode_Base> OnUnSelectedNode;
        /// <summary>
        /// 当节点被选中时的回调委托
        /// </summary>
        public Action<List<VNode_Base>> OnSelectionNodes;
        /// <summary>
        /// 当节点被移除选中时的回调委托
        /// </summary>
        public Action<List<VNode_Base>> OnRemoveSelectionNodes;
        /// <summary>
        /// 当前正在编辑的资源
        /// </summary>
        public ActionNode_Asset ActionTreeAsset;
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
        private List<VNode_Base> CurrentSelectedNodes = new List<VNode_Base>();
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
            RegisterCallback<KeyDownEvent>(Action_KeyDown);

            // 注册鼠标点击事件
            RegisterCallback<PointerDownEvent>(Action_PointerDown, TrickleDown.TrickleDown);
            #endregion

            // 读取Group主题色方案
            LoadThemes();
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
        private void RefreshTheme_GraphNode(VNode_Base node)
        {
            // 应用配置文件的颜色到节点的标识颜色
            ThemesList.Node.ForEach(colorData =>
            {
                if (colorData.solution == node.ActionNode.themeSolution)
                {
                    node.ActionNode.themeColor = util_EditorUtility.Color_From_HexString(colorData.nodecolor);
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
            Node_Clear();

            // 清空GraphView的所有连线
            EdgesClear();

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
        private VNode_Base FindNodeView(string guid)
        {
            return GetNodeByGuid(guid) as VNode_Base;
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
        /// <param h_name="screenMousePosition">asdsadasdsad</param>
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

        #region 内部重写方法
        /// <summary>
        /// 添加到选择集中
        /// </summary>
        /// <param solution="selectable"></param>
        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            List<VNode_Base> gvs = new List<VNode_Base>();
            List<Group> gps = new List<Group>();
            selection.ForEach(n =>
            {
                if (n is VNode_Base node)
                    if (node.ActionNode.actionNodeType != "StickNote")
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
            List<VNode_Base> gvs = new List<VNode_Base>();
            List<Group> gps = new List<Group>();
            selection.ForEach(n =>
            {
                if (n is VNode_Base node)
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

        #endregion

        #region 连线规则
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
        #endregion      
    }
}