namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public class action_mc_camera_transformData : xAction_Property
    {
        /// <summary>
        /// 目标相机
        /// </summary>
        [SerializeField] public Camera Camera;

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

            #region 相机位置
            Variable vare_pos = new Variable_Vector3("位置");
            vare_pos.description = "实时获取到的 - 相机位置";
            Propertys_Add(vare_pos);
            #endregion

            #region 相机旋转
            Variable vare_rotate = new Variable_Vector3("旋转");
            vare_rotate.description = "实时获取到的 - 相机旋转";
            Propertys_Add(vare_rotate);
            #endregion

            #region 相机距离
            Variable vare_distance = new Variable_Float("距离");
            vare_distance.description = "实时获取到的 - 相机距离";
            Propertys_Add(vare_distance);
            #endregion
        }

        /// <summary>
        /// 更新属性
        /// </summary>
        public override void Propertys_Update()
        {
            base.Propertys_Update();

            mc_GraphAsset asset = BaseArgs.RootAsset as mc_GraphAsset;
            if (asset != null)
            {
                if (asset.ModuleController.cam != null)
                {
                    Camera = asset.ModuleController.cam;

                    // 更新属性数值
                    foreach (var prop in PropertyVariables)
                    {
                        switch (prop.name)
                        {
                            case "位置":
                                prop.SetValue(Camera.transform.position);
                                break;
                            case "角度":
                                prop.SetValue(Camera.transform.eulerAngles);
                                break;
                            case "距离":
                                prop.SetValue(Camera.transform.localPosition.z);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 克隆该节点时确保改脚本独立变量正确克隆
        /// </summary>
        /// <returns></returns>
        public override xAction_Base Clone()
        {
            // 调用基类的Clone方法
            action_mc_camera_transformData clone = base.Clone() as action_mc_camera_transformData;

            // 复制派生类特有的字段
            if (clone != null)
            {
                clone.Camera = this.Camera;
            }

            return clone;
        }
    }
}