namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;

    public class VariableTheme
    {
        public string type;
        public string color;
    }

    public class VariableThemesGroup
    {
        public List<VariableTheme> VariableThemes = new List<VariableTheme>();
    }
}