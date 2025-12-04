using UnityEngine;

namespace SevenStrikeModules.XGraph
{
    public class action_mc_end : xAction_End
    {
        [Header("- 扩展 -")]
        [SerializeField] public bool activateAllModules;

        public override void Execute()
        {
            base.Execute();


            mc_GraphAsset asset = RootAsset as mc_GraphAsset;
            if (asset != null)
                asset.ModuleController.Modules_Active(activateAllModules, true);
        }

        #region 赋值
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
        #endregion

        #region 克隆
        /// <summary>
        /// 克隆该节点时确保改脚本独立变量正确克隆
        /// </summary>
        /// <returns></returns>
        public override xAction_Base Clone()
        {
            // 调用基类的Clone方法
            action_mc_end clone = base.Clone() as action_mc_end;

            // 复制派生类特有的字段
            if (clone != null)
            {
                clone.activateAllModules = this.activateAllModules;
            }

            return clone;
        }
        #endregion
    }
}