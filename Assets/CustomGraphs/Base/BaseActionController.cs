namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class BaseActionController : MonoBehaviour
    {
        public ActionNode_Workflow ActionTreeController;
        public string msg;
        public float delay;

        void Start()
        {
            if (ActionTreeController == null)
                ActionTreeController = GetComponent<ActionNode_Workflow>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                ActionTreeController.Action_Start();
            if (Input.GetKeyDown(KeyCode.K))
                ActionTreeController.Action_Stop();
            if (Input.GetKeyDown(KeyCode.P))
                ActionTreeController.Action_Pause();
            if (Input.GetKeyDown(KeyCode.R))
                ActionTreeController.Action_Resume();

            if (Input.GetKeyDown(KeyCode.F))
            {
                ActionTreeController.ActionAsset.Variable_SetValue("msg", msg);
                ActionTreeController.ActionAsset.Variable_SetValue("delay", delay);
            }
        }
    }
}