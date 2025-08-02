namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
#endif
    using UnityEngine;
    using Object = UnityEngine.Object;

    [System.Serializable]
    /// <summary>
    /// 便签数据
    /// </summary>
    public class StickNoteData
    {
        /// <summary>
        /// 便签标题
        /// </summary>
        public string name;
        /// <summary>
        /// 便签内容
        /// </summary>
        public string content;
        /// <summary>
        /// 便签识别ID码
        /// </summary>
        public string guid;
        /// <summary>
        /// 节点位置
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// 节点尺寸
        /// </summary>
        public Vector2 size;
        /// <summary>
        /// 便签构造器
        /// </summary>
        public StickNoteData() { }
        /// <summary>
        /// 便签构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        public StickNoteData(string name, string content, string guid, Vector2 pos, Vector2 size)
        {
            this.name = name;
            this.guid = guid;
            this.content = content;
            this.position = pos;
            this.size = size;
        }
        /// <summary>
        /// 便签克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public StickNoteData Clone(bool guid_create)
        {
            var clone = new StickNoteData();
            clone.name = name;
            clone.content = content;
#if UNITY_EDITOR
            clone.guid = guid_create ? GUID.Generate().ToString() : guid;
#endif
            clone.position = position;
            clone.size = size;
            return clone;
        }
    }

    [System.Serializable]
    /// <summary>
    /// 编组数据
    /// </summary>
    public class NodeGroupData
    {
#if UNITY_EDITOR
        /// <summary>
        /// 编组的标题
        /// </summary>
        public string name;
        /// <summary>
        /// 编组的识别ID码
        /// </summary>
        public string guid;
        /// <summary>
        /// 编组的位置
        /// </summary>
        public Vector2 pos;
        /// <summary>
        /// 编组的颜色识别
        /// </summary>
        public string solution = "M 默认";
        /// <summary>
        /// 编组组件
        /// </summary>
        [SerializeField] public Group group;
        /// <summary>
        /// 编组内的所有节点的识别ID码
        /// </summary>
        public List<string> guids = new List<string>();

        /// <summary>
        /// 编组克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public NodeGroupData Clone(bool guid_create)
        {
            var clone = new NodeGroupData();
            clone.name = name;
            clone.guid = guid_create ? GUID.Generate().ToString() : guid;
            clone.pos = pos;
            clone.guids = guids;
            clone.solution = solution;
            clone.group = null;
            return clone;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public NodeGroupData() { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="guids"></param>
        /// <param name="group"></param>
        public NodeGroupData(string name, string guid, Vector2 pos, List<string> guids, string solution, Group group)
        {
            this.name = name;
            this.guid = guid;
            this.pos = pos;
            this.guids = guids;
            this.solution = solution;
            this.group = group;
        }
#endif
    }

    [Serializable]
    public class RelayNodeData
    {
        public string guid;           // 唯一标识
        public Vector2 position;      // 节点位置
        public string inputGuid;      // 输入连接的节点GUID
        public string outputGuid;     // 输出连接的节点GUID
    }

    [CreateAssetMenu(fileName = "ActionTree", menuName = "XGraph/ActionTree")]
    public class ActionTree_Nodes_Asset : ScriptableObject
    {
        /// <summary>
        /// 记录的节点编辑器最后一次的窗口尺寸
        /// </summary>
        [SerializeField] public Vector2Int LastGraphWindowSize;
        /// <summary>
        /// 记录的节点编辑器最后一次的视图内位置
        /// </summary>
        [SerializeField] public Vector2 LastGraphViewPosition = Vector2.zero;
        /// <summary>
        /// 记录的节点编辑器最后一次的视图内缩放
        /// </summary>
        [SerializeField] public float LastGraphViewZoom = 1;
        /// <summary>
        /// 数据节点列表
        /// </summary>
        [SerializeField] public List<ActionTree_Node_Base> ActionTreeNodes = new List<ActionTree_Node_Base>();
        /// <summary>
        /// 便签列表
        /// </summary>
        [SerializeField] public List<StickNoteData> StickNoteDatas = new List<StickNoteData>();
        /// <summary>
        /// 编组列表
        /// </summary>
        [SerializeField] public List<NodeGroupData> NodeGroupDatas = new List<NodeGroupData>();
        /// <summary>
        /// 中继器列表
        /// </summary>
        [SerializeField] public List<RelayNodeData> relayNodes = new List<RelayNodeData>();

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {

        }

        #region 资源操作
        /// <summary>
        /// 创建数据节点到列表中
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionTree_Node_Base Create(ActionTree_Node_Base node)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Added ActionTree Asset");
            // 添加到列表中
            ActionTreeNodes.Add(node);

            // 添加到资源文件下
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();

            // 创建后获取该行为树节点相对行为树资源根节点的路径
            string re_path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ActionTreeNodes[^1]));
            string opt_path = re_path.Replace("Temp", $"{this.name}");
            string combine_path = $"{opt_path}   >   {ActionTreeNodes[^1].name}.asset";
            ActionTreeNodes[^1].nodePath = combine_path;
#endif
            return node;
        }
        /// <summary>
        /// 从列表中移除一个数据节点
        /// </summary>
        /// <param name="node"></param>
        public void Remove(ActionTree_Node_Base node)
        {
            if (node == null) return;

#if UNITY_EDITOR
            Undo.RecordObject(this, "Removed ChildAction");
            ActionTreeNodes.Remove(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        /// <summary>
        /// 从列表中清空所有数据节点
        /// </summary>
        public void Clear()
        {
#if UNITY_EDITOR
            // 移除子级的所有资源
            foreach (var node in ActionTreeNodes)
            {
                AssetDatabase.RemoveObjectFromAsset(node);
                DestroyImmediate(node, true);
            }

            // 清空资源列表
            ActionTreeNodes.Clear();
            // 清空便签列表
            StickNote_Clear();
            // 清空编组列表
            NodeGroup_Clear();

            // 刷新资源状态
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        /// <summary>
        /// 使用目标资源替换当前资源
        /// </summary>
        /// <param name="root"></param>
        public void Replace(ActionTree_Nodes_Asset root)
        {
            if (root == null) return;

#if UNITY_EDITOR
            // 清空当前原始资源的所有子节点
            Clear();

            // 覆盖原有的便签数据列表
            StickNoteDatas = new List<StickNoteData>();
            foreach (var stick in root.StickNoteDatas)
            {
                StickNoteDatas.Add(stick.Clone(false));
            }

            // 覆盖原有的编组数据列表
            NodeGroupDatas = new List<NodeGroupData>();
            foreach (var group in root.NodeGroupDatas)
            {
                NodeGroupDatas.Add(group.Clone(false));
            }

            // 创建新节点副本并添加到原始资源中
            Dictionary<ActionTree_Node_Base, ActionTree_Node_Base> dictionary = new Dictionary<ActionTree_Node_Base, ActionTree_Node_Base>();
            foreach (var sourceNode in root.ActionTreeNodes)
            {
                var newNode = Instantiate(sourceNode);
                newNode.name = sourceNode.name;
                newNode.hideFlags = HideFlags.None;
                ActionTreeNodes.Add(newNode);
                AssetDatabase.AddObjectToAsset(newNode, this);
                dictionary[sourceNode] = newNode;
            }

            // 重建父子引用关系
            foreach (var source in root.ActionTreeNodes)
            {
                if (source is ActionTree_Node_Start s && s.ChildNode != null)
                {
                    (dictionary[source] as ActionTree_Node_Start).ChildNode = dictionary[s.ChildNode];
                }

                if (source is ActionTree_Node_Wait w && w.ChildNode != null)
                {
                    (dictionary[source] as ActionTree_Node_Wait).ChildNode = dictionary[w.ChildNode];
                }

                if (source is ActionTree_Node_Debug d && d.ChildNode != null)
                {
                    (dictionary[source] as ActionTree_Node_Debug).ChildNode = dictionary[d.ChildNode];
                }

                if (source is ActionTree_Node_Composite composite && composite.childNodes != null)
                {
                    var newComposite = dictionary[source] as ActionTree_Node_Composite;
                    newComposite.childNodes.Clear();
                    foreach (var node in composite.childNodes)
                    {
                        newComposite.childNodes.Add(dictionary[node]);
                    }
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 创建当前流程设计的克隆体（仅编辑器下）
        /// </summary>
        /// <returns></returns>
        public ActionTree_Nodes_Asset Clone(string clonepath = "")
        {
            // 创建新的 ActionTree_Nodes_Asset
            ActionTree_Nodes_Asset newRoot = ScriptableObject.CreateInstance<ActionTree_Nodes_Asset>();

            // 实例化新的 StickNoteDatas 并从原始资源复制项
            newRoot.StickNoteDatas = new List<StickNoteData>();
            foreach (var item in StickNoteDatas)
            {
                newRoot.StickNoteDatas.Add(item.Clone(false));
            }

            // 实例化新的 NodeGroupData 并从原始资源复制项
            newRoot.NodeGroupDatas = new List<NodeGroupData>();
            foreach (var item in NodeGroupDatas)
            {
                newRoot.NodeGroupDatas.Add(item.Clone(false));
            }

            newRoot.name = this.name + "_CloneRoot";
            newRoot.LastGraphWindowSize = this.LastGraphWindowSize;
            newRoot.LastGraphViewPosition = this.LastGraphViewPosition;
            newRoot.LastGraphViewZoom = this.LastGraphViewZoom;

            // 用于映射原始节点到新节点
            Dictionary<ActionTree_Node_Base, ActionTree_Node_Base> originalRootDic = new Dictionary<ActionTree_Node_Base, ActionTree_Node_Base>();

            // 第一步：复制所有节点（不处理父子关系）
            foreach (var node in this.ActionTreeNodes)
            {
                ActionTree_Node_Base newTreeNode = Object.Instantiate(node);
                newTreeNode.name = node.name;
                newTreeNode.hideFlags = HideFlags.None;

                // 关键修复点：初始化时清空所有子引用
                if (newTreeNode is ActionTree_Node_Start newStart)
                    newStart.ChildNode = null;
                else if (newTreeNode is ActionTree_Node_Wait newWait)
                    newWait.ChildNode = null;
                else if (newTreeNode is ActionTree_Node_Debug newDebug)
                    newDebug.ChildNode = null;
                else if (newTreeNode is ActionTree_Node_Composite newComp)
                    newComp.childNodes.Clear();

                newRoot.ActionTreeNodes.Add(newTreeNode);
                originalRootDic[node] = newTreeNode;
            }

            // 第二步：重建父子关系
            foreach (var node in this.ActionTreeNodes)
            {
                ActionTree_Node_Base newParentNode = originalRootDic[node];

                // 处理 ActionTree_Node_Start
                if (node is ActionTree_Node_Start originalStart)
                {
                    var newStart = newParentNode as ActionTree_Node_Start;
                    if (originalStart.ChildNode != null && originalRootDic.TryGetValue(originalStart.ChildNode, out var newNode))
                    {
                        newStart.ChildNode = newNode;
                    }
                }

                // 处理 ActionTree_Node_Wait
                else if (node is ActionTree_Node_Wait originalWait)
                {
                    var newWait = newParentNode as ActionTree_Node_Wait;
                    if (originalWait.ChildNode != null && originalRootDic.TryGetValue(originalWait.ChildNode, out var newNode))
                    {
                        newWait.ChildNode = newNode;
                    }
                }

                // 处理 ActionTree_Node_Debug
                else if (node is ActionTree_Node_Debug originalDebug)
                {
                    var newDebug = newParentNode as ActionTree_Node_Debug;
                    if (originalDebug.ChildNode != null && originalRootDic.TryGetValue(originalDebug.ChildNode, out var newNode))
                    {
                        newDebug.ChildNode = newNode;
                    }
                }

                // 处理 ActionTree_Node_Composite
                else if (node is ActionTree_Node_Composite originalComposite)
                {
                    var newComposite = newParentNode as ActionTree_Node_Composite;
                    foreach (var originalChild in originalComposite.childNodes)
                    {
                        if (originalRootDic.TryGetValue(originalChild, out var newChild))
                        {
                            newComposite.childNodes.Add(newChild);
                        }
                    }
                }
            }
            SaveNodeRootAsset(newRoot, string.IsNullOrEmpty(clonepath) ? $"{util_Dashboard.GetPath_Temp()}/CloneTree.asset" : clonepath);

            return newRoot;
        }
        /// <summary>
        /// 保存为Tree资源到目标路径下
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        public void SaveNodeRootAsset(ActionTree_Nodes_Asset root, string path)
        {
#if UNITY_EDITOR
            // 提取根路径整理 - 去掉尾部的 /
            string path_root = util_Dashboard.GetPath_Root();
            path_root = path_root.Substring(0, path_root.Length - 1);

            // 目标路径整理 - 去掉尾部的 /
            string path_target = $"{util_Dashboard.GetPath_Temp()}";
            path_target = path_target.Substring(0, path_target.Length - 1);

            // 判断是否存在目标路径，如果不存在就创建该路径的文件夹
            if (!AssetDatabase.AssetPathExists(path_target))
            {
                AssetDatabase.CreateFolder(path_root, "Temp");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            // 保存为临时.asset 文件，供Unity资源系统进行操作
            AssetDatabase.CreateAsset(root, path);
            foreach (var treenode in root.ActionTreeNodes)
            {
                AssetDatabase.AddObjectToAsset(treenode, root);
            }

            AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 为资源指定子资源
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNode"></param>
        public void AddNodeToChild(ActionTree_Node_Base parentNode, ActionTree_Node_Base childNode)
        {
            //Debug.Log($"{parentNode.nodeName}       |  建立链接  √  |      {childNode.nodeName}");

            #region 特化处理 - Start
            ActionTree_Node_Start start = parentNode as ActionTree_Node_Start;
            if (start)
            {
#if UNITY_EDITOR
                Undo.RecordObject(start, "Connect_StartNode");
#endif
                start.ChildNode = childNode;
            }
            #endregion

            #region 特化处理 - Wait
            ActionTree_Node_Wait wait = parentNode as ActionTree_Node_Wait;
            if (wait)
            {
#if UNITY_EDITOR
                Undo.RecordObject(wait, "Connect_WaitNode");
#endif
                wait.ChildNode = childNode;
            }
            #endregion

            #region 特化处理 - Debug
            ActionTree_Node_Debug debug = parentNode as ActionTree_Node_Debug;
            if (debug)
            {
#if UNITY_EDITOR
                Undo.RecordObject(debug, "Connect_DebugNode");
#endif
                debug.ChildNode = childNode;
            }
            #endregion

            #region 特化处理 - Composite
            ActionTree_Node_Composite comp = parentNode as ActionTree_Node_Composite;
            if (comp)
            {
#if UNITY_EDITOR
                Undo.RecordObject(comp, "Connect_CompositeNode");
#endif
                comp.childNodes.Add(childNode);
            }
            #endregion
        }
        /// <summary>
        /// 从指定的父资源中移除子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void RemoveChildNode(ActionTree_Node_Base parent, ActionTree_Node_Base child)
        {
            //Debug.Log($"{parent.nodeName}       |  断开链接  ×  |      {child.nodeName}");

            #region 特化处理 - Start
            ActionTree_Node_Start start = parent as ActionTree_Node_Start;
            if (start)
            {
#if UNITY_EDITOR
                Undo.RecordObject(start, "RemoveConnect_StartNode");
#endif
                start.ChildNode = null;
            }
            #endregion

            #region 特化处理 - Wait
            ActionTree_Node_Wait wait = parent as ActionTree_Node_Wait;
            if (wait)
            {
#if UNITY_EDITOR
                Undo.RecordObject(wait, "RemoveConnect_WaitNode");
#endif
                wait.ChildNode = null;
            }
            #endregion

            #region 特化处理 - Debug
            ActionTree_Node_Debug debug = parent as ActionTree_Node_Debug;
            if (debug)
            {
#if UNITY_EDITOR
                Undo.RecordObject(debug, "RemoveConnect_DebugNode");
#endif
                debug.ChildNode = null;
            }
            #endregion

            #region 特化处理 - Composite
            ActionTree_Node_Composite comp = parent as ActionTree_Node_Composite;
            if (comp)
            {
#if UNITY_EDITOR
                Undo.RecordObject(comp, "RemoveConnect_CompositeNode");
#endif
                comp.childNodes.Remove(child);
            }
            #endregion

        }
        /// <summary>
        /// 获取指定数据节点的子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<ActionTree_Node_Base> GetChildrenNodes(ActionTree_Node_Base parent)
        {
            List<ActionTree_Node_Base> nodes = new List<ActionTree_Node_Base>();

            // 如果是 "ActionTree_Node_Start" 节点，那么就收集 "ActionTree_Node_Start" 节点下的 "childNode"
            ActionTree_Node_Start start = parent as ActionTree_Node_Start;
            if (start != null && start.ChildNode != null)
            {
                nodes.Add(start.ChildNode);
            }

            // 如果是 "ActionTree_Node_Wait" 节点，那么就收集 "ActionTree_Node_Wait" 节点下的 "childNode"
            ActionTree_Node_Wait wait = parent as ActionTree_Node_Wait;
            if (wait != null && wait.ChildNode != null)
            {
                nodes.Add(wait.ChildNode);
            }

            // 如果是 "ActionTree_Node_Debug" 节点，那么就收集 "ActionTree_Node_Debug" 节点下的 "childNode"
            ActionTree_Node_Debug debug = parent as ActionTree_Node_Debug;
            if (debug != null && debug.ChildNode != null)
            {
                nodes.Add(debug.ChildNode);
            }

            // 如果是 "ActionTree_Node_Composite" 节点，那么就收集 "ActionTree_Node_Composite" 节点下的 "childNodes"
            ActionTree_Node_Composite comp = parent as ActionTree_Node_Composite;
            if (comp != null && comp.childNodes != null)
            {
                nodes = comp.childNodes;
            }

            // 返回的列表就是 "GraphView组件" 那边需要根据这子资源列表才能知道跟哪些子节点重建节点之间的连线
            return nodes;
        }
        #endregion

        #region 便签操作
        /// <summary>
        /// 添加便签数据
        /// </summary>
        /// <param name="data"></param>
        public void StickNote_Add(StickNoteData data)
        {
            StickNoteDatas.Add(data);
        }
        /// <summary>
        /// 清空便签数据列表
        /// </summary>
        public void StickNote_Clear()
        {
            StickNoteDatas.Clear();
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标便签数据
        /// </summary>
        /// <param name="data"></param>
        public void StickNote_Remove(StickNoteData data)
        {
            StickNoteDatas.Remove(data);
        }
        #endregion

        #region 编组操作
        /// <summary>
        /// 添加编组数据
        /// </summary>
        /// <param name="data"></param>
        public void NodeGroup_Add(NodeGroupData data)
        {
            NodeGroupDatas.Add(data);
        }
        /// <summary>
        /// 清空编组数据列表
        /// </summary>
        public void NodeGroup_Clear()
        {
            NodeGroupDatas.Clear();
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 移除目标编组数据
        /// </summary>
        /// <param name="data"></param>
        public void NodeGroup_Remove(NodeGroupData data)
        {
            NodeGroupDatas.Remove(data);
        }
        #endregion

        #region 快速清除子资源
#if UNITY_EDITOR
        [MenuItem("Assets/XGraph/清空所有子级资源")]
        public static void at_ClearChildNodes()
        {
            Object obj = Selection.activeObject;
            if (obj is ActionTree_Nodes_Asset tree)
            {
                tree.Clear();
            }
        }

        [MenuItem("Assets/XGraph/复位编辑器窗口配置")]
        public static void at_ClearGraphWindowConfigs()
        {
            EditorPrefs.DeleteKey("XGraph_InspectorViewPosition");
            EditorPrefs.DeleteKey("XGraph_InspectorViewSize");
            EditorPrefs.DeleteKey("XGraph_InspectorViewDisplay");
        }
#endif
        #endregion
    }
}