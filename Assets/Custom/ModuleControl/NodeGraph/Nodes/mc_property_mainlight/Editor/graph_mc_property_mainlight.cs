namespace SevenStrikeModules.XGraph
{
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class graph_mc_property_mainlight : xNode_Property
    {
        action_mc_property_mainlight mainlight;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            OutputPort_Add(new xGraph_NodePort("强度", typeof(Variable_Float), Port.Capacity.Multi));// 加入变量端口（输出）
            OutputPort_Add(new xGraph_NodePort("范围", typeof(Variable_Float), Port.Capacity.Multi));// 加入变量端口（输出）
            OutputPort_Add(new xGraph_NodePort("颜色", typeof(Variable_Color), Port.Capacity.Multi));// 加入变量端口（输出）

            mainlight = base.property as action_mc_property_mainlight;
        }
    }
}