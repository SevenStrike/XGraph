namespace SevenStrikeModules.XGraph
{
    public class Action_ModuleInitialize : ActionNode_Start
    {
        public bool activateAllModules;

        /// <summary>
        /// 节点执行
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            Sample_GraphAsset asset = RootAsset as Sample_GraphAsset;

            asset.ModuleController.Modules_Active(activateAllModules);
        }
        /// <summary>
        /// 当任意变量值改变时调用
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            SetStartDispayModules("激活所有模组");
        }
        /// <summary>
        /// 从目标端口的变量值来设置启动时显示所有模组变量值
        /// </summary>
        /// <param name="portName"></param>
        public void SetStartDispayModules(string portName)
        {
            Variable variable = Variable_Get(portName);
            if (variable != null)
                activateAllModules = variable.GetValue<bool>();
        }
    }
}