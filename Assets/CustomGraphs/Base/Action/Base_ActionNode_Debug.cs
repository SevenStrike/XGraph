namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class Base_ActionNode_Debug : ActionNode_Debug
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