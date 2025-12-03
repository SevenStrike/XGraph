namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class graph_mc_module_activate : xNode_Base
    {
        action_mc_module_activate active;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口设置
            List<xGraph_NodePort> port_in = new List<xGraph_NodePort>();
            // 加入行为端口
            port_in.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Single));
            // 加入变量端口
            port_in.Add(new xGraph_NodePort("名称", typeof(Variable_String), Port.Capacity.Single));
            // 加入变量端口
            port_in.Add(new xGraph_NodePort("激活", typeof(Variable_Bool), Port.Capacity.Single));
            InputPort_Set(port_in);

            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            // 加入行为端口
            port_out.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Multi));
            OutputPort_Set(port_out);
            #endregion

            active = data as action_mc_module_activate;
        }

        #region 节点绘制
        public override xNode_Base Draw()
        {
            // 绘制主容器
            Draw_Main();

            // 绘制标题容器
            Draw_Title();

            // 绘制标题按钮容器
            Draw_TitleButton();

            // 绘制顶部容器
            Draw_Top();

            // 绘制输入节点容器
            Draw_Input();

            // 绘制输出节点容器
            Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }
        #endregion

        #region 重写 - 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            active.Set_ActivateModuleName("名称");
            active.Set_ActivateModuleState("激活");
        }
        /// <summary>
        /// 当克隆节点时
        /// </summary>
        /// <param name="list"></param>
        public override void On_Nodes_Duplicated(List<DuplicateNodeData> list)
        {
            base.On_Nodes_Duplicated(list);
        }
        /// <summary>
        /// 当节点重建时
        /// </summary>
        public override void On_Node_Restructure()
        {
            base.On_Node_Restructure();
        }
        /// <summary>
        /// 当节点连线时
        /// </summary>
        /// <param name="edge"></param>
        public override void On_Node_CreateEdge(Edge edge)
        {
            base.On_Node_CreateEdge(edge);
        }
        /// <summary>
        /// 当节点移除连线时
        /// </summary>
        /// <param name="edge"></param>
        public override void On_Node_RemovedEdge(Edge edge)
        {
            base.On_Node_RemovedEdge(edge);
        }
        /// <summary>
        /// 当节点头像改变时
        /// </summary>
        /// <param name="tex"></param>
        public override void On_Node_AvatarChanged(Texture2D tex)
        {
            base.On_Node_AvatarChanged(tex);
        }
        /// <summary>
        /// 当节点执行模式改变时
        /// </summary>
        /// <param name="state"></param>
        public override void On_Node_ConcurrentChanged(bool state)
        {
            base.On_Node_ConcurrentChanged(state);
        }
        /// <summary>
        /// 当改变节点图标时
        /// </summary>
        /// <param name="tex"></param>
        public override void On_Node_IconChanged(Texture2D tex)
        {
            base.On_Node_IconChanged(tex);
        }
        /// <summary>
        /// 当改变节点颜色主题时
        /// </summary>
        public override void On_Node_ThemeColorChanged()
        {
            base.On_Node_ThemeColorChanged();
        }
        /// <summary>
        /// 当改变节点通透模式时
        /// </summary>
        /// <param name="state"></param>
        public override void On_Node_TransparentChanged(bool state)
        {
            base.On_Node_TransparentChanged(state);
        }
        /// <summary>
        /// 当改变节点尺寸时
        /// </summary>
        /// <param name="evt"></param>
        public override void OnSizeChanged(GeometryChangedEvent evt)
        {
            base.OnSizeChanged(evt);
        }
        /// <summary>
        /// 当选中节点时
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
        }
        /// <summary>
        /// 当取消选中节点时
        /// </summary>
        public override void OnUnselected()
        {
            base.OnUnselected();
        }
        #endregion

        #region 重写 - 绘制Inspector
        /// <summary>
        /// 节点的Inspector属性界面绘制
        /// </summary>
        /// <returns></returns>
        public override VisualElement NodeInspectorGUI()
        {
            VisualElement InspectorElement = base.NodeInspectorGUI();

            return InspectorElement;
        }
        /// <summary>
        /// 节点父行为容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout ins_Folder_ParentNode(VisualElement root)
        {
            return base.ins_Folder_ParentNode(root);
        }
        /// <summary>
        /// 子行为折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout ins_Folder_ChildActions(VisualElement root)
        {
            Foldout fold = base.ins_Folder_ChildActions(root);
            fold.text = $"{fold.text}（{active.childNodes.Count}）";

            for (int i = 0; i < active.childNodes.Count; i++)
            {
                xAction_Base child = active.RootAsset.FindActionNode(active.childNodes[i]);

                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);

                container.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(child.guid);
                    if (node is xNode_Base n_base)
                    {
                        n_base.Highlight();
                    }
                });

                container.RegisterCallback<PointerLeaveEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(child.guid);
                    if (node is xNode_Base n_base)
                    {
                        n_base.UnHighlight();
                    }
                });

                container.RegisterCallback<PointerDownEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    if (child != null)
                    {
                        //EditorGUIUtility.PingObject(child);
                    }
                });

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = child.NodeIcon == null ? util_XGraphEditorUtility.AssetLoad<Texture2D>(AssetDatabase.GUIDToAssetPath(child.icon)) : child.NodeIcon;
                container_title.Add(container_icon);

                util_XGraphInspectorGUI.GUI_Label(container_title, $"目标：{child.identifyName}", new string[] { "labeltext", "list_item_title" });
                util_XGraphInspectorGUI.GUI_Label(container_title, "行为", new string[] { "list_item_marktext" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>Guid：</b><color=#e1e1e1>{child.guid}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>行为类型：</b><color=#e1e1e1>{child.actionNodeType}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点类型：</b><color=#e1e1e1>{child.visualNodeType}</color>", new string[] { "list_item_label" });
            }

            return fold;
        }
        /// <summary>
        /// 属性节点的属性项折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout ins_Folder_Propertys(VisualElement root)
        {
            return null;
        }
        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout ins_Folder_Extensions(VisualElement root)
        {
            Foldout fold = base.ins_Folder_Extensions(root);

            #region 模组名称
            TextField field_name = util_XGraphInspectorGUI.GUI_Field_String(fold, "名称", active.activateName, new string[] { "field_text" });
            field_name.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                // 如果该节点已经存在绑定的变量，则将当前序列化值给到控件值，因为该序列化值已受变量控制
                if (isVariableBinded("名称"))
                {
                    field_name.value = active.activateName;
                    Debug.Log(1);
                }
                // 否则就代表没有任何变量节点接入，而使用控件值给到序列化属性值
                else
                {
                    active.activateName = field_name.value;
                    Debug.Log(2);
                }
                active.Set_ActivateModuleName("名称");
            });
            #endregion

            #region 模组激活
            Toggle field_state = util_XGraphInspectorGUI.GUI_Field_Bool(fold, "激活", active.activateState, new string[] { "field_bool" });
            field_state.RegisterValueChangedCallback((v) =>
            {
                // 如果该节点已经存在绑定的变量，则将当前序列化值给到控件值，因为该序列化值已受变量控制
                if (isVariableBinded("激活"))
                {
                    field_state.value = active.activateState;
                }
                // 否则就代表没有任何变量节点接入，而使用控件值给到序列化属性值
                else
                {
                    active.activateState = v.newValue;
                }
                active.Set_ActivateModuleState("激活");
            });
            #endregion

            // 当节点绑定变量时，将变量值同步到控件值
            active.On_Node_Variable_Binded += (vare) =>
            {
                field_name.value = active.activateName;
                field_state.value = active.activateState;
            };

            // 克隆节点后刷新控件值为克隆后的最新值
            active.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                action_mc_module_activate s_source = (action_mc_module_activate)source;
                // 克隆后的行为数据
                action_mc_module_activate s_clone = (action_mc_module_activate)clone;
                s_clone.activateName = s_source.activateName;
                s_clone.activateState = s_source.activateState;

                field_name.value = active.activateName;
                field_state.value = active.activateState;
            };

            return fold;
        }
        #endregion
    }
}