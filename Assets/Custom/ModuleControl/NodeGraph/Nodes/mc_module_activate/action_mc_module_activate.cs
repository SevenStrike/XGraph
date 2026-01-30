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
    using UnityEngine;

    [Serializable]
    public class action_mc_module_activate : xAction_Composite
    {
        /// <summary>
        /// 是否激活模组
        /// </summary>
        [SerializeField] public bool activateState;
        /// <summary>
        /// 激活的模组名称
        /// </summary>
        [SerializeField] public string activateName;

        /// <summary>
        /// 节点执行
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            mc_GraphAsset asset = BaseArgs.RootAsset as mc_GraphAsset;
            if (asset != null)
                asset.ModuleController.Module_Active(activateName, activateState);
        }

        /// <summary>
        /// 设置待激活的模块名称
        /// </summary>
        /// <param name="portName"></param>
        public void Set_ActivateModuleName_withPort(string portName)
        {
            PortValue_Set<string>(portName, value => activateName = value);
        }

        /// <summary>
        /// 设置模块的激活状态
        /// </summary>
        /// <param name="portName"></param>
        public void Set_ActivateModuleState_withPort(string portName)
        {
            PortValue_Set<bool>(portName, value => activateState = value);
        }

        /// <summary>
        /// 设置待激活的模块名称
        /// </summary>
        /// <param name="portName"></param>
        public void Set_ActivateModuleName(string value)
        {
            activateName = value;
        }

        /// <summary>
        /// 设置模块的激活状态
        /// </summary>
        /// <param name="portName"></param>
        public void Set_ActivateModuleState(bool state)
        {
            activateState = state;
        }

        /// <summary>
        /// 克隆该节点时确保改脚本独立变量正确克隆
        /// </summary>
        /// <returns></returns>
        public override xAction_Base Clone()
        {
            // 调用基类的Clone方法
            action_mc_module_activate clone = base.Clone() as action_mc_module_activate;

            // 复制派生类特有的字段
            if (clone != null)
            {
                clone.activateState = this.activateState;
                clone.activateName = this.activateName;
            }

            return clone;
        }

        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            Set_ActivateModuleName_withPort("名称");
            Set_ActivateModuleState_withPort("激活");
        }
    }
}