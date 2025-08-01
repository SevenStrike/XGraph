namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Callbacks;
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
        #endregion

        #region 控件
        /// <summary>
        ///  xw_graphView 控件 - 移动式属性视图容器组件的标题
        /// </summary>
        internal Label xw_label_InspectorView_Container_Title;
        /// <summary>
        /// 用于显示和隐藏移动式属性视图容器组件
        /// </summary>
        internal Toggle xw_toggle_InspectorViewDisplay;
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
        xGraphNode_Base xw_currentSelectedVisualNode;
        /// <summary>
        /// 此参数用于当取消选中视觉节点的时候的单次执行的判断开关，
        /// </summary>
        bool xw_isUnSelectedNode;
        /// <summary>
        /// GraphView窗口图标
        /// </summary>
        private Texture2D xw_graph_icon = null;
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
        public ActionTree_Nodes_Asset CloneTree;
        /// <summary>
        /// 原始行为树
        /// </summary>
        public ActionTree_Nodes_Asset SourceTree;
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
            if (EditorUtility.InstanceIDToObject(id) is ActionTree_Nodes_Asset datatree)
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
                bool remote_toggle = wnd.LoadInspectorView_Display("XGraph_InspectorViewDisplay");
                // 设置 InspectorView Remote 容器可见性
                wnd.xw_Set_InspectorViewRemoteVisibility(remote_toggle);
                // 设置移动式属性视图容器可见性按钮开关状态
                wnd.xw_toggle_InspectorViewDisplay.value = remote_toggle;
                EditorApplication.delayCall += () =>
                {
                    if (remote_toggle)
                    {
                        wnd.xw_InspectorView.UpdateSelection(wnd.CloneTree);
                    }
                };
                #endregion

                #region Node节点颜色标记的状态恢复
                // 获取最后一次的Node节点颜色标记状态
                bool nodeColorDisplayState = wnd.LoadNodeColorDisplay("XGraph_DisplayNodeColor");
                // 设置Node节点颜色标记按钮开关状态
                wnd.xw_toggle_DisplayNodeColor.value = nodeColorDisplayState;
                EditorApplication.delayCall += () =>
                {
                    if (wnd.OnNodeColorToggleChanged != null)
                        wnd.OnNodeColorToggleChanged(nodeColorDisplayState);
                };
                #endregion

                // 加载 InspectorView 面板标题文字
                wnd.Set_InspectorView_Container_Title($"{wnd.SourceTree.name} 行为根节点属性");

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
                    wnd.xw_graphView.Node_Make_With_ActionTreeData(wnd.CloneTree);
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

            #region 找到并获取 GraphView | InspectorView | SplitView 组件
            // 在布局中找到 xw_graphView 组件
            xw_graphView = root.Q<xg_GraphView>();
            xw_graphView.gv_GaphWindow = this;
            xw_graphView.RegisterNodeColorDisplayAction();

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

            #region InspectorView 初始化
            // 在布局中找到 InspectorView 容器组件
            xw_InspectorView_Container = root.Q<VisualElement>("InspectorView_Container");

            // 设置 InspectorView 容器组件最小尺寸
            xg_ResizableElement reselement = (xg_ResizableElement)xw_InspectorView_Container;
            reselement.SetMinSize(new Vector2(250, 320));

            // 加载 InspectorView 面板位置
            LoadInspectorView_Position("XGraph_InspectorViewPosition", reselement);

            // 加载 InspectorView 面板尺寸
            LoadInspectorView_Size("XGraph_InspectorViewSize", reselement);

            // 在布局中找到 InspectorView 组件
            xw_InspectorView = root.Q<xg_InspectorView>("InspectorView");
            xw_InspectorView.SendToBack();

            // 添加拖动支持
            RegisterDrag(reselement, reselement, "XGraph_InspectorViewPosition", "XGraph_InspectorViewSize");

            // 在布局中找到 InspectorView Remote 容器标题组件
            xw_label_InspectorView_Container_Title = root.Q<Label>("InspectorView_Container_Title");
            xw_label_InspectorView_Container_Title.SendToBack();
            #endregion

            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    reselement.SnapToNearestQuadrant();
                };
            };

            #endregion

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
                        var tree_source = AssetDatabase.LoadAssetAtPath<ActionTree_Nodes_Asset>(path_source);
                        var tree_clone = AssetDatabase.LoadAssetAtPath<ActionTree_Nodes_Asset>(path_clone);
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
                    var tree_source = AssetDatabase.LoadAssetAtPath<ActionTree_Nodes_Asset>(path_source);
                    var tree_clone = AssetDatabase.LoadAssetAtPath<ActionTree_Nodes_Asset>(path_clone);
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
        public void ReloadTreeFromPath(ActionTree_Nodes_Asset tree_source, ActionTree_Nodes_Asset tree_clone)
        {
            if (tree_source == null) return;

            // 清理旧数据 
            xw_graphView?.Nodes_Clear();
            xw_graphView?.Edges_Clear();
            xw_graphView?.Groups_Clear();

            #region 恢复上一次退出 GraphView 时记录的内视图位置以及缩放等级
            xw_graphView.SetViewPosition(tree_source.LastGraphViewPosition, tree_source.LastGraphViewZoom);
            #endregion

            #region 移动式属性面板的状态恢复
            // 获取最后一次的移动式属性面板开关状态
            bool remote_toggle = LoadInspectorView_Display("XGraph_InspectorViewDisplay");
            // 设置 InspectorView Remote 容器可见性
            xw_Set_InspectorViewRemoteVisibility(remote_toggle);
            // 设置移动式属性视图容器可见性按钮开关状态
            xw_toggle_InspectorViewDisplay.value = remote_toggle;
            if (remote_toggle)
                // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                xw_InspectorView.UpdateSelection(tree_clone);
            #endregion


            // 加载 RemoteInspector 面板标题文字
            Set_InspectorView_Container_Title($"{SourceTree.name} 行为根节点属性");

            xg_ResizableElement reselement = (xg_ResizableElement)xw_InspectorView_Container;

            // 加载 RemoteInspector 面板位置
            LoadInspectorView_Position("XGraph_InspectorViewPosition", reselement);

            // 加载 RemoteInspector 面板尺寸

            LoadInspectorView_Size("XGraph_InspectorViewSize", reselement);

            // 重新加载行为树资源
            SourceTree = tree_source;
            CloneTree = tree_clone;


            // 延迟重建可视化行为树结构
            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    reselement.SnapToNearestQuadrant();
                    xw_graphView?.Node_Make_With_ActionTreeData(CloneTree);

                    /*  以下逻辑必须保证先让 xw_graphView 的ActionTree不为空才行否则会报错，
                     *  而 xw_graphView?.Node_Make_With_ActionTreeData(CloneTree); 正是将 CloneTree 赋值到  xw_graphView 中的 ActionTree 的逻辑根源
                     */
                    #region Node节点颜色标记的状态恢复
                    // 获取最后一次的移动式属性面板开关状态
                    bool nodeColor_toggle = LoadNodeColorDisplay("XGraph_DisplayNodeColor");
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
        private void OnSelectNodeView(xGraphNode_Base nodeview)
        {
            if (nodeview == null) return;

            if (xw_InspectorView == null)
                return;
            xw_currentSelectedVisualNode = nodeview;

            // 清空 Inspector 视图
            xw_InspectorView.ClearInspector();

            // 当点击任意一个节点时调用 移动式 Inspector 面板显示对应的资源节点的属性
            xw_InspectorView.UpdateSelection(nodeview);

            // 加载 RemoteInspector 面板标题文字
            Set_InspectorView_Container_Title($"节点属性 - {nodeview.ActionTreeNode.nodeName}");

            xw_Set_CurrentGraphNodeIntro(nodeview.ActionTreeNode.GetInfo());
            xw_Set_CurrentGraphNodePathText(nodeview.ActionTreeNode.GetPath());
            xw_isUnSelectedNode = false;
        }
        /// <summary>
        /// 当选中视觉节点时执行
        /// </summary>
        /// <param h_name="nodeviews"></param>
        private void OnSelectionNodesView(List<xGraphNode_Base> nodeviews)
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

                // 加载 RemoteInspector 面板标题文字
                Set_InspectorView_Container_Title($"节点属性 - {nodeviews[0].ActionTreeNode.nodeName}");

                xw_Set_CurrentGraphNodeIntro(nodeviews[0].ActionTreeNode.GetInfo());
                xw_Set_CurrentGraphNodePathText(nodeviews[0].ActionTreeNode.GetPath());
                xw_isUnSelectedNode = false;
            }
            else if (nodeviews.Count > 1)
            {
                if (xw_InspectorView == null)
                    return;
                xw_currentSelectedVisualNode = null;

                // 清空 Inspector 视图
                xw_InspectorView.ClearInspector();

                // 加载 RemoteInspector 面板标题文字
                Set_InspectorView_Container_Title($"节点属性 - 多选状态");

                xw_Set_CurrentGraphNodeIntro("-");
                xw_Set_CurrentGraphNodePathText("-");
            }
        }
        /// <summary>
        /// 当从选中的所有视觉节点中移除某一个选择时执行
        /// </summary>
        /// <param h_name="nodeviews"></param>
        private void OnRemovedSelectionNodesView(List<xGraphNode_Base> nodeviews)
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
        private void OnUnSelectNodeView(xGraphNode_Base nodeview)
        {
            if (!xw_isUnSelectedNode)
            {
                xw_isUnSelectedNode = true;

                // 清空 Inspector 视图
                xw_InspectorView.ClearInspector();

                // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                xw_InspectorView.UpdateSelection(CloneTree);

                // 加载 RemoteInspector 面板标题文字
                Set_InspectorView_Container_Title($"{SourceTree.name} 行为根节点属性");

                xw_currentSelectedVisualNode = null;

                xw_Set_CurrentGraphNodeIntro(null);
                xw_Set_CurrentGraphNodePathText(null);
            }
        }
        #endregion

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
            xw_Set_InspectorViewRemoteVisibility(state);

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
            SaveInspectorView_Display("XGraph_InspectorViewDisplay");
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
            SaveNodeColorDisplay("XGraph_DisplayNodeColor");
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
        }
        /// <summary>
        /// 打开行为树资源
        /// </summary>
        public void ActionTree_Open()
        {
            // 准备预打开的资源类
            ActionTree_Nodes_Asset tree = null;

            #region 获取打开资源路径并获取目标资源
            string path = EditorUtility.OpenFilePanel("Select Tree Asset", "Assets", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace(Application.dataPath, "Assets"); // 转为 Unity 相对路径
                tree = AssetDatabase.LoadAssetAtPath<ActionTree_Nodes_Asset>(path);
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

            // 加载 RemoteInspector 面板标题文字
            Set_InspectorView_Container_Title($"{SourceTree.name} 行为根节点属性");

            EditorApplication.delayCall += () =>
            {
                // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                xw_InspectorView.UpdateSelection(CloneTree);
            };

            // 当 GraphView 组件不为空时，根据资源结构加载节点信息！               
            if (xw_graphView != null)
            {
                xw_graphView.Node_Make_With_ActionTreeData(CloneTree);
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

        #region InspectorView 面板行为逻辑
        private Vector2 dragOffset;

        /// <summary>
        /// 拖动目标VisualElement方法
        /// </summary>
        /// <param h_name="target"></param>
        /// <param h_name="handle"></param>
        private void RegisterDrag(VisualElement target, VisualElement handle, string saveKey_pos, string saveKey_size)
        {
            // 鼠标按下
            handle.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 0)
                {
                    // ✅ 关键：用当前容器左上角相对于鼠标点击位置的偏移
                    Vector2 mouseInTarget = target.WorldToLocal(evt.position);
                    dragOffset = mouseInTarget;
                    target.CapturePointer(evt.pointerId);
                    evt.StopPropagation();
                }
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
                    Vector2 newPos = mouseInParent - dragOffset;

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
        /// 记录元素位置和定位方式
        /// </summary>
        private void SaveInspectorView_Position(VisualElement element, string key)
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
        /// 读取元素位置并根据记录的定位方式恢复
        /// </summary>
        public void LoadInspectorView_Position(string key, VisualElement element)
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
                // 默认右上角
                element.style.left = StyleKeyword.Auto;
                element.style.right = 20;  // 右边距
                element.style.top = 40;    // 顶部距离
                element.style.bottom = StyleKeyword.Auto;
            }
        }

        /// <summary>
        /// 记录元素尺寸
        /// </summary>
        /// <param h_name="element"></param>
        /// <param h_name="key"></param>
        private void SaveInspectorView_Size(VisualElement element, string key)
        {
            var pos = new Vector2(
                element.resolvedStyle.width,
                element.resolvedStyle.height
            );
            EditorPrefs.SetString(key, JsonUtility.ToJson(pos));
        }
        /// <summary>
        /// 读取元素尺寸
        /// </summary>
        /// <param h_name="key"></param>
        /// <param h_name="element"></param>
        public void LoadInspectorView_Size(string key, VisualElement element)
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
                element.style.width = 320;
                element.style.height = 400;
            }
        }

        /// <summary>
        /// 读取InspectorView开关状态
        /// </summary>
        /// <param h_name="key"></param>
        /// <returns></returns>
        private bool LoadInspectorView_Display(string key)
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
        /// 记录InspectorView开关状态
        /// </summary>
        /// <param h_name="element"></param>
        /// <param h_name="key"></param>
        private void SaveInspectorView_Display(string key)
        {
            EditorPrefs.SetBool(key, xw_toggle_InspectorViewDisplay.value);
        }

        /// <summary>
        /// 读取NodeColorDisplay开关状态
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool LoadNodeColorDisplay(string key)
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
        /// 记录NodeColorDisplay开关状态
        /// </summary>
        /// <param name="key"></param>
        private void SaveNodeColorDisplay(string key)
        {
            EditorPrefs.SetBool(key, xw_toggle_DisplayNodeColor.value);
        }

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
                SaveInspectorView_Position(xw_InspectorView_Container, "XGraph_InspectorViewPosition");
            }
            else if (isWindowResizing)
            {
                isWindowResizing = false;
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
            foreach (var dataNode in CloneTree.ActionTreeNodes)
            {
                var visualNode = xw_graphView.GetNodeByGuid(dataNode.nodeGUID) as xGraphNode_Base;
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

            // 1. 重新读取数据并重建所有视觉节点，清空旧节点和连线
            // 清空GraphView的所有节点
            xw_graphView.Nodes_Clear();

            // 清空GraphView的所有连线
            xw_graphView.Edges_Clear();

            // 清空GraphView的所有Group
            xw_graphView.Groups_Clear(false);

            // 清空 Inspector 视图
            xw_InspectorView.ClearInspector();

            // 2. 根据当前数据重新生成节点
            xw_graphView.Node_Make_With_ActionTreeData(CloneTree);

            // 3. 刷新 Inspector 显示
            if (xw_currentSelectedVisualNode != null)
                xw_InspectorView.UpdateSelection(xw_currentSelectedVisualNode);
            else
                xw_InspectorView.UpdateSelection(CloneTree);

            //Debug.Log("执行撤销逻辑");
        }
        /// <summary>
        /// InspectorView Container 标题文字设置
        /// </summary>
        /// <param h_name="text"></param>
        public void Set_InspectorView_Container_Title(string text)
        {
            xw_label_InspectorView_Container_Title.text = text;
        }
        /// <summary>
        /// 设置当前选中节点的简要信息展示文字内容
        /// </summary>
        /// <param h_name="text"></param>
        private void xw_Set_CurrentGraphNodeIntro(string text)
        {
            xw_label_graph_CurrentNodeName.text = text;
        }
        /// <summary>
        /// 设置当前选中节点的路径文字内容
        /// </summary>
        /// <param h_name="text"></param>
        private void xw_Set_CurrentGraphNodePathText(string text)
        {
            xw_label_graph_CurrentNodePath.text = text;
        }
        /// <summary>
        /// 设置 InspectorView Remote 容器可见性
        /// </summary>
        /// <param h_name="state"></param>
        private void xw_Set_InspectorViewRemoteVisibility(bool state)
        {
            // 如果为 True 则将 InspectorView Remote 容器的可见性设为：Visiblity，即：可见，否则就是不可见：Hidden
            if (state)
            {
                xw_InspectorView_Container.style.visibility = Visibility.Visible;
            }
            else
            {
                xw_InspectorView_Container.style.visibility = Visibility.Hidden;
            }
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
            // 保存 InspectorRemote 面板的位置
            SaveInspectorView_Position(xw_InspectorView_Container, "XGraph_InspectorViewPosition");
            // 保存 InspectorRemote 面板的尺寸
            SaveInspectorView_Size(xw_InspectorView_Container, "XGraph_InspectorViewSize");

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