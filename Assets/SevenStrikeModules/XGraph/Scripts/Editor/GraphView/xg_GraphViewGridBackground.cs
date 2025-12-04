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

        public Label TipLabel;

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
            gra.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI_Bg()}GraphViewViggnet.png");
            Add(gra);

            TipLabel = new Label();
            TipLabel.enableRichText = true;
            TipLabel.text = $"按下空格以创建节点\n<size=20>Pressed Space Key</size>";
            TipLabel.style.alignSelf = new StyleEnum<Align>(Align.Center);
            TipLabel.style.width = new StyleLength(StyleKeyword.Auto);
            TipLabel.style.flexGrow = 1;
            TipLabel.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
            TipLabel.style.fontSize = 40;
            TipLabel.style.unityFont = new StyleFont(util_XGraphEditorUtility.AssetLoad<Font>($"{util_Dashboard.GetPath_Fonts()}x_Heavy.ttf"));
            TipLabel.style.unityFontDefinition = new StyleFontDefinition(util_XGraphEditorUtility.AssetLoad<Font>($"{util_Dashboard.GetPath_Fonts()}x_Heavy.ttf"));
            TipLabel.style.opacity = 0.12f;
            TipLabel.style.letterSpacing = 5;
            Add(TipLabel);
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

        /// <summary>
        /// 获取提示文字组件
        /// </summary>
        /// <returns></returns>
        public Label GetTipLabel()
        {
            return TipLabel;
        }

        /// <summary>
        /// 提示文字组件的内容设置
        /// </summary>
        /// <param name="text"></param>
        public void SetTipLabelText(string text)
        {
            TipLabel.text = text;
        }

        /// <summary>
        /// 提示文字显示控制
        /// </summary>
        /// <param name="state"></param>
        public void TipLabel_Displayer(bool state)
        {
            TipLabel.style.display = new StyleEnum<DisplayStyle>(state ? DisplayStyle.Flex : DisplayStyle.None);
        }
    }
}