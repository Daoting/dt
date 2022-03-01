#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Trace的日志项
    /// </summary>
    public class TraceLogItem
    {
        public LogEvent Log { get; set; }
    }
}