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
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
#endif

    [Serializable]
    /// <summary>
    /// 贴纸数据
    /// </summary>
    public class xDecalData
    {
        /// <summary>
        /// 贴纸识别ID码
        /// </summary>
        [SerializeField] public string guid;
        /// <summary>
        /// 节点位置
        /// </summary>
        [SerializeField] public Vector2 position;
        /// <summary>
        /// 节点尺寸
        /// </summary>
        [SerializeField] public Vector2 size;
        /// <summary>
        /// 贴图着色颜色
        /// </summary>
        [SerializeField] public Color color = Color.white;
        /// <summary>
        /// 节点贴图透明度
        /// </summary>
        [SerializeField] public float opacity = 1;
        [SerializeField] public bool texture_exist;
        [SerializeField] public Texture2D texture_decal;
        /// <summary>
        /// 贴图缩放
        /// </summary>
        [SerializeField] public Vector3 scale = Vector3.one;

        /// <summary>
        /// 贴纸构造器
        /// </summary>
        public xDecalData() { }
        /// <summary>
        /// 贴纸构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        public xDecalData(string guid, Vector2 pos, Vector2 size, Vector3 scale, Color color, float opacity, bool hastex, Texture2D tex)
        {
            this.guid = guid;
            this.position = pos;
            this.scale = scale;
            this.size = size;
            this.color = color;
            this.opacity = opacity;
            this.texture_exist = hastex;
            this.texture_decal = tex;
        }
        /// <summary>
        /// 贴纸克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public xDecalData Clone(bool guid_create)
        {
            var clone = new xDecalData();
#if UNITY_EDITOR
            clone.guid = guid_create ? GUID.Generate().ToString() : guid;
#endif
            clone.position = position;
            clone.size = size;
            clone.color = color;
            clone.texture_exist = texture_exist;
            clone.texture_decal = texture_decal;
            clone.scale = scale;
            clone.opacity = opacity;
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 便签数据
    /// </summary>
    public class xStickData
    {
        /// <summary>
        /// 便签标题
        /// </summary>
        [SerializeField] public string name;
        /// <summary>
        /// 便签内容
        /// </summary>
        [SerializeField] public string content;
        /// <summary>
        /// 便签识别ID码
        /// </summary>
        [SerializeField] public string guid;
        /// <summary>
        /// 节点位置
        /// </summary>
        [SerializeField] public Vector2 position;
        /// <summary>
        /// 节点尺寸
        /// </summary>
        [SerializeField] public Vector2 size;
        /// <summary>
        /// 便签构造器
        /// </summary>
        public xStickData() { }
        /// <summary>
        /// 便签构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        public xStickData(string name, string content, string guid, Vector2 pos, Vector2 size)
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
        public xStickData Clone(bool guid_create)
        {
            var clone = new xStickData();
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

    [Serializable]
    /// <summary>
    /// 标签数据
    /// </summary>
    public class xLabelData
    {
        /// <summary>
        /// 标签识别ID码
        /// </summary>
        [SerializeField] public string guid;
        /// <summary>
        /// 标签内容
        /// </summary>
        [SerializeField] public string content;
        /// <summary>
        /// 节点位置
        /// </summary>
        [SerializeField] public Vector2 position;
        /// <summary>
        /// 节点尺寸
        /// </summary>
        [SerializeField] public Vector2 size;
        /// <summary>
        /// 标签颜色
        /// </summary>
        [SerializeField] public Color color = Color.white;
        /// <summary>
        /// 字体
        /// </summary>
        [SerializeField] public Font font;
        /// <summary>
        /// 标签透明度
        /// </summary>
        [SerializeField] public float opacity;
        /// <summary>
        /// 文字内容尺寸
        /// </summary>
        [SerializeField] public int fontSize;
        /// <summary>
        /// 文字是否粗体
        /// </summary>
        [SerializeField] public bool bold;
        /// <summary>
        /// 文字是否斜体
        /// </summary>
        [SerializeField] public bool italic;

        /// <summary>
        /// 标签构造器
        /// </summary>
        public xLabelData() { }
        /// <summary>
        /// 标签构造器
        /// </summary>
        /// <param name="content"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="opacity"></param>
        public xLabelData(string content, string guid, Vector2 pos, Vector2 size, Color color, Font font, float opacity, int fontSize, bool bold, bool italic)
        {
            this.guid = guid;
            this.content = content;
            this.position = pos;
            this.size = size;
            this.color = color;
            this.font = font;
            this.opacity = opacity;
            this.fontSize = fontSize;
            this.bold = bold;
            this.italic = italic;
        }
        /// <summary>
        /// 标签克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public xLabelData Clone(bool guid_create)
        {
            var clone = new xLabelData();
            clone.content = content;
#if UNITY_EDITOR
            clone.guid = guid_create ? GUID.Generate().ToString() : guid;
#endif
            clone.position = position;
            clone.size = size;
            clone.color = color;
            clone.font = font;
            clone.opacity = opacity;
            clone.fontSize = fontSize;
            clone.bold = bold;
            clone.italic = italic;
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 编组数据
    /// </summary>
    public class xGroupData
    {
#if UNITY_EDITOR
        /// <summary>
        /// 编组的标题
        /// </summary>
        [SerializeField] public string name;
        /// <summary>
        /// 编组的识别ID码
        /// </summary>
        [SerializeField] public string guid;
        /// <summary>
        /// 编组的位置
        /// </summary>
        [SerializeField] public Vector2 pos;
        /// <summary>
        /// 编组的颜色识别
        /// </summary>
        [SerializeField] public string solution = "M 默认";
        /// <summary>
        /// 编组组件
        /// </summary>
        [SerializeField] public Group group;
        /// <summary>
        /// 编组内容容器组件
        /// </summary>
        [SerializeField] public VisualElement groupcontainer;
        /// <summary>
        /// 编组内的所有节点的识别ID码
        /// </summary>
        [SerializeField] public List<string> guids = new List<string>();
        /// <summary>
        /// 编组中是否包含节点
        /// </summary>
        [SerializeField] public bool hasAvatarNodes;

        /// <summary>
        /// 编组克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public xGroupData Clone(bool guid_create)
        {
            var clone = new xGroupData();
            clone.name = name;
            clone.guid = guid_create ? GUID.Generate().ToString() : guid;
            clone.pos = pos;
            clone.guids = new List<string>();
            foreach (string guid in guids)
            {
                clone.guids.Add(guid);
            }
            clone.solution = solution;
            clone.group = null;
            clone.groupcontainer = null;
            clone.hasAvatarNodes = hasAvatarNodes;
            return clone;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public xGroupData() { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="guids"></param>
        /// <param name="group"></param>
        public xGroupData(string name, string guid, Vector2 pos, List<string> guids, string solution, Group group, VisualElement groupcontainer)
        {
            this.name = name;
            this.guid = guid;
            this.pos = pos;
            this.guids = guids;
            this.solution = solution;
            this.group = group;
            this.groupcontainer = groupcontainer;
        }
#endif
    }

    [Serializable]
    /// <summary>
    /// 变量数据
    /// </summary>
    public class xVariableData
    {
        /// <summary>
        /// 变量节点显示名称
        /// </summary>
        [SerializeField] public string name;
        /// <summary>
        /// 变量节点显示解释
        /// </summary>
        [SerializeField] public string description;
        /// <summary>
        /// 变量节点识别ID码
        /// </summary>
        [SerializeField] public string guid_n;
        /// <summary>
        /// 变量原始识别ID码
        /// </summary>
        [SerializeField] public string guid_v;
        /// <summary>
        /// 变量节点显示类型
        /// </summary>
        [SerializeField] public xVariableType type;
        /// <summary>
        /// 变量类
        /// </summary>
        [SerializeReference] public Variable variable;
        /// <summary>
        /// 透明背景节点模式
        /// </summary>
        [SerializeField] public bool transparent = false;
        /// <summary>
        /// 节点位置
        /// </summary>
        [SerializeField] public Vector2 position;
        /// <summary>
        /// 节点尺寸
        /// </summary>
        [SerializeField] public Vector2 size;

        /// <summary>
        /// 变量构造器
        /// </summary>
        public xVariableData() { }
        /// <summary>
        /// 变量构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="type"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="varguid"></param>
        public xVariableData(string name, string description, xVariableType type, string guid, Vector2 pos, Vector2 size, string varguid, Variable variable, bool transparentNode)
        {
            this.name = name;
            this.description = description;
            this.type = type;
            this.guid_n = guid;
            this.position = pos;
            this.size = size;
            this.guid_v = varguid;
            this.variable = variable;
        }
        /// <summary>
        /// 变量克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public xVariableData Clone(bool guid_create)
        {
            var clone = new xVariableData();
            clone.name = this.name;
            clone.description = this.description;
            clone.type = this.type;
#if UNITY_EDITOR
            clone.guid_n = guid_create ? GUID.Generate().ToString() : guid_n;
#endif
            clone.position = position;
            clone.guid_v = guid_v;
            clone.size = size;
            clone.transparent = transparent;
            clone.variable = variable.Clone(false);
            return clone;
        }
    }

    [Serializable]
    /// <summary>
    /// 变量数据
    /// </summary>
    public class xPropertyData
    {
        /// <summary>
        /// 变量节点显示名称
        /// </summary>
        [SerializeField] public string name;
        /// <summary>
        /// 变量节点显示解释
        /// </summary>
        [SerializeField] public string description;
        /// <summary>
        /// 变量节点识别ID码
        /// </summary>
        [SerializeField] public string guid;
        /// <summary>
        /// 变量原始识别ID码
        /// </summary>
        [SerializeField] public string varguid;
        /// <summary>
        /// 变量节点显示类型
        /// </summary>
        [SerializeField] public xVariableType type;
        /// <summary>
        /// 变量类
        /// </summary>
        [SerializeReference] public Variable variable;
        /// <summary>
        /// 透明背景节点模式
        /// </summary>
        [SerializeField] public bool transparent = false;
        /// <summary>
        /// 节点位置
        /// </summary>
        [SerializeField] public Vector2 position;
        /// <summary>
        /// 节点尺寸
        /// </summary>
        [SerializeField] public Vector2 size;

        /// <summary>
        /// 变量构造器
        /// </summary>
        public xPropertyData() { }
        /// <summary>
        /// 变量构造器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="type"></param>
        /// <param name="guid"></param>
        /// <param name="pos"></param>
        /// <param name="varguid"></param>
        public xPropertyData(string name, string description, xVariableType type, string guid, Vector2 pos, Vector2 size, string varguid, Variable variable, bool transparentNode)
        {
            this.name = name;
            this.description = description;
            this.type = type;
            this.guid = guid;
            this.position = pos;
            this.size = size;
            this.varguid = varguid;
            this.variable = variable;
        }
        /// <summary>
        /// 变量克隆
        /// </summary>
        /// <param name="guid_create"></param>
        /// <returns></returns>
        public xPropertyData Clone(bool guid_create)
        {
            var clone = new xPropertyData();
            clone.name = this.name;
            clone.description = this.description;
            clone.type = this.type;
#if UNITY_EDITOR
            clone.guid = guid_create ? GUID.Generate().ToString() : guid;
#endif
            clone.position = position;
            clone.varguid = varguid;
            clone.size = size;
            clone.transparent = transparent;
            clone.variable = variable;
            return clone;
        }
    }
}