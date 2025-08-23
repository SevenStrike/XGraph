namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Unity.Plastic.Newtonsoft.Json;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;

    public class VariableTheme
    {
        public string type;
        public string color;
    }

    public class VariableThemesGroup
    {
        public List<VariableTheme> VariableThemes = new List<VariableTheme>();
    }

    /// <summary>
    /// XGraph的GraphView BlackBoard基础件，[UxmlElement]用于在UIBuilder中出现BlackBoardView的控件
    /// </summary>
    [UxmlElement]
    public partial class xg_BlackBoardView : VisualElement
    {
        #region 组件
        public xg_Window graphWindow;
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
        public Button btn_AddVariable;
        /// <summary>
        /// 列表模版
        /// </summary>
        public VisualTreeAsset ListViewTemplate;
        /// <summary>
        /// 黑板属性图标主题
        /// </summary>
        public VariableThemesGroup VariableThemes;
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        public xg_BlackBoardView()
        {
            // 读取菜单结构列表内容
            string theme = util_EditorUtility.AssetLoad<TextAsset>($"{util_Dashboard.GetPath_Config()}/BlackBoarVariablesThemes.json").text;
            VariableThemes = JsonConvert.DeserializeObject<VariableThemesGroup>(theme);

            // 指定样式
            var uss_BlackBoardView = util_EditorUtility.AssetLoad<StyleSheet>($"{util_Dashboard.GetPath_GUI_Uss()}uss_ListViewItem.uss");
            styleSheets.Add(uss_BlackBoardView);

            // 获取Item模版
            ListViewTemplate = util_EditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_ListViewItem.uxml");
        }

        /// <summary>
        /// 初始化ListView组件
        /// </summary>
        public void Initialize()
        {
            // 在此模块下寻找 ListView 组件
            BlackBoard_List = this.Q<ListView>("ParamsList");

            // 创造 ListView 的模版样式
            BlackBoard_List.makeItem = VariableListItem_Make;

            // 绑定 ListView 数据
            BlackBoard_List.bindItem = VariableListItem_Bind;

            // ListView 每一项点击的动作
            BlackBoard_List.selectionChanged += VariableListItem_SelectionChanged;

            // 注册添加属性按钮动作
            btn_AddVariable.RegisterCallback<ClickEvent>(VariableListView_AddMenuOpen);
        }

        /// <summary>
        /// 获取列表项模版
        /// </summary>
        /// <returns></returns>
        private TemplateContainer VariableListItem_TemplateGet()
        {
            return ListViewTemplate.CloneTree();
        }

        /// <summary>
        /// 指定列表项的组成结构
        /// </summary>
        /// <returns></returns>
        private VisualElement VariableListItem_Make()
        {
            return VariableListItem_TemplateGet().Q<VisualElement>("container");
        }

        /// <summary>
        /// 绑定列表项
        /// </summary>
        /// <param text_name="element"></param>
        /// <param text_name="index"></param>
        private void VariableListItem_Bind(VisualElement element, int index)
        {
            VisualElement ele_pill = element.Q<VisualElement>("pill");
            VisualElement icon = ele_pill.Q<VisualElement>("icon");
            Label text_name = ele_pill.Q<Label>("text");

            VisualElement ele_des = element.Q<VisualElement>("description");
            Label text_des = ele_des.Q<Label>("text");

            var obj = graphWindow.CloneTree.BlackboardVariables[index];
            text_name.text = obj.name;
            text_des.text = obj.type.ToString();

            // 属性前的图标的颜色根据属性类型来定（通过 json 配置对应主题色）
            VariableThemes.VariableThemes.ForEach(theme =>
            {
                if (theme.type == obj.type.ToString())
                {
                    icon.style.backgroundColor = util_EditorUtility.Color_From_HexString(theme.color);
                }
            });
        }

        /// <summary>
        /// 重建黑板的所有属性
        /// </summary>
        /// <param name="vars"></param>
        public void VariableListView_Rebuild(List<BlackboardVariable> vars)
        {
            // 设置数据源并刷新
            BlackBoard_List.itemsSource = vars;

            BlackBoard_List.Rebuild();
            BlackBoard_List.RefreshItems();
        }

        /// <summary>
        /// 添加属性的按钮的事件
        /// </summary>
        /// <param name="evt"></param>
        private void VariableListView_AddMenuOpen(ClickEvent evt)
        {
            // 获取按钮引用
            Button button = evt.target as Button;

            if (button != null)
            {
                // 获取按钮在屏幕上的位置
                Vector2 screenPosition = button.localBound.position;

                // 调整位置，让菜单出现在按钮下方
                screenPosition.y += button.worldBound.height;

                // 创建上下文菜单
                var menu = new GenericMenu();

                // 添加菜单项
                menu.AddItem(new GUIContent("添加整数参数"), false, () => Debug.Log("int"));
                menu.AddItem(new GUIContent("添加浮点数参数"), false, () => Debug.Log("float"));
                menu.AddItem(new GUIContent("添加字符串参数"), false, () => Debug.Log("string"));
                menu.AddItem(new GUIContent("添加布尔参数"), false, () => Debug.Log("bool"));
                // 显示菜单
                menu.DropDown(new Rect(screenPosition, Vector2.zero));
            }

            // 阻止事件继续传播
            evt.StopPropagation();
        }

        /// <summary>
        /// 选择列表项时
        /// </summary>
        /// <param name="enumerable"></param>
        private void VariableListItem_SelectionChanged(IEnumerable<object> enumerable)
        {
            foreach (var obj in enumerable)
            {
                EditorGUIUtility.PingObject(obj as UnityEngine.Object);
            }
        }

        /// <summary>
        /// 清空所有属性
        /// </summary>
        internal void ClearVariables()
        {

        }
    }
}