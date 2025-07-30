namespace SevenStrikeModules.XGraph
{
    using System.Collections;
    using UnityEngine;

    public class ActionTreeRunner : MonoBehaviour
    {
        public ActionTree_Nodes_Asset assetNode;
        private bool isRunning = false;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                FlowStart();
        }

        #region 对外控制
        public void FlowStart()
        {
            if (assetNode == null || assetNode.ActionTreeNodes.Count == 0)
            {
                Debug.LogError("没有行为树资源！");
                return;
            }

            var start = assetNode.ActionTreeNodes.Find(n => n.actionNodeType == xg_ActionTreeType.Start);
            if (start == null)
            {
                Debug.LogError("缺少 Start 节点！");
                return;
            }

            // 防止重复启动
            StopAllCoroutines();
            StartCoroutine(FlowActionLoop());
        }

        public void Stop() => isRunning = false;
        #endregion

        #region 主流程
        IEnumerator FlowActionLoop()
        {
            isRunning = true;
            Debug.Log("----------------->> FlowStart");

            var start = assetNode.ActionTreeNodes.Find(n =>
            {
                return n.actionNodeType == xg_ActionTreeType.Start;
            });
            yield return ExecuteNode(start);

            isRunning = false;
            Debug.Log("----------------->> FlowEnd");
        }
        #endregion

        #region 递归执行节点
        IEnumerator ExecuteNode(ActionTree_Node_Base ActionNode)
        {
            if (ActionNode == null || !isRunning) yield break;

            switch (ActionNode)
            {
                // 组合节点：顺序执行所有子节点
                case ActionTree_Node_Composite composite:
                    ActionNode.Execute();
                    foreach (var child in composite.childNodes)
                        yield return ExecuteNode(child);
                    break;

                // 等待节点：先等待，再继续子节点
                case ActionTree_Node_Wait wait:
                    ActionNode.Execute();
                    yield return new WaitForSeconds(wait.Time);
                    yield return ExecuteNode(wait.ChildNode);
                    break;

                // 调试信息节点：打印信息并继续子节点
                case ActionTree_Node_Debug debug:
                    ActionNode.Execute();
                    yield return ExecuteNode(debug.ChildNode);
                    break;

                // 开始节点：继续子节点
                case ActionTree_Node_Start start:
                    ActionNode.Execute();
                    yield return ExecuteNode(start.ChildNode);
                    break;

                // 结束节点：仅打印信息
                case ActionTree_Node_End end:
                    ActionNode.Execute();
                    break;

                // 其他普通节点：执行后按 GetNextNode 继续
                default:
                    ActionNode.Execute();
                    yield return ExecuteNode(GetNextNode(ActionNode));
                    break;
            }
        }
        #endregion

        #region 获取下一节点
        private ActionTree_Node_Base GetNextNode(ActionTree_Node_Base current)
        {
            return current switch
            {
                ActionTree_Node_Start s => s.ChildNode,
                ActionTree_Node_Wait w => w.ChildNode,
                ActionTree_Node_Debug a => a.ChildNode,
                _ => null
            };
        }
        #endregion
    }
}