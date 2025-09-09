namespace SevenStrikeModules.XGraph
{
    using Codice.CM.Common.Tree;
    using System;
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
        /// 警告文本
        /// </summary>
        public Label warningtext;
        /// <summary>
        /// 按钮添加变量
        /// </summary>
        public Button btn_AddVariable;
        /// <summary>
        /// 列表模版
        /// </summary>
        public VisualTreeAsset ListViewTemplate;
        /// <summary>
        /// 黑板变量图标主题
        /// </summary>
        public VariableThemesGroup VariableThemes;
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        public xg_BlackBoardView()
        {
            // 读取菜单结构列表内容
            string theme = util_XGraphEditorUtility.AssetLoad<TextAsset>($"{util_Dashboard.GetPath_Config()}/BlackBoarVariablesThemes.json").text;
            VariableThemes = JsonConvert.DeserializeObject<VariableThemesGroup>(theme);

            // 指定样式
            util_XGraphEditorUtility.ElementStyle_Add(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_ListViewItem.uss");

            // 获取Item模版
            ListViewTemplate = util_XGraphEditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_ListViewItem.uxml");
        }

        /// <summary>
        /// 初始化ListView组件
        /// </summary>
        public void Initialize()
        {
            // 在此模块下寻找 ListView 组件
            VariableList = this.Q<ListView>("VariableList");
            warningtext = this.Q<Label>("warningtext");

            // 创造 ListView 的模版样式
            VariableList.makeItem = GetElement;

            // 绑定 ListView 数据
            VariableList.bindItem = BindData;

            // ListView 每一项点击切换的动作
            VariableList.selectionChanged += SelectionChanged;

            // ListView 每一项选中时的按键动作
            VariableList.RegisterCallback<KeyDownEvent>(KeyControl);

            // 注册添加变量按钮动作
            btn_AddVariable.RegisterCallback<ClickEvent>(AddVariablesMenu);

            // 注册拖拽相关事件
            VariableList.RegisterCallback<DragUpdatedEvent>(OnVariableDragUpdated);
            VariableList.RegisterCallback<DragPerformEvent>(OnVariableDragPerform);
            VariableList.RegisterCallback<DragExitedEvent>(OnVariableDragExited);
        }

        #region 拖拽排序 & 添加进Graphview作为节点的相关逻辑
        /// <summary>
        /// 拖拽离开元素时触发
        /// </summary>
        /// <param name="evt"></param>
        private void OnVariableDragExited(DragExitedEvent evt)
        {

        }

        /// <summary>
        /// 拖拽完成时触发（松开鼠标）
        /// </summary>
        /// <param name="evt"></param>
        private void OnVariableDragPerform(DragPerformEvent evt)
        {
            // 检查是否拖拽到了GraphView区域（而不是在ListView内部排序）
            var graphView = FindGraphViewUnderPosition(evt.mousePosition);
            if (graphView != null)
            {
                // 获取选中的 BlackboardVariables
                var selectedVariables = VariableList.selectedItems;

                // 创建变量节点
                VaiableNodeGenerate(evt, selectedVariables);

                // 完全停止事件传播，阻止ListView内部处理
                evt.StopImmediatePropagation();

                // 标记为已处理
                evt.StopPropagation();
            }
        }

        /// <summary>
        /// 拖拽过程中每帧触发
        /// </summary>
        /// <param name="evt"></param>
        private void OnVariableDragUpdated(DragUpdatedEvent evt)
        {
            // 检查是否在GraphView区域
            var graphView = FindGraphViewUnderPosition(evt.mousePosition);
            if (graphView != null)
            {
                // 设置拖拽的视觉反馈
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                // 完全停止事件传播
                evt.StopImmediatePropagation();

                // 标记为已处理
                evt.StopPropagation();
            }
        }

        /// <summary>
        /// 查找位置下的GraphView
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private xg_GraphView FindGraphViewUnderPosition(Vector2 position)
        {
            var element = panel.Pick(position);
            while (element != null)
            {
                if (element is xg_GraphView graphView)
                    return graphView;
                element = element.parent;
            }
            return null;
        }
        #endregion

        #region 创建节点
        /// <summary>
        /// 创建变量节点
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="selectedVariables"></param>
        private void VaiableNodeGenerate(DragPerformEvent evt, IEnumerable<object> selectedVariables)
        {
            // 将鼠标位置从屏幕坐标转换为 xw_graphView 的局部坐标用于创建节点时指定位置
            Vector2 localMousePosition = graphWindow.xw_graphView.contentViewContainer.WorldToLocal(evt.mousePosition);

            // 用于偏移大于1个的变量节点坐标
            int index = 0;

            // 遍历选中的黑板变量
            foreach (var variableItem in selectedVariables)
            {
                // 根据解析的黑板变量类型来创建变量节点到编辑器中
                BlackboardVariable variable = variableItem as BlackboardVariable;
                if (variable != null)
                {
                    index = CreateVariableNode(localMousePosition, index, variable);
                }
            }
        }

        /// <summary>
        /// 创建变量节点
        /// </summary>
        /// <param name="localMousePosition"></param>
        /// <param name="index"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        private int CreateVariableNode(Vector2 localMousePosition, int index, BlackboardVariable variable)
        {
            #region 计算偏移值
            Vector2 offset = Vector2.zero;

            /* 如果是只有一个被拖入Graphview则该节点会居中对齐鼠标位置
             * 如果是批量加入从第2个节点开始按照步数和缩放的系数进行堆叠排列
             */
            if (index > 0)
            {
                float step_x = 81;
                float step_y = 46.5f;
                offset = new Vector2(-(step_x * 0.4f) * index, (step_y * 0.9f) * index);
            }

            index++;
            #endregion

            #region 节点配色的颜色根据变量类型来定（通过 json 配置对应主题色）
            Color node_color = GetVariableThemeColor(variable);
            #endregion

            #region 创建变量节点
            //object node = graphWindow.xw_graphView.CreateNode(variable.name, "SevenStrikeModules.XGraph", "Base_ActionNode_", "Start", "start", null, "Base_GraphNode_Start", false, null, "自定义", node_color, false, "", localMousePosition + offset + new Vector2(-81, -46.5f), Vector2.one);

            //VNode_Base node_base = node as VNode_Base;
            //if (node_base != null)
            //{

            //}
            #endregion

            //graphWindow.xw_graphView.AddToSelection(node_base);

            return index;
        }

        /// <summary>
        /// 获取黑板变量的专属主题色
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        private Color GetVariableThemeColor(BlackboardVariable variable)
        {
            Color node_color = Color.white;
            VariableThemes.VariableThemes.ForEach(theme =>
            {
                if (theme.type == variable.type.ToString())
                {
                    node_color = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            });
            return node_color;
        }
        #endregion

        #region 模版获取指定 & 绑定数据
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
        /// <param x_textfield="container"></param>
        /// <param x_textfield="index"></param>
        private void BindData(VisualElement element, int index)
        {
            // 获取黑板变量数据
            var variable = graphWindow.CloneTree.BlackboardVariables[index];

            #region 获取变量项的UI元素
            // 变量名称容器
            VisualElement ele_pill_container = element.Q<VisualElement>("pill");
            // 变量名称
            Label var_label_name = ele_pill_container.Q<Label>("var_label");
            // 变量名称输入框
            TextField var_textfield_name = ele_pill_container.Q<TextField>("var_textfield");
            var_textfield_name.multiline = false;
            var_textfield_name.Q<VisualElement>("unity-text-input").AddToClassList("PureTextfieldinput");

            // 变量图标
            VisualElement icon = ele_pill_container.Q<VisualElement>("icon");

            // 变量解释容器
            VisualElement ele_des_container = element.Q<VisualElement>("description");
            // 变量解释内容
            Label var_label_des = ele_des_container.Q<Label>("des");
            // 变量解释内容输入框
            TextField var_textfield_des = ele_des_container.Q<TextField>("des_textfield");
            var_textfield_des.multiline = false;
            var_textfield_des.Q<VisualElement>("unity-text-input").AddToClassList("PureTextfieldinput");

            #endregion

            #region 注册 - 变量名称标签 - 双击事件
            // 双击变量名称标签，以切换为输入框模式
            ele_pill_container.RegisterCallback<PointerDownEvent>((evt) =>
            {
                if (evt.clickCount == 2 && evt.button == (int)MouseButton.LeftMouse)
                {
                    var_label_name.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

                    // 从当前物体的父物体再次查找并获取变量 - 输入框
                    VisualElement pill = (VisualElement)evt.target;
                    TextField x_textfield = pill.Q<TextField>("var_textfield");
                    x_textfield.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    x_textfield.value = variable.name;
                    EditorApplication.delayCall += () =>
                    {
                        x_textfield.Focus();
                    };
                }
            });
            #endregion

            #region 注册 - 变量名称输入框 - 焦点丢失事件
            // 单击任意处，以切换为标签模式
            var_textfield_name.RegisterCallback<BlurEvent>((evt) =>
            {
                // 从当前物体的父物体再次查找并获取变量 - 标签
                TextField x_textfield = (TextField)evt.target;
                Label x_name = x_textfield.parent.Q<Label>("var_label");

                if (var_textfield_name.value != string.Empty)
                {
                    string value = var_textfield_name.value;
                    x_name.text = value;
                    Undo.RecordObject(graphWindow.CloneTree, "Change BlackBoard Variable Name");
                    // 改变行为资源的黑板变量列表中的变量名称
                    variable.name = value;
                }

                x_name.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                var_textfield_name.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            });
            #endregion

            #region 注册 - 变量解释标签 - 双击事件
            // 双击变量名称标签，以切换为输入框模式
            ele_des_container.RegisterCallback<PointerDownEvent>((evt) =>
            {
                if (evt.clickCount == 2 && evt.button == (int)MouseButton.LeftMouse)
                {
                    var_label_des.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

                    // 从当前物体的父物体再次查找并获取变量 - 输入框
                    VisualElement description = (VisualElement)evt.target;
                    TextField x_textfield = description.Q<TextField>("des_textfield");
                    x_textfield.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    x_textfield.value = variable.des;
                    EditorApplication.delayCall += () =>
                    {
                        x_textfield.Focus();
                    };
                }
            });
            #endregion

            #region 注册 - 变量解释输入框 - 焦点丢失事件
            // 单击任意处，以切换为标签模式
            var_textfield_des.RegisterCallback<BlurEvent>((evt) =>
            {
                // 从当前物体的父物体再次查找并获取变量 - 标签
                TextField x_textfield = (TextField)evt.target;
                Label x_des = x_textfield.parent.Q<Label>("des");

                if (var_textfield_des.value != string.Empty)
                {
                    string value = var_textfield_des.value;
                    x_des.text = value;
                    Undo.RecordObject(graphWindow.CloneTree, "Change BlackBoard Variable Description");
                    // 改变行为资源的黑板变量列表中的变量名称
                    variable.des = value;
                }

                x_des.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                var_textfield_des.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            });
            #endregion

            // 将行为资源的黑板变量列表中的 "变量名称" 赋予给标签
            var_label_name.text = variable.name;
            // 将行为资源的黑板变量列表中的 "变量解释" 赋予给标签
            var_label_des.text = variable.des;

            // 变量图标的颜色根据变量类型来定（通过 json 配置对应主题色）
            VariableThemes.VariableThemes.ForEach(theme =>
            {
                if (theme.type == variable.type.ToString())
                {
                    icon.style.backgroundColor = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            });

            element.RegisterCallback<PointerEnterEvent>((evt) =>
            {
                //Debug.Log("在节点视图中高亮显示 Variable 节点");
            });
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 重建黑板的所有变量
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
        /// 添加变量的按钮的事件
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
                menu.AddItem(new GUIContent("S 字符串变量"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.String));
                });
                menu.AddItem(new GUIContent("F 浮点数变量"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Float));
                });
                menu.AddItem(new GUIContent("I 整数变量"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Int));
                });
                menu.AddItem(new GUIContent("B 布尔变量"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Bool));
                });
                menu.AddItem(new GUIContent("V 2维向量变量"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Vector2));
                });
                menu.AddItem(new GUIContent("V 3维向量变量"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Vector3));
                });
                menu.AddItem(new GUIContent("V 4维向量变量"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Vector4));
                });
                menu.AddItem(new GUIContent("C 颜色变量"), false, () =>
                {
                    AddVariable(Variable_Create(BlackboardVariableType.Color));
                });
                menu.AddItem(new GUIContent("O 物体变量"), false, () =>
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
        /// 添加黑板变量
        /// </summary>
        /// <param name="vars"></param>
        public void AddVariable(BlackboardVariable vars)
        {
            // 确保ListView的Item不为空
            if (VariableList.itemsSource == null)
                VariableList.itemsSource = new List<BlackboardVariable>();

            Undo.RecordObject(graphWindow.CloneTree, "Added BlackBoardVariable");

            if (VariableList.itemsSource.Count <= 0)
            {
                util_XGraphEditorUtility.Element_Dispaly_Set(VariableList, true);
                util_XGraphEditorUtility.Element_Dispaly_Set(warningtext, false);
            }

            // 添加变量数据源并刷新
            VariableList.itemsSource.Add(vars);

            VariableList.Rebuild();
            VariableList.RefreshItems();
            VariableList.selectedIndex = -1;
            // 每次添加完变量后将焦点给到GraphView窗口控件，便于能正确识别Ctrl+S保存节点图
            graphWindow.xw_graphView.Focus();
        }

        /// <summary>
        /// 移除当前选择的黑板变量
        /// </summary>
        public void Remove_CurrentSelectedVariable()
        {
            Undo.RecordObject(graphWindow.CloneTree, "Removed BlackBoardVariable");
            // 设置数据源并刷新
            VariableList.itemsSource.RemoveAt(VariableList.selectedIndex);

            Debug.Log(warningtext.text);
            VariableList.Rebuild();
            VariableList.RefreshItems();


        }

        /// <summary>
        /// 移除当前所选的黑板变量
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

            if (VariableList.itemsSource.Count <= 0)
            {
                util_XGraphEditorUtility.Element_Dispaly_Set(VariableList, false);
                util_XGraphEditorUtility.Element_Dispaly_Set(warningtext, true);

                // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                graphWindow.xw_InspectorView.UpdateSelection(graphWindow.CloneTree);
            }

        }

        /// <summary>
        /// 创建黑板的变量
        /// </summary>
        /// <param name="type"></param>
        public BlackboardVariable Variable_Create(BlackboardVariableType type)
        {
            BlackboardVariable vare = new BlackboardVariable();
            vare.type = type;
            vare.name = type.ToString();
            vare.des = $"变量 {type}";
#if UNITY_EDITOR
            vare.variableGUID = UnityEditor.GUID.Generate().ToString();
            Undo.RecordObject(graphWindow.CloneTree, "Create BlackboardVariable");
#endif
            //Debug.Log($"创建了： {vare.type.ToString()} 到黑板中！");

            return vare;
        }

        /// <summary>
        /// 选择列表项时
        /// </summary>
        /// <param name="enumerable"></param>
        private void SelectionChanged(IEnumerable<object> enumerable)
        {
            // 当选择了变量时让 InspectorView显示变量值
            if (enumerable.Count() > 0)
            {
                foreach (var obj in enumerable)
                {
                    BlackboardVariable vare = obj as BlackboardVariable;
                    util_XGraphEditorUtility.Element_Label_ValueSet(graphWindow.xw_label_InspectorView_Container_Title, $"黑板变量 - {vare.name}");
                    graphWindow.xw_InspectorView.UpdateSelection(vare);
                }
            }
            // 当取消选择变量时让 InspectorView显示当前行为树根节点变量
            else
            {
                graphWindow.InspectorViewAction_SetTitle($"{graphWindow.SourceTree.name} 行为根节点变量");
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
        #endregion
    }
}