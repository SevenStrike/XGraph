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
    /// <summary>
    /// 面板的布局位置记录类
    /// </summary>
    public class VisualElementTransformData
    {
        /// <summary>
        /// 上边距
        /// </summary>
        public float top;
        /// <summary>
        /// 下边距
        /// </summary>
        public float bottom;
        /// <summary>
        /// 左边距
        /// </summary>
        public float left;
        /// <summary>
        /// 右边距
        /// </summary>
        public float right;
        /// <summary>
        /// 上边距是否为Auto状态
        /// </summary>
        public bool anc_Top;
        /// <summary>
        /// 下边距是否为Auto状态
        /// </summary>
        public bool anc_Bottom;
        /// <summary>
        /// 左边距是否为Auto状态
        /// </summary>
        public bool anc_Left;
        /// <summary>
        /// 右边距是否为Auto状态
        /// </summary>
        public bool anc_Right;

        /// <summary>
        /// 尺寸
        /// </summary>
        public Vector2 size;

        internal VisualElementTransformData Clone()
        {
            VisualElementTransformData value = new VisualElementTransformData();
            value.top = this.top;
            value.bottom = this.bottom;
            value.left = this.left;
            value.right = this.right;
            value.anc_Top = this.anc_Top;
            value.anc_Bottom = this.anc_Bottom;
            value.anc_Left = this.anc_Left;
            value.anc_Right = this.anc_Right;
            value.size = this.size;

            return value;
        }
    }
}