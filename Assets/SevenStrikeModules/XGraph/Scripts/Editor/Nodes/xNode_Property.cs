namespace SevenStrikeModules.XGraph
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Property : xNode_Base
    {
        public xAction_Property property;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            property = data as xAction_Property;

            // 当Graphview编辑器的主题色改变时
            graphView.gv_GraphWindow.OnThemeColorChanged += OnGraphViewEditorThemeColorChanged;

            // 每次初始化时先清空，避免重复注册
            property.On_InternalVariableValue_Changed = null;
        }

        #region 节点绘制
        public override xNode_Base Draw()
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
            //Draw_Input();

            // 绘制输出节点容器
            Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            // 因为开始节点没有行为输入端，为了让首个输入端口视觉上不会和分割线重叠所以需要矫正偏移
            outputContainer.style.paddingTop = 25;

            return this;
        }

        public override void Draw_Main()
        {
            base.Draw_Main();
        }

        public override void Draw_Extension()
        {
            //base.Draw_Extension();
        }

        public override void Draw_Title()
        {
            base.Draw_Title();

            TitleInputField.RegisterCallback<BlurEvent>(SyncChangeVariableName);
        }

        public override void Draw_Output()
        {
            base.Draw_Output();
        }
        #endregion    

        #region 回调
        /// <summary>
        /// 当Graphview编辑器的主题色改变时
        /// </summary>
        /// <param name="color"></param>
        private void OnGraphViewEditorThemeColorChanged(Color color)
        {

        }
        /// <summary>
        /// 改变节点名称的同时同步修改变量类的名称
        /// </summary>
        /// <param name="evt"></param>
        private void SyncChangeVariableName(BlurEvent evt)
        {
            // Inspector 面板显示属性
            graphView.gv_GraphWindow.xw_InspectorView.InspectorViewer(this);
        }
        public void VariableNodeFieldValueUpdate()
        {

        }
        #endregion

        #region 重写
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();
            VariableNodeFieldValueUpdate();
        }

        public override void ExecutionModeMark()
        {
            //base.ExecutionModeMark();
        }
        #endregion
    }
}