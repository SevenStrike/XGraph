namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class VNode_Variable_Internal : VNode_Base
    {
        /// <summary>
        /// 视觉节点尺寸控制图标
        /// </summary>
        public VisualElement ResizerIcon;
        /// <summary>
        /// 视觉节点尺寸控制组件
        /// </summary>
        public VisualElement Resizer;
        public ActionNode_Variable VariableData;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            // 设置节点的容器样式
            util_XGraphEditorUtility.ElementStyle_Add(this, $"{util_Dashboard.GetPath_GUI_Uss()}uss_VariableInternalNode.uss");

            AddToClassList("rootcontainer");

            ActionNode_Variable actionvar = VariableData = data as ActionNode_Variable;

            if (VariableData.variable.type == VariableType.String)
            {
                // 指定可调整大小
                capabilities |= Capabilities.Resizable;
            }

            style.width = data.nodeGraphSize.x;
            style.height = data.nodeGraphSize.y;

            // 动态获取变量类型
            string asm = "Assembly-CSharp";
            Type var_type = Type.GetType($"SevenStrikeModules.XGraph.Variable_{actionvar.variable.type}, {asm}");

            #region 端口设置
            // 加入行为端口
            Port_Inputs.Add(new xGraph_NodePort("In", var_type, Port.Capacity.Single));
            Port_Outputs.Add(new xGraph_NodePort("Out", var_type, Port.Capacity.Multi));
            #endregion
        }

        /// <summary>
        /// 可改变尺寸
        /// </summary>
        /// <returns></returns>
        public override bool IsResizable()
        {
            if (VariableData.variable.type == VariableType.String)
                return true;
            else
                return false;
        }

        #region 节点绘制
        public override VNode_Base Draw()
        {
            // 绘制主容器
            Draw_Main();

            // 绘制标题容器
            Draw_Title();

            // 绘制标题按钮容器
            Draw_TitleButton();

            // 绘制顶部容器
            Draw_Top();

            // 绘制输入节点容器
            Draw_Input();

            // 绘制输出节点容器
            Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }

        public override void Draw_Main()
        {
            base.Draw_Main();

            if (VariableData.variable.type == VariableType.String)
            {
                // 拖拽尺寸控件图标
                ResizerIcon = this.Q<VisualElement>(className: "resizer-icon");
                ResizerIcon.pickingMode = PickingMode.Ignore;
                ResizerIcon.style.opacity = 0f;

                // 拖拽尺寸控件
                Resizer = this.Q<VisualElement>(className: "resizer");
                Resizer.style.width = 30;
                Resizer.style.height = 30;
                Resizer.RegisterCallback<PointerEnterEvent>(Decal_DisplayResizer);
                Resizer.RegisterCallback<PointerLeaveEvent>(Decal_HideResizer);
            }
        }

        public override void Draw_Extension()
        {
            base.Draw_Extension();

            VisualElement element = null;

            switch (VariableData.variable.type)
            {
                case VariableType.String:
                    TextField field = new TextField();
                    field.value = VariableData.variable.GetValue<string>();
                    field.RegisterCallback<BlurEvent>(VariableDataChanged_String);
                    field.AddToClassList("value_field");
                    field.Q<TextElement>().AddToClassList("value_field_text");
                    element = field;
                    break;
                case VariableType.Float:
                    break;
                case VariableType.Int:
                    break;
                case VariableType.Bool:
                    break;
                case VariableType.Vector2:
                    break;
                case VariableType.Vector3:
                    break;
                case VariableType.Vector4:
                    break;
                case VariableType.Color:
                    break;
            }

            AppendElement(GraphNodeContainerType.ExtensionContainer, element);
        }

        public override void Draw_Title()
        {
            base.Draw_Title();

            TitleInputField.RegisterCallback<BlurEvent>(SyncChangeVariableName);
        }

        private void SyncChangeVariableName(BlurEvent evt)
        {
            VariableData.variable.name = VariableData.identifyName;
        }
        #endregion

        #region 回调
        private void VariableDataChanged_String(BlurEvent evt)
        {
            TextField textField = evt.target as TextField;
            VariableData.variable.SetValue<string>(textField.text);
        }

        /// <summary>
        /// 鼠标移出时隐藏角点拖拽显示
        /// </summary>
        /// <param name="evt"></param>
        private void Decal_HideResizer(PointerLeaveEvent evt)
        {
            ResizerIcon.style.opacity = 0f;
        }
        /// <summary>
        /// 鼠标进入时显示角点拖拽显示
        /// </summary>
        /// <param name="evt"></param>
        private void Decal_DisplayResizer(PointerEnterEvent evt)
        {
            ResizerIcon.style.opacity = 1f;
        }
        #endregion

        #region 辅助

        #endregion
    }
}