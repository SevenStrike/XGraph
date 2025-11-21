namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
#if UNITY_EDITOR
    using UnityEditor.Experimental.GraphView;
#endif
    using UnityEngine;
    using static UnityEngine.Rendering.DebugUI;

    public abstract class xAction_Base : ScriptableObject
    {
        #region 节点参数
        /// <summary>
        /// 行为节点 - 名称
        /// </summary>
        [SerializeField] public string identifyName;
        /// <summary>
        /// 行为节点 - 内容表述
        /// </summary>
        [SerializeField] public string content;
        /// <summary>
        /// 行为节点 - guid_self
        /// </summary>
        [SerializeField] public string guid;
        /// <summary>
        /// 行为节点 - guid_self
        /// </summary>
        [SerializeField] public string namespaces;
        /// <summary>
        /// 行为节点 - guid_self
        /// </summary>
        [SerializeField] public string classes;
        /// <summary>
        /// 行为节点 - 相对路径
        /// </summary>
        [SerializeField] public string path;
        /// <summary>
        /// 行为节点 - 类型
        /// </summary>
        [SerializeField] public string actionNodeType;
        /// <summary>
        /// 行为节点 - 图标
        /// </summary>
        [SerializeField] public string icon;
        /// <summary>
        /// 行为节点 - 类型
        /// </summary>
        [SerializeField] public string visualNodeType = "None";
        /// <summary>
        /// 行为节点 - 在GraphView里的位置记录
        /// </summary>
        [SerializeField] public Vector2 nodeGraphPosition;
        /// <summary>
        /// 行为节点 - 在GraphView里的尺寸记录
        /// </summary>
        [SerializeField] public Vector2 nodeGraphSize;
        /// <summary>
        /// 行为节点 - 在GraphView里的颜色标记方案名称
        /// </summary>
        [SerializeField] public string themeSolution = "M 默认";
        /// <summary>
        /// 行为节点 - 在GraphView里的颜色标记
        /// </summary>
        [SerializeField] public Color themeColor = Color.clear;
        /// <summary>
        /// 并发执行开关
        /// </summary>
        [SerializeField] public bool isConcurrentExecution = false;
        /// <summary>
        /// 设定头像状态
        /// </summary>
        [SerializeField] public bool HasAvatar = false;
        /// <summary>
        /// 透明背景节点模式
        /// </summary>
        [SerializeField] public bool TransparentNode = false;
        /// <summary>
        /// 头像图像
        /// </summary>
        [SerializeField] public Texture2D Avatar;
        /// <summary>
        /// 标题图标
        /// </summary>
        [SerializeField] public Texture2D NodeIcon;
        /// <summary>
        /// 行为根节点
        /// </summary>
        [SerializeField] public xAction_Asset RootAsset;
        /// <summary>
        /// 起始节点
        /// </summary>
        [SerializeField] public bool isStartNode;
        #endregion

        #region 节点数据接驳器
        /// <summary>
        /// 变量Guids接驳列表
        /// </summary>
        [SerializeField] public List<Binder_Varialble> VariableDatas = new List<Binder_Varialble>();
        /// <summary>
        /// 内部变量Guids接驳列表
        /// </summary>
        [SerializeField] public List<Binder_Varialble> InternalVariableDatas = new List<Binder_Varialble>();
        #endregion

        #region 回调
        /// <summary>
        /// 当节点执行时的委托事件
        /// </summary>
        public Action On_Node_Excute;
        /// <summary>
        /// 当移动节点位置时的委托事件
        /// </summary>
        public Action<Vector2> On_Node_Moved;
        /// <summary>
        /// 当改变节点尺寸时的委托事件
        /// </summary>
        public Action<Vector2> On_Node_SizeChanged;
        /// <summary>
        /// 当改变节点图标时的委托事件
        /// </summary>
        public Action<Texture2D> On_Node_IconChanged;
        /// <summary>
        /// 当改变节点头像时的委托事件
        /// </summary>
        public Action<Texture2D> On_Node_AvatarChanged;
        /// <summary>
        /// 当改变节点主题色时的委托事件
        /// </summary>
        public Action On_Node_ThemeColorChanged;
        /// <summary>
        /// 当改变节点标题名称时的委托事件
        /// </summary>
        public Action<string> On_Node_TitleChanged;
        /// <summary>
        /// 当改变节点执行模式的委托事件
        /// </summary>
        public Action<bool> On_Node_ConcurrentChanged;
        /// <summary>
        /// 当改变节点通透样式的委托事件
        /// </summary>
        public Action<bool> On_Node_TransparentChanged;
        /// <summary>
        /// 当节点绑定变量的时候的委托事件
        /// </summary>
        public Action<Variable> On_Node_Variable_Binded;
        /// <summary>
        /// 当节点解除绑定变量的时候的委托事件
        /// </summary>
        public Action On_Node_Variable_Unbinded;
        /// <summary>
        /// 当节点绑定属性的时候的委托事件
        /// </summary>
        public Action<xAction_Property> On_Node_Property_Binded;
        /// <summary>
        /// 当节点解除绑定属性的时候的委托事件
        /// </summary>
        public Action On_Node_Property_Unbinded;
        /// <summary>
        /// 当节点被克隆的时候的委托事件
        /// </summary>
        public Action<xAction_Base, xAction_Base> On_Node_Duplicated;
        /// <summary>
        /// 当节点重建的时候的委托事件
        /// </summary>
        public Action On_Node_Restructure;
        /// <summary>
        /// 当节点为内部变量且内部变量值改变时
        /// </summary>
        public Action On_InternalVariableValue_Changed;
        /// <summary>
        /// 当指定起始节点时
        /// </summary>
        public Action<bool> On_Node_IsStartNodeChanged;
#if UNITY_EDITOR
        /// <summary>
        /// 当节点连线的时候的委托事件
        /// </summary>
        public Action<Edge> On_Node_CreateEdge;
        /// <summary>
        /// 当节点移除连线的时候的委托事件
        /// </summary>
        public Action<Edge> On_Node_RemovedEdge;
#endif
        #endregion

        #region 父节点
        [SerializeField] private xAction_Base _parentNode;
        public xAction_Base ParentNode => _parentNode;
        #endregion

        #region 绑定的属性列表
        public List<Binder_Property> binded_propertys = new List<Binder_Property>();
        #endregion

        #region 节点执行方法
        /// <summary>
        /// 行为节点执行方法
        /// </summary>
        /// <returns></returns>
        public abstract void Execute();
        #endregion

        #region 辅助
        /// <summary>
        /// 设置父节点的方法
        /// </summary>
        /// <param name="parent"></param>
        public void SetParentNode(xAction_Base parent)
        {
            _parentNode = parent;
        }
        /// <summary>
        /// 设置行为根节点资源的方法
        /// </summary>
        /// <param name="root"></param>
        public void SetActionAssetRoot(xAction_Asset root)
        {
            RootAsset = root;
        }
        #endregion

        #region 回调
        /// <summary>
        /// 注册运行时变量值改变回调
        /// </summary>
        public void RegisterVariableValueChanged()
        {
            RootAsset.On_VariablesValue_Changed += On_VariablesValue_Changed;
        }
        /// <summary>
        /// 注销运行时变量值改变回调
        /// </summary>
        public void UnregisterVariableValueChanged()
        {
            RootAsset.On_VariablesValue_Changed -= On_VariablesValue_Changed;
        }
        /// <summary>
        /// 行为节点的变量值（如果存在）改变时
        /// </summary>
        public virtual void On_VariablesValue_Changed()
        {

        }
        #endregion

        #region 节点信息获取
        /// <summary>
        /// 获取行为的基础信息数据（行为类型 / 显示名称）
        /// </summary>
        /// <returns></returns>
        public string GetInfo()
        {
            return $"{namespaces}.{classes}{actionNodeType.ToString()}   /   {visualNodeType.ToString()}   /   {identifyName}";
        }
        /// <summary>
        /// 获取行为资源的路径
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            string val = "";
            string[] texts = path.Split(new char[1] { '\\' });
            int index = 0;
            foreach (string t in texts)
            {
                if (index != texts.Length - 1)
                {
                    val += t + "  >  ";
                    index++;
                }
                else
                {
                    val += t;
                    index++;
                }
            }
            return val;
        }
        #endregion

        #region 变量获取
        /// <summary>
        /// 获取变量值（根据预设的端口名称匹配）
        /// </summary>
        /// <param name="portName"></param>
        /// <returns>如果返回为空请检查传入的节点端口名称 "portName" 是否存在，或者 "portName" 端口是否链接了变量节点数据</returns>
        public Variable VariableDatas_Get(string portName)
        {
            Variable vare = null;

            // 先从绑定的黑板变量数据列表中找
            foreach (var data in VariableDatas)
            {
                if (portName == data.TargetPortName)
                {
                    vare = data.variable;
                    break;
                }
            }

            return vare;
        }
        /// <summary>
        /// 从 InternalVariableDatas 列表中遍历获取变量值（根据预设的端口名称匹配）
        /// </summary>
        /// <param name="portName"></param>
        /// <returns>如果返回为空请检查传入的节点端口名称 "portName" 是否存在，或者 "portName" 端口是否链接了变量节点数据</returns>
        public Variable InternalVariableDatas_Get(string portName)
        {
            Variable vare = null;

            foreach (var data in InternalVariableDatas)
            {
                if (portName == data.TargetPortName)
                {
                    vare = data.variable;
                    break;
                }
            }

            return vare;
        }
        /// <summary>
        /// 从 binded_propertys 列表中遍历获取变量值（根据预设的端口名称匹配）
        /// </summary>
        /// <param name="portName"></param>
        /// <returns>如果返回为空请检查传入的节点端口名称 "portName" 是否存在，或者 "portName" 端口是否链接了变量节点数据</returns>
        public Variable PropertysDatas_Get(string portName)
        {
            Variable vare = null;

            foreach (var property in binded_propertys)
            {
                if (portName == property.Action_PortName)
                {
                    // 获取属性节点
                    xAction_Property prop = RootAsset.FindActionNode(property.Property_GUID) as xAction_Property;
                    // 属性节点更新
                    prop.Propertys_Update();
                    // 获取属性节点的指定端口名称的属性变量值
                    vare = prop.Propertys_Get(property.Property_PortName);
                    break;
                }
            }

            return vare;
        }
        #endregion

        #region 变量操作
        /// <summary>
        /// 设置变量值（根据预设的端口名称）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="portName"></param>
        /// <param name="value"></param>
        ///         /// <returns>如果返回为空请检查传入的节点端口名称 "portName" 是否存在，或者 "portName" 端口是否链接了变量节点数据</returns>
        public void PortValue_Set<T>(string portName, T value)
        {
            Variable vare = null;
            string targetNode = null;
            bool exist = false;

            // 先从绑定的黑板变量数据列表中找
            foreach (var item in VariableDatas)
            {
                if (portName == item.TargetPortName)
                {
                    vare = item.variable;
                    targetNode = item.VariableNodeGuid;
                    exist = true;

                    // 如果能找到存在的变量指定记录
                    if (vare != null)
                    {
                        foreach (var cd in RootAsset.BlackboardVariable)
                        {
                            if (cd.guid == vare.guid)
                            {
                                cd.SetValue(value);
                            }
                        }
                    }
                }
            }

            // 再从绑定的内部变量数据列表中找
            if (!exist)
            {
                foreach (var item in InternalVariableDatas)
                {
                    if (portName == item.TargetPortName)
                    {
                        vare = item.variable;
                        targetNode = item.VariableNodeGuid;

                        // 如果能找到存在的变量指定记录
                        if (vare != null)
                        {
                            xAction_Base ac = RootAsset.FindActionNode(targetNode);
                            if (ac != null)
                            {
                                if (ac is xAction_Variable va)
                                {
                                    // 如果已经指定了黑板变量，则修改匹配的黑板变量值
                                    if (va.VariableDatas != null && va.VariableDatas.Count > 0)
                                    {
                                        Binder_Varialble con = va.VariableDatas.First();
                                        foreach (var cd in RootAsset.BlackboardVariable)
                                        {
                                            if (cd.guid == con.variable.guid)
                                            {
                                                cd.SetValue(value);
                                            }
                                        }
                                    }
                                    // 否则直接修改内部变量自身变量值
                                    else
                                    {
                                        va.variable.SetValue(value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 从目标端口的变量值来设置模组参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="portName">端口名称</param>
        /// <param name="setter">设置目标字段的委托</param>
        public void PortValue_Set<T>(string portName, Action<T> setter)
        {
            // 按优先级查找变量：属性变量 → 黑板变量 → 内部变量
            Variable variable = PropertysDatas_Get(portName) ?? VariableDatas_Get(portName) ?? InternalVariableDatas_Get(portName);

            // 如果找到变量则获取数值并设置
            if (variable != null)
            {
                T value = variable.GetValue<T>();
                setter(value);
            }
        }
        /// <summary>
        /// 获取变量项（根据预设的端口名称）
        /// </summary>
        /// <param name="portName"></param>
        /// <returns>如果返回为空请检查传入的节点端口名称 "portName" 是否存在，或者 "portName" 端口是否链接了变量节点数据</returns>
        public string VariableNodeGuid_Get(string portName)
        {
            string vare = null;
            bool exist = false;

            // 先从绑定的黑板变量数据列表中找
            foreach (var item in VariableDatas)
            {
                if (portName == item.TargetPortName)
                {
                    vare = item.VariableNodeGuid;
                    exist = true;
                }
            }

            // 再从绑定的内部变量数据列表中找
            if (!exist)
            {
                foreach (var item in InternalVariableDatas)
                {
                    if (portName == item.TargetPortName)
                    {
                        vare = item.VariableNodeGuid;
                    }
                }
            }

            return vare;
        }
        #endregion

        #region 黑板变量 绑定/解绑
        /// <summary>
        /// 绑定黑板变量数据到行为节点的 ”黑板变量数据列表中“
        /// </summary>
        /// <param name="data"></param>
        /// <param name="portName"></param>
        public void VariableData_Bind(xVariableData data, string portName)
        {
            #region 检查节点的变量链接信息列表中是否存在重复的项
            bool isExistConenctor = false;
            foreach (var item in VariableDatas)
            {
                if (item.VariableNodeGuid == data.guid && item.TargetPortName == portName)
                {
                    isExistConenctor = true;
                }
            }
            #endregion

            // 如果已经存在则忽略，否则记录该变量链接信息
            if (isExistConenctor)
                return;
            else
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Bind Variable Connector");
                VariableDatas.Add(new Binder_Varialble(data.guid, portName, data.variable));
#endif
            }

            if (On_Node_Variable_Binded != null)
                On_Node_Variable_Binded(data.variable);
        }
        /// <summary>
        /// 从行为节点的 ”黑板变量数据列表中“中解绑指定的guid和端口名称的变量数据的绑定
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="portName"></param>
        public void VariableData_Unbind(string guid, string portName)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Unbind Variable Connector");

            // 如果在变量链接信息列表中找到指定的guid和名称的变量链接信息列表项则删除
            VariableDatas.RemoveAll(item => item.VariableNodeGuid == guid && item.TargetPortName == portName);
            if (On_Node_Variable_Unbinded != null)
                On_Node_Variable_Unbinded();
#endif
        }
        #endregion

        #region 内部变量 绑定/解绑
        /// <summary>
        /// 绑定内部变量数据到行为节点的 ”内部变量数据列表中“
        /// </summary>
        /// <param name="data"></param>
        /// <param name="portName"></param>
        public void InternalVariableData_Bind(xAction_Variable data, string portName)
        {
            #region 检查节点的变量链接信息列表中是否存在重复的项
            bool isExistConenctor = false;
            foreach (var item in InternalVariableDatas)
            {
                if (item.VariableNodeGuid == data.guid && item.TargetPortName == portName)
                {
                    isExistConenctor = true;
                }
            }
            #endregion

            // 如果已经存在则忽略，否则记录该变量链接信息
            if (isExistConenctor)
                return;
            else
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Bind VariableInternal Connector");
                Binder_Varialble con = new Binder_Varialble(data.guid, portName, data.variable);
                con.variable.name = data.identifyName;
                InternalVariableDatas.Add(con);
#endif
            }

            if (On_Node_Variable_Binded != null)
                On_Node_Variable_Binded(data.variable);
        }
        /// <summary>
        /// 从行为节点的 ”内部变量数据列表中“中解绑指定的guid和端口名称的变量数据的绑定
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="portName"></param>
        public void InternalVariableData_Unbind(string guid, string portName)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Unbind InternalVariable Connector");

            // 如果在变量链接信息列表中找到指定的guid和名称的变量链接信息列表项则删除
            InternalVariableDatas.RemoveAll(item => item.VariableNodeGuid == guid && item.TargetPortName == portName);

            if (On_Node_Variable_Unbinded != null)
                On_Node_Variable_Unbinded();
#endif
        }
        #endregion

        #region 属性 绑定/解绑
        /// <summary>
        /// 绑定属性数据到行为节点的 ”属性数据列表中“
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="property_port_name"></param>
        public void Property_Bind(xAction_Property prop, string property_port_name, string property_port_type, string action_port_name, string action_port_type)
        {
            #region 检查节点的变量链接信息列表中是否存在重复的项
            bool isExistConenctor = false;
            foreach (var p in binded_propertys)
            {
                if (p.Property_GUID == prop.guid &&
                    p.Property_PortName == property_port_name &&
                    p.Property_PortType == property_port_type &&
                    p.Action_PortName == action_port_name &&
                    p.Action_PortType == action_port_type)
                {
                    isExistConenctor = true;
                }
            }
            #endregion

            // 如果已经存在则忽略，否则记录该变量链接信息
            if (isExistConenctor)
                return;
            else
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Bind Property");
                Binder_Property con = new Binder_Property(
                    prop.identifyName,
                    prop.guid,
                    property_port_name,
                    property_port_type,
                    action_port_name,
                    action_port_type);
                binded_propertys.Add(con);
#endif
            }

            if (On_Node_Property_Binded != null)
                On_Node_Property_Binded(prop);
        }
        /// <summary>
        /// 从行为节点的 ”属性数据列表中“中解绑指定的guid和端口名称的属性数据的绑定
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="property_port_name"></param>
        public void Property_Unbind(string guid, string property_port_name, string property_port_type, string action_port_name, string action_port_type)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Unbind Property");

            // 如果在变量链接信息列表中找到指定的guid和名称的变量链接信息列表项则删除
            binded_propertys.RemoveAll(item =>
            {
                return item.Property_GUID == guid &&
                item.Property_PortName == property_port_name &&
                item.Property_PortType == property_port_type &&
                item.Action_PortName == action_port_name &&
                item.Action_PortType == action_port_type;
            });

            if (On_Node_Property_Unbinded != null)
                On_Node_Property_Unbinded();
#endif
        }
        #endregion
    }
}