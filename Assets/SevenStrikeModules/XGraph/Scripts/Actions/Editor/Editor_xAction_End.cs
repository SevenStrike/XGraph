namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(xAction_End), true)]
    public class editor_xAction_End : editor_xAction_Base
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private xAction_End actionScript;

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

            //actionScript = target as xAction_End;
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
        /// 子行为组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout Folder_ChildActions(VisualElement root)
        {
            return null;
        }
        /// <summary>
        /// 属性节点的属性项折叠容器
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override Foldout Folder_Propertys(VisualElement root)
        {
            return null;
        }
    }
}