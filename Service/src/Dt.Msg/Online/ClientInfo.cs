#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Dt.Core.Rpc;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 客户端会话
    /// </summary>
    public class ClientInfo
    {
        readonly BlockingCollection<string> _queue;
        readonly LobContext _c;
        readonly ResponseWriter _writer;
        bool _closed;

        public ClientInfo(LobContext p_context, ClientSystem p_system, ResponseWriter p_writer)
        {
            _c = p_context;
            System = p_system;
            _writer = p_writer;

            _queue = new BlockingCollection<string>();
            StartTime = DateTime.Now;
        }

        /// <summary>
        /// 客户端系统
        /// </summary>
        public ClientSystem System { get; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// 增加待推送信息
        /// </summary>
        /// <param name="p_msg"></param>
        public void AddMsg(string p_msg)
        {
            _queue.TryAdd(p_msg);
        }

        /// <summary>
        /// 推送信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SendMsg()
        {
            try
            {
                // 客户端取消请求时触发 OperationCanceledException 异常，推送结束
                var msg = _queue.Take(_c.Context.RequestAborted);
                Log.Debug($"向{_c.UserID}推送：{msg}");
                return _writer.Write(msg);
            }
            catch { }

            // 未关闭时需要删除会话
            if (!_closed)
            {
                Online.All.TryRemove(_c.UserID, out var ci);
                _closed = true;
            }

            _queue.Dispose();
            Log.Debug($"用户{_c.UserID}退出推送");
            return Task.FromResult(false);
        }

        /// <summary>
        /// 关闭推送，调用前已删除会话！
        /// </summary>
        public void Close()
        {
            _closed = true;
            // 通知客户端退出
            _queue.TryAdd("[\"SysPushApi.StopPush\"]");
            // 触发_queue.Take()异常，会话结束
            _queue.CompleteAdding();
        }
    }
}
