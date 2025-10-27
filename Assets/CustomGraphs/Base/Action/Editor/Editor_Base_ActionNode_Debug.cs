namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEditor.Rendering;
    using UnityEngine;
    using UnityEngine.TextCore.Text;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(Base_ActionNode_Debug))]
    public class Editor_Base_ActionNode_Debug : Editor_ActionNode_Base
    {
        private Base_ActionNode_Debug actionScript;
        private TextField field_msg;
        private Toggle field_enabled;

        #region 序列化属性
        private SerializedProperty
            sp_Message,
            sp_isEnabled;
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

            actionScript = target as Base_ActionNode_Debug;
        }

        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();

            #region 寻找序列化属性
            sp_Message = serializedObject.FindProperty("Message");
            sp_isEnabled = serializedObject.FindProperty("isEnabled");
            #endregion
        }

        /// <summary>
        /// 自定义派生类扩展控件
        /// </summary>
        /// <param name="fold"></param>
        public override void ExtensionFolder_ItemDisplay(Foldout fold)
        {
            base.ExtensionFolder_ItemDisplay(fold);

            #region 调试内容
            field_msg = util_XGraphInspectorGUI.GUI_Field_String(fold, "<b>调试内容： </b>", sp_Message.stringValue, new string[] { "field_text" });
            field_msg.RegisterCallback<BlurEvent>((evt) =>
            {
                if (VariableBindConnectorsExist())
                {
                    actionScript.SetMessage("对象");
                    serializedObject.Update();
                    field_msg.value = sp_Message.stringValue;
                }
                else
                {
                    sp_Message.stringValue = field_msg.value;
                    serializedObject.ApplyModifiedProperties();
                }
            });
            // 当根行为资源绑定变量时
            actionScript.On_Node_VariableBinded += (var) =>
            {
                actionScript.SetMessage("对象");
                serializedObject.Update();
                field_msg.value = sp_Message.stringValue;
                serializedObject.ApplyModifiedProperties();
            };
            #endregion

            #region 调试状态
            field_enabled = util_XGraphInspectorGUI.GUI_Field_Bool(fold, "<b>打印调试信息： </b>", sp_isEnabled.boolValue, new string[] { "field_text" });
            field_enabled.RegisterValueChangedCallback((value) =>
            {
                sp_isEnabled.boolValue = value.newValue;
                serializedObject.ApplyModifiedProperties();
            });
            #endregion
        }
    }
}