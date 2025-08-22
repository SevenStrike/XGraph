namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;

    /// <summary>
    /// XGraph的GraphView BlackBoard基础件，[UxmlElement]用于在UIBuilder中出现BlackBoardView的控件
    /// </summary>
    [UxmlElement]
    public partial class xg_BlackBoardView : VisualElement
    {
        #region 组件
        /// <summary>
        /// params列表
        /// </summary>
        public ListView BlackBoard_List;
        /// <summary>
        /// 标题容器
        /// </summary>
        public VisualElement titlecontainer;
        /// <summary>
        /// 头部统计信息容器
        /// </summary>
        public VisualElement graphstatistic;
        /// <summary>
        /// 标题图标
        /// </summary>
        public Label icon_title;
        /// <summary>
        /// 标题文字
        /// </summary>
        public Label label_title;
        /// <summary>
        /// 标题副文本
        /// </summary>
        public Label label_sub;
        /// <summary>
        /// 按钮添加属性
        /// </summary>
        public Button btn_addparam;
        /// <summary>
        /// 列表模版
        /// </summary>
        public VisualTreeAsset ListViewTemplate;
        #endregion

        GameObject[] sceneobjs;

        /// <summary>
        /// 构造器
        /// </summary>
        public xg_BlackBoardView()
        {
            // 指定样式
            var uss_BlackBoardView = util_EditorUtility.AssetLoad<StyleSheet>($"{util_Dashboard.GetPath_GUI_Uss()}uss_ListViewItem.uss");
            styleSheets.Add(uss_BlackBoardView);

            // 获取Item模版
            ListViewTemplate = util_EditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_ListViewItem.uxml");
        }

        /// <summary>
        /// 初始化ListView组件
        /// </summary>
        public void InitializeListView()
        {
            // 在此模块下寻找 ListView 组件
            BlackBoard_List = this.Q<ListView>("ParamsList");
            // 创造 ListView 的模版样式
            BlackBoard_List.makeItem = ParamListItem_Make;
            // 绑定 ListView 数据
            BlackBoard_List.bindItem = ParamListItem_Bind;
            // ListView 每一项点击的动作
            BlackBoard_List.selectionChanged += act_selectionChanged;
        }
        /// <summary>
        /// 获取列表项模版
        /// </summary>
        /// <returns></returns>
        private TemplateContainer ParamListItem_TemplateGet()
        {
            return ListViewTemplate.CloneTree();
        }
        /// <summary>
        /// 指定列表项的组成结构
        /// </summary>
        /// <returns></returns>
        private VisualElement ParamListItem_Make()
        {
            return ParamListItem_TemplateGet().Q<VisualElement>("container");
        }
        /// <summary>
        /// 绑定列表项
        /// </summary>
        /// <param text_name="element"></param>
        /// <param text_name="index"></param>
        private void ParamListItem_Bind(VisualElement element, int index)
        {
            VisualElement ele_pill = element.Q<VisualElement>("pill");

            VisualElement icon = ele_pill.Q<VisualElement>("icon");
            Label text_name = ele_pill.Q<Label>("text");

            VisualElement ele_des = element.Q<VisualElement>("description");
            Label text_des = element.Q<Label>("text");

            var obj = sceneobjs[index];
            text_name.text = obj.name;
        }
        /// <summary>
        /// 选择列表项时
        /// </summary>
        /// <param name="enumerable"></param>
        private void act_selectionChanged(IEnumerable<object> enumerable)
        {
            foreach (var obj in enumerable)
            {
                EditorGUIUtility.PingObject(obj as UnityEngine.Object);
            }
        }
        /// <summary>
        /// 添加ListViewItem
        /// </summary>
        public void ParamListItem_Add()
        {
            Scene scene = SceneManager.GetActiveScene();
            sceneobjs = scene.GetRootGameObjects();

            // 设置数据源并刷新
            BlackBoard_List.itemsSource = sceneobjs;

            BlackBoard_List.Rebuild();
            BlackBoard_List.RefreshItems();
        }
        /// <summary>
        /// 清空面板内容
        /// </summary>
        internal void ClearInspector()
        {

        }
        /// <summary>
        /// 更新黑板标题信息
        /// </summary>
        /// <param text_name="name"></param>
        /// <param text_name="sub"></param>
        internal void UpdateGraphInfos(string name, string sub)
        {
            label_title.text = name;
            label_sub.text = sub;
        }
    }
}