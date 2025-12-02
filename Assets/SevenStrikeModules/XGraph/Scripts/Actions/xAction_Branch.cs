namespace SevenStrikeModules.XGraph
{
    public class xAction_Branch : xAction_Base
    {
        /// <summary>
        /// 条件状态
        /// </summary>
        public bool PredicateState;
        /// <summary>
        /// 子节点 True
        /// </summary>
        public string childNode_true;
        /// <summary>
        /// 子节点 False
        /// </summary>
        public string childNode_false;

        /// <summary>
        /// 节点执行方法
        /// </summary>
        public override void Execute()
        {
            if (On_Node_Excute != null)
                On_Node_Excute();
        }

        #region 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            SetPredicateState("条件");
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 设置条件判断状态
        /// </summary>
        /// <param name="state"></param>
        public void SetPredicateState(string portName)
        {
            PortValue_Set<bool>(portName, value => PredicateState = value);
        }
        #endregion
    }
}