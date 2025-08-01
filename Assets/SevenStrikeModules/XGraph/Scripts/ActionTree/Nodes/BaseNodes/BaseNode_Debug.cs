using UnityEngine;

namespace SevenStrikeModules.XGraph
{
    public class BaseNode_Debug : ActionTree_Node_Debug
    {
        public string Message;

        public override void Execute()
        {
            DebugMsg();
        }

        public void DebugMsg()
        {
            Debug.Log(Message);
        }
    }
}