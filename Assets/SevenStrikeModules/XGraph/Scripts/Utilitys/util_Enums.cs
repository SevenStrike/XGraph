namespace SevenStrikeModules.XGraph
{
    /// <summary>
    /// 视觉节点枚举类型
    /// </summary>
    public enum xg_GraphViewNode
    {
        None = 0,
        /// <summary>
        /// 单入空出
        /// </summary>
        S_In_N_Out = 1,
        /// <summary>
        /// 单入单出
        /// </summary>
        S_In_S_Out = 2,
        /// <summary>
        /// 单入多出
        /// </summary>
        S_In_M_Out = 3,
        /// <summary>
        /// 多入单出
        /// </summary>
        M_In_S_Out = 4,
        /// <summary>
        /// 多入空出
        /// </summary>
        M_In_N_Out = 5,
        /// <summary>
        /// 空入单出
        /// </summary>
        N_In_S_Out = 6,
        /// <summary>
        /// 空入多出
        /// </summary>
        N_In_m_Out = 7,
        /// <summary>
        /// 空入空出
        /// </summary>
        N_In_N_Out = 8
    }

    /// <summary>
    /// 行为节点枚举类型
    /// </summary>
    public enum xg_ActionTreeType
    {
        None = 0,
        Start = 1,
        End = 2,
        Composite = 3,
        Wait = 4,
        Debug = 5,
        StickNote = 6
    }
}