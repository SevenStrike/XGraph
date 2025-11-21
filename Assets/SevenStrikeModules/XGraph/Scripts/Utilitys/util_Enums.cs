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
}