namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class action_ff_ccset : xAction_Property
    {
        /// <summary>
        /// 节点执行
        /// </summary>
        public override void Execute()
        {
            base.Execute();
        }


        /// <summary>
        /// 初始化属性变量列表
        /// </summary>
        public override void Propertys_Initialize()
        {
            base.Propertys_Initialize();

            #region 属性
            Variable vare_property = new Variable_Float("属性");
            vare_property.description = "实时获取到的 - 属性";
            Propertys_Add(vare_property);
            #endregion
        }

        /// <summary>
        /// 更新属性
        /// </summary>
        public override void Propertys_Update()
        {
            base.Propertys_Update();
        }
    }
}