namespace SevenStrikeModules.XGraph
{
    using Codice.CM.Common.Tree;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEditor.Hardware;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.UIElements;

    [System.Serializable]
    /// <summary>
    /// InspectorView 面板的布局位置记录类
    /// </summary>
    public class PositionData
    {
        /// <summary>
        /// 左边距
        /// </summary>
        public float left;
        /// <summary>
        /// 上边距
        /// </summary>
        public float top;
        /// <summary>
        /// 右边距
        /// </summary>
        public float right;
        /// <summary>
        /// 下边距
        /// </summary>
        public float bottom;
        /// <summary>
        /// 右边距是否为Auto状态
        /// </summary>
        public bool anc_Right;
        /// <summary>
        /// 下边距是否为Auto状态
        /// </summary>
        public bool anc_Bottom;
        /// <summary>
        /// 上边距是否为Auto状态
        /// </summary>
        public bool anc_Top;
        /// <summary>
        /// 左边距是否为Auto状态
        /// </summary>
        public bool anc_Left;
    }

    public class xg_Window : EditorWindow
    {
        #region 组件
        /// <summary>
        /// xw_graphView 视图组件
        /// </summary>
        internal xg_GraphView xw_graphView;
        /// <summary>
        /// InspectorView 属性视图组件（移动式）
        /// </summary>
        internal xg_InspectorView xw_InspectorView;
        /// <summary>
        /// 移动式属性视图容器组件
        /// </summary>
        internal VisualElement xw_InspectorView_Container;
        /// <summary>
        /// InspectorView 属性视图组件（移动式）
        /// </summary>
        internal xg_BlackBoardView xw_BlackBoardView;
        /// <summary>
        /// 黑板视图容器组件
        /// </summary>
        internal VisualElement xw_BlackBoardView_Container;
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
        ///  xw_graphView 控件 - 标题
        /// </summary>
        private Label xw_label_graphTitle;
        /// <summary>
        ///  xw_graphView 控件 - 当前选择的节点的简介
        /// </summary>
        private Label xw_label_graph_CurrentNodeName;
        /// <summary>
        ///  xw_graphView 控件 - 当前选择的节点的路径
        /// </summary>
        private Label xw_label_graph_CurrentNodePath;
        /// <summary>
        ///  xw_graphView 控件 - 水印文字
        /// </summary>
        private Label xw_label_graphMarkText;
        /// <summary>
        ///  xw_graphView 控件 - 保存按钮
        /// </summary>
        private Button xw_btn_save;
        /// <summary>
        ///  xw_graphView 控件 - 打开按钮
        /// </summary>
        private Button xw_btn_open;
        /// <summary>
        ///  xw_graphView 控件 - 清空按钮
        /// </summary>
        private Button xw_btn_clear;
        /// <summary>
        ///  xw_graphView 控件 - 聚焦内容
        /// </summary>
        private Button xw_btn_FrameAll;
        #endregion

        #region 参数
        /// <summary>
        /// 当前选中的视觉节点
        /// </summary>
        VNode_Base xw_currentSelectedVisualNode;
        /// <summary>
        /// 此参数用于当取消选中视觉节点的时候的单次执行的判断开关，
        /// </summary>
        bool xw_isUnSelectedNode;
        /// <summary>
        /// GraphView窗口图标
        /// </summary>
        private Texture2D xw_graph_icon = null;
        private Vector2 dragOffset_InspectorView;
        private Vector2 dragOffset_BlackBoard;
        #endregion

        #region 委托
        /// <summary>
        /// 当节点颜色标记开关改变时的回调委托
        /// </summary>
        public Action<bool> OnNodeColorToggleChanged;
        #endregion

        #region 资源类
        /// <summary>
        /// 原始行为树复制体，放置修改源资源，保证安全修改
        /// </summary>
        public ActionNode_Asset CloneTree;
        /// <summary>
        /// 原始行为树
        /// </summary>
        public ActionNode_Asset SourceTree;
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
        /// 打开资源节点编辑器
        /// </summary>
        /// <param root_title="id"></param>
        /// <param root_title="line"></param>
        /// <returns></returns>
        [OnOpenAsset(1)]
        public static bool OnOpenAssets(int id, int line)
        {
            if (EditorUtility.InstanceIDToObject(id) is ActionNode_Asset datatree)
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

                #region 移动式属性面板的状态恢复
                // 获取最后一次的移动式属性面板开关状态
                bool inspector_view_toggle = wnd.Element_State_Load("XGraph_InspectorViewDisplay");
                // 设置 InspectorView Remote 容器可见性
                wnd.Element_Visibility_Set(wnd.xw_InspectorView_Container, inspector_view_toggle);
                // 设置移动式属性视图容器可见性按钮开关状态
                wnd.xw_toggle_InspectorViewDisplay.value = inspector_view_toggle;
                EditorApplication.delayCall += () =>
                {
                    if (inspector_view_toggle)
                    {
                        wnd.xw_InspectorView.UpdateSelection(wnd.CloneTree);
                    }
                };
                wnd.InspectorViewAction_SetTitle($"{wnd.SourceTree.name} 行为根节点属性");
                #endregion

                #region 黑板面板的状态恢复
                // 获取最后一次的黑板面板开关状态
                bool blackboard_view_toggle = wnd.Element_State_Load("XGraph_BlackBoardViewDisplay");
                // 设置 BlackBoardView Remote 容器可见性
                wnd.Element_Visibility_Set(wnd.xw_BlackBoardView_Container, blackboard_view_toggle);
                // 设置黑板视图容器可见性按钮开关状态
                wnd.xw_toggle_BlackBoardViewDisplay.value = blackboard_view_toggle;
                EditorApplication.delayCall += () =>
                {
                    // 刷新 BlackBoard 标题显示
                    wnd.xw_UpdateBlackBoardInfo();
                    // 刷新 BlackBoard 属性列表
                    wnd.xw_BlackBoardVariablesRestructure();
                };
                wnd.BlackBoardViewAction_SetTitle($"{wnd.SourceTree.name} 属性黑板");
                #endregion

                #region Node节点颜色标记的状态恢复
                // 获取最后一次的Node节点颜色标记状态
                bool nodeColorDisplayState = wnd.Element_State_Load("XGraph_DisplayNodeColor");
                // 设置Node节点颜色标记按钮开关状态
                wnd.xw_toggle_DisplayNodeColor.value = nodeColorDisplayState;
                EditorApplication.delayCall += () =>
                {
                    if (wnd.OnNodeColorToggleChanged != null)
                        wnd.OnNodeColorToggleChanged(nodeColorDisplayState);
                };
                #endregion

                // 加载 View 面板标题文字

                // 用于记录资源的原始路径，便于重新编译 & 运行状态切换 资源重载的保险操作
                EditorPrefs.SetString("XGraph->ActionTreePath_Source", AssetDatabase.GetAssetPath(wnd.SourceTree));
                EditorPrefs.SetString("XGraph->ActionTreePath_Clone", AssetDatabase.GetAssetPath(wnd.CloneTree));

                // 如果最后一次窗口尺寸值不为0则使用最后一次的窗口尺寸，否则就是用默认窗口尺寸，这里使用的 SourceTree 的原因是因为窗口尺寸这个变量不受克隆影响
                xw_CenterEditorWindow(wnd.SourceTree.LastGraphWindowSize == Vector2Int.zero ? new Vector2Int(1000, 700) : wnd.SourceTree.LastGraphWindowSize, wnd);
                #endregion

                #region 根据资源结构重建可视化行为树节点
                // 当 GraphView 组件不为空时
                if (wnd.xw_graphView != null)
                {
                    wnd.xw_graphView.Restructure_VisualNodes(wnd.CloneTree);
                    //Debug.Log("打开 XGraphView 并加载节点信息！");
                }
                #endregion

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
            var visual_window = util_EditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_Window.uxml");
            visual_window.CloneTree(root);

            // 读取uss样式到 root 布局
            var uss_window = util_EditorUtility.AssetLoad<StyleSheet>($"{util_Dashboard.GetPath_GUI_Uss()}uss_Window.uss");
            root.styleSheets.Add(uss_window);

            // 设置图标
            Texture2D icon = xw_Toolbar_IconSet(util_EditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/main.png"));
            VisualElement lab_graphIcon = util_EditorUtility.GetUIElement<VisualElement>(root, "graphIcon");
            lab_graphIcon.style.backgroundImage = new StyleBackground(icon);
            lab_graphIcon.style.unityBackgroundImageTintColor = util_Dashboard.Theme_Primary;
            #endregion

            #region 找到并获取 GraphView | InspectorView | BlackBoardView | SplitView 组件
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
            #endregion

            #region BlackBoardView ---------- 初始化
            // 在布局中找到 InspectorView 容器组件
            xw_BlackBoardView_Container = root.Q<VisualElement>("BlackBoardView_Container");

            // 设置 InspectorView 容器组件最小尺寸
            xg_ResizableElement ele_blackboard = (xg_ResizableElement)xw_BlackBoardView_Container;
            ele_blackboard.SetMinSize(new Vector2(250, 320));

            // 加载 BlackBoardView 面板位置
            Element_Position_Load("XGraph_BlackBoardViewPosition", ele_blackboard, "左上");
            // 加载 BlackBoardView 面板尺寸
            Element_Size_Load("XGraph_BlackBoardViewSize", ele_blackboard);

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
            Element_Drag(ele_blackboard, ele_blackboard, "XGraph_BlackBoardViewPosition", "XGraph_BlackBoardViewSize", dragOffset_BlackBoard);

            // 在布局中找到 BlackBoardView Remote 容器标题组件
            xw_label_BlackBoardView_Container_Title = xw_BlackBoardView_Container.Q<Label>("BlackBoardView_Container_Title");
            xw_label_BlackBoardView_Container_Title.SendToBack();
            #endregion

            #region InspectorView ---------- 初始化
            // 在布局中找到 InspectorView 容器组件
            xw_InspectorView_Container = root.Q<VisualElement>("InspectorView_Container");

            // 设置 InspectorView 容器组件最小尺寸
            xg_ResizableElement ele_inspector = (xg_ResizableElement)xw_InspectorView_Container;
            ele_inspector.SetMinSize(new Vector2(250, 320));

            // 加载 InspectorView 面板位置
            Element_Position_Load("XGraph_InspectorViewPosition", ele_inspector, "右上");

            // 加载 InspectorView 面板尺寸
            Element_Size_Load("XGraph_InspectorViewSize", ele_inspector);

            // 在布局中找到 InspectorView 组件
            xw_InspectorView = root.Q<xg_InspectorView>("InspectorView");
            xw_InspectorView.SendToBack();

            // 添加拖动支持
            Element_Drag(ele_inspector, ele_inspector, "XGraph_InspectorViewPosition", "XGraph_InspectorViewSize", dragOffset_InspectorView);

            // 在布局中找到 InspectorView Remote 容器标题组件
            xw_label_InspectorView_Container_Title = root.Q<Label>("InspectorView_Container_Title");
            xw_label_InspectorView_Container_Title.SendToBack();
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

            #region 找到并获取 GraphTitle | GraphMarkText | GraphNodeIntro 文字组件
            xw_label_graphTitle = root.Q<Label>("graphTitle");
            xw_label_graph_CurrentNodeName = root.Q<Label>("graph_CurrentNodeName");
            xw_label_graph_CurrentNodeName.style.color = util_Dashboard.Theme_Primary;
            xw_label_graph_CurrentNodePath = root.Q<Label>("graph_CurrentNodePath");
            xw_label_graphMarkText = root.Q<Label>("graphMarkText");
            #endregion

            #region 找到并获取 Save | Open | Clear | inspector_Remote 按钮组件
            xw_btn_save = root.Q<Button>("btn_Save");
            xw_btn_save.clicked += xw_btn_save_clicked;
            xw_btn_open = root.Q<Button>("btn_Load");
            xw_btn_open.clicked += xw_btn_open_clicked;
            xw_btn_clear = root.Q<Button>("btn_Clear");
            xw_btn_clear.clicked += xw_btn_clear_clicked;
            xw_btn_FrameAll = root.Q<Button>("btn_FrameAll");
            xw_btn_FrameAll.clicked += xw_btn_FrameAll_clicked;
            xw_toggle_InspectorViewDisplay = root.Q<Toggle>("toggle_InspectorViewDisplay");
            xw_toggle_InspectorViewDisplay.RegisterValueChangedCallback(xw_toggle_inspectorDisplay_changed);
            xw_toggle_BlackBoardViewDisplay = root.Q<Toggle>("toggle_BlackBoardViewDisplay");
            xw_toggle_BlackBoardViewDisplay.RegisterValueChangedCallback(xw_toggle_BlackBoardDisplay_changed);
            xw_toggle_DisplayNodeColor = root.Q<Toggle>("toggle_DisplayNodeColor");
            xw_toggle_DisplayNodeColor.RegisterValueChangedCallback(xw_toggle_DisplayNodeColor_changed);
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
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            // 初始化窗口尺寸记录
            lastWindowSize = position.size;
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

        #region 重新编译 & 运行状态切换 资源保险操作
        /// <summary>
        /// 使行为树编辑器在游戏运行状态切换时重新加载目标行为树资源，而不会导致引用丢失产生报错
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Reloader_In_PlayModeSwitched()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredPlayMode)
                {
                    EditorApplication.delayCall += () =>
                    {
                        //Debug.Log("退出编辑模式，进入播放模式");
                        string path_source = EditorPrefs.GetString("XGraph->ActionTreePath_Source", "");
                        string path_clone = EditorPrefs.GetString("XGraph->ActionTreePath_Clone", "");

                        // 根据路径恢复加载节点方案资源
                        var tree_source = AssetDatabase.LoadAssetAtPath<ActionNode_Asset>(path_source);
                        var tree_clone = AssetDatabase.LoadAssetAtPath<ActionNode_Asset>(path_clone);
                        if (tree_clone != null)
                        {
                            // 打开窗口并加载资源
                            var window = EditorWindow.GetWindow<xg_Window>();
                            window.ReloadTreeFromPath(tree_source, tree_clone);
                        }
                    };
                }
            };
        }

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
                    string path_source = EditorPrefs.GetString("XGraph->ActionTreePath_Source", "");
                    string path_clone = EditorPrefs.GetString("XGraph->ActionTreePath_Clone", "");

                    if (string.IsNullOrEmpty(path_source)) return;

                    // 根据路径恢复加载节点方案资源
                    var tree_source = AssetDatabase.LoadAssetAtPath<ActionNode_Asset>(path_source);
                    var tree_clone = AssetDatabase.LoadAssetAtPath<ActionNode_Asset>(path_clone);
                    if (tree_clone != null)
                    {
                        var window = GetWindow<xg_Window>();
                        window.ReloadTreeFromPath(tree_source, tree_clone);
                    }
                };
            };
        }

        /// <summary>
        /// 重新引用行为树编辑器的目标行为树资源
        /// </summary>
        /// <param h_name="tree_source"></param>
        /// <param h_name="tree_clone"></param>
        public void ReloadTreeFromPath(ActionNode_Asset tree_source, ActionNode_Asset tree_clone)
        {
            if (tree_source == null) return;

            // 清理旧数据 
            xw_graphView?.Node_Clear();
            xw_graphView?.EdgesClear();
            xw_graphView?.Groups_Clear();

            #region 恢复上一次退出 GraphView 时记录的内视图位置以及缩放等级
            xw_graphView.SetViewPosition(tree_source.LastGraphViewPosition, tree_source.LastGraphViewZoom);
            #endregion

            #region 移动式属性面板的状态恢复
            // 获取最后一次的移动式属性面板开关状态
            bool remote_toggle = Element_State_Load("XGraph_InspectorViewDisplay");
            // 设置 InspectorView Remote 容器可见性
            Element_Visibility_Set(xw_InspectorView_Container, remote_toggle);
            // 设置移动式属性视图容器可见性按钮开关状态
            xw_toggle_InspectorViewDisplay.value = remote_toggle;
            if (remote_toggle)
                // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                xw_InspectorView.UpdateSelection(tree_clone);

            xg_ResizableElement element_inspector = (xg_ResizableElement)xw_InspectorView_Container;
            // 加载 RemoteInspector 面板位置
            Element_Position_Load("XGraph_InspectorViewPosition", element_inspector, "右上");
            // 加载 RemoteInspector 面板尺寸
            Element_Size_Load("XGraph_InspectorViewSize", element_inspector);

            // 加载 BlackBoard 面板标题文字
            InspectorViewAction_SetTitle($"{SourceTree.name} 行为根节点属性");
            #endregion

            #region 黑板面板的状态恢复
            // 获取最后一次的移动式属性面板开关状态
            bool blackboard_toggle = Element_State_Load("XGraph_BlackBoardViewDisplay");
            // 设置 InspectorView Remote 容器可见性
            Element_Visibility_Set(xw_BlackBoardView_Container, blackboard_toggle);
            // 设置移动式属性视图容器可见性按钮开关状态
            xw_toggle_BlackBoardViewDisplay.value = blackboard_toggle;

            // 刷新 BlackBoard 显示
            xw_UpdateBlackBoardInfo();
            // 刷新 BlackBoard 属性列表
            xw_BlackBoardVariablesRestructure();

            xg_ResizableElement element_blackboard = (xg_ResizableElement)xw_BlackBoardView_Container;
            // 加载 BlackBoard 面板位置
            Element_Position_Load("XGraph_BlackBoardViewPosition", element_blackboard, "左上");
            // 加载 BlackBoard 面板尺寸
            Element_Size_Load("XGraph_BlackBoardViewSize", element_blackboard);

            // 加载 BlackBoard 面板标题文字
            BlackBoardViewAction_SetTitle($"{SourceTree.name} 属性黑板");
            #endregion

            // 重新加载行为树资源
            SourceTree = tree_source;
            CloneTree = tree_clone;

            // 延迟重建可视化行为树结构
            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    element_inspector.SnapToNearestQuadrant();
                    xw_graphView?.Restructure_VisualNodes(CloneTree);

                    /*  以下逻辑必须保证先让 xw_graphView 的ActionTree不为空才行否则会报错，
                     *  而 xw_graphView?.Restructure_VisualNodes(CloneTree); 正是将 CloneTree 赋值到  xw_graphView 中的 ActionTreeAsset 的逻辑根源
                     */
                    #region Node节点颜色标记的状态恢复
                    // 获取最后一次的移动式属性面板开关状态
                    bool nodeColor_toggle = Element_State_Load("XGraph_DisplayNodeColor");
                    // 设置移动式属性视图容器可见性按钮开关状态
                    xw_toggle_DisplayNodeColor.value = nodeColor_toggle;
                    #endregion
                };
            };
        }
        #endregion

        #region 视觉节点回调
        /// <summary>
        /// 当选中视觉节点时执行
        /// </summary>
        /// <param h_name="nodeview"></param>
        private void OnSelectNodeView(VNode_Base nodeview)
        {
            if (nodeview == null) return;

            if (xw_InspectorView == null)
                return;
            xw_currentSelectedVisualNode = nodeview;

            // 清空 Inspector 视图
            xw_InspectorView.ClearInspector();

            // 当点击任意一个节点时调用 移动式 Inspector 面板显示对应的资源节点的属性
            xw_InspectorView.UpdateSelection(nodeview);

            // 加载 Inspector 面板标题文字
            InspectorViewAction_SetTitle($"节点属性 - {nodeview.ActionNode.identifyName}");

            // 显示当前选中的节点的类型信息
            InspectorViewAction_SetNodeInfo(nodeview.ActionNode.GetInfo(), nodeview.ActionNode.GetPath());
            xw_isUnSelectedNode = false;
        }
        /// <summary>
        /// 当选中视觉节点时执行
        /// </summary>
        /// <param h_name="nodeviews"></param>
        private void OnSelectionNodesView(List<VNode_Base> nodeviews)
        {
            if (nodeviews == null) return;

            if (nodeviews.Count == 1)
            {
                if (xw_InspectorView == null)
                    return;
                xw_currentSelectedVisualNode = nodeviews[0];

                // 清空 Inspector 视图
                xw_InspectorView.ClearInspector();

                // 当点击任意一个节点时调用 移动式 Inspector 面板显示对应的资源节点的属性
                xw_InspectorView.UpdateSelection(nodeviews[0]);

                // 加载 Inspector 面板标题文字
                InspectorViewAction_SetTitle($"节点属性 - {nodeviews[0].ActionNode.identifyName}");

                // 显示当前选中的节点的类型信息
                InspectorViewAction_SetNodeInfo(nodeviews[0].ActionNode.GetInfo(), nodeviews[0].ActionNode.GetPath());
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
                InspectorViewAction_SetNodeInfo("-", "-");
            }
        }
        /// <summary>
        /// 当从选中的所有视觉节点中移除某一个选择时执行
        /// </summary>
        /// <param h_name="nodeviews"></param>
        private void OnRemovedSelectionNodesView(List<VNode_Base> nodeviews)
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
        /// <param h_name="nodeview"></param>
        private void OnUnSelectNodeView(VNode_Base nodeview)
        {
            if (!xw_isUnSelectedNode)
            {
                xw_isUnSelectedNode = true;

                // 清空 Inspector 视图
                xw_InspectorView.ClearInspector();

                // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                xw_InspectorView.UpdateSelection(CloneTree);

                // 加载 Inspector 面板标题文字
                InspectorViewAction_SetTitle($"{SourceTree.name} 行为根节点属性");

                xw_currentSelectedVisualNode = null;

                // 节点的类型信息 - 清空
                InspectorViewAction_SetNodeInfo(null, null);
            }
        }
        #endregion

        public void InspectorViewAction_SetTitle(string Title)
        {
            // 加载 Inspector 面板标题文字
            Element_Label_Set(xw_label_InspectorView_Container_Title, Title);
        }

        public void BlackBoardViewAction_SetTitle(string Title)
        {
            // 加载 Inspector 面板标题文字
            Element_Label_Set(xw_label_BlackBoardView_Container_Title, Title);
        }

        public void InspectorViewAction_SetNodeInfo(string name, string path)
        {
            // 节点的类型信息 - 清空
            Element_Label_Set(xw_label_graph_CurrentNodeName, name);
            // 节点的挂载资源路径 - 清空
            Element_Label_Set(xw_label_graph_CurrentNodePath, path);
        }

        #region 控件逻辑
        /// <summary>
        /// 清空按钮逻辑
        /// </summary>
        private void xw_btn_clear_clicked()
        {
            ActionTree_Clear();
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
        /// toggle_RemoteInspectorDisplay 开关改变状态时
        /// </summary>
        /// <param name="evt"></param>
        private void xw_toggle_inspectorDisplay_changed(ChangeEvent<bool> evt)
        {
            bool state = evt.newValue;

            // 设置 InspectorView 容器可见性
            Element_Visibility_Set(xw_InspectorView_Container, state);

            // 如果打开开关的话，就让 InspectorView 更新节点属性显示（前提是当前存在节点被选中）
            xw_InspectorView.Clear();
            if (state)
            {
                if (xw_currentSelectedVisualNode != null)
                    xw_InspectorView.UpdateSelection(xw_currentSelectedVisualNode);
                else
                    xw_InspectorView.UpdateSelection(CloneTree);
            }

            // 记录 InspectorView 开关状态到行为树根节点变量
            Element_State_Save("XGraph_InspectorViewDisplay", xw_toggle_InspectorViewDisplay.value);
        }
        /// <summary>
        /// toggle_BlackBoardDisplay 开关改变状态时
        /// </summary>
        /// <param name="evt"></param>
        private void xw_toggle_BlackBoardDisplay_changed(ChangeEvent<bool> evt)
        {
            bool state = evt.newValue;

            // 设置 BlackBoardView 容器可见性
            Element_Visibility_Set(xw_BlackBoardView_Container, state);

            if (state)
            {
                xw_UpdateBlackBoardInfo();
            }

            // 记录 BlackBoardView 开关状态到行为树根节点变量
            Element_State_Save("XGraph_BlackBoardViewDisplay", xw_toggle_BlackBoardViewDisplay.value);
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

            // 记录 InspectorView 开关状态到行为树根节点变量
            Element_State_Save("XGraph_DisplayNodeColor", xw_toggle_DisplayNodeColor.value);
        }
        #endregion

        #region ActionTree全局操作
        /// <summary>
        /// 清空行为树
        /// </summary>
        public void ActionTree_Clear()
        {
            // 清空GraphView的所有节点
            xw_graphView.ClearGraphViewContents();
            // 刷新 BlackBoard 显示
            xw_UpdateBlackBoardInfo();
        }
        /// <summary>
        /// 打开行为树资源
        /// </summary>
        public void ActionTree_Open()
        {
            // 准备预打开的资源类
            ActionNode_Asset tree = null;

            #region 获取打开资源路径并获取目标资源
            string path = EditorUtility.OpenFilePanel("Select Tree Asset", "Assets", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace(Application.dataPath, "Assets"); // 转为 Unity 相对路径
                tree = AssetDatabase.LoadAssetAtPath<ActionNode_Asset>(path);
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
            BlackBoardViewAction_SetTitle($"{SourceTree.name} 属性黑板");

            EditorApplication.delayCall += () =>
            {
                // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                xw_InspectorView.UpdateSelection(CloneTree);
            };

            // 当 GraphView 组件不为空时，根据资源结构加载节点信息！               
            if (xw_graphView != null)
            {
                xw_graphView.Restructure_VisualNodes(CloneTree);
            }
        }
        /// <summary>
        /// 保存替换行为树源
        /// </summary>
        public void ActionTree_SaveAndReplace()
        {
            // 将调整好的克隆Tree替换回原始Tree
            SourceTree.Replace(CloneTree);
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

                // 保存新位置
                Element_Position_Save(xw_InspectorView_Container, "XGraph_InspectorViewPosition");
                Element_Position_Save(xw_BlackBoardView_Container, "XGraph_BlackBoardViewPosition");

                // 刷新 BlackBoard 显示
                xw_UpdateBlackBoardInfo();
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
        /// <param h_name="sourceNode"></param>
        /// <param h_name="handle"></param>
        private void Element_Drag(VisualElement target, VisualElement handle, string saveKey_pos, string saveKey_size, Vector2 offset)
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
        /// 控制元素 - 可见性
        /// </summary>
        /// <param name="state"></param>
        private void Element_Visibility_Set(VisualElement element, bool state)
        {
            // 如果为 True 则将 InspectorView Remote 容器的可见性设为：Visiblity，即：可见，否则就是不可见：Hidden
            if (state)
            {
                element.style.visibility = Visibility.Visible;
            }
            else
            {
                element.style.visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// 控件元素 - 文字设置
        /// </summary>
        /// <param name="label"></param>
        /// <param name="text"></param>
        public void Element_Label_Set(Label label, string text)
        {
            label.text = text;
        }
        /// <summary>
        /// 控制元素 - 读取Prefs并控制开关状态
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool Element_State_Load(string key)
        {
            if (EditorPrefs.HasKey(key))
            {
                var state = EditorPrefs.GetBool(key);
                return state;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        ///  控制元素 - 记录开关状态到Prefs
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void Element_State_Save(string key, bool value)
        {
            EditorPrefs.SetBool(key, value);
        }
        /// <summary>
        ///  控制元素 - 记录位置坐标信息到Prefs
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        private void Element_Position_Save(VisualElement element, string key)
        {
            // 创建一个位置数据结构，记录所有可能的定位值
            var posData = new PositionData
            {
                left = element.style.left.value.value,
                top = element.style.top.value.value,
                right = element.style.right.value.value,
                bottom = element.style.bottom.value.value,

                anc_Right = !element.style.right.keyword.Equals(StyleKeyword.Auto),
                anc_Bottom = !element.style.bottom.keyword.Equals(StyleKeyword.Auto),
                anc_Left = !element.style.left.keyword.Equals(StyleKeyword.Auto),
                anc_Top = !element.style.top.keyword.Equals(StyleKeyword.Auto)
            };
            string json = JsonUtility.ToJson(posData);
            EditorPrefs.SetString(key, json);
        }
        /// <summary>
        ///  控制元素 - 读取Prefs到位置坐标
        /// </summary>
        /// <param name="key"></param>
        /// <param name="element"></param>
        public void Element_Position_Load(string key, VisualElement element, string DefaultAnchor)
        {
            if (EditorPrefs.HasKey(key))
            {
                var posData = JsonUtility.FromJson<PositionData>(EditorPrefs.GetString(key));

                // 重置所有定位属性
                element.style.left = StyleKeyword.Auto;
                element.style.right = StyleKeyword.Auto;
                element.style.top = StyleKeyword.Auto;
                element.style.bottom = StyleKeyword.Auto;

                // 定位方式 - 右上
                if (posData.anc_Right && posData.anc_Top)
                {
                    element.style.right = posData.right;
                    element.style.top = posData.top;
                }
                // 定位方式 - 右下
                if (posData.anc_Right && posData.anc_Bottom)
                {
                    element.style.right = posData.right;
                    element.style.bottom = posData.bottom;
                }
                // 定位方式 - 左上
                if (posData.anc_Left && posData.anc_Top)
                {
                    element.style.left = posData.left;
                    element.style.top = posData.top;
                }
                // 定位方式 - 左下
                if (posData.anc_Left && posData.anc_Bottom)
                {
                    element.style.left = posData.left;
                    element.style.bottom = posData.bottom;
                }
            }
            else
            {
                if (DefaultAnchor == "右上")
                {
                    // 默认右上角
                    element.style.left = StyleKeyword.Auto;
                    element.style.right = 15;
                    element.style.top = 10;
                    element.style.bottom = StyleKeyword.Auto;
                }
                if (DefaultAnchor == "左上")
                {
                    // 默认左上角
                    element.style.left = 15;
                    element.style.right = StyleKeyword.Auto;
                    element.style.top = 10;
                    element.style.bottom = StyleKeyword.Auto;
                }
                if (DefaultAnchor == "左下")
                {
                    // 默认左下角
                    element.style.left = 15;
                    element.style.right = StyleKeyword.Auto;
                    element.style.top = StyleKeyword.Auto;
                    element.style.bottom = 10;
                }
                if (DefaultAnchor == "右下")
                {
                    // 默认右下角
                    element.style.left = StyleKeyword.Auto;
                    element.style.right = 15;
                    element.style.top = StyleKeyword.Auto;
                    element.style.bottom = 10;
                }
            }
        }
        /// <summary>
        ///  控制元素 - 记录尺寸信息到Prefs
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        private void Element_Size_Save(VisualElement element, string key)
        {
            var pos = new Vector2(
                element.resolvedStyle.width,
                element.resolvedStyle.height
            );
            EditorPrefs.SetString(key, JsonUtility.ToJson(pos));
        }
        /// <summary>
        ///  控制元素 - 读取Prefs到尺寸
        /// </summary>
        /// <param name="key"></param>
        /// <param name="element"></param>
        public void Element_Size_Load(string key, VisualElement element)
        {
            if (EditorPrefs.HasKey(key))
            {
                var size = JsonUtility.FromJson<Vector2>(EditorPrefs.GetString(key));
                element.style.width = size.x;
                element.style.height = size.y;
            }
            else
            {
                // 默认右上角
                element.style.width = 250;
                element.style.height = 370;
            }
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 撤销&重做逻辑
        /// </summary>
        private void OnUndoRedoPerformed()
        {
            if (CloneTree == null || xw_graphView == null) return;

            /* 撤销或重做操作时对节点视图进行刷新的逻辑*/

            // 检测并刷新所有视觉节点的位置
            foreach (var dataNode in CloneTree.ActionNodes)
            {
                var visualNode = xw_graphView.GetNodeByGuid(dataNode.guid) as VNode_Base;
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

            // 清空GraphView的所有节点
            xw_graphView.Node_Clear();

            // 清空GraphView的所有连线
            xw_graphView.EdgesClear();

            // 清空GraphView的所有Group
            xw_graphView.Groups_Clear(false);

            // 清空 View 视图
            xw_InspectorView.ClearInspector();
            //xw_BlackBoardView.ClearVariables();

            // 根据当前数据重新生成节点
            xw_graphView.Restructure_VisualNodes(CloneTree);

            // 刷新 Inspector 显示
            if (xw_currentSelectedVisualNode != null)
                xw_InspectorView.UpdateSelection(xw_currentSelectedVisualNode);
            else
                xw_InspectorView.UpdateSelection(CloneTree);

            // 刷新 BlackBoard 标题显示
            xw_UpdateBlackBoardInfo();
            // 刷新 BlackBoard 属性列表
            xw_BlackBoardVariablesRestructure();
        }
        /// <summary>
        /// GraphView窗口关闭时的逻辑操作
        /// </summary>
        private void xw_DestroyGraphView()
        {
            // 保存最后一次的窗口尺寸，窗口尺寸不受克隆操作影响，总是回保存到原始资源而非克隆资源
            if (SourceTree != null)
            {
                SourceTree.LastGraphWindowSize = new Vector2Int((int)position.size.x, (int)position.size.y);
                SourceTree.LastGraphViewPosition = xw_graphView.GetCurrentViewPosition();
                SourceTree.LastGraphViewZoom = xw_graphView.GetCurrentZoomLevel();
            }

            // 保存 InspectorView 面板的位置
            Element_Position_Save(xw_InspectorView_Container, "XGraph_InspectorViewPosition");
            // 保存 InspectorView 面板的尺寸
            Element_Size_Save(xw_InspectorView_Container, "XGraph_InspectorViewSize");

            // 保存 BlackBoardView 面板的位置
            Element_Position_Save(xw_BlackBoardView_Container, "XGraph_BlackBoardViewPosition");
            // 保存 BlackBoardView 面板的尺寸
            Element_Size_Save(xw_BlackBoardView_Container, "XGraph_BlackBoardViewSize");

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
#if UNITY_EDITOR
                string clonePath = AssetDatabase.GetAssetPath(CloneTree);
                if (!string.IsNullOrEmpty(clonePath))
                {
                    AssetDatabase.DeleteAsset(clonePath);
                }
                else
                {
                    // 如果是内存对象（未保存），直接销毁
                    DestroyImmediate(CloneTree, true);
                }
                CloneTree = null;
#endif
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
        public void xw_UpdateBlackBoardInfo()
        {
            xw_BlackBoardView.label_title.text = SourceTree.name;
            xw_BlackBoardView.label_sub.text = $"节点：{CloneTree.ActionNodes.Count}  /  便签：{CloneTree.StickNoteDatas.Count}  /  编组：{CloneTree.NodeGroupDatas.Count}";
        }
        /// <summary>
        /// 读取 BlackBoardVariables 属性列表
        /// </summary>
        public void xw_BlackBoardVariablesRestructure()
        {
            xw_BlackBoardView.Restructure(CloneTree.BlackboardVariables);
        }
        /// <summary>
        /// 设置工具栏前端图标
        /// </summary>
        /// <param h_name="icon"></param>
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