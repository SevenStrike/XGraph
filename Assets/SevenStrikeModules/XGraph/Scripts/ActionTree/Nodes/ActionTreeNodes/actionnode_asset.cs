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
    using UnityEngine.InputSystem.HID;
    using static Unity.Burst.Intrinsics.X86.Avx;
    using static UnityEditor.PlayerSettings;
    using Object = UnityEngine.Object;

    [System.Serializable]
    /// <summary>
    /// 便签数据
    /// </summary>
    public class stickdata
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
        public stickdata() { }
        /// <summary>
        /// 便签构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        public stickdata(string name, string content, string guid, Vector2 pos, Vector2 size)
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
        public stickdata Clone(bool guid_create)
        {
            var clone = new stickdata();
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
    public class groupdata
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
        public groupdata Clone(bool guid_create)
        {
            var clone = new groupdata();
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
        public groupdata() { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="guids"></param>
        /// <param name="group"></param>
        public groupdata(string name, string guid, Vector2 pos, List<string> guids, string solution, Group group)
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

    [CreateAssetMenu(fileName = "ActionTree", menuName = "XGraph/ActionTree")]
    public class actionnode_asset : ScriptableObject
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
        [SerializeField] public List<actionnode_base> ActionNodes = new List<actionnode_base>();
        /// <summary>
        /// 便签列表
        /// </summary>
        [SerializeField] public List<stickdata> StickNoteDatas = new List<stickdata>();
        /// <summary>
        /// 编组列表
        /// </summary>
        [SerializeField] public List<groupdata> NodeGroupDatas = new List<groupdata>();

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
        public actionnode_base Create(actionnode_base node)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Added ActionTree Asset");
            // 添加到列表中
            ActionNodes.Add(node);

            // 添加到资源文件下
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();

            // 创建后获取该行为树节点相对行为树资源根节点的路径
            string re_path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ActionNodes[^1]));
            string opt_path = re_path.Replace("Temp", $"{this.name}");
            string combine_path = $"{opt_path}   >   {ActionNodes[^1].name}.asset";
            ActionNodes[^1].path = combine_path;
#endif
            return node;
        }
        /// <summary>
        /// 从列表中移除一个数据节点
        /// </summary>
        /// <param name="node"></param>
        public void Remove(actionnode_base node)
        {
            if (node == null) return;

#if UNITY_EDITOR
            Undo.RecordObject(this, "Removed ChildAction");
            ActionNodes.Remove(node);
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
            foreach (var node in ActionNodes)
            {
                AssetDatabase.RemoveObjectFromAsset(node);
                DestroyImmediate(node, true);
            }

            // 清空资源列表
            ActionNodes.Clear();
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
        public void Replace(actionnode_asset root)
        {
            if (root == null) return;

#if UNITY_EDITOR
            // 清空当前原始资源的所有子节点
            Clear();

            // 覆盖原有的便签数据列表
            StickNoteDatas = new List<stickdata>();
            foreach (var stick in root.StickNoteDatas)
            {
                StickNoteDatas.Add(stick.Clone(false));
            }

            // 覆盖原有的编组数据列表
            NodeGroupDatas = new List<groupdata>();
            foreach (var group in root.NodeGroupDatas)
            {
                NodeGroupDatas.Add(group.Clone(false));
            }

            // 创建新节点副本并添加到原始资源中
            Dictionary<actionnode_base, actionnode_base> dictionary = new Dictionary<actionnode_base, actionnode_base>();
            foreach (var sourceNode in root.ActionNodes)
            {
                var newNode = Instantiate(sourceNode);
                newNode.name = sourceNode.name;
                newNode.hideFlags = HideFlags.None;
                ActionNodes.Add(newNode);
                AssetDatabase.AddObjectToAsset(newNode, this);
                dictionary[sourceNode] = newNode;
            }

            // 重建父子引用关系
            foreach (var source in root.ActionNodes)
            {
                if (source is actionnode_start s && s.ChildNode != null)
                {
                    (dictionary[source] as actionnode_start).ChildNode = dictionary[s.ChildNode];
                }

                if (source is actionnode_wait w && w.ChildNode != null)
                {
                    (dictionary[source] as actionnode_wait).ChildNode = dictionary[w.ChildNode];
                }

                if (source is actionnode_debug d && d.ChildNode != null)
                {
                    (dictionary[source] as actionnode_debug).ChildNode = dictionary[d.ChildNode];
                }

                if (source is actionnode_composite composite && composite.childNodes != null)
                {
                    var newComposite = dictionary[source] as actionnode_composite;
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
        public actionnode_asset Clone(string clonepath = "")
        {
            // 创建新的 actionnode_asset
            actionnode_asset newRoot = ScriptableObject.CreateInstance<actionnode_asset>();

            // 实例化新的 StickNoteDatas 列表，并从原始资源复制项
            newRoot.StickNoteDatas = new List<stickdata>();
            foreach (var item in StickNoteDatas)
            {
                newRoot.StickNoteDatas.Add(item.Clone(false));
            }

            // 实例化新的 groupdata 列表，并从原始资源复制项
            newRoot.NodeGroupDatas = new List<groupdata>();
            foreach (var item in NodeGroupDatas)
            {
                newRoot.NodeGroupDatas.Add(item.Clone(false));
            }

            newRoot.name = this.name + "_CloneRoot";
            newRoot.LastGraphWindowSize = this.LastGraphWindowSize;
            newRoot.LastGraphViewPosition = this.LastGraphViewPosition;
            newRoot.LastGraphViewZoom = this.LastGraphViewZoom;

            // 用于映射原始节点到新节点
            Dictionary<actionnode_base, actionnode_base> originalRootDic = new Dictionary<actionnode_base, actionnode_base>();

            // 第一步：复制所有节点（不处理父子关系）
            foreach (var node in this.ActionNodes)
            {
                actionnode_base newTreeNode = Object.Instantiate(node);
                newTreeNode.name = node.name;
                newTreeNode.hideFlags = HideFlags.None;

                // 关键修复点：初始化时清空所有子引用
                if (newTreeNode is actionnode_start newStart)
                    newStart.ChildNode = null;
                else if (newTreeNode is actionnode_wait newWait)
                    newWait.ChildNode = null;
                else if (newTreeNode is actionnode_debug newDebug)
                    newDebug.ChildNode = null;
                else if (newTreeNode is actionnode_composite newComp)
                    newComp.childNodes.Clear();

                newRoot.ActionNodes.Add(newTreeNode);
                originalRootDic[node] = newTreeNode;
            }

            // 第二步：重建父子关系
            foreach (var node in this.ActionNodes)
            {
                actionnode_base newParentNode = originalRootDic[node];

                // 处理 actionnode_start
                if (node is actionnode_start originalStart)
                {
                    var newStart = newParentNode as actionnode_start;
                    if (originalStart.ChildNode != null && originalRootDic.TryGetValue(originalStart.ChildNode, out var newNode))
                    {
                        newStart.ChildNode = newNode;
                    }
                }

                // 处理 actionnode_wait
                else if (node is actionnode_wait originalWait)
                {
                    var newWait = newParentNode as actionnode_wait;
                    if (originalWait.ChildNode != null && originalRootDic.TryGetValue(originalWait.ChildNode, out var newNode))
                    {
                        newWait.ChildNode = newNode;
                    }
                }

                // 处理 actionnode_debug
                else if (node is actionnode_debug originalDebug)
                {
                    var newDebug = newParentNode as actionnode_debug;
                    if (originalDebug.ChildNode != null && originalRootDic.TryGetValue(originalDebug.ChildNode, out var newNode))
                    {
                        newDebug.ChildNode = newNode;
                    }
                }

                // 处理 actionnode_composite
                else if (node is actionnode_composite originalComposite)
                {
                    var newComposite = newParentNode as actionnode_composite;
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
        public void SaveNodeRootAsset(actionnode_asset root, string path)
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
            foreach (var treenode in root.ActionNodes)
            {
                AssetDatabase.AddObjectToAsset(treenode, root);
            }

            AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 为资源指定子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void AddNodeToChild(actionnode_base parent, actionnode_base child)
        {
            //Debug.Log($"{parent.identifyName}       |  建立链接  √  |      {child.identifyName}");

            #region 特化处理 - Start
            actionnode_start start = parent as actionnode_start;
            if (start)
            {
#if UNITY_EDITOR
                Undo.RecordObject(start, "Connect_StartNode");
#endif
                if (start.ChildNode != null)
                {
                    if (child.guid == start.ChildNode.guid)
                    {
                        Debug.Log("start节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                        return;
                    }
                }
                start.ChildNode = child;
            }
            #endregion

            #region 特化处理 - Wait
            actionnode_wait wait = parent as actionnode_wait;
            if (wait)
            {
#if UNITY_EDITOR
                Undo.RecordObject(wait, "Connect_WaitNode");
#endif
                if (wait.ChildNode != null)
                {
                    if (child.guid == wait.ChildNode.guid)
                    {
                        Debug.Log("wait节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                        return;
                    }
                }
                wait.ChildNode = child;
            }
            #endregion

            #region 特化处理 - Debug
            actionnode_debug debug = parent as actionnode_debug;
            if (debug)
            {
#if UNITY_EDITOR
                Undo.RecordObject(debug, "Connect_DebugNode");
#endif
                if (debug.ChildNode != null)
                {
                    if (child.guid == debug.ChildNode.guid)
                    {
                        Debug.Log("debug节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                        return;
                    }
                }
                debug.ChildNode = child;
            }
            #endregion

            #region 特化处理 - Composite
            actionnode_composite comp = parent as actionnode_composite;
            if (comp)
            {
#if UNITY_EDITOR
                Undo.RecordObject(comp, "Connect_CompositeNode");
#endif
                bool existChild = false;
                comp.childNodes.ForEach(c =>
                {
                    if (child.guid == c.guid)
                        existChild = true;
                });
                if (existChild)
                {
                    Debug.Log("comp 节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                    return;
                }
                comp.childNodes.Add(child);
            }
            #endregion
        }
        /// <summary>
        /// 从指定的父资源中移除子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void RemoveChildNode(actionnode_base parent, actionnode_base child)
        {
            //Debug.Log($"{parent.identifyName}       |  断开链接  ×  |      {c.identifyName}");

            #region 特化处理 - Start
            actionnode_start start = parent as actionnode_start;
            if (start)
            {
#if UNITY_EDITOR
                Undo.RecordObject(start, "RemoveConnect_StartNode");
#endif
                start.ChildNode = null;
            }
            #endregion

            #region 特化处理 - Wait
            actionnode_wait wait = parent as actionnode_wait;
            if (wait)
            {
#if UNITY_EDITOR
                Undo.RecordObject(wait, "RemoveConnect_WaitNode");
#endif
                wait.ChildNode = null;
            }
            #endregion

            #region 特化处理 - Debug
            actionnode_debug debug = parent as actionnode_debug;
            if (debug)
            {
#if UNITY_EDITOR
                Undo.RecordObject(debug, "RemoveConnect_DebugNode");
#endif
                debug.ChildNode = null;
            }
            #endregion

            #region 特化处理 - Composite
            actionnode_composite comp = parent as actionnode_composite;
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
        public List<actionnode_base> GetChildrenNodes(actionnode_base parent)
        {
            List<actionnode_base> nodes = new List<actionnode_base>();

            // 如果是 "actionnode_start" 节点，那么就收集 "actionnode_start" 节点下的 "child"
            actionnode_start start = parent as actionnode_start;
            if (start != null && start.ChildNode != null)
            {
                nodes.Add(start.ChildNode);
            }

            // 如果是 "actionnode_wait" 节点，那么就收集 "actionnode_wait" 节点下的 "child"
            actionnode_wait wait = parent as actionnode_wait;
            if (wait != null && wait.ChildNode != null)
            {
                nodes.Add(wait.ChildNode);
            }

            // 如果是 "actionnode_debug" 节点，那么就收集 "actionnode_debug" 节点下的 "child"
            actionnode_debug debug = parent as actionnode_debug;
            if (debug != null && debug.ChildNode != null)
            {
                nodes.Add(debug.ChildNode);
            }

            // 如果是 "actionnode_composite" 节点，那么就收集 "actionnode_composite" 节点下的 "childNodes"
            actionnode_composite comp = parent as actionnode_composite;
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
        public void StickNote_Add(stickdata data)
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
        public void StickNote_Remove(stickdata data)
        {
            StickNoteDatas.Remove(data);
        }
        #endregion

        #region 编组操作
        /// <summary>
        /// 添加编组数据
        /// </summary>
        /// <param name="data"></param>
        public void NodeGroup_Add(groupdata data)
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
        public void NodeGroup_Remove(groupdata data)
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
            if (obj is actionnode_asset tree)
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