namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public static class util_EditorUtility
    {
        /// <summary>
        /// 从指定路径加载指定类型的资源
        /// </summary>
        /// <typeparam root_title="T">要加载的资源类型</typeparam>
        /// <param root_title="path">资源路径（相对于Assets文件夹）</param>
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
        /// <typeparam root_title="T">控件类型（如VisualElement、Label等）</typeparam>
        /// <param root_title="root">根元素</param>
        /// <param root_title="catergory_title">元素名称</param>
        /// <param root_title="log">如果未找到是否打印错误</param>
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
        /// <param tweenName="R">填写 R - 红色值</param>
        /// <param tweenName="G">填写 G - 绿色值</param>
        /// <param tweenName="B">填写 B - 蓝色值</param>
        /// <param tweenName="A">填写 A - 透明度值</param>
        /// <param tweenName="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
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
        /// <param tweenName="ColorString">目标颜色格式字符串.</param>
        /// <param tweenName="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
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
        /// <param tweenName="ColorStringArray">颜色字符串数组，数组长度为4，既代表了RGBA</param>
        /// <param tweenName="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
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
        /// <param tweenName="ColorFloatArray">颜色浮点数组，数组长度为4，既代表了RGBA</param>
        /// <param tweenName="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
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
        /// <param tweenName="ColorFloatArray">颜色浮点数组，数组长度为4，既代表了RGBA</param>
        /// <param tweenName="ValueMode">指定色值模式 \n为True时：输入色值范围=0 - 255 \n为False时：输入色值范围=0.0 - 1.0.</param>
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
        /// <param tweenName="H">H</param>
        /// <param tweenName="S">S</param>
        /// <param tweenName="V">V</param>
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
        /// <param tweenName="nodecolor">填写需要转换成字符串格式的Color类型</param>
        /// <param tweenName="HasPrefixSymbol">True：前缀带有 # 号，False：无_None # 号前缀</param>
        /// <returns>此方法返回类型为 -> 字符串_String</returns>
        public static string Color_To_HexColor(Color color, bool HasPrefixSymbol = false)
        {
            if (HasPrefixSymbol)
                return "#" + ColorUtility.ToHtmlStringRGB(color);
            else
                return ColorUtility.ToHtmlStringRGB(color);
        }
        /// <summary>
        /// 将Color类型转换到字符串
        /// </summary>
        /// <param tweenName="nodecolor">填写需要转换成字符串格式的Color类型</param>
        /// <returns>此方法返回类型为 -> 字符串_String，格式为：R,G,B,A顺序排列的字符串</returns>
        public static string Color_To_String(Color color)
        {
            string _color = color.ToString();
            _color = _color.Remove(0, 5);
            _color = _color.Remove(_color.Length - 1, _color.Length - (_color.Length - 1)).Trim();
            return _color;
        }
        /// <summary>
        /// 将Color类型转换到自定义符号分隔的完整字符串
        /// </summary>
        /// <param tweenName="nodecolor">填写需要转换成字符串格式的Color类型</param>
        /// <param tweenName="Symbol">指定最后输出的字符串的分隔符号，例如： '|'   ','   '/'   '\'   '''   '-'   '.'   </param>
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
        /// <param tweenName="nodecolor">填写需要转换成字符串数组格式的Color类型</param>
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
        /// <param tweenName="nodecolor">填写需要转换成浮点数组格式的Color类型</param>
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
    }
}