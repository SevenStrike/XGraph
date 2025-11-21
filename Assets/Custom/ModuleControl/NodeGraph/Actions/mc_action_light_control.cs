namespace SevenStrikeModules.XGraph
{
    public class mc_action_light_control : xAction_Composite
    {
        public bool lightEnable;

        public override void Execute()
        {
            base.Execute();

            mc_GraphAsset asset = RootAsset as mc_GraphAsset;
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
    }
}