namespace SevenStrikeModules.XGraph
{
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(INode_Wait))]
    public class Editor_INode_Wait : Editor
    {
        //GUIStyle style_text 

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("这是 BaseNode_Wait 的自定义编辑器");
            GUILayout.Button("ClickedMe");
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Open Color Picker Programmatically"))
            {
                OpenColorPicker(serializedObject.FindProperty("themeColor").colorValue);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
            EditorGUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }

        private void OpenColorPicker(Color initialColor)
        {
            // 尝试调用 EditorUtility 方法（不一定所有版本都支持）
            var editorUtilityType = typeof(EditorUtility);
            var showColorPickerMethod = editorUtilityType.GetMethod(
                "ShowColorPicker",
                BindingFlags.Static | BindingFlags.Public
            );

            if (showColorPickerMethod != null)
            {
                showColorPickerMethod.Invoke(null, new object[] { serializedObject.FindProperty("themeColor").colorValue });
            }
        }
    }
}