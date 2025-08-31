namespace SevenStrikeModules.XGraph
{
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xg_GraphViewGridBackground : GridBackground
    {
        /// <summary>
        /// 通过反射获取私有字段 - 网格线间距
        /// </summary>
        private static System.Reflection.FieldInfo s_SpacingField;
        /// <summary>
        /// 通过反射获取私有字段 - 分界线数量
        /// </summary>
        private static System.Reflection.FieldInfo s_ThickLinesField;
        /// <summary>
        /// 通过反射获取私有字段 - 网格线颜色
        /// </summary>
        private static System.Reflection.FieldInfo s_LineColorField;
        /// <summary>
        /// 通过反射获取私有字段 - 分界线颜色
        /// </summary>
        private static System.Reflection.FieldInfo s_ThickLineColorField;
        /// <summary>
        /// 通过反射获取私有字段 - 背景颜色
        /// </summary>
        private static System.Reflection.FieldInfo s_GridBackgroundColorField;

        static xg_GraphViewGridBackground()
        {
            // 在静态构造函数中通过反射获取私有字段信息
            var gridBackgroundType = typeof(GridBackground);
            var bindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;

            s_SpacingField = gridBackgroundType.GetField("m_Spacing", bindingFlags);
            s_ThickLinesField = gridBackgroundType.GetField("m_ThickLines", bindingFlags);
            s_LineColorField = gridBackgroundType.GetField("m_LineColor", bindingFlags);
            s_ThickLineColorField = gridBackgroundType.GetField("m_ThickLineColor", bindingFlags);
            s_GridBackgroundColorField = gridBackgroundType.GetField("m_GridBackgroundColor", bindingFlags);
        }

        public void Initialize()
        {
            name = "GridBackground";

            VisualElement gra = new VisualElement();
            gra.name = "gradient";
            gra.pickingMode = PickingMode.Ignore;
            gra.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}GraphViewViggnet.png");
            Add(gra);
        }

        /// <summary>
        /// 改变间距
        /// </summary>
        /// <param name="value"></param>
        public void SetSpacing(float value)
        {
            if (s_SpacingField != null)
            {
                s_SpacingField.SetValue(this, value);
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// 改变分界线数量
        /// </summary>
        /// <param name="value"></param>
        public void SetThickLines(int value)
        {
            if (s_ThickLinesField != null)
            {
                s_ThickLinesField.SetValue(this, value);
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// 改变网格颜色
        /// </summary>
        /// <param name="value"></param>
        public void SetLineColor(Color value)
        {
            if (s_LineColorField != null)
            {
                s_LineColorField.SetValue(this, value);
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// 改变分界线颜色
        /// </summary>
        /// <param name="value"></param>
        public void SetThickLineColor(Color value)
        {
            if (s_ThickLineColorField != null)
            {
                s_ThickLineColorField.SetValue(this, value);
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// 改变背景颜色
        /// </summary>
        /// <param name="value"></param>
        public void SetGridBackgroundColor(Color value)
        {
            if (s_GridBackgroundColorField != null)
            {
                s_GridBackgroundColorField.SetValue(this, value);
                MarkDirtyRepaint();
            }
        }
    }
}