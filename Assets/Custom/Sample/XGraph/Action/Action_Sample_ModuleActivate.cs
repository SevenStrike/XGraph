namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class Action_Sample_ModuleActivate : ActionNode_Composite
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

            Sample_GraphAsset asset = RootAsset as Sample_GraphAsset;
            if (asset != null)
            {
                asset.Sample_Controller.module_active(activateName, activateState);
            }

        }

        public void Set_ActivateModuleName(string portName)
        {
            PortValue_Set<string>(portName, value => activateName = value);
        }

        public void Set_ActivateModuleState(string portName)
        {
            PortValue_Set<bool>(portName, value => activateState = value);
        }
    }
}