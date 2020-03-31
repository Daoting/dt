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

        public ClientInfo(LobContext p_context, ClientSystem p_system, ResponseWriter p_writer)
        {
            _c = p_context;
            System = p_system;
            _writer = p_writer;
            Serilog.Log.Debug(_c.Context.TraceIdentifier);

            _queue = new BlockingCollection<string>();
            StartTime = DateTime.Now;
        }

        /// <summary>
        /// 会话上下文
        /// </summary>
        public LobContext Context
        {
            get { return _c; }
        }

        /// <summary>
        /// 当前用户标识
        /// </summary>
        public long UserID
        {
            get { return _c.UserID; }
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
        /// 发送当前用户的离线信息
        /// </summary>
        /// <returns></returns>
        public async Task SendOfflineMsg()
        {
            // 所有离线信息
            string key = MsgKit.MsgQueueKey + _c.UserID.ToString();
            var db = Redis.Db;
            var ls = await db.ListRangeAsync(key);
            if (ls != null && ls.Length > 0)
            {
                try
                {
                    foreach (var mi in ls)
                    {
                        await _writer.Write((string)mi);
                    }
                    // 删除避免重复推送
                    await db.KeyDeleteAsync(key);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, $"向{_c.UserID}发送离线信息异常");
                }
            }
        }

        /// <summary>
        /// 增加待推送信息
        /// </summary>
        /// <param name="p_msg"></param>
        public bool AddMsg(string p_msg)
        {
            return _queue.TryAdd(p_msg);
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
                Log.Debug($"推送：{_c.UserID}  {msg}");
                return _writer.Write(msg);
            }
            catch { }

            _queue.Dispose();
            return Task.FromResult(false);
        }

        /// <summary>
        /// 通知客户端退出
        /// </summary>
        public void StopPush()
        {
            _queue.TryAdd("[\"SysPushApi.StopPush\"]");
        }

        /// <summary>
        /// 关闭推送
        /// </summary>
        /// <returns></returns>
        public void Close()
        {
            _c.Context.Abort();
        }
    }
}
