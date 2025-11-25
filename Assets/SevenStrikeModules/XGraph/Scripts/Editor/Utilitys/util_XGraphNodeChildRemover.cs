namespace SevenStrikeModules.XGraph
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class util_XGraphNodeChildRemover : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset;

        [MenuItem("Assets/Create/XGraph/Z 子节点移除助手", false, -100)]
        public static void CreateActionGraphNode()
        {
            util_XGraphNodeChildRemover wnd = GetWindow<util_XGraphNodeChildRemover>();
            wnd.titleContent = new GUIContent("XGraphNodeChildRemover");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // 读取并克隆uxml布局到 root 布局
            var visual_window = util_XGraphEditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_XGraphNodeChildRemover.uxml");
            visual_window.CloneTree(root);
        }

        private string SavePath()
        {
            // 打开文件夹选择对话框，默认从当前项目的Assets文件夹开始[citation:5]
            return EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
        }

        private void CreateScript(string ScriptName, string namespaceName)
        {
            // 获取选中的文件夹路径
            string folderPath = "Assets";
            if (Selection.activeObject != null)
            {
                folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    folderPath = Path.GetDirectoryName(folderPath);
                }
            }

            string scriptPath = Path.Combine(folderPath, ScriptName + ".cs");

            // 生成脚本内容
            string scriptContent = GenerateScriptContent(ScriptName, namespaceName);

            // 创建脚本文件
            File.WriteAllText(scriptPath, scriptContent);

            // 刷新资源数据库
            AssetDatabase.Refresh();

            Debug.Log($"脚本已创建: {scriptPath}");
        }

        private string GenerateScriptContent(string className, string namespaceName = "")
        {
            string content = "";

            if (!string.IsNullOrEmpty(namespaceName))
            {
                content += $@"using UnityEngine;
                namespace {namespaceName}
                {{
                    public class {className} : MonoBehaviour
                    {{
                        // Start is called before the first frame update
                        void Start()
                        {{
                            
                        }}
                
                        // Update is called once per frame
                        void Update()
                        {{
                            
                        }}
                    }}
                }}";
            }
            else
            {
                content = $@"using UnityEngine;
                public class {className} : MonoBehaviour
                {{
                    // Start is called before the first frame update
                    void Start()
                    {{
                        
                    }}
                
                    // Update is called once per frame
                    void Update()
                    {{
                        
                    }}
                }}";
            }

            return content;
        }
    }
}