namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public static class util_XGraphEditorUtility
    {
        /// <summary>
        /// 从指定路径加载指定类型的资源
        /// </summary>
        /// <typeparam name="T">要加载的资源类型</typeparam>
        /// <param name="path">资源路径（相对于Assets文件夹）</param>
        /// <returns>加载的资源对象，如果找不到则返回null</returns>
        public static T AssetLoad<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("资源路径不能为空");
                return null;
            }

            T asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset == null)
            {
                Debug.LogError($"在路径 {path} 找不到类型为 {typeof(T)} 的资源");
            }

            return asset;
        }

        /// <summary>
        /// 获取指定的VisualElement下的指定类型和名称的控件
        /// </summary>
        /// <typeparam name="T">控件类型（如VisualElement、Label等）</typeparam>
        /// <param name="root">根元素</param>
        /// <param name="name">元素名称</param>
        /// <param name="log">如果未找到是否打印错误</param>
        /// <returns>找到的控件，未找到则返回null</returns>
        public static T GetUIElement<T>(VisualElement root, string name, bool log = true) where T : VisualElement
        {
            T element = root.Q<T>(name);
            if (element == null && log)
            {
                Debug.LogError($"未找到UI元素: {name} (类型: {typeof(T).Name})");
            }
            return element;
        }

        #region 字符串 -> 颜色
        /// <summary>
        /// 根据RGBA值转换到Color类型
        /// </summary>
        /// <param name="R">填写 R - 红色值</param>
        /// <param name="G">填写 G - 绿色值</param>
        /// <param name="B">填写 B - 蓝色值</param>
        /// <param name="A">填写 A - 透明度值</param>
        /// <param name="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color</returns>
        public static Color Color_From_RGBA(float R, float G, float B, float A, bool ValueMode = true)
        {
            Color color = new Color();
            if (ValueMode)
            {
                color.r = R / 255f;
                color.g = G / 255f;
                color.b = B / 255f;
                color.a = A / 255f;
            }
            else
            {
                color.r = R;
                color.g = G;
                color.b = B;
                color.a = A;
            }
            return color;
        }
        /// <summary>
        /// 将分隔符为逗号的"r,g,b,a"的字符串转换为Color类型
        /// </summary>
        /// <param name="ColorString">目标颜色格式字符串.</param>
        /// <param name="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color Color_From_String(string ColorString, bool ValueMode = true)
        {
            if (string.IsNullOrEmpty(ColorString))
                return Color.white;
            int Count = 0;
            string[] xx = ColorString.Split(new char[1] { ',' });

            float[] x = new float[xx.Length];

            foreach (string a in xx)
            {
                if (ValueMode)
                    x[Count] = float.Parse(a) / 255f;
                else
                    x[Count] = float.Parse(a);
                Count++;
                if (Count >= 4)
                {
                    Count = 0;
                }
            }
            Color color = new Color(x[0], x[1], x[2], x[3]);
            return color;
        }
        /// <summary>
        /// 将字符串数组转换为Color类型
        /// </summary>
        /// <param name="ColorStringArray">颜色字符串数组，数组长度为4，既代表了RGBA</param>
        /// <param name="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color Color_From_StringArray(string[] ColorStringArray, bool ValueMode = true)
        {
            Color color = new Color();
            if (ValueMode)
            {
                color.r = float.Parse(ColorStringArray[0]) / 255f;
                color.g = float.Parse(ColorStringArray[1]) / 255f;
                color.b = float.Parse(ColorStringArray[2]) / 255f;
                color.a = float.Parse(ColorStringArray[3]);
            }
            else
            {
                color.r = float.Parse(ColorStringArray[0]);
                color.g = float.Parse(ColorStringArray[1]);
                color.b = float.Parse(ColorStringArray[2]);
                color.a = float.Parse(ColorStringArray[3]);
            }
            return color;
        }
        /// <summary>
        /// 将浮点数组转换为Color类型
        /// </summary>
        /// <param name="ColorFloatArray">颜色浮点数组，数组长度为4，既代表了RGBA</param>
        /// <param name="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color Color_From_FloatArray(float[] ColorFloatArray, bool ValueMode = true)
        {
            Color color = new Color();
            if (ValueMode)
            {
                color.r = ColorFloatArray[0] / 255f;
                color.g = ColorFloatArray[1] / 255f;
                color.b = ColorFloatArray[2] / 255f;
                color.a = ColorFloatArray[3];
            }
            else
            {
                color.r = ColorFloatArray[0];
                color.g = ColorFloatArray[1];
                color.b = ColorFloatArray[2];
                color.a = ColorFloatArray[3];
            }
            return color;
        }
        /// <summary>
        /// 将整形数组转换为Color类型
        /// </summary>
        /// <param name="ColorFloatArray">颜色浮点数组，数组长度为4，既代表了RGBA</param>
        /// <param name="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
        /// <returns>此方法返回类型为 -> 颜色_Color.</returns>
        public static Color Color_From_IntArray(int[] ColorFloatArray, bool ValueMode = true)
        {
            Color color = new Color();
            if (ValueMode)
            {
                color.r = (float)ColorFloatArray[0] / 255f;
                color.g = (float)ColorFloatArray[1] / 255f;
                color.b = (float)ColorFloatArray[2] / 255f;
                color.a = (float)ColorFloatArray[3];
            }
            else
            {
                color.r = (float)ColorFloatArray[0];
                color.g = (float)ColorFloatArray[1];
                color.b = (float)ColorFloatArray[2];
                color.a = (float)ColorFloatArray[3];
            }
            return color;
        }
        /// <summary>
        /// 将十六位进制颜色码字符串转换到Color类型
        /// </summary>
        /// <param name="hex">需要转换成Color类型的十六位进制的颜色码字符串（格式可以是#RRGGBB或#RRGGBBAA）</param>
        /// <returns>转换后的Color对象</returns>
        public static Color Color_From_HexString(string hex)
        {
            // 移除可能存在的#号
            string cleanHex = hex.Replace("#", "");

            // 默认颜色为黑色
            Color nowColor = Color.black;

            // 处理8位颜色码
            if (cleanHex.Length == 8)
            {
                // 处理8位颜色码（带透明度）
                string rgbPart = cleanHex.Substring(0, 6);
                string alphaPart = cleanHex.Substring(6, 2);

                // 解析RGB部分
                if (ColorUtility.TryParseHtmlString("#" + rgbPart, out nowColor))
                {
                    // 解析透明度部分（0-255转换为0f-1f）
                    byte alphaByte = byte.Parse(alphaPart, System.Globalization.NumberStyles.HexNumber);
                    nowColor.a = alphaByte / 255f;
                }
            }
            // 处理6位颜色码
            else if (cleanHex.Length == 6)
            {
                // 处理6位颜色码（无透明度）
                ColorUtility.TryParseHtmlString("#" + cleanHex, out nowColor);
            }
            else
            {
                Debug.LogWarning("Invalid hex color format. Expected #RRGGBB or #RRGGBBAA.");
            }

            return nowColor;
        }
        /// <summary>
        /// 将HSV颜色转换到Color类型
        /// </summary>
        /// <param name="H">H</param>
        /// <param name="S">S</param>
        /// <param name="V">V</param>
        /// <returns>此方法返回类型为 -> 颜色_Color</returns>
        public static Color Color_From_HSV(float H, float S, float V)
        {
            //  将HSV值标准化到0-1范围内
            H = Mathf.Repeat(H, 1.0f);
            S = Mathf.Clamp01(S);
            V = Mathf.Clamp01(V);

            //  计算色相对应的角位置
            float C = V * S;  //  Chroma
            float HPrime = H * 6.0f;
            float X = C * (1.0f - Mathf.Abs(Mod(HPrime, 2.0f) - 1.0f));
            float m = V - C;
            float r, g, b;

            if (0 <= HPrime && HPrime < 1) { r = C; g = X; b = 0; }
            else if (1 <= HPrime && HPrime < 2) { r = X; g = C; b = 0; }
            else if (2 <= HPrime && HPrime < 3) { r = 0; g = C; b = X; }
            else if (3 <= HPrime && HPrime < 4) { r = 0; g = X; b = C; }
            else if (4 <= HPrime && HPrime < 5) { r = X; g = 0; b = C; }
            else if (5 <= HPrime && HPrime < 6) { r = C; g = 0; b = X; }
            else { r = 0; g = 0; b = 0; }

            //  转换为RGB值
            r = r + m;
            g = g + m;
            b = b + m;

            return new Color(r, g, b, 1.0f);
        }
        static float Mod(float dividend, float divisor)
        {
            return ((dividend % divisor) + divisor) % divisor;
        }
        #endregion

        #region 颜色 -> 字符串
        /// <summary>
        /// 将Color类型转换到十六进制颜色码字符串
        /// </summary>
        /// <param name="color">需要转换成字符串格式的Color类型</param>
        /// <param name="includeAlpha">是否包含透明度通道</param>
        /// <param name="hasPrefixSymbol">True：前缀带有 # 号，False：无 # 号前缀</param>
        /// <returns>十六进制颜色码字符串</returns>
        public static string Color_To_HexColor(Color color, bool includeAlpha = false, bool hasPrefixSymbol = false)
        {
            if (includeAlpha)
            {
                // 使用Unity内置方法处理8位颜色码（包含透明度）
                string hexString = ColorUtility.ToHtmlStringRGBA(color);
                return hasPrefixSymbol ? "#" + hexString : hexString;
            }
            else
            {
                // 使用Unity内置方法处理6位颜色码
                string hexString = ColorUtility.ToHtmlStringRGB(color);
                return hasPrefixSymbol ? "#" + hexString : hexString;
            }
        }
        /// <summary>
        /// 将Color类型转换到字符串
        /// </summary>
        /// <param name="color">填写需要转换成字符串格式的Color类型</param>
        /// <param name="mode">数值范围指定为True则是0-255，False则是0-1</param>
        /// <returns>此方法返回类型为 -> 字符串_String，格式为：R,G,B,A顺序排列的字符串</returns>
        public static string Color_To_String(Color color, bool mode)
        {
            string _color = color.ToString();
            if (mode)
            {
                int r = Mathf.RoundToInt(color.r * 255f);
                int g = Mathf.RoundToInt(color.g * 255f);
                int b = Mathf.RoundToInt(color.b * 255f);

                _color = $"{r},{g},{b},{color.a}";
            }
            else
            {
                _color = _color.Remove(0, 5);
                _color = _color.Remove(_color.Length - 1, _color.Length - (_color.Length - 1)).Trim();
            }
            return _color;
        }
        /// <summary>
        /// 将Color类型转换到自定义符号分隔的完整字符串
        /// </summary>
        /// <param name="nodecolor">填写需要转换成字符串格式的Color类型</param>
        /// <param name="Symbol">指定最后输出的字符串的分隔符号，例如： '|'   ','   '/'   '\'   '''   '-'   '.'   </param>
        /// <returns>此方法返回类型为 -> 字符串_String，格式为：R "自定义分隔符" G "自定义分隔符" B "自定义分隔符" A 顺序排列的字符串</returns>
        public static string Color_To_String(Color color, char Symbol = ',')
        {
            string _color = color.ToString();
            _color = _color.Remove(0, 5);
            _color = _color.Remove(_color.Length - 1, _color.Length - (_color.Length - 1)).Trim();
            string[] _splites = _color.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            string combine = "";
            for (int i = 0; i < _splites.Length; i++)
            {
                _splites[i] = _splites[i].Trim();
                if (i < _splites.Length - 1)
                    combine += _splites[i] + Symbol;
                else
                    combine += _splites[i];
            }
            return combine;
        }
        /// <summary>
        /// 将Color类型转换到字符串数组
        /// </summary>
        /// <param name="nodecolor">填写需要转换成字符串数组格式的Color类型</param>
        /// <returns>此方法返回类型为 -> 字符串_String[]，格式为：字符串数组</returns>
        public static string[] Color_To_StringArray(Color color)
        {
            string _color = color.ToString();
            _color = _color.Remove(0, 5);
            _color = _color.Remove(_color.Length - 1, _color.Length - (_color.Length - 1)).Trim();
            string[] _SpliteColors = _color.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < _SpliteColors.Length; i++)
            {
                _SpliteColors[i] = _SpliteColors[i].Trim();
            }
            return _SpliteColors;
        }
        /// <summary>
        /// 将Color类型转换到浮点数组
        /// </summary>
        /// <param name="nodecolor">填写需要转换成浮点数组格式的Color类型</param>
        /// <returns>此方法返回类型为 -> 浮点数_Float[]，格式为：浮点数组</returns>
        public static float[] Color_To_FloatArray(Color color)
        {
            string _color = color.ToString();
            _color = _color.Remove(0, 5);
            _color = _color.Remove(_color.Length - 1, _color.Length - (_color.Length - 1)).Trim();
            string[] _SpliteColors = _color.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            float[] fs = new float[_SpliteColors.Length];
            for (int i = 0; i < _SpliteColors.Length; i++)
            {
                _SpliteColors[i] = _SpliteColors[i].Trim();
                fs[i] = float.Parse(_SpliteColors[i]);
            }
            return fs;
        }
        #endregion

        #region Uss 增加/移除
        /// <summary>
        /// 设置元素的样式
        /// </summary>
        /// <param name="element">要添加样式的目标元素</param>
        /// <param name="path">样式资源路径（要包含.uss后缀）</param>
        public static StyleSheet ElementStyle_Add(VisualElement element, string path)
        {
            // 读取uss样式
            var uss = util_XGraphEditorUtility.AssetLoad<StyleSheet>(path);
            element.styleSheets.Add(uss);
            return uss;
        }

        /// <summary>
        /// 移除元素的样式
        /// </summary>
        /// <param name="element">要添加样式的目标元素</param>
        /// <param name="path">样式资源路径（要包含.uss后缀）</param>
        public static void ElementStyle_Remove(VisualElement element, string path)
        {
            // 读取uss样式
            var uss = util_XGraphEditorUtility.AssetLoad<StyleSheet>(path);
            element.styleSheets.Remove(uss);
        }
        #endregion

        #region 日期&时间计算
        // 更精确的版本，考虑边界情况
        public static string GetTimeSinceLastSavePrecise(string lastSaveTime)
        {
            string[] formats = { "yyyy-MM-dd  -  HH:mm:ss", "yyyy-MM-dd  -  HH:mm:ss" };

            if (DateTime.TryParseExact(lastSaveTime, formats,
                                      System.Globalization.CultureInfo.InvariantCulture,
                                      System.Globalization.DateTimeStyles.None,
                                      out DateTime savedTime))
            {
                DateTime currentTime = DateTime.Now;
                TimeSpan timeDifference = currentTime - savedTime;

                // 先检查是否超过1天（不同日期）
                if (timeDifference.TotalDays >= 1)
                {
                    int days = (int)timeDifference.TotalDays;
                    return $"{days}天前";
                }
                // 检查是否超过1小时（不同小时）
                else if (timeDifference.TotalHours >= 1)
                {
                    int hours = (int)timeDifference.TotalHours;
                    return $"{hours}小时前";
                }
                // 检查是否超过1分钟
                else if (timeDifference.TotalMinutes >= 1)
                {
                    int minutes = (int)timeDifference.TotalMinutes;
                    return $"{minutes}分钟前";
                }
                // 少于1分钟
                else
                {
                    int seconds = (int)timeDifference.TotalSeconds;
                    return $"{seconds}秒前";
                }
            }
            else
            {
                throw new ArgumentException("无效的时间格式。请使用 'yyyy-MM-dd - HH:mm:ss' 格式");
            }
        }
        #endregion

        #region 元素控制
        /// <summary>
        /// Toggle组件设置
        /// </summary>
        /// <param name="val"></param>
        public static void Element_ToggleField_ValueSet(Toggle toggle, bool val)
        {
            toggle.value = val;
        }
        /// <summary>
        /// IntegerField输入框组件设置
        /// </summary>
        /// <param name="val"></param>
        public static void Element_IntegerField_ValueSet(IntegerField field, int val)
        {
            field.value = val;
        }
        /// <summary>
        /// FloatField输入框组件设置
        /// </summary>
        /// <param name="val"></param>
        public static void Element_FloatField_ValueSet(FloatField field, float val)
        {
            field.value = val;
        }
        /// <summary>
        /// ColorField颜色组件设置
        /// </summary>
        /// <param name="color"></param>
        public static void Element_ColorField_ValueSet(ColorField field, Color color)
        {
            field.value = color;
        }
        /// <summary>
        /// ObjectField物体组件设置
        /// </summary>
        /// <param name="obj"></param>
        public static void Element_ObjectField_ValueSet(ObjectField field, UnityEngine.Object obj)
        {
            field.value = obj;
        }
        /// <summary>
        /// 控件元素 - 文字设置
        /// </summary>
        /// <param name="label"></param>
        /// <param name="text"></param>
        public static void Element_Label_ValueSet(Label label, string text)
        {
            label.text = text;
        }
        /// <summary>
        /// 标签字体大小设置
        /// </summary>
        /// <param name="label"></param>
        /// <param name="size"></param>
        public static void Element_Label_SizeSet(Label label, int size)
        {
            StyleLength fontSize = label.style.fontSize;
            fontSize.value = size;
            label.style.fontSize = fontSize;
        }
        /// <summary>
        /// 标签字体斜体设置
        /// </summary>
        /// <param name="label"></param>
        /// <param name="state"></param>
        public static void Element_Label_ItalicSet(Label label, bool state)
        {
            if (state)
                label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Italic);
            else
            {
                if (label.style.unityFontStyleAndWeight.value == FontStyle.Bold)
                {
                    label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
                }
                else
                {
                    label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Normal);
                }
            }
        }
        /// <summary>
        /// 标签字体粗细设置
        /// </summary>
        /// <param name="label"></param>
        /// <param name="bold"></param>
        public static void Element_Label_BoldSet(Label label, bool bold)
        {
            if (bold)
            {
                if (label.style.unityFontStyleAndWeight.value == FontStyle.Italic)
                {
                    label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.BoldAndItalic);
                }
                else
                {
                    label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Italic);
                }
            }
            else
            {
                if (label.style.unityFontStyleAndWeight.value == FontStyle.Italic)
                {
                    label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.BoldAndItalic);
                }
                else
                {
                    label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Italic);
                }
            }
        }
        /// <summary>
        /// 标签字体样式设置
        /// </summary>
        /// <param name="label"></param>
        /// <param name="style"></param>
        public static void Element_Label_StyleSet(Label label, FontStyle style)
        {
            label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(style);
        }
        /// <summary>
        /// 字体设置
        /// </summary>
        /// <param name="element"></param>
        /// <param name="font"></param>
        public static void Element_Label_FontSet(Label element, Font font)
        {
            element.style.unityFont = new StyleFont(font);
            element.style.unityFontDefinition = new StyleFontDefinition(font);
        }
        /// <summary>
        /// 控制元素 - 可见性
        /// </summary>
        /// <param name="state"></param>
        public static void Element_Visibility_Set(VisualElement element, bool state)
        {
            // 如果为 True 则将 element 容器的可见性设为：Visiblity，即：可见，否则就是不可见：Hidden
            if (state)
            {
                element.style.visibility = Visibility.Visible;
            }
            else
            {
                element.style.visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// 控制元素 - 布局可视模式
        /// </summary>
        /// <param name="state"></param>
        public static void Element_Dispaly_Set(VisualElement element, bool state)
        {
            // 如果为 True 则将 element  的布局模式设为：Flex，即：可见，否则就是：None
            if (state)
            {
                element.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            }
            else
            {
                element.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
            }
        }
        /// <summary>
        /// 控制元素 - 边框颜色设置
        /// </summary>
        /// <param name="element"></param>
        /// <param name="col_top"></param>
        /// <param name="col_bottom"></param>
        /// <param name="col_left"></param>
        /// <param name="col_right"></param>
        public static void Element_BorderColor_Set(VisualElement element, Color col_top, Color col_bottom, Color col_left, Color col_right)
        {
            element.style.borderTopColor = col_top;
            element.style.borderBottomColor = col_bottom;
            element.style.borderLeftColor = col_left;
            element.style.borderRightColor = col_right;
        }
        /// <summary>
        /// 控制元素 - 边框颜色设置
        /// </summary>
        /// <param name="element"></param>
        /// <param name="color"></param>
        public static void Element_BorderColor_Set(VisualElement element, Color color)
        {
            element.style.borderTopColor = color;
            element.style.borderBottomColor = color;
            element.style.borderLeftColor = color;
            element.style.borderRightColor = color;
        }
        /// <summary>
        /// 控制元素 - 背景颜色设置
        /// </summary>
        /// <param name="element"></param>
        /// <param name="color"></param>
        public static void Element_BackgroundColor_Set(VisualElement element, Color color)
        {
            element.style.backgroundColor = color;
        }
        /// <summary>
        /// 控制元素 - 背景图像着色颜色设置
        /// </summary>
        /// <param name="element"></param>
        /// <param name="color"></param>
        public static void Element_BackgroundColorTint_Set(VisualElement element, Color color)
        {
            element.style.unityBackgroundImageTintColor = color;
        }
        /// <summary>
        /// 控制元素 - 颜色设置
        /// </summary>
        /// <param name="element"></param>
        /// <param name="color"></param>
        public static void Element_Color_Set(VisualElement element, Color color)
        {
            element.style.color = color;
        }
        /// <summary>
        /// 控制元素 - 透明度设置
        /// </summary>
        /// <param name="element"></param>
        /// <param name="alpha"></param>
        public static void Element_Opacity_Set(VisualElement element, float alpha)
        {
            element.style.opacity = alpha;
        }
        #endregion

        #region 端口获取
        /// <summary>
        /// 从端口列表中获取指定类型的端口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="portlist"></param>
        /// <returns></returns>
        public static Port GetPort_WithType_OfPortList<T>(List<xGraph_NodePort> portlist)
        {
            Port port = null;
            foreach (var p in portlist)
            {
                if (p is xGraph_NodePort x)
                {
                    if (x.Type == typeof(T))
                    {
                        port = x.Port;
                    }
                }
            }
            return port;
        }
        #endregion

        #region XGraph窗口获取
        /// <summary>
        /// 获取XGraph的主窗口
        /// </summary>
        /// <returns></returns>
        public static xg_Window GetGraphviewWindow()
        {
            return EditorWindow.GetWindow<xg_Window>();
        }
        #endregion

        /// <summary>
        /// 拷贝文本到系统剪贴板
        /// </summary>
        /// <param name="text">要拷贝的文本</param>
        public static void CopyToClipboard(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }

        #region 弹窗提示
        /// <summary>
        /// 弹窗提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="ok"></param>
        public static void DialogMsg(string title, string text, string ok)
        {
            EditorUtility.DisplayDialog(title, text, ok);
        }

        /// <summary>
        /// 弹窗提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="ok"></param>
        /// <param name="cancel"></param>
        public static void DialogMsg(string title, string text, string ok, string cancel = null)
        {
            EditorUtility.DisplayDialog(title, text, ok, cancel);
        }

        /// <summary>
        /// 弹窗提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="ok"></param>
        /// <param name="cancel"></param>
        /// <param name="alt"></param>
        public static void DialogMsg(string title, string text, string ok, string cancel = null, string alt = null)
        {
            EditorUtility.DisplayDialogComplex(title, text, ok, cancel, alt);
        }
        #endregion
    }
}