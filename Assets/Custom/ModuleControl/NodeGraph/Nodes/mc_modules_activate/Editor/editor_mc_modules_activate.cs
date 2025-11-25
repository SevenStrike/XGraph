namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(action_mc_modules_activate))]
    public class editor_mc_modules_activate : editor_xAction_Composite
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private action_mc_modules_activate activator;
        /// <summary>
        /// 节点序列化属性
        /// </summary>
        private SerializedProperty
            sp_activateState;

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

            activator = target as action_mc_modules_activate;
        }
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();
            sp_activateState = serializedObject.FindProperty("activateState");
        }

        //------------------------------------------------------

        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout Folder_Extensions(VisualElement root)
        {
            Foldout fold = base.Folder_Extensions(root);

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

                field_state.value = sp_activateState.boolValue;
            };

            // 克隆节点后刷新控件值为克隆后的最新值
            activator.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                action_mc_modules_activate s_source = (action_mc_modules_activate)source;
                // 克隆后的行为数据
                action_mc_modules_activate s_clone = (action_mc_modules_activate)clone;
                s_clone.activateState = s_source.activateState;

                GetProperties();

                serializedObject.Update();

                field_state.value = sp_activateState.boolValue;
            };

            return fold;
        }
    }
}