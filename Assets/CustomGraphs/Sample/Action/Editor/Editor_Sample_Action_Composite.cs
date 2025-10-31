namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(Sample_Action_Composite))]
    public class Editor_Sample_Action_Composite : Editor_ActionNode_Base
    {
        private Sample_Action_Composite actionScript;

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

            actionScript = target as Sample_Action_Composite;
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
            fold.text = $"{fold.text}（{actionScript.childNodes.Count}）";
            base.ChildActionFolder_ItemDisplay(fold);

            for (int i = 0; i < actionScript.childNodes.Count; i++)
            {
                ActionNode_Base child = actionScript.childNodes[i];
                util_XGraphInspectorGUI.GUI_Object<ActionNode_Base>(fold, $"{child.identifyName}：", child, new string[] { "field_object" });
            }
        }
    }
}