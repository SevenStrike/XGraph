namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class ActionTree_Node_TweenDebug : ActionTree_Node_Debug
    {
        public string Info;
        public void DebugInfo()
        {
            Debug.Log(Info);
            Debug.LogWarning(Info);
            Debug.LogError(Info);
        }
        public override void Execute()
        {
            DebugInfo();
        }
    }
}