namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Collections.Generic;
    using Unity.Plastic.Newtonsoft.Json;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    #region 解析节点菜单结构
    [Serializable]
    public class searchBox_NodesRoot
    {
        /// <summary>
        /// 根级标题
        /// </summary>
        public string root_title;
        /// <summary>
        /// 根级分类
        /// </summary>
        public List<searchBox_Nodes> root_types = new List<searchBox_Nodes>();
    }

    [Serializable]
    public class searchBox_Nodes
    {
        /// <summary>
        /// 节点一级分类标题
        /// </summary>
        public string catergory_title;
        /// <summary>
        /// 节点一级分类图标
        /// </summary>
        public string catergory_icon;
        /// <summary>
        /// 节点一级分类结构
        /// </summary>
        public List<searchBox_Node> catergory_nodes = new List<searchBox_Node>();
    }

    [Serializable]
    public class searchBox_Node
    {
        /// <summary>
        /// 节点级类型标题
        /// </summary>
        public string name;
        /// <summary>
        /// 节点级类型命名空间 - 前缀
        /// </summary>
        public string prefixNamespace;
        /// <summary>
        /// 节点级类型类名 - 前缀
        /// </summary>
        public string prefixClass;
        /// <summary>
        /// 节点级类型行为节点枚举
        /// </summary>
        public string actionNodeType;
        /// <summary>
        /// 图标名称
        /// </summary>
        public string iconName;
        /// <summary>
        /// 节点级类型视觉节点枚举
        /// </summary>
        public string visualNodeType;
    }
    #endregion

    /// <summary>
    /// 用于将从Json解析的节点菜单项传递参数给创建节点菜单列表的结构体
    /// </summary>
    public struct NodeMenuParam
    {
        /// <summary>
        /// 节点显示名称
        /// </summary>
        public string visual_name;
        /// <summary>
        /// 命名空间前缀
        /// </summary>
        public string prefix_namespace;
        /// <summary>
        /// 类名前缀
        /// </summary>
        public string prefix_class;
        /// <summary>
        /// 行为节点枚举类型
        /// </summary>
        public string action_nodeType;
        /// <summary>
        /// 图标名称
        /// </summary>
        public string icon;
        /// <summary>
        /// 视觉节点枚举类型
        /// </summary>
        public string visual_nodeType;

        public NodeMenuParam(string _name, string prefix_namespace, string prefix_class, string action_nodeType, string icon, string visual_nodeType)
        {
            visual_name = _name;
            this.prefix_namespace = prefix_namespace;
            this.prefix_class = prefix_class;
            this.action_nodeType = action_nodeType;
            this.icon = icon;
            this.visual_nodeType = visual_nodeType;
        }
    }

    /// <summary>
    /// 此类为GraphView实现节点创建的搜索框结构
    /// </summary>
    public partial class xg_NodesSearchBox : ScriptableObject, ISearchWindowProvider
    {
        /// <summary>
        /// xw_graphView 主视图
        /// </summary>
        private xg_GraphView graphView;

        /// <summary>
        /// 初始化节点搜索框
        /// </summary>
        /// <param catergory_title="graphView"></param>
        public void Init(xg_GraphView graphView)
        {
            this.graphView = graphView;
        }

        /// <summary>
        /// 创建搜索树
        /// </summary>
        /// <param catergory_title="context"></param>
        /// <returns></returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return NodeMenuListStructure();
        }

        /// <summary>
        /// 将 json 数据 转化为搜索树菜单列表内容
        /// </summary>
        /// <returns></returns>
        private List<SearchTreeEntry> NodeMenuListStructure()
        {
            // 读取菜单结构列表内容
            graphView.SearchStructures_Json = util_XGraphEditorUtility.AssetLoad<TextAsset>($"{util_Dashboard.GetPath_Config()}/NodeStructure.json");

            // 序列化解析到类
            graphView.SearchStructures = JsonConvert.DeserializeObject<searchBox_NodesRoot>(graphView.SearchStructures_Json.text);

            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();

            // 搜索框内添加主标题分类名称
            entries.Add(new SearchTreeGroupEntry(new GUIContent(graphView.SearchStructures.root_title)) { level = 1 });

            // 节点一级分类
            List<searchBox_Nodes> list = graphView.SearchStructures.root_types;
            for (int s = 0; s < list.Count; s++)
            {
                Texture2D categoryicon = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{list[s].catergory_icon}.png");
                entries.Add(new SearchTreeGroupEntry(new GUIContent($"   {list[s].catergory_title}", categoryicon)) { level = 2 });
                for (int i = 0; i < list[s].catergory_nodes.Count; i++)
                {
                    // 节点二级分类
                    searchBox_Node item = list[s].catergory_nodes[i];
                    Texture2D icon = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{item.iconName}.png");
                    entries.Add(new SearchTreeEntry(new GUIContent($"   {item.name}", icon))
                    {
                        level = 3,
                        // 需要使用自定义的结构体装箱：行为树节点类型 & 可视化节点类型，传递给菜单项执行OnSelectEntry的时候的创建节点时必要的参数
                        userData = new NodeMenuParam(
                            item.name,
                            item.prefixNamespace,
                            item.prefixClass,
                            item.actionNodeType,
                            item.iconName,
                            item.visualNodeType)
                    });
                }
            }
            // 返回列表内容结构到搜索框中
            return entries;
        }

        /// <summary>
        /// 当点击搜索框中的项时的逻辑
        /// </summary>
        /// <param root_title="SearchTreeEntry"></param>
        /// <param root_title="context"></param>
        /// <returns></returns>
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            graphView.gv_NodeCreatedPosition = graphView.GetGraphMousePosition_With_ScreenMousePosition(context.screenMousePosition);
            NodeMenuParam types = (NodeMenuParam)SearchTreeEntry.userData;

            if (types.action_nodeType == "Stick")
            {
                NodeCreateArgs_Stick args = new NodeCreateArgs_Stick();
                args.stickName = "便签";
                args.stickContent = "双击此处更改内容";
                args.position = graphView.GetNodeCreatedMousePosition();
                args.size = Vector2.one * 100;

                graphView.CreateNode(args);
            }
            else if (types.action_nodeType == "Label")
            {
                NodeCreateArgs_Label args = new NodeCreateArgs_Label();
                args.content = "双击此处以更改标签的内容";
                args.position = graphView.GetNodeCreatedMousePosition();
                args.size = new Vector2(210, 60);
                args.opacity = 1;
                args.fontSize = 13;
                args.color = Color.white;
                args.italic = false;
                args.bold = false;
                args.font = util_XGraphEditorUtility.AssetLoad<Font>($"{util_Dashboard.GetPath_Fonts()}x_Regular.ttf");
                graphView.CreateNode(args);
            }
            else if (types.action_nodeType == "Decal")
            {
                NodeCreateArgs_Decal args = new NodeCreateArgs_Decal();
                args.position = graphView.GetNodeCreatedMousePosition();
                args.size = Vector2.one * 100;
                args.opacity = 1;
                args.hasTexture = false;
                args.decalTexture = null;
                args.scale = Vector3.one;
                args.color = Color.white;
                graphView.CreateNode(args);
            }
            else
            {
                NodeCreateArgs_Action args = new NodeCreateArgs_Action();
                args.visualName = types.visual_name;
                args.prefixNamespace = types.prefix_namespace;
                args.prefixClass = types.prefix_class;
                args.actionNodeType = types.action_nodeType;
                args.iconName = types.icon;
                args.nodeIcon = null;
                args.visualNodeType = types.visual_nodeType;
                args.hasAvatar = false;
                args.avatar = null;
                args.themeSolution = null;
                args.themeColor = Color.white;
                args.transparentNode = false;
                args.content = null;
                args.position = graphView.GetNodeCreatedMousePosition();
                args.size = Vector2.one * 100;

                xVariableType type;
                // 判断类型是否是有效的类型
                bool parsedVariableType = Enum.TryParse(types.visual_name, out type);
                if (parsedVariableType)
                {
                    args.variable = new Variable(types.visual_name, type);
                }

                graphView.CreateNode(args);
            }

            graphView.RecheckNodesIsExist();
            return true;
        }
    }
}