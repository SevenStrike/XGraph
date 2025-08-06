namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;

    public abstract class actionnode_composite : actionnode_base
    {
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<actionnode_base> childNodes = new List<actionnode_base>();

        public override void Execute()
        {

        }
    }
}