/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGraph 行为流程图插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Property : xNode_Base
    {
        internal xAction_Property property;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口 - 输出
            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            OutputPort_Set(port_out);
            #endregion

            property = data as xAction_Property;

            // 当Graphview编辑器的主题色改变时
            graphView.gv_GraphWindow.OnThemeColorChanged += OnGraphViewEditorThemeColorChanged;

            // 每次初始化时先清空，避免重复注册
            property.On_InternalVariableValue_Changed = null;
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
            //Draw_Input();

            // 绘制输出节点容器
            Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            // 因为开始节点没有行为输入端，为了让首个输入端口视觉上不会和分割线重叠所以需要矫正偏移
            outputContainer.style.paddingTop = 25;

            return this;
        }

        public override void Draw_Main()
        {
            base.Draw_Main();
        }

        public override void Draw_Extension()
        {
            //base.Draw_Extension();
        }

        public override void Draw_Title()
        {
            base.Draw_Title();

            TitleInputField.RegisterCallback<BlurEvent>(SyncChangeVariableName);
        }

        public override void Draw_Output()
        {
            base.Draw_Output();
        }
        #endregion    

        #region 回调
        /// <summary>
        /// 当Graphview编辑器的主题色改变时
        /// </summary>
        /// <param name="color"></param>
        private void OnGraphViewEditorThemeColorChanged(Color color)
        {

        }
        /// <summary>
        /// 改变节点名称的同时同步修改变量类的名称
        /// </summary>
        /// <param name="evt"></param>
        private void SyncChangeVariableName(BlurEvent evt)
        {
            // Inspector 面板显示属性
            graphView.gv_GraphWindow.xw_InspectorView.InspectorViewer(this);
        }
        #endregion

        #region 重写 - 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();
        }
        #endregion

        #region 重写 - 逻辑
        /// <summary>
        /// 此方法重写目的：属性节点不需要“节点执行模式（顺序/并发）的图标组件”
        /// </summary>
        public override void ExecutionModeMark()
        {
            //base.ExecutionModeMark();
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
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout ins_Folder_Extensions(VisualElement root)
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
            fold.text = $"{fold.text}（{property.PropertyVariables.Count}）";

            CreatePropertyItems(fold);

            // 克隆节点后刷新控件值为克隆后的最新值
            property.On_Node_Duplicated += (clone, source) =>
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
            for (int i = 0; i < property.PropertyVariables.Count; i++)
            {
                Variable prop_vare = property.PropertyVariables[i];

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