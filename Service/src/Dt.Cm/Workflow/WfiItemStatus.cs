#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-24 创建
**************************************************************************/
#endregion

#region 命名空间

#endregion

namespace Dt.Cm.Workflow
{
    /// <summary>
    /// 工作项状态
    /// </summary>
    public enum WfiItemStatus
    {
        /// <summary>
        /// 活动
        /// </summary>
        Active,

        /// <summary>
        /// 结束
        /// </summary>
        Finish,

        /// <summary>
        /// 终止
        /// </summary>
        Terminate,

        /// <summary>
        /// 同步活动
        /// </summary>
        Sync
    }

    /// <summary>
    /// 指派方式
    /// </summary>
    public enum WfiItemAssignKind
    {
        /// <summary>
        /// 普通指派
        /// </summary>
        Normal,

        /// <summary>
        /// 起始指派
        /// </summary>
        Start,

        /// <summary>
        /// 回退
        /// </summary>
        Back,

        /// <summary>
        /// 跳转
        /// </summary>
        Jump,

        /// <summary>
        /// 追回
        /// </summary>
        Rollback,

        /// <summary>
        /// 回退指派
        /// </summary>
        FallBack
    }
}
