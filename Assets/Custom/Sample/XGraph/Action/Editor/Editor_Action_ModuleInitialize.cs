namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(Action_ModuleInitialize))]
    public class Editor_Action_ModuleInitialize : Editor_ActionNode_Start
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private Action_ModuleInitialize start;
        /// <summary>
        /// 节点属性
        /// </summary>
        private SerializedProperty sp_activateAllModules;

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

            start = target as Action_ModuleInitialize;
        }
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();

            sp_activateAllModules = serializedObject.FindProperty("activateAllModules");
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
        /// <param name="root"></param>
        public override Foldout Folder_Extensions(VisualElement root)
        {
            Foldout fold = base.Folder_Extensions(root);

            #region  启动时所有模组的激活开关
            Toggle toggle = util_XGraphInspectorGUI.GUI_Field_Bool(fold, "激活所有模组", sp_activateAllModules.boolValue, new string[] { "field_bool" });
            toggle.RegisterValueChangedCallback((value) =>
            {
                // 如果该节点已经存在绑定的变量，则将当前序列化值给到控件值，因为该序列化值已受变量控制
                if (VariableBindConnectorsExist())
                {
                    serializedObject.Update();
                    toggle.value = sp_activateAllModules.boolValue;
                }
                // 否则就代表没有任何变量节点接入，而使用控件值给到序列化属性值
                else
                {
                    sp_activateAllModules.boolValue = value.newValue;
                    serializedObject.ApplyModifiedProperties();
                }
                start.SetStartDispayModules("激活所有模组");
            });
            #endregion

            // 当节点绑定变量时，将变量值同步到控件值
            start.On_Node_Variable_Binded += ((vare) =>
            {
                serializedObject.Update();

                toggle.value = sp_activateAllModules.boolValue;
            });

            // 克隆节点后刷新控件值为克隆后的最新值
            start.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                Action_ModuleInitialize s_clone = clone as Action_ModuleInitialize;
                // 克隆后的行为数据
                Action_ModuleInitialize s_source = source as Action_ModuleInitialize;
                s_clone.activateAllModules = s_source.activateAllModules;

                GetProperties();

                serializedObject.Update();

                toggle.value = sp_activateAllModules.boolValue;
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