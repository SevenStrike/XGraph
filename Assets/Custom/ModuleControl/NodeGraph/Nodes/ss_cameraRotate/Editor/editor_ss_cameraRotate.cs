namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(action_ss_cameraRotate))]
    public class editor_ss_cameraRotate : editor_xAction_Composite
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private action_ss_cameraRotate cameraRotate;

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

            cameraRotate = target as action_ss_cameraRotate;
        }
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();
        }

        //------------------------------------------------------

        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout Folder_Extensions(VisualElement root)
        {
            Foldout fold = base.Folder_Extensions(root);

            return fold;
        }
    }
}