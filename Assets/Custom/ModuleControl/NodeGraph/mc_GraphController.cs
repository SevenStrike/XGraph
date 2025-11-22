using UnityEngine;

public class mc_GraphController : MonoBehaviour
{
    public mc_GraphRunner GraphRunner;
    public KeyCode actionStart = KeyCode.S;
    public KeyCode actionKill = KeyCode.K;
    public KeyCode actionPause = KeyCode.P;
    public KeyCode actionResume = KeyCode.R;

    private void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(actionStart))
            GraphRunner.Action_Start();
        if (Input.GetKeyDown(actionKill))
            GraphRunner.Action_Kill();
        if (Input.GetKeyDown(actionPause))
            GraphRunner.Action_Pause();
        if (Input.GetKeyDown(actionResume))
            GraphRunner.Action_Resume();
    }
}