namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(mc_action_module_activate))]
    public class Editor_mc_action_module_activate : Editor_xAction_Composite
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private mc_action_module_activate activator;
        /// <summary>
        /// 节点序列化属性
        /// </summary>
        private SerializedProperty
            sp_activateState,
            sp_activateName;

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

            activator = target as mc_action_module_activate;
        }
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();
            sp_activateState = serializedObject.FindProperty("activateState");
            sp_activateName = serializedObject.FindProperty("activateName");
        }

        //------------------------------------------------------

        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout Folder_Extensions(VisualElement root)
        {
            Foldout fold = base.Folder_Extensions(root);

            #region 模组名称
            TextField field_name = util_XGraphInspectorGUI.GUI_Field_String(fold, "名称", sp_activateName.stringValue, new string[] { "field_text" });
            field_name.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                // 如果该节点已经存在绑定的变量，则将当前序列化值给到控件值，因为该序列化值已受变量控制
                if (isVariableBinded("名称"))
                {
                    serializedObject.Update();
                    field_name.value = sp_activateName.stringValue;
                }
                // 否则就代表没有任何变量节点接入，而使用控件值给到序列化属性值
                else
                {
                    sp_activateName.stringValue = field_name.value;
                    serializedObject.ApplyModifiedProperties();
                }
                activator.Set_ActivateModuleName("名称");
            });
            #endregion

            #region 模组激活
            Toggle field_state = util_XGraphInspectorGUI.GUI_Field_Bool(fold, "激活", sp_activateState.boolValue, new string[] { "field_bool" });
            field_state.RegisterValueChangedCallback((v) =>
            {
                // 如果该节点已经存在绑定的变量，则将当前序列化值给到控件值，因为该序列化值已受变量控制
                if (isVariableBinded("激活"))
                {
                    serializedObject.Update();
                    field_state.value = sp_activateState.boolValue;
                }
                // 否则就代表没有任何变量节点接入，而使用控件值给到序列化属性值
                else
                {
                    sp_activateState.boolValue = v.newValue;
                    serializedObject.ApplyModifiedProperties();
                }
                activator.Set_ActivateModuleState("激活");
            });
            #endregion

            // 当节点绑定变量时，将变量值同步到控件值
            activator.On_Node_Variable_Binded += (vare) =>
            {
                serializedObject.Update();

                field_name.value = sp_activateName.stringValue;
                field_state.value = sp_activateState.boolValue;
            };

            // 克隆节点后刷新控件值为克隆后的最新值
            activator.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                mc_action_module_activate s_source = (mc_action_module_activate)source;
                // 克隆后的行为数据
                mc_action_module_activate s_clone = (mc_action_module_activate)clone;
                s_clone.activateName = s_source.activateName;
                s_clone.activateState = s_source.activateState;

                GetProperties();

                serializedObject.Update();

                field_name.value = sp_activateName.stringValue;
                field_state.value = sp_activateState.boolValue;
            };

            return fold;
        }
    }
}