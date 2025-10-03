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
            Variable variable = Variable_Get("string-s");
            if (variable != null)
            {
                string msg = variable.GetValue<string>();
                Debug.Log(msg);
            }
            else
            {
                Debug.Log(Message);
            }
        }
    }
}