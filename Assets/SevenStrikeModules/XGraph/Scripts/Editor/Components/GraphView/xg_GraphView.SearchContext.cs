namespace SevenStrikeModules.XGraph
{
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public partial class xg_GraphView
    {
        /// <summary>
        /// 实例化节点搜索框的主体
        /// </summary>
        private void AddNodesSearchBox()
        {
            gv_NodesSearchBox = ScriptableObject.CreateInstance<xg_NodesSearchBox>();
            gv_NodesSearchBox.Init(this);

            // 当激活节点创建时使用实例化的节点搜索主体指定为搜索框的内容
            OpenNodeSearchBox();
        }
        /// <summary>
        /// 当激活节点创建时使用实例化的节点搜索主体指定为搜索框的内容
        /// </summary>
        private void OpenNodeSearchBox()
        {
            nodeCreationRequest = context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), gv_NodesSearchBox);
            };
        }
    }
}