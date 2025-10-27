namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(Base_ActionNode_Wait))]
    public class Editor_Base_ActionNode_Wait : Editor_ActionNode_Base
    {
        private Base_ActionNode_Wait actionScript;

        #region 序列化属性
        private SerializedProperty
            sp_Time;
        #endregion

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

            actionScript = target as Base_ActionNode_Wait;
        }

        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();

            #region 寻找序列化属性
            sp_Time = serializedObject.FindProperty("Time");
            #endregion
        }

        /// <summary>
        /// 自定义派生类扩展控件
        /// </summary>
        /// <param name="fold"></param>
        public override void ExtensionFolder_ItemDisplay(Foldout fold)
        {
            base.ExtensionFolder_ItemDisplay(fold);

            #region 等待时间
            FloatField field_time = util_XGraphInspectorGUI.GUI_Field_Float(fold, "<b>等待时间： </b>", sp_Time.floatValue, new string[] { "field_float" });
            field_time.RegisterCallback<BlurEvent>((evt) =>
            {
                if (VariableBindConnectorsExist())
                {
                    actionScript.SetWaitTime("时间");
                    serializedObject.Update();
                    field_time.value = sp_Time.floatValue;
                }
                else
                {
                    sp_Time.floatValue = field_time.value;
                    serializedObject.ApplyModifiedProperties();
                }
            });
            // 当根行为资源绑定变量时
            actionScript.On_Node_VariableBinded += (var) =>
            {
                actionScript.SetWaitTime("时间");
                serializedObject.Update();
                field_time.value = sp_Time.floatValue;
                serializedObject.ApplyModifiedProperties();
            };

            #endregion
        }
    }
}