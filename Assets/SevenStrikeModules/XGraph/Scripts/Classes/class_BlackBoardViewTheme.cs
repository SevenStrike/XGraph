namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class VariableTheme
    {
        public string type;
        public string color;
    }

    public class VariableThemesData
    {
        [SerializeField]
        public List<VariableTheme> VariableThemes = new List<VariableTheme>();
    }
}