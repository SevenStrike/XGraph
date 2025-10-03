namespace SevenStrikeModules.XGraph
{
    using UnityEditor;

    public static class util_XGraphmenuItems
    {
        #region 快捷菜单
        [MenuItem("Assets/XGraph/C 清空所有子级资源", priority = 10, validate = true)]
        private static bool validate_util_ClearChildNodes()
        {
            // 获取当前选中的对象
            var objs = Selection.objects;
            if (objs != null && objs.Length > 0)
            {
                foreach (var item in objs)
                {
                    // 只要有一个有效的 ActionNode_Asset，菜单就可用
                    if (item is ActionNode_Asset asset && asset != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        [MenuItem("Assets/XGraph/C 清空所有子级资源")]
        public static void util_ClearChildNodes()
        {
            // 获取当前选中的对象
            var objs = Selection.objects;
            if (objs != null && objs.Length > 0)
            {
                foreach (var item in objs)
                {
                    if (item is ActionNode_Asset asset)
                    {
                        asset.Clear();
                    }
                }
            }
        }
        [MenuItem("Assets/XGraph/R 移除选中的行为资源", priority = 20, validate = true)]
        private static bool validate_util_Clear_TargetChildNode()
        {
            // 获取当前选中的对象
            var objs = Selection.objects;
            if (objs != null && objs.Length > 0)
            {
                foreach (var item in objs)
                {
                    // 只要有一个有效的 ActionNode_Asset，菜单就可用
                    if (item is ActionNode_Asset asset && asset != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        [MenuItem("Assets/XGraph/R 移除选中的行为资源")]
        public static void util_Clear_TargetChildNode()
        {
            // 获取当前选中的对象
            var objs = Selection.objects;
            if (objs != null && objs.Length > 0)
            {
                foreach (var item in objs)
                {
                    if (item is ActionNode_Asset asset)
                    {
                        asset.Clear();
                    }
                }
            }
        }
        [MenuItem("Assets/XGraph/F 复位编辑器黑板和属性面板配置", priority = 80)]
        public static void util_ClearGraphWindowConfigs()
        {
            EditorPrefs.DeleteKey("XGraph_InspectorViewPosition");
            EditorPrefs.DeleteKey("XGraph_InspectorViewSize");
            EditorPrefs.DeleteKey("XGraph_InspectorViewDisplay");
            EditorPrefs.DeleteKey("XGraph_BlackBoardViewPosition");
            EditorPrefs.DeleteKey("XGraph_BlackBoardViewSize");
            EditorPrefs.DeleteKey("XGraph_BlackBoardViewDisplay");
        }
        #endregion
    }
}