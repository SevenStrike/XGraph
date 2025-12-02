namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;

    public abstract class xAction_Wait : xAction_Base
    {
        /// <summary>
        /// 等待时间
        /// </summary>
        public float Time;
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<string> childNodes = new List<string>();

        /// <summary>
        /// 行为节点执行方法
        /// </summary>
        public override void Execute()
        {
            SetWaitTime("时间");

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
        /// <summary>
        /// 设置等待时间
        /// </summary>
        /// <param name="portName"></param>
        public void SetWaitTime(string portName)
        {
            PortValue_Set<float>(portName, value => Time = value);
        }
        #endregion
    }
}