namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
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
            return $"{path}";
        }

        #region VariableDatas
        /// <summary>
        /// 绑定变量数据到节点数据中
        /// </summary>
        /// <param name="data"></param>
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
                Undo.RecordObject(this, "Assigned Variable Connector");
                VariableDatas.Add(new VarialbleGuidConnector(data.guid, portName, data.variable));
            }
        }
        /// <summary>
        /// 从节点中解绑变量数据
        /// </summary>
        /// <param name="variableData"></param>
        public void VariableData_Unbind(string guid, string portName)
        {
            // 如果在变量链接信息列表中找到指定的guid和名称的变量链接信息列表项则删除
            int removedCount = VariableDatas.RemoveAll(item => item.VariableNodeGuid == guid && item.TargetPortName == portName);
            if (removedCount > 0)
            {
                Undo.RecordObject(this, "Unassigned Variable Connector");
            }
        }
        /// <summary>
        /// 变量 - 值获取
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Variable VariableData_GetValue(string name)
        {
            Variable vare = null;
            VariableDatas.ForEach(data =>
            {
                if (name == data.TargetPortName)
                {
                    vare = data.variable;
                }
            });
            return vare;
        }
        /// <summary>
        /// 变量 - 存在检查
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool VariableData_ValueExist(string name)
        {
            bool isExist = false;
            VariableDatas.ForEach((data) =>
            {
                if (name == data.TargetPortName)
                    isExist = true;
            });
            return isExist;
        }
        #endregion
    }
}