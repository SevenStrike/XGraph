namespace SevenStrikeModules.XGraph
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ActionNode_Asset))]
    public class Editor_ActionNode_Asset : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("这是 NodeAssetRoot 的自定义编辑器");
            GUILayout.Button("ClickedMe");
            EditorGUILayout.Space(10);
            EditorGUILayout.EndVertical();


            EditorGUILayout.Space(10);
            EditorGUILayout.EndHorizontal();

            if (target != null)
                base.OnInspectorGUI();
        }
    }
}