namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;

    public abstract class ActionNode_Wait : ActionNode_Base
    {
        /// <summary>
        /// 等待时间
        /// </summary>
        public float Time;
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<ActionNode_Base> childNodes = new List<ActionNode_Base>();

        public override void Execute()
        {
            if (On_Node_Excute != null)
                On_Node_Excute();
        }

        #region 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            SetWaitTime("时间");
        }
        #endregion

        #region 辅助
        public void SetWaitTime(string portName)
        {
            PortValue_Set<float>(portName, value => Time = value);
        }
        #endregion
    }
}