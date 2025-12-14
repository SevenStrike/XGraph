namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEngine;

    [Serializable]
    public class xAction_Branch : xAction_Base
    {
        /// <summary>
        /// 条件状态
        /// </summary>
        [SerializeField] public bool PredicateState;
        /// <summary>
        /// 子节点 True
        /// </summary>
        [SerializeField] public string childNode_true;
        /// <summary>
        /// 子节点 False
        /// </summary>
        [SerializeField] public string childNode_false;

        /// <summary>
        /// 节点执行方法
        /// </summary>
        public override void Execute()
        {
            if (On_Node_Excute != null)
                On_Node_Excute();
        }

        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            SetPredicateState("条件");
        }

        /// <summary>
        /// 设置条件判断状态
        /// </summary>
        /// <param name="state"></param>
        public void SetPredicateState(string portName)
        {
            PortValue_Set<bool>(portName, value => PredicateState = value);
        }

        /// <summary>
        /// 设置条件判断状态
        /// </summary>
        /// <param name="state"></param>
        public void SetPredicateState(bool state)
        {
            PredicateState = state;
        }

        /// <summary>
        /// 克隆该节点时确保改脚本独立变量正确克隆
        /// </summary>
        /// <returns></returns>
        public override xAction_Base Clone()
        {
            // 调用基类的Clone方法
            xAction_Branch clone = base.Clone() as xAction_Branch;

            // 复制派生类特有的字段
            if (clone != null)
            {
                clone.PredicateState = this.PredicateState;
                clone.childNode_true = this.childNode_true;
                clone.childNode_false = this.childNode_false;
            }

            return clone;
        }
    }
}