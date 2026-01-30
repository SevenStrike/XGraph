/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGraph 行为流程图插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
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