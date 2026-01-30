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
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    /// <summary>
    /// 绑定器 - 变量
    /// </summary>
    public class class_ActionBaseArgs
    {
        #region 节点参数
        /// <summary>
        /// 行为节点 - 内容表述
        /// </summary>
        [SerializeField] public string content;
        /// <summary>
        /// 行为节点 - guid_self
        /// </summary>
        [SerializeField] public string guid;
        /// <summary>
        /// 行为节点 - guid_self
        /// </summary>
        [SerializeField] public string namespaces;
        /// <summary>
        /// 行为节点 - guid_self
        /// </summary>
        [SerializeField] public string classes;
        /// <summary>
        /// 行为节点 - 类型
        /// </summary>
        [SerializeField] public string actionNodeType;
        /// <summary>
        /// 行为节点 - 图标
        /// </summary>
        [SerializeField] public string icon;
        /// <summary>
        /// 行为节点 - 类型
        /// </summary>
        [SerializeField] public string visualNodeType = "None";
        /// <summary>
        /// 行为节点 - 在GraphView里的位置记录
        /// </summary>
        [SerializeField] public Vector2 nodeGraphPosition;
        /// <summary>
        /// 行为节点 - 在GraphView里的尺寸记录
        /// </summary>
        [SerializeField] public Vector2 nodeGraphSize;
        /// <summary>
        /// 行为节点 - 在GraphView里的颜色标记方案名称
        /// </summary>
        [SerializeField] public string themeSolution = "M 默认";
        /// <summary>
        /// 行为节点 - 在GraphView里的颜色标记
        /// </summary>
        [SerializeField] public Color themeColor = Color.clear;
        /// <summary>
        /// 并发执行开关
        /// </summary>
        [SerializeField] public bool isConcurrentExecution = false;
        /// <summary>
        /// 设定头像状态
        /// </summary>
        [SerializeField] public bool HasAvatar = false;
        /// <summary>
        /// 透明背景节点模式
        /// </summary>
        [SerializeField] public bool TransparentNode = false;
        /// <summary>
        /// 头像图像
        /// </summary>
        [SerializeField] public Texture2D Avatar;
        /// <summary>
        /// 标题图标
        /// </summary>
        [SerializeField] public Texture2D NodeIcon;
        /// <summary>
        /// 行为根节点
        /// </summary>
        [SerializeField] public xAction_Asset RootAsset;
        /// <summary>
        /// 起始节点
        /// </summary>
        [SerializeField] public bool isStartNode;
        /// <summary>
        /// 父节点
        /// </summary>
        [SerializeField] public string ParentNodeGuid;
        #endregion

        #region 节点数据接驳器
        /// <summary>
        /// 变量Guids接驳列表
        /// </summary>
        [SerializeField] public List<Binder_Varialble> VariableDatas = new List<Binder_Varialble>();
        /// <summary>
        /// 内部变量Guids接驳列表
        /// </summary>
        [SerializeField] public List<Binder_Varialble> InternalVariableDatas = new List<Binder_Varialble>();
        /// <summary>
        /// 绑定的属性列表
        /// </summary>
        [SerializeField] public List<Binder_Property> binded_propertys = new List<Binder_Property>();
        #endregion

        /// <summary>
        /// 克隆脱离引用
        /// </summary>
        /// <returns></returns>
        public class_ActionBaseArgs Clone()
        {
            // 创建新实例
            class_ActionBaseArgs clone = new class_ActionBaseArgs();
            //// 复制基础字段
            //clone.identifyName = this.identifyName;
            clone.content = this.content;
            clone.guid = guid;
            clone.namespaces = this.namespaces;
            clone.classes = this.classes;
            clone.actionNodeType = this.actionNodeType;
            clone.icon = this.icon;
            clone.visualNodeType = this.visualNodeType;
            clone.nodeGraphPosition = this.nodeGraphPosition;
            clone.nodeGraphSize = this.nodeGraphSize;
            clone.themeSolution = this.themeSolution;
            clone.themeColor = this.themeColor;
            clone.isConcurrentExecution = this.isConcurrentExecution;
            clone.HasAvatar = this.HasAvatar;
            clone.TransparentNode = this.TransparentNode;
            clone.Avatar = this.Avatar;
            clone.NodeIcon = this.NodeIcon;
            clone.RootAsset = this.RootAsset;
            clone.isStartNode = this.isStartNode;

            // 深复制列表
            clone.VariableDatas = new List<Binder_Varialble>();
            foreach (var item in this.VariableDatas)
            {
                clone.VariableDatas.Add(new Binder_Varialble(
                    item.VariableNodeGuid,
                    item.TargetPortName,
                    item.variable?.Clone(false)
                ));
            }

            clone.InternalVariableDatas = new List<Binder_Varialble>();
            foreach (var item in this.InternalVariableDatas)
            {
                clone.InternalVariableDatas.Add(new Binder_Varialble(
                    item.VariableNodeGuid,
                    item.TargetPortName,
                    item.variable?.Clone(false)
                ));
            }

            clone.binded_propertys = new List<Binder_Property>();
            foreach (var item in this.binded_propertys)
            {
                Binder_Property prop = new Binder_Property();
                prop.Property_GUID = item.Property_GUID;
                prop.Property_PortType = item.Property_PortType;
                prop.Property_PortName = item.Property_PortName;
                prop.Property_NodeName = item.Property_NodeName;
                prop.Action_PortType = item.Action_PortType;
                prop.Action_PortName = item.Action_PortName;

                clone.binded_propertys.Add(prop);
            }

            return clone;
        }
    }
}