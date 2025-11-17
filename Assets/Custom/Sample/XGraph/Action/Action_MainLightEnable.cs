namespace SevenStrikeModules.XGraph
{
    public class Action_MainLightEnable : ActionNode_Composite
    {
        public bool lightEnable;

        public override void Execute()
        {
            base.Execute();

            Sample_GraphAsset asset = RootAsset as Sample_GraphAsset;
            if (asset != null)
            {
                asset.ModuleController.MainLight_Control(lightEnable);
            }
        }

        /// <summary>
        /// 设置灯光的开关状态
        /// </summary>
        /// <param name="portName"></param>
        public void Set_MainLightToggle(string portName)
        {
            PortValue_Set<bool>(portName, value => lightEnable = value);
        }
    }
}