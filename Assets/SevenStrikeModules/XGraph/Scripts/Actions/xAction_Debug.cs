namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEngine;

    public class xAction_Debug : xAction_Base
    {
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<string> childNodes = new List<string>();

        [SerializeField] public string Msg;

        /// <summary>
        /// 节点执行方法
        /// </summary>
        public override void Execute()
        {
            if (On_Node_Excute != null)
                On_Node_Excute();

            DebugMessage();
        }

        public void DebugMessage()
        {
            Variable vare = Variable_Get("对象");
            if (vare != null)
                Msg = vare.GetValue().ToString();
            util_Dashboard.LogMsg(xMessageType.信息, $"---> ：", $"{identifyName}  {Msg}  （{(BaseArgs.isConcurrentExecution ? "并发" : "顺序")}）", BaseArgs.RootAsset.LogEnabled);
        }

        /// <summary>
        /// 克隆该节点时确保改脚本独立变量正确克隆
        /// </summary>
        /// <returns></returns>
        public override xAction_Base Clone()
        {
            // 调用基类的Clone方法
            xAction_Debug clone = base.Clone() as xAction_Debug;

            // 复制派生类特有的字段
            if (clone != null)
            {
                clone.Msg = this.Msg;
            }

            return clone;
        }
    }
}