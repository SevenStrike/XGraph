namespace SevenStrikeModules.XGraph
{
    public abstract class ActionNode_End : ActionNode_Base
    {
        public override void Execute()
        {
            if (On_Node_Excute != null)
                On_Node_Excute();
        }
    }
}