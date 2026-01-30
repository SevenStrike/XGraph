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
    /// <summary>
    /// 黑板变量类
    /// </summary>
    public class Variable
    {
        public string name;
        public string guid;
        public string description;
        public xVariableType type;

        #region 构造
        /// <summary>
        /// 黑板变量构造
        /// </summary>
        public Variable() { }

        /// <summary>
        /// 黑板变量构造
        /// </summary>
        public Variable(string name = null, xVariableType type = xVariableType.String)
        {
            this.name = name;
            this.type = type;
#if UNITY_EDITOR
            this.guid = UnityEditor.GUID.Generate().ToString();
#endif
        }

        /// <summary>
        /// 黑板变量构造（带描述）
        /// </summary>
        public Variable(string name = null, string des = null, xVariableType type = xVariableType.String)
            : this(name, type) // 调用上面的构造函数
        {
            this.description = des;
        }
        #endregion

        #region 克隆
        /// <summary>
        /// 克隆字段到目标变量
        /// </summary>
        public void CloneVars(Variable target, bool guid_create)
        {
            target.name = name;
#if UNITY_EDITOR
            target.guid = guid_create ? UnityEditor.GUID.Generate().ToString() : guid;
#endif
            target.description = description;
            target.type = type;
        }
        /// <summary>
        /// 克隆字段到目标变量
        /// </summary>
        public Variable CloneVars(bool guid_create)
        {
            Variable vare = new Variable();
            vare.name = name;
#if UNITY_EDITOR
            vare.guid = guid_create ? UnityEditor.GUID.Generate().ToString() : guid;
#endif
            vare.description = description;
            vare.type = type;

            return vare;
        }
        /// <summary>
        /// 克隆变量
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public virtual Variable Clone(bool guid_create)
        {
            return null;
        }
        #endregion

        #region 获取类型
        /// <summary>
        /// 获取值类型
        /// </summary>
        /// <returns></returns>
        public xVariableType GetActiveType() => type;
        #endregion

        #region 获取值
        /// <summary>
        /// 获取值 - 拆箱值
        /// </summary>
        /// <returns></returns>
        public virtual object GetValue()
        {
            return null;
        }

        /// <summary>
        /// 获取值 - 根据类型
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public virtual T GetValue<T>()
        {
            return (T)GetValue();
        }
        #endregion

        #region 设置值
        /// <summary>
        /// 设置值 - 装箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual void SetValue(object value)
        {

        }

        /// <summary>
        /// 设置值 - 根据类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual void SetValue<T>(T value)
        {

        }
        #endregion
    }

    [Serializable]
    /// <summary>
    /// 变量类型 - 整数
    /// </summary>
    public class Variable_Int : Variable
    {
        public int value;

        /// <summary>
        /// 构造器
        /// </summary>
        public Variable_Int(string name, int value = 0) : base(name, xVariableType.Int)
        {
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }

        public override T GetValue<T>()
        {
            // 直接强制转换，调用者知道正确的T类型
            return (T)(object)value;
        }

        public override void SetValue(object value)
        {
            if (value is int intValue)
                this.value = intValue;
            else
                throw new InvalidCastException($"Cannot set Int from {value?.GetType()}");
        }

        public override void SetValue<T>(T value)
        {
            if (typeof(T) == typeof(int) && value is int intValue)
                this.value = intValue;
            else
                throw new InvalidCastException($"Cannot set Int from {typeof(T)}");
        }

        public override Variable Clone(bool guid_create)
        {
            var clone = new Variable_Int(name, value);
            CloneVars(clone, guid_create);
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 变量类型 - 浮点数
    /// </summary>
    public class Variable_Float : Variable
    {
        public float value;

        /// <summary>
        /// 构造器
        /// </summary>
        public Variable_Float(string name, float value = 0) : base(name, xVariableType.Float)
        {
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }

        public override T GetValue<T>()
        {
            // 直接强制转换，调用者知道正确的T类型
            return (T)(object)value;
        }

        public override void SetValue(object value)
        {
            if (value is float floatValue)
                this.value = floatValue;
            else
                throw new InvalidCastException($"Cannot set Float from {value?.GetType()}");
        }

        public override void SetValue<T>(T value)
        {
            if (typeof(T) == typeof(float) && value is float floatValue)
                this.value = floatValue;
            else
                throw new InvalidCastException($"Cannot set Float from {typeof(T)}");
        }

        public override Variable Clone(bool guid_create)
        {
            var clone = new Variable_Float(name, value);
            CloneVars(clone, guid_create);
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 变量类型 - 字符串
    /// </summary>
    public class Variable_String : Variable
    {
        public string value;

        /// <summary>
        /// 构造器
        /// </summary>
        public Variable_String(string name, string value = "") : base(name, xVariableType.String)
        {
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }

        public override T GetValue<T>()
        {
            // 直接强制转换，调用者知道正确的T类型
            return (T)(object)value;
        }

        public override void SetValue(object value)
        {
            if (value is string stringValue)
                this.value = stringValue;
            else
                throw new InvalidCastException($"Cannot set String from {value?.GetType()}");
        }

        public override void SetValue<T>(T value)
        {
            if (typeof(T) == typeof(string) && value is string stringValue)
                this.value = stringValue;
            else
                throw new InvalidCastException($"Cannot set String from {typeof(T)}");
        }

        public override Variable Clone(bool guid_create)
        {
            var clone = new Variable_String(name, value);
            CloneVars(clone, guid_create);
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 变量类型 - 布尔
    /// </summary>
    public class Variable_Bool : Variable
    {
        public bool value;

        /// <summary>
        /// 构造器
        /// </summary>
        public Variable_Bool(string name, bool value = false) : base(name, xVariableType.Bool)
        {
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }

        public override T GetValue<T>()
        {
            // 直接强制转换，调用者知道正确的T类型
            return (T)(object)value;
        }

        public override void SetValue(object value)
        {
            if (value is bool boolValue)
                this.value = boolValue;
            else
                throw new InvalidCastException($"Cannot set Bool from {value?.GetType()}");
        }

        public override void SetValue<T>(T value)
        {
            if (typeof(T) == typeof(bool) && value is bool boolValue)
                this.value = boolValue;
            else
                throw new InvalidCastException($"Cannot set Bool from {typeof(T)}");
        }

        public override Variable Clone(bool guid_create)
        {
            var clone = new Variable_Bool(name, value);
            CloneVars(clone, guid_create);
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 变量类型 - Vector2
    /// </summary>
    public class Variable_Vector2 : Variable
    {
        public Vector2 value;

        /// <summary>
        /// 构造器
        /// </summary>
        public Variable_Vector2(string name, Vector2 value = default) : base(name, xVariableType.Vector2)
        {
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }

        public override T GetValue<T>()
        {
            // 直接强制转换，调用者知道正确的T类型
            return (T)(object)value;
        }

        public override void SetValue(object value)
        {
            if (value is Vector2 Vector2Value)
                this.value = Vector2Value;
            else
                throw new InvalidCastException($"Cannot set Vector2 from {value?.GetType()}");
        }

        public override void SetValue<T>(T value)
        {
            if (typeof(T) == typeof(Vector2) && value is Vector2 Vector2Value)
                this.value = Vector2Value;
            else
                throw new InvalidCastException($"Cannot set Vector2 from {typeof(T)}");
        }

        public override Variable Clone(bool guid_create)
        {
            var clone = new Variable_Vector2(name, value);
            CloneVars(clone, guid_create);
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 变量类型 - Vector3
    /// </summary>
    public class Variable_Vector3 : Variable
    {
        public Vector3 value;

        /// <summary>
        /// 构造器
        /// </summary>
        public Variable_Vector3(string name, Vector3 value = default) : base(name, xVariableType.Vector3)
        {
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }

        public override T GetValue<T>()
        {
            // 直接强制转换，调用者知道正确的T类型
            return (T)(object)value;
        }

        public override void SetValue(object value)
        {
            if (value is Vector3 Vector3Value)
                this.value = Vector3Value;
            else
                throw new InvalidCastException($"Cannot set Vector3 from {value?.GetType()}");
        }

        public override void SetValue<T>(T value)
        {
            if (typeof(T) == typeof(Vector3) && value is Vector3 Vector3Value)
                this.value = Vector3Value;
            else
                throw new InvalidCastException($"Cannot set Vector3 from {typeof(T)}");
        }

        public override Variable Clone(bool guid_create)
        {
            var clone = new Variable_Vector3(name, value);
            CloneVars(clone, guid_create);
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 变量类型 - Vector4
    /// </summary>
    public class Variable_Vector4 : Variable
    {
        public Vector4 value;

        /// <summary>
        /// 构造器
        /// </summary>
        public Variable_Vector4(string name, Vector4 value = default) : base(name, xVariableType.Vector4)
        {
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }

        public override T GetValue<T>()
        {
            // 直接强制转换，调用者知道正确的T类型
            return (T)(object)value;
        }

        public override void SetValue(object value)
        {
            if (value is Vector4 Vector4Value)
                this.value = Vector4Value;
            else
                throw new InvalidCastException($"Cannot set Vector4 from {value?.GetType()}");
        }

        public override void SetValue<T>(T value)
        {
            if (typeof(T) == typeof(Vector4) && value is Vector4 Vector4Value)
                this.value = Vector4Value;
            else
                throw new InvalidCastException($"Cannot set Vector4 from {typeof(T)}");
        }

        public override Variable Clone(bool guid_create)
        {
            var clone = new Variable_Vector4(name, value);
            CloneVars(clone, guid_create);
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 变量类型 - 颜色
    /// </summary>
    public class Variable_Color : Variable
    {
        public Color value;

        /// <summary>
        /// 构造器
        /// </summary>
        public Variable_Color(string name, Color value = default) : base(name, xVariableType.Color)
        {
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }

        public override T GetValue<T>()
        {
            // 直接强制转换，调用者知道正确的T类型
            return (T)(object)value;
        }

        public override void SetValue(object value)
        {
            if (value is Color ColorValue)
                this.value = ColorValue;
            else
                throw new InvalidCastException($"Cannot set Color from {value?.GetType()}");
        }

        public override void SetValue<T>(T value)
        {
            if (typeof(T) == typeof(Color) && value is Color ColorValue)
                this.value = ColorValue;
            else
                throw new InvalidCastException($"Cannot set Color from {typeof(T)}");
        }

        public override Variable Clone(bool guid_create)
        {
            var clone = new Variable_Color(name, value);
            CloneVars(clone, guid_create);
            return clone;
        }
    }
}