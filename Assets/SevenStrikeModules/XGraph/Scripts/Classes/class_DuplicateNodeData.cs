namespace SevenStrikeModules.XGraph
{
    using System;

    [Serializable]
    /// <summary>
    /// 克隆节点时的参数引用传递类
    /// </summary>
    public class DuplicateNodeData
    {
        public string SourceNodeGuid;
#if UNITY_EDITOR
        public UnityEditor.Experimental.GraphView.Node DuplicatedNode;
#endif
    }
}