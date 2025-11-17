namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class Action_ModuleActivate : ActionNode_Composite
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
                asset.ModuleController.Module_Active(activateName, activateState);
            }

        }

        /// <summary>
        /// 设置待激活的模块名称
        /// </summary>
        /// <param name="portName"></param>
        public void Set_ActivateModuleName(string portName)
        {
            PortValue_Set<string>(portName, value => activateName = value);
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