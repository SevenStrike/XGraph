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
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class graph_mc_mainlight_control : xNode_Composite
    {
        action_mc_mainlight_control mainlight_control;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            // 加入变量端口
            InputPort_Add(new xGraph_NodePort("开关", typeof(Variable_Bool), Port.Capacity.Single));// 加入变量端口（输入）

            mainlight_control = base.composite as action_mc_mainlight_control;
        }

        #region 节点绘制
        public override xNode_Base Draw()
        {
            return base.Draw();
        }
        #endregion

        #region 重写 - 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            mainlight_control.Set_LightEnabled("开关");
        }
        #endregion

        #region 重写 - 绘制Inspector
        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout ins_Folder_Extensions(VisualElement root)
        {
            Foldout fold = base.ins_Folder_Extensions(root);

            #region  启动时所有模组的激活开关
            Toggle toggle = util_XGraphInspectorGUI.GUI_Field_Bool(fold, "开关", mainlight_control.lightEnable, new string[] { "field_bool" });
            toggle.RegisterValueChangedCallback((value) =>
            {
                // 如果该节点已经存在绑定的变量，则将当前序列化值给到控件值，因为该序列化值已受变量控制
                if (isVariableBinded("开关"))
                {
                    toggle.value = mainlight_control.lightEnable;
                    mainlight_control.Set_LightEnabled("开关");
                }
                // 否则就代表没有任何变量节点接入，而使用控件值给到序列化属性值
                else
                {
                    mainlight_control.Set_LightEnabled(value.newValue);
                }
            });
            #endregion

            // 当节点绑定变量时，将变量值同步到控件值
            mainlight_control.On_Node_Variable_Binded += ((vare) =>
            {
                toggle.value = mainlight_control.lightEnable;
            });

            // 克隆节点后刷新控件值为克隆后的最新值
            mainlight_control.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                action_mc_mainlight_control s_clone = clone as action_mc_mainlight_control;
                // 克隆后的行为数据
                action_mc_mainlight_control s_source = source as action_mc_mainlight_control;
                s_clone.lightEnable = s_source.lightEnable;

                toggle.value = mainlight_control.lightEnable;
            };

            return fold;
        }
        #endregion
    }
}