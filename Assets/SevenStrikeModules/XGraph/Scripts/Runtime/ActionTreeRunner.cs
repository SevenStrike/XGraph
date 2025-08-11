namespace SevenStrikeModules.XGraph
{
    using System.Collections;
    using UnityEngine;

    public class ActionTreeRunner : MonoBehaviour
    {
        public ActionNode_Asset assetNode;
        private bool isRunning = false;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                FlowStart();
        }

        #region 对外控制
        public void FlowStart()
        {
            if (assetNode == null || assetNode.ActionNodes.Count == 0)
            {
                Debug.LogError("没有行为树资源！");
                return;
            }

            var start = assetNode.ActionNodes.Find(n => n.actionNodeType == "Start");
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

            var start = assetNode.ActionNodes.Find(n =>
            {
                return n.actionNodeType == "Start";
            });
            yield return ExecuteNode(start);

            isRunning = false;
            Debug.Log("----------------->> FlowEnd");
        }
        #endregion

        #region 递归执行节点
        IEnumerator ExecuteNode(ActionNode_Base ActionNode)
        {
            if (ActionNode == null || !isRunning) yield break;

            switch (ActionNode)
            {
                // 组合节点：顺序执行所有子节点
                case ActionNode_Composite composite:
                    ActionNode.Execute();
                    foreach (var child in composite.childNodes)
                        yield return ExecuteNode(child);
                    break;

                // 组合节点：顺序执行所有子节点
                case ActionNode_Relay relay:
                    ActionNode.Execute();
                    foreach (var child in relay.childNodes)
                        yield return ExecuteNode(child);
                    break;

                // 等待节点：先等待，再继续子节点
                case ActionNode_Wait wait:
                    ActionNode.Execute();
                    yield return new WaitForSeconds(wait.Time);
                    yield return ExecuteNode(wait.childNode);
                    break;

                // 调试信息节点：打印信息并继续子节点
                case ActionNode_Debug debug:
                    ActionNode.Execute();
                    yield return ExecuteNode(debug.childNode);
                    break;

                // 开始节点：继续子节点
                case ActionNode_Start start:
                    ActionNode.Execute();
                    yield return ExecuteNode(start.childNode);
                    break;

                // 结束节点：仅打印信息
                case ActionNode_End end:
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
        private ActionNode_Base GetNextNode(ActionNode_Base current)
        {
            return current switch
            {
                ActionNode_Start s => s.childNode,
                ActionNode_Wait w => w.childNode,
                ActionNode_Debug a => a.childNode,
                _ => null
            };
        }
        #endregion
    }
}