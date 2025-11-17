namespace SevenStrikeModules.XGraph
{
    using UnityEngine;
    public class Action_GetLight : ActionNode_Composite
    {
        public Light Light;

        public override void Execute()
        {
            base.Execute();

            Sample_GraphAsset asset = RootAsset as Sample_GraphAsset;
            if (asset != null)
                if (asset.ModuleController.MainLight != null)
                    Light = asset.ModuleController.MainLight;
        }

        /// <summary>
        /// 设置灯光的开关状态
        /// </summary>
        /// <param name="portName"></param>
        public void Get_MainLight(string portName)
        {

        }
    }
}