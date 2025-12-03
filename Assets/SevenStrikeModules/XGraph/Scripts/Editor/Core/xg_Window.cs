namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using Unity.Plastic.Newtonsoft.Json;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xg_Window : EditorWindow
    {
        #region 组件
        /// <summary>
        /// xw_graphView 视图组件
        /// </summary>
        internal xg_GraphView xw_graphView;
        /// <summary>
        /// InspectorViewer 属性视图组件（移动式）
        /// </summary>
        internal xg_InspectorView xw_InspectorView;
        /// <summary>
        /// 移动式属性视图容器组件
        /// </summary>
        internal VisualElement xw_InspectorView_Container;
        /// <summary>
        /// InspectorViewer 属性视图组件（移动式）
        /// </summary>
        internal xg_BlackBoardView xw_BlackBoardView;
        /// <summary>
        /// Graphview编辑器的选项面板分区组件 - RegOther
        /// </summary>
        internal VisualElement xw_Graphview_Icon;
        /// <summary>
        /// 黑板视图容器组件
        /// </summary>
        internal VisualElement xw_BlackBoardView_Container;
        /// <summary>
        /// Graphview编辑器的选项面板组件
        /// </summary>
        internal VisualElement xw_OptionsContainer;
        /// <summary>
        /// Graphview编辑器的选项面板折叠按钮组件容器
        /// </summary>
        internal VisualElement xw_OptionsPanel_ExpanderButton_Container;
        /// <summary>
        /// Graphview编辑器的选项面板分区组件 - RegColors
        /// </summary>
        internal VisualElement xw_OptionsContainerRegion_Reg_Color;
        /// <summary>
        /// Graphview编辑器的选项面板分区组件 - RegParams
        /// </summary>
        internal VisualElement xw_OptionsContainerRegion_Reg_Params;
        /// <summary>
        /// Graphview编辑器的选项面板分区组件 - RegRectangleSelector
        /// </summary>
        internal VisualElement xw_OptionsContainerRegion_Reg_RectangleSelector;
        /// <summary>
        /// Graphview编辑器的选项面板分区组件 - RegBlackBoards
        /// </summary>
        internal VisualElement xw_OptionsContainerRegion_Reg_BlackBoards;
        /// <summary>
        /// Graphview编辑器的选项面板分区组件 - RegOther
        /// </summary>
        internal VisualElement xw_OptionsContainerRegion_Reg_Other;
        /// <summary>
        /// Graphview编辑器的组件 - 行为资源信息容器
        /// </summary>
        internal VisualElement xw_GraphInfo_Container;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 背景颜色
        /// </summary>
        internal ColorField xw_OptionsPanel_Colorfield_Bg;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 网格颜色
        /// </summary>
        internal ColorField xw_OptionsPanel_Colorfield_Grid;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 分界线颜色
        /// </summary>
        internal ColorField xw_OptionsPanel_Colorfield_Thickline;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 背景图像着色
        /// </summary>
        internal ColorField xw_OptionsPanel_Colorfield_CustomImage;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 编辑器主题色
        /// </summary>
        internal ColorField xw_OptionsPanel_Colorfield_ThemeColor;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 黑板变量面板背景色
        /// </summary>
        internal ColorField xw_OptionsPanel_Colorfield_bg_blackboard;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 属性面板背景色
        /// </summary>
        internal ColorField xw_OptionsPanel_Colorfield_bg_inspector;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 选择框颜色
        /// </summary>
        internal ColorField xw_OptionsPanel_Colorfield_RectangleSelector;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 选择框线分段
        /// </summary>
        internal IntegerField xw_OptionsPanel_Integerfield_SelectorLineSegment;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 网格间距
        /// </summary>
        internal FloatField xw_OptionsPanel_Floatfield_GridSpace;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 分界线数量
        /// </summary>
        internal IntegerField xw_OptionsPanel_Integerfield_ThicklineCount;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 背景图像
        /// </summary>
        internal ObjectField xw_OptionsPanel_Objectfield_CustomImage;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 复位主题配色参数
        /// </summary>
        internal Button xw_OptionsPanel_Button_ResetThemeColors;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 设置主题配色参数
        /// </summary>
        internal Button xw_OptionsPanel_Button_SetThemeColors;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 复位背景参数
        /// </summary>
        internal Button xw_OptionsPanel_Button_ResetGridBackGround;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 复位选择框参数
        /// </summary>
        internal Button xw_OptionsPanel_Button_ResetRectangleSelector;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 显示选择框坐标
        /// </summary>
        internal Toggle xw_OptionsPanel_Toggle_DisplaySelectorCoordinate;
        /// <summary>
        /// Graphview编辑器的组件 - 选项面板 - 背景反转
        /// </summary>
        internal Toggle xw_OptionsPanel_Toggle_Invert_Bg_Gradient;
        #endregion

        #region 控件
        /// <summary>
        ///  xw_graphView 控件 - 移动式属性视图容器组件的标题
        /// </summary>
        internal Label xw_label_InspectorView_Container_Title;
        /// <summary>
        ///  xw_graphView 控件 - 黑板视图容器组件的标题
        /// </summary>
        internal Label xw_label_BlackBoardView_Container_Title;
        /// <summary>
        /// 用于显示和隐藏移动式属性视图容器组件
        /// </summary>
        internal Toggle xw_toggle_InspectorViewDisplay;
        /// <summary>
        /// 用于显示和隐藏黑板视图容器组件
        /// </summary>
        internal Toggle xw_toggle_BlackBoardViewDisplay;
        /// <summary>
        /// 用于显示和隐藏节点的颜色标记
        /// </summary>
        internal Toggle xw_toggle_DisplayNodeColor;
        /// <summary>
        /// 用于启用和禁用节点的数据流效果
        /// </summary>
        internal Toggle xw_toggle_DisplayNodeFlow;
        /// <summary>
        /// 用于显示和隐藏选项面板
        /// </summary>
        internal Toggle xw_toggle_Options;
        /// <summary>
        ///  xw_graphView 控件 - 标题
        /// </summary>
        private Label xw_label_graphTitle;
        /// <summary>
        ///  xw_graphView 控件 - 当前选择的节点的简介
        /// </summary>
        private Label xw_label_graph_nodeinfo_header;
        /// <summary>
        ///  xw_graphView 控件 - 当前选择的节点的路径
        /// </summary>
        private Label xw_label_graph_nodeinfo_footer;
        /// <summary>
        ///  xw_graphView 控件 - 水印文字
        /// </summary>
        private Label xw_label_graphMarkText;
        /// <summary>
        ///  xw_graphView 控件 - 保存按钮
        /// </summary>
        private Button xw_btn_save;
        /// <summary>
        ///  xw_graphView 控件 - 刷新按钮（用于快速重载节点）
        /// </summary>
        private Button xw_btn_reload;
        /// <summary>
        ///  xw_graphView 控件 - 打开按钮
        /// </summary>
        private Button xw_btn_open;
        /// <summary>
        ///  xw_graphView 控件 - 聚焦内容
        /// </summary>
        private Button xw_btn_FrameAll;
        /// <summary>
        ///  xw_graphView 控件 - 帮助与教程
        /// </summary>
        private Button xw_btn_Help;
        /// <summary>
        /// Graphview编辑器的选项面板折叠按钮组件
        /// </summary>
        internal Button xw_OptionsPanel_ExpanderButton;
        /// <summary>
        /// Graphview编辑器的选项面板关闭按钮组件
        /// </summary>
        internal Button xw_OptionsPanel_Btn_Close;
        /// <summary>
        ///  xw_graphView 控件 - 行为资源信息容器组件的显示资源路径的标题
        /// </summary>
        internal Label xw_GraphInfo_PathTitle;
        /// <summary>
        ///  xw_graphView 控件 - 行为资源信息容器组件的显示资源路径的内容
        /// </summary>
        internal Label xw_GraphInfo_PathContent;
        /// <summary>
        ///  xw_graphView 控件 - 行为资源信息容器组件的显示资源最后一次保存的时间&日期信息
        /// </summary>
        internal Label xw_GraphInfo_LastSaveDateTime;
        /// <summary>
        ///  xw_graphView 控件 - 行为资源信息容器组件的显示资源最后一次保存的时间&日期的时差
        /// </summary>
        internal Label xw_GraphInfo_LastSaveLag;
        /// <summary>
        ///  xw_graphView 控件 - 窗口尺寸显示文字
        /// </summary>
        private Label xw_label_GraphWindowSize;
        /// <summary>
        ///  xw_graphView 控件 - 窗口内鼠标在Graphview中的位置 X
        /// </summary>
        private Label xw_label_GraphMousePos_x;
        /// <summary>
        ///  xw_graphView 控件 - 窗口内鼠标在Graphview中的位置 Y
        /// </summary>
        private Label xw_label_GraphMousePos_y;
        #endregion

        #region 参数
        /// <summary>
        /// 当前选中的视觉节点
        /// </summary>
        public Node xw_currentSelectedVisualNode;
        /// <summary>
        /// 此参数用于当取消选中视觉节点的时候的单次执行的判断开关，
        /// </summary>
        bool xw_isUnSelectedNode;
        public bool xw_isSelectedVariable;
        /// <summary>
        /// GraphView窗口图标
        /// </summary>
        private Texture2D xw_graph_icon = null;
        private Vector2 dragOffset_InspectorView;
        private Vector2 dragOffset_BlackBoard;
        /// <summary>
        /// 选中节点时是否显示数据流效果
        /// </summary>
        public bool DisplayNodeFlow = true;
        #endregion

        #region 委托
        /// <summary>
        /// 当节点颜色标记开关改变时的回调委托
        /// </summary>
        public Action<bool> OnNodeColorToggleChanged;
        /// <summary>
        /// 当节点数据流开关改变时的回调委托
        /// </summary>
        public Action<bool> OnNodeFlowToggleChanged;
        /// <summary>
        /// 当编辑器主题色变化时
        /// </summary>
        public Action<Color> OnThemeColorChanged;
        #endregion

        #region 资源类
        /// <summary>
        /// 原始行为树复制体，放置修改源资源，保证安全修改
        /// </summary>
        public xAction_Asset CloneTree;
        /// <summary>
        /// 原始行为树
        /// </summary>
        public xAction_Asset SourceTree;
        #endregion

        /// <summary>
        /// 窗口是否改变了尺寸
        /// </summary>
        private bool isWindowResizing = false;
        /// <summary>
        /// 窗口最后一次尺寸
        /// </summary>
        private Vector2 lastWindowSize;
        /// <summary>
        /// 节点编辑器窗口是否已经准备就绪
        /// </summary>
        public bool NodeEditorIsReady = false;

        /// <summary>
        /// 打开资源节点编辑器
        /// </summary>
        /// <param root_title="id"></param>
        /// <param root_title="line"></param>
        /// <returns></returns>
        [OnOpenAsset(1)]
        public static bool OnOpenAssets(int id, int line)
        {
            if (EditorUtility.InstanceIDToObject(id) is xAction_Asset datatree)
            {
                #region 加载窗口
                // 注意：执行顺序强调 ！！！ GetWindow 方法会先触发执行 CreateGUI 方法然后再继续下面的代码
                xg_Window wnd = GetWindow<xg_Window>();

                wnd.titleContent = new GUIContent($"XGraph");

                #region 克隆资源
                // 保留原始资源引用
                wnd.SourceTree = datatree;
                // 安全深度克隆资源以保护源资源，待修改满意后使用保存覆盖源资源
                wnd.CloneTree = datatree.Clone();
                #endregion

                #region 恢复上一次退出 GraphView 时记录的内视图位置以及缩放等级
                wnd.xw_graphView.SetViewPosition(wnd.CloneTree.LastGraphViewPosition, wnd.CloneTree.LastGraphViewZoom);
                #endregion

                #region 属性面板的状态恢复
                // 获取最后一次的移动式属性面板开关状态
                bool inspector_view_toggle = wnd.CloneTree.XGraph_InspectorViewDisplay;
                // 设置 InspectorViewer 容器可见性
                util_XGraphEditorUtility.Element_Dispaly_Set(wnd.xw_InspectorView_Container, inspector_view_toggle);
                // 设置移动式属性视图容器可见性按钮开关状态
                wnd.xw_toggle_InspectorViewDisplay.value = inspector_view_toggle;

                if (inspector_view_toggle)
                {
                    if (wnd.CloneTree != null)
                        wnd.xw_InspectorView.InspectorViewer(wnd.CloneTree);
                }
                wnd.xw_InspectorView_Container.style.backgroundColor = wnd.CloneTree.GraphviewGridBackgroundThemes.inspector_bgcolor;
                wnd.InspectorViewAction_SetTitle($"{wnd.SourceTree.name} 行为根节点属性");
                #endregion

                #region 黑板面板的状态恢复
                // 获取最后一次的黑板面板开关状态
                bool blackboard_view_toggle = wnd.CloneTree.XGraph_BlackBoardViewDisplay;
                // 设置 BlackBoardView  容器可见性
                util_XGraphEditorUtility.Element_Dispaly_Set(wnd.xw_BlackBoardView_Container, blackboard_view_toggle);
                // 设置黑板视图容器可见性按钮开关状态
                wnd.xw_toggle_BlackBoardViewDisplay.value = blackboard_view_toggle;
                EditorApplication.delayCall += () =>
                {
                    // 刷新 BlackBoard 标题显示
                    wnd.xw_BlackBoard_UpdateTitleInfo();
                    // 刷新 BlackBoard 属性列表
                    wnd.xw_BlackBoard_VariablesRestructure();
                };
                wnd.BlackBoardViewAction_SetTitle($"{wnd.SourceTree.name} 变量黑板");
                #endregion

                #region Node节点颜色标记的状态恢复
                // 获取最后一次的Node节点颜色标记状态
                bool nodeColorDisplayState = wnd.CloneTree.XGraph_DisplayNodeColor;
                // 设置Node节点颜色标记按钮开关状态
                wnd.xw_toggle_DisplayNodeColor.value = nodeColorDisplayState;
                wnd.DisplayNodeFlow = nodeColorDisplayState;
                EditorApplication.delayCall += () =>
                {
                    if (wnd.OnNodeColorToggleChanged != null)
                        wnd.OnNodeColorToggleChanged(nodeColorDisplayState);
                };
                #endregion

                #region Node节点数据流的状态恢复
                // 获取最后一次的Node节点数据流状态
                bool nodeFlowDisplayState = wnd.CloneTree.XGraph_DisplayNodeFlow;
                // 设置Node节点数据流按钮开关状态
                wnd.xw_toggle_DisplayNodeFlow.value = nodeFlowDisplayState;

                EditorApplication.delayCall += () =>
                {
                    if (wnd.OnNodeFlowToggleChanged != null)
                        wnd.OnNodeFlowToggleChanged(nodeFlowDisplayState);
                };
                #endregion

                // 用于记录资源的原始路径，便于重新编译 & 运行状态切换 资源重载的保险操作
                //EditorPrefs.SetString("XGraph->ActionTreePath_Source", AssetDatabase.GetAssetPath(wnd.SourceTree));
                //EditorPrefs.SetString("XGraph->ActionTreePath_Clone", AssetDatabase.GetAssetPath(wnd.CloneTree));

                // 如果最后一次窗口尺寸值不为0则使用最后一次的窗口尺寸，否则就是用默认窗口尺寸，这里使用的 SourceTree 的原因是因为窗口尺寸这个变量不受克隆影响
                xw_CenterEditorWindow(wnd.SourceTree.LastGraphWindowSize == Vector2Int.zero ? new Vector2Int(1330, 800) : wnd.SourceTree.LastGraphWindowSize, wnd);
                #endregion

                #region 根据资源结构重建可视化行为树节点
                // 当 GraphView 组件不为空时
                if (wnd.xw_graphView != null)
                {
                    wnd.xw_graphView.Restructure_Graph(wnd.CloneTree);
                    //Debug.Log("打开 XGraphView 并加载节点信息！");
                }
                #endregion

                // 加载 BlackBoardView 面板变换信息
                wnd.Element_Transform_Load(wnd.CloneTree.Last_GraphView_InspectorPanel_TransformData, wnd.xw_InspectorView_Container);
                // 加载 BlackBoardView 面板变换信息
                wnd.Element_Transform_Load(wnd.CloneTree.Last_GraphView_BlackboardPanel_TransformData, wnd.xw_BlackBoardView_Container);

                return true;
            }
            return false;
        }

        /// <summary>
        /// 编辑器界面创建逻辑
        /// </summary>
        public void CreateGUI()
        {
            // 窗口根节点
            VisualElement root = rootVisualElement;

            #region 布局样式
            // 读取并克隆uxml布局到 root 布局
            var visual_window = util_XGraphEditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_Window.uxml");
            visual_window.CloneTree(root);

            // 读取uss样式到 root 布局
            util_XGraphEditorUtility.ElementStyle_Add(root, $"{util_Dashboard.GetPath_GUI_Uss()}uss_Window.uss");

            // 设置图标
            Texture2D icon = xw_Toolbar_IconSet(util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/main.png"));
            xw_Graphview_Icon = util_XGraphEditorUtility.GetUIElement<VisualElement>(root, "graphIcon");
            xw_Graphview_Icon.style.backgroundImage = new StyleBackground(icon);
            #endregion

            #region 找到并获取 GraphView | InspectorView | BlackBoardView | GraphviewOptions 组件
            // 在布局中找到 xw_graphView 组件
            xw_graphView = root.Q<xg_GraphView>();
            xw_graphView.gv_GraphWindow = this;
            xw_graphView.Action_Register_NodeColorDisplayer();

            #region 注册GraphView事件
            // 创建节点时注册的（点击节点时）的回调，用于将与之对应的资源节点的属性展示在内置自定义Inspector面板上
            xw_graphView.OnSelectedNode = OnSelectNodeView;
            // 创建节点时注册的（取消点击节点时）的回调，用于将Inspector面板清空
            xw_graphView.OnUnSelectedNode = OnUnSelectNodeView;
            // 监听 GraphView 的 selection 变化
            xw_graphView.OnSelectionNodes = OnSelectionNodesView;
            // 监听 GraphView 的 selection 移除节点变化
            xw_graphView.OnRemoveSelectionNodes = OnRemovedSelectionNodesView;
            // 选择连线时注册的（点击节点时）的回调
            xw_graphView.OnSelectionEdges = OnSelectEdges;
            // 取消选择连线时注册的（点击节点时）的回调
            xw_graphView.OnRemoveSelectionEdges = OnRemoveSelectionEdges;
            // 创建节点时注册的（取消点击节点时）的回调，用于将Inspector面板清空
            xw_graphView.OnUnSelectedEdge = OnUnSelectedEdge;
            #endregion

            #region BlackBoardView ---------- 初始化
            // 在布局中找到 InspectorViewer 容器组件
            xw_BlackBoardView_Container = root.Q<VisualElement>("BlackBoardView_Container");

            // 设置 InspectorViewer 容器组件最小尺寸
            xg_ResizableElement ele_blackboard = (xg_ResizableElement)xw_BlackBoardView_Container;
            ele_blackboard.SetMinSize(new Vector2(250, 320));

            // 在布局中找到 BlackBoardView 的组件
            xw_BlackBoardView = xw_BlackBoardView_Container.Q<xg_BlackBoardView>("BlackBoardView");
            xw_BlackBoardView.graphWindow = this;

            xw_BlackBoardView.titlecontainer = xw_BlackBoardView_Container.Q<VisualElement>("titleContainer");
            xw_BlackBoardView.graphstatistic = xw_BlackBoardView_Container.Q<VisualElement>("GraphStatistic");
            xw_BlackBoardView.icon_title = xw_BlackBoardView_Container.Q<Label>("icon");
            xw_BlackBoardView.label_title = xw_BlackBoardView_Container.Q<Label>("text");
            xw_BlackBoardView.label_sub = xw_BlackBoardView_Container.Q<Label>("sub");
            xw_BlackBoardView.btn_AddVariable = xw_BlackBoardView_Container.Q<Button>("btnadd");

            xw_BlackBoardView.BringToFront();

            // BlackBoardView 的ListView组件初始化
            xw_BlackBoardView.Initialize();

            // 添加拖动支持
            Element_Drag(ele_blackboard, ele_blackboard, dragOffset_BlackBoard);

            // 在布局中找到 BlackBoardView Remote 容器标题组件
            xw_label_BlackBoardView_Container_Title = xw_BlackBoardView_Container.Q<Label>("BlackBoardView_Container_Title");
            xw_label_BlackBoardView_Container_Title.SendToBack();

            //ele_blackboard.RegisterCallback<GeometryChangedEvent>((evt) =>
            //{
            //    // 保存 BlackBoardView 面板的变换信息
            //    CloneTree.Last_GraphView_BlackboardPanel_TransformData = Element_Transform_Save(xw_BlackBoardView_Container);
            //});
            #endregion

            #region InspectorView ---------- 初始化
            // 在布局中找到 InspectorViewer 容器组件
            xw_InspectorView_Container = root.Q<VisualElement>("InspectorView_Container");

            // 设置 InspectorViewer 容器组件最小尺寸
            xg_ResizableElement ele_inspector = (xg_ResizableElement)xw_InspectorView_Container;
            ele_inspector.SetMinSize(new Vector2(250, 320));

            // 在布局中找到 InspectorViewer 组件
            xw_InspectorView = root.Q<xg_InspectorView>("InspectorView");
            xw_InspectorView.SendToBack();
            xw_InspectorView.InitializeStyle();
            xw_InspectorView.graphwindow = this;

            // 添加拖动支持
            Element_Drag(ele_inspector, ele_inspector, dragOffset_InspectorView);

            // 在布局中找到 InspectorViewer Remote 容器标题组件
            xw_label_InspectorView_Container_Title = root.Q<Label>("InspectorView_Container_Title");
            xw_label_InspectorView_Container_Title.SendToBack();

            //ele_inspector.RegisterCallback<GeometryChangedEvent>((evt) =>
            //{
            //    // 保存 InspectorViewer 面板的变换信息
            //    CloneTree.Last_GraphView_InspectorPanel_TransformData = Element_Transform_Save(xw_InspectorView_Container);
            //});
            #endregion

            #endregion

            #region 找到并获取 OptionsPanel | 背景各类颜色和参数组件
            #region  读取并克隆选项面板布局树元素
            var xw_GraphviewOptionsPanel = util_XGraphEditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_OptionsPanel.uxml").CloneTree().Q<VisualElement>("OptionsPanel");
            root.Add(xw_GraphviewOptionsPanel);

            util_XGraphEditorUtility.ElementStyle_Add(root, $"{util_Dashboard.GetPath_GUI_Uss()}uss_OptionsPanel.uss");

            xw_OptionsContainer = xw_GraphviewOptionsPanel.Q<VisualElement>("OptionsContainer");
            xw_OptionsContainer.AddToClassList("OptionsContainer");
            xw_OptionsContainer.AddToClassList("OptionsContainer_Hide");

            xw_OptionsPanel_ExpanderButton_Container = xw_GraphviewOptionsPanel.Q<VisualElement>(name: "ExpandContainer");
            #endregion

            #region 选项面板分区组件
            // 颜色配置区域
            xw_OptionsContainerRegion_Reg_Color = xw_OptionsContainer.Q<VisualElement>("reg_color");
            // 参数配置区域
            xw_OptionsContainerRegion_Reg_Params = xw_OptionsContainer.Q<VisualElement>("reg_params");
            // 参数配置区域
            xw_OptionsContainerRegion_Reg_RectangleSelector = xw_OptionsContainer.Q<VisualElement>("reg_rectselector");
            // 暂未开放
            xw_OptionsContainerRegion_Reg_BlackBoards = xw_OptionsContainer.Q<VisualElement>("reg_blackboards");
            // 暂未开放
            xw_OptionsContainerRegion_Reg_Other = xw_OptionsContainer.Q<VisualElement>("reg_other");
            #endregion

            VisualElement vm_col_bg = xw_OptionsContainerRegion_Reg_Color.Q<VisualElement>(name: "color_bg");
            VisualElement vm_col_grid = xw_OptionsContainerRegion_Reg_Color.Q<VisualElement>(name: "color_grid");
            VisualElement vm_col_thickline = xw_OptionsContainerRegion_Reg_Color.Q<VisualElement>(name: "color_thickline");
            VisualElement vm_col_customimg = xw_OptionsContainerRegion_Reg_Color.Q<VisualElement>(name: "color_bgimage");
            VisualElement vm_col_theme = xw_OptionsContainerRegion_Reg_Color.Q<VisualElement>(name: "color_themecolor");
            VisualElement vm_col_blackboard = xw_OptionsContainerRegion_Reg_Color.Q<VisualElement>(name: "color_bg_blackboard");
            VisualElement vm_col_inspector = xw_OptionsContainerRegion_Reg_Color.Q<VisualElement>(name: "color_bg_inspector");
            VisualElement vm_toggle_invert_bg_gradient = xw_OptionsContainerRegion_Reg_Color.Q<VisualElement>(name: "toggle_invert_bg_gradient");

            #region GraphView背景颜色组件
            xw_OptionsPanel_Colorfield_Bg = vm_col_bg.Q<ColorField>(name: "colorfield");
            xw_OptionsPanel_Colorfield_Bg.RegisterValueChangedCallback(on_xw_OptionsPanel_Color_Bg_changed);
            vm_col_bg.Q<Label>(name: "title").RegisterCallback<ClickEvent>((evt) =>
            {
                if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
                {
                    util_XGraphEditorUtility.CopyToClipboard(util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_Bg.value, true));
                }
            });
            #endregion

            #region GraphView背景网格颜色组件
            xw_OptionsPanel_Colorfield_Grid = vm_col_grid.Q<ColorField>(name: "colorfield");
            xw_OptionsPanel_Colorfield_Grid.RegisterValueChangedCallback(on_xw_OptionsPanel_Colorfield_Grid_changed);
            vm_col_grid.Q<Label>(name: "title").RegisterCallback<ClickEvent>((evt) =>
            {
                if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
                {
                    util_XGraphEditorUtility.CopyToClipboard(util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_Grid.value, true));
                }
            });
            #endregion

            #region GraphView背景网格分界线颜色组件
            xw_OptionsPanel_Colorfield_Thickline = vm_col_thickline.Q<ColorField>(name: "colorfield");
            xw_OptionsPanel_Colorfield_Thickline.RegisterValueChangedCallback(on_xw_OptionsPanel_Colorfield_Thickline_changed);
            vm_col_thickline.Q<Label>(name: "title").RegisterCallback<ClickEvent>((evt) =>
            {
                if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
                {
                    util_XGraphEditorUtility.CopyToClipboard(util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_Thickline.value, true));
                }
            });
            #endregion

            #region GraphView背景图像着色颜色组件
            xw_OptionsPanel_Colorfield_CustomImage = vm_col_customimg.Q<ColorField>(name: "colorfield");
            xw_OptionsPanel_Colorfield_CustomImage.RegisterValueChangedCallback(on_xw_OptionsPanel_Colorfield_CustomImage_changed);
            vm_col_customimg.Q<Label>(name: "title").RegisterCallback<ClickEvent>((evt) =>
            {
                if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
                {
                    util_XGraphEditorUtility.CopyToClipboard(util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_CustomImage.value, true));
                }
            });
            #endregion

            #region GraphView主题色颜色组件
            xw_OptionsPanel_Colorfield_ThemeColor = vm_col_theme.Q<ColorField>(name: "colorfield");
            xw_OptionsPanel_Colorfield_ThemeColor.RegisterValueChangedCallback(on_xw_OptionsPanel_Colorfield_ThemeColor_changed);
            vm_col_theme.Q<Label>(name: "title").RegisterCallback<ClickEvent>((evt) =>
            {
                if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
                {
                    util_XGraphEditorUtility.CopyToClipboard(util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_ThemeColor.value, true));
                }
            });
            #endregion

            #region BlackBoard 背景色颜色组件
            xw_OptionsPanel_Colorfield_bg_blackboard = vm_col_blackboard.Q<ColorField>(name: "colorfield");
            xw_OptionsPanel_Colorfield_bg_blackboard.RegisterValueChangedCallback(on_xw_OptionsPanel_Colorfield_bg_blackboard_changed);
            vm_col_blackboard.Q<Label>(name: "title").RegisterCallback<ClickEvent>((evt) =>
            {
                if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
                {
                    util_XGraphEditorUtility.CopyToClipboard(util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_bg_blackboard.value, true));
                }
            });
            #endregion

            #region Inspector 背景色颜色组件
            xw_OptionsPanel_Colorfield_bg_inspector = vm_col_inspector.Q<ColorField>(name: "colorfield");
            xw_OptionsPanel_Colorfield_bg_inspector.RegisterValueChangedCallback(on_xw_OptionsPanel_Colorfield_bg_inspector_changed);
            vm_col_inspector.Q<Label>(name: "title").RegisterCallback<ClickEvent>((evt) =>
            {
                if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
                {
                    util_XGraphEditorUtility.CopyToClipboard(util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_bg_inspector.value, true));
                }
            });
            #endregion

            #region Inspector 背景色颜色组件
            xw_OptionsPanel_Toggle_Invert_Bg_Gradient = vm_toggle_invert_bg_gradient.Q<Toggle>(name: "toggle");
            xw_OptionsPanel_Toggle_Invert_Bg_Gradient.RegisterValueChangedCallback(on_xw_OptionsPanel_Toggle_Invert_Bg_Gradient_changed);
            #endregion

            #region GraphView主题配色复位按钮
            xw_OptionsPanel_Button_ResetThemeColors = xw_OptionsContainerRegion_Reg_Color.Q<Button>(name: "button_theme_reset");
            xw_OptionsPanel_Button_ResetThemeColors.clicked += xw_OptionsPanel_Button_ResetThemeColors_Clicked;
            #endregion

            #region GraphView主题配色设置按钮
            xw_OptionsPanel_Button_SetThemeColors = xw_OptionsContainerRegion_Reg_Color.Q<Button>(name: "button_theme_set");
            xw_OptionsPanel_Button_SetThemeColors.RegisterCallback<ClickEvent>(xw_OptionsPanel_Button_SetThemeColors_Clicked);
            #endregion

            #region GraphView背景网格间距输入框组件
            xw_OptionsPanel_Floatfield_GridSpace = xw_OptionsContainerRegion_Reg_Params.Q<FloatField>(name: "floatfield_gridspace");
            xw_OptionsPanel_Floatfield_GridSpace.RegisterCallback<BlurEvent>(on_xw_OptionsPanel_Textfield_GridSpace_changed);
            #endregion

            #region GraphView背景网格分界线数量输入框组件
            xw_OptionsPanel_Integerfield_ThicklineCount = xw_OptionsContainerRegion_Reg_Params.Q<IntegerField>(name: "integerfield_thicklinecount");
            xw_OptionsPanel_Integerfield_ThicklineCount.RegisterCallback<BlurEvent>(on_xw_OptionsPanel_Textfield_ThicklineCount_changed);
            #endregion

            #region GraphView背景图像对象框组件
            xw_OptionsPanel_Objectfield_CustomImage = xw_OptionsContainerRegion_Reg_Params.Q<ObjectField>(name: "objectfield_customimage");
            xw_OptionsPanel_Objectfield_CustomImage.RegisterValueChangedCallback(on_xw_OptionsPanel_Objectfield_CustomImage_changed);
            #endregion

            #region GraphView背景参数复位按钮
            xw_OptionsPanel_Button_ResetGridBackGround = xw_OptionsContainerRegion_Reg_Params.Q<Button>(name: "button_reset");
            xw_OptionsPanel_Button_ResetGridBackGround.clicked += xw_OptionsPanel_Button_Reset_Clicked;
            #endregion

            //-------------------------------- 黑板标记解释

            #region BlackBoard 的变量类型的颜色标记物

            VisualElement paramtypes = xw_OptionsContainerRegion_Reg_BlackBoards.Q<VisualElement>(name: "paramTypes");

            // 获取所有名称为dot的 VisualElement
            var dotElements = paramtypes.Query<VisualElement>(name: "dot").ToList();

            for (int i = 0; i < dotElements.Count; i++)
            {
                VisualElement dot = dotElements[i];
                string dotparentName = dot.parent.name;
                for (int s = 0; s < xw_BlackBoardView.VariableThemeList.VariableThemes.Count; s++)
                {
                    if (dotparentName == $"param_type_{xw_BlackBoardView.VariableThemeList.VariableThemes[s].type}")
                    {
                        dot.style.backgroundColor = util_XGraphEditorUtility.Color_From_HexString(xw_BlackBoardView.VariableThemeList.VariableThemes[s].color);
                    }
                }
            }

            // 为BlackBoard 的标记添加鼠标悬停时在节点视图中显示同类型节点高亮
            var labelElements = paramtypes.Query<Label>().ToList();
            foreach (var item in labelElements)
            {
                // 便于查找高亮节点
                string typeName = item.name.Substring(4, item.name.Length - 4);
                item.userData = typeName;

                item.RegisterCallback<PointerEnterEvent>(ValriablesInGraphview_SyncDisplay);
                item.RegisterCallback<PointerLeaveEvent>(ValriablesInGraphview_SyncHide);
            }
            #endregion

            //-------------------------------- 选择框

            #region RectangleSelector 选择框显示坐标
            xw_OptionsPanel_Toggle_DisplaySelectorCoordinate = xw_OptionsContainerRegion_Reg_RectangleSelector.Q<Toggle>(name: "toggle_selectorline_displaycoordinate");
            xw_OptionsPanel_Toggle_DisplaySelectorCoordinate.RegisterValueChangedCallback(xw_OptionsPanel_Toggle_DisplaySelectorCoordinate_changed);
            #endregion

            #region RectangleSelector 选择框线颜色
            xw_OptionsPanel_Colorfield_RectangleSelector = xw_OptionsContainerRegion_Reg_RectangleSelector.Q<ColorField>(name: "colorfield_selectorlinecolor");
            xw_OptionsPanel_Colorfield_RectangleSelector.RegisterValueChangedCallback(on_xw_OptionsPanel_Colorfield_RectangleSelector_changed);
            #endregion

            #region RectangleSelector 选择框线分段
            xw_OptionsPanel_Integerfield_SelectorLineSegment = xw_OptionsContainerRegion_Reg_RectangleSelector.Q<IntegerField>(name: "integerfield_selectorline_seg");
            xw_OptionsPanel_Integerfield_SelectorLineSegment.RegisterCallback<BlurEvent>(on_xw_OptionsPanel_Integerfield_SelectorLineSegment_changed);
            #endregion

            #region RectangleSelector 复位按钮
            xw_OptionsPanel_Button_ResetRectangleSelector = xw_OptionsContainerRegion_Reg_RectangleSelector.Q<Button>(name: "button_reset");
            xw_OptionsPanel_Button_ResetRectangleSelector.clicked += xw_OptionsPanel_Button_ResetRectangleSelector_Clicked;
            #endregion

            #endregion

            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    ele_inspector.SnapToNearestQuadrant();
                    ele_blackboard.SnapToNearestQuadrant();
                };
            };

            #region GraphIntros 组件
            xw_label_graphTitle = root.Q<Label>("graphTitle");
            xw_label_graph_nodeinfo_header = root.Q<Label>("graph_nodeinfo_header");
            xw_label_graph_nodeinfo_footer = root.Q<Label>("graph_nodeinfo_footer");
            xw_label_graphMarkText = root.Q<Label>("graphintro_MarkText");
            #endregion

            #region 工具栏按钮组件
            // 保存按钮
            xw_btn_save = root.Q<Button>("btn_Save");
            xw_btn_save.clicked += xw_btn_save_clicked;

            // 重载节点按钮
            xw_btn_reload = root.Q<Button>("btn_Reload");
            xw_btn_reload.clicked += xw_btn_reload_clicked;

            // 打开按钮
            xw_btn_open = root.Q<Button>("btn_Load");
            xw_btn_open.clicked += xw_btn_open_clicked;

            // 重聚焦按钮
            xw_btn_FrameAll = root.Q<Button>("btn_FrameAll");
            xw_btn_FrameAll.clicked += xw_btn_FrameAll_clicked;

            // 帮助按钮
            xw_btn_Help = root.Q<Button>("btn_Help");
            xw_btn_Help.clicked += xw_btn_Help_clicked;


            // Inspector面板开关按钮
            xw_toggle_InspectorViewDisplay = root.Q<Toggle>("toggle_InspectorViewDisplay");
            xw_toggle_InspectorViewDisplay.RegisterValueChangedCallback(xw_toggle_inspectorDisplay_changed);

            // BlackBoard面板开关按钮
            xw_toggle_BlackBoardViewDisplay = root.Q<Toggle>("toggle_BlackBoardViewDisplay");
            xw_toggle_BlackBoardViewDisplay.RegisterValueChangedCallback(xw_toggle_BlackBoardDisplay_changed);

            // 节点颜色标记显示开关按钮
            xw_toggle_DisplayNodeColor = root.Q<Toggle>("toggle_DisplayNodeColor");
            xw_toggle_DisplayNodeColor.RegisterValueChangedCallback(xw_toggle_DisplayNodeColor_changed);

            // 节点数据流启用开关按钮
            xw_toggle_DisplayNodeFlow = root.Q<Toggle>("toggle_DisplayNodeFlow");
            xw_toggle_DisplayNodeFlow.RegisterValueChangedCallback(xw_toggle_DisplayNodeFlow_changed);

            #region 选项面板
            // 选项面板开关按钮
            xw_toggle_Options = root.Q<Toggle>("toggle_Options");
            xw_toggle_Options.RegisterValueChangedCallback(xw_toggle_OptionsPanel_changed);

            // 选项面板折叠按钮
            xw_OptionsPanel_ExpanderButton = xw_OptionsPanel_ExpanderButton_Container.Q<Button>(name: "btn_expand");
            xw_OptionsPanel_ExpanderButton.clicked += xw_btn_OptionsPanel_ExpanderButton_changed;
            OptionsPanel_ExpanderButton_Display();

            // 选项面板关闭按钮
            xw_OptionsPanel_Btn_Close = xw_OptionsContainerRegion_Reg_Other.Q<Button>(name: "btn_close");
            xw_OptionsPanel_Btn_Close.clicked += xw_btn_OptionsPanel_CloseButton_Clicked;
            OptionsPanel_CloseButton_Hide();
            #endregion
            #endregion

            #region 页脚信息容器
            xw_GraphInfo_Container = root.Q<VisualElement>("GraphInfo");
            xw_GraphInfo_Container.BringToFront();

            // 行为资源信息容器 - 路径标题
            xw_GraphInfo_PathTitle = xw_GraphInfo_Container.Q<VisualElement>("left").Q<Label>("pathtitle");
            // 行为资源信息容器 - 路径内容
            xw_GraphInfo_PathContent = xw_GraphInfo_Container.Q<VisualElement>("left").Q<Label>("pathcontent");
            xw_GraphInfo_PathContent.RegisterCallback<PointerDownEvent>(xw_GraphInfo_PathContent_clicked);
            // 行为资源信息容器 - 最后一次保存日期 & 时间
            xw_GraphInfo_LastSaveDateTime = xw_GraphInfo_Container.Q<VisualElement>("right").Q<Label>("lastsavedatetime");
            // 行为资源信息容器 - 最后一次保存日期 & 时间的时差
            xw_GraphInfo_LastSaveLag = xw_GraphInfo_Container.Q<VisualElement>("right").Q<Label>("lastsavelag");
            xw_GraphInfo_LastSaveLag.style.color = Color.white * 0.8f;
            // 窗口尺寸大小显示
            xw_label_GraphWindowSize = xw_GraphInfo_Container.Q<VisualElement>("center").Q<Label>("size");
            // 鼠标位置
            xw_label_GraphMousePos_x = xw_GraphInfo_Container.Q<VisualElement>("front").Q<Label>("mousepos_x");
            xw_label_GraphMousePos_y = xw_GraphInfo_Container.Q<VisualElement>("front").Q<Label>("mousepos_y");
            #endregion          
        }

        /// <summary>
        /// 编辑器界面绘制逻辑
        /// </summary>
        private void OnGUI()
        {
            xw_SetFlowNames();
        }

        private void OnEnable()
        {
            // 注册撤销逻辑
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            // 初始化窗口尺寸记录
            lastWindowSize = position.size;

            EditorApplication.update -= OnWindowResizingUpdate;
            EditorApplication.update += OnWindowResizingUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnWindowResizingUpdate;
        }

        private void OnDestroy()
        {
            // 注销撤销逻辑
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;

            xw_DestroyGraphView();
        }

        #region 重新编译时XGraph资源重载操作
        /// <summary>
        /// 使行为树编辑器在脚本重新编辑后重新加载目标行为树资源，而不会导致引用丢失产生报错
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Reloader_In_ScriptRecomplier()
        {
            AssemblyReloadEvents.afterAssemblyReload += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    xg_Window win = util_XGraphEditorUtility.GetExistingGraphviewWindow();

                    // 没有窗口打开，直接返回
                    if (win == null)
                        return;

                    win.ReloadTreeFromPath(win.SourceTree, win.CloneTree);
                };
            };
        }
        /// <summary>
        /// 重新引用行为树编辑器的目标行为树资源
        /// </summary>
        /// <param name="tree_source"></param>
        /// <param name="tree_clone"></param>
        public void ReloadTreeFromPath(xAction_Asset tree_source, xAction_Asset tree_clone)
        {
            if (tree_source == null) return;

            xw_graphView.UnregisterGroupEvent();
            xw_graphView.OnDuplicateNodes = null;

            // 清理旧数据 
            xw_graphView.Node_Clear();
            xw_graphView.EdgesClear();
            xw_graphView.Groups_Clear();

            #region 恢复上一次退出 GraphView 时记录的内视图位置以及缩放等级
            xw_graphView.SetViewPosition(tree_source.LastGraphViewPosition, tree_source.LastGraphViewZoom);
            #endregion

            #region 移动式属性面板的状态恢复
            // 获取最后一次的移动式属性面板开关状态
            bool remote_toggle = CloneTree.XGraph_InspectorViewDisplay;
            // 设置 InspectorViewer Remote 容器可见性
            util_XGraphEditorUtility.Element_Dispaly_Set(xw_InspectorView_Container, remote_toggle);
            // 设置移动式属性视图容器可见性按钮开关状态
            xw_toggle_InspectorViewDisplay.value = remote_toggle;
            if (remote_toggle)
                // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                xw_InspectorView.InspectorViewer(tree_clone);

            xg_ResizableElement element_inspector = (xg_ResizableElement)xw_InspectorView_Container;
            // 加载 RemoteInspector 变换信息
            Element_Transform_Load(CloneTree.Last_GraphView_InspectorPanel_TransformData, element_inspector);

            // 加载 BlackBoard 面板标题文字
            InspectorViewAction_SetTitle($"{SourceTree.name} 行为根节点属性");
            #endregion

            #region 黑板变量面板的状态恢复
            // 获取最后一次的移动式变量面板开关状态
            bool blackboard_toggle = CloneTree.XGraph_BlackBoardViewDisplay;
            // 设置 InspectorViewer Remote 容器可见性
            util_XGraphEditorUtility.Element_Dispaly_Set(xw_BlackBoardView_Container, blackboard_toggle);
            // 设置黑板变量视图容器可见性按钮开关状态
            xw_toggle_BlackBoardViewDisplay.value = blackboard_toggle;

            // 刷新 BlackBoard 显示
            xw_BlackBoard_UpdateTitleInfo();
            // 刷新 BlackBoard 属性列表
            xw_BlackBoard_VariablesRestructure();

            xg_ResizableElement element_blackboard = (xg_ResizableElement)xw_BlackBoardView_Container;
            // 加载 BlackBoard 面板位置
            Element_Transform_Load(CloneTree.Last_GraphView_BlackboardPanel_TransformData, element_blackboard);

            // 加载 BlackBoard 面板标题文字
            BlackBoardViewAction_SetTitle($"{SourceTree.name} 变量黑板");
            #endregion

            xw_graphView.RegisterGroupEvent();

            // 重新加载行为树资源
            SourceTree = tree_source;
            CloneTree = tree_clone;

            // 延迟重建可视化行为树结构
            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    element_inspector.SnapToNearestQuadrant();

                    xw_graphView.Restructure_Graph(CloneTree);

                    /*  以下逻辑必须保证先让 xw_graphView 的ActionTree不为空才行否则会报错，
                     *  而 xw_graphView?.Restructure_Graph(CloneTree); 正是将 CloneTree 赋值到  xw_graphView 中的 ActionTreeAsset 的逻辑根源
                     */
                    #region Node节点颜色标记的状态恢复
                    // 获取最后一次的节点颜色标记开关状态
                    bool nodeColor_toggle = CloneTree.XGraph_DisplayNodeColor;
                    // 设置节点颜色标记可见性按钮开关状态
                    xw_toggle_DisplayNodeColor.value = nodeColor_toggle;
                    #endregion

                    #region Node节点数据流连线的状态恢复
                    // 获取最后一次的节点数据流连线开关状态
                    bool nodeanimateEdge_toggle = CloneTree.XGraph_DisplayNodeFlow;
                    // 设置节点数据流连线可见性按钮开关状态
                    xw_toggle_DisplayNodeFlow.value = nodeanimateEdge_toggle;
                    #endregion
                };
            };
        }
        #endregion

        #region 视觉节点回调
        #region 节点类
        /// <summary>
        /// 当选中视觉节点时执行
        /// </summary>
        /// <param name="nodeview"></param>
        private void OnSelectNodeView(Node nodeview)
        {
            if (nodeview == null) return;

            if (xw_InspectorView == null)
                return;
            xw_currentSelectedVisualNode = nodeview;

            // 清空 Inspector 视图
            xw_InspectorView.ClearInspector();

            // 选中的节点为：行为节点
            if (nodeview is xNode_Base n_base)
            {
                // 选中的节点为：内部变量节点
                if (n_base is xNode_Variable_Internal n_vare_internal)
                {
                    Node_InternalVariable_Selected(n_vare_internal);
                }
                // 选中的节点为：内部变量节点
                else if (n_base is xNode_Property n_property)
                {
                    Node_Property_Selected(n_property);
                }
                // 选中的节点为：行为节点
                else
                {
                    Node_Action_Selected(n_base);
                }

                // Inspector 面板显示属性
                xw_InspectorView.InspectorViewer(nodeview);
            }
            // 选中的节点为：黑板变量节点
            else if (nodeview is xNode_Variable n_vare)
            {
                Node_Variable_Selected(n_vare);
            }
            // 选中的节点为：贴图节点
            else if (nodeview is xNode_Decal n_decal)
            {
                Node_Decal_Selected(n_decal);
            }
            // 选中的节点为：便签节点
            else if (nodeview is xNode_Stick n_stick)
            {
                Node_Stick_Selected(n_stick);
            }
            // 选中的节点为：标签节点
            else if (nodeview is xNode_Label n_label)
            {
                Node_Label_Selected(n_label);
            }

            xw_isUnSelectedNode = false;
        }
        /// <summary>
        /// 当选中多个视觉节点时执行
        /// </summary>
        /// <param name="nodeviews"></param>
        private void OnSelectionNodesView(List<Node> nodeviews)
        {
            if (nodeviews == null) return;

            if (nodeviews.Count == 1)
            {
                if (xw_InspectorView == null)
                    return;

                Node selectedNode = nodeviews[0];
                xw_currentSelectedVisualNode = selectedNode;

                // 清空 Inspector 视图
                xw_InspectorView.ClearInspector();

                // 当点击任意一个节点时调用 移动式 Inspector 面板显示对应的资源节点的属性
                xw_InspectorView.InspectorViewer(selectedNode);

                // 选中的节点为：行为节点
                if (selectedNode is xNode_Base n_base)
                {
                    // 选中的节点为：内部变量节点
                    if (n_base is xNode_Variable_Internal n_vare_internal)
                    {
                        Node_InternalVariable_Selected(n_vare_internal);
                    }
                    // 选中的节点为：行为节点
                    else
                    {
                        Node_Action_Selected(n_base);
                    }
                }
                // 选中的节点为：黑板变量节点
                else if (selectedNode is xNode_Variable n_vare)
                {
                    Node_Variable_Selected(n_vare);
                }
                // 选中的节点为：贴图节点
                else if (selectedNode is xNode_Decal n_decal)
                {
                    Node_Decal_Selected(n_decal);
                }
                // 选中的节点为：便签节点
                else if (selectedNode is xNode_Stick n_stick)
                {
                    Node_Stick_Selected(n_stick);
                }
                // 选中的节点为：标签节点
                else if (selectedNode is xNode_Label n_label)
                {
                    Node_Label_Selected(n_label);
                }

                xw_isUnSelectedNode = false;
            }
            else if (nodeviews.Count > 1)
            {
                if (xw_InspectorView == null)
                    return;
                xw_currentSelectedVisualNode = null;

                // 清空 Inspector 视图
                xw_InspectorView.ClearInspector();

                // 加载 Inspector 面板标题文字
                InspectorViewAction_SetTitle($"节点属性 - 多选状态");
                xw_SetNodeInfos("-", "-");
            }
        }
        /// <summary>
        /// 当从选中的所有视觉节点中移除某一个选择时执行
        /// </summary>
        /// <param name="nodeviews"></param>
        private void OnRemovedSelectionNodesView(List<Node> nodeviews)
        {
            if (nodeviews == null) return;
            if (nodeviews.Count > 1 || nodeviews.Count == 0)
                OnSelectionNodesView(nodeviews);
            else
                OnSelectNodeView(nodeviews[0]);
        }
        /// <summary>
        /// 取消选中视觉节点时执行
        /// </summary>
        /// <param name="nodeview"></param>
        private void OnUnSelectNodeView(Node nodeview)
        {
            if (!xw_isUnSelectedNode)
            {
                xw_isUnSelectedNode = true;

                if (!xw_isSelectedVariable)
                {
                    // 清空 Inspector 视图
                    xw_InspectorView.ClearInspector();

                    // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                    xw_InspectorView.InspectorViewer(CloneTree);

                    // 加载 Inspector 面板标题文字
                    InspectorViewAction_SetTitle($"{SourceTree.name} 行为根节点属性");
                }

                xw_currentSelectedVisualNode = null;

                // 节点的类型信息 - 清空
                xw_SetNodeInfos(null, null);
            }
        }
        #endregion

        #region 连线类
        /// <summary>
        /// 当选中连线时执行
        /// </summary>
        /// <param name="edges"></param>
        private void OnSelectEdges(List<util_AnimatedEdge> edges)
        {
            if (edges.Count == 1)
            {
                if (xw_InspectorView == null)
                    return;

                util_AnimatedEdge selectedEdge = edges[0];

                // 清空 Inspector 视图
                xw_InspectorView.ClearInspector();

                // 加载 Inspector 面板标题文字
                InspectorViewAction_SetTitle($"连线状态");

                // 当点击任意一个节点时调用 移动式 Inspector 面板显示对应的资源节点的属性
                xw_InspectorView.InspectorViewer(selectedEdge);
            }
            else if (edges.Count > 1)
            {
                if (xw_InspectorView == null)
                    return;
                xw_currentSelectedVisualNode = null;

                // 清空 Inspector 视图
                xw_InspectorView.ClearInspector();

                // 加载 Inspector 面板标题文字
                InspectorViewAction_SetTitle($"连线属性 - 多选状态");
                xw_SetNodeInfos("-", "-");
            }
        }
        /// <summary>
        /// 当选中连线时执行
        /// </summary>
        /// <param name="edges"></param>
        private void OnSelectEdge(util_AnimatedEdge edge)
        {
            // 清空 Inspector 视图
            xw_InspectorView.ClearInspector();

            // 加载 Inspector 面板标题文字
            InspectorViewAction_SetTitle($"连线状态");

            // 当点击任意一个节点时调用 移动式 Inspector 面板显示对应的资源节点的属性
            xw_InspectorView.InspectorViewer(edge);
        }
        /// <summary>
        /// 当取消选中连线时执行
        /// </summary>
        /// <param name="edges"></param>
        private void OnRemoveSelectionEdges(List<util_AnimatedEdge> edges)
        {
            if (edges == null) return;
            if (edges.Count > 1 || edges.Count == 0)
                OnSelectEdges(edges);
            else
                OnSelectEdge(edges[0]);
        }
        /// <summary>
        /// 取消选中连线节点时执行
        /// </summary>
        /// <param name="nodeview"></param>
        private void OnUnSelectedEdge()
        {
            // 清空 Inspector 视图
            xw_InspectorView.ClearInspector();

            // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
            xw_InspectorView.InspectorViewer(CloneTree);

            // 加载 Inspector 面板标题文字
            InspectorViewAction_SetTitle($"{SourceTree.name} 行为根节点属性");
            // 节点的类型信息 - 清空
            xw_SetNodeInfos(null, null);
        }
        #endregion
        #endregion

        #region 选中节点时简要信息展示
        /// <summary>
        /// 选中节点：变量
        /// </summary>
        /// <param name="n_vare"></param>
        private void Node_Variable_Selected(xNode_Variable n_vare)
        {
            // 加载 Inspector 面板标题文字
            InspectorViewAction_SetTitle($"黑板变量节点");
            // 显示当前选中的节点的类型信息
            xw_SetNodeInfos($"{n_vare.VariableData.variable.name}  /  {n_vare.VariableData.variable.GetActiveType()}  /  {n_vare.VariableData.variable.guid}", $"值：{n_vare.VariableData.variable.GetValue()}");
        }
        /// <summary>
        /// 选中节点：行为
        /// </summary>
        /// <param name="n_vare"></param>
        private void Node_Action_Selected(xNode_Base n_base)
        {
            // 加载 Inspector 面板标题文字
            InspectorViewAction_SetTitle($"行为节点");
            // 显示当前选中的节点的类型信息
            xw_SetNodeInfo_Header(n_base.ActionData.GetInfo());
        }
        /// <summary>
        /// 选中节点：内部变量
        /// </summary>
        /// <param name="n_vare"></param>
        private void Node_InternalVariable_Selected(xNode_Variable_Internal n_vare_internal)
        {
            // 加载 Inspector 面板标题文字
            InspectorViewAction_SetTitle($"内部变量节点");
            // 显示当前选中的节点的类型信息
            xw_SetNodeInfos($"{n_vare_internal.VariableData.variable.name}  /  {n_vare_internal.VariableData.variable.GetActiveType()}  /  {n_vare_internal.VariableData.guid}", $"值：{n_vare_internal.VariableData.variable.GetValue()}");
        }
        /// <summary>
        /// 选中节点：属性
        /// </summary>
        /// <param name="n_vare"></param>
        private void Node_Property_Selected(xNode_Property n_property)
        {
            // 加载 Inspector 面板标题文字
            InspectorViewAction_SetTitle($"属性节点");
            // 显示当前选中的节点的类型信息
            //xw_SetNodeInfos($"{n_vare_internal.VariableData.variable.name}  /  {n_vare_internal.VariableData.variable.GetActiveType()}  /  {n_vare_internal.VariableData.guid}", $"值：{n_vare_internal.VariableData.variable.GetValue()}");
        }
        /// <summary>
        /// 选中节点：标签
        /// </summary>
        /// <param name="n_vare"></param>
        private void Node_Label_Selected(xNode_Label n_label)
        {
            // 加载 Inspector 面板标题文字
            InspectorViewAction_SetTitle($"标签节点");
            // 显示当前选中的节点的类型信息
            xw_SetNodeInfos($"{n_label.LabelData.guid}  /  {n_label.LabelData.position.ToString()}  /  {n_label.LabelData.size.ToString()}", $"{n_label.LabelData.content}");
        }
        /// <summary>
        /// 选中节点：便签
        /// </summary>
        /// <param name="n_vare"></param>
        private void Node_Stick_Selected(xNode_Stick n_stick)
        {
            // 加载 Inspector 面板标题文字
            InspectorViewAction_SetTitle($"便签节点");
            // 显示当前选中的节点的类型信息
            xw_SetNodeInfos($"{n_stick.StickData.guid}  /   {n_stick.StickData.position.ToString()}  /   {n_stick.StickData.size.ToString()}", $"{n_stick.StickData.content}");
        }
        /// <summary>
        /// 选中节点：贴图
        /// </summary>
        /// <param name="n_vare"></param>
        private void Node_Decal_Selected(xNode_Decal n_decal)
        {
            // 加载 Inspector 面板标题文字
            InspectorViewAction_SetTitle($"贴图节点");
            // 显示当前选中的节点的类型信息
            xw_SetNodeInfos($"{n_decal.DecalData.guid}  /   {n_decal.DecalData.position.ToString()}  /   {n_decal.DecalData.size.ToString()}", $"{n_decal.DecalData.DecalTexture}");
        }
        #endregion

        #region 控件逻辑
        #region OptionsPanel逻辑
        #region OptionsPanel_GraphView背景颜色组件逻辑      
        /// <summary>
        /// OptionsPanel_GraphView背景颜色组件值改变时
        /// </summary>
        /// <param name="evt"></param>
        private void on_xw_OptionsPanel_Color_Bg_changed(ChangeEvent<Color> evt)
        {
            Undo.RecordObject(CloneTree, "Change Graphview BgColor");
            CloneTree.GraphviewGridBackgroundThemes.bgcolor = evt.newValue;
            xw_graphView.GridBackgroundThemeUpdate();

            string col_tooltip = util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_Bg.value, true, true);
            xw_OptionsPanel_Colorfield_Bg.tooltip = col_tooltip;
        }
        #endregion

        #region OptionsPanel_GraphViewGrid颜色组件逻辑
        private void on_xw_OptionsPanel_Colorfield_Grid_changed(ChangeEvent<Color> evt)
        {
            Undo.RecordObject(CloneTree, "Change Graphview GridColor");
            CloneTree.GraphviewGridBackgroundThemes.gridcolor = evt.newValue;
            xw_graphView.GridBackgroundThemeUpdate();

            xw_OptionsPanel_Colorfield_Grid.tooltip = util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_Grid.value, true, true);
        }
        #endregion

        #region OptionsPanel_GraphViewThickline颜色组件逻辑
        private void on_xw_OptionsPanel_Colorfield_Thickline_changed(ChangeEvent<Color> evt)
        {
            Undo.RecordObject(CloneTree, "Change Graphview ThicklineColor");
            CloneTree.GraphviewGridBackgroundThemes.thickLinecolor = evt.newValue;
            xw_graphView.GridBackgroundThemeUpdate();

            xw_OptionsPanel_Colorfield_Thickline.tooltip = util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_Thickline.value, true, true);
        }
        #endregion

        #region OptionsPanel_GraphViewCustomImage颜色组件逻辑
        private void on_xw_OptionsPanel_Colorfield_CustomImage_changed(ChangeEvent<Color> evt)
        {
            Undo.RecordObject(CloneTree, "Change Graphview CustomImageColor");
            CloneTree.GraphviewGridBackgroundThemes.customimagecolor = evt.newValue;
            xw_graphView.GridBackgroundThemeUpdate();

            xw_OptionsPanel_Colorfield_CustomImage.tooltip = util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_CustomImage.value, true, true);
        }
        #endregion

        #region OptionsPanel_GraphViewThemeColor主题色组件逻辑
        private void on_xw_OptionsPanel_Colorfield_ThemeColor_changed(ChangeEvent<Color> evt)
        {
            Undo.RecordObject(CloneTree, "Change Graphview ThemeColor");
            CloneTree.GraphviewGridBackgroundThemes.themecolor = evt.newValue;

            xw_GraphInfo_GraphViewIcon_ColorSyncUpdate();
            xw_GraphInfo_LastSaveLag_ColorSyncUpdate();
            xw_GraphInfo_LastSaveLag_ColorSyncUpdate();
            xw_label_graph_nodeinfo_header_ColorSyncUpdate();
            xw_label_graph_nodeinfo_footer_ColorSyncUpdate();

            xw_OptionsPanel_Colorfield_ThemeColor.tooltip = util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_ThemeColor.value, true, true);

            if (OnThemeColorChanged != null)
                OnThemeColorChanged.Invoke(CloneTree.GraphviewGridBackgroundThemes.themecolor);
        }
        #endregion

        #region OptionsPanel_BlackBoard 背景色组件逻辑
        private void on_xw_OptionsPanel_Colorfield_bg_inspector_changed(ChangeEvent<Color> evt)
        {
            Undo.RecordObject(CloneTree, "Change Graphview bg_inspectorColor");
            CloneTree.GraphviewGridBackgroundThemes.inspector_bgcolor = evt.newValue;

            util_XGraphEditorUtility.Element_BackgroundColor_Set(xw_InspectorView_Container, CloneTree.GraphviewGridBackgroundThemes.inspector_bgcolor);

            xw_OptionsPanel_Colorfield_bg_inspector.tooltip = util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_bg_inspector.value, true, true);
        }
        #endregion

        #region OptionsPanel_Inspector 背景色组件逻辑
        private void on_xw_OptionsPanel_Colorfield_bg_blackboard_changed(ChangeEvent<Color> evt)
        {
            Undo.RecordObject(CloneTree, "Change Graphview bg_blackboardColor");
            CloneTree.GraphviewGridBackgroundThemes.blackboard_bgcolor = evt.newValue;

            util_XGraphEditorUtility.Element_BackgroundColor_Set(xw_BlackBoardView_Container, CloneTree.GraphviewGridBackgroundThemes.blackboard_bgcolor);

            xw_OptionsPanel_Colorfield_bg_blackboard.tooltip = util_XGraphEditorUtility.Color_To_HexColor(xw_OptionsPanel_Colorfield_bg_blackboard.value, true, true);
        }
        #endregion

        #region OptionsPanel_GraphView网格间距组件逻辑
        private void on_xw_OptionsPanel_Textfield_GridSpace_changed(BlurEvent evt)
        {
            Undo.RecordObject(CloneTree, "Set GraphView GridSpace");
            CloneTree.GraphviewGridBackgroundThemes.spacing = xw_OptionsPanel_Floatfield_GridSpace.value;
            xw_graphView.GridBackgroundThemeUpdate();
        }
        #endregion

        #region OptionsPanel_GraphView分界线数量组件逻辑
        private void on_xw_OptionsPanel_Textfield_ThicklineCount_changed(BlurEvent evt)
        {
            Undo.RecordObject(CloneTree, "Set GraphView ThicklineCount");
            CloneTree.GraphviewGridBackgroundThemes.thicklines = xw_OptionsPanel_Integerfield_ThicklineCount.value;
            xw_graphView.GridBackgroundThemeUpdate();
        }
        #endregion

        #region OptionsPanel_GraphView背景图像组件逻辑
        private void on_xw_OptionsPanel_Objectfield_CustomImage_changed(ChangeEvent<UnityEngine.Object> evt)
        {
            Undo.RecordObject(CloneTree, "Set GraphView CustomBg Texture");
            CloneTree.GraphviewGridBackgroundThemes.customimage = evt.newValue as Texture2D;
            xw_graphView.GridBackgroundThemeUpdate();
        }
        #endregion

        #region OptionsPanel_Toggle 组件逻辑
        /// <summary>
        /// toggle_Options 开关改变状态时
        /// </summary>
        /// <param name="evt"></param>
        private void xw_toggle_OptionsPanel_changed(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                OptionsPanel_Display();
                xw_OptionsPanel_ExpanderButton_Hide();
                OptionsPanel_CloseButton_Display();
            }
            else
            {
                OptionsPanel_Hide();
                OptionsPanel_ExpanderButton_Display();
                OptionsPanel_CloseButton_Hide();
            }
        }
        /// <summary>
        /// 显示选项面板
        /// </summary>
        public void OptionsPanel_Display()
        {
            xw_OptionsContainer.AddToClassList("OptionsContainer_Display");
            xw_OptionsContainer.RemoveFromClassList("OptionsContainer_Hide");
        }
        /// <summary>
        /// 隐藏选项面板
        /// </summary>
        public void OptionsPanel_Hide()
        {
            xw_OptionsContainer.AddToClassList("OptionsContainer_Hide");
            xw_OptionsContainer.RemoveFromClassList("OptionsContainer_Display");
        }
        /// <summary>
        /// 选项面板按钮的开关状态设置
        /// </summary>
        /// <param name="state"></param>
        public void OptionsPanel_ToggleChange_WithoutNotify(bool state)
        {
            xw_toggle_Options.SetValueWithoutNotify(state);
        }
        #endregion

        #region OptionsPanel_CloseButton 组件逻辑
        /// <summary>
        /// 选项面板的关闭按钮点击动作
        /// </summary>
        private void xw_btn_OptionsPanel_CloseButton_Clicked()
        {
            OptionsPanel_Hide();
            OptionsPanel_ToggleChange_WithoutNotify(false);
            OptionsPanel_ExpanderButton_Display();
            OptionsPanel_CloseButton_Hide();
        }
        /// <summary>
        /// 显示选项面板的关闭按钮
        /// </summary>
        public void OptionsPanel_CloseButton_Display()
        {
            xw_OptionsPanel_Btn_Close.AddToClassList("btn_close_Display");
            xw_OptionsPanel_Btn_Close.RemoveFromClassList("btn_close_Hide");
        }
        /// <summary>
        /// 隐藏选项面板的关闭按钮
        /// </summary>
        public void OptionsPanel_CloseButton_Hide()
        {
            xw_OptionsPanel_Btn_Close.AddToClassList("btn_close_Hide");
            xw_OptionsPanel_Btn_Close.RemoveFromClassList("btn_close_Display");
        }
        #endregion

        #region OptionsPanel_ExpanderButton 组件逻辑
        /// <summary>
        /// 选项面板折叠按钮点击动作
        /// </summary>
        private void xw_btn_OptionsPanel_ExpanderButton_changed()
        {
            OptionsPanel_ToggleChange_WithoutNotify(true);
            xw_OptionsPanel_ExpanderButton_Hide();
            OptionsPanel_Display();
            OptionsPanel_CloseButton_Display();
        }
        /// <summary>
        /// 选项面板折叠按钮 - 显示
        /// </summary>
        public void OptionsPanel_ExpanderButton_Display()
        {
            xw_OptionsPanel_ExpanderButton.AddToClassList("btn_expand_Display");
            xw_OptionsPanel_ExpanderButton.RemoveFromClassList("btn_expand_Hide");
        }
        /// <summary>
        /// 选项面板折叠按钮 - 隐藏
        /// </summary>
        public void xw_OptionsPanel_ExpanderButton_Hide()
        {
            xw_OptionsPanel_ExpanderButton.AddToClassList("btn_expand_Hide");
            xw_OptionsPanel_ExpanderButton.RemoveFromClassList("btn_expand_Display");
        }
        #endregion

        #region OptionsPanel Graphview 选项面板的背景参数复位
        /// <summary>
        /// 复位选项面板的所有背景参数控件的值，以及背景参数的原始值复位
        /// </summary>
        private void xw_OptionsPanel_Button_Reset_Clicked()
        {
            CloneTree.GraphviewGridBackgroundThemes.spacing = 18;
            CloneTree.GraphviewGridBackgroundThemes.thicklines = 18;
            CloneTree.GraphviewGridBackgroundThemes.customimage = null;

            OptionsPanel_ParamsUpdate();

            xw_graphView.GridBackgroundThemeUpdate();
        }
        #endregion

        #region OptionsPanel Graphview 选项面板的主题配色设置
        /// <summary>
        /// 弹出主题配色菜单对节点编辑器配色样式设定
        /// </summary>
        private void xw_OptionsPanel_Button_SetThemeColors_Clicked(ClickEvent evt)
        {
            Button btn = evt.target as Button;

            if (btn != null)
            {
                // 获取按钮在屏幕上的位置
                Vector2 screenPosition = btn.worldBound.position;

                // 调整位置，让菜单出现在按钮下方
                screenPosition.y += btn.worldBound.height;

                // 创建上下文菜单
                var menu = new GenericMenu();

                // 读取菜单结构列表内容
                string json = util_XGraphEditorUtility.AssetLoad<TextAsset>($"{util_Dashboard.GetPath_Config()}/cfg_Themes_Window.json").text;
                WindowThemeList themelist = JsonConvert.DeserializeObject<WindowThemeList>(json);

                foreach (var theme in themelist.Themes)
                {
                    // 添加菜单项
                    menu.AddItem(new GUIContent(theme.solution), false, () =>
                    {
                        CloneTree.GraphviewGridBackgroundThemes.bgcolor = util_XGraphEditorUtility.Color_From_HexString(theme.col_bg);
                        CloneTree.GraphviewGridBackgroundThemes.gridcolor = util_XGraphEditorUtility.Color_From_HexString(theme.col_grid);
                        CloneTree.GraphviewGridBackgroundThemes.customimagecolor = util_XGraphEditorUtility.Color_From_HexString(theme.col_customimage);
                        CloneTree.GraphviewGridBackgroundThemes.thickLinecolor = util_XGraphEditorUtility.Color_From_HexString(theme.col_thickLine);
                        CloneTree.GraphviewGridBackgroundThemes.themecolor = util_XGraphEditorUtility.Color_From_HexString(theme.col_theme);
                        CloneTree.GraphviewGridBackgroundThemes.blackboard_bgcolor = util_XGraphEditorUtility.Color_From_HexString(theme.col_blackboard_bg);
                        CloneTree.GraphviewGridBackgroundThemes.inspector_bgcolor = util_XGraphEditorUtility.Color_From_HexString(theme.col_inspector_bg);
                        CloneTree.GraphviewGridBackgroundThemes.spacing = theme.val_griddistance;
                        CloneTree.GraphviewGridBackgroundThemes.thicklines = theme.val_thicklines;

                        xw_graphView.GridBackgroundThemeUpdate();
                        OptionsPanel_ParamsUpdate();
                    });
                }

                // 显示菜单
                menu.DropDown(new Rect(screenPosition, Vector2.zero));
            }

            // 阻止事件继续传播
            evt.StopPropagation();
        }
        #endregion

        #region OptionsPanel Graphview 选项面板的主题配色复位
        /// <summary>
        /// 复位选项面板的主题配色参数
        /// </summary>
        private void xw_OptionsPanel_Button_ResetThemeColors_Clicked()
        {
            CloneTree.GraphviewGridBackgroundThemes.bgcolor = new Color(0.1647059f, 0.1647059f, 0.1647059f, 0.6862745f);
            CloneTree.GraphviewGridBackgroundThemes.gridcolor = new Color(0.5803922f, 0.5803922f, 0.5803922f, 0.04705882f);
            CloneTree.GraphviewGridBackgroundThemes.customimagecolor = new Color(0.5607843f, 0.5607843f, 0.5607843f, 0.9294118f);
            CloneTree.GraphviewGridBackgroundThemes.thickLinecolor = new Color(0.5529412f, 0.5529412f, 0.5529412f, 0f);
            CloneTree.GraphviewGridBackgroundThemes.themecolor = new Color(0.2313726f, 0.9882353f, 0.6f, 1);
            CloneTree.GraphviewGridBackgroundThemes.blackboard_bgcolor = new Color(0.2705882f, 0.2705882f, 0.2705882f, 0.8784314f);
            CloneTree.GraphviewGridBackgroundThemes.inspector_bgcolor = new Color(0.2705882f, 0.2705882f, 0.2705882f, 0.8784314f);
            CloneTree.GraphviewGridBackgroundThemes.customimage = null;

            OptionsPanel_ParamsUpdate();

            xw_graphView.GridBackgroundThemeUpdate();
        }
        #endregion

        #region OptionsPanel Graphview 选项面板的主题背景反转
        /// <summary>
        /// 选项面板的背景渐变反转
        /// </summary>
        /// <param name="evt"></param>
        private void on_xw_OptionsPanel_Toggle_Invert_Bg_Gradient_changed(ChangeEvent<bool> evt)
        {
            Undo.RecordObject(CloneTree, "Change Graphview Invert_Bg_Gradient");
            CloneTree.GraphviewGridBackgroundThemes.InvertBgGradient = evt.newValue;

            xw_graphView.GridBackgroundThemeUpdate();
            OptionsPanel_ParamsUpdate();
        }
        #endregion

        #region OptionsPanel Graphview 选项面板的所有UI参数刷新
        /// <summary>
        /// 刷新选项面板的所有参数控件的值
        /// </summary>
        private void OptionsPanel_ParamsUpdate()
        {
            // OptionsPanel_GraphView主题颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(xw_OptionsPanel_Colorfield_ThemeColor, CloneTree.GraphviewGridBackgroundThemes.themecolor);

            // OptionsPanel_GraphView背景颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(xw_OptionsPanel_Colorfield_Bg, CloneTree.GraphviewGridBackgroundThemes.bgcolor);

            // OptionsPanel_GraphView属性面板背景颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(xw_OptionsPanel_Colorfield_bg_inspector, CloneTree.GraphviewGridBackgroundThemes.inspector_bgcolor);

            // OptionsPanel_GraphView黑板面板背景颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(xw_OptionsPanel_Colorfield_bg_blackboard, CloneTree.GraphviewGridBackgroundThemes.blackboard_bgcolor);

            // OptionsPanel_GraphView网格颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(xw_OptionsPanel_Colorfield_Grid, CloneTree.GraphviewGridBackgroundThemes.gridcolor);

            // OptionsPanel_GraphView分界线颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(xw_OptionsPanel_Colorfield_Thickline, CloneTree.GraphviewGridBackgroundThemes.thickLinecolor);

            // OptionsPanel_GraphView背景图像颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(xw_OptionsPanel_Colorfield_CustomImage, CloneTree.GraphviewGridBackgroundThemes.customimagecolor);

            // OptionsPanel_GraphView网格间距值设置
            util_XGraphEditorUtility.Element_FloatField_ValueSet(xw_OptionsPanel_Floatfield_GridSpace, CloneTree.GraphviewGridBackgroundThemes.spacing);

            // OptionsPanel_GraphView网格分界线值设置
            util_XGraphEditorUtility.Element_IntegerField_ValueSet(xw_OptionsPanel_Integerfield_ThicklineCount, CloneTree.GraphviewGridBackgroundThemes.thicklines);

            // OptionsPanel_GraphView背景图像值设置
            util_XGraphEditorUtility.Element_ObjectField_ValueSet(xw_OptionsPanel_Objectfield_CustomImage, CloneTree.GraphviewGridBackgroundThemes.customimage);

            // OptionsPanel_GraphView 选择框坐标显示开关值设置
            util_XGraphEditorUtility.Element_ToggleField_ValueSet(xw_OptionsPanel_Toggle_DisplaySelectorCoordinate, CloneTree.GraphviewRectangleSelectorThemes.displayCoordinate);

            // OptionsPanel_GraphView 选择框坐标显示开关值设置
            util_XGraphEditorUtility.Element_ToggleField_ValueSet(xw_OptionsPanel_Toggle_Invert_Bg_Gradient, CloneTree.GraphviewGridBackgroundThemes.InvertBgGradient);

            // OptionsPanel_GraphView 选择框线分段值设置
            util_XGraphEditorUtility.Element_IntegerField_ValueSet(xw_OptionsPanel_Integerfield_SelectorLineSegment, CloneTree.GraphviewRectangleSelectorThemes.segments);

            // OptionsPanel_GraphView 选择框线颜色值设置
            util_XGraphEditorUtility.Element_ColorField_ValueSet(xw_OptionsPanel_Colorfield_RectangleSelector, CloneTree.GraphviewRectangleSelectorThemes.rectangleSelectorLineColor);
        }
        #endregion

        //-------------------------------- 黑板标记解释

        #region OptionsPanel Graphview 选项面板的标记在鼠标悬停时在节点视图中显示同类型节点高亮
        /// <summary>
        /// 鼠标悬停时在节点视图中同类型节点高亮
        /// </summary>
        /// <param name="evt"></param>
        private void ValriablesInGraphview_SyncDisplay(PointerEnterEvent evt)
        {
            Label label = evt.target as Label;

            #region 鼠标悬停样式           
            for (int s = 0; s < xw_BlackBoardView.VariableThemeList.VariableThemes.Count; s++)
            {
                if ((string)label.userData == xw_BlackBoardView.VariableThemeList.VariableThemes[s].type)
                {
                    util_XGraphEditorUtility.Element_BackgroundColor_Set(label, util_XGraphEditorUtility.Color_From_HexString(xw_BlackBoardView.VariableThemeList.VariableThemes[s].color));
                    util_XGraphEditorUtility.Element_Color_Set(label, Color.black);
                }
            }
            #endregion

            #region 高亮显示节点
            xVariableType v_type = (xVariableType)Enum.Parse(typeof(xVariableType), (string)label.userData);
            List<xNode_Variable> vNode_Variables = xw_BlackBoardView.FindVariableNodes(v_type);
            foreach (xNode_Variable v in vNode_Variables)
            {
                v.Highlight();
            }
            #endregion
        }
        /// <summary>
        /// 鼠标离开时在节点视图中同类型节点恢复正常
        /// </summary>
        /// <param name="evt"></param>
        private void ValriablesInGraphview_SyncHide(PointerLeaveEvent evt)
        {
            Label label = evt.target as Label;

            #region 鼠标离开样式
            label.style.backgroundColor = Color.clear;
            util_XGraphEditorUtility.Element_BackgroundColor_Set(label, Color.clear);
            util_XGraphEditorUtility.Element_Color_Set(label, Color.white);
            #endregion

            #region 取消高亮显示节点
            xVariableType v_type = (xVariableType)Enum.Parse(typeof(xVariableType), (string)label.userData);
            List<xNode_Variable> vNode_Variables = xw_BlackBoardView.FindVariableNodes(v_type);
            foreach (var v in vNode_Variables)
            {
                v.UnHighlight();
            }
            #endregion
        }
        #endregion

        //-------------------------------- 选择框

        #region RectangleSelector 复位按钮
        /// <summary>
        /// 当点击选择框选项中的复位按钮时
        /// </summary>
        private void xw_OptionsPanel_Button_ResetRectangleSelector_Clicked()
        {
            Undo.RecordObject(CloneTree, "Change Graphview RectangleSelectorTheme");
            CloneTree.GraphviewRectangleSelectorThemes.segments = 4;
            CloneTree.GraphviewRectangleSelectorThemes.rectangleSelectorLineColor = new Color(1, 1, 1, 0.6f);
            CloneTree.GraphviewRectangleSelectorThemes.displayCoordinate = false;
            xw_graphView.RectangleSelectorThemeUpdate(CloneTree.GraphviewRectangleSelectorThemes);

            OptionsPanel_ParamsUpdate();
        }
        #endregion

        #region RectangleSelector 选择框线分段
        /// <summary>
        /// 当改变选择框选项中的分段数的值时
        /// </summary>
        /// <param name="evt"></param>
        private void on_xw_OptionsPanel_Integerfield_SelectorLineSegment_changed(BlurEvent evt)
        {
            Undo.RecordObject(CloneTree, "Change Graphview RectangleSelectorTheme");
            CloneTree.GraphviewRectangleSelectorThemes.segments = xw_OptionsPanel_Integerfield_SelectorLineSegment.value;
            xw_graphView.RectangleSelectorThemeUpdate(CloneTree.GraphviewRectangleSelectorThemes);
        }
        #endregion

        #region RectangleSelector 选择框线颜色
        /// <summary>
        /// 当改变选择框选项中的选择框线的颜色值时
        /// </summary>
        /// <param name="evt"></param>
        private void on_xw_OptionsPanel_Colorfield_RectangleSelector_changed(ChangeEvent<Color> evt)
        {
            Undo.RecordObject(CloneTree, "Change Graphview RectangleSelectorTheme");
            CloneTree.GraphviewRectangleSelectorThemes.rectangleSelectorLineColor = evt.newValue;
            xw_graphView.RectangleSelectorThemeUpdate(CloneTree.GraphviewRectangleSelectorThemes);
        }
        #endregion

        #region RectangleSelector 选择框显示坐标开关
        /// <summary>
        /// 当改变选择框选项中的显示选择框的坐标的开关时
        /// </summary>
        /// <param name="evt"></param>
        private void xw_OptionsPanel_Toggle_DisplaySelectorCoordinate_changed(ChangeEvent<bool> evt)
        {
            Undo.RecordObject(CloneTree, "Change Graphview RectangleSelectorTheme");
            CloneTree.GraphviewRectangleSelectorThemes.displayCoordinate = evt.newValue;
            xw_graphView.RectangleSelectorThemeUpdate(CloneTree.GraphviewRectangleSelectorThemes);
        }
        #endregion
        #endregion

        #region 工具栏按钮逻辑
        /// <summary>
        /// 重载节点逻辑
        /// </summary>
        private void xw_btn_reload_clicked()
        {
            RestructureGraphViews();
        }
        /// <summary>
        /// 打开按钮逻辑
        /// </summary>
        private void xw_btn_open_clicked()
        {
            ActionTree_Open();
        }
        /// <summary>
        /// 保存按钮逻辑
        /// </summary>
        private void xw_btn_save_clicked()
        {
            ActionTree_SaveAndReplace();
        }
        /// <summary>
        /// 聚焦按钮逻辑
        /// </summary>
        private void xw_btn_FrameAll_clicked()
        {
            xw_graphView?.FrameAll();
        }
        /// <summary>
        /// 帮助按钮
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void xw_btn_Help_clicked()
        {
            Debug.Log("打开教程网站！");
        }
        #endregion

        #region 工具栏开关逻辑
        /// <summary>
        /// toggle_RemoteInspectorDisplay 开关改变状态时
        /// </summary>
        /// <param name="evt"></param>
        private void xw_toggle_inspectorDisplay_changed(ChangeEvent<bool> evt)
        {
            bool state = evt.newValue;

            // 设置 InspectorViewer 容器可见性
            util_XGraphEditorUtility.Element_Dispaly_Set(xw_InspectorView_Container, state);

            // 如果打开开关的话，就让 InspectorViewer 更新节点属性显示（前提是当前存在节点被选中）
            xw_InspectorView.Clear();
            if (state)
            {
                if (xw_currentSelectedVisualNode != null)
                    xw_InspectorView.InspectorViewer(xw_currentSelectedVisualNode);
                else
                    xw_InspectorView.InspectorViewer(CloneTree);
            }

            // 记录 InspectorViewer 开关状态到行为树根节点变量
            CloneTree.XGraph_InspectorViewDisplay = xw_toggle_InspectorViewDisplay.value;
        }
        /// <summary>
        /// toggle_BlackBoardDisplay 开关改变状态时
        /// </summary>
        /// <param name="evt"></param>
        private void xw_toggle_BlackBoardDisplay_changed(ChangeEvent<bool> evt)
        {
            bool state = evt.newValue;

            // 设置 BlackBoardView 容器可见性
            util_XGraphEditorUtility.Element_Dispaly_Set(xw_BlackBoardView_Container, state);

            if (state)
            {
                xw_BlackBoard_UpdateTitleInfo();
            }

            // 记录 BlackBoardView 开关状态到行为树根节点变量
            CloneTree.XGraph_BlackBoardViewDisplay = xw_toggle_BlackBoardViewDisplay.value;
        }
        /// <summary>
        /// toggle_DisplayNodeColor 开关改变状态时
        /// </summary>
        /// <param name="evt"></param>
        private void xw_toggle_DisplayNodeColor_changed(ChangeEvent<bool> evt)
        {
            bool state = evt.newValue;

            if (OnNodeColorToggleChanged != null)
                OnNodeColorToggleChanged(state);

            // 记录 InspectorViewer 开关状态到行为树根节点变量
            CloneTree.XGraph_DisplayNodeColor = xw_toggle_DisplayNodeColor.value;
        }
        /// <summary>
        /// toggle_DisplayNodeFlow 开关改变状态时
        /// </summary>
        /// <param name="evt"></param>
        private void xw_toggle_DisplayNodeFlow_changed(ChangeEvent<bool> evt)
        {
            bool state = evt.newValue;

            DisplayNodeFlow = state;

            if (OnNodeFlowToggleChanged != null)
                OnNodeFlowToggleChanged(state);

            // 记录 InspectorViewer 开关状态到行为树根节点变量
            CloneTree.XGraph_DisplayNodeFlow = xw_toggle_DisplayNodeFlow.value;
        }
        #endregion

        #region GraphInfo 信息设置
        /// <summary>
        /// 行为资源信息容器 - 路径内容拷贝
        /// </summary>
        /// <param name="evt"></param>
        private void xw_GraphInfo_PathContent_clicked(PointerDownEvent evt)
        {
            if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
            {
                Debug.Log("已拷贝行为资源路径到系统剪贴板！");
                GUIUtility.systemCopyBuffer = xw_GraphInfo_PathContent.text;
            }
        }
        /// <summary>
        /// 获取并计算正确的相差上一次保存时间
        /// </summary>
        /// <param name="lasttime"></param>
        public void xw_GraphInfo_LastSaveDateTime_Set(string lasttime)
        {
            xw_GraphInfo_LastSaveDateTime.text = $"{lasttime}";
            xw_GraphInfo_LastSaveLag.text = $"{util_XGraphEditorUtility.GetTimeSinceLastSavePrecise(lasttime)}";

            if (CloneTree.On_GraphviewLastSave_Changed != null)
                CloneTree.On_GraphviewLastSave_Changed(CloneTree.LastSaveDateTime);
        }
        /// <summary>
        /// 相差上一次保存时间的文本标签同步到主题色
        /// </summary>
        public void xw_GraphInfo_LastSaveLag_ColorSyncUpdate()
        {
            util_XGraphEditorUtility.Element_Color_Set(xw_GraphInfo_LastSaveLag, CloneTree.GraphviewGridBackgroundThemes.themecolor);
        }
        /// <summary>
        /// 显示行为资源的路径
        /// </summary>
        /// <param name="path"></param>
        public void xw_GraphInfo_PathContent_Set(string path)
        {
            xw_GraphInfo_PathContent.text = path;
        }
        /// <summary>
        /// 显示Graphview的鼠标位置
        /// </summary>
        /// <param name="pos"></param>
        public void xw_GraphInfo_GraphMousePos_Set(Vector2 pos)
        {
            xw_label_GraphMousePos_x.text = $"X : {pos.x.ToString("F2")}";
            xw_label_GraphMousePos_y.text = $"Y : {pos.y.ToString("F2")}";
        }
        /// <summary>
        /// 编辑器图标颜色同步到主题色
        /// </summary>
        public void xw_GraphInfo_GraphViewIcon_ColorSyncUpdate()
        {
            util_XGraphEditorUtility.Element_BackgroundColorTint_Set(xw_Graphview_Icon, CloneTree.GraphviewGridBackgroundThemes.themecolor);
        }
        #endregion

        #region Inspctor 和 BlackBoard 面板标题设置
        /// <summary>
        /// 设置Inspector面板的标题
        /// </summary>
        /// <param name="Title"></param>
        public void InspectorViewAction_SetTitle(string Title)
        {
            // 加载 Inspector 面板标题文字
            util_XGraphEditorUtility.Element_Label_ValueSet(xw_label_InspectorView_Container_Title, Title);
        }
        /// <summary>
        /// 设置BlackBoard面板的标题
        /// </summary>
        /// <param name="Title"></param>
        public void BlackBoardViewAction_SetTitle(string Title)
        {
            // 加载 Inspector 面板标题文字
            util_XGraphEditorUtility.Element_Label_ValueSet(xw_label_BlackBoardView_Container_Title, Title);
        }
        /// <summary>
        /// 设置节点编辑器页眉页脚的标签组件的显示内容（页眉页脚节点信息）
        /// </summary>
        /// <param name="header"></param>
        /// <param name="footer"></param>
        public void xw_SetNodeInfos(string header, string footer)
        {
            xw_SetNodeInfo_Header(header);
            xw_SetNodeInfo_Footer(footer);
        }
        /// <summary>
        /// 设置窗口右上方的文字组件 (页眉组件) 的显示内容
        /// </summary>
        /// <param name="value"></param>
        public void xw_SetNodeInfo_Header(string value)
        {
            util_XGraphEditorUtility.Element_Label_ValueSet(xw_label_graph_nodeinfo_header, value);
        }
        /// <summary>
        /// 设置窗口右下方的文字组件 (页脚组件) 的显示内容
        /// </summary>
        /// <param name="value"></param>
        public void xw_SetNodeInfo_Footer(string value)
        {
            util_XGraphEditorUtility.Element_Label_ValueSet(xw_label_graph_nodeinfo_footer, value);
        }
        /// <summary>
        /// 当前选择的节点名称的文本标签同步到主题色
        /// </summary>
        public void xw_label_graph_nodeinfo_header_ColorSyncUpdate()
        {
            util_XGraphEditorUtility.Element_Color_Set(xw_label_graph_nodeinfo_header, CloneTree.GraphviewGridBackgroundThemes.themecolor);
        }
        /// <summary>
        /// 当前选择的节点的路径的文本标签同步到主题色
        /// </summary>
        public void xw_label_graph_nodeinfo_footer_ColorSyncUpdate()
        {
            util_XGraphEditorUtility.Element_Color_Set(xw_label_graph_nodeinfo_footer, CloneTree.GraphviewGridBackgroundThemes.themecolor);
        }

        #endregion
        #endregion

        #region ActionTree全局操作
        /// <summary>
        /// 打开行为树资源
        /// </summary>
        public void ActionTree_Open()
        {
            // 准备预打开的资源类
            xAction_Asset tree = null;

            #region 获取打开资源路径并获取目标资源
            string path = EditorUtility.OpenFilePanel("Select Tree Asset", "Assets", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace(Application.dataPath, "Assets"); // 转为 Unity 相对路径
                tree = AssetDatabase.LoadAssetAtPath<xAction_Asset>(path);
            }
            else
            {
                return;
            }
            #endregion

            #region 清空所有节点内容
            // 清空GraphView的所有节点
            xw_graphView.ClearGraphViewContents(false);
            #endregion

            // 删除克隆体
            xw_DeleteCloneTreeAsset();

            // 打开新的Tree
            SourceTree = tree;
            CloneTree = tree.Clone();

            // 加载 Inspector 面板标题文字
            InspectorViewAction_SetTitle($"{SourceTree.name} 行为根节点属性");

            // 加载 BlackBoard 面板标题文字
            BlackBoardViewAction_SetTitle($"{SourceTree.name} 变量黑板");

            EditorApplication.delayCall += () =>
            {
                // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                xw_InspectorView.InspectorViewer(CloneTree);
            };

            // 当 GraphView 组件不为空时，根据资源结构加载节点信息！               
            if (xw_graphView != null)
            {
                xw_graphView.Restructure_Graph(CloneTree);
            }
        }
        /// <summary>
        /// 保存替换行为树源
        /// </summary>
        public void ActionTree_SaveAndReplace()
        {
            // 保存最后一次的窗口尺寸，窗口尺寸不受克隆操作影响，总是回保存到原始资源而非克隆资源
            if (CloneTree != null && NodeEditorIsReady)
            {
                CloneTree.LastGraphWindowSize = new Vector2Int((int)position.size.x, (int)position.size.y);
                CloneTree.LastGraphViewPosition = xw_graphView.GetCurrentViewPosition();
                CloneTree.LastGraphViewZoom = xw_graphView.GetCurrentZoomLevel();
            }

            // 将调整好的克隆Tree替换回原始Tree
            SourceTree.Replace(CloneTree);
            xw_GraphInfo_LastSaveDateTime_Set(CloneTree.LastSaveDateTime);
        }
        #endregion

        #region 窗口变化回调
        /// <summary>
        /// 当窗口尺寸变化时
        /// </summary>
        private void OnWindowResizingUpdate()
        {
            // 检测窗口尺寸变化
            if (position.size != lastWindowSize)
            {
                lastWindowSize = position.size;
                isWindowResizing = true;

                xw_label_GraphWindowSize.text = $"{lastWindowSize.x} x {lastWindowSize.y}";

                // 刷新 BlackBoard 显示
                xw_BlackBoard_UpdateTitleInfo();

                if (CloneTree.On_GraphviewSize_Changed != null)
                    CloneTree.On_GraphviewSize_Changed(position.size);
            }
            else if (isWindowResizing)
            {
                isWindowResizing = false;
            }
        }
        #endregion

        #region 元素控制     
        /// <summary>
        /// 拖动目标VisualElement方法
        /// </summary>
        /// <param name="sourceNode"></param>
        /// <param name="handle"></param>
        private void Element_Drag(VisualElement target, VisualElement handle, Vector2 offset)
        {
            // 鼠标按下
            handle.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button != 0) return;

                // 如果 ListView 没有初始化，或者当前点击在 ListView 内部，就放行
                if (xw_BlackBoardView.VariableList != null && xw_BlackBoardView.VariableList.worldBound.Contains(evt.position))
                    return; // 让事件继续冒泡给 ListView

                // ✅ 关键：用当前容器左上角相对于鼠标点击位置的偏移
                Vector2 mouseInTarget = target.WorldToLocal(evt.position);
                offset = mouseInTarget;
                target.CapturePointer(evt.pointerId);
                evt.StopPropagation();
            });
            // 鼠标拖动
            handle.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (target.HasPointerCapture(evt.pointerId))
                {
                    var parent = target.parent;
                    if (parent == null) return;

                    // ✅ 将鼠标屏幕坐标转换为父容器本地坐标
                    Vector2 mouseInParent = parent.WorldToLocal(evt.position);

                    // ✅ 计算目标左上角的新位置
                    Vector2 newPos = mouseInParent - offset;

                    // ✅ 限制边界（防止拖出父容器）
                    newPos.x = Mathf.Clamp(newPos.x, 0, parent.layout.width - target.layout.width);
                    newPos.y = Mathf.Clamp(newPos.y, 0, parent.layout.height - target.layout.height);

                    // ✅ 设置位置（绝对坐标，相对于父容器）
                    target.style.left = newPos.x;
                    target.style.top = newPos.y;
                }
            });
            // 鼠标松开
            handle.RegisterCallback<PointerUpEvent>(evt =>
            {
                if (target.HasPointerCapture(evt.pointerId))
                {
                    target.ReleasePointer(evt.pointerId);
                }
            });
        }
        /// <summary>
        ///  控制元素 - 记录位置坐标信息到Prefs
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        private VisualElementTransformData Element_Transform_Save(VisualElement element)
        {
            // 创建一个位置数据结构，记录所有可能的定位值
            VisualElementTransformData data = new VisualElementTransformData();

            data.left = element.style.left.value.value;
            data.top = element.style.top.value.value;
            data.right = element.style.right.value.value;
            data.bottom = element.style.bottom.value.value;

            data.anc_Right = !element.style.right.keyword.Equals(StyleKeyword.Auto);
            data.anc_Bottom = !element.style.bottom.keyword.Equals(StyleKeyword.Auto);
            data.anc_Left = !element.style.left.keyword.Equals(StyleKeyword.Auto);
            data.anc_Top = !element.style.top.keyword.Equals(StyleKeyword.Auto);

            if (element.style.display == new StyleEnum<DisplayStyle>(DisplayStyle.Flex))
                data.size = new Vector2(element.resolvedStyle.width, element.resolvedStyle.height);
            return data;
        }
        /// <summary>
        ///  控制元素 - 读取Prefs到位置坐标
        /// </summary>
        /// <param name="key"></param>
        /// <param name="element"></param>
        public void Element_Transform_Load(VisualElementTransformData data, VisualElement element)
        {
            // 重置所有定位属性
            element.style.left = StyleKeyword.Auto;
            element.style.right = StyleKeyword.Auto;
            element.style.top = StyleKeyword.Auto;
            element.style.bottom = StyleKeyword.Auto;

            // 定位方式 - 右上
            if (data.anc_Right && data.anc_Top)
            {
                element.style.right = data.right;
                element.style.top = data.top;
            }
            // 定位方式 - 右下
            if (data.anc_Right && data.anc_Bottom)
            {
                element.style.right = data.right;
                element.style.bottom = data.bottom;
            }
            // 定位方式 - 左上
            if (data.anc_Left && data.anc_Top)
            {
                element.style.left = data.left;
                element.style.top = data.top;
            }
            // 定位方式 - 左下
            if (data.anc_Left && data.anc_Bottom)
            {
                element.style.left = data.left;
                element.style.bottom = data.bottom;
            }

            element.style.width = data.size.x;
            element.style.height = data.size.y;
        }
        #endregion

        #region 撤销&重做逻辑
        /// <summary>
        /// 撤销&重做逻辑
        /// </summary>
        private void OnUndoRedoPerformed()
        {
            RestructureGraphViews();
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 重建GraphView相关的的所有设置和节点以及内容
        /// </summary>
        public void RestructureGraphViews()
        {
            if (CloneTree == null || xw_graphView == null) return;

            /* 撤销或重做操作时对节点视图进行刷新的逻辑*/

            // 检测并刷新所有视觉节点的位置
            foreach (var dataNode in CloneTree.Actions)
            {
                var visualNode = xw_graphView.GetNodeByGuid(dataNode.guid) as xNode_Base;
                if (visualNode != null)
                {
                    // 如果该节点位置有变化则刷新该节点位置
                    if (visualNode.GetPosition().position != dataNode.nodeGraphPosition)
                    {
                        visualNode.SetPosition(new Rect(dataNode.nodeGraphPosition, visualNode.GetPosition().size));
                    }
                }
            }

            if (CloneTree == null || xw_graphView == null) return;

            xw_graphView.UnregisterGroupEvent();

            // 清空GraphView的所有节点
            xw_graphView.Node_Clear();

            // 清空GraphView的所有连线
            xw_graphView.EdgesClear();

            // 清空GraphView的所有Group
            xw_graphView.Groups_Clear(false);

            // 清空 View 视图
            xw_InspectorView.ClearInspector();
            //xw_BlackBoardView.ClearVariables();

            xw_graphView.RegisterGroupEvent();

            // 根据当前数据重新生成节点
            xw_graphView.Restructure_Graph(CloneTree);

            // 刷新 Inspector 显示
            if (xw_currentSelectedVisualNode != null)
                xw_InspectorView.InspectorViewer(xw_currentSelectedVisualNode);
            else
                xw_InspectorView.InspectorViewer(CloneTree);

            // 刷新选项面板UI参数显示
            OptionsPanel_ParamsUpdate();

            // 刷新 BlackBoard 标题显示
            xw_BlackBoard_UpdateTitleInfo();
            // 刷新 BlackBoard 属性列表
            xw_BlackBoard_VariablesRestructure();
        }
        /// <summary>
        /// GraphView窗口关闭时的逻辑操作
        /// </summary>
        private void xw_DestroyGraphView()
        {
            // 保存最后一次的窗口尺寸，窗口尺寸不受克隆操作影响，总是回保存到原始资源而非克隆资源
            if (SourceTree != null && NodeEditorIsReady)
            {
                SourceTree.LastGraphWindowSize = new Vector2Int((int)position.size.x, (int)position.size.y);
                SourceTree.LastGraphViewPosition = xw_graphView.GetCurrentViewPosition();
                SourceTree.LastGraphViewZoom = xw_graphView.GetCurrentZoomLevel();
            }

            if (xw_toggle_BlackBoardViewDisplay.value)
            {
                // 保存 BlackBoardView 面板的变换信息
                SourceTree.Last_GraphView_BlackboardPanel_TransformData = Element_Transform_Save(xw_BlackBoardView_Container);
            }
            if (xw_toggle_InspectorViewDisplay.value)
            {
                // 保存 InspectorViewer 面板的变换信息
                SourceTree.Last_GraphView_InspectorPanel_TransformData = Element_Transform_Save(xw_InspectorView_Container);
            }

            // 记录 InspectorViewer 开关状态到行为树根节点变量
            SourceTree.XGraph_InspectorViewDisplay = xw_toggle_InspectorViewDisplay.value;
            // 记录 BlackBoardView 开关状态到行为树根节点变量
            SourceTree.XGraph_BlackBoardViewDisplay = xw_toggle_BlackBoardViewDisplay.value;
            // 记录 InspectorViewer 开关状态到行为树根节点变量
            SourceTree.XGraph_DisplayNodeColor = xw_toggle_DisplayNodeColor.value;
            // 记录 InspectorViewer 开关状态到行为树根节点变量 
            SourceTree.XGraph_DisplayNodeFlow = xw_toggle_DisplayNodeFlow.value;

            xw_DeleteCloneTreeAsset();
        }
        /// <summary>
        /// 移除临时使用的TreeAsset
        /// </summary>
        private void xw_DeleteCloneTreeAsset()
        {
            // 删除临时的克隆Tree资源
            if (CloneTree != null)
            {
                CloneTree = null;
            }
        }
        /// <summary>
        /// 设置设计流程标题显示
        /// </summary>
        private void xw_SetFlowNames()
        {
            // 小标题文字显示 
            if (xw_label_graphTitle != null)
                xw_label_graphTitle.text = SourceTree.name;
            // 小标题鼠标悬停显示资源路径
            if (xw_label_graphTitle != null)
                xw_label_graphTitle.tooltip = AssetDatabase.GetAssetPath(SourceTree);
            // 水印文字显示
            if (xw_label_graphMarkText != null)
                xw_label_graphMarkText.text = SourceTree.name;
        }
        /// <summary>
        /// 刷新 BlackBoard 标题信息显示
        /// </summary>
        public void xw_BlackBoard_UpdateTitleInfo()
        {
            xw_BlackBoardView.label_title.text = SourceTree.name;
            xw_BlackBoardView.label_sub.text = $"行为：{CloneTree.Actions.Count}  /  便签：{CloneTree.Sticks.Count}  /  贴图：{CloneTree.Decals.Count}  /  变量：{CloneTree.BlackboardVariable.Count}";
        }
        /// <summary>
        /// 读取 BlackBoardVariables 属性列表
        /// </summary>
        public void xw_BlackBoard_VariablesRestructure()
        {
            xw_BlackBoardView.Restructure(CloneTree.BlackboardVariable);
        }
        /// <summary>
        /// 设置工具栏前端图标
        /// </summary>
        /// <param name="iconName"></param>
        /// <returns></returns>
        public Texture2D xw_Toolbar_IconSet(Texture2D icon)
        {
            xw_graph_icon = icon;
            return xw_graph_icon;
        }
        /// <summary>
        /// XGraph窗口居中
        /// </summary>
        /// <param root_title="state"></param>
        /// <param root_title="window"></param>
        private static void xw_CenterEditorWindow(Vector2Int size, EditorWindow window)
        {
            // 获取当前屏幕的分辨率
            int screenWidth = Screen.currentResolution.width;
            int screenHeight = Screen.currentResolution.height;

            // 计算窗口位置（屏幕中心）
            Rect windowRect = new Rect((screenWidth - size.x) / 2.0f, (screenHeight - size.y) / 2.0f, size.x, size.y);

            // 更新窗口位置和大小
            window.position = windowRect;
        }
        #endregion
    }
}