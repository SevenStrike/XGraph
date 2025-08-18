namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class ActionTreeTest : MonoBehaviour
    {
        ActionTreeController ActionTreeController;

        void Start()
        {
            if (ActionTreeController == null)
                ActionTreeController = GetComponent<ActionTreeController>();
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
        }
    }
}