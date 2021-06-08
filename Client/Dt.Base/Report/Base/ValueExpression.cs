#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 内置表达式
    /// </summary>
    public static class ValueExpression
    {
        static Table _tbl = null;

        /// <summary>
        /// 内置表达式Drop数据源
        /// </summary>
        public static Table Data
        {
            get
            {
                if (_tbl == null)
                {
                    _tbl = new Table { { "name" }, { "desc" } };
                    _tbl.AddRow(new ExpRow(":UserID", "用户id"));
                    _tbl.AddRow(new ExpRow(":UserName", "用户姓名"));
                    _tbl.AddRow(new ExpRow(":Guid", "新Guid"));
                    _tbl.AddRow(new ExpRow(":Now", "当前时间"));
                    _tbl.AddRow(new ExpRow(":LastYear", "上一年年份"));
                    _tbl.AddRow(new ExpRow(":Year", "当前年份"));
                    _tbl.AddRow(new ExpRow(":NextYear", "明年年份"));
                    _tbl.AddRow(new ExpRow(":LastQuarter", "上个季度"));
                    _tbl.AddRow(new ExpRow(":Quarter", "当前季度"));
                    _tbl.AddRow(new ExpRow(":NextQuarter", "下个季度"));
                    _tbl.AddRow(new ExpRow(":LastMonth", "上个月月份"));
                    _tbl.AddRow(new ExpRow(":Month", "当前月份"));
                    _tbl.AddRow(new ExpRow(":NextMonth", "下个月月份"));
                    _tbl.AddRow(new ExpRow(":Yesterday", "昨天"));
                    _tbl.AddRow(new ExpRow(":Today", "今天"));
                    _tbl.AddRow(new ExpRow(":Tomorrow", "明天"));
                    _tbl.AddRow(new ExpRow(":StartOfToday", "今天开始时间"));
                    _tbl.AddRow(new ExpRow(":EndOfToday", "今天结束时间"));
                    _tbl.AddRow(new ExpRow(":StartOfYesterday", "昨天开始时间"));
                    _tbl.AddRow(new ExpRow(":EndOfYesterday", "昨天结束时间"));
                    _tbl.AddRow(new ExpRow(":FirstDayOfMonth", "当月第一天"));
                    _tbl.AddRow(new ExpRow(":LastDayOfMonth", "当月最后一天"));
                    _tbl.AddRow(new ExpRow(":FirstDayOfLastMonth", "上个月第一天"));
                    _tbl.AddRow(new ExpRow(":LastDayOfLastMonth", "上个月最后一天"));
                    _tbl.AddRow(new ExpRow(":FirstDayOfYear", "本年第一天"));
                    _tbl.AddRow(new ExpRow(":FirstDayOfLastQuarter", "上季度第一天"));
                    _tbl.AddRow(new ExpRow(":LastDayOfLastQuarter", "上季度最后一天"));
                }
                return _tbl;
            }
        }

        /// <summary>
        /// 获取内置表达式的值
        /// </summary>
        /// <param name="p_expression"></param>
        /// <returns></returns>
        public static string GetValue(string p_expression)
        {
            ValExp exp;
            if (!Enum.TryParse(p_expression, true, out exp))
            {
                Kit.Warn($"无法识别内置表达式【{p_expression}】!");
                return "";
            }

            switch (exp)
            {
                case ValExp.UserID:
                    return Kit.UserID.ToString();
                case ValExp.UserName:
                    return Kit.UserName;
                case ValExp.Guid:
                    return Guid.NewGuid().ToString("N");
                case ValExp.Now:
                    return Kit.Now.ToString("yyyy-MM-dd HH:mm:ss");
                case ValExp.LastYear:
                    return Kit.Now.AddYears(-1).Year.ToString();
                case ValExp.Year:
                    return Kit.Now.Year.ToString();
                case ValExp.NextYear:
                    return Kit.Now.AddYears(1).Year.ToString();
                case ValExp.LastQuarter:
                    int thisQuarter = (Kit.Now.Month - 1) / 3 + 1;
                    return thisQuarter == 1 ? "4" : (thisQuarter - 1).ToString();
                case ValExp.Quarter:
                    return ((Kit.Now.Month - 1) / 3 + 1).ToString();
                case ValExp.NextQuarter:
                    int quarter = (Kit.Now.Month - 1) / 3 + 1;
                    return quarter == 4 ? "1" : (quarter + 1).ToString();
                case ValExp.LastMonth:
                    return Kit.Now.AddMonths(-1).Month.ToString();
                case ValExp.Month:
                    return Kit.Now.Month.ToString();
                case ValExp.NextMonth:
                    return Kit.Now.AddMonths(1).Month.ToString();
                case ValExp.Yesterday:
                    return Kit.Now.AddDays(-1).Day.ToString();
                case ValExp.Today:
                    return Kit.Now.Day.ToString();
                case ValExp.Tomorrow:
                    return Kit.Now.AddDays(1).Day.ToString();
                case ValExp.StartOfToday:
                    return Kit.Now.ToString("yyyy-MM-dd 00:00:00");
                case ValExp.EndOfToday:
                    return Kit.Now.ToString("yyyy-MM-dd 23:59:59");
                case ValExp.StartOfYesterday:
                    return Kit.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                case ValExp.EndOfYesterday:
                    return Kit.Now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59");
                case ValExp.FirstDayOfMonth:
                    DateTime dt = Kit.Now;
                    return new DateTime(dt.Year, dt.Month, 1).ToString("yyyy-MM-dd 00:00:00");
                case ValExp.LastDayOfMonth:
                    dt = Kit.Now;
                    return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month)).ToString("yyyy-MM-dd 23:59:59");
                case ValExp.FirstDayOfLastMonth:
                    dt = Kit.Now;
                    return new DateTime(dt.Year, dt.AddMonths(-1).Month, 1).ToString("yyyy-MM-dd 00:00:00");
                case ValExp.LastDayOfLastMonth:
                    dt = Kit.Now;
                    return new DateTime(dt.Year, dt.AddMonths(-1).Month, DateTime.DaysInMonth(dt.Year, dt.AddMonths(-1).Month)).ToString("yyyy-MM-dd 23:59:59");
                case ValExp.FirstDayOfYear:
                    dt = Kit.Now;
                    return new DateTime(dt.Year, 1, 1).ToString("yyyy-MM-dd 00:00:00");
                case ValExp.FirstDayOfLastQuarter:
                    dt = Kit.Now;
                    return new DateTime(dt.Year, GetFirstMonthInQuarter(dt.AddMonths(-3)), 1).ToString("yyyy-MM-dd 00:00:00");
                case ValExp.LastDayOfLastQuarter:
                    dt = Kit.Now;
                    return new DateTime(dt.Year, GetFirstMonthInQuarter(dt.AddMonths(-3)) + 2, DateTime.DaysInMonth(dt.Year, GetFirstMonthInQuarter(dt) + 2)).ToString("yyyy-MM-dd 23:59:59");
            }
            return "";
        }

        /// <summary>
        /// 根据指定时间获取时间对应季度的第一个月
        /// </summary>
        /// <param name="p_time">指定时间</param>
        /// <returns>对应季度的第一个月</returns>
        static int GetFirstMonthInQuarter(DateTime p_time)
        {
            int thisQuarter = (p_time.Month - 1) / 3;
            return thisQuarter * 3 + 1;
        }

        class ExpRow
        {
            public ExpRow(string p_name, string p_desc)
            {
                Name = p_name;
                Desc = p_desc;
            }

            public string Name { get; }

            public string Desc { get; }
        }

        /// <summary>
        /// 值表达式类型
        /// </summary>
        enum ValExp
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
}
