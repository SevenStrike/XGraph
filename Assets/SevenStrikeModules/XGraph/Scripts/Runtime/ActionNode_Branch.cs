namespace SevenStrikeModules.XGraph
{
    public class ActionNode_Branch : ActionNode_Base
    {
        /// <summary>
        /// 条件状态
        /// </summary>
        public bool PredicateState;
        /// <summary>
        /// 子节点 True
        /// </summary>
        public ActionNode_Base childNode_true;
        /// <summary>
        /// 子节点 False
        /// </summary>
        public ActionNode_Base childNode_false;

        public override void Execute()
        {

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
            Variable variable = Variable_Get(portName);
            if (variable != null)
                PredicateState = variable.GetValue<bool>();
        }
        #endregion
    }
}