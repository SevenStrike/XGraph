namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(mc_action_light_control))]
    public class Editor_mc_action_light_control : Editor_xAction_Composite
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private mc_action_light_control mainlight;
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

            mainlight = target as mc_action_light_control;
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
                mc_action_light_control s_source = (mc_action_light_control)source;
                // 克隆后的行为数据
                mc_action_light_control s_clone = (mc_action_light_control)clone;
                s_clone.lightEnable = s_source.lightEnable;

                GetProperties();

                serializedObject.Update();

                toggle.value = sp_lightEnable.boolValue;
            };

            return fold;
        }
    }
}