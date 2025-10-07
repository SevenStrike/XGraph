using System.Collections.Generic;

namespace SevenStrikeModules.XGraph
{
    public abstract class ActionNode_Debug : ActionNode_Base
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