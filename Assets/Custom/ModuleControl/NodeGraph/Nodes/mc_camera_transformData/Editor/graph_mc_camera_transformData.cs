namespace SevenStrikeModules.XGraph
{
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class graph_mc_camera_transformData : xNode_Property
    {
        action_mc_camera_transformData camera_transformData;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            OutputPort_Add(new xGraph_NodePort("位置", typeof(Variable_Vector3), Port.Capacity.Multi));// 加入变量端口（输出）
            OutputPort_Add(new xGraph_NodePort("角度", typeof(Variable_Vector3), Port.Capacity.Multi));// 加入变量端口（输出）
            OutputPort_Add(new xGraph_NodePort("距离", typeof(Variable_Float), Port.Capacity.Multi));// 加入变量端口（输出）

            camera_transformData = property as action_mc_camera_transformData;
        }

        #region 节点绘制
        public override xNode_Base Draw()
        {
            return base.Draw();
        }
        #endregion
    }
}