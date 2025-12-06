namespace SevenStrikeModules.XGraph
{
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class graph_mc_module_initialize : xNode_Start
    {
        action_mc_module_initialize module_initialize;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            InputPort_Add(new xGraph_NodePort("激活所有模组", typeof(Variable_Bool), Port.Capacity.Single));// 加入变量端口（输入）

            module_initialize = base.start as action_mc_module_initialize;
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

            // 根据获取的目标端口的变量节点值来更新节点变量
            module_initialize.Set_ModulesInitialized("激活所有模组");
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

            #region  启动时所有模组的激活开关
            Toggle toggle = util_XGraphInspectorGUI.GUI_Field_Bool(fold, "激活所有模组", module_initialize.activateAllModules, new string[] { "field_bool" });
            toggle.RegisterValueChangedCallback((value) =>
            {
                // 如果该节点已经存在绑定的变量，则将当前序列化值给到控件值，因为该序列化值已受变量控制
                if (isVariableBinded("激活所有模组"))
                {
                    toggle.value = module_initialize.activateAllModules;
                    module_initialize.Set_ModulesInitialized("激活所有模组");
                }
                // 否则就代表没有任何变量节点接入，而使用控件值给到序列化属性值
                else
                {
                    module_initialize.Set_ModulesInitialized(value.newValue);
                }
            });
            #endregion

            // 当节点绑定变量时，将变量值同步到控件值
            module_initialize.On_Node_Variable_Binded += ((vare) =>
            {
                toggle.value = module_initialize.activateAllModules;
            });

            // 克隆节点后刷新控件值为克隆后的最新值
            module_initialize.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                action_mc_module_initialize s_clone = clone as action_mc_module_initialize;
                // 克隆后的行为数据
                action_mc_module_initialize s_source = source as action_mc_module_initialize;
                s_clone.activateAllModules = s_source.activateAllModules;

                toggle.value = module_initialize.activateAllModules;
            };

            return fold;
        }
        #endregion
    }
}