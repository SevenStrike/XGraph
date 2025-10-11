namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class Base_ActionNode_Debug : ActionNode_Debug
    {
        public override void Execute()
        {
            Debug.Log(Message);
        }
    }
}