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
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class xNode_Branch : xNode_Base
    {
        internal xAction_Branch branch;

        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, xAction_Base data = null)
        {
            base.Initialize(graphView, pos, data);

            #region 端口设置
            // 加入行为端口
            Port_Inputs.Add(new xGraph_NodePort("", typeof(xAction_Base), Port.Capacity.Single));
            Port_Inputs.Add(new xGraph_NodePort("条件", typeof(Variable_Bool), Port.Capacity.Single));
            Port_Outputs.Add(new xGraph_NodePort("开", typeof(xAction_Base), Port.Capacity.Single));
            Port_Outputs.Add(new xGraph_NodePort("关", typeof(xAction_Base), Port.Capacity.Single));
            #endregion          

            branch = data as xAction_Branch;
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
            Draw_Input();

            // 绘制输出节点容器
            Draw_Output();

            // 绘制扩展容器
            Draw_Extension();

            return this;
        }

        public override void Draw_Output()
        {
            outputContainer.AddToClassList("branchOutputOffset");
            base.Draw_Output();
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
        }
        #endregion

        #region 重写 - 回调
        /// <summary>
        /// 黑板变量数值变化时的回调
        /// </summary>
        public override void On_VariablesValue_Changed()
        {
            base.On_VariablesValue_Changed();

            branch.SetPredicateState("条件");
        }
        #endregion
    }
}