namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(ActionNode_Branch))]
    public class Editor_ActionNode_Branch : Editor_ActionNode_Base
    {
        private ActionNode_Branch actionScript;

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

            actionScript = target as ActionNode_Branch;
        }

        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();
        }

        /// <summary>
        /// 自定义派生类扩展控件
        /// </summary>
        /// <param name="fold"></param>
        public override void ExtensionFolder_ItemDisplay(Foldout fold)
        {
            base.ExtensionFolder_ItemDisplay(fold);
        }

        /// <summary>
        /// 子行为列表
        /// </summary>
        /// <param name="fold"></param>
        public override void ChildActionFolder_ItemDisplay(Foldout fold)
        {
            base.ChildActionFolder_ItemDisplay(fold);

            util_XGraphInspectorGUI.GUI_Object<ActionNode_Base>(fold, $"{actionScript.childNode_true.identifyName}：", actionScript.childNode_true, new string[] { "field_object" });

            util_XGraphInspectorGUI.GUI_Object<ActionNode_Base>(fold, $"{actionScript.childNode_false.identifyName}：", actionScript.childNode_false, new string[] { "field_object" });

        }
    }
}