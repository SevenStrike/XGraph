namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class action_mc_modules_activate : xAction_Composite
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

            mc_GraphAsset asset = BaseArgs.RootAsset as mc_GraphAsset;
            if (asset != null)
                asset.ModuleController.Modules_Active(activateState);
        }

        /// <summary>
        /// 设置模块的激活状态
        /// </summary>
        /// <param name="portName"></param>
        public void Set_ActivateModulesState(string portName)
        {
            PortValue_Set<bool>(portName, value => activateState = value);
        }

        /// <summary>
        /// 设置模块的激活状态
        /// </summary>
        /// <param name="state"></param>
        public void Set_ActivateModulesState(bool state)
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
            action_mc_modules_activate clone = base.Clone() as action_mc_modules_activate;

            // 复制派生类特有的字段
            if (clone != null)
            {
                clone.activateState = this.activateState;
            }

            return clone;
        }
    }
}