#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    static class HtmlLogHub
    {
        static readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _queue = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

        public static bool ExistListener
        {
            get { return !_queue.IsEmpty; }
        }

        public static void AddLog(string p_msg)
        {
            foreach (var waiter in _queue.Values)
            {
                waiter.SetResult(p_msg);
            }
            _queue.Clear();
        }

        public static Task<string> GetLog()
        {
            string id = Guid.NewGuid().ToString();
            var waiter = new TaskCompletionSource<string>();
            _queue[id] = waiter;

            try
            {
                waiter.Task.Wait(Bag.Context.RequestAborted);
                return Task.FromResult(waiter.Task.Result);
            }
            catch
            {
                _queue.TryRemove(id, out _);
                throw;
            }
        }
    }
}
