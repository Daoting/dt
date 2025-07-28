#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-07-28 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 输出耗时信息、普通信息
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 输出信息，与Log不同，始终将信息输出到控制台和实时日志窗口
        /// </summary>
        /// <param name="p_msg">信息</param>
        public static void Trace(string p_msg)
        {
#if WASM || DESKTOP
            Console.WriteLine(p_msg);
#else
            System.Diagnostics.Debug.WriteLine(p_msg);
#endif

            var item = new TraceLogItem
            {
                Info = $"[{DateTime.Now:HH:mm:ss.fff} Trace]",
                Msg = p_msg
            };
            TraceLogs.AddItem(item);
        }

        /// <summary>
        /// 重置耗时计时器，从当前时间开始计时
        /// </summary>
        public static void ResetTick()
        {
            _lastTick = Environment.TickCount;
        }

        /// <summary>
        /// 输出耗时信息，计算从上次调用 ResetTick 或 TraceTick 的时间差
        /// </summary>
        /// <param name="prefixText">信息前缀</param>
        public static void TraceTick(string prefixText = null)
        {
            int now = Environment.TickCount;
            Trace($"{(string.IsNullOrEmpty(prefixText) ? "耗时" : prefixText)} {now - _lastTick}ms");
            _lastTick = now;
        }
        
        static int _lastTick;
    }
}