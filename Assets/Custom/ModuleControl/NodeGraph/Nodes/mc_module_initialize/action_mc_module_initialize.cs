namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class action_mc_module_initialize : xAction_Start
    {
        [SerializeField] public bool activateAllModules;

        /// <summary>
        /// 节点执行
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            mc_GraphAsset asset = RootAsset as mc_GraphAsset;
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
    }
}