namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class BaseActionController : MonoBehaviour
    {
        public ActionNode_Workflow ActionTreeWorkflow;
        public string msg;
        public float delay;
        public bool branch;

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
                ActionTreeWorkflow.ActionAsset.Variable_SetValue("消息内容", msg);
                ActionTreeWorkflow.ActionAsset.Variable_SetValue("Float", delay);
                ActionTreeWorkflow.ActionAsset.Variable_SetValue("Bool", branch);
            }
        }
    }
}