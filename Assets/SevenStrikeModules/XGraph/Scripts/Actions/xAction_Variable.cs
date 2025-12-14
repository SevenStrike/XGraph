namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEngine;

    [Serializable]
    public class xAction_Variable : xAction_Base
    {
        [SerializeReference] public Variable variable;

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

        /// <summary>
        /// 为变量类型行为数据特化克隆方法，将变量先克隆一份然后根据类型赋新值以脱离引用
        /// </summary>
        /// <returns></returns>
        public override xAction_Base Clone()
        {
            // 调用基类的Clone方法
            xAction_Variable clone = base.Clone() as xAction_Variable;

            // 复制派生类特有的字段
            if (clone != null)
            {
                clone.variable = this.variable.Clone(false);
                switch (this.variable.type)
                {
                    case xVariableType.String:
                        clone.variable.SetValue(this.variable.GetValue<string>());
                        break;
                    case xVariableType.Float:
                        clone.variable.SetValue(this.variable.GetValue<float>());
                        break;
                    case xVariableType.Int:
                        clone.variable.SetValue(this.variable.GetValue<int>());
                        break;
                    case xVariableType.Bool:
                        clone.variable.SetValue(this.variable.GetValue<bool>());
                        break;
                    case xVariableType.Vector2:
                        clone.variable.SetValue(this.variable.GetValue<Vector2>());
                        break;
                    case xVariableType.Vector3:
                        clone.variable.SetValue(this.variable.GetValue<Vector3>());
                        break;
                    case xVariableType.Vector4:
                        clone.variable.SetValue(this.variable.GetValue<Vector4>());
                        break;
                    case xVariableType.Color:
                        clone.variable.SetValue(this.variable.GetValue<Color>());
                        break;
                }
            }

            return clone;
        }

        /// <summary>
        /// 当内部变量有其他变量&属性值接入时的变量的获取
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public Variable VariableChildVariableGet(string portName)
        {
            // 按优先级查找变量：属性变量 → 黑板变量 → 内部变量
            Variable variable = PropertysDatas_Get(portName) ?? VariableDatas_Get(portName) ?? InternalVariableDatas_Get(portName);

            if (variable != null)
            {
                switch (variable.type)
                {
                    case xVariableType.String:
                        this.variable.SetValue(variable.GetValue<string>());
                        break;
                    case xVariableType.Float:
                        this.variable.SetValue(variable.GetValue<float>());
                        break;
                    case xVariableType.Int:
                        this.variable.SetValue(variable.GetValue<int>());
                        break;
                    case xVariableType.Bool:
                        this.variable.SetValue(variable.GetValue<bool>());
                        break;
                    case xVariableType.Vector2:
                        this.variable.SetValue(variable.GetValue<Vector2>());
                        break;
                    case xVariableType.Vector3:
                        this.variable.SetValue(variable.GetValue<Vector3>());
                        break;
                    case xVariableType.Vector4:
                        this.variable.SetValue(variable.GetValue<Vector4>());
                        break;
                    case xVariableType.Color:
                        this.variable.SetValue(variable.GetValue<Color>());
                        break;
                }

                return variable;
            }

            return this.variable;
        }
    }
}