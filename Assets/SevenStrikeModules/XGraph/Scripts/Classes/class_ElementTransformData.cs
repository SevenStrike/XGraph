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