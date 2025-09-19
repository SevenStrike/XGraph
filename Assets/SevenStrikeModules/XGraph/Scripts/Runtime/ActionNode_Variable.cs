using System;
using UnityEngine;

namespace SevenStrikeModules.XGraph
{
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

        public override void Execute()
        {

        }
    }
}