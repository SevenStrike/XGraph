namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class xAction_Variable : xAction_Base
    {
        [SerializeReference]
        public Variable variable;

        public Variable Initialized(string name, xVariableType type)
        {
            Variable var = null;

            switch (type)
            {
                case xVariableType.String:
                    var = new Variable_String(name);
                    break;
                case xVariableType.Float:
                    var = new Variable_Float(name);
                    break;
                case xVariableType.Int:
                    var = new Variable_Int(name);
                    break;
                case xVariableType.Bool:
                    var = new Variable_Bool(name);
                    break;
                case xVariableType.Vector2:
                    var = new Variable_Vector2(name);
                    break;
                case xVariableType.Vector3:
                    var = new Variable_Vector3(name);
                    break;
                case xVariableType.Vector4:
                    var = new Variable_Vector4(name);
                    break;
                case xVariableType.Color:
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
        public xVariableType GetVariableType()
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

        /// <summary>
        /// 行为节点执行方法
        /// </summary>
        public override void Execute()
        {

        }
    }
}