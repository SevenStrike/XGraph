namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;

    public abstract class xAction_Start : xAction_Base
    {
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<xAction_Base> childNodes = new List<xAction_Base>();

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