namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(action_mc_property_mainlight))]
    public class editor_mc_property_mainlight : editor_xAction_Property
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private action_mc_property_mainlight property_light;

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

            property_light = target as action_mc_property_mainlight;
        }
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {
            base.GetProperties();
        }

        //------------------------------------------------------
    }
}