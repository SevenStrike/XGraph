namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class Base_ActionNode_Debug : ActionNode_Debug
    {
        public string Message;
        //public BlackBoardVariable_String Variable;

        public override void Execute()
        {
            DebugMsg();
        }

        public void DebugMsg()
        {
            //if (Variable != null)
            //{
            //    Debug.Log(Variable.GetValue());
            //}
            //else
            //    Debug.Log(Message);
        }
    }
}