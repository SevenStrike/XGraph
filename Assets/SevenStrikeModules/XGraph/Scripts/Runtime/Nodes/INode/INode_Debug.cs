namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class INode_Debug : ActionNode_Debug
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