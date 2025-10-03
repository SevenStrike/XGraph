namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class BaseActionController : MonoBehaviour
    {
        public ActionNode_Workflow ActionTreeWorkflow;
        public string msg;
        public float delay;

        void Start()
        {
            if (ActionTreeWorkflow == null)
                ActionTreeWorkflow = GetComponent<ActionNode_Workflow>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                ActionTreeWorkflow.Action_Start();
            if (Input.GetKeyDown(KeyCode.K))
                ActionTreeWorkflow.Action_Stop();
            if (Input.GetKeyDown(KeyCode.P))
                ActionTreeWorkflow.Action_Pause();
            if (Input.GetKeyDown(KeyCode.R))
                ActionTreeWorkflow.Action_Resume();

            if (Input.GetKeyDown(KeyCode.F))
            {
                ActionTreeWorkflow.ActionAsset.FindActionNode("8b5f6576e6ba48b48ab3a44232d1e631").Variable_Set("内容", "9876543210");
                ActionTreeWorkflow.ActionAsset.Variable_SetValue("msg", msg);
                ActionTreeWorkflow.ActionAsset.Variable_SetValue("delay", delay);
            }
        }
    }
}