namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class basenode_debug : actionnode_debug
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