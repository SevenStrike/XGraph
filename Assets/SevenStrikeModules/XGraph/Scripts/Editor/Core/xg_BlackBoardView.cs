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
        public VariableThemesGroup VariableThemeList;
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        public xg_BlackBoardView()
        {
            // 读取菜单结构列表内容
            string theme = util_XGraphEditorUtility.AssetLoad<TextAsset>($"{util_Dashboard.GetPath_Config()}/VariableThemes.json").text;
            VariableThemeList = JsonConvert.DeserializeObject<VariableThemesGroup>(theme);

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
                // 获取选中的 Variable
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
                Variable variable = variableItem as Variable;
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
        private int CreateVariableNode(Vector2 localMousePosition, int index, Variable variable)
        {
            #region 计算偏移值
            Vector2 offset = Vector2.zero;
            float step_x = 81;
            float step_y = 46.5f;

            /* 如果是只有一个被拖入Graphview则该节点会居中对齐鼠标位置
             * 如果是批量加入从第2个节点开始按照步数和缩放的系数进行堆叠排列
             */
            if (index > 0)
            {
                offset = new Vector2(-(step_x * 0.4f) * index, (step_y * 0.9f) * index);
            }

            index++;
            #endregion

            #region 节点配色的颜色根据变量类型来定（通过 json 配置对应主题色）
            Color node_color = GetVariableThemeColor(variable);
            #endregion

            #region 创建变量节点
            NodeCreateArgs_Variable args = new NodeCreateArgs_Variable();
            args.name = variable.name;
            args.description = variable.description;
            args.type = variable.type;
            args.position = localMousePosition + offset;
            args.varguid = variable.guid;
            args.transparentNode = false;
            args.variable = variable;

            // 添加可视化变量节点
            graphWindow.xw_graphView.CreateNode(args);
            #endregion

            return index;
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
            var variable = graphWindow.CloneTree.BlackboardVariable[index];

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

                    SyncVariableNodeDatas(variable);
                }

                x_name.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                var_textfield_name.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

                // 更新变量赋值数据
                graphWindow.xw_graphView.ActionTreeAsset.Variables_Refresh();
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
                    x_textfield.value = variable.description;
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
                    variable.description = value;
                }

                x_des.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                var_textfield_des.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

                SyncVariableNodeDatas(variable);

                // 更新变量赋值数据
                graphWindow.xw_graphView.ActionTreeAsset.Variables_Refresh();
            });
            #endregion

            // 将行为资源的黑板变量列表中的 "变量名称" 赋予给标签
            var_label_name.text = variable.name;
            // 将行为资源的黑板变量列表中的 "变量解释" 赋予给标签
            var_label_des.text = variable.description;

            // 变量图标的颜色根据变量类型来定（通过 json 配置对应主题色）
            foreach (var theme in VariableThemeList.VariableThemes)
            {
                if (theme.type == variable.type.ToString())
                {
                    icon.style.backgroundColor = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            }

            // 赋值Key便于高亮查找
            element.viewDataKey = variable.guid;

            // 注册鼠标进入变量标签节点时 - 高亮节点
            element.RegisterCallback<PointerEnterEvent>(HighlightVariableNodes);

            // 注册鼠标离开变量标签节点时 - 取消高亮节点
            element.RegisterCallback<PointerLeaveEvent>(UnHighlightVariableNodes);
        }

        /// <summary>
        /// 高亮显示匹配色所有目标节点
        /// </summary>
        /// <param name="evt"></param>
        private void HighlightVariableNodes(PointerEnterEvent evt)
        {
            VisualElement ele = (VisualElement)evt.target;
            string guid = ele.viewDataKey;

            #region 高亮显示节点
            List<VNode_Variable> vNode_Variables = FindVariableNodes(guid);
            foreach (VNode_Variable v in vNode_Variables)
            {
                v.Highlight();
            }
            #endregion
        }

        /// <summary>
        /// 取消高亮显示匹配色所有目标节点
        /// </summary>
        /// <param name="evt"></param>
        private void UnHighlightVariableNodes(PointerLeaveEvent evt)
        {
            VisualElement ele = (VisualElement)evt.target;
            string guid = ele.viewDataKey;

            #region 取消高亮显示节点
            List<VNode_Variable> vNode_Variables = FindVariableNodes(guid);
            foreach (var v in vNode_Variables)
            {
                v.UnHighlight();
            }
            #endregion
        }

        #endregion

        #region 变量添加与删除
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
                    AddVariable(Variable_Create(VariableType.String));
                });
                menu.AddItem(new GUIContent("F 浮点数变量"), false, () =>
                {
                    AddVariable(Variable_Create(VariableType.Float));
                });
                menu.AddItem(new GUIContent("I 整数变量"), false, () =>
                {
                    AddVariable(Variable_Create(VariableType.Int));
                });
                menu.AddItem(new GUIContent("B 布尔变量"), false, () =>
                {
                    AddVariable(Variable_Create(VariableType.Bool));
                });
                menu.AddItem(new GUIContent("V 2维向量变量"), false, () =>
                {
                    AddVariable(Variable_Create(VariableType.Vector2));
                });
                menu.AddItem(new GUIContent("V 3维向量变量"), false, () =>
                {
                    AddVariable(Variable_Create(VariableType.Vector3));
                });
                menu.AddItem(new GUIContent("V 4维向量变量"), false, () =>
                {
                    AddVariable(Variable_Create(VariableType.Vector4));
                });
                menu.AddItem(new GUIContent("C 颜色变量"), false, () =>
                {
                    AddVariable(Variable_Create(VariableType.Color));
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
        public void AddVariable(Variable vars)
        {
            // 确保ListView的Item不为空
            if (VariableList.itemsSource == null)
                VariableList.itemsSource = new List<Variable>();

            Undo.RecordObject(graphWindow.CloneTree, "Added BlackBoardVariable");

            // 添加变量数据源并刷新
            VariableList.itemsSource.Add(vars);

            VariableList.Rebuild();
            VariableList.RefreshItems();
            VariableList.selectedIndex = -1;

            ChceckVariableVisual();

            // 每次添加完变量后将焦点给到GraphView窗口控件，便于能正确识别Ctrl+S保存节点图
            graphWindow.xw_graphView.Focus();
        }

        /// <summary>
        /// 创建黑板的变量
        /// </summary>
        /// <param name="type"></param>
        public Variable Variable_Create(VariableType type)
        {
            Variable vare = null;
            switch (type)
            {
                case VariableType.String:
                    vare = new Variable_String(type.ToString());
                    break;
                case VariableType.Float:
                    vare = new Variable_Float(type.ToString());
                    break;
                case VariableType.Int:
                    vare = new Variable_Int(type.ToString());
                    break;
                case VariableType.Bool:
                    vare = new Variable_Bool(type.ToString());
                    break;
                case VariableType.Vector2:
                    vare = new Variable_Vector2(type.ToString());
                    break;
                case VariableType.Vector3:
                    vare = new Variable_Vector3(type.ToString());
                    break;
                case VariableType.Vector4:
                    vare = new Variable_Vector4(type.ToString());
                    break;
                case VariableType.Color:
                    vare = new Variable_Color(type.ToString());
                    break;
            }

#if UNITY_EDITOR
            vare.guid = UnityEditor.GUID.Generate().ToString();
#endif
            vare.description = type.ToString();
            //Debug.Log($"创建了： {vare.type.ToString()} 到黑板中！");
            return vare;
        }

        /// <summary>
        /// 移除当前所选的黑板变量
        /// </summary>
        public void Remove_CurrentSelectedVariables()
        {
            Undo.RecordObject(graphWindow.CloneTree, "Removed BlackBoardVariables");

            foreach (var item in VariableList.selectedItems)
            {
                // 获取到变量结构类
                Variable var = item as Variable;

                List<VNode_Variable> varNodes = FindVariableNodes(var.guid);
                foreach (var node in varNodes)
                {
                    graphWindow.xw_graphView.Node_Delete(node);
                }

                // 设置数据源并刷新
                VariableList.itemsSource.Remove(item);
            }

            VariableList.Rebuild();
            VariableList.RefreshItems();

            if (ChceckVariableVisual())
            {
                // 当取消选中任意视觉节点时让行为树根节点的Inspector属性显示
                graphWindow.xw_InspectorView.InspectorViewer(graphWindow.CloneTree);
            }
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 获取黑板变量的专属主题色
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public Color GetVariableThemeColor(Variable variable)
        {
            Color node_color = Color.white;
            foreach (var theme in VariableThemeList.VariableThemes)
            {
                if (theme.type == variable.type.ToString())
                {
                    node_color = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            }
            return node_color;
        }
        /// <summary>
        /// 获取黑板变量的专属主题色
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public Color GetVariableThemeColor(VariableType type)
        {
            Color node_color = Color.white;
            foreach (var theme in VariableThemeList.VariableThemes)
            {
                if (theme.type == type.ToString())
                {
                    node_color = util_XGraphEditorUtility.Color_From_HexString(theme.color);
                }
            }
            return node_color;
        }
        /// <summary>
        /// 同步目标 BlackboardVariable 的属性到 Variable 列表中匹配项并将匹配的目标 Variable 节点的显示更新
        /// </summary>
        /// <param name="variable"></param>
        private void SyncVariableNodeDatas(Variable variable)
        {
            // 同步修改 Variables 列表中匹配的项的变量名称
            foreach (var node in graphWindow.CloneTree.Variables)
            {
                if (node.varguid == variable.guid)
                {
                    node.name = variable.name;
                    node.description = variable.description;

                    // 注意：同时通过匹配到的 node 节点数据的guid找到节点并修改节点的显示名称！
                    VNode_Variable varNode = graphWindow.xw_graphView.FindNode(node.guid) as VNode_Variable;
                    if (varNode != null)
                    {
                        // 设置 Variable 节点的标题名称
                        varNode.SetNode_TitleLabel(variable.name);
                        // 设置 Variable 节点的解释文字
                        varNode.SetNode_Description(variable.description);
                    }
                }
            }
        }

        /// <summary>
        /// 列表是否为空检查
        /// </summary>
        /// <returns></returns>
        public bool ChceckVariableVisual()
        {
            if (VariableList.itemsSource == null || VariableList.itemsSource.Count <= 0)
            {
                util_XGraphEditorUtility.Element_Dispaly_Set(VariableList, false);
                util_XGraphEditorUtility.Element_Dispaly_Set(warningtext, true);
                return false;
            }
            else
            {
                util_XGraphEditorUtility.Element_Dispaly_Set(VariableList, true);
                util_XGraphEditorUtility.Element_Dispaly_Set(warningtext, false);
                return true;
            }
        }

        /// <summary>
        /// 重建黑板的所有变量
        /// </summary>
        /// <param name="vars"></param>
        public void Restructure(List<Variable> vars)
        {
            // 设置数据源并刷新
            VariableList.itemsSource = vars;

            VariableList.Rebuild();
            VariableList.RefreshItems();

            ChceckVariableVisual();
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
                    Variable vare = obj as Variable;
                    util_XGraphEditorUtility.Element_Label_ValueSet(graphWindow.xw_label_InspectorView_Container_Title, $"黑板变量");
                    graphWindow.xw_InspectorView.InspectorViewer(vare);
                    graphWindow.xw_isSelectedVariable = true;
                    graphWindow.xw_graphView.ClearSelection();
                }
            }
            // 当取消选择变量时让 InspectorView显示当前行为树根节点变量
            else
            {
                graphWindow.InspectorViewAction_SetTitle($"{graphWindow.SourceTree.name} 行为根节点变量");
                graphWindow.xw_InspectorView.InspectorViewer(graphWindow.CloneTree);
                graphWindow.xw_isSelectedVariable = false;
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
                List<Variable> vares = new List<Variable>();
                foreach (var item in VariableList.selectedItems)
                {
                    Variable vare = item as Variable;
                    if (vare != null)
                        vares.Add(vare);
                    Debug.Log(vare.type);
                }

                foreach (var v in vares)
                {
                    AddVariable(v.Clone(true));
                }

                evt.StopPropagation();
            }
            if (evt.keyCode == KeyCode.S && (evt.ctrlKey || evt.commandKey))
            {
                graphWindow.ActionTree_SaveAndReplace();
                evt.StopPropagation();
            }

        }

        /// <summary>
        /// 寻找节点中所有匹配目标类型的节点
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<VNode_Variable> FindVariableNodes(VariableType type)
        {
            List<VNode_Variable> list = new List<VNode_Variable>();

            foreach (var v in graphWindow.CloneTree.Variables)
            {
                if (v.type == type)
                {
                    list.Add(graphWindow.xw_graphView.FindNode(v.guid) as VNode_Variable);
                }
            }
            return list;
        }

        /// <summary>
        /// 寻找节点中所有匹配目标类型的节点
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public List<VNode_Variable> FindVariableNodes(string guid)
        {
            List<VNode_Variable> list = new List<VNode_Variable>();
            foreach (var v in graphWindow.CloneTree.Variables)
            {
                if (v.varguid == guid)
                {
                    list.Add(graphWindow.xw_graphView.FindNode(v.guid) as VNode_Variable);
                }
            }
            return list;
        }
        #endregion
    }
}