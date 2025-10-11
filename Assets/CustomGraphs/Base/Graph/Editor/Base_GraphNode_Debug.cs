namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class Base_GraphNode_Debug : VNode_Base
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口设置
            List<xGraph_NodePort> port_in = new List<xGraph_NodePort>();
            // 加入行为端口
            port_in.Add(new xGraph_NodePort("", typeof(ActionNode_Base), Port.Capacity.Single));
            // 加入变量端口
            port_in.Add(new xGraph_NodePort("对象", typeof(Variable), Port.Capacity.Single));
            InputPort_Set(port_in);

            List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
            // 加入行为端口
            port_out.Add(new xGraph_NodePort("", typeof(ActionNode_Base), Port.Capacity.Multi));
            OutputPort_Set(port_out);
            #endregion
        }

        #region 节点绘制
        public override VNode_Base Draw()
        {
            // 绘制主容器
            Draw_Main();

            // 绘制标题容器
            Draw_Title();

            // 绘制标题按钮容器
            Draw_TitleButton();

            // 绘制顶部容器
            Draw_Top();

            // 绘制输入节点容器
            Draw_Input();

            // 绘制输出节点容器
            Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }

        public override void Draw_Input()
        {
            base.Draw_Input();
        }
        #endregion

        #region 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();
            SetMessage("对象");
        }
        #endregion

        #region 辅助
        public void SetMessage(string portName)
        {
            ActionNode_Debug debug = ActionData as ActionNode_Debug;

            Variable variable = debug.Variable_Get(portName);
            if (variable != null)
            {
                string value = "";
                switch (variable.GetActiveType())
                {
                    case VariableType.String:
                        value = variable.GetValue<string>();
                        break;
                    case VariableType.Float:
                        value = variable.GetValue<float>().ToString();
                        break;
                    case VariableType.Int:
                        value = variable.GetValue<int>().ToString();
                        break;
                    case VariableType.Bool:
                        value = variable.GetValue<bool>().ToString();
                        break;
                    case VariableType.Vector2:
                        value = variable.GetValue<Vector2>().ToString();
                        break;
                    case VariableType.Vector3:
                        value = variable.GetValue<Vector3>().ToString();
                        break;
                    case VariableType.Vector4:
                        value = variable.GetValue<Vector4>().ToString();
                        break;
                    case VariableType.Color:
                        value = variable.GetValue<Color>().ToString();
                        break;
                }
                debug.Message = value;
            }
        }
        #endregion
    }
}