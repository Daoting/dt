#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.HtmlLog;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 
    /// </summary>
    static class HtmlLogHub
    {
        static readonly ConcurrentBag<TaskCompletionSource<string>> _queue = new ConcurrentBag<TaskCompletionSource<string>>();

        public static bool ExistListener
        {
            get { return _queue.Count > 0; }
        }

        public static void AddLog(string p_msg)
        {
            lock (_queue)
            {
                TaskCompletionSource<string> waiter;
                while (_queue.TryTake(out waiter))
                {
                    waiter.SetResult(p_msg);
                }
            }
        }

        public static Task<string> GetLog()
        {
            lock (_queue)
            {
                var waiter = new TaskCompletionSource<string>();
                _queue.Add(waiter);
                return waiter.Task;
            }
        }
    }
}
