namespace SevenStrikeModules.XGraph
{
    public abstract class ActionNode_Wait : ActionNode_Base
    {
        /// <summary>
        /// 子节点
        /// </summary>
        public ActionNode_Base childNode;
        /// <summary>
        /// 等待时间
        /// </summary>
        public float Time;

        public override void Execute()
        {

        }
    }
}