namespace SevenStrikeModules.XGraph
{
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

    [System.Serializable]
    public class BlackboardVariable
    {
        public string name;
        public BlackboardVariableType type;
        public string stringValue;
        public float floatValue;
        public int intValue;
        public bool boolValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public Vector4 vector4Value;
        public UnityEngine.Object objectValue;

        /// <summary>
        /// 黑板属性克隆
        /// </summary>
        /// <returns></returns>
        public BlackboardVariable Clone()
        {
            var clone = new BlackboardVariable();
            clone.name = name;
            clone.type = type;
            clone.stringValue = stringValue;
            clone.floatValue = floatValue;
            clone.intValue = intValue;
            clone.boolValue = boolValue;
            clone.vector2Value = vector2Value;
            clone.vector3Value = vector3Value;
            clone.vector4Value = vector4Value;
            clone.objectValue = objectValue;
            return clone;
        }

        /// <summary>
        /// 黑板属性构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="stringValue"></param>
        /// <param name="floatValue"></param>
        /// <param name="intValue"></param>
        /// <param name="boolValue"></param>
        /// <param name="vector2Value"></param>
        /// <param name="vector3Value"></param>
        /// <param name="vector4Value"></param>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        public BlackboardVariable(string name = null, BlackboardVariableType type = BlackboardVariableType.String, string stringValue = null, float floatValue = 0f, int intValue = 0, bool boolValue = false, Vector2 vector2Value = default, Vector3 vector3Value = default, Vector4 vector4Value = default, Object objectValue = null)
        {
            this.name = name;
            this.type = type;
            this.stringValue = stringValue;
            this.floatValue = floatValue;
            this.intValue = intValue;
            this.boolValue = boolValue;
            this.vector2Value = vector2Value;
            this.vector3Value = vector3Value;
            this.vector4Value = vector4Value;
            this.objectValue = objectValue;
        }

        /// <summary>
        /// 黑板属性构造
        /// </summary>
        public BlackboardVariable() { }
    }

    /// <summary>
    /// 黑板值类型
    /// </summary>
    public enum BlackboardVariableType
    {
        /// <summary>
        /// 值 - 字符串
        /// </summary>
        String = 0,
        /// <summary>
        /// 值 - 浮点
        /// </summary>
        Float = 1,
        /// <summary>
        /// 值 - 整数
        /// </summary>
        Int = 2,
        /// <summary>
        /// 值 - 布尔开关
        /// </summary>
        Bool = 3,
        /// <summary>
        /// 值 - 2维向量
        /// </summary>
        Vector2 = 4,
        /// <summary>
        /// 值 - 3维向量
        /// </summary>
        Vector3 = 5,
        /// <summary>
        /// 值 - 4维向量
        /// </summary>
        Vector4 = 6,
        /// <summary>
        /// 值 - 物体
        /// </summary>
        Object = 7
    }

    [CreateAssetMenu(fileName = "ActionTree", menuName = "XGraph/ActionGraphAsset")]
    public class ActionNode_Asset : ScriptableObject
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
        [SerializeField] public List<ActionNode_Base> ActionNodes = new List<ActionNode_Base>();
        /// <summary>
        /// 便签列表
        /// </summary>
        [SerializeField] public List<stickdata> StickNoteDatas = new List<stickdata>();
        /// <summary>
        /// 编组列表
        /// </summary>
        [SerializeField] public List<groupdata> NodeGroupDatas = new List<groupdata>();
        /// <summary>
        /// 黑板值列表
        /// </summary>
        [SerializeField] public List<BlackboardVariable> BlackboardVariables = new List<BlackboardVariable>();

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
        public ActionNode_Base Create(ActionNode_Base node)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Added ActionTree Asset");
            // 添加到列表中
            ActionNodes.Add(node);

            // 添加到资源文件下
            AssetDatabase.AddObjectToAsset(node, this);
            //AssetDatabase.SaveAssets();

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
        public void Remove(ActionNode_Base node)
        {
            if (node == null) return;

#if UNITY_EDITOR
            Undo.RecordObject(this, "Removed ChildAction");
            ActionNodes.Remove(node);
            Undo.DestroyObjectImmediate(node);
            //AssetDatabase.SaveAssets();
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
            //AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        /// <summary>
        /// 使用目标资源替换当前资源
        /// </summary>
        /// <param name="root"></param>
        public void Replace(ActionNode_Asset root)
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

            // 覆盖原有的黑板数据列表
            BlackboardVariables = new List<BlackboardVariable>();
            foreach (var bbv in root.BlackboardVariables)
            {
                BlackboardVariables.Add(bbv.Clone());
            }

            // 创建新节点副本并添加到原始资源中
            Dictionary<ActionNode_Base, ActionNode_Base> dictionary = new Dictionary<ActionNode_Base, ActionNode_Base>();
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
                if (source is ActionNode_Start s && s.childNode != null)
                {
                    (dictionary[source] as ActionNode_Start).childNode = dictionary[s.childNode];
                }

                if (source is ActionNode_Debug d && d.childNode != null)
                {
                    (dictionary[source] as ActionNode_Debug).childNode = dictionary[d.childNode];
                }

                if (source is ActionNode_Wait w && w.childNodes != null)
                {
                    //(dictionary[source] as ActionNode_Wait).childNode = dictionary[w.childNode];
                    var newComposite = dictionary[source] as ActionNode_Wait;
                    newComposite.childNodes.Clear();
                    foreach (var node in w.childNodes)
                    {
                        newComposite.childNodes.Add(dictionary[node]);
                    }
                }

                if (source is ActionNode_Composite c && c.childNodes != null)
                {
                    var newComposite = dictionary[source] as ActionNode_Composite;
                    newComposite.childNodes.Clear();
                    foreach (var node in c.childNodes)
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
        public ActionNode_Asset Clone(string clonepath = "")
        {
            // 创建新的 ActionNode_Asset
            ActionNode_Asset newRoot = ScriptableObject.CreateInstance<ActionNode_Asset>();

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
#if UNITY_EDITOR
                newRoot.NodeGroupDatas.Add(item.Clone(false));
#endif
            }

            // 实例化新的 blackboardVariable 列表，并从原始资源复制项
            newRoot.BlackboardVariables = new List<BlackboardVariable>();
            foreach (var bbv in BlackboardVariables)
            {
#if UNITY_EDITOR
                newRoot.BlackboardVariables.Add(bbv.Clone());
#endif
            }

            newRoot.name = this.name + "_CloneRoot";
            newRoot.LastGraphWindowSize = this.LastGraphWindowSize;
            newRoot.LastGraphViewPosition = this.LastGraphViewPosition;
            newRoot.LastGraphViewZoom = this.LastGraphViewZoom;

            // 用于映射原始节点到新节点
            Dictionary<ActionNode_Base, ActionNode_Base> originalRootDic = new Dictionary<ActionNode_Base, ActionNode_Base>();

            // 第一步：复制所有节点（不处理父子关系）
            foreach (var node in this.ActionNodes)
            {
                ActionNode_Base newTreeNode = Object.Instantiate(node);
                newTreeNode.name = node.name;
                newTreeNode.hideFlags = HideFlags.None;

                // 关键修复点：初始化时清空所有子引用
                if (newTreeNode is ActionNode_Start newStart)
                    newStart.childNode = null;
                else if (newTreeNode is ActionNode_Wait newWait)
                    newWait.childNodes.Clear();
                else if (newTreeNode is ActionNode_Debug newDebug)
                    newDebug.childNode = null;
                else if (newTreeNode is ActionNode_Composite newComp)
                    newComp.childNodes.Clear();

                newRoot.ActionNodes.Add(newTreeNode);
                originalRootDic[node] = newTreeNode;
            }

            // 第二步：重建父子关系
            foreach (var node in this.ActionNodes)
            {
                ActionNode_Base newParentNode = originalRootDic[node];

                // 处理 ActionNode_Start
                if (node is ActionNode_Start originalStart)
                {
                    var newStart = newParentNode as ActionNode_Start;
                    if (originalStart.childNode != null && originalRootDic.TryGetValue(originalStart.childNode, out var newNode))
                    {
                        newStart.childNode = newNode;
                    }
                }

                // 处理 ActionNode_Wait
                else if (node is ActionNode_Wait originalWait)
                {
                    var newWait = newParentNode as ActionNode_Wait;
                    foreach (var originalChild in originalWait.childNodes)
                    {
                        if (originalRootDic.TryGetValue(originalChild, out var newChild))
                        {
                            newWait.childNodes.Add(newChild);
                        }
                    }
                }

                // 处理 ActionNode_Debug
                else if (node is ActionNode_Debug originalDebug)
                {
                    var newDebug = newParentNode as ActionNode_Debug;
                    if (originalDebug.childNode != null && originalRootDic.TryGetValue(originalDebug.childNode, out var newNode))
                    {
                        newDebug.childNode = newNode;
                    }
                }

                // 处理 ActionNode_Composite
                else if (node is ActionNode_Composite originalComposite)
                {
                    var newComposite = newParentNode as ActionNode_Composite;
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
        public void SaveNodeRootAsset(ActionNode_Asset root, string path)
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
                //AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            // 保存为临时.asset 文件，供Unity资源系统进行操作
            AssetDatabase.CreateAsset(root, path);
            foreach (var treenode in root.ActionNodes)
            {
                AssetDatabase.AddObjectToAsset(treenode, root);
            }

            //AssetDatabase.SaveAssets();
#endif
        }
        /// <summary>
        /// 获取指定数据节点的子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<ActionNode_Base> GetChildrenNodes(ActionNode_Base parent)
        {
            List<ActionNode_Base> nodes = new List<ActionNode_Base>();

            // 如果是 "ActionNode_Start" 节点，那么就收集 "ActionNode_Start" 节点下的 "child"
            ActionNode_Start start = parent as ActionNode_Start;
            if (start != null && start.childNode != null)
            {
                nodes.Add(start.childNode);
            }

            // 如果是 "ActionNode_Wait" 节点，那么就收集 "ActionNode_Wait" 节点下的 "child"
            ActionNode_Wait wait = parent as ActionNode_Wait;
            if (wait != null && wait.childNodes != null)
            {
                nodes = wait.childNodes;
            }

            // 如果是 "ActionNode_Debug" 节点，那么就收集 "ActionNode_Debug" 节点下的 "child"
            ActionNode_Debug debug = parent as ActionNode_Debug;
            if (debug != null && debug.childNode != null)
            {
                nodes.Add(debug.childNode);
            }

            // 如果是 "ActionNode_Composite" 节点，那么就收集 "ActionNode_Composite" 节点下的 "childNodes"
            ActionNode_Composite comp = parent as ActionNode_Composite;
            if (comp != null && comp.childNodes != null)
            {
                nodes = comp.childNodes;
            }

            // 返回的列表就是 "GraphView组件" 那边需要根据这子资源列表才能知道跟哪些子节点重建节点之间的连线
            return nodes;
        }
        /// <summary>
        /// 为资源指定子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void ChildNode_Add(ActionNode_Base parent, ActionNode_Base child)
        {
            //Debug.Log($"{parent.identifyName}       |  建立链接  √  |      {child.identifyName}");

            #region 特化处理 - Start
            ActionNode_Start start = parent as ActionNode_Start;
            if (start)
            {
#if UNITY_EDITOR
                Undo.RecordObject(start, "Connect_StartNode");
#endif
                if (start.childNode != null)
                {
                    if (child.guid == start.childNode.guid)
                    {
                        Debug.Log("start节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                        return;
                    }
                }
                start.childNode = child;
            }
            #endregion

            #region 特化处理 - Wait
            ActionNode_Wait wait = parent as ActionNode_Wait;
            if (wait)
            {
#if UNITY_EDITOR
                Undo.RecordObject(wait, "Connect_WaitNode");
#endif
                bool existChild = false;
                wait.childNodes.ForEach(c =>
                {
                    if (child.guid == c.guid)
                        existChild = true;
                });
                if (existChild)
                {
                    Debug.Log("wait 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                wait.childNodes.Add(child);
            }
            #endregion

            #region 特化处理 - Debug
            ActionNode_Debug debug = parent as ActionNode_Debug;
            if (debug)
            {
#if UNITY_EDITOR
                Undo.RecordObject(debug, "Connect_DebugNode");
#endif
                if (debug.childNode != null)
                {
                    if (child.guid == debug.childNode.guid)
                    {
                        Debug.Log("debug节点已经存在因删除Relay后的重新添加的指定资源！忽略它！");
                        return;
                    }
                }
                debug.childNode = child;
            }
            #endregion

            #region 特化处理 - Composite
            ActionNode_Composite comp = parent as ActionNode_Composite;
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
                    Debug.Log("comp 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                comp.childNodes.Add(child);
            }
            #endregion

            #region 特化处理 - Relay
            ActionNode_Relay relay = parent as ActionNode_Relay;
            if (relay)
            {
#if UNITY_EDITOR
                Undo.RecordObject(relay, "Connect_RelayNode");
#endif
                bool existChild = false;
                relay.childNodes.ForEach(c =>
                {
                    if (child.guid == c.guid)
                        existChild = true;
                });
                if (existChild)
                {
                    Debug.Log("comp 节点已经存在添加的指定资源！忽略它！");
                    return;
                }
                relay.childNodes.Add(child);
            }
            #endregion
        }
        /// <summary>
        /// 从指定的父资源中移除子资源
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void ChildNode_Remove(ActionNode_Base parent, ActionNode_Base child)
        {
            //Debug.Log($"{parent.identifyName}       |  断开链接  ×  |      {c.identifyName}");

            #region 特化处理 - Start
            ActionNode_Start start = parent as ActionNode_Start;
            if (start)
            {
#if UNITY_EDITOR
                Undo.RecordObject(start, "RemoveConnect_StartNode");
#endif
                start.childNode = null;
            }
            #endregion

            #region 特化处理 - Wait
            ActionNode_Wait wait = parent as ActionNode_Wait;
            if (wait)
            {
#if UNITY_EDITOR
                Undo.RecordObject(wait, "RemoveConnect_WaitNode");
#endif
                wait.childNodes.Remove(child);
            }
            #endregion

            #region 特化处理 - Debug
            ActionNode_Debug debug = parent as ActionNode_Debug;
            if (debug)
            {
#if UNITY_EDITOR
                Undo.RecordObject(debug, "RemoveConnect_DebugNode");
#endif
                debug.childNode = null;
            }
            #endregion

            #region 特化处理 - Composite
            ActionNode_Composite comp = parent as ActionNode_Composite;
            if (comp)
            {
#if UNITY_EDITOR
                Undo.RecordObject(comp, "RemoveConnect_CompositeNode");
#endif
                comp.childNodes.Remove(child);
            }
            #endregion

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
            //AssetDatabase.SaveAssets();
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
            BlackboardVariables.Clear();
#if UNITY_EDITOR
            //AssetDatabase.SaveAssets();
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
    }
}