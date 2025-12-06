namespace SevenStrikeModules.XGraph
{
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class graph_mc_modules_activate : xNode_Composite
    {
        action_mc_modules_activate modules_activate;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            InputPort_Add(new xGraph_NodePort("激活", typeof(Variable_Bool), Port.Capacity.Single));// 加入变量端口（输入）

            modules_activate = base.composite as action_mc_modules_activate;
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

            modules_activate.Set_ActivateModulesState("激活");
        }
        /// <summary>
        /// 当克隆节点时
        /// </summary>
        /// <param name="list"></param>      
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
            Toggle toggle = util_XGraphInspectorGUI.GUI_Field_Bool(fold, "激活", modules_activate.activateState, new string[] { "field_bool" });
            toggle.RegisterValueChangedCallback((value) =>
            {
                // 如果该节点已经存在绑定的变量，则将当前序列化值给到控件值，因为该序列化值已受变量控制
                if (isVariableBinded("激活"))
                {
                    toggle.value = modules_activate.activateState;
                    modules_activate.Set_ActivateModulesState("激活");
                }
                // 否则就代表没有任何变量节点接入，而使用控件值给到序列化属性值
                else
                {
                    modules_activate.Set_ActivateModulesState(value.newValue);
                }
            });
            #endregion

            // 当节点绑定变量时，将变量值同步到控件值
            modules_activate.On_Node_Variable_Binded += ((vare) =>
            {
                toggle.value = modules_activate.activateState;
            });

            // 克隆节点后刷新控件值为克隆后的最新值
            modules_activate.On_Node_Duplicated += (clone, source) =>
            {
                // 克隆父物体的行为数据
                action_mc_modules_activate s_clone = clone as action_mc_modules_activate;
                // 克隆后的行为数据
                action_mc_modules_activate s_source = source as action_mc_modules_activate;
                s_clone.activateState = s_source.activateState;

                toggle.value = modules_activate.activateState;
            };

            return fold;
        }
        #endregion
    }
}