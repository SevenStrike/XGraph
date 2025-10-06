namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class Base_ActionNode_Debug : ActionNode_Debug
    {
        public override void Execute()
        {
            DebugMsg();
        }

        public void DebugMsg()
        {
            Variable variable = null;

            variable = Variable_Get("对象");
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
                Debug.Log(value);
            }
        }
    }
}