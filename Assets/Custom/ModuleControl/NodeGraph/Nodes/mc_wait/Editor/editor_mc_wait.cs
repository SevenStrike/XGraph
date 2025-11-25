namespace SevenStrikeModules.XGraph
{
    using UnityEditor;

    [CustomEditor(typeof(action_mc_wait))]
    public class editor_mc_wait : editor_xAction_Wait
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        private action_mc_wait wait;

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

            wait = target as action_mc_wait;
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