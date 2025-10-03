namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public abstract class ActionNode_Base : ScriptableObject
    {
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
        /// 变量Guids接驳列表
        /// </summary>
        [SerializeField] public List<VarialbleGuidConnector> VariableDatas = new List<VarialbleGuidConnector>();
        /// <summary>
        /// 内部变量Guids接驳列表
        /// </summary>
        [SerializeField] public List<VarialbleInternalGuidConnector> InternalVariableDatas = new List<VarialbleInternalGuidConnector>();
        /// <summary>
        /// 行为根节点
        /// </summary>
        [SerializeField] public ActionNode_Asset RootAsset;
        // 新增：父节点引用
        [NonSerialized] private ActionNode_Base _parentNode;
        public ActionNode_Base ParentNode => _parentNode;

        /// <summary>
        /// 设置父节点的方法
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(ActionNode_Base parent)
        {
            _parentNode = parent;
        }
        /// <summary>
        /// 设置行为根节点的方法
        /// </summary>
        /// <param name="root"></param>
        public void SetRoot(ActionNode_Asset root)
        {
            RootAsset = root;
        }
        /// <summary>
        /// 行为执行方法
        /// </summary>
        /// <returns></returns>
        public abstract void Execute();
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
        /// <summary>
        /// 获取变量值（根据预设的端口名称）
        /// </summary>
        /// <param name="portName"></param>
        /// <returns>如果返回为空请检查传入的节点端口名称 "portName" 是否存在，或者 "portName" 端口是否链接了变量节点数据</returns>
        public Variable Variable_Get(string portName)
        {
            Variable vare = null;
            bool exist = false;

            // 先从绑定的黑板变量数据列表中找
            foreach (var item in VariableDatas)
            {
                if (portName == item.TargetPortName)
                {
                    vare = item.variable;
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
                        vare = item.variable;
                    }
                }
            }

            return vare;
        }
        /// <summary>
        /// 设置变量值（根据预设的端口名称）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="portName"></param>
        /// <param name="value"></param>
        ///         /// <returns>如果返回为空请检查传入的节点端口名称 "portName" 是否存在，或者 "portName" 端口是否链接了变量节点数据</returns>
        public void Variable_Set<T>(string portName, T value)
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
                            ActionNode_Base ac = RootAsset.FindActionNode(targetNode);
                            if (ac != null)
                            {
                                if (ac is ActionNode_Variable va)
                                {
                                    // 如果已经指定了黑板变量，则修改匹配的黑板变量值
                                    if (va.VariableDatas != null && va.VariableDatas.Count > 0)
                                    {
                                        VarialbleGuidConnector con = va.VariableDatas.First();
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

        #region 黑板变量 绑定/解绑
        /// <summary>
        /// 绑定黑板变量数据到行为节点的 ”黑板变量数据列表中“
        /// </summary>
        /// <param name="data"></param>
        /// <param name="portName"></param>
        public void VariableData_Bind(ActionVariableData data, string portName)
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
                Undo.RecordObject(this, "Assigned Variable Connector");
                VariableDatas.Add(new VarialbleGuidConnector(data.guid, portName, data.variable));
#endif
            }
        }
        /// <summary>
        /// 从行为节点的 ”黑板变量数据列表中“中解绑指定的guid和端口名称的变量数据的绑定
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="portName"></param>
        public void VariableData_Unbind(string guid, string portName)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Unassigned Variable Connector");

            // 如果在变量链接信息列表中找到指定的guid和名称的变量链接信息列表项则删除
            VariableDatas.RemoveAll(item => item.VariableNodeGuid == guid && item.TargetPortName == portName);
#endif
        }
        #endregion

        #region 内部变量 绑定/解绑
        /// <summary>
        /// 绑定内部变量数据到行为节点的 ”内部变量数据列表中“
        /// </summary>
        /// <param name="data"></param>
        /// <param name="portName"></param>
        public void InternalVariableData_Bind(ActionNode_Variable data, string portName)
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
                Undo.RecordObject(this, "Binded VariableInternal Connector");
                VarialbleInternalGuidConnector con = new VarialbleInternalGuidConnector(data.guid, portName, data.variable);
                con.variable.name = data.identifyName;
                InternalVariableDatas.Add(con);
#endif
            }
        }
        /// <summary>
        /// 从行为节点的 ”内部变量数据列表中“中解绑指定的guid和端口名称的变量数据的绑定
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="portName"></param>
        public void InternalVariableData_Unbind(string guid, string portName)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Unbinded InternalVariable Connector");

            // 如果在变量链接信息列表中找到指定的guid和名称的变量链接信息列表项则删除
            InternalVariableDatas.RemoveAll(item => item.VariableNodeGuid == guid && item.TargetPortName == portName);
#endif
        }
        #endregion
    }
}