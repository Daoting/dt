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
    /// 监视输出内容
    /// </summary>
    internal static class TraceLogs
    {
        const int _maxTrace = 30;

        public static readonly Nl<TraceLogItem> Data = new Nl<TraceLogItem>();

        /// <summary>
        /// 向监视窗口输出信息
        /// </summary>
        /// <param name="p_logEvent"></param>
        public static void Add(LogEvent p_logEvent)
        {
            var item = new TraceLogItem { Log = p_logEvent };
            Kit.RunAsync(() =>
            {
                using (Data.Defer())
                {
                    Data.Add(item);
                    if (Data.Count > _maxTrace)
                    {
                        // 确保输出行数不超过给定的最大行数
                        Data.RemoveAt(0);
                    }
                }
            });
        }
    }
}