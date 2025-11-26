namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(action_AZ_desetAllObjects))]
    public class editor_AZ_desetAllObjects : editor_xAction_Start
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private action_AZ_desetAllObjects desetAllObjects;

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

            desetAllObjects = target as action_AZ_desetAllObjects;
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