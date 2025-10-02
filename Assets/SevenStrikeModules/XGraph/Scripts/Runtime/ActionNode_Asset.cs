namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection.Emit;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
#endif
    using UnityEngine;
    using UnityEngine.UIElements;
    using static UnityEditor.Progress;
    using Object = UnityEngine.Object;

    #region 创建节点信息结构体
    [Serializable]
    /// <summary>
    /// 创建节点信息结构体 - 行为
    /// </summary>
    public struct NodeCreateArgs_Action
    {
        public string visualName;
        public string prefixNamespace;
        public string prefixClass;
        public string actionNodeType;
        public string iconName;
        public Texture2D nodeIcon;
        public string visualNodeType;
        public bool hasAvatar;
        public Texture2D avatar;
        public string themeSolution;
        public Color themeColor;
        public bool transparentNode;
        public string content;
        public Vector2 position;
        public Vector2 size;
        public Variable variable;
    }

    [Serializable]
    /// <summary>
    /// 创建节点信息结构体 - 便签
    /// </summary>
    public struct NodeCreateArgs_Stick
    {
        public string stickName;
        public string stickContent;
        public Vector2 position;
        public Vector2 size;
    }

    [Serializable]
    /// <summary>
    /// 创建节点信息结构体 - 标签
    /// </summary>
    public struct NodeCreateArgs_Label
    {
        public string content;
        public Vector2 position;
        public Vector2 size;
        public float opacity;
        public int fontSize;
        public bool bold;
        public bool italic;
    }

    [Serializable]
    /// <summary>
    /// 创建节点信息结构体 - 贴图
    /// </summary>
    public struct NodeCreateArgs_Decal
    {
        public Vector2 position;
        public Vector2 size;
        public float opacity;
        public bool hasTexture;
        public Texture2D decalTexture;
        public Vector3 scale;
    }

    [Serializable]
    /// <summary>
    /// 创建节点信息结构体 - 变量
    /// </summary>
    public struct NodeCreateArgs_Variable
    {
        public string name;
        public string description;
        public VariableType type;
        public Vector2 position;
        public Vector2 size;
        public string varguid;
        public bool transparentNode;
        public Variable variable;
    }
    #endregion

    #region 节点数据类
    [Serializable]
    /// <summary>
    /// 贴纸数据
    /// </summary>
    public class ActionDecalData
    {
        /// <summary>
        /// 贴纸识别ID码
        /// </summary>
        public string guid;
        /// <summary>
        /// 节点位置
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// 节点尺寸
        /// </summary>
        public Vector2 size;
        /// <summary>
        /// 节点贴图透明度
        /// </summary>
        public float opacity = 1;
        public bool HasTexture;
        public Texture2D DecalTexture;
        /// <summary>
        /// 贴图缩放
        /// </summary>
        public Vector3 scale = Vector3.one;
        /// <summary>
        /// 贴纸构造器
        /// </summary>
        public ActionDecalData() { }
        /// <summary>
        /// 贴纸构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        public ActionDecalData(string guid, Vector2 pos, Vector2 size, Vector3 scale, float opacity, bool hastex, Texture2D tex)
        {
            this.guid = guid;
            this.position = pos;
            this.scale = scale;
            this.size = size;
            this.opacity = opacity;
            this.HasTexture = hastex;
            this.DecalTexture = tex;
        }
        /// <summary>
        /// 贴纸克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public ActionDecalData Clone(bool guid_create)
        {
            var clone = new ActionDecalData();
#if UNITY_EDITOR
            clone.guid = guid_create ? GUID.Generate().ToString() : guid;
#endif
            clone.position = position;
            clone.size = size;
            clone.HasTexture = HasTexture;
            clone.DecalTexture = DecalTexture;
            clone.scale = scale;
            clone.opacity = opacity;
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 便签数据
    /// </summary>
    public class ActionStickData
    {
        /// <summary>
        /// 便签标题
        /// </summary>
        public string name;
        /// <summary>
        /// 便签内容
        /// </summary>
        public string content;
        /// <summary>
        /// 便签识别ID码
        /// </summary>
        public string guid;
        /// <summary>
        /// 节点位置
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// 节点尺寸
        /// </summary>
        public Vector2 size;
        /// <summary>
        /// 便签构造器
        /// </summary>
        public ActionStickData() { }
        /// <summary>
        /// 便签构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        public ActionStickData(string name, string content, string guid, Vector2 pos, Vector2 size)
        {
            this.name = name;
            this.guid = guid;
            this.content = content;
            this.position = pos;
            this.size = size;
        }
        /// <summary>
        /// 便签克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public ActionStickData Clone(bool guid_create)
        {
            var clone = new ActionStickData();
            clone.name = name;
            clone.content = content;
#if UNITY_EDITOR
            clone.guid = guid_create ? GUID.Generate().ToString() : guid;
#endif
            clone.position = position;
            clone.size = size;
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 标签数据
    /// </summary>
    public class ActionLabelData
    {
        /// <summary>
        /// 标签识别ID码
        /// </summary>
        public string guid;
        /// <summary>
        /// 标签内容
        /// </summary>
        public string content;
        /// <summary>
        /// 节点位置
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// 节点尺寸
        /// </summary>
        public Vector2 size;
        /// <summary>
        /// 标签透明度
        /// </summary>
        public float opacity;
        /// <summary>
        /// 文字内容尺寸
        /// </summary>
        public int fontSize;
        /// <summary>
        /// 文字是否粗体
        /// </summary>
        public bool bold;
        /// <summary>
        /// 文字是否斜体
        /// </summary>
        public bool italic;

        /// <summary>
        /// 标签构造器
        /// </summary>
        public ActionLabelData() { }
        /// <summary>
        /// 标签构造器
        /// </summary>
        /// <param name="content"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="opacity"></param>
        public ActionLabelData(string content, string guid, Vector2 pos, Vector2 size, float opacity, int fontSize, bool bold, bool italic)
        {
            this.guid = guid;
            this.content = content;
            this.position = pos;
            this.size = size;
            this.opacity = opacity;
            this.fontSize = fontSize;
            this.bold = bold;
            this.italic = italic;
        }
        /// <summary>
        /// 标签克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public ActionLabelData Clone(bool guid_create)
        {
            var clone = new ActionLabelData();
            clone.content = content;
#if UNITY_EDITOR
            clone.guid = guid_create ? GUID.Generate().ToString() : guid;
#endif
            clone.position = position;
            clone.size = size;
            clone.opacity = opacity;
            clone.fontSize = fontSize;
            clone.bold = bold;
            clone.italic = italic;
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 编组数据
    /// </summary>
    public class ActionGroupData
    {
#if UNITY_EDITOR
        /// <summary>
        /// 编组的标题
        /// </summary>
        public string name;
        /// <summary>
        /// 编组的识别ID码
        /// </summary>
        public string guid;
        /// <summary>
        /// 编组的位置
        /// </summary>
        public Vector2 pos;
        /// <summary>
        /// 编组的颜色识别
        /// </summary>
        public string solution = "M 默认";
        /// <summary>
        /// 编组组件
        /// </summary>
        [SerializeField] public Group group;
        /// <summary>
        /// 编组内容容器组件
        /// </summary>
        [SerializeField] public VisualElement groupcontainer;
        /// <summary>
        /// 编组内的所有节点的识别ID码
        /// </summary>
        public List<string> guids = new List<string>();
        /// <summary>
        /// 编组中是否包含节点
        /// </summary>
        public bool hasAvatarNodes;

        /// <summary>
        /// 编组克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public ActionGroupData Clone(bool guid_create)
        {
            var clone = new ActionGroupData();
            clone.name = name;
            clone.guid = guid_create ? GUID.Generate().ToString() : guid;
            clone.pos = pos;
            clone.guids = new List<string>();
            guids.ForEach(guid => clone.guids.Add(guid));
            clone.solution = solution;
            clone.group = null;
            clone.groupcontainer = null;
            clone.hasAvatarNodes = hasAvatarNodes;
            return clone;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public ActionGroupData() { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="guids"></param>
        /// <param name="group"></param>
        public ActionGroupData(string name, string guid, Vector2 pos, List<string> guids, string solution, Group group, VisualElement groupcontainer)
        {
            this.name = name;
            this.guid = guid;
            this.pos = pos;
            this.guids = guids;
            this.solution = solution;
            this.group = group;
            this.groupcontainer = groupcontainer;
        }
#endif
    }

    [Serializable]
    /// <summary>
    /// 变量数据
    /// </summary>
    public class ActionVariableData
    {
        /// <summary>
        /// 变量节点显示名称
        /// </summary>
        [SerializeField] public string name;
        /// <summary>
        /// 变量节点显示解释
        /// </summary>
        [SerializeField] public string description;
        /// <summary>
        /// 变量节点识别ID码
        /// </summary>
        [SerializeField] public string guid;
        /// <summary>
        /// 变量原始识别ID码
        /// </summary>
        [SerializeField] public string varguid;
        /// <summary>
        /// 变量节点显示类型
        /// </summary>
        [SerializeField] public VariableType type;
        /// <summary>
        /// 变量类
        /// </summary>
        [SerializeReference] public Variable variable;
        /// <summary>
        /// 透明背景节点模式
        /// </summary>
        [SerializeField] public bool TransparentNode = false;
        /// <summary>
        /// 节点位置
        /// </summary>
        [SerializeField] public Vector2 position;
        /// <summary>
        /// 节点尺寸
        /// </summary>
        [SerializeField] public Vector2 size;

        /// <summary>
        /// 变量构造器
        /// </summary>
        public ActionVariableData() { }
        /// <summary>
        /// 变量构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="type"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="varguid"></param>
        public ActionVariableData(string name, string description, VariableType type, string guid, Vector2 pos, Vector2 size, string varguid, Variable variable, bool transparentNode)
        {
            this.name = name;
            this.description = description;
            this.type = type;
            this.guid = guid;
            this.position = pos;
            this.size = size;
            this.varguid = varguid;
            this.variable = variable;
        }
        /// <summary>
        /// 变量克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public ActionVariableData Clone(bool guid_create)
        {
            var clone = new ActionVariableData();
            clone.name = this.name;
            clone.description = this.description;
            clone.type = this.type;
#if UNITY_EDITOR
            clone.guid = guid_create ? GUID.Generate().ToString() : guid;
#endif
            clone.position = position;
            clone.varguid = varguid;
            clone.size = size;
            clone.TransparentNode = TransparentNode;
            clone.variable = variable;
            return clone;
        }
    }
    #endregion

    [Serializable]
    public class VarialbleGuidConnector
    {
        [SerializeField] public string VariableNodeGuid;
        [SerializeField] public string TargetPortName;
        [SerializeReference] public Variable variable;

        public VarialbleGuidConnector() { }

        public VarialbleGuidConnector(string guid, string name, Variable variable)
        {
            this.VariableNodeGuid = guid;
            this.TargetPortName = name;
            this.variable = variable.Clone(false);
        }
    }

    [Serializable]
    public class VarialbleInternalGuidConnector
    {
        [SerializeField] public string VariableNodeGuid;
        [SerializeField] public string TargetPortName;
        [SerializeReference] public Variable variable;

        public VarialbleInternalGuidConnector() { }

        public VarialbleInternalGuidConnector(string guid, string name, Variable variable)
        {
            this.VariableNodeGuid = guid;
            this.TargetPortName = name;
            this.variable = variable.Clone(false);
        }
    }

    [Serializable]
    public class VarialbleInternalConnector
    {
        [SerializeField] public string VariableNodeGuid;
        [SerializeField] public string TargetPortName;
        [SerializeReference] public Variable variable;

        public VarialbleInternalConnector() { }

        public VarialbleInternalConnector(string guid, string name, Variable variable)
        {
            this.VariableNodeGuid = guid;
            this.TargetPortName = name;
            this.variable = variable.Clone(false);
        }
    }

    #region 变量
    /// <summary>
    /// 黑板变量值类型
    /// </summary>
    public enum VariableType
    {
        /// <summary>
        /// 值 - 字符串
        /// </summary>
        String = 0,
        /// <summary>
        /// 值 - 浮点
        /// </summary>
        Float = 1,
        /// <summary>
        /// 值 - 整数
        /// </summary>
        Int = 2,
        /// <summary>
        /// 值 - 布尔开关
        /// </summary>
        Bool = 3,
        /// <summary>
        /// 值 - 2维向量
        /// </summary>
        Vector2 = 4,
        /// <summary>
        /// 值 - 3维向量
        /// </summary>
        Vector3 = 5,
        /// <summary>
        /// 值 - 4维向量
        /// </summary>
        Vector4 = 6,
        /// <summary>
        /// 值 - 颜色
        /// </summary>
        Color = 7
    }

    [Serializable]
    /// <summary>
    /// 黑板变量类
    /// </summary>
    public class Variable
    {
        public string name;
        public string guid;
        public string description;
        public VariableType type;

        #region 构造
        /// <summary>
        /// 黑板变量构造
        /// </summary>
        public Variable() { }

        /// <summary>
        /// 黑板变量构造
        /// </summary>
        public Variable(string name = null, VariableType type = VariableType.String)
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
        public Variable(string name = null, string des = null, VariableType type = VariableType.String)
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
        public VariableType GetActiveType() => type;
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
        public Variable_Int(string name, int value = 0) : base(name, VariableType.Int)
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
        public Variable_Float(string name, float value = 0) : base(name, VariableType.Float)
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
        public Variable_String(string name, string value = "") : base(name, VariableType.String)
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
        public Variable_Bool(string name, bool value = false) : base(name, VariableType.Bool)
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
        public Variable_Vector2(string name, Vector2 value = default) : base(name, VariableType.Vector2)
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
        public Variable_Vector3(string name, Vector3 value = default) : base(name, VariableType.Vector3)
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
        public Variable_Vector4(string name, Vector4 value = default) : base(name, VariableType.Vector4)
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
        public Variable_Color(string name, Color value = default) : base(name, VariableType.Color)
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
    #endregion

    #region Graphview 主题类
    [Serializable]
    /// <summary>
    /// Graphview 背景主题样式参数类
    /// </summary>
    public class GraphviewGridBackgroundThemes
    {
        public Color bgcolor = new Color(0.15f, 0.15f, 0.15f, 1);
        public Color gridcolor = new Color(0.18f, 0.18f, 0.18f, 1);
        public Color customimagecolor = new Color(1, 1, 1, 0);
        public Color thickLinecolor = new Color(0, 0, 0, 0);
        public Color themecolor = new Color(0.23f, 0.99f, 0.60f, 1);
        public float spacing = 18;
        public int thicklines = 18;
        public Texture2D customimage;

        public GraphviewGridBackgroundThemes() { }

        public GraphviewGridBackgroundThemes Clone()
        {
            GraphviewGridBackgroundThemes t = new GraphviewGridBackgroundThemes();
            t.bgcolor = bgcolor;
            t.gridcolor = gridcolor;
            t.thickLinecolor = thickLinecolor;
            t.spacing = spacing;
            t.thicklines = thicklines;
            t.customimagecolor = customimagecolor;
            t.customimage = customimage;
            t.themecolor = themecolor;
            return t;
        }
    }

    [Serializable]
    /// <summary>
    /// Graphview 选择框主题样式参数类
    /// </summary>
    public class GraphviewRectangleSelectorThemes
    {
        public Color rectangleSelectorLineColor = new Color(1, 1, 1, 0.6f);
        public int segments = 4;
        public bool displayCoordinate = false;

        public GraphviewRectangleSelectorThemes() { }

        public GraphviewRectangleSelectorThemes Clone()
        {
            GraphviewRectangleSelectorThemes t = new GraphviewRectangleSelectorThemes();
            t.rectangleSelectorLineColor = rectangleSelectorLineColor;
            t.segments = segments;
            t.displayCoordinate = displayCoordinate;
            return t;
        }
    }
    #endregion

    #region 主题配置文件类结构
    [Serializable]
    /// <summary>
    /// 编组主题结构
    /// </summary>
    public class ThemeData_Group
    {
        public string solution = "默认";
        public string title_bg_color = "#3C725D";
        public string title_text_color = "#FFFFFF";
        public string content_bg_color = "#DBDBDB1A";
        public string logo_color = "#ffffff";

        public ThemeData_Group() { }

        public ThemeData_Group(string solution, string bg_color, string text_color, string content_bg_color, string logo_color)
        {
            this.solution = solution;
            this.title_bg_color = bg_color;
            this.title_text_color = text_color;
            this.content_bg_color = content_bg_color;
            this.logo_color = logo_color;
        }
    }

    [Serializable]
    /// <summary>
    /// 节点主题结构
    /// </summary>
    public class ThemeData_Node
    {
        public string solution = "默认";
        public string nodecolor = "#747474";

        public ThemeData_Node() { }

        public ThemeData_Node(string solution, string nodecolor)
        {
            this.solution = solution;
            this.nodecolor = nodecolor;
        }
    }

    [Serializable]
    /// <summary>
    /// 主题列表
    /// </summary>
    public class ThemesList
    {
        /// <summary>
        /// ThemeData_Group 颜色集
        /// </summary>
        public List<ThemeData_Group> Group = new List<ThemeData_Group>();
        /// <summary>
        /// ThemeData_Node 颜色集
        /// </summary>
        public List<ThemeData_Node> Node = new List<ThemeData_Node>();
    }
    #endregion

    [Serializable]
    /// <summary>
    /// 克隆节点时的参数引用传递类
    /// </summary>
    public class DuplicateNodeData
    {
        public string SourceNodeGuid;
        public object DuplicatedNode;
    }

    [CreateAssetMenu(fileName = "ActionTree", menuName = "XGraph/ActionGraphAsset")]
    public class ActionNode_Asset : ScriptableObject
    {
        /// <summary>
        /// 记录的节点编辑器最后一次的窗口尺寸
        /// </summary>
        [SerializeField] public Vector2Int LastGraphWindowSize;
        /// <summary>
        /// 记录的节点编辑器最后一次的视图内位置
        /// </summary>
        [SerializeField] public Vector2 LastGraphViewPosition = Vector2.zero;
        /// <summary>
        /// 记录的节点编辑器最后一次的视图内缩放
        /// </summary>
        [SerializeField] public float LastGraphViewZoom = 1;
        /// <summary>
        /// 最后一次保存时间
        /// </summary>
        [SerializeField] public string LastSaveDateTime = DateTime.Now.ToString("yyyy-MM-dd  -  HH:mm:ss");
        /// <summary>
        /// 节点编辑器的背景参数
        /// </summary>
        [SerializeField] public GraphviewGridBackgroundThemes GraphviewGridBackgroundThemes;
        /// <summary>
        /// 节点编辑器的选择框主题参数
        /// </summary>
        [SerializeField] public GraphviewRectangleSelectorThemes GraphviewRectangleSelectorThemes;
        /// <summary>
        /// 数据节点列表
        /// </summary>
        [SerializeReference] public List<ActionNode_Base> Actions = new List<ActionNode_Base>();
        /// <summary>
        /// 变量节点列表
        /// </summary>
        [SerializeField] public List<ActionVariableData> Variables = new List<ActionVariableData>();
        /// <summary>
        /// 便签列表
        /// </summary>
        [SerializeField] public List<ActionStickData> Sticks = new List<ActionStickData>();
        /// <summary>
        /// 标签列表
        /// </summary>
        [SerializeField] public List<ActionLabelData> Labels = new List<ActionLabelData>();
        /// <summary>
        /// 贴纸列表
        /// </summary>
        [SerializeField] public List<ActionDecalData> Decals = new List<ActionDecalData>();
        /// <summary>
        /// 编组列表
        /// </summary>
        [SerializeField] public List<ActionGroupData> Groups = new List<ActionGroupData>();
        /// <summary>
        /// 黑板变量列表
        /// </summary>
        [SerializeReference] public List<Variable> VariableCategory = new List<Variable>();

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {

        }

        #region 资源操作
        /// <summary>
        /// 创建数据节点到列表中
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ActionNode_Base Create(NodeCreateArgs_Action args)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Added ActionTree Asset");

            // 解析得到行为基础类
            string asm = typeof(ActionNode_Base).Assembly.FullName;
            // 拼接字符串得到行为类
            Type type = Type.GetType($"{args.prefixNamespace}.{args.prefixClass}{args.actionNodeType},{asm}", true);

            // 创建行为资源
            ActionNode_Base actionData = ScriptableObject.CreateInstance(type) as ActionNode_Base;
            actionData.name = args.visualName;
            actionData.guid = GUID.Generate().ToString();
            actionData.actionNodeType = args.actionNodeType;
            actionData.icon = args.iconName;
            actionData.NodeIcon = args.nodeIcon;
            actionData.visualNodeType = args.visualNodeType;
            actionData.identifyName = args.visualName;
            actionData.namespaces = args.prefixNamespace;
            actionData.classes = args.prefixClass;
            actionData.HasAvatar = args.hasAvatar;
            actionData.Avatar = args.avatar;
            actionData.themeSolution = args.themeSolution;
            actionData.themeColor = args.themeColor;
            actionData.TransparentNode = args.transparentNode;
            actionData.content = args.content;
            actionData.nodeGraphSize = args.size;

            // 为变量类型节点数据特化处理，需要初始化类型 Variable
            if (actionData is ActionNode_Variable avnode)
            {
                avnode.variable = avnode.Initialized(args.visualName, args.variable.type);
                if (args.variable != null)
                {
                    switch (args.variable.type)
                    {
                        case VariableType.String:
                            if (args.variable is Variable_String v_string)
                                avnode.variable.SetValue(v_string.GetValue<string>());
                            break;
                        case VariableType.Float:
                            if (args.variable is Variable_Float v_float)
                                avnode.variable.SetValue(v_float.GetValue<float>());
                            break;
                        case VariableType.Int:
                            if (args.variable is Variable_Int v_int)
                                avnode.variable.SetValue(v_int.GetValue<int>());
                            break;
                        case VariableType.Bool:
                            if (args.variable is Variable_Bool v_bool)
                                avnode.variable.SetValue(v_bool.GetValue<bool>());
                            break;
                        case VariableType.Vector2:
                            if (args.variable is Variable_Vector2 v_Vector2)
                                avnode.variable.SetValue(v_Vector2.GetValue<Vector2>());
                            break;
                        case VariableType.Vector3:
                            if (args.variable is Variable_Vector3 v_Vector3)
                                avnode.variable.SetValue(v_Vector3.GetValue<Vector3>());
                            break;
                        case VariableType.Vector4:
                            if (args.variable is Variable_Vector4 v_Vector4)
                                avnode.variable.SetValue(v_Vector4.GetValue<Vector4>());
                            break;
                        case VariableType.Color:
                            if (args.variable is Variable_Color v_Color)
                                avnode.variable.SetValue(v_Color.GetValue<Color>());
                            break;
                    }
                }
            }

            // 添加到列表中
            Actions.Add(actionData);

            // 添加到资源文件下
            AssetDatabase.AddObjectToAsset(actionData, this);
            //AssetDatabase.SaveAssets();

            // 创建后获取该行为树节点相对行为树资源根节点的路径
            string re_path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Actions[^1]));
            string opt_path = re_path.Replace("Temp", $"{this.name}");
            string combine_path = $"{opt_path}   >   {Actions[^1].name}.asset";
            Actions[^1].path = combine_path;
#endif
            return actionData;
        }
        /// <summary>
        /// 从列表中移除一个数据节点
        /// </summary>
        /// <param name="node"></param>
        public void Remove(ActionNode_Base node)
        {
            if (node == null) return;

#if UNITY_EDITOR
            Undo.RecordObject(this, "Removed ChildAction");
            Actions.Remove(node);
            Undo.DestroyObjectImmediate(node);
            //AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        /// <summary>
        /// 从列表中清空所有数据节点
        /// </summary>
        public void Clear()
        {
#if UNITY_EDITOR
            // 移除子级的所有资源
            foreach (var node in Actions)
            {
                AssetDatabase.RemoveObjectFromAsset(node);
                DestroyImmediate(node, true);
            }

            // 清空资源列表
            Actions.Clear();
            // 清空便签列表
            StickNote_Clear();
            // 清空贴图列表
            Decal_Clear();
            // 清空变量列表
            Variable_Clear();
            // 清空编组列表
            NodeGroup_Clear();

            // 刷新资源状态
            EditorUtility.SetDirty(this);
            //AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        /// <summary>
        /// 使用目标资源替换当前资源
        /// </summary>
        /// <param name="root"></param>
        public void Replace(ActionNode_Asset root)
        {
            if (root == null) return;

#if UNITY_EDITOR
            // 清空当前原始资源的所有子节点
            Clear();

            // 更新所有使用到的变量值（当前资源 - 更新）
            this.Variables_Refresh();
            AssetDatabase.SaveAssetIfDirty(this);

            // 更新所有使用到的变量值（目标资源 - 更新）
            root.Variables_Refresh();
            AssetDatabase.SaveAssetIfDirty(root);

            LastSaveDateTime = DateTime.Now.ToString("yyyy-MM-dd  -  HH:mm:ss");

            GraphviewGridBackgroundThemes = root.GraphviewGridBackgroundThemes.Clone();

            GraphviewRectangleSelectorThemes = root.GraphviewRectangleSelectorThemes.Clone();

            // 覆盖原有的 Decals 数据列表
            Decals = new List<ActionDecalData>();
            foreach (var decal in root.Decals)
            {
                Decals.Add(decal.Clone(false));
            }

            // 覆盖原有的 Sticks 数据列表
            Sticks = new List<ActionStickData>();
            foreach (var stick in root.Sticks)
            {
                Sticks.Add(stick.Clone(false));
            }

            // 覆盖原有的 Labels  数据列表
            Labels = new List<ActionLabelData>();
            foreach (var label in root.Labels)
            {
                Labels.Add(label.Clone(false));
            }

            // 覆盖原有的 Groups 数据列表
            Groups = new List<ActionGroupData>();
            foreach (var group in root.Groups)
            {
                Groups.Add(group.Clone(false));
            }

            // 覆盖原有的 Variables 数据列表
            Variables = new List<ActionVariableData>();
            foreach (var vare in root.Variables)
            {
                Variables.Add(vare.Clone(false));
            }

            // 覆盖原有的 VariableCategory 数据列表
            VariableCategory = new List<Variable>();
            foreach (var vare in root.VariableCategory)
            {
                VariableCategory.Add(vare.Clone(false));
            }

            // 创建新节点副本并添加到原始资源中
            Dictionary<ActionNode_Base, ActionNode_Base> dictionary = new Dictionary<ActionNode_Base, ActionNode_Base>();
            foreach (var sourceNode in root.Actions)
            {
                var newNode = Instantiate(sourceNode);
                newNode.name = sourceNode.name;
                newNode.hideFlags = HideFlags.None;
                Actions.Add(newNode);
                AssetDatabase.AddObjectToAsset(newNode, this);
                dictionary[sourceNode] = newNode;
            }

            // 重建父子引用关系
            foreach (var source in root.Actions)
            {
                if (source is ActionNode_Start s && s.childNode != null)
                {
                    (dictionary[source] as ActionNode_Start).childNode = dictionary[s.childNode];
                }

                if (source is ActionNode_Debug d && d.childNode != null)
                {
                    (dictionary[source] as ActionNode_Debug).childNode = dictionary[d.childNode];
                }

                if (source is ActionNode_Wait w && w.childNodes != null)
                {
                    //(dictionary[source] as ActionNode_Wait).childNode = dictionary[w.childNode];
                    var newComposite = dictionary[source] as ActionNode_Wait;
                    newComposite.childNodes.Clear();
                    foreach (var node in w.childNodes)
                    {
                        newComposite.childNodes.Add(dictionary[node]);
                    }
                }

                if (source is ActionNode_Composite c && c.childNodes != null)
                {
                    var newComposite = dictionary[source] as ActionNode_Composite;
                    newComposite.childNodes.Clear();
                    foreach (var node in c.childNodes)
                    {
                        newComposite.childNodes.Add(dictionary[node]);
                    }
                }
            }


#endif
        }
        /// <summary>
        /// 创建当前流程设计的克隆体（仅编辑器下）
        /// </summary>
        /// <returns></returns>
        public ActionNode_Asset Clone(string clonepath = "")
        {
            // 创建新的 ActionNode_Asset
            ActionNode_Asset newRoot = ScriptableObject.CreateInstance<ActionNode_Asset>();

            newRoot.LastSaveDateTime = LastSaveDateTime;

            newRoot.GraphviewGridBackgroundThemes = GraphviewGridBackgroundThemes.Clone();

            newRoot.GraphviewRectangleSelectorThemes = GraphviewRectangleSelectorThemes.Clone();

            // 实例化新的 Sticks 列表，并从原始资源复制项
            newRoot.Sticks = new List<ActionStickData>();
            foreach (var item in Sticks)
            {
                newRoot.Sticks.Add(item.Clone(false));
            }

            // 实例化新的 Labels 列表，并从原始资源复制项
            newRoot.Labels = new List<ActionLabelData>();
            foreach (var item in Labels)
            {
                newRoot.Labels.Add(item.Clone(false));
            }

            // 实例化新的 Decals 列表，并从原始资源复制项
            newRoot.Decals = new List<ActionDecalData>();
            foreach (var item in Decals)
            {
                newRoot.Decals.Add(item.Clone(false));
            }

            // 实例化新的 Variables 列表，并从原始资源复制项
            newRoot.Variables = new List<ActionVariableData>();
            foreach (var item in Variables)
            {
                newRoot.Variables.Add(item.Clone(false));
            }

            // 实例化新的 Groups 列表，并从原始资源复制项
            newRoot.Groups = new List<ActionGroupData>();
            foreach (var item in Groups)
            {
#if UNITY_EDITOR
                newRoot.Groups.Add(item.Clone(false));
#endif
            }

            // 实例化新的 VariableCategory 列表，并从原始资源复制项
            newRoot.VariableCategory = new List<Variable>();
            foreach (var bbv in VariableCategory)
            {
#if UNITY_EDITOR
                newRoot.VariableCategory.Add(bbv.Clone(false));
#endif
            }

            newRoot.name = this.name + "_CloneRoot";
            newRoot.LastGraphWindowSize = this.LastGraphWindowSize;
            newRoot.LastGraphViewPosition = this.LastGraphViewPosition;
            newRoot.LastGraphViewZoom = this.LastGraphViewZoom;

            // 用于映射原始节点到新节点
            Dictionary<ActionNode_Base, ActionNode_Base> originalRootDic = new Dictionary<ActionNode_Base, ActionNode_Base>();

            // 第一步：复制所有节点（不处理父子关系）
            foreach (var node in this.Actions)
            {
                ActionNode_Base newTreeNode = Object.Instantiate(node);
                newTreeNode.name = node.name;
                newTreeNode.hideFlags = HideFlags.None;

                // 关键修复点：初始化时清空所有子引用
                if (newTreeNode is ActionNode_Start newStart)
                    newStart.childNode = null;
                else if (newTreeNode is ActionNode_Wait newWait)
                    newWait.childNodes.Clear();
                else if (newTreeNode is ActionNode_Debug newDebug)
                    newDebug.childNode = null;
                else if (newTreeNode is ActionNode_Composite newComp)
                    newComp.childNodes.Clear();

                newRoot.Actions.Add(newTreeNode);
                originalRootDic[node] = newTreeNode;
            }

            // 第二步：重建父子关系
            foreach (var node in this.Actions)
            {
                ActionNode_Base newParentNode = originalRootDic[node];

                // 处理 ActionNode_Start
                if (node is ActionNode_Start originalStart)
                {
                    var newStart = newParentNode as ActionNode_Start;
                    if (originalStart.childNode != null && originalRootDic.TryGetValue(originalStart.childNode, out var newNode))
                    {
                        newStart.childNode = newNode;
                    }
                }

                // 处理 ActionNode_Wait
                else if (node is ActionNode_Wait originalWait)
                {
                    var newWait = newParentNode as ActionNode_Wait;
                    foreach (var originalChild in originalWait.childNodes)
                    {
                        if (originalRootDic.TryGetValue(originalChild, out var newChild))
                        {
                            newWait.childNodes.Add(newChild);
                        }
                    }
                }

                // 处理 ActionNode_Debug
                else if (node is ActionNode_Debug originalDebug)
                {
                    var newDebug = newParentNode as ActionNode_Debug;
                    if (originalDebug.childNode != null && originalRootDic.TryGetValue(originalDebug.childNode, out var newNode))
                    {
                        newDebug.childNode = newNode;
                    }
                }

                // 处理 ActionNode_Composite
                else if (node is ActionNode_Composite originalComposite)
                {
                    var newComposite = newParentNode as ActionNode_Composite;
                    foreach (var originalChild in originalComposite.childNodes)
                    {
                        if (originalRootDic.TryGetValue(originalChild, out var newChild))
                        {
                            newComposite.childNodes.Add(newChild);
                        }
                    }
                }
            }
            SaveNodeRootAsset(newRoot, string.IsNullOrEmpty(clonepath) ? $"{util_Dashboard.GetPath_Temp()}/CloneTree.asset" : clonepath);

            // 更新变量赋值数据
            newRoot.Variables_Refresh();

            AssetDatabase.SaveAssetIfDirty(newRoot);

            return newRoot;
        }
        /// <summary>
        /// 保存为Tree资源到目标路径下
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        public void SaveNodeRootAsset(ActionNode_Asset root, string path)
        {
#if UNITY_EDITOR
            // 提取根路径整理 - 去掉尾部的 /
            string path_root = util_Dashboard.GetPath_Root();
            path_root = path_root.Substring(0, path_root.Length - 1);

            // 目标路径整理 - 去掉尾部的 /
            string path_target = $"{util_Dashboard.GetPath_Temp()}";
            path_target = path_target.Substring(0, path_target.Length - 1);

            // 判断是否存在目标路径，如果不存在就创建该路径的文件夹
            if (!AssetDatabase.AssetPathExists(path_target))
            {
                AssetDatabase.CreateFolder(path_root, "Temp");
                //AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            // 保存为临时.asset 文件，供Unity资源系统进行操作
            AssetDatabase.CreateAsset(root, path);
            foreach (var treenode in root.Actions)
            {
                AssetDatabase.AddObjectToAsset(treenode, root);
            }

            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 获取指定数据节点的子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<ActionNode_Base> GetChildrenNodes(ActionNode_Base parent)
        {
            List<ActionNode_Base> nodes = new List<ActionNode_Base>();

            // 如果是 "ActionNode_Start" 节点，那么就收集 "ActionNode_Start" 节点下的 "child"
            ActionNode_Start start = parent as ActionNode_Start;
            if (start != null && start.childNode != null)
            {
                nodes.Add(start.childNode);
            }

            // 如果是 "ActionNode_Wait" 节点，那么就收集 "ActionNode_Wait" 节点下的 "child"
            ActionNode_Wait wait = parent as ActionNode_Wait;
            if (wait != null && wait.childNodes != null)
            {
                nodes = wait.childNodes;
            }

            // 如果是 "ActionNode_Debug" 节点，那么就收集 "ActionNode_Debug" 节点下的 "child"
            ActionNode_Debug debug = parent as ActionNode_Debug;
            if (debug != null && debug.childNode != null)
            {
                nodes.Add(debug.childNode);
            }

            // 如果是 "ActionNode_Composite" 节点，那么就收集 "ActionNode_Composite" 节点下的 "childNodes"
            ActionNode_Composite comp = parent as ActionNode_Composite;
            if (comp != null && comp.childNodes != null)
            {
                nodes = comp.childNodes;
            }

            // 返回的列表就是 "GraphView组件" 那边需要根据这子资源列表才能知道跟哪些子节点重建节点之间的连线
            return nodes;
        }
        /// <summary>
        /// 为资源指定子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void ChildNode_Add(ActionNode_Base parent, ActionNode_Base child)
        {
            //Debug.Log($"{parent.identifyName}       |  建立链接  √  |      {child.identifyName}");

            #region 特化处理 - Start
            ActionNode_Start start = parent as ActionNode_Start;
            if (start)
            {
#if UNITY_EDITOR
                Undo.RecordObject(start, "Connect_StartNode");
#endif
                if (start.childNode != null)
                {
                    if (child.guid == start.childNode.guid)
                    {
                        Debug.Log("start节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                        return;
                    }
                }
                start.childNode = child;
            }
            #endregion

            #region 特化处理 - Wait
            ActionNode_Wait wait = parent as ActionNode_Wait;
            if (wait)
            {
#if UNITY_EDITOR
                Undo.RecordObject(wait, "Connect_WaitNode");
#endif
                bool existChild = false;
                wait.childNodes.ForEach(c =>
                {
                    if (child.guid == c.guid)
                        existChild = true;
                });
                if (existChild)
                {
                    Debug.Log("wait 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                wait.childNodes.Add(child);
            }
            #endregion

            #region 特化处理 - Debug
            ActionNode_Debug debug = parent as ActionNode_Debug;
            if (debug)
            {
#if UNITY_EDITOR
                Undo.RecordObject(debug, "Connect_DebugNode");
#endif
                if (debug.childNode != null)
                {
                    if (child.guid == debug.childNode.guid)
                    {
                        Debug.Log("debug节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                        return;
                    }
                }
                debug.childNode = child;
            }
            #endregion

            #region 特化处理 - Composite
            ActionNode_Composite comp = parent as ActionNode_Composite;
            if (comp)
            {
#if UNITY_EDITOR
                Undo.RecordObject(comp, "Connect_CompositeNode");
#endif
                bool existChild = false;
                comp.childNodes.ForEach(c =>
                {
                    if (child.guid == c.guid)
                        existChild = true;
                });
                if (existChild)
                {
                    Debug.Log("comp 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                comp.childNodes.Add(child);
            }
            #endregion

            #region 特化处理 - Relay
            ActionNode_Relay relay = parent as ActionNode_Relay;
            if (relay)
            {
#if UNITY_EDITOR
                Undo.RecordObject(relay, "Connect_RelayNode");
#endif
                bool existChild = false;
                relay.childNodes.ForEach(c =>
                {
                    if (child.guid == c.guid)
                        existChild = true;
                });
                if (existChild)
                {
                    //Debug.Log("comp 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                relay.childNodes.Add(child);
            }
            #endregion
        }
        /// <summary>
        /// 从指定的父资源中移除子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void ChildNode_Remove(ActionNode_Base parent, ActionNode_Base child)
        {
            //Debug.Log($"{parent.identifyName}       |  断开链接  ×  |      {c.identifyName}");

            #region 特化处理 - Start
            ActionNode_Start start = parent as ActionNode_Start;
            if (start)
            {
#if UNITY_EDITOR
                Undo.RecordObject(start, "RemoveConnect_StartNode");
#endif
                start.childNode = null;
            }
            #endregion

            #region 特化处理 - Wait
            ActionNode_Wait wait = parent as ActionNode_Wait;
            if (wait)
            {
#if UNITY_EDITOR
                Undo.RecordObject(wait, "RemoveConnect_WaitNode");
#endif
                wait.childNodes.Remove(child);
            }
            #endregion

            #region 特化处理 - Debug
            ActionNode_Debug debug = parent as ActionNode_Debug;
            if (debug)
            {
#if UNITY_EDITOR
                Undo.RecordObject(debug, "RemoveConnect_DebugNode");
#endif
                debug.childNode = null;
            }
            #endregion

            #region 特化处理 - Composite
            ActionNode_Composite comp = parent as ActionNode_Composite;
            if (comp)
            {
#if UNITY_EDITOR
                Undo.RecordObject(comp, "RemoveConnect_CompositeNode");
#endif
                comp.childNodes.Remove(child);
            }
            #endregion

        }
        /// <summary>
        /// 寻找匹配guid的行为数据节点
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionNode_Base FindActionNode(string guid)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i].guid == guid)
                {
                    return Actions[i];
                }
            }

            return null;
        }
        #endregion

        #region 便签操作
        /// <summary>
        /// 添加便签数据
        /// </summary>
        /// <param name="data"></param>
        public void StickNote_Add(ActionStickData data)
        {
            Sticks.Add(data);
        }
        /// <summary>
        /// 清空便签数据列表
        /// </summary>
        public void StickNote_Clear()
        {
            Sticks.Clear();
#if UNITY_EDITOR
            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标便签数据
        /// </summary>
        /// <param name="data"></param>
        public void StickNote_Remove(ActionStickData data)
        {
            Sticks.Remove(data);
        }
        #endregion

        #region 标签操作
        /// <summary>
        /// 添加标签数据
        /// </summary>
        /// <param name="data"></param>
        public void Label_Add(ActionLabelData data)
        {
            Labels.Add(data);
        }
        /// <summary>
        /// 清空标签数据列表
        /// </summary>
        public void Label_Clear()
        {
            Labels.Clear();
#if UNITY_EDITOR
            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标标签数据
        /// </summary>
        /// <param name="data"></param>
        public void Label_Remove(ActionLabelData data)
        {
            Labels.Remove(data);
        }
        #endregion

        #region 贴图操作
        /// <summary>
        /// 添加贴图数据
        /// </summary>
        /// <param name="data"></param>
        public void Decal_Add(ActionDecalData data)
        {
            Decals.Add(data);
        }
        /// <summary>
        /// 清空贴图数据列表
        /// </summary>
        public void Decal_Clear()
        {
            Decals.Clear();
#if UNITY_EDITOR
            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标贴图数据
        /// </summary>
        /// <param name="data"></param>
        public void Decal_Remove(ActionDecalData data)
        {
            Decals.Remove(data);
        }
        #endregion

        #region 变量操作
        /// <summary>
        /// 添加变量数据
        /// </summary>
        /// <param name="data"></param>
        public void Variable_Add(ActionVariableData data)
        {
            Variables.Add(data);
        }
        /// <summary>
        /// 清空变量数据列表
        /// </summary>
        public void Variable_Clear()
        {
            Variables.Clear();
#if UNITY_EDITOR
            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标变量数据
        /// </summary>
        /// <param name="data"></param>
        public void Variable_Remove(ActionVariableData data)
        {
            Variables.Remove(data);
        }
        /// <summary>
        /// 获取目标变量源头数据
        /// </summary>
        /// <param name="data"></param>
        public Variable Variable_GetVarSource(string varguid)
        {
            Variable vare = null;
            VariableCategory.ForEach((n) =>
            {
                if (varguid == n.guid)
                {
                    vare = n;
                }
            });

            return vare;
        }
        /// <summary>
        /// 设置变量值
        /// </summary>
        public void Variable_SetValue<T>(string name, T value)
        {
            foreach (var item in VariableCategory)
            {
                if (name == item.name)
                {
                    item.SetValue<T>(value);
                }
            }

            // 更新变量赋值数据
            Variables_Refresh();
        }
        /// <summary>
        /// 根据VraiableCategory的变量列表源来更新在Actions列表 & Variables列表中所有用到这些变量的值
        /// </summary>
        public void Variables_Refresh()
        {
            // 更新 “黑板变量数据列表”的变量信息
            foreach (var vare in Variables)
            {
                foreach (var variable in VariableCategory)
                {
                    // 匹配黑板变量
                    if (vare.varguid == variable.guid)
                    {
                        vare.variable.name = variable.name;
                        vare.description = variable.description;
                        vare.variable.description = variable.description;
                        switch (vare.variable.type)
                        {
                            case VariableType.String:
                                vare.variable.SetValue<string>(variable.GetValue<string>());
                                break;
                            case VariableType.Float:
                                vare.variable.SetValue<float>(variable.GetValue<float>());
                                break;
                            case VariableType.Int:
                                vare.variable.SetValue<int>(variable.GetValue<int>());
                                break;
                            case VariableType.Bool:
                                vare.variable.SetValue<bool>(variable.GetValue<bool>());
                                break;
                            case VariableType.Vector2:
                                vare.variable.SetValue<Vector2>(variable.GetValue<Vector2>());
                                break;
                            case VariableType.Vector3:
                                vare.variable.SetValue<Vector3>(variable.GetValue<Vector3>());
                                break;
                            case VariableType.Vector4:
                                vare.variable.SetValue<Vector4>(variable.GetValue<Vector4>());
                                break;
                            case VariableType.Color:
                                vare.variable.SetValue<Color>(variable.GetValue<Color>());
                                break;
                        }
                    }
                }
            }

            // 更新行为节点中的 “黑板变量数据列表” 的变量信息
            foreach (var action in Actions)
            {
                foreach (var data in action.VariableDatas)
                {
                    foreach (var variable in VariableCategory)
                    {
                        // 匹配黑板变量
                        if (data.variable.guid == variable.guid)
                        {
                            data.variable.name = variable.name;
                            data.variable.description = variable.description;
                            switch (data.variable.type)
                            {
                                case VariableType.String:
                                    data.variable.SetValue<string>(variable.GetValue<string>());
                                    break;
                                case VariableType.Float:
                                    data.variable.SetValue<float>(variable.GetValue<float>());
                                    break;
                                case VariableType.Int:
                                    data.variable.SetValue<int>(variable.GetValue<int>());
                                    break;
                                case VariableType.Bool:
                                    data.variable.SetValue<bool>(variable.GetValue<bool>());
                                    break;
                                case VariableType.Vector2:
                                    data.variable.SetValue<Vector2>(variable.GetValue<Vector2>());
                                    break;
                                case VariableType.Vector3:
                                    data.variable.SetValue<Vector3>(variable.GetValue<Vector3>());
                                    break;
                                case VariableType.Vector4:
                                    data.variable.SetValue<Vector4>(variable.GetValue<Vector4>());
                                    break;
                                case VariableType.Color:
                                    data.variable.SetValue<Color>(variable.GetValue<Color>());
                                    break;
                            }

                            if (action is ActionNode_Variable vare)
                            {
                                string originalGUID = vare.variable.guid;
                                vare.variable = data.variable.Clone(false);
                                vare.variable.guid = originalGUID;
                                vare.variable.name = action.identifyName;
                            }
                        }
                    }
                }
            }

            // 更新行为节点中的 “内部变量数据列表” 的变量信息
            foreach (var action in Actions)
            {
                // 刷新内部变量数据数值
                foreach (var data in action.InternalVariableDatas)
                {
                    ActionNode_Variable internalVar = FindActionNode(data.VariableNodeGuid) as ActionNode_Variable;
                    internalVar.variable.name = internalVar.identifyName;
                    data.variable.name = internalVar.identifyName;
                    switch (data.variable.type)
                    {
                        case VariableType.String:
                            string val_string = internalVar.variable.GetValue<string>();
                            data.variable.SetValue<string>(val_string);
                            break;
                        case VariableType.Float:
                            data.variable.SetValue<float>(internalVar.variable.GetValue<float>());
                            break;
                        case VariableType.Int:
                            data.variable.SetValue<int>(internalVar.variable.GetValue<int>());
                            break;
                        case VariableType.Bool:
                            data.variable.SetValue<bool>(internalVar.variable.GetValue<bool>());
                            break;
                        case VariableType.Vector2:
                            data.variable.SetValue<Vector2>(internalVar.variable.GetValue<Vector2>());
                            break;
                        case VariableType.Vector3:
                            data.variable.SetValue<Vector3>(internalVar.variable.GetValue<Vector3>());
                            break;
                        case VariableType.Vector4:
                            data.variable.SetValue<Vector4>(internalVar.variable.GetValue<Vector4>());
                            break;
                        case VariableType.Color:
                            data.variable.SetValue<Color>(internalVar.variable.GetValue<Color>());
                            break;
                    }
                }
            }
        }
        #endregion

        #region 编组操作
        /// <summary>
        /// 添加编组数据
        /// </summary>
        /// <param name="data"></param>
        public void NodeGroup_Add(ActionGroupData data)
        {
            Groups.Add(data);
        }
        /// <summary>
        /// 清空编组数据列表
        /// </summary>
        public void NodeGroup_Clear()
        {
            VariableCategory.Clear();
#if UNITY_EDITOR
            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标编组数据
        /// </summary>
        /// <param name="data"></param>
        public void NodeGroup_Remove(ActionGroupData data)
        {
            Groups.Remove(data);
        }
        #endregion
    }
}