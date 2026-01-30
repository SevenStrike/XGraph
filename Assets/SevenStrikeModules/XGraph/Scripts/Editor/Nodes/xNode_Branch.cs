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
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Branch : xNode_Base
    {
        internal xAction_Branch branch;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口 - 输入
            List<xGraph_NodePort> port_in = new List<xGraph_NodePort>();
            port_in.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Single));// 加入行为端口（输入）
            port_in.Add(new xGraph_NodePort("条件", typeof(Variable_Bool), Port.Capacity.Single));// 加入变量端口（输入）
            InputPort_Set(port_in);
            #endregion

            #region 端口 - 输出
            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            port_out.Add(new xGraph_NodePort("开", typeof(xAction_Base), Port.Capacity.Single));// 加入行为端口（输出）
            port_out.Add(new xGraph_NodePort("关", typeof(xAction_Base), Port.Capacity.Single));// 加入行为端口（输出）
            OutputPort_Set(port_out);
            #endregion          

            branch = data as xAction_Branch;
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

        public override void Draw_Output()
        {
            outputContainer.AddToClassList("branchOutputOffset");
            base.Draw_Output();
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
        }
        #endregion

        #region 重写 - 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            branch.SetPredicateState("条件");
        }
        #endregion

        #region 重写 - 绘制Inspector
        /// <summary>
        /// 子行为折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout ins_Folder_ChildActions(VisualElement root)
        {
            Foldout fold = base.ins_Folder_ChildActions(root);

            BranchChildDraw(fold, branch.childNode_true, "开路行为");
            BranchChildDraw(fold, branch.childNode_false, "闭路行为");

            return fold;
        }
        /// <summary>
        /// 绘制分支节点信息面板
        /// </summary>
        /// <param name="element"></param>
        /// <param name="child"></param>
        private void BranchChildDraw(VisualElement root, string guid, string des)
        {
            xAction_Base child_true = branch.BaseArgs.RootAsset.FindActionNode(guid);

            if (child_true != null)
            {
                VisualElement container = new VisualElement();
                container.AddToClassList("list_container");
                root.Add(container);

                container.RegisterCallback<PointerEnterEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(child_true.BaseArgs.guid);
                    if (node is xNode_Base n_base)
                    {
                        n_base.Highlight();
                    }
                });

                container.RegisterCallback<PointerLeaveEvent>((evt) =>
                {
                    xg_Window wnd = util_XGraphEditorUtility.GetGraphviewWindow();
                    Node node = wnd.xw_graphView.FindNode(child_true.BaseArgs.guid);
                    if (node is xNode_Base n_base)
                    {
                        n_base.UnHighlight();
                    }
                });

                VisualElement container_title = new VisualElement();
                container_title.AddToClassList("list_titlebg");
                container.Add(container_title);

                VisualElement container_icon = new VisualElement();
                container_icon.AddToClassList("list_item_icon");
                container_icon.style.backgroundImage = child_true.BaseArgs.NodeIcon == null ? util_XGraphEditorUtility.AssetLoad<Texture2D>(AssetDatabase.GUIDToAssetPath(child_true.BaseArgs.icon)) : child_true.BaseArgs.NodeIcon;
                container_title.Add(container_icon);

                util_XGraphInspectorGUI.GUI_Label(container_title, $"目标：{child_true.identifyName}", new string[] { "labeltext", "list_item_title" });
                util_XGraphInspectorGUI.GUI_Label(container_title, des, new string[] { "list_item_marktext" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>Guid：</b><color=#e1e1e1>{child_true.BaseArgs.guid}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>行为类型：</b><color=#e1e1e1>{child_true.BaseArgs.actionNodeType}</color>", new string[] { "list_item_label" });
                util_XGraphInspectorGUI.GUI_Label(container, $"<b>节点类型：</b><color=#e1e1e1>{child_true.BaseArgs.visualNodeType}</color>", new string[] { "list_item_label" });
            }
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

            #region 条件
            Toggle toggle_childselector = util_XGraphInspectorGUI.GUI_Field_Bool(fold, "条件", branch.PredicateState, new string[] { "field_float" });
            toggle_childselector.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                if (isVariableBinded("条件"))
                {
                    toggle_childselector.value = branch.PredicateState;
                    branch.SetPredicateState("条件");
                }
                else
                {
                    branch.SetPredicateState(toggle_childselector.value);
                }
            });
            #endregion

            // 当根行为资源绑定变量时
            branch.On_Node_Variable_Binded += (var) =>
            {
                toggle_childselector.value = branch.PredicateState;
            };

            // 克隆节点后刷新控件值为克隆后的最新值
            branch.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                xAction_Branch s_source = (xAction_Branch)source;
                // 克隆后的行为数据
                xAction_Branch s_clone = (xAction_Branch)clone;
                s_clone.PredicateState = s_source.PredicateState;

                toggle_childselector.value = branch.PredicateState;
            };

            return fold;
        }
        #endregion
    }
}