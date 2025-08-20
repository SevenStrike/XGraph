namespace SevenStrikeModules.XGraph
{
    using UnityEngine.UIElements;

    /// <summary>
    /// XGraph的GraphView BlackBoard基础件，[UxmlElement]用于在UIBuilder中出现BlackBoardView的控件
    /// </summary>
    [UxmlElement]
    public partial class xg_BlackBoardView : VisualElement
    {
        /// <summary>
        /// params列表
        /// </summary>
        public ListView ParamsList;
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
        /// 清空面板内容
        /// </summary>
        internal void ClearInspector()
        {
            ParamsList.Clear();
        }

        internal void UpdateGraphInfos(string name, string sub)
        {
            label_title.text = name;
            label_sub.text = sub;
        }
    }
}