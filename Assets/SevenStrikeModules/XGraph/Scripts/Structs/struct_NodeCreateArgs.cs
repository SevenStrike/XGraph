namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEngine;

    [Serializable]
    /// <summary>
    /// 创建节点信息结构体 - 行为
    /// </summary>
    public struct NodeCreateArgs_Action
    {
        public string visualName;
        public string prefixNamespace;
        public string prefixClass;
        public string actionNodeType;
        public string iconName;
        public Texture2D nodeIcon;
        public string visualNodeType;
        public bool hasAvatar;
        public Texture2D avatar;
        public string themeSolution;
        public Color themeColor;
        public bool transparentNode;
        public bool isConcurrentExecution;
        public string content;
        public Vector2 position;
        public Vector2 size;
        public Variable variable;
    }

    [Serializable]
    /// <summary>
    /// 创建节点信息结构体 - 便签
    /// </summary>
    public struct NodeCreateArgs_Stick
    {
        public string stickName;
        public string stickContent;
        public Vector2 position;
        public Vector2 size;
    }

    [Serializable]
    /// <summary>
    /// 创建节点信息结构体 - 标签
    /// </summary>
    public struct NodeCreateArgs_Label
    {
        public string content;
        public Vector2 position;
        public Vector2 size;
        public Color color;
        public Font font;
        public float opacity;
        public int fontSize;
        public bool bold;
        public bool italic;
    }

    [Serializable]
    /// <summary>
    /// 创建节点信息结构体 - 贴图
    /// </summary>
    public struct NodeCreateArgs_Decal
    {
        public Vector2 position;
        public Vector2 size;
        public Color color;
        public float opacity;
        public bool hasTexture;
        public Texture2D decalTexture;
        public Vector3 scale;
    }

    [Serializable]
    /// <summary>
    /// 创建节点信息结构体 - 变量
    /// </summary>
    public struct NodeCreateArgs_Variable
    {
        public string name;
        public string description;
        public xVariableType type;
        public Vector2 position;
        public Vector2 size;
        public string varguid;
        public bool transparentNode;
        public Variable variable;
    }
}