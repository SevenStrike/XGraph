namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ActionNode_Base : ScriptableObject
    {
        /// <summary>
        /// 行为节点 - 名称
        /// </summary>
        [SerializeField] public string identifyName;
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
        /// 行为节点 - 在GraphView里的颜色标记方案名称
        /// </summary>
        [SerializeField] public string themeSolution = "M 默认";
        /// <summary>
        /// 行为节点 - 在GraphView里的颜色标记
        /// </summary>
        [SerializeField] public Color themeColor = Color.clear;

        /// <summary>
        /// 行为执行方法
        /// </summary>
        /// <returns></returns>
        public abstract void Execute();

        public string GetInfo()
        {
            return $"{namespaces}.{classes}{actionNodeType.ToString()}   /   {visualNodeType.ToString()}   /   {identifyName}";
        }

        public string GetPath()
        {
            return $"{path}";
        }
    }
}