namespace SevenStrikeModules.XGraph
{
    public abstract class ActionTree_Node_Wait : ActionTree_Node_Base
    {
        /// <summary>
        /// 子节点
        /// </summary>
        public ActionTree_Node_Base ChildNode;
        /// <summary>
        /// 等待时间
        /// </summary>
        public float Time;

        public override void Execute()
        {

        }
    }
}