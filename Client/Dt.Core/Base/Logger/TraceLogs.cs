#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Events;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 日志的Trace输出内容
    /// </summary>
    internal static class TraceLogs
    {
        const int _maxTrace = 30;

        /// <summary>
        /// Trace日志列表
        /// </summary>
        public static readonly Nl<TraceLogItem> Data = new Nl<TraceLogItem>();

        /// <summary>
        /// 向Trace窗口输出信息
        /// </summary>
        /// <param name="p_logEvent"></param>
        public static void AddLogEvent(LogEvent p_logEvent)
        {
            AddItem(new TraceLogItem { Log = p_logEvent });
        }

        /// <summary>
        /// 向Trace窗口输出信息
        /// </summary>
        /// <param name="p_logItem"></param>
        public static void AddItem(TraceLogItem p_logItem)
        {
            Kit.RunAsync(() =>
            {
                using (Data.Defer())
                {
                    Data.Add(p_logItem);
                    if (Data.Count > _maxTrace)
                    {
                        // 确保输出行数不超过给定的最大行数
                        Data.RemoveAt(0);
                    }
                }
            });
        }
        
        /// <summary>
        /// 清空输出
        /// </summary>
        public static void Clear()
        {
            Data.Clear();
        }

        /// <summary>
        /// 除某项外清空
        /// </summary>
        /// <param name="p_item"></param>
        public static void ClearExcept(TraceLogItem p_item)
        {
            Kit.RunAsync(() =>
            {
                using (Data.Defer())
                {
                    Data.Clear();
                    Data.Add(p_item);
                }
            });
        }
    }
}