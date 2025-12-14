namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Data;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    [Serializable]
    public class VariableController<T>
    {
        public KeyCode key_control;
        public T[] data_list;
        public int index = -1;
        public T current;
    }

    public class mc_GraphController : MonoBehaviour
    {
        /// <summary>
        /// 节点运行器
        /// </summary>
        public mc_GraphRunner GraphRunner;

        /// <summary>
        /// 执行节点流程
        /// </summary>
        public KeyCode key_ActionStart;
        /// <summary>
        /// 杀死节点流程执行
        /// </summary>
        public KeyCode key_ActionKill;
        /// <summary>
        /// 暂停节点流程执行
        /// </summary>
        public KeyCode key_ActionPause;
        /// <summary>
        /// 继续节点流程执行
        /// </summary>
        public KeyCode key_ActionResume;
        /// <summary>
        /// 手动执行节点流程
        /// </summary>
        public KeyCode key_ActionManualStep;
        /// <summary>
        /// 手动执行节点流程开关
        /// </summary>
        public KeyCode key_RunnerMode;

        public VariableController<string> vc_modules;
        public VariableController<string> vc_msg;
        public VariableController<float> vc_delay;
        public VariableController<bool> vc_switch;

        private void OnManual_StepComplete()
        {
            util_Dashboard.LogMsg(xMessageType.警告, "节点流程执行控制器", "手动步骤执行完成，可以执行下一步", GraphRunner.SampleAsset.LogEnabled);
        }
        private void OnManual_WaitComplete()
        {
            util_Dashboard.LogMsg(xMessageType.警告, "节点流程执行控制器", "等待节点完成，可以继续执行", GraphRunner.SampleAsset.LogEnabled);
        }

        private void Start()
        {
            Key_Initialize();
            Variable_Initialize();
        }
        private void OnDestroy()
        {
            // 注销回调
            if (GraphRunner != null)
            {
                GraphRunner.OnManual_StepComplete -= OnManual_StepComplete;
                GraphRunner.OnManual_WaitComplete -= OnManual_WaitComplete;
            }
        }
        void Update()
        {
            // 流程节点 - 开始执行
            if (Input.GetKeyDown(key_ActionStart))
            {
                GraphRunner.Runner_Start();
            }
            ActionControl();
            Variable_Modified();
        }
        /// <summary>
        /// 行为流程控制按键
        /// </summary>
        private void Key_Initialize()
        {
            key_ActionStart = KeyCode.A;
            key_ActionKill = KeyCode.S;
            key_ActionPause = KeyCode.D;
            key_ActionResume = KeyCode.F;
            key_ActionManualStep = KeyCode.G;
            key_RunnerMode = KeyCode.H;
        }
        /// <summary>
        /// 初始化变量的目标修改值
        /// </summary>
        private void Variable_Initialize()
        {
            vc_modules.key_control = KeyCode.V;
            vc_modules.data_list = new string[4] { "Module_A", "Module_B", "Module_C", "Module_D" };
            vc_modules.index = 0;
            vc_modules.current = vc_modules.data_list[vc_modules.index];

            vc_msg.key_control = KeyCode.B;
            vc_msg.data_list = new string[4] { "m_bK7TdR2N", "m_Yp5sQ9Lm", "m_3xG8FjZc", "m_qHn1Vw4R" };
            vc_msg.index = 0;
            vc_msg.current = vc_msg.data_list[vc_msg.index];

            vc_delay.key_control = KeyCode.N;
            vc_delay.data_list = new float[8] { 0, 0.02f, 0.05f, 0.1f, 0.2f, 0.5f, 2f, 3f };
            vc_delay.index = 0;
            vc_delay.current = vc_delay.data_list[vc_delay.index];

            vc_switch.key_control = KeyCode.M;
            vc_switch.data_list = new bool[2] { true, false };
            vc_switch.index = 0;
            vc_switch.current = vc_switch.data_list[vc_switch.index];
        }
        /// <summary>
        /// 变量修改
        /// </summary>
        private void Variable_Modified()
        {
            // 修改变量 - modue
            if (Input.GetKeyDown(vc_modules.key_control))
            {
                vc_modules.current = vc_modules.data_list[vc_modules.index];

                GraphRunner.SampleAsset.Variable_SetValue<string>("module", vc_modules.current);

                if (vc_modules.index >= vc_modules.data_list.Length - 1)
                    vc_modules.index = 0;
                else
                    vc_modules.index++;
            }
            // 修改变量 - msg
            if (Input.GetKeyDown(vc_msg.key_control))
            {
                vc_msg.current = vc_msg.data_list[vc_msg.index];

                GraphRunner.SampleAsset.Variable_SetValue<string>("msg", vc_msg.current);

                if (vc_msg.index >= vc_msg.data_list.Length - 1)
                    vc_msg.index = 0;
                else
                    vc_msg.index++;
            }
            // 修改变量 - delay
            if (Input.GetKeyDown(vc_delay.key_control))
            {
                vc_delay.current = vc_delay.data_list[vc_delay.index];

                GraphRunner.SampleAsset.Variable_SetValue<float>("delay", vc_delay.current);

                if (vc_delay.index >= vc_delay.data_list.Length - 1)
                    vc_delay.index = 0;
                else
                    vc_delay.index++;
            }
            // 修改变量 - switch
            if (Input.GetKeyDown(vc_switch.key_control))
            {
                vc_switch.current = vc_switch.data_list[vc_switch.index];

                GraphRunner.SampleAsset.Variable_SetValue<bool>("switch", vc_switch.current);

                if (vc_switch.index >= vc_switch.data_list.Length - 1)
                    vc_switch.index = 0;
                else
                    vc_switch.index++;
            }
        }
        /// <summary>
        /// 行为流程控制
        /// </summary>
        private void ActionControl()
        {
            // 流程节点 - 杀死
            if (Input.GetKeyDown(key_ActionKill))
            {
                GraphRunner.Runner_Kill();
            }
            // 流程节点 - 暂停执行
            if (Input.GetKeyDown(key_ActionPause))
            {
                GraphRunner.Runner_Pause();
            }
            // 流程节点 - 继续执行
            if (Input.GetKeyDown(key_ActionResume))
            {
                GraphRunner.Runner_Resume();
            }
            // 流程节点 - 切换执行模式
            if (Input.GetKeyDown(key_RunnerMode))
            {
                GraphRunner.ManualExecutionMode = !GraphRunner.ManualExecutionMode;
                util_Dashboard.LogMsg(xMessageType.警告, $"节点流程执行控制器", $"手动执行模式: {(GraphRunner.ManualExecutionMode ? "开启" : "关闭")}", "00ff9d", GraphRunner.SampleAsset.LogEnabled);
            }
            // 流程节点 - 手动步进执行
            if (Input.GetKeyDown(key_ActionManualStep))
            {
                GraphRunner.Manual_Action_Execution();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(mc_GraphController))]
    public class Editor_mc_GraphController : Editor
    {
        private mc_GraphController baseScript;
        private Font font;
        private StyleFontDefinition font_def;

        public void OnEnable()
        {
            baseScript = target as mc_GraphController;

            font = AssetDatabase.LoadAssetAtPath<Font>($"{util_Dashboard.GetPath_Fonts()}x_Regular.ttf");
            font_def = new StyleFontDefinition(AssetDatabase.LoadAssetAtPath<Font>($"{util_Dashboard.GetPath_Fonts()}x_Regular.ttf"));
        }

        public override VisualElement CreateInspectorGUI()
        {
            // 创建根 VisualElement
            VisualElement rootElement = new VisualElement();

            // 首先添加默认的属性字段
            //InspectorElement.FillDefaultInspector(rootElement, serializedObject, this);

            // 添加一个分隔线
            rootElement.Add(new VisualElement
            {
                style =
                {
                    height = 1,
                    backgroundColor = new Color(0.3f, 0.3f, 0.3f),
                    marginTop = 10,
                    marginBottom = 10
                }
            });

            // 创建流程控制部分
            CreateSection_ActionKey(rootElement, new[]
            {
                ("节点运行器", serializedObject.FindProperty("GraphRunner")),
                ("开始执行", serializedObject.FindProperty("key_ActionStart")),
                ("杀死流程", serializedObject.FindProperty("key_ActionKill")),
                ("暂停执行", serializedObject.FindProperty("key_ActionPause")),
                ("继续执行", serializedObject.FindProperty("key_ActionResume")),
                ("手动步进", serializedObject.FindProperty("key_ActionManualStep")),
                ("切换模式", serializedObject.FindProperty("key_RunnerMode"))
            });

            // 创建变量控制部分
            CreateSection_VariableModified(rootElement, "模块变量", serializedObject.FindProperty("vc_modules"));
            CreateSection_VariableModified(rootElement, "消息变量", serializedObject.FindProperty("vc_msg"));
            CreateSection_VariableModified(rootElement, "延迟变量", serializedObject.FindProperty("vc_delay"));
            CreateSection_VariableModified(rootElement, "开关变量", serializedObject.FindProperty("vc_switch"));

            // 添加使用说明
            CreateSection_InfoBox(rootElement);

            return rootElement;
        }

        private void CreateSection_ActionKey(VisualElement parent, (string label, SerializedProperty prop)[] properties)
        {
            var section = new VisualElement();
            section.style.marginBottom = 10;

            foreach (var (label, prop) in properties)
            {
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.justifyContent = Justify.SpaceBetween;
                row.style.marginBottom = 4;

                var labelElement = new Label(label);
                labelElement.style.width = 80;
                labelElement.style.unityFont = font;
                labelElement.style.unityFontDefinition = font_def;
                labelElement.style.unityTextAlign = TextAnchor.MiddleLeft;

                var propertyField = new PropertyField(prop);
                propertyField.style.flexGrow = 1;
                propertyField.Bind(serializedObject);
                propertyField.label = "";

                row.Add(labelElement);
                row.Add(propertyField);
                section.Add(row);
            }

            parent.Add(section);
        }

        private void CreateSection_VariableModified(VisualElement parent, string title, SerializedProperty variableProp)
        {
            var section = new VisualElement();
            section.style.marginBottom = 10;

            // 添加一个分隔线
            section.Add(new VisualElement
            {
                style =
                {
                    height = 1,
                    backgroundColor = new Color(0.3f, 0.3f, 0.3f),
                    marginTop = 5,
                    marginBottom = 5
                }
            });

            var sectionHeader = new Label(title);
            sectionHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            sectionHeader.style.fontSize = 12;
            sectionHeader.style.marginBottom = 6;
            sectionHeader.style.color = new Color(0.7f, 0.7f, 0.7f);
            section.Add(sectionHeader);

            // 控制按键
            var keyRow = new VisualElement();
            keyRow.style.flexDirection = FlexDirection.Row;
            keyRow.style.justifyContent = Justify.SpaceBetween;
            keyRow.style.marginBottom = 4;

            var keyLabel = new Label("控制按键");
            keyLabel.style.width = 80;
            keyLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            keyLabel.style.unityFont = font;
            keyLabel.style.unityFontDefinition = font_def;

            var keyField = new PropertyField(variableProp.FindPropertyRelative("key_control"));
            keyField.style.flexGrow = 1;
            keyField.Bind(serializedObject);
            keyField.label = "";

            keyRow.Add(keyLabel);
            keyRow.Add(keyField);
            section.Add(keyRow);

            // 当前值
            var currentValue_row = new VisualElement();
            currentValue_row.style.flexDirection = FlexDirection.Row;
            currentValue_row.style.justifyContent = Justify.SpaceBetween;
            currentValue_row.style.marginBottom = 4;

            // 当前值
            var currentValue_title = new Label("当前值");
            currentValue_title.style.width = 110;
            currentValue_title.style.unityTextAlign = TextAnchor.MiddleLeft;
            currentValue_title.style.unityFont = font;
            currentValue_title.style.unityFontDefinition = font_def;

            var currentValue_value = new PropertyField(variableProp.FindPropertyRelative("current"));
            currentValue_value.style.flexGrow = 1;
            currentValue_value.style.marginLeft = 10;
            currentValue_value.Bind(serializedObject);
            currentValue_value.label = "";

            currentValue_row.Add(currentValue_title);
            currentValue_row.Add(currentValue_value);
            section.Add(currentValue_row);

            // 数据列表
            var dataRow = new VisualElement();
            dataRow.style.flexDirection = FlexDirection.Row;
            dataRow.style.justifyContent = Justify.SpaceBetween;

            var dataLabel = new Label("可选值列表");
            dataLabel.style.width = 80;
            dataLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            dataLabel.style.unityFont = font;
            dataLabel.style.unityFontDefinition = font_def;

            var dataField = new PropertyField(variableProp.FindPropertyRelative("data_list"));
            dataField.style.flexGrow = 1;
            dataField.Bind(serializedObject);
            dataField.label = "";

            dataRow.Add(dataLabel);
            dataRow.Add(dataField);
            section.Add(dataRow);

            parent.Add(section);
        }

        private void CreateSection_InfoBox(VisualElement parent)
        {
            var infoBox = new VisualElement();
            infoBox.style.backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.3f);
            infoBox.style.paddingBottom = 15;
            infoBox.style.paddingLeft = 10;
            infoBox.style.paddingRight = 10;
            infoBox.style.paddingTop = 10;
            infoBox.style.marginTop = 10;
            infoBox.style.marginBottom = 20;
            infoBox.style.borderBottomLeftRadius = 5;
            infoBox.style.borderBottomRightRadius = 5;
            infoBox.style.borderTopLeftRadius = 5;
            infoBox.style.borderTopRightRadius = 5;

            var infoTitle = new Label("使用说明");
            infoTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
            infoTitle.style.marginBottom = 15;
            infoTitle.style.fontSize = 14;
            infoTitle.style.color = new Color(0.6f, 0.8f, 1f);
            infoTitle.style.unityFont = font;
            infoTitle.style.unityFontDefinition = font_def;
            infoBox.Add(infoTitle);

            var infoContent = new Label("• 运行时使用快捷键控制节点图执行\n\n• 修改变量时会在黑板中实时更新\n\n• 手动模式开启后需按空格键步进执行");
            infoContent.style.whiteSpace = WhiteSpace.Normal;
            infoContent.style.fontSize = 12;
            infoContent.style.unityFont = font;
            infoContent.style.unityFontDefinition = font_def;
            infoBox.Add(infoContent);

            parent.Add(infoBox);
        }
    }
#endif
}