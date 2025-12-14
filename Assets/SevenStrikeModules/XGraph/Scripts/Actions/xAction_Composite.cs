namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public abstract class xAction_Composite : xAction_Base
    {
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<string> childNodes = new List<string>();

        /// <summary>
        /// 节点执行方法
        /// </summary>
        public override void Execute()
        {
            if (On_Node_Excute != null)
                On_Node_Excute();
        }
    }
}