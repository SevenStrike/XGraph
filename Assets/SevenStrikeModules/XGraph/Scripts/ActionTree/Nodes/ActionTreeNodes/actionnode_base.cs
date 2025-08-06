namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public abstract class actionnode_base : ScriptableObject
    {
        /// <summary>
        /// 行为节点 - 名称
        /// </summary>
        [SerializeField] public string nodeName;
        /// <summary>
        /// 行为节点 - guid
        /// </summary>
        [SerializeField] public string nodeGUID;
        /// <summary>
        /// 行为节点 - guid
        /// </summary>
        [SerializeField] public string nodeNameSpaceName;
        /// <summary>
        /// 行为节点 - guid
        /// </summary>
        [SerializeField] public string nodeClassName;
        /// <summary>
        /// 行为节点 - 相对路径
        /// </summary>
        [SerializeField] public string nodePath;
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
        [SerializeField] public string graphNodeType = "None";
        /// <summary>
        /// 行为节点 - 在GraphView里的位置记录
        /// </summary>
        [SerializeField] public Vector2 nodeGraphPosition;
        /// <summary>
        /// 行为节点 - 在GraphView里的颜色标记方案名称
        /// </summary>
        [SerializeField] public string nodeThemeSolution = "M 默认";
        /// <summary>
        /// 行为节点 - 在GraphView里的颜色标记
        /// </summary>
        [SerializeField] public Color nodeThemeColor = Color.clear;

        /// <summary>
        /// 行为执行方法
        /// </summary>
        /// <returns></returns>
        public abstract void Execute();

        public string GetInfo()
        {
            return $"{nodeNameSpaceName}.{nodeClassName}{actionNodeType.ToString()}   /   {graphNodeType.ToString()}   /   {nodeName}";
        }

        public string GetPath()
        {
            return $"{nodePath}";
        }
    }
}