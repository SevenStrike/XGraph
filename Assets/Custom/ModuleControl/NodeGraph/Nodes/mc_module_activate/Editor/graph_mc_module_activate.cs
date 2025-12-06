namespace SevenStrikeModules.XGraph
{
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class graph_mc_module_activate : xNode_Composite
    {
        action_mc_module_activate module_activate;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            InputPort_Add(new xGraph_NodePort("名称", typeof(Variable_String), Port.Capacity.Single));// 加入变量端口（输入）

            InputPort_Add(new xGraph_NodePort("激活", typeof(Variable_Bool), Port.Capacity.Single));// 加入变量端口（输入）

            module_activate = base.composite as action_mc_module_activate;
        }

        #region 节点绘制
        public override xNode_Base Draw()
        {
            return base.Draw();
        }
        #endregion

        #region 重写 - 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            module_activate.Set_ActivateModuleName_withPort("名称");
            module_activate.Set_ActivateModuleState_withPort("激活");
        }
        #endregion

        #region 重写 - 绘制Inspector
        /// <summary>
        /// 自定义组件折叠容器
        /// </summary>
        /// <param name="root"></param>
        public override Foldout ins_Folder_Extensions(VisualElement root)
        {
            Foldout fold = base.ins_Folder_Extensions(root);

            #region 模组名称
            TextField field_name = util_XGraphInspectorGUI.GUI_Field_String(fold, "名称", module_activate.activateName, new string[] { "field_text" });
            field_name.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                // 如果该节点已经存在绑定的变量，则将当前序列化值给到控件值，因为该序列化值已受变量控制
                if (isVariableBinded("名称"))
                {
                    field_name.value = module_activate.activateName;
                    module_activate.Set_ActivateModuleName_withPort("名称");
                }
                // 否则就代表没有任何变量节点接入，而使用控件值给到序列化属性值
                else
                {
                    module_activate.Set_ActivateModuleName(field_name.value);
                }
            });
            #endregion

            #region 模组激活
            Toggle field_state = util_XGraphInspectorGUI.GUI_Field_Bool(fold, "激活", module_activate.activateState, new string[] { "field_bool" });
            field_state.RegisterValueChangedCallback((v) =>
            {
                // 如果该节点已经存在绑定的变量，则将当前序列化值给到控件值，因为该序列化值已受变量控制
                if (isVariableBinded("激活"))
                {
                    field_state.value = module_activate.activateState;
                    module_activate.Set_ActivateModuleState_withPort("激活");
                }
                // 否则就代表没有任何变量节点接入，而使用控件值给到序列化属性值
                else
                {
                    module_activate.Set_ActivateModuleState(v.newValue);
                }
            });
            #endregion

            // 当节点绑定变量时，将变量值同步到控件值
            module_activate.On_Node_Variable_Binded += (vare) =>
            {
                field_name.value = module_activate.activateName;
                field_state.value = module_activate.activateState;
            };

            // 克隆节点后刷新控件值为克隆后的最新值
            module_activate.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                action_mc_module_activate s_source = (action_mc_module_activate)source;
                // 克隆后的行为数据
                action_mc_module_activate s_clone = (action_mc_module_activate)clone;
                s_clone.activateName = s_source.activateName;
                s_clone.activateState = s_source.activateState;

                field_name.value = module_activate.activateName;
                field_state.value = module_activate.activateState;
            };

            return fold;
        }
        #endregion
    }
}