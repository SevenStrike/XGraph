namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class action_mc_mainlight_control : xAction_Composite
    {
        public bool lightEnable;

        public override void Execute()
        {
            base.Execute();

            mc_GraphAsset asset = BaseArgs.RootAsset as mc_GraphAsset;
            if (asset != null)
            {
                asset.ModuleController.MainLight_Control(lightEnable);
            }
        }

        /// <summary>
        /// 设置灯光的开关状态
        /// </summary>
        /// <param name="portName"></param>
        public void Set_LightEnabled(string portName)
        {
            PortValue_Set<bool>(portName, value => lightEnable = value);
        }

        /// <summary>
        /// 设置灯光的开关状态
        /// </summary>
        /// <param name="state"></param>
        public void Set_LightEnabled(bool state)
        {
            lightEnable = state;
        }

        /// <summary>
        /// 克隆该节点时确保改脚本独立变量正确克隆
        /// </summary>
        /// <returns></returns>
        public override xAction_Base Clone()
        {
            // 调用基类的Clone方法
            action_mc_mainlight_control clone = base.Clone() as action_mc_mainlight_control;

            // 复制派生类特有的字段
            if (clone != null)
            {
                clone.lightEnable = this.lightEnable;
            }

            return clone;
        }
    }
}