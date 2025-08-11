namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;

    public abstract class ActionNode_Relay : ActionNode_Base
    {
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<ActionNode_Base> childNodes = new List<ActionNode_Base>();

        public override void Execute()
        {

        }
    }
}