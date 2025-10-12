namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ActionNode_Debug : ActionNode_Base
    {
        /// <summary>
        /// 调试信息
        /// </summary>
        public string Message;
        /// <summary>
        /// 是否启用调试
        /// </summary>
        public bool isEnabled;

        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<ActionNode_Base> childNodes = new List<ActionNode_Base>();

        public override void Execute()
        {
            util_Dashboard.LogMsg(util_Dashboard.MsgType.信息, "---> ：", $"调试内容： <color=#d5c2d3>{Message}</color>", isEnabled);
        }

        #region 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            SetMessage("对象");
        }
        #endregion

        #region 辅助
        public void SetMessage(string portName)
        {
            Variable variable = Variable_Get(portName);
            if (variable != null)
            {
                string value = "";
                switch (variable.GetActiveType())
                {
                    case VariableType.String:
                        value = variable.GetValue<string>();
                        break;
                    case VariableType.Float:
                        value = variable.GetValue<float>().ToString();
                        break;
                    case VariableType.Int:
                        value = variable.GetValue<int>().ToString();
                        break;
                    case VariableType.Bool:
                        value = variable.GetValue<bool>().ToString();
                        break;
                    case VariableType.Vector2:
                        value = variable.GetValue<Vector2>().ToString();
                        break;
                    case VariableType.Vector3:
                        value = variable.GetValue<Vector3>().ToString();
                        break;
                    case VariableType.Vector4:
                        value = variable.GetValue<Vector4>().ToString();
                        break;
                    case VariableType.Color:
                        value = variable.GetValue<Color>().ToString();
                        break;
                }
                Message = value;
            }
        }
        #endregion
    }
}