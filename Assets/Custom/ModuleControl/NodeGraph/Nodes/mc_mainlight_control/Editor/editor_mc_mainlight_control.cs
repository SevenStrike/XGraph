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
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(action_mc_mainlight_control))]
    public class editor_mc_mainlight_control : editor_xAction_Composite
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private action_mc_mainlight_control mainlight;
        /// <summary>
        /// 节点序列化属性
        /// </summary>
        private SerializedProperty sp_lightEnable;

        public override void OnEnable()
        {
            base.OnEnable();
        }
        /// <summary>
        /// 获取脚本
        /// </summary>
        public override void GetTargetScript()
        {
            base.GetTargetScript();

            mainlight = target as action_mc_mainlight_control;
        }
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();
            sp_lightEnable = serializedObject.FindProperty("lightEnable");
        }

        //------------------------------------------------------

        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="fold"></param>
        public override Foldout Folder_Extensions(VisualElement root)
        {
            Foldout fold = base.Folder_Extensions(root);

            Toggle toggle = util_XGraphInspectorGUI.GUI_Field_Bool(fold, "开关", sp_lightEnable.boolValue, new string[] { "field_bool" });
            toggle.RegisterValueChangedCallback((v) =>
            {
                if (isVariableBinded("开关"))
                {
                    serializedObject.Update();
                    toggle.value = sp_lightEnable.boolValue;
                }
                else
                {
                    sp_lightEnable.boolValue = v.newValue;
                    serializedObject.ApplyModifiedProperties();
                }
                mainlight.Set_LightEnabled("开关");
            });

            // 当节点绑定变量时，将变量值同步到控件值
            mainlight.On_Node_Variable_Binded += (vare) =>
            {
                serializedObject.Update();
                toggle.value = sp_lightEnable.boolValue;
            };

            // 克隆节点后刷新控件值为克隆后的最新值
            mainlight.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                action_mc_mainlight_control s_source = (action_mc_mainlight_control)source;
                // 克隆后的行为数据
                action_mc_mainlight_control s_clone = (action_mc_mainlight_control)clone;
                s_clone.lightEnable = s_source.lightEnable;

                GetProperties();

                serializedObject.Update();

                toggle.value = sp_lightEnable.boolValue;
            };

            return fold;
        }
    }
}