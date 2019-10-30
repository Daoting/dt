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
    /// 内置表达式Drop数据源
    /// </summary>
    public static class ValExpTbl
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
                    _tbl = new Table { { "id" }, { "name" } };
                    _tbl.AddRow(new ExpRow(":UserID", "用户id"));
                    _tbl.AddRow(new ExpRow(":UserName", "用户姓名"));
                    _tbl.AddRow(new ExpRow(":Guid", "新Guid"));
                    _tbl.AddRow(new ExpRow(":Now", "当前时间"));
                    _tbl.AddRow(new ExpRow(":IP", "本机IP"));
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

                    _tbl.AddRow(new ExpRow(":sq_xxx", "序列值"));
                    _tbl.AddRow(new ExpRow(":", "取CtDrop第一行"));
                }
                return _tbl;
            }
        }

        class ExpRow
        {
            public ExpRow(string p_id, string p_name)
            {
                ID = p_id;
                Name = p_name;
            }

            public string ID { get; }

            public string Name { get; }
        }
    }
}
