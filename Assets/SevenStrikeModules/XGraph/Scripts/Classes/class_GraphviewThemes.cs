namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    /// <summary>
    /// Graphview 背景主题样式参数类
    /// </summary>
    public class GraphviewGridBackgroundThemes
    {
        public Color bgcolor = new Color(0.1647059f, 0.1647059f, 0.1647059f, 0.6862745f);
        public Color gridcolor = new Color(0.5803922f, 0.5803922f, 0.5803922f, 0.04705882f);
        public Color customimagecolor = new Color(0.5607843f, 0.5607843f, 0.5607843f, 0.9294118f);
        public Color thickLinecolor = new Color(0.5529412f, 0.5529412f, 0.5529412f, 0f);
        public Color themecolor = new Color(0.2313726f, 0.9882353f, 0.6f, 1);
        public Color blackboard_bgcolor = new Color(0.2705882f, 0.2705882f, 0.2705882f, 0.8784314f);
        public Color inspector_bgcolor = new Color(0.2705882f, 0.2705882f, 0.2705882f, 0.8784314f);
        public bool InvertBgGradient;
        public float spacing = 18;
        public int thicklines = 18;
        public Texture2D customimage;

        public GraphviewGridBackgroundThemes() { }

        public GraphviewGridBackgroundThemes Clone()
        {
            GraphviewGridBackgroundThemes t = new GraphviewGridBackgroundThemes();
            t.bgcolor = bgcolor;
            t.gridcolor = gridcolor;
            t.thickLinecolor = thickLinecolor;
            t.blackboard_bgcolor = blackboard_bgcolor;
            t.inspector_bgcolor = inspector_bgcolor;
            t.spacing = spacing;
            t.thicklines = thicklines;
            t.customimagecolor = customimagecolor;
            t.customimage = customimage;
            t.themecolor = themecolor;
            t.InvertBgGradient = InvertBgGradient;
            return t;
        }
    }

    [Serializable]
    /// <summary>
    /// Graphview 选择框主题样式参数类
    /// </summary>
    public class GraphviewRectangleSelectorThemes
    {
        public Color rectangleSelectorLineColor = new Color(1, 1, 1, 0.6f);
        public int segments = 4;
        public bool displayCoordinate = false;

        public GraphviewRectangleSelectorThemes() { }

        public GraphviewRectangleSelectorThemes Clone()
        {
            GraphviewRectangleSelectorThemes t = new GraphviewRectangleSelectorThemes();
            t.rectangleSelectorLineColor = rectangleSelectorLineColor;
            t.segments = segments;
            t.displayCoordinate = displayCoordinate;
            return t;
        }
    }

    [Serializable]
    /// <summary>
    /// 编组主题结构
    /// </summary>
    public class ThemeData_Group
    {
        public string solution = "默认";
        public string title_bg_color = "#3C725D";
        public string title_text_color = "#FFFFFF";
        public string content_bg_color = "#DBDBDB1A";
        public string logo_color = "#ffffff";

        public ThemeData_Group() { }

        public ThemeData_Group(string solution, string bg_color, string text_color, string content_bg_color, string logo_color)
        {
            this.solution = solution;
            this.title_bg_color = bg_color;
            this.title_text_color = text_color;
            this.content_bg_color = content_bg_color;
            this.logo_color = logo_color;
        }
    }

    [Serializable]
    /// <summary>
    /// 节点主题结构
    /// </summary>
    public class ThemeData_Node
    {
        public string solution = "默认";
        public string nodecolor = "#747474";

        public ThemeData_Node() { }

        public ThemeData_Node(string solution, string nodecolor)
        {
            this.solution = solution;
            this.nodecolor = nodecolor;
        }
    }

    [Serializable]
    /// <summary>
    /// 主题列表
    /// </summary>
    public class ThemesList
    {
        /// <summary>
        /// ThemeData_Group 颜色集
        /// </summary>
        public List<ThemeData_Group> Group = new List<ThemeData_Group>();
        /// <summary>
        /// ThemeData_Node 颜色集
        /// </summary>
        public List<ThemeData_Node> Node = new List<ThemeData_Node>();
    }
}