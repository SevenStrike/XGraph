using System.Collections.Generic;

namespace SevenStrikeModules.XGraph
{
    public class Mind_ActionNode_Start : ActionNode_Start
    {
        public string Message;
        public List<string> Names = new List<string>();

        public override void Execute()
        {
            base.Execute();
        }
    }
}