using System.Collections.Generic;

namespace SevenStrikeModules.XGraph
{
    public abstract class ActionNode_Wait : ActionNode_Base
    {
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<ActionNode_Base> childNodes = new List<ActionNode_Base>();
        /// <summary>
        /// 等待时间
        /// </summary>
        public float Time;

        public override void Execute()
        {

        }
    }
}