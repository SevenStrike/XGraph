namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class BaseNode_End : ActionTree_Node_End
    {
        public override void Execute()
        {
            Debug.Log("我是结束节点消息！");
        }
    }
}