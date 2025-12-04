namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class action_mc_property_mainlight : xAction_Property
    {
        [Header("- 组件 / 对象 -")]
        [SerializeField] public Light Light;


        /// <summary>
        /// 行为节点执行方法
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

            #region 灯光强度
            Variable vare_intensity = new Variable_Float("强度");
            vare_intensity.description = "实时获取到的 - 灯光强度";
            Propertys_Add(vare_intensity);
            #endregion

            #region 灯光范围
            Variable vare_range = new Variable_Float("范围");
            vare_range.description = "实时获取到的 - 灯光范围";
            Propertys_Add(vare_range);
            #endregion

            #region 灯光颜色
            Variable vare_color = new Variable_Color("颜色");
            vare_color.description = "实时获取到的 - 灯光颜色";
            Propertys_Add(vare_color);
            #endregion
        }

        /// <summary>
        /// 更新属性
        /// </summary>
        public override void Propertys_Update()
        {
            base.Propertys_Update();

            mc_GraphAsset asset = RootAsset as mc_GraphAsset;
            if (asset != null)
            {
                if (asset.ModuleController.sp_MainLight.light != null)
                {
                    Light = asset.ModuleController.sp_MainLight.light;

                    // 更新属性数值
                    foreach (var prop in PropertyVariables)
                    {
                        switch (prop.name)
                        {
                            case "强度":
                                prop.SetValue(Light.intensity);
                                break;
                            case "范围":
                                prop.SetValue(Light.range);
                                break;
                            case "颜色":
                                prop.SetValue(Light.color);
                                break;
                        }
                    }
                }
            }
        }
    }
}