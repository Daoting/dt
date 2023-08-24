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
    public static class TraceLogs
    {
        const int _maxTrace = 30;

        /// <summary>
        /// Trace日志列表
        /// </summary>
        internal static readonly Nl<TraceLogItem> Data = new Nl<TraceLogItem>();

        /// <summary>
        /// 向Trace窗口输出信息
        /// </summary>
        /// <param name="p_logEvent"></param>
        internal static void AddItem(LogEvent p_logEvent)
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

        /// <summary>
        /// 清空输出
        /// </summary>
        internal static void Clear()
        {
            Data.Clear();
        }

        /// <summary>
        /// 除某项外清空
        /// </summary>
        /// <param name="p_item"></param>
        internal static void ClearExcept(TraceLogItem p_item)
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