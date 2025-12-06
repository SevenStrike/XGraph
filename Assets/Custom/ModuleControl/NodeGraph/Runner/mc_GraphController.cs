namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class mc_GraphController : MonoBehaviour
    {
        /// <summary>
        /// 节点运行器
        /// </summary>
        public mc_GraphRunner GraphRunner;
        /// <summary>
        /// 执行节点流程
        /// </summary>
        public KeyCode key_ActionStart = KeyCode.S;
        /// <summary>
        /// 杀死节点流程执行
        /// </summary>
        public KeyCode key_ActionKill = KeyCode.K;
        /// <summary>
        /// 暂停节点流程执行
        /// </summary>
        public KeyCode key_ActionPause = KeyCode.P;
        /// <summary>
        /// 继续节点流程执行
        /// </summary>
        public KeyCode key_ActionResume = KeyCode.R;
        /// <summary>
        /// 手动执行节点流程
        /// </summary>
        public KeyCode key_ActionManualStep = KeyCode.Space;
        /// <summary>
        /// 手动执行节点流程开关
        /// </summary>
        public KeyCode key_RunnerMode = KeyCode.M;

        private void Start()
        {

        }

        private void OnDestroy()
        {
            // 注销回调
            if (GraphRunner != null)
            {
                GraphRunner.OnManual_StepComplete -= OnManual_StepComplete;
                GraphRunner.OnManual_WaitComplete -= OnManual_WaitComplete;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(key_ActionStart))
                GraphRunner.Runner_Start();
            if (Input.GetKeyDown(key_ActionKill))
                GraphRunner.Runner_Kill();
            if (Input.GetKeyDown(key_ActionPause))
                GraphRunner.Runner_Pause();
            if (Input.GetKeyDown(key_ActionResume))
                GraphRunner.Runner_Resume();
            if (Input.GetKeyDown(key_RunnerMode))
            {
                GraphRunner.ManualExecutionMode = !GraphRunner.ManualExecutionMode;
                util_Dashboard.LogMsg(xMessageType.警告, $"节点流程执行控制器", $"手动执行模式: {(GraphRunner.ManualExecutionMode ? "开启" : "关闭")}", "00ff9d", GraphRunner.SampleAsset.LogEnabled);
            }

            if (Input.GetKeyDown(key_ActionManualStep))
            {
                GraphRunner.Manual_Action_Execution();
            }
        }

        private void OnManual_StepComplete()
        {
            util_Dashboard.LogMsg(xMessageType.警告, "节点流程执行控制器", "手动步骤执行完成，可以执行下一步", GraphRunner.SampleAsset.LogEnabled);
        }

        private void OnManual_WaitComplete()
        {
            util_Dashboard.LogMsg(xMessageType.警告, "节点流程执行控制器", "等待节点完成，可以继续执行", GraphRunner.SampleAsset.LogEnabled);
        }
    }
}