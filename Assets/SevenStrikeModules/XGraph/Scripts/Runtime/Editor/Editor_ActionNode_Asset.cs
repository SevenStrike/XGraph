namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(ActionNode_Asset), true)]
    public class Editor_ActionNode_Asset : Editor
    {
        private ActionNode_Asset baseScript;

        #region 序列化属性
        private SerializedProperty
            sp_LastGraphWindowSize,
            sp_LastGraphViewPosition,
            sp_LastGraphViewZoom,
            sp_LastSaveDateTime;
        #endregion

        void OnEnable()
        {
            if (target == null)
                return;

            GetTargetScript();
            GetProperties();
        }

        #region 绘制界面
        /// <summary>
        /// 绘制节点编辑器内的Inspector界面
        /// </summary>
        /// <returns></returns>
        public VisualElement CreateGraphviewInespector()
        {
            VisualElement rootElement = new VisualElement();

            #region 标题
            VisualElement titlegroup = util_XGraphInspectorGUI.GUI_Title(rootElement, util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/Icon.png"), baseScript.name.Substring(0, baseScript.name.Length - 10), new string[] { "titlegroup" }, new string[] { "titleicon" }, new string[] { "titlename" });
            #endregion

            #region 标题附加 - 变量类型标签
            string[] styles_sub = new string[] { "type" };
            Label lab_sub = new Label("行为资源");
            lab_sub.name = "sub";
            for (int i = 0; i < styles_sub.Length; i++)
            {
                lab_sub.AddToClassList(styles_sub[i]);
            }
            titlegroup.Add(lab_sub);
            #endregion

            #region 视口属性折叠器
            Foldout fo_graphview = util_XGraphInspectorGUI.GUI_Foldout(rootElement, "资源基础属性", "actionasset-base", new string[] { "foldout" });
            #endregion         

            #region 节点尺寸
            Vector2Field label_size = util_XGraphInspectorGUI.GUI_Field_Vector2(fo_graphview, "上次窗口尺寸", sp_LastGraphWindowSize.vector2IntValue, new string[] { "field_vector2" });
            baseScript.On_GraphviewSize_Changed += (size) =>
            {
                label_size.value = size;
            };
            #endregion

            #region Graphview视口位置
            Vector2Field label_pos = util_XGraphInspectorGUI.GUI_Field_Vector2(fo_graphview, "上次视口位置", sp_LastGraphViewPosition.vector2Value, new string[] { "field_vector2" });
            baseScript.On_GraphviewPos_Changed += (pos) =>
            {
                label_pos.value = pos;
            };
            #endregion

            #region Graphview视口缩放
            FloatField label_zoom = util_XGraphInspectorGUI.GUI_Field_Float(fo_graphview, "上次视口缩放", sp_LastGraphViewZoom.floatValue, new string[] { "field_float" });
            baseScript.On_GraphviewZoom_Changed += (zoom) =>
            {
                label_zoom.value = zoom;
            };
            #endregion

            #region Graph图标上一次保存时间
            TextField label_save = util_XGraphInspectorGUI.GUI_Field_String(fo_graphview, "上次保存时间", sp_LastSaveDateTime.stringValue, new string[] { "field_text" });
            baseScript.On_GraphviewLastSave_Changed += (date) =>
            {
                label_save.value = date;
            };
            #endregion

            #region 行为节点折叠器
            Foldout fo_actions = util_XGraphInspectorGUI.GUI_Foldout(rootElement, $"行为（{baseScript.Actions.Count}）", "actionasset-actions", new string[] { "foldout" });
            ListDisplay_Actions(fo_actions);
            #endregion

            #region 变量节点折叠器
            Foldout fo_bbvar = util_XGraphInspectorGUI.GUI_Foldout(rootElement, $"变量（{baseScript.Variables.Count}）", "actionasset-bbvar", new string[] { "foldout" });
            ListDisplay_Variables(fo_bbvar);
            #endregion

            #region 便签节点折叠器
            Foldout fo_sticks = util_XGraphInspectorGUI.GUI_Foldout(rootElement, $"便签（{baseScript.Sticks.Count}）", "actionasset-sticks", new string[] { "foldout" });
            ListDisplay_Sticks(fo_sticks);
            #endregion

            #region 标签节点折叠器
            Foldout fo_labels = util_XGraphInspectorGUI.GUI_Foldout(rootElement, $"标签（{baseScript.Labels.Count}）", "actionasset-labels", new string[] { "foldout" });
            ListDisplay_Labels(fo_labels);
            #endregion

            #region 贴图节点折叠器
            Foldout fo_decals = util_XGraphInspectorGUI.GUI_Foldout(rootElement, $"贴图（{baseScript.Decals.Count}）", "actionasset-decals", new string[] { "foldout" });
            ListDisplay_Decals(fo_decals);
            #endregion

            #region 编组节点折叠器
            Foldout fo_group = util_XGraphInspectorGUI.GUI_Foldout(rootElement, $"编组（{baseScript.Groups.Count}）", "actionasset-groups", new string[] { "foldout" });
            ListDisplay_Groups(fo_group);
            #endregion
            return rootElement;
        }
        /// <summary>
        /// 列表 - 行为节点
        /// </summary>
        /// <param name="fold"></param>
        private void ListDisplay_Actions(Foldout fold)
        {
            fold.Clear();
            for (int i = 0; i < baseScript.Actions.Count; i++)
            {
                ActionNode_Base action = baseScript.Actions[i];

                VisualElement container = new VisualElement();
                container.pickingMode = PickingMode.Ignore;
                container.AddToClassList("list_container_actionasset");
                fold.Add(container);

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg_actionasset");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.pickingMode = PickingMode.Ignore;
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = action.NodeIcon == null ? util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{action.icon}.png") : action.NodeIcon;
                container_title.Add(container_icon);
                container_icon.style.unityBackgroundImageTintColor = action.themeColor;

                Label label_title = util_XGraphInspectorGUI.GUI_Label(container_title, $"{action.identifyName}", new string[] { "labeltext", "list_item_title" });
                label_title.pickingMode = PickingMode.Ignore;

                VisualElement container_concurrent_icon = new VisualElement();
                container_concurrent_icon.pickingMode = PickingMode.Ignore;
                container_concurrent_icon.AddToClassList("llist_concurrent_icon_actionasset");
                string mode = action.isConcurrentExecution ? "concurrent" : "sepline";
                container_concurrent_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{mode}.png");
                container_title.Add(container_concurrent_icon);

                container_title.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    VNode_Base act_node = util_XGraphEditorUtility.GetGraphviewWindow().xw_graphView.FindNodeView(action.guid);
                    act_node.Highlight();
                });

                container_title.RegisterCallback<PointerOutEvent>((evt) =>
                {
                    VNode_Base act_node = util_XGraphEditorUtility.GetGraphviewWindow().xw_graphView.FindNodeView(action.guid);
                    act_node.UnHighlight();
                });
            }
        }
        /// <summary>
        /// 列表 - 变量节点
        /// </summary>
        /// <param name="fold"></param>
        private void ListDisplay_Variables(Foldout fold)
        {
            fold.Clear();
            for (int i = 0; i < baseScript.Variables.Count; i++)
            {
                ActionVariableData vare = baseScript.Variables[i];

                VisualElement container = new VisualElement();
                container.pickingMode = PickingMode.Ignore;
                container.AddToClassList("list_container_actionasset");
                fold.Add(container);

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg_actionasset");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.pickingMode = PickingMode.Ignore;
                container_icon.AddToClassList("llist_variable_icon_actionasset");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/sepline.png");
                container_title.Add(container_icon);
                container_icon.style.unityBackgroundImageTintColor = util_XGraphEditorUtility.GetGraphviewWindow().xw_BlackBoardView.GetVariableThemeColor(vare.type);

                Label label_title = util_XGraphInspectorGUI.GUI_Label(container_title, $"{vare.name}", new string[] { "labeltext", "list_item_title" });
                label_title.pickingMode = PickingMode.Ignore;

                container_title.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    List<VNode_Variable> var_nodes = util_XGraphEditorUtility.GetGraphviewWindow().xw_BlackBoardView.FindVariableNodes(vare.varguid);
                    foreach (var node in var_nodes)
                    {
                        node.Highlight();
                    }
                });

                container_title.RegisterCallback<PointerOutEvent>((evt) =>
                {
                    List<VNode_Variable> var_nodes = util_XGraphEditorUtility.GetGraphviewWindow().xw_BlackBoardView.FindVariableNodes(vare.varguid);
                    foreach (var node in var_nodes)
                    {
                        node.UnHighlight();
                    }
                });
            }
        }
        /// <summary>
        /// 列表 - 便签节点
        /// </summary>
        /// <param name="fold"></param>
        private void ListDisplay_Sticks(Foldout fold)
        {
            fold.Clear();
            for (int i = 0; i < baseScript.Sticks.Count; i++)
            {
                ActionStickData stick = baseScript.Sticks[i];

                VisualElement container = new VisualElement();
                container.pickingMode = PickingMode.Ignore;
                container.AddToClassList("list_container_actionasset");
                fold.Add(container);

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg_actionasset");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.pickingMode = PickingMode.Ignore;
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/stick.png");
                container_title.Add(container_icon);

                Label label_title = util_XGraphInspectorGUI.GUI_Label(container_title, $"{stick.name}", new string[] { "labeltext", "list_item_title" });
                label_title.pickingMode = PickingMode.Ignore;

                container_title.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    Node var_nodes = util_XGraphEditorUtility.GetGraphviewWindow().xw_graphView.FindNode(stick.guid);
                    if (var_nodes is VNode_Stick sk)
                        sk.Highlight();
                });

                container_title.RegisterCallback<PointerOutEvent>((evt) =>
                {
                    Node var_nodes = util_XGraphEditorUtility.GetGraphviewWindow().xw_graphView.FindNode(stick.guid);
                    if (var_nodes is VNode_Stick sk)
                        sk.UnHighlight();
                });
            }
        }
        /// <summary>
        /// 列表 - 标签节点
        /// </summary>
        /// <param name="fold"></param>
        private void ListDisplay_Labels(Foldout fold)
        {
            fold.Clear();
            for (int i = 0; i < baseScript.Labels.Count; i++)
            {
                ActionLabelData label = baseScript.Labels[i];

                VisualElement container = new VisualElement();
                container.pickingMode = PickingMode.Ignore;
                container.AddToClassList("list_container_actionasset");
                fold.Add(container);

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg_actionasset");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.pickingMode = PickingMode.Ignore;
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/stick.png");
                container_title.Add(container_icon);

                Label label_title = util_XGraphInspectorGUI.GUI_Label(container_title, $"{label.content}", new string[] { "labeltext", "list_item_title" });
                label_title.pickingMode = PickingMode.Ignore;

                container_title.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    Node var_nodes = util_XGraphEditorUtility.GetGraphviewWindow().xw_graphView.FindNode(label.guid);
                    if (var_nodes is VNode_Label sk)
                        sk.Highlight();
                });

                container_title.RegisterCallback<PointerOutEvent>((evt) =>
                {
                    Node var_nodes = util_XGraphEditorUtility.GetGraphviewWindow().xw_graphView.FindNode(label.guid);
                    if (var_nodes is VNode_Label sk)
                        sk.UnHighlight();
                });
            }
        }
        /// <summary>
        /// 列表 - 贴图节点
        /// </summary>
        /// <param name="fold"></param>
        private void ListDisplay_Decals(Foldout fold)
        {
            fold.Clear();
            for (int i = 0; i < baseScript.Decals.Count; i++)
            {
                ActionDecalData decal = baseScript.Decals[i];

                VisualElement container = new VisualElement();
                container.pickingMode = PickingMode.Ignore;
                container.AddToClassList("list_container_actionasset");
                fold.Add(container);

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg_actionasset");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.pickingMode = PickingMode.Ignore;
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/decal.png");
                container_title.Add(container_icon);

                Label label_title = util_XGraphInspectorGUI.GUI_Label(container_title, $"{(decal.DecalTexture == null ? "暂未指定" : decal.DecalTexture.name)} ", new string[] { "labeltext", "list_item_title" });
                label_title.pickingMode = PickingMode.Ignore;

                container_title.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    Node var_nodes = util_XGraphEditorUtility.GetGraphviewWindow().xw_graphView.FindNode(decal.guid);
                    if (var_nodes is VNode_Decal dc)
                        dc.Highlight();
                });

                container_title.RegisterCallback<PointerOutEvent>((evt) =>
                {
                    Node var_nodes = util_XGraphEditorUtility.GetGraphviewWindow().xw_graphView.FindNode(decal.guid);
                    if (var_nodes is VNode_Decal dc)
                        dc.UnHighlight();
                });
            }
        }
        /// <summary>
        /// 列表 - 编组节点
        /// </summary>
        /// <param name="fold"></param>
        private void ListDisplay_Groups(Foldout fold)
        {
            fold.Clear();
            for (int i = 0; i < baseScript.Groups.Count; i++)
            {
                ActionGroupData group = baseScript.Groups[i];

                VisualElement container = new VisualElement();
                container.pickingMode = PickingMode.Ignore;
                container.AddToClassList("list_container_actionasset");
                fold.Add(container);

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg_actionasset");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.pickingMode = PickingMode.Ignore;
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/decal.png");
                container_title.Add(container_icon);

                Label label_title = util_XGraphInspectorGUI.GUI_Label(container_title, $"{group.name} ", new string[] { "labeltext", "list_item_title" });
                label_title.pickingMode = PickingMode.Ignore;

                container_title.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    xg_GraphView graph = util_XGraphEditorUtility.GetGraphviewWindow().xw_graphView;
                    graph.Group_Highlight(group.group, util_XGraphEditorUtility.Color_From_HexString(graph.FindGroupTheme(group.solution).title_bg_color));
                });

                container_title.RegisterCallback<PointerOutEvent>((evt) =>
                {
                    xg_GraphView graph = util_XGraphEditorUtility.GetGraphviewWindow().xw_graphView;
                    graph.Group_UnHighlight(group.group);
                });
            }
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 初始化目标脚本
        /// </summary>
        public virtual void GetTargetScript()
        {
            baseScript = target as ActionNode_Asset;
        }
        /// <summary>
        /// 寻找序列化属性
        /// </summary>
        public virtual void GetProperties()
        {
            #region 寻找序列化属性
            sp_LastGraphWindowSize = serializedObject.FindProperty("LastGraphWindowSize");
            sp_LastGraphViewPosition = serializedObject.FindProperty("LastGraphViewPosition");
            sp_LastGraphViewZoom = serializedObject.FindProperty("LastGraphViewZoom");
            sp_LastSaveDateTime = serializedObject.FindProperty("LastSaveDateTime");
            #endregion
        }
        #endregion
    }
}