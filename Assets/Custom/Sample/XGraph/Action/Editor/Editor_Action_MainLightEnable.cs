namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(Action_MainLightEnable))]
    public class Editor_Action_MainLightEnable : Editor_ActionNode_Composite
    {
        private Action_MainLightEnable mainlight;
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

            mainlight = target as Action_MainLightEnable;
        }
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();
            sp_lightEnable = serializedObject.FindProperty("lightEnable");
        }
        /// <summary>
        /// 子行为组件折叠容器
        /// </summary>
        /// <param name="fold"></param>
        public override Foldout Folder_ChildActions(VisualElement root)
        {
            return base.Folder_ChildActions(root);
        }
        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="fold"></param>
        public override Foldout Folder_Extensions(VisualElement root)
        {
            Foldout fold = base.Folder_Extensions(root);

            Toggle toggle = util_XGraphInspectorGUI.GUI_Field_Bool(fold, "灯光开关", sp_lightEnable.boolValue, new string[] { "field_bool" });
            toggle.RegisterValueChangedCallback((v) =>
            {
                if (VariableBindConnectorsExist())
                {
                    serializedObject.Update();
                    toggle.value = sp_lightEnable.boolValue;
                }
                else
                {
                    sp_lightEnable.boolValue = v.newValue;
                    serializedObject.ApplyModifiedProperties();
                }
                mainlight.Set_MainLightToggle("开关");
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
                Action_MainLightEnable s_source = (Action_MainLightEnable)source;
                // 克隆后的行为数据
                Action_MainLightEnable s_clone = (Action_MainLightEnable)clone;
                s_clone.lightEnable = s_source.lightEnable;

                GetProperties();

                serializedObject.Update();

                toggle.value = sp_lightEnable.boolValue;
            };

            return fold;
        }
        /// <summary>
        /// 黑板变量组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout Folder_BlackBoardVariable(VisualElement root)
        {
            return base.Folder_BlackBoardVariable(root);
        }
        /// <summary>
        /// 内部变量组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout Folder_InternalVariable(VisualElement root)
        {
            return base.Folder_InternalVariable(root);
        }
    }
}