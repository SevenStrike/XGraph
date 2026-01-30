/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGraph 行为流程图插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using ObjectField = UnityEditor.UIElements.ObjectField;

    public class util_XGraphNodeAssistant : EditorWindow
    {
        #region 控件
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset;
        [SerializeField] private TextField input_prefix;
        [SerializeField] private TextField input_name;
        [SerializeField] private TextField contentField;
        [SerializeField] private TextField pr_action;
        [SerializeField] private TextField pr_editor;
        [SerializeField] private TextField pr_graph;
        [SerializeField] private Label fd_folderName;
        [SerializeField] private Label fd_actionName;
        [SerializeField] private Label fd_graphName;
        [SerializeField] private Button btn_save;
        [SerializeField] private ObjectField obj_icon;
        [SerializeField] private TextField input_nickname;
        [SerializeField] private VisualElement node_icon;
        [SerializeField] private Label node_nickname;
        [SerializeField] private DropdownField drop_types;
        #endregion

        #region 参数
        [SerializeField] private string nodeName = "";
        [SerializeField] private string nickName = "";
        [SerializeField] private string prefix = "";
        [SerializeField] private string iconguid = "";
        [SerializeField] private string xtype = "";
        #endregion

        [MenuItem("Assets/XGraph/N 节点扩展助手", false, 100)]
        public static void CreateActionGraphNode()
        {
            util_XGraphNodeAssistant wnd = GetWindow<util_XGraphNodeAssistant>(true);
            wnd.titleContent = new GUIContent("节点扩展助手");
            wnd.minSize = new Vector2(350, 800);
            wnd.maxSize = wnd.minSize;
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // 读取并克隆uxml布局到 root 布局
            var visual_window = util_XGraphEditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_XGraphNodeAssistant.uxml");
            visual_window.CloneTree(root);

            // 先获取所有控件引用
            input_prefix = root.Q<TextField>("input_prefix");
            input_name = root.Q<TextField>("input_name");
            contentField = root.Q<TextField>("contentField");
            pr_action = root.Q<TextField>("pr_action");
            pr_editor = root.Q<TextField>("pr_editor");
            pr_graph = root.Q<TextField>("pr_graph");
            fd_folderName = root.Q<VisualElement>("fd_folderName")?.Q<Label>("name");
            fd_actionName = root.Q<VisualElement>("fd_actionName")?.Q<Label>("name");
            fd_graphName = root.Q<VisualElement>("fd_graphName")?.Q<Label>("name");
            btn_save = root.Q<Button>("btn_save");
            obj_icon = root.Q<ObjectField>("obj_icon");
            input_nickname = root.Q<TextField>("input_nickname");
            node_icon = root.Q<VisualElement>("node_icon");
            node_nickname = root.Q<Label>("node_nickname");
            drop_types = root.Q<DropdownField>("drop_types");

            // 注册事件回调
            input_prefix.RegisterValueChangedCallback(input_prefix_changed);
            input_name.RegisterValueChangedCallback(input_name_changed);
            obj_icon.objectType = typeof(Texture2D);
            obj_icon.RegisterValueChangedCallback(obj_icon_changed);
            input_nickname.RegisterValueChangedCallback(input_nickname_changed);
            drop_types.RegisterValueChangedCallback(drop_types_changed);
            btn_save.clicked += Btn_save_clicked;

            // 设置下拉框选项
            drop_types.choices = new List<string>
            {
                "起始",
                "合成",
                "属性",
                "结束"
            };
            drop_types.value = drop_types.choices[LoadPrefsData_Int("xtype")];

            setxtype(drop_types.value);

            // 最后设置初始值（触发回调）
            input_prefix.value = LoadPrefsData_String("prefix");
            input_name.value = LoadPrefsData_String("nodeName");
            input_nickname.value = LoadPrefsData_String("nickName");

            // 确保数据正确更新
            update_Data();
        }

        #region 控件回调
        private void input_name_changed(ChangeEvent<string> evt)
        {
            // 使用正则表达式移除所有非小写字母和非下划线的字符
            string filtered = Regex.Replace(evt.newValue, @"[^a-zA-Z0-9_]", "");
            // 如果值被修改，更新TextField但不触发回调
            if (filtered != evt.newValue)
            {
                var textField = (TextField)evt.target;
                textField.SetValueWithoutNotify(filtered);
            }

            nodeName = filtered;
            update_Data();

            SavePrefsData_String("nodeName", filtered);
        }
        private void input_prefix_changed(ChangeEvent<string> evt)
        {
            // 使用正则表达式移除所有非小写字母和非下划线的字符
            string filtered = Regex.Replace(evt.newValue, @"[^a-zA-Z0-9_]", "");
            // 如果值被修改，更新TextField但不触发回调
            if (filtered != evt.newValue)
            {
                var textField = (TextField)evt.target;
                textField.SetValueWithoutNotify(filtered);
            }

            prefix = filtered;
            update_Data();

            SavePrefsData_String("prefix", filtered);
        }
        private void input_nickname_changed(ChangeEvent<string> evt)
        {
            nickName = evt.newValue;
            update_Data();

            SavePrefsData_String("nickName", nickName);
        }
        private void obj_icon_changed(ChangeEvent<UnityEngine.Object> evt)
        {
            node_icon.style.backgroundImage = new StyleBackground(Background.FromTexture2D(evt.newValue as Texture2D));
            iconguid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(evt.newValue)).ToString();
            update_Data();
        }
        private void drop_types_changed(ChangeEvent<string> evt)
        {

            setxtype(evt.newValue);
            int currentIndex = drop_types.choices.IndexOf(drop_types.value);
            SavePrefsData_Int("xtype", currentIndex);
        }
        private void Btn_save_clicked()
        {
            string scr_action = ScriptMaker_CreateContent_Action();
            string scr_graph = ScriptMaker_CreateContent_Graph();

            string path = ScriptMaker_SavePath();
            if (string.IsNullOrEmpty(path))
                return;

            if (!AssetDatabase.IsValidFolder($"{path}/{nodeName}"))
            {
                string path_root = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(path, $"{prefix}_{nodeName}"));
                string path_editor = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(path_root, "Editor"));
                Debug.Log(path_root);
                Debug.Log(path_editor);

                ScriptMaker_Create(path_root, $"action_{prefix}_{nodeName}", scr_action);
                ScriptMaker_Create(path_editor, $"graph_{prefix}_{nodeName}", scr_graph);

                // 刷新资源数据库
                AssetDatabase.Refresh();

                Debug.Log($"扩展节点 {nodeName} 结构已创建！");
            }
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 设置生成的扩展节点类型
        /// </summary>
        /// <param name="type"></param>
        private void setxtype(string type)
        {
            switch (type)
            {
                case "起始":
                    xtype = "xAction_Start";
                    break;
                case "合成":
                    xtype = "xAction_Composite";
                    break;
                case "属性":
                    xtype = "xAction_Property";
                    break;
                case "结束":
                    xtype = "xAction_End";
                    break;
            }
        }
        #endregion

        #region 数据刷新
        /// <summary>
        /// 根据输入的参数刷新控件显示内容
        /// </summary>
        private void update_Data()
        {
            field_content_set(pr_action, $"action_{prefix}_{nodeName}");
            field_content_set(pr_graph, $"graph_{prefix}_{nodeName}");

            update_StructContent();

            label_content_set(node_nickname, string.IsNullOrEmpty(nickName) ? nodeName : nickName);

            label_content_set(fd_folderName, $"{prefix}_{nodeName}");
            label_content_set(fd_actionName, $"<b><color=#FFBF23>action_</color></b>{prefix}_{nodeName}");
            label_content_set(fd_graphName, $"<b><color=#FFBF23>graph_</color></b>{prefix}_{nodeName}");
        }
        /// <summary>
        /// 根据输入的参数刷新节点配置数据
        /// </summary>
        private void update_StructContent()
        {
            string content = $"{{\r\n    \"name\": \"{(string.IsNullOrEmpty(nickName) ? nodeName : nickName)}\",\r\n    \"prefixNamespace\": \"SevenStrikeModules.XGraph\",\r\n    \"prefixClass\": \"action_{prefix}_\",\r\n    \"actionNodeType\": \"{nodeName}\",\r\n    \"icon\": \"{(obj_icon.value == null ? "0a4daa5b210366743a06c03f7d6ff078" : iconguid)}\",\r\n    \"visualNodeType\": \"graph_{prefix}_{nodeName}\"\r\n}}";
            field_content_set(contentField, $"{content}");
        }
        #endregion

        #region 控件控制
        /// <summary>
        /// 输入框控件输入内容
        /// </summary>
        /// <param name="field"></param>
        /// <param name="content"></param>
        private void field_content_set(TextField field, string content)
        {
            if (field != null)
            {
                field.value = content;
            }
        }
        /// <summary>
        /// 标签控件内容
        /// </summary>
        /// <param name="label"></param>
        /// <param name="content"></param>
        private void label_content_set(Label label, string content)
        {
            if (label != null)
            {
                label.text = content;
            }
        }
        #endregion

        #region 生成扩展节点脚本结构
        /// <summary>
        /// 生成行为代码内容
        /// </summary>
        /// <returns></returns>
        private string ScriptMaker_CreateContent_Action()
        {
            string content = "";

            content = $@"/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGraph 行为流程图插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
namespace SevenStrikeModules.XGraph
{{    {fragment_using_unityengine()}    
    public class action_{prefix}_{nodeName} : {xtype}
    {{
        /// <summary>
        /// 节点执行
        /// </summary>
        public override void Execute()
        {{
            base.Execute();
        }}{fragment_PropertyNodeContent()}
       
        /// <summary>
        /// 克隆该节点时确保改脚本独立变量正确克隆
        /// </summary>
        /// <returns></returns>
        public override xAction_Base Clone()
        {{
            // 调用基类的Clone方法
            action_{prefix}_{nodeName} clone = base.Clone() as action_{prefix}_{nodeName};

            // 复制派生类特有的字段
            if (clone != null)
            {{
                // 此处可以将原始数据赋值到克隆数据
                // clone.property = this.property;
            }}

            return clone;
        }}
    }}
}}";
            return content;
        }
        /// <summary>
        /// 生成节点代码内容
        /// </summary>
        /// <returns></returns>
        private string ScriptMaker_CreateContent_Graph()
        {
            string content = "";

            content = $@"/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGraph 行为流程图插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
namespace SevenStrikeModules.XGraph
{{
{fragment_using_UnityEditor_Experimental_GraphView()}    using UnityEngine;{fragment_using_UnityEngine_UIElements()}

    public class graph_{prefix}_{nodeName} : xNode_{xtype.Split('_')[1]}
    {{
        action_{prefix}_{nodeName} {nodeName};

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {{
            base.Initialize(graphView, pos, data);

            {nodeName} = base.{xtype.Split('_')[1].ToLower()} as action_{prefix}_{nodeName};
        }}{fragment_NodeDraw()}{fragment_Callbacks()}{fragment_OverrideInspectorGUI()}
    }}
}}";
            return content;
        }
        /// <summary>
        /// 选择保存路径
        /// </summary>
        /// <returns></returns>
        private string ScriptMaker_SavePath()
        {
            // 打开文件夹选择对话框，从Assets目录开始
            string selectedPath = EditorUtility.SaveFolderPanel("选择保存文件夹", "Assets", "");

            // 将绝对路径转换为相对于项目根目录的路径
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    return "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
                else
                {
                    Debug.LogWarning("请选择Assets目录或其子目录下的文件夹");
                    return null;
                }
            }

            return null;
        }
        /// <summary>
        /// 创建代码文件
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="ScriptName"></param>
        /// <param name="ScriptContent"></param>
        private void ScriptMaker_Create(string folderPath, string ScriptName, string ScriptContent)
        {
            string scriptPath = Path.Combine(folderPath, ScriptName + ".cs");

            // 创建脚本文件
            File.WriteAllText(scriptPath, ScriptContent);
        }
        #endregion

        #region Fragment 代码片段
        private string fragment_NodeDraw()
        {
            string content = "";

            if (xtype != "xAction_Property")
            {
                content = $@"

        #region 节点绘制
        public override xNode_Base Draw()
        {{
            return base.Draw();
        }}
        #endregion";
            }
            return content;
        }
        private string fragment_PropertyNodeContent()
        {
            if (xtype != "xAction_Property")
                return null;

            string content = "";
            content = $@"

        /// <summary>
        /// 初始化属性变量列表
        /// </summary>
        public override void Propertys_Initialize()
        {{
            base.Propertys_Initialize();
        }}

        /// <summary>
        /// 更新属性
        /// </summary>
        public override void Propertys_Update()
        {{
            base.Propertys_Update();
        }}";

            return content;
        }
        private string fragment_Callbacks()
        {
            if (xtype == "xAction_Property")
                return null;

            string content = "";
            content = $@"

        #region 重写 - 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {{
            base.On_VariablesValue_Changed();
        }}
        #endregion";

            return content;
        }
        private string fragment_OverrideInspectorGUI()
        {
            if (xtype == "xAction_Property")
                return null;

            string content = "";
            content = $@"

        #region 重写 - 绘制Inspector
        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name=""root""></param>
        public override Foldout ins_Folder_Extensions(VisualElement root)
        {{
            Foldout fold = base.ins_Folder_Extensions(root);
            return fold;
        }}
        #endregion";

            return content;
        }
        private string fragment_using_unityengine()
        {
            if (xtype == "xAction_Property")
                return null;

            string content = "";
            content = $@"
    using UnityEngine;
";
            return content;
        }
        private string fragment_using_UnityEditor_Experimental_GraphView()
        {
            if (xtype == "xAction_Property")
                return null;

            string content = "";
            content = $@"    using UnityEditor.Experimental.GraphView;
";
            return content;
        }
        private string fragment_using_UnityEngine_UIElements()
        {
            if (xtype == "xAction_Property")
                return null;

            string content = "";
            content = $@"
    using UnityEngine.UIElements;";
            return content;
        }
        #endregion

        #region 数据持久化
        private void SavePrefsData_String(string key, string data)
        {
            EditorPrefs.SetString(key, data);
        }
        private string LoadPrefsData_String(string key)
        {
            return EditorPrefs.GetString(key, "");
        }
        private void SavePrefsData_Int(string key, int value)
        {
            EditorPrefs.SetInt(key, value);
        }
        private int LoadPrefsData_Int(string key)
        {
            return EditorPrefs.GetInt(key, 0);
        }
        #endregion
    }
}