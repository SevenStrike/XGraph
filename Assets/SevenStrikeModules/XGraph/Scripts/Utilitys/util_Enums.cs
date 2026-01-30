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
    /// <summary>
    /// 黑板变量值类型
    /// </summary>
    public enum xVariableType
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
        /// 值 - 颜色
        /// </summary>
        Color = 7
    }
    /// <summary>
    /// PortStyle 类型
    /// </summary>
    public enum xPortType
    {
        /// <summary>
        /// 输入
        /// </summary>
        In = 0,
        /// <summary>
        /// 输出
        /// </summary>
        Out = 1,
    }
    /// <summary>
    /// GraphView 容器类型
    /// </summary>
    public enum xNodeContainerType
    {
        /// <summary>
        /// 主容器
        /// </summary>
        MainContainer = 0,
        /// <summary>
        /// 标题
        /// </summary>
        TitleContainer = 1,
        /// <summary>
        /// 标题按钮
        /// </summary>
        TitleButtonContainer = 2,
        /// <summary>
        /// 顶部容器
        /// </summary>
        TopContainer = 3,
        /// <summary>
        /// 输入端口容器
        /// </summary>
        InputContainer = 4,
        /// <summary>
        /// 输出端口容器
        /// </summary>
        OutputContainer = 5,
        /// <summary>
        /// 扩展容器
        /// </summary>
        ExtensionContainer = 6,
    }
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum xMessageType
    {
        信息 = 0,
        警告 = 1,
        错误 = 2
    }
    /// <summary>
    /// 通知类型
    /// </summary>
    public enum xNotifyType
    {
        信息 = 0,
        警告 = 1,
        错误 = 2
    }
}