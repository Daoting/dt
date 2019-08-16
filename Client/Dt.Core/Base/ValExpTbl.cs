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
                    _tbl.NewRow(":UserID", "用户id");
                    _tbl.NewRow(":UserName", "用户姓名");
                    _tbl.NewRow(":Guid", "新Guid");
                    _tbl.NewRow(":Now", "当前时间");
                    _tbl.NewRow(":IP", "本机IP");
                    _tbl.NewRow(":LastYear", "上一年年份");
                    _tbl.NewRow(":Year", "当前年份");
                    _tbl.NewRow(":NextYear", "明年年份");
                    _tbl.NewRow(":LastQuarter", "上个季度");
                    _tbl.NewRow(":Quarter", "当前季度");
                    _tbl.NewRow(":NextQuarter", "下个季度");
                    _tbl.NewRow(":LastMonth", "上个月月份");
                    _tbl.NewRow(":Month", "当前月份");
                    _tbl.NewRow(":NextMonth", "下个月月份");
                    _tbl.NewRow(":Yesterday", "昨天");
                    _tbl.NewRow(":Today", "今天");
                    _tbl.NewRow(":Tomorrow", "明天");
                    _tbl.NewRow(":StartOfToday", "今天开始时间");
                    _tbl.NewRow(":EndOfToday", "今天结束时间");
                    _tbl.NewRow(":StartOfYesterday", "昨天开始时间");
                    _tbl.NewRow(":EndOfYesterday", "昨天结束时间");
                    _tbl.NewRow(":FirstDayOfMonth", "当月第一天");
                    _tbl.NewRow(":LastDayOfMonth", "当月最后一天");
                    _tbl.NewRow(":FirstDayOfLastMonth", "上个月第一天");
                    _tbl.NewRow(":LastDayOfLastMonth", "上个月最后一天");
                    _tbl.NewRow(":FirstDayOfYear", "本年第一天");
                    _tbl.NewRow(":FirstDayOfLastQuarter", "上季度第一天");
                    _tbl.NewRow(":LastDayOfLastQuarter", "上季度最后一天");

                    _tbl.NewRow(":sq_xxx", "序列值");
                    _tbl.NewRow(":", "取CtDrop第一行");
                }
                return _tbl;
            }
        }
    }
}
