namespace SevenStrikeModules.XGraph
{
    using UnityEditor;

    [CustomEditor(typeof(mc_action_wait))]
    public class Editor_mc_action_wait : Editor_xAction_Wait
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private mc_action_wait wait;

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

            wait = target as mc_action_wait;
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