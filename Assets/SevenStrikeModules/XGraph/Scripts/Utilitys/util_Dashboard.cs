namespace SevenStrikeModules.XGraph
{
    using UnityEngine;

    public static class util_Dashboard
    {
        #region 主题色
        public static Color Theme_Primary { get; set; } = GetHexString("#3BFE9B");
        public static Color Theme_Secondary { get; set; } = GetHexString("#1E1E1E");
        public static Color Theme_Part { get; set; } = GetHexString("#d2d2d2");
        #endregion

        #region 公共路径
        public static string path_Temp = "Assets/SevenStrikeModules/XGraph/Temp/";
        public static string path_Root = "Assets/SevenStrikeModules/XGraph/";
        public static string path_Config = "Assets/SevenStrikeModules/XGraph/Config/";
        public static string path_Editor = "Assets/SevenStrikeModules/XGraph/Scripts/Editor/";
        public static string path_Node = "Assets/SevenStrikeModules/XGraph/Scripts/Editor/Node/";
        public static string path_Utility = "Assets/SevenStrikeModules/XGraph/Scripts/Editor/Utilitys/";
        public static string path_GUI = "Assets/SevenStrikeModules/XGraph/GUI/";
        public static string path_Uss = "Assets/SevenStrikeModules/XGraph/GUI/Uss/";
        public static string path_Uxml = "Assets/SevenStrikeModules/XGraph/GUI/Uxml/";
        public static string path_Textures = "Assets/SevenStrikeModules/XGraph/Textures/";
        public static string path_Fonts = "Assets/SevenStrikeModules/XGraph/Fonts/";

        #region 路径获取  
        /// <summary>
        /// 获取XGraph Root路径，根目录：SevenStrikeModules/XGraph/
        /// </summary>
        /// <returns></returns>
        public static string GetPath_Root()
        {
            return path_Root;
        }
        /// <summary>
        /// 获取XGraph Config路径，根目录：SevenStrikeModules/XGraph/Config/
        /// </summary>
        /// <returns></returns>
        public static string GetPath_Config()
        {
            return path_Config;
        }
        /// <summary>
        /// 获取XGraph Temp路径，根目录：SevenStrikeModules/XGraph/Temp/
        /// </summary>
        /// <returns></returns>
        public static string GetPath_Temp()
        {
            return path_Temp;
        }
        /// <summary>
        /// 获取XGraph Editor路径，根目录：SevenStrikeModules/XGraph/Editor/
        /// </summary>
        /// <returns></returns>
        public static string GetPath_Editor()
        {
            return path_Editor;
        }
        /// <summary>
        /// 获取XGraph GUI路径，根目录：SevenStrikeModules/XGraph/GUI/
        /// </summary>
        /// <returns></returns>
        public static string GetPath_GUI()
        {
            return path_GUI;
        }
        /// <summary>
        /// 获取XGraph GUI/Uss路径，根目录：SevenStrikeModules/XGraph/GUI/Uss/
        /// </summary>
        /// <returns></returns>
        public static string GetPath_GUI_Uss()
        {
            return path_Uss;
        }
        /// <summary>
        /// 获取XGraph GUI/Uxml路径，根目录：SevenStrikeModules/XGraph/GUI/Uxml/
        /// </summary>
        /// <returns></returns>
        public static string GetPath_GUI_Uxml()
        {
            return path_Uxml;
        }
        /// <summary>
        /// 获取XGraph Node路径，根目录：SevenStrikeModules/XGraph/Node/
        /// </summary>
        /// <returns></returns>
        public static string GetPath_Node()
        {
            return path_Node;
        }
        /// <summary>
        /// 获取XGraph Utility路径，根目录：SevenStrikeModules/XGraph/Utility/
        /// </summary>
        /// <returns></returns>
        public static string GetPath_Utility()
        {
            return path_Utility;
        }
        /// <summary>
        /// 获取XGraph Textures路径，根目录：SevenStrikeModules/XGraph/Textures/
        /// </summary>
        /// <returns></returns>
        public static string GetPath_Textures()
        {
            return path_Textures;
        }
        /// <summary>
        /// 获取XGraph Fonts路径，根目录：SevenStrikeModules/XGraph/Fonts/
        /// </summary>
        /// <returns></returns>
        public static string GetPath_Fonts()
        {
            return path_Fonts;
        }
        #endregion
        #endregion

        /// <summary>
        /// 将十六位进制颜色码字符串转换到Color类型
        /// </summary>
        /// <param tweenName="hex">填写需要转换成Color类型的十六位进制的颜色码字符串（请忽略颜色代码开头的 #）</param>
        /// <returns>此方法返回类型为 -> 颜色_Color</returns>
        public static Color GetHexString(string hex)
        {
            Color nowColor = Color.black;

            bool HasPrefix = false;
            for (int i = 0; i < hex.Length; i++)
            {
                if (hex[i] == '#')
                    HasPrefix = true;
            }
            if (HasPrefix)
                ColorUtility.TryParseHtmlString(hex, out nowColor);
            else
                ColorUtility.TryParseHtmlString("#" + hex, out nowColor);
            return nowColor;
        }
    }
}