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
        [SerializeField] private TextField input_suffix;
        [SerializeField] private TextField input_name;
        [SerializeField] private TextField contentField;
        [SerializeField] private TextField pr_action;
        [SerializeField] private TextField pr_editor;
        [SerializeField] private TextField pr_graph;
        [SerializeField] private Label fd_folderName;
        [SerializeField] private Label fd_actionName;
        [SerializeField] private Label fd_editorName;
        [SerializeField] private Label fd_graphName;
        [SerializeField] private Button btn_save;
        [SerializeField] private ObjectField obj_icon;
        [SerializeField] private TextField input_nickname;
        [SerializeField] private VisualElement node_icon;
        [SerializeField] private Label node_nickname;
        [SerializeField] private DropdownField drop_types;
        #endregion

        #region 参数
        string nodeName = "";
        string nickName = "";
        string surffix = "";
        string iconguid = "";
        string xtype = "";
        #endregion

        [MenuItem("Assets/Create/XGraph/J 节点扩展助手", false, -100)]
        public static void CreateActionGraphNode()
        {
            util_XGraphNodeAssistant wnd = GetWindow<util_XGraphNodeAssistant>(true);
            wnd.titleContent = new GUIContent("XGraphNodeAssistant");
            wnd.minSize = new Vector2(350, 800);
            wnd.maxSize = wnd.minSize;
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // 读取并克隆uxml布局到 root 布局
            var visual_window = util_XGraphEditorUtility.AssetLoad<VisualTreeAsset>($"{util_Dashboard.GetPath_GUI_Uxml()}uxml_XGraphNodeAssistant.uxml");
            visual_window.CloneTree(root);

            input_suffix = root.Q<TextField>("input_suffix");
            input_suffix.RegisterValueChangedCallback(input_suffix_changed);

            input_name = root.Q<TextField>("input_name");
            input_name.RegisterValueChangedCallback(input_name_changed);

            contentField = root.Q<TextField>("contentField");
            pr_action = root.Q<TextField>("pr_action");
            pr_editor = root.Q<TextField>("pr_editor");
            pr_graph = root.Q<TextField>("pr_graph");

            fd_folderName = root.Q<VisualElement>("fd_folderName").Q<Label>("name");
            fd_actionName = root.Q<VisualElement>("fd_actionName").Q<Label>("name");
            fd_editorName = root.Q<VisualElement>("fd_editorName").Q<Label>("name");
            fd_graphName = root.Q<VisualElement>("fd_graphName").Q<Label>("name");

            btn_save = root.Q<Button>("btn_save");
            btn_save.clicked += Btn_save_clicked;

            obj_icon = root.Q<ObjectField>("obj_icon");
            obj_icon.objectType = typeof(Texture2D);
            obj_icon.RegisterValueChangedCallback(obj_icon_changed);

            input_nickname = root.Q<TextField>("input_nickname");
            input_nickname.RegisterValueChangedCallback(input_nickname_changed);

            node_icon = root.Q<VisualElement>("node_icon");
            node_nickname = root.Q<Label>("node_nickname");

            drop_types = root.Q<DropdownField>("drop_types");
            drop_types.RegisterValueChangedCallback(drop_types_changed);
            // 添加选项
            drop_types.choices = new List<string>
            {
                "起始",
                "合成",
                "等待",
                "属性",
                "结束"
            };
            drop_types.value = drop_types.choices[0];
            setxtype(drop_types.value);
        }

        #region 控件回调
        private void input_name_changed(ChangeEvent<string> evt)
        {
            // 使用正则表达式移除所有非小写字母和非下划线的字符
            string filtered = Regex.Replace(evt.newValue, @"[^a-zA-Z_]", "");
            // 如果值被修改，更新TextField但不触发回调
            if (filtered != evt.newValue)
            {
                var textField = (TextField)evt.target;
                textField.SetValueWithoutNotify(filtered);
            }

            nodeName = filtered;
            update_Data();
        }
        private void input_suffix_changed(ChangeEvent<string> evt)
        {
            // 使用正则表达式移除所有非小写字母和非下划线的字符
            string filtered = Regex.Replace(evt.newValue, @"[^a-zA-Z_]", "");
            // 如果值被修改，更新TextField但不触发回调
            if (filtered != evt.newValue)
            {
                var textField = (TextField)evt.target;
                textField.SetValueWithoutNotify(filtered);
            }

            surffix = filtered;
            update_Data();
        }
        private void input_nickname_changed(ChangeEvent<string> evt)
        {
            nickName = evt.newValue;
            update_Data();
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
        }
        private void Btn_save_clicked()
        {
            string scr_action = ScriptMaker_CreateContent_Action();
            string scr_editor = ScriptMaker_CreateContent_Editor();
            string scr_graph = ScriptMaker_CreateContent_Graph();

            string path = ScriptMaker_SavePath();
            if (string.IsNullOrEmpty(path))
                return;

            if (!AssetDatabase.IsValidFolder($"{path}/{nodeName}"))
            {
                string path_root = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(path, nodeName));
                string path_editor = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(path_root, "Editor"));
                Debug.Log(path_root);
                Debug.Log(path_editor);

                ScriptMaker_Create(path_root, $"action_{surffix}_{nodeName}", scr_action);
                ScriptMaker_Create(path_editor, $"editor_{surffix}_{nodeName}", scr_editor);
                ScriptMaker_Create(path_editor, $"graph_{surffix}_{nodeName}", scr_graph);

                // 刷新资源数据库
                AssetDatabase.Refresh();

                Debug.Log($"扩展节点 {nodeName} 结构已创建！");
            }
        }
        #endregion

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
                case "等待":
                    xtype = "xAction_Wait";
                    break;
                case "属性":
                    xtype = "xAction_Property";
                    break;
                case "结束":
                    xtype = "xAction_End";
                    break;
            }
        }

        #region 数据刷新
        private void update_Data()
        {
            field_content_set(pr_action, $"action_{surffix}_{nodeName}");
            field_content_set(pr_editor, $"editor_{surffix}_{nodeName}");
            field_content_set(pr_graph, $"graph_{surffix}_{nodeName}");

            update_StructContent();

            label_content_set(node_nickname, string.IsNullOrEmpty(nickName) ? nodeName : nickName);

            label_content_set(fd_folderName, $"{surffix}_{nodeName}");
            label_content_set(fd_actionName, $"<b><color=#FFBF23>action_</color></b>{surffix}_{nodeName}");
            label_content_set(fd_editorName, $"<b><color=#FFBF23>editor_</color></b>{surffix}_{nodeName}");
            label_content_set(fd_graphName, $"<b><color=#FFBF23>graph_</color></b>{surffix}_{nodeName}");
        }
        private void update_StructContent()
        {
            string content = $"{{\r\n    \"name\": \"{(string.IsNullOrEmpty(nickName) ? nodeName : nickName)}\",\r\n    \"prefixNamespace\": \"SevenStrikeModules.XGraph\",\r\n    \"prefixClass\": \"action_{surffix}_\",\r\n    \"actionNodeType\": \"{nodeName}\",\r\n    \"icon\": \"{(obj_icon.value == null ? "0a4daa5b210366743a06c03f7d6ff078" : iconguid)}\",\r\n    \"visualNodeType\": \"graph_{surffix}_{nodeName}\"\r\n}}";
            field_content_set(contentField, $"{content}");
        }
        #endregion

        #region 控件控制
        private void field_content_set(TextField field, string content)
        {
            field.value = content;
        }
        private void label_content_set(Label label, string content)
        {
            label.text = content;
        }
        #endregion

        #region 生成扩展节点脚本结构
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
        private void ScriptMaker_Create(string folderPath, string ScriptName, string ScriptContent)
        {
            string scriptPath = Path.Combine(folderPath, ScriptName + ".cs");

            // 创建脚本文件
            File.WriteAllText(scriptPath, ScriptContent);
        }
        private string CreatePortContent()
        {
            string content = "";
            if (xtype == "xAction_Property")
            {
                content = @$"  #region 端口设置
                List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
                // 加入行为端口
                port_out.Add(new xGraph_NodePort(""属性端口"", typeof(Variable_String), Port.Capacity.Multi));
                OutputPort_Set(port_out);
                #endregion";
            }
            else
            {
                if (xtype == "xAction_Start")
                {
                    content = @$" #region 端口设置
                    List<xGraph_NodePort> ports_out = new List<xGraph_NodePort>();
                    // 加入行为端口
                    ports_out.Add(new xGraph_NodePort("""", typeof(xAction_Base), Port.Capacity.Multi));
                    OutputPort_Set(ports_out);
                    #endregion";
                }
                else if (xtype == "xAction_Composite" || xtype == "xAction_Wait")
                {
                    content = @$"   #region 端口设置
                    List<xGraph_NodePort> port_in = new List<xGraph_NodePort>();
                    // 加入行为端口
                    port_in.Add(new xGraph_NodePort("""", typeof(xAction_Base), Port.Capacity.Single));
                    InputPort_Set(port_in);
                    
                    List<xGraph_NodePort> port_out = new List<xGraph_NodePort>();
                    // 加入行为端口
                    port_out.Add(new xGraph_NodePort("""", typeof(xAction_Base), Port.Capacity.Multi));
                    OutputPort_Set(port_out);
                    #endregion";
                }
                else if (xtype == "xAction_End")
                {
                    content = @$"  #region 端口设置
                    List<xGraph_NodePort> port_in = new List<xGraph_NodePort>();
                    // 加入行为端口
                    port_in.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Single));
                    InputPort_Set(port_in);
                    #endregion";
                }
            }
            return content;
        }
        private string CreateStartNodePortOffset()
        {
            string content = "";

            if (xtype == "xAction_Start")
            {
                content = @$"

                        // 因为开始节点没有行为输入端，为了让首个输入端口视觉上不会和分割线重叠所以需要矫正偏移
                        inputContainer.style.paddingTop = 25;";
            }
            return content;
        }
        private string CreateNodeOutputDraw()
        {
            string content = "";

            if (xtype != "xAction_End")
            {
                content = @$"

                        // 绘制输出节点容器
                        Draw_Output();";
            }
            return content;
        }
        private string CreateNodeInputDraw()
        {
            string content = "";

            if (xtype != "xAction_Property")
            {
                content = @$"

                        // 绘制输入节点容器
                        Draw_Input();";
            }
            return content;
        }
        private string ScriptMaker_CreateContent_Action()
        {
            string content = "";

            content = $@"namespace SevenStrikeModules.XGraph
{{
    using UnityEngine;

    public class action_{surffix}_{nodeName} : {xtype}
    {{
        /// <summary>
        /// 节点执行
        /// </summary>
        public override void Execute()
        {{
            base.Execute();
        }}           
    }}
}}";
            return content;
        }
        private string ScriptMaker_CreateContent_Editor()
        {
            string content = "";

            content = $@"namespace SevenStrikeModules.XGraph
{{
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(action_{surffix}_{nodeName}))]
    public class editor_{surffix}_{nodeName} : editor_{xtype}
    {{
        /// <summary>
        /// 目标对象
        /// </summary>
        private action_{surffix}_{nodeName} {nodeName};

        public override void OnEnable()
        {{
            base.OnEnable();
        }}
        /// <summary>
        /// 获取脚本
        /// </summary>
        public override void GetTargetScript()
        {{
            base.GetTargetScript();

            {nodeName} = target as action_{surffix}_{nodeName};
        }}
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        public override void GetProperties()
        {{
            base.GetProperties();
        }}

        //------------------------------------------------------

        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name=""root""></param>
        public override Foldout Folder_Extensions(VisualElement root)
        {{
            Foldout fold = base.Folder_Extensions(root);

            return fold;
        }}
    }}
}}";
            return content;
        }
        private string ScriptMaker_CreateContent_Graph()
        {
            string content = "";

            content = $@"namespace SevenStrikeModules.XGraph
{{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class graph_{surffix}_{nodeName} : {(xtype != "xAction_Property" ? "xNode_Base" : "xNode_Property")}
    {{
        action_{surffix}_{nodeName} {nodeName};

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {{
            base.Initialize(graphView, pos, data);

            {CreatePortContent()}     

            {nodeName} = ActionData as action_{surffix}_{nodeName};
        }}                       

        #region 节点绘制
        public override xNode_Base Draw()
        {{
            // 绘制主容器
            Draw_Main();

            // 绘制标题容器
            Draw_Title();

            // 绘制标题按钮容器
            Draw_TitleButton();

            // 绘制顶部容器
            Draw_Top();{CreateNodeInputDraw()}{CreateNodeOutputDraw()}                        

            // 绘制扩展容器
            Draw_Extension();{CreateStartNodePortOffset()}

            return this;
        }}
        #endregion

        #region 重写
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {{
            base.On_VariablesValue_Changed();
        }}
        /// <summary>
        /// 当克隆节点时
        /// </summary>
        /// <param name=""list""></param>
        public override void On_Nodes_Duplicated(List<DuplicateNodeData> list)
        {{
            base.On_Nodes_Duplicated(list);
        }}
        /// <summary>
        /// 当节点重建时
        /// </summary>
        public override void On_Node_Restructure()
        {{
            base.On_Node_Restructure();
        }}
        /// <summary>
        /// 当节点连线时
        /// </summary>
        /// <param name=""edge""></param>
        public override void On_Node_CreateEdge(Edge edge)
        {{
            base.On_Node_CreateEdge(edge);
        }}
        /// <summary>
        /// 当节点移除连线时
        /// </summary>
        /// <param name=""edge""></param>
        public override void On_Node_RemovedEdge(Edge edge)
        {{
            base.On_Node_RemovedEdge(edge);
        }}
        /// <summary>
        /// 当节点头像改变时
        /// </summary>
        /// <param name=""tex""></param>
        public override void On_Node_AvatarChanged(Texture2D tex)
        {{
            base.On_Node_AvatarChanged(tex);
        }}
        /// <summary>
        /// 当节点执行模式改变时
        /// </summary>
        /// <param name=""state""></param>
        public override void On_Node_ConcurrentChanged(bool state)
        {{
            base.On_Node_ConcurrentChanged(state);
        }}
        /// <summary>
        /// 当改变节点图标时
        /// </summary>
        /// <param name=""tex""></param>
        public override void On_Node_IconChanged(Texture2D tex)
        {{
            base.On_Node_IconChanged(tex);
        }}
        /// <summary>
        /// 当改变节点颜色主题时
        /// </summary>
        public override void On_Node_ThemeColorChanged()
        {{
            base.On_Node_ThemeColorChanged();
        }}
        /// <summary>
        /// 当改变节点通透模式时
        /// </summary>
        /// <param name=""state""></param>
        public override void On_Node_TransparentChanged(bool state)
        {{
            base.On_Node_TransparentChanged(state);
        }}
        /// <summary>
        /// 当改变节点尺寸时
        /// </summary>
        /// <param name=""evt""></param>
        public override void OnSizeChanged(GeometryChangedEvent evt)
        {{
            base.OnSizeChanged(evt);
        }}
        /// <summary>
        /// 当选中节点时
        /// </summary>
        public override void OnSelected()
        {{
            base.OnSelected();
        }}
        /// <summary>
        /// 当取消选中节点时
        /// </summary>
        public override void OnUnselected()
        {{
            base.OnUnselected();
        }}
        #endregion
    }}
}}";
            return content;
        }
        #endregion
    }
}