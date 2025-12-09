namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class util_XGraphNodeChildManager : EditorWindow
    {
        /// <summary>
        /// 原始行为树复制体，放置修改源资源，保证安全修改
        /// </summary>
        public xAction_Asset CloneNodeAsset;

        [SerializeField] private VisualTreeAsset m_VisualTreeAsset;
        [SerializeField] private Label graphAssetTitle;
        [SerializeField] private Label warnText;
        [SerializeField] private ListView Childs;
        [SerializeField] private VisualTreeAsset childItem;
        [SerializeField] private Button btn_save;
        [SerializeField] private Button btn_fixed;
        public string TargetAssetGuid;

        [MenuItem("Assets/XGraph/C 子节点管理助手", false, 101)]
        public static void CreateActionGraphNode()
        {
            util_XGraphNodeChildManager wnd = GetWindow<util_XGraphNodeChildManager>(true);
            wnd.titleContent = new GUIContent("子节点管理助手");
            wnd.minSize = new Vector2(350, 500);
        }

        private void OnEnable()
        {
            Selection.selectionChanged += selectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= selectionChanged;

            // 删除临时的克隆Tree资源
            if (CloneNodeAsset != null)
            {
#if UNITY_EDITOR
                string clonePath = AssetDatabase.GetAssetPath(CloneNodeAsset);
                if (!string.IsNullOrEmpty(clonePath))
                {
                    AssetDatabase.DeleteAsset(clonePath);
                    AssetDatabase.Refresh();
                }
                else
                {
                    // 如果是内存对象（未保存），直接销毁
                    DestroyImmediate(CloneNodeAsset, true);
                }
                CloneNodeAsset = null;
#endif
            }
        }

        #region 选择回调
        private void selectionChanged()
        {
            SelectedObject();
        }
        private void SelectedObject()
        {
            // 获取当前选中的对象
            UnityEngine.Object[] selectedObjects = Selection.objects;

            if (selectedObjects != null && selectedObjects.Length > 0)
            {
                // 只处理第一个选中的对象
                UnityEngine.Object selectedObject = selectedObjects[0];

                // 检查选中的对象类型
                if (selectedObject != null)
                {
                    if (selectedObject is xAction_Asset asset)
                    {
                        IsSelectedAssetCheck(true);
                        LoadNodeGraphAsset(asset);
                    }
                }
            }
        }
        #endregion

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // 读取并克隆uxml布局到 root 布局
            var visual_window = util_XGraphEditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_XGraphNodeChildManager.uxml");
            visual_window.CloneTree(root);

            // 读取并克隆uxml布局到 scrollview 布局
            childItem = util_XGraphEditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_XGraphNodeChildManager_Item.uxml");

            // 读取uss样式到 root 布局
            util_XGraphEditorUtility.ElementStyle_Add(root, $"{util_Dashboard.GetPath_GUI_Uss()}uss_XGraphNodeChildManager.uss");

            graphAssetTitle = root.Q<VisualElement>("graphAssetTitle").Q<Label>("title");
            Childs = root.Q<ListView>("Childs");
            warnText = root.Q<Label>("warnText");

            btn_save = root.Q<Button>("btn_save");

            btn_save.clicked += Btn_save_clicked;

            IsSelectedAssetCheck(false);

            SelectedObject();
        }

        #region 检测
        public void IsSelectedAssetCheck(bool state)
        {
            util_XGraphEditorUtility.Element_Dispaly_Set(warnText, !state);
        }
        #endregion

        #region 按钮回调
        private void Btn_save_clicked()
        {
            if (CloneNodeAsset == null || string.IsNullOrEmpty(TargetAssetGuid))
                return;
            xAction_Asset asset = AssetDatabase.LoadAssetAtPath<xAction_Asset>(AssetDatabase.GUIDToAssetPath(TargetAssetGuid));
            asset.Replace(CloneNodeAsset);
            // 标记原始资源为脏，确保保存
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            util_XGraphEditorUtility.DialogMsg("XGraph", $"已成功将修改保存到 '{asset.name}'！", "明白");
        }
        #endregion

        #region 获取子节点项模板
        /// <summary>
        /// 获取列表项模版
        /// </summary>
        /// <returns></returns>
        private TemplateContainer GetTemplate()
        {
            return childItem.CloneTree();
        }
        /// <summary>
        /// 获取列表项的模版跟物体作为列表项的基础样式和元素组成结构
        /// </summary>
        /// <returns></returns>
        private VisualElement ChildElement()
        {
            return GetTemplate().Q<VisualElement>("item");
        }
        #endregion

        /// <summary>
        /// 读取节点图资源
        /// </summary>
        private void LoadNodeGraphAsset(xAction_Asset asset)
        {
            TargetAssetGuid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(asset)).ToString();
            //Debug.Log(TargetAssetGuid);
            CloneNodeAsset = asset.Clone();

            // 标题显示为选中的行为资源文件名称
            util_XGraphEditorUtility.Element_Label_ValueSet(graphAssetTitle, CloneNodeAsset.name);

            // 清空 ListView 现有项（可选）
            Childs.bindItem = null;
            Childs.makeItem = null;
            Childs.unbindItem = null;

            // 设置数据源
            Childs.itemsSource = CloneNodeAsset.Actions;

            Childs.fixedItemHeight = 40;

            // 创造 ListView 的模版样式
            Childs.makeItem = ChildElement;

            // 将模板添加到 ListView
            Childs.bindItem = BindData;

            // 添加解绑回调 - 关键修复
            Childs.unbindItem = UnbindData;

            // 重建 ListView
            Childs.Rebuild();
            Childs.RefreshItems();
        }

        /// <summary>
        /// 绑定列表项的数据
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        private void BindData(VisualElement element, int index)
        {
            xAction_Base action = CloneNodeAsset.Actions[index];

            // 读取uss样式到 root 布局
            util_XGraphEditorUtility.ElementStyle_Add(element, $"{util_Dashboard.GetPath_GUI_Uss()}uss_XGraphNodeChildManager_Item.uss");

            // 获取 icon
            VisualElement icon = element.Q<VisualElement>("icon");
            // 获取 mark
            VisualElement mark = element.Q<VisualElement>("mark");
            // 获取 title 标签
            Label lab_title = element.Q<Label>("name");
            // 获取 action_type 标签
            Label lab_act_type = element.Q<Label>("act_type");
            // 获取 type 标签
            Label lab_node_type = element.Q<Label>("node_type");
            // 获取 type 标签
            Button btn_del = element.Q<Button>("btn_del");

            mark.style.opacity = (action.BaseArgs.isStartNode ? 1 : 0);

            // 设置 icon 图像
            if (icon != null)
                icon.style.backgroundImage = action.BaseArgs.NodeIcon != null ? action.BaseArgs.NodeIcon : AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(action.BaseArgs.icon));

            // 设置 title 标签
            if (lab_title != null)
                lab_title.text = action.identifyName;

            // 设置 type 标签
            if (lab_node_type != null)
                lab_node_type.text = action.BaseArgs.actionNodeType;

            // 设置 type 标签
            if (lab_act_type != null)
            {
                if (action is xAction_Base)
                {
                    lab_act_type.text = "行为";
                    lab_act_type.style.color = util_XGraphEditorUtility.Color_From_HexString("#3BFE9B");
                }

                if (action is xAction_Variable)
                {
                    lab_act_type.text = "变量";
                    lab_act_type.style.color = util_XGraphEditorUtility.Color_From_HexString("#FFA96E");
                }

                if (action is xAction_Property)
                {
                    lab_act_type.text = "属性";
                    lab_act_type.style.color = util_XGraphEditorUtility.Color_From_HexString("#CDCDCD");
                }
            }
            // 注册删除节点按钮事件
            btn_del.clicked += () =>
            {
                if (util_XGraphEditorUtility.DialogMsg("确认删除", $"确定要删除节点 '{action.identifyName}' 吗？", "删除", "取消"))
                {
                    CloneNodeAsset.Remove(action);
                    CloneNodeAsset.Variables_Refresh();
                    AssetDatabase.SaveAssets();
                    RefreshListView();
                }
            };
        }

        /// <summary>
        /// 解绑列表项 - 清理事件监听器
        /// </summary>
        private void UnbindData(VisualElement element, int index)
        {
            // 获取删除按钮
            Button btn_del = element.Q<Button>("btn_del");

            if (btn_del != null)
            {
                // 清理按钮的点击事件
                btn_del.clickable = null;
                btn_del.userData = null;
            }

            // 清理其他可能的事件监听器
            element.ClearClassList();
        }

        /// <summary>
        /// 刷新 ListView 显示
        /// </summary>
        private void RefreshListView()
        {
            if (CloneNodeAsset == null) return;

            // 重新设置数据源
            Childs.itemsSource = CloneNodeAsset.Actions;

            // 刷新 ListView
            Childs.Rebuild();
            Childs.RefreshItems();
        }

    }
}