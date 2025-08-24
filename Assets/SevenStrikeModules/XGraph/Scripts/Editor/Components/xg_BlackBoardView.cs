namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Plastic.Newtonsoft.Json;
    using UnityEditor;
    using UnityEngine;
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
        public ListView VariableList;
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
            VariableList = this.Q<ListView>("VariableList");

            // 创造 ListView 的模版样式
            VariableList.makeItem = GetElement;

            // 绑定 ListView 数据
            VariableList.bindItem = BindData;

            // ListView 每一项点击切换的动作
            VariableList.selectionChanged += SelectionChanged;

            // ListView 每一项选中时的按键动作
            VariableList.RegisterCallback<KeyDownEvent>(KeyControl);

            // 注册添加属性按钮动作
            btn_AddVariable.RegisterCallback<ClickEvent>(AddVariablesMenu);
        }

        /// <summary>
        /// 获取列表项模版
        /// </summary>
        /// <returns></returns>
        private TemplateContainer GetTemplate()
        {
            return ListViewTemplate.CloneTree();
        }

        /// <summary>
        /// 获取列表项的模版跟物体作为列表项的基础样式和元素组成结构
        /// </summary>
        /// <returns></returns>
        private VisualElement GetElement()
        {
            return GetTemplate().Q<VisualElement>("container");
        }

        /// <summary>
        /// 绑定列表项的数据
        /// </summary>
        /// <param text_name="element"></param>
        /// <param text_name="index"></param>
        private void BindData(VisualElement element, int index)
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
        public void Restructure(List<BlackboardVariable> vars)
        {
            // 设置数据源并刷新
            VariableList.itemsSource = vars;

            VariableList.Rebuild();
            VariableList.RefreshItems();
        }

        /// <summary>
        /// 添加属性的按钮的事件
        /// </summary>
        /// <param name="evt"></param>
        private void AddVariablesMenu(ClickEvent evt)
        {
            // 获取按钮引用
            Button button = evt.target as Button;

            if (button != null)
            {
                // 获取按钮在屏幕上的位置
                Vector2 screenPosition = button.worldBound.position;

                // 调整位置，让菜单出现在按钮下方
                screenPosition.y += button.worldBound.height;

                // 创建上下文菜单
                var menu = new GenericMenu();

                // 添加菜单项
                menu.AddItem(new GUIContent("添加 - 字符串参数"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.String));
                });
                menu.AddItem(new GUIContent("添加 - 浮点数参数"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Float));
                });
                menu.AddItem(new GUIContent("添加 - 整数参数"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Int));
                });
                menu.AddItem(new GUIContent("添加 - 布尔参数"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Bool));
                });
                menu.AddItem(new GUIContent("添加 - 2维向量参数"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Vector2));
                });
                menu.AddItem(new GUIContent("添加 - 3维向量参数"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Vector3));
                });
                menu.AddItem(new GUIContent("添加 - 4维向量参数"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Vector4));
                });
                menu.AddItem(new GUIContent("添加 - 物体参数"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Object));
                });
                // 显示菜单
                menu.DropDown(new Rect(screenPosition, Vector2.zero));
            }

            // 阻止事件继续传播
            evt.StopPropagation();
        }

        /// <summary>
        /// 添加黑板属性
        /// </summary>
        /// <param name="vars"></param>
        public void AddVariable(BlackboardVariable vars)
        {
            // 确保ListView的Item不为空
            if (VariableList.itemsSource == null)
                VariableList.itemsSource = new List<BlackboardVariable>();

            Undo.RecordObject(graphWindow.CloneTree, "Added BlackBoardVariable");

            // 添加属性数据源并刷新
            VariableList.itemsSource.Add(vars);

            VariableList.Rebuild();
            VariableList.RefreshItems();

            // 每次添加完属性后将焦点给到GraphView窗口控件，便于能正确识别Ctrl+S保存节点图
            graphWindow.xw_graphView.Focus();
        }

        /// <summary>
        /// 移除当前选择的黑板属性
        /// </summary>
        public void Remove_CurrentSelectedVariable()
        {
            Undo.RecordObject(graphWindow.CloneTree, "Removed BlackBoardVariable");
            // 设置数据源并刷新
            VariableList.itemsSource.RemoveAt(VariableList.selectedIndex);

            VariableList.Rebuild();
            VariableList.RefreshItems();
        }

        /// <summary>
        /// 移除当前所选的黑板属性
        /// </summary>
        public void Remove_CurrentSelectedVariables()
        {
            Undo.RecordObject(graphWindow.CloneTree, "Removed BlackBoardVariables");

            foreach (var item in VariableList.selectedItems)
            {
                // 设置数据源并刷新
                VariableList.itemsSource.Remove(item);
            }

            VariableList.Rebuild();
            VariableList.RefreshItems();
        }

        /// <summary>
        /// 创建黑板的属性
        /// </summary>
        /// <param name="type"></param>
        public BlackboardVariable Variable_Create(BlackboardVariableType type)
        {
            BlackboardVariable vare = new BlackboardVariable();
            vare.type = type;
            vare.name = type.ToString();
#if UNITY_EDITOR
            vare.guid = UnityEditor.GUID.Generate().ToString();
            Undo.RecordObject(graphWindow.CloneTree, "Create BlackboardVariable");
#endif
            Debug.Log($"创建了： {vare.type.ToString()} 到黑板中！");

            return vare;
        }

        /// <summary>
        /// 选择列表项时
        /// </summary>
        /// <param name="enumerable"></param>
        private void SelectionChanged(IEnumerable<object> enumerable)
        {
            // 当选择了属性时让 InspectorView显示属性值
            if (enumerable.Count() > 0)
            {
                foreach (var obj in enumerable)
                {
                    BlackboardVariable vare = obj as BlackboardVariable;
                    graphWindow.Element_Label_Set(graphWindow.xw_label_InspectorView_Container_Title, $"黑板属性 - {vare.name}");
                    graphWindow.xw_InspectorView.UpdateSelection(vare);
                }
            }
            // 当取消选择属性时让 InspectorView显示当前行为树根节点属性
            else
            {
                graphWindow.InspectorViewAction_SetTitle($"{graphWindow.SourceTree.name} 行为根节点属性");
                graphWindow.xw_InspectorView.UpdateSelection(graphWindow.CloneTree);
            }
        }

        /// <summary>
        /// 列表项的按键动作
        /// </summary>
        /// <param name="evt"></param>
        private void KeyControl(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Delete)
            {
                if (VariableList.selectedIndices.Count() == 1)
                    Remove_CurrentSelectedVariable();
                else
                    Remove_CurrentSelectedVariables();
                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.D && (evt.ctrlKey || evt.commandKey))
            {
                List<BlackboardVariable> vares = new List<BlackboardVariable>();
                foreach (var item in VariableList.selectedItems)
                {
                    BlackboardVariable vare = item as BlackboardVariable;
                    if (vare != null)
                        vares.Add(vare);
                    Debug.Log(vare.type);
                }

                vares.ForEach(v =>
                {
                    AddVariable(v);
                });

                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.S && (evt.ctrlKey || evt.commandKey))
            {
                graphWindow.ActionTree_SaveAndReplace();
                evt.StopPropagation();
            }

        }
    }
}