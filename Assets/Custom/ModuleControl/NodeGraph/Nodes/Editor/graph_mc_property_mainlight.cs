namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class graph_mc_property_mainlight : xNode_Property
    {
        action_mc_property_mainlight property_mainlight;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口设置
            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            // 加入行为端口
            port_out.Add(new xGraph_NodePort("强度", typeof(Variable_Float), Port.Capacity.Multi));
            port_out.Add(new xGraph_NodePort("范围", typeof(Variable_Float), Port.Capacity.Multi));
            port_out.Add(new xGraph_NodePort("颜色", typeof(Variable_Color), Port.Capacity.Multi));
            OutputPort_Set(port_out);
            #endregion

            property_mainlight = data as action_mc_property_mainlight;
        }

        #region 重写 - 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();
        }
        /// <summary>
        /// 当克隆节点时
        /// </summary>
        /// <param name="list"></param>
        public override void On_Nodes_Duplicated(List<DuplicateNodeData> list)
        {
            base.On_Nodes_Duplicated(list);

            foreach (var node in list)
            {
                if (node.DuplicatedNode is graph_mc_property_mainlight cur)
                {
                    // 找到克隆的父物体行为节点
                    graph_mc_property_mainlight source = graphView.FindNode(node.SourceNodeGuid) as graph_mc_property_mainlight;

                    // 调用行为数据脚本中的 On_Node_Duplicated 事件以便于行为数据Editor界面下的控件获取克隆父物体的特定变量数据
                    if (cur.ActionData.On_Node_Duplicated != null)
                        cur.ActionData.On_Node_Duplicated(cur.ActionData, source.ActionData);
                }
            }
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
        /// 子行为折叠容器
        /// </summary>
        /// <param name="fold"></param>
        public override Foldout ins_Folder_ChildActions(VisualElement root)
        {
            return null;
        }
        /// <summary>
        /// 黑板变量折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout ins_Folder_BlackBoardVariable(VisualElement root)
        {
            return null;
        }
        /// <summary>
        /// 内部变量折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout ins_Folder_InternalVariable(VisualElement root)
        {
            return null;
        }
        /// <summary>
        /// 属性记录折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout ins_Folder_BindedPropertys(VisualElement root)
        {
            return null;
        }
        /// <summary>
        /// 节点父行为容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout ins_Folder_ParentNode(VisualElement root)
        {
            return null;
        }
        /// <summary>
        /// 属性节点的属性项折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout ins_Folder_Propertys(VisualElement root)
        {
            Foldout fold = base.ins_Folder_Propertys(root);
            fold.text = $"{fold.text}（{property_mainlight.PropertyVariables.Count}）";

            CreatePropertyItems(fold);

            // 克隆节点后刷新控件值为克隆后的最新值
            property_mainlight.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                xAction_Property s_source = (xAction_Property)source;
                // 克隆后的行为数据
                xAction_Property s_clone = (xAction_Property)clone;
                s_clone.PropertyVariables = s_source.PropertyVariables;

                CreatePropertyItems(fold);
            };
            return fold;
        }
        /// <summary>
        /// 创建属性项卡片
        /// </summary>
        /// <param name="fold"></param>
        private void CreatePropertyItems(Foldout fold)
        {
            fold.Clear();
            for (int i = 0; i < property_mainlight.PropertyVariables.Count; i++)
            {
                Variable prop_vare = property_mainlight.PropertyVariables[i];

                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                fold.Add(container);

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/property.png");
                container_title.Add(container_icon);

                util_XGraphInspectorGUI.GUI_Label(container_title, prop_vare.type.ToString(), new string[] { "list_item_marktext" });
                util_XGraphInspectorGUI.GUI_Label(container_title, $"{prop_vare.name}", new string[] { "labeltext", "list_item_title" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>解释：</b><color=#e1e1e1>{prop_vare.description}</color>", new string[] { "list_item_label" });

                xVariableType vare_type = prop_vare.type;
                switch (vare_type)
                {
                    case xVariableType.String:
                        util_XGraphInspectorGUI.GUI_Label(container, $"<b>值：</b><color=#e1e1e1>{prop_vare.GetValue<string>()}</color>", new string[] { "list_item_label" });
                        break;
                    case xVariableType.Float:
                        util_XGraphInspectorGUI.GUI_Field_Float(container, $"<b>值：</b>", prop_vare.GetValue<float>(), new string[] { "list_item_label" });
                        break;
                    case xVariableType.Int:
                        util_XGraphInspectorGUI.GUI_Field_Int(container, $"<b>值：</b>", prop_vare.GetValue<int>(), new string[] { "list_item_label" });
                        break;
                    case xVariableType.Bool:
                        util_XGraphInspectorGUI.GUI_Field_Bool(container, $"<b>值：</b>", prop_vare.GetValue<bool>(), new string[] { "list_item_label" });
                        break;
                    case xVariableType.Vector2:
                        util_XGraphInspectorGUI.GUI_Field_Vector2(container, $"<b>值：</b>", prop_vare.GetValue<Vector2>(), new string[] { "list_item_label" });
                        break;
                    case xVariableType.Vector3:
                        util_XGraphInspectorGUI.GUI_Field_Vector3(container, $"<b>值：</b>", prop_vare.GetValue<Vector3>(), new string[] { "list_item_label" });
                        break;
                    case xVariableType.Vector4:
                        util_XGraphInspectorGUI.GUI_Field_Vector4(container, $"<b>值：</b>", prop_vare.GetValue<Vector4>(), new string[] { "list_item_label" });
                        break;
                    case xVariableType.Color:
                        util_XGraphInspectorGUI.GUI_Field_Color(container, $"<b>值：</b>", prop_vare.GetValue<Color>(), new string[] { "list_item_label" });
                        break;
                }
            }
        }
        #endregion
    }
}