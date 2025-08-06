namespace SevenStrikeModules.XGraph
{
    public abstract class actionnode_wait : actionnode_base
    {
        /// <summary>
        /// 子节点
        /// </summary>
        public actionnode_base ChildNode;
        /// <summary>
        /// 等待时间
        /// </summary>
        public float Time;

        public override void Execute()
        {

        }
    }
}