namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class Sample_Controller : MonoBehaviour
    {
        public Sample_Workflow Workflow;
        public string msg;
        public float delay;
        public bool branch;

        void Start()
        {
            if (Workflow == null)
                Workflow = GetComponent<Sample_Workflow>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                Workflow.Action_Start();
            if (Input.GetKeyDown(KeyCode.K))
                Workflow.Action_Stop();
            if (Input.GetKeyDown(KeyCode.P))
                Workflow.Action_Pause();
            if (Input.GetKeyDown(KeyCode.R))
                Workflow.Action_Resume();

            if (Input.GetKeyDown(KeyCode.F))
            {
                Workflow.ActionAsset.Variable_SetValue("消息内容", msg);
                Workflow.ActionAsset.Variable_SetValue("Float", delay);
                Workflow.ActionAsset.Variable_SetValue("Bool", branch);
            }
        }
    }
}