namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class xAction_Wait : xAction_Base
    {
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<string> childNodes = new List<string>();

        /// <summary>
        /// 等待时间
        /// </summary>
        public float Time;

        /// <summary>
        /// 行为节点执行方法
        /// </summary>
        public override void Execute()
        {
            SetWaitTime("时间");

            if (On_Node_Excute != null)
                On_Node_Excute();
        }

        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            SetWaitTime("时间");
        }

        /// <summary>
        /// 设置等待时间
        /// </summary>
        /// <param name="portName"></param>
        public void SetWaitTime(string portName)
        {
            PortValue_Set<float>(portName, value => Time = value);
        }

        /// <summary>
        /// 设置等待时间
        /// </summary>
        /// <param name="time"></param>
        public void SetWaitTime(float time)
        {
            Time = time;
        }

        /// <summary>
        /// 克隆该节点时确保改脚本独立变量正确克隆
        /// </summary>
        /// <returns></returns>
        public override xAction_Base Clone()
        {
            // 调用基类的Clone方法
            xAction_Wait clone = base.Clone() as xAction_Wait;

            // 复制派生类特有的字段
            if (clone != null)
            {
                clone.Time = this.Time;
            }

            return clone;
        }
    }
}