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
using Microsoft.AspNetCore.Http;
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
        readonly ResponseWriter _writer;
        readonly Dict _deviceInfo;

        public ClientInfo(Dict p_deviceInfo, ResponseWriter p_writer)
        {
            _deviceInfo = p_deviceInfo;
            _writer = p_writer;
            Context = Kit.HttpContext;
            UserID = Kit.ContextUserID;

            _queue = new BlockingCollection<string>();
            StartTime = DateTime.Now;
            LastMsgTime = StartTime;
        }

        /// <summary>
        /// http请求上下文
        /// </summary>
        public HttpContext Context { get; }

        /// <summary>
        /// 当前用户标识
        /// </summary>
        public long UserID { get; }

        /// <summary>
        /// 会话标识，区分同一账号多个登录的情况
        /// </summary>
        public string SessionID => _deviceInfo.Str("sessionid");

        /// <summary>
        /// 客户端系统
        /// </summary>
        public string Platform => _deviceInfo.Str("platform");

        /// <summary>
        /// 客户端系统版本
        /// </summary>
        public string Version => _deviceInfo.Str("version");

        /// <summary>
        /// 客户端设备名称
        /// </summary>
        public string DeviceName => _deviceInfo.Str("name");

        /// <summary>
        /// 客户端设备
        /// </summary>
        public string DeviceModel => _deviceInfo.Str("model");

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// 最后一次的发送时间
        /// </summary>
        public DateTime LastMsgTime { get; private set; }

        /// <summary>
        /// 发送当前用户的离线信息
        /// </summary>
        /// <returns></returns>
        public async Task SendOfflineMsg()
        {
            // 所有离线信息
            var lc = new ListCache<string>(MsgKit.MsgQueueKey);
            var ls = await lc.GetRange(UserID);
            if (ls != null && ls.Count > 0)
            {
                try
                {
                    foreach (var mi in ls)
                    {
                        await _writer.Write(mi);
                    }
                    // 删除避免重复推送
                    await lc.Delete(UserID);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "向{0}发送离线信息异常", UserID);
                }
            }
        }

        /// <summary>
        /// 增加待推送信息
        /// </summary>
        /// <param name="p_msg"></param>
        public bool AddMsg(string p_msg)
        {
            bool suc = _queue.TryAdd(p_msg);
            if (suc)
                LastMsgTime = DateTime.Now;
            return suc;
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
                var msg = _queue.Take(Context.RequestAborted);
                Log.Debug("向{0}推送：\r\n{1}", UserID, msg);
                return _writer.Write(msg);
            }
            catch { }

            _queue.Dispose();
            return Task.FromResult(false);
        }

        /// <summary>
        /// 关闭推送
        /// </summary>
        /// <returns></returns>
        public async Task Close()
        {
            // 通知客户端退出 并 禁止重连
            _queue.TryAdd(":Close");
            while (_queue.Count > 0)
            {
                await Task.Delay(20);
            }
            Context.Abort();
        }

        /// <summary>
        /// 会话结束，未通知客户端退出，也未取消本次请求，只是SendMsg()方法返回false结束长连接会话
        /// </summary>
        public void Exit()
        {
            // 触发_queue.Take()异常，会话结束
            _queue.CompleteAdding();
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        public void OnHeartbeat()
        {
            try
            {
                RpcServerKit.WriteFrame(Context.Response.BodyWriter, RpcKit.ShakeHands, false);
                LastMsgTime = DateTime.Now;
            }
            catch
            {
                // 发送失败，退出
                Exit();
            }
        }
    }
}
