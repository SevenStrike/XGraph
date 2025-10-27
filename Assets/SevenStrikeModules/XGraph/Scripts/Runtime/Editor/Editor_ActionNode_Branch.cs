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
    }
}