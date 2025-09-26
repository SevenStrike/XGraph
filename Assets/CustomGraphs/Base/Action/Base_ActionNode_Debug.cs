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
            if (VariableData_ValueExist("string-s"))
            {
                string msg = VariableData_GetValue("string-s").GetValue<string>();
                Debug.Log(msg);
            }
            else
            {
                Debug.Log(Message);
            }
        }
    }
}