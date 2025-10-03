namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class ActionNode_Variable : ActionNode_Base
    {
        [SerializeReference]
        public Variable variable;

        public Variable Initialized(string name, VariableType type)
        {
            Variable var = null;

            switch (type)
            {
                case VariableType.String:
                    var = new Variable_String(name);
                    break;
                case VariableType.Float:
                    var = new Variable_Float(name);
                    break;
                case VariableType.Int:
                    var = new Variable_Int(name);
                    break;
                case VariableType.Bool:
                    var = new Variable_Bool(name);
                    break;
                case VariableType.Vector2:
                    var = new Variable_Vector2(name);
                    break;
                case VariableType.Vector3:
                    var = new Variable_Vector3(name);
                    break;
                case VariableType.Vector4:
                    var = new Variable_Vector4(name);
                    break;
                case VariableType.Color:
                    var = new Variable_Color(name);
                    break;
            }

            return var;
        }

        /// <summary>
        /// 获取变量解释
        /// </summary>
        /// <returns></returns>
        public string GetVariableDescription()
        {
            return variable.description;
        }

        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public T GetVariableValue<T>(T type)
        {
            return variable.GetValue<T>();
        }

        /// <summary>
        /// 获取变量类型
        /// </summary>
        /// <returns></returns>
        public VariableType GetVariableType()
        {
            return variable.type;
        }

        /// <summary>
        /// 获取变量名称
        /// </summary>
        /// <returns></returns>
        public string GetVariableName()
        {
            return variable.name;
        }

        /// <summary>
        /// 获取变量Guid
        /// </summary>
        /// <returns></returns>
        public string GetVariableGuid()
        {
            return variable.guid;
        }

        public override void Execute()
        {

        }
    }
}