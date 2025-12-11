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

        public string[] vare_data_list;
        public int index_vare = -1;

        public string[] msg_data_list;
        public int index_msg = -1;

        public float[] delay_data_list;
        public int index_delay = -1;

        private void Start()
        {
            vare_data_list = new string[4] { "Module_A", "Module_B", "Module_C", "Module_D" };
            index_vare = -1;

            msg_data_list = new string[4] { "m_bK7TdR2N", "m_Yp5sQ9Lm", "m_3xG8FjZc", "m_qHn1Vw4R" };
            index_msg = -1;

            delay_data_list = new float[4] { 1, 2, 3, 4 };
            index_delay = -1;
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
            // 流程节点 - 开始执行
            if (Input.GetKeyDown(key_ActionStart))
            {
                GraphRunner.Runner_Start();
            }
            // 流程节点 - 杀死
            if (Input.GetKeyDown(key_ActionKill))
            {
                GraphRunner.Runner_Kill();
            }
            // 流程节点 - 暂停执行
            if (Input.GetKeyDown(key_ActionPause))
            {
                GraphRunner.Runner_Pause();
            }
            // 流程节点 - 继续执行
            if (Input.GetKeyDown(key_ActionResume))
            {
                GraphRunner.Runner_Resume();
            }
            // 流程节点 - 切换执行模式
            if (Input.GetKeyDown(key_RunnerMode))
            {
                GraphRunner.ManualExecutionMode = !GraphRunner.ManualExecutionMode;
                util_Dashboard.LogMsg(xMessageType.警告, $"节点流程执行控制器", $"手动执行模式: {(GraphRunner.ManualExecutionMode ? "开启" : "关闭")}", "00ff9d", GraphRunner.SampleAsset.LogEnabled);
            }
            // 流程节点 - 手动步进执行
            if (Input.GetKeyDown(key_ActionManualStep))
            {
                GraphRunner.Manual_Action_Execution();
            }

            // 修改变量 - vare
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (index_vare >= vare_data_list.Length - 1)
                    index_vare = 0;
                else
                    index_vare++;

                GraphRunner.SampleAsset.Variable_SetValue<string>("vare", vare_data_list[index_vare]);
            }

            // 修改变量 - msg
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (index_msg >= msg_data_list.Length - 1)
                    index_msg = 0;
                else
                    index_msg++;

                GraphRunner.SampleAsset.Variable_SetValue<string>("msg", msg_data_list[index_msg]);
            }

            // 修改变量 - delay
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (index_delay >= delay_data_list.Length - 1)
                    index_delay = 0;
                else
                    index_delay++;

                GraphRunner.SampleAsset.Variable_SetValue<float>("delay", delay_data_list[index_delay]);
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