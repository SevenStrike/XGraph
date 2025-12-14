namespace SevenStrikeModules.XGraph
{
    using System;

    [Serializable]
    public abstract class xAction_End : xAction_Base
    {
        /// <summary>
        /// 行为节点执行方法
        /// </summary>
        public override void Execute()
        {
            if (On_Node_Excute != null)
                On_Node_Excute();
        }
    }
}