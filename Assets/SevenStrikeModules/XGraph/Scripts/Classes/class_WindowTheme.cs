namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;

    public class WindowThemeParams
    {
        public string solution;
        public string col_bg;
        public string col_grid;
        public string col_thickLine;
        public string col_customimage;
        public string col_theme;
        public string col_blackboard_bg;
        public string col_inspector_bg;
        public float val_griddistance;
        public int val_thicklines;
    }

    public class WindowThemeList
    {
        public List<WindowThemeParams> Themes = new List<WindowThemeParams>();
    }
}