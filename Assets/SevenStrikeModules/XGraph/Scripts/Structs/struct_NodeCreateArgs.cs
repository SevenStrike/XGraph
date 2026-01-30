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