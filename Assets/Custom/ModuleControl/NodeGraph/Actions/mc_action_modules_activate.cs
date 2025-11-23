namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class mc_action_modules_activate : xAction_Composite
    {
        /// <summary>
        /// 是否激活所有模组
        /// </summary>
        [SerializeField] public bool activateState;

        /// <summary>
        /// 节点执行
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            mc_GraphAsset asset = RootAsset as mc_GraphAsset;
            if (asset != null)
                asset.ModuleController.Modules_Active(activateState);
        }

        /// <summary>
        /// 设置模块的激活状态
        /// </summary>
        /// <param name="portName"></param>
        public void Set_ActivateModuleState(string portName)
        {
            PortValue_Set<bool>(portName, value => activateState = value);
        }
    }
}