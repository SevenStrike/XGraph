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
    }
}