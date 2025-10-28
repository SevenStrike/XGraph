namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(Base_ActionNode_Start))]
    public class Editor_Base_ActionNode_Start : Editor_ActionNode_Base
    {
        private Base_ActionNode_Start actionScript;

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

            actionScript = target as Base_ActionNode_Start;
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

            for (int i = 0; i < actionScript.childNodes.Count; i++)
            {
                ActionNode_Base child = actionScript.childNodes[i];
                util_XGraphInspectorGUI.GUI_Object<ActionNode_Base>(fold, $"{child.identifyName}：", child, new string[] { "field_object" });
            }
        }
    }
}