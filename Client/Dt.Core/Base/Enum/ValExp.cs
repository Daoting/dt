#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 值表达式类型
    /// </summary>
    public enum ValExp
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        UserID,

        /// <summary>
        /// 用户姓名
        /// </summary>
        UserName,

        /// <summary>
        /// 新Guid
        /// </summary>
        Guid,

        /// <summary>
        /// 当前时间
        /// </summary>
        Now,

        /// <summary>
        /// 上一年年份
        /// </summary>
        LastYear,

        /// <summary>
        /// 当前年份
        /// </summary>
        Year,

        /// <summary>
        /// 明年年份
        /// </summary>
        NextYear,

        /// <summary>
        /// 上一个季度
        /// </summary>
        LastQuarter,

        /// <summary>
        /// 当前季度
        /// </summary>
        Quarter,

        /// <summary>
        /// 下个季度
        /// </summary>
        NextQuarter,

        /// <summary>
        /// 上个月月份
        /// </summary>
        LastMonth,

        /// <summary>
        /// 当前月份
        /// </summary>
        Month,

        /// <summary>
        /// 下个月月份
        /// </summary>
        NextMonth,

        /// <summary>
        /// 昨天
        /// </summary>
        Yesterday,

        /// <summary>
        /// 今天
        /// </summary>
        Today,

        /// <summary>
        /// 明天
        /// </summary>
        Tomorrow,

        /// <summary>
        /// 今天开始时间
        /// </summary>
        StartOfToday,

        /// <summary>
        /// 今天结束时间
        /// </summary>
        EndOfToday,

        /// <summary>
        /// 昨天开始时间
        /// </summary>
        StartOfYesterday,

        /// <summary>
        /// 昨天结束时间
        /// </summary>
        EndOfYesterday,

        /// <summary>
        /// 当月第一天
        /// </summary>
        FirstDayOfMonth,

        /// <summary>
        /// 当月最后一天
        /// </summary>
        LastDayOfMonth,

        /// <summary>
        /// 上个月第一天
        /// </summary>
        FirstDayOfLastMonth,

        /// <summary>
        /// 上个月最后一天
        /// </summary>
        LastDayOfLastMonth,

        /// <summary>
        /// 本年第一天
        /// </summary>
        FirstDayOfYear,

        /// <summary>
        /// 上季度第一天
        /// </summary>
        FirstDayOfLastQuarter,

        /// <summary>
        /// 上季度最后一天
        /// </summary>
        LastDayOfLastQuarter,
    }
}
