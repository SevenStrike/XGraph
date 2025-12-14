namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEngine;

    [Serializable]
    public class action_mc_module_initialize : xAction_Start
    {
        [SerializeField] public bool activateAllModules;

        /// <summary>
        /// 节点执行
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            mc_GraphAsset asset = BaseArgs.RootAsset as mc_GraphAsset;
            if (asset != null)
                asset.ModuleController.Modules_Active(activateAllModules, true);
        }

        /// <summary>
        /// 从目标端口的变量值来设置启动时显示所有模组变量值
        /// </summary>
        /// <param name="portName"></param>
        public void Set_ModulesInitialized(string portName)
        {
            PortValue_Set<bool>(portName, value => activateAllModules = value);
        }

        /// <summary>
        /// 设置启动时显示所有模组变量值
        /// </summary>
        /// <param name="portName"></param>
        public void Set_ModulesInitialized(bool state)
        {
            activateAllModules = state;
        }

        /// <summary>
        /// 克隆该节点时确保改脚本独立变量正确克隆
        /// </summary>
        /// <returns></returns>
        public override xAction_Base Clone()
        {
            // 调用基类的Clone方法
            action_mc_module_initialize clone = base.Clone() as action_mc_module_initialize;

            // 复制派生类特有的字段
            if (clone != null)
            {
                clone.activateAllModules = this.activateAllModules;
            }

            return clone;
        }

        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            Set_ModulesInitialized("激活所有模组");
        }
    }
}