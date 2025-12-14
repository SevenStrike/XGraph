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