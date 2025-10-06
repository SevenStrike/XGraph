namespace SevenStrikeModules.XGraph
{
    public class ActionNode_Branch : ActionNode_Base
    {
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
            SetPredicateState(Variable_Get("条件").GetValue<bool>());
        }

        public void SetPredicateState(bool state)
        {
            PredicateState = state;
        }

        public bool Predicated()
        {
            return PredicateState;
        }
    }
}