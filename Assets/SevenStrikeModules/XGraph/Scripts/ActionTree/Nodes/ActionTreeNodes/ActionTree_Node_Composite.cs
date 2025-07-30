namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;

    public abstract class ActionTree_Node_Composite : ActionTree_Node_Base
    {
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<ActionTree_Node_Base> childNodes = new List<ActionTree_Node_Base>();

        public override void Execute()
        {

        }
    }
}