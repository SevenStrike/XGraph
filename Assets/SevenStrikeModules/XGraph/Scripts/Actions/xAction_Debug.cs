/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGraph 行为流程图插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class xAction_Debug : xAction_Base
    {
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<string> childNodes = new List<string>();

        [SerializeField] public string Prefix;
        [SerializeField] public string Msg;

        /// <summary>
        /// 节点执行方法
        /// </summary>
        public override void Execute()
        {
            if (On_Node_Excute != null)
                On_Node_Excute();

            DebugMessage_withPort("对象");
            SetPrefix_withPort("前缀");

            util_Dashboard.LogMsg(xMessageType.信息, $"---> ：", $"{identifyName}  {Prefix}{Msg}  （{(BaseArgs.isConcurrentExecution ? "并发" : "顺序")}）", BaseArgs.RootAsset.LogEnabled);
        }

        public void DebugMessage_withPort(string value)
        {
            Variable vare = Variable_Get(value);
            if (vare != null)
                Msg = vare.GetValue().ToString();
        }

        public void SetPrefix_withPort(string value)
        {
            Variable vare = Variable_Get(value);
            if (vare != null)
                Prefix = vare.GetValue().ToString();
        }

        public void SetPrefix(string value)
        {
            Prefix = value;
        }

        public void DebugMessage(string value)
        {
            Msg = value;
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
                clone.Prefix = this.Prefix;
            }

            return clone;
        }
    }
}