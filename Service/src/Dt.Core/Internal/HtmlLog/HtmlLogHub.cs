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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    static class HtmlLogHub
    {
        const int _maxMsgCount = 50;
        static readonly ConcurrentDictionary<string, HtmlLogClient> _clientQueue = new ConcurrentDictionary<string, HtmlLogClient>();
        static int _topIndex = 0;
        static readonly ConcurrentQueue<string> _msgQueue = new ConcurrentQueue<string>();

        public static void AddLog(string p_msg)
        {
            _msgQueue.Enqueue(p_msg);
            if (_msgQueue.Count >= _maxMsgCount)
            {
                // 队列超长，移除最前
                _msgQueue.TryDequeue(out _);
                _topIndex++;
            }

            if (_clientQueue.Count > 0)
            {
                // 队列可能变化
                var ls = _clientQueue.Values.ToList();
                foreach (var client in ls)
                {
                    client.Update();
                }
            }
        }

        /// <summary>
        /// 实时获取日志
        /// </summary>
        /// <param name="p_startIndex">起始索引</param>
        /// <returns></returns>
        public static Task<string> GetLog(int p_startIndex)
        {
            HtmlLogClient client = new HtmlLogClient(p_startIndex);
            if (!client.Update())
            {
                string id = Guid.NewGuid().ToString();
                _clientQueue[id] = client;

                try
                {
                    client.Wait(Kit.HttpContext.RequestAborted);
                }
                catch { }
                finally
                {
                    _clientQueue.TryRemove(id, out _);
                }
            }
            return Task.FromResult(client.Result);
        }

        class HtmlLogClient
        {
            TaskCompletionSource<bool> _waiter;

            public HtmlLogClient(int p_startIndex)
            {
                StartIndex = p_startIndex;
            }

            public int StartIndex { get; }

            public string Result { get; private set; }

            public void Wait(CancellationToken p_cancel)
            {
                _waiter = new TaskCompletionSource<bool>();
                _waiter.Task.Wait(p_cancel);
            }

            public bool Update()
            {
                int start = Math.Max(0, StartIndex - _topIndex);
                if (start >= _msgQueue.Count)
                    return false;

                StringBuilder sb = new StringBuilder();
                // 本次读取的末索引
                sb.Append(_topIndex + _msgQueue.Count);
                sb.Append("+");
                foreach (var msg in _msgQueue.Skip(start))
                {
                    sb.Append(msg);
                }

                Result = sb.ToString();
                _waiter?.TrySetResult(true);
                return true;
            }
        }
    }
}
