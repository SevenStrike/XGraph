namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class VariableController<T>
    {
        public KeyCode key_control;
        public T[] data_list;
        public int index = -1;
    }

    public class mc_GraphController : MonoBehaviour
    {
        /// <summary>
        /// 节点运行器
        /// </summary>
        public mc_GraphRunner GraphRunner;

        [Header("节点图流程控制")]
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

        [Header("修改黑板变量：modules")]
        public VariableController<string> vc_modules;
        [Header("修改黑板变量：msg")]
        public VariableController<string> vc_msg;
        [Header("修改黑板变量：delay")]
        public VariableController<float> vc_delay;
        [Header("修改黑板变量：switch")]
        public VariableController<bool> vc_switch;

        private void OnManual_StepComplete()
        {
            util_Dashboard.LogMsg(xMessageType.警告, "节点流程执行控制器", "手动步骤执行完成，可以执行下一步", GraphRunner.SampleAsset.LogEnabled);
        }
        private void OnManual_WaitComplete()
        {
            util_Dashboard.LogMsg(xMessageType.警告, "节点流程执行控制器", "等待节点完成，可以继续执行", GraphRunner.SampleAsset.LogEnabled);
        }

        private void Start()
        {
            Variable_Initialize();
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
            ActionControl();
            Variable_Modified();
        }

        /// <summary>
        /// 初始化变量的目标修改值
        /// </summary>
        private void Variable_Initialize()
        {
            vc_modules.data_list = new string[4] { "Module_A", "Module_B", "Module_C", "Module_D" };
            vc_modules.index = -1;

            vc_msg.data_list = new string[4] { "m_bK7TdR2N", "m_Yp5sQ9Lm", "m_3xG8FjZc", "m_qHn1Vw4R" };
            vc_msg.index = -1;

            vc_delay.data_list = new float[4] { 0, 1, 2, 3 };
            vc_delay.index = -1;

            vc_switch.data_list = new bool[2] { true, false };
            vc_switch.index = -1;
        }
        /// <summary>
        /// 变量修改
        /// </summary>
        private void Variable_Modified()
        {
            // 修改变量 - modue
            if (Input.GetKeyDown(vc_modules.key_control))
            {
                if (vc_modules.index >= vc_modules.data_list.Length - 1)
                    vc_modules.index = 0;
                else
                    vc_modules.index++;

                GraphRunner.SampleAsset.Variable_SetValue<string>("module", vc_modules.data_list[vc_modules.index]);
            }
            // 修改变量 - msg
            if (Input.GetKeyDown(vc_msg.key_control))
            {
                if (vc_msg.index >= vc_msg.data_list.Length - 1)
                    vc_msg.index = 0;
                else
                    vc_msg.index++;

                GraphRunner.SampleAsset.Variable_SetValue<string>("msg", vc_msg.data_list[vc_msg.index]);
            }
            // 修改变量 - delay
            if (Input.GetKeyDown(vc_delay.key_control))
            {
                if (vc_delay.index >= vc_delay.data_list.Length - 1)
                    vc_delay.index = 0;
                else
                    vc_delay.index++;

                GraphRunner.SampleAsset.Variable_SetValue<float>("delay", vc_delay.data_list[vc_delay.index]);
            }
            // 修改变量 - switch
            if (Input.GetKeyDown(vc_switch.key_control))
            {
                if (vc_switch.index >= vc_switch.data_list.Length - 1)
                    vc_switch.index = 0;
                else
                    vc_switch.index++;

                GraphRunner.SampleAsset.Variable_SetValue<bool>("switch", vc_switch.data_list[vc_switch.index]);
            }
        }
        /// <summary>
        /// 行为流程控制
        /// </summary>
        private void ActionControl()
        {
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
        }

    }

#if UNITY_EDITOR
    public class Editor_mc_GraphController : Editor
    {

    }
#endif
}