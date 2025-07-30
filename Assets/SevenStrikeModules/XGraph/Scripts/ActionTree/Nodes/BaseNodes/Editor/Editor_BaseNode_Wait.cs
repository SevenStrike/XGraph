namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(BaseNode_Wait))]
    public class BaseNode_WaitEditor : Editor
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
            EditorGUILayout.EndVertical();


            EditorGUILayout.Space(10);
            EditorGUILayout.EndHorizontal();





            //base.OnInspectorGUI();
        }
    }
}