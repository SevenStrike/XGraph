/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGraph 行为流程图插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEngine;

    [Serializable]
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