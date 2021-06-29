#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Serilog;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 在线推送类
    /// </summary>
    public static class Online
    {
        /// <summary>
        /// 当前服务的所有在线用户
        /// </summary>
        static readonly ConcurrentDictionary<long, List<ClientInfo>> _all = new ConcurrentDictionary<long, List<ClientInfo>>();

        /// <summary>
        /// 注册新会话，同一账号支持多个会话(但保证SessionID不同)，并向新会话发送离线信息
        /// </summary>
        /// <param name="p_client"></param>
        /// <returns></returns>
        public static async Task Register(ClientInfo p_client)
        {
            Throw.IfNull(p_client);
            long userID = p_client.UserID;

            List<ClientInfo> ls;
            if (!_all.TryGetValue(userID, out ls))
            {
                ls = new List<ClientInfo>();
                _all[userID] = ls;
            }
            if (ls.Count > 0)
            {
                // 若存在同一账号同一SessionID的会话，强制旧会话退出
                // 在不同副本时暂时未处理！！！
                var old = (from ci in ls
                           where ci.SessionID == p_client.SessionID
                           select ci).FirstOrDefault();
                if (old != null)
                {
                    // 关闭后在 Register 方法中删除账号，不需直接在此处删除！
                    await old.Close();
                    Log.Debug("{0}({1}) 关闭重复连接", userID, old.Context.GetClientIpPort());
                }
            }
            Log.Debug("{0}({1}) 在线", userID, p_client.Context.GetClientIpPort());
            ls.Add(p_client);

            await p_client.SendOfflineMsg();
        }

        /// <summary>
        /// 获取指定账号的所有会话对象
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public static List<ClientInfo> GetSessions(long p_userID)
        {
            if (_all.TryGetValue(p_userID, out var ls))
                return ls;
            return null;
        }

        /// <summary>
        /// 获取指定账号的会话对象
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_sessionID">会话标识</param>
        /// <returns></returns>
        public static ClientInfo GetSession(long p_userID, string p_sessionID)
        {
            if (_all.TryGetValue(p_userID, out var ls))
            {
                return (from ci in ls
                        where ci.SessionID == p_sessionID
                        select ci).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// 移除指定账号的会话对象
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_sessionID">会话标识</param>
        /// <returns></returns>
        public static bool RemoveSession(long p_userID, string p_sessionID)
        {
            if (!_all.TryGetValue(p_userID, out var ci))
                return false;

            bool suc = false;
            for (int i = 0; i < ci.Count; i++)
            {
                var item = ci[i];
                if (item.SessionID == p_sessionID)
                {
                    suc = true;
                    ci.RemoveAt(i);
                    break;
                }
            }

            if (ci.Count == 0)
                _all.TryRemove(p_userID, out _);
            return suc;
        }

        /// <summary>
        /// 当前服务的所有在线用户，只读
        /// </summary>
        public static IReadOnlyDictionary<long, List<ClientInfo>> All
        {
            get { return _all; }
        }

        /// <summary>
        /// 当前服务的所有会话总数，同一账号可有多个会话
        /// </summary>
        public static int TotalCount
        {
            get
            {
                int cnt = 0;
                foreach (var item in _all)
                {
                    cnt += item.Value.Count;
                }
                return cnt;
            }
        }

        #region 心跳包
        //*******************************************
        // 心跳包为解决两个数据包之间超时的问题，因以下原因不再需要
        // 服务端 Startup.ConfigureServices 中设置Kestrel不限制请求/响应的速率（MinResponseDataRate），部署在IIS时设置应用池的"闲置超时(分钟)"为0即可
        // UWP、Android无超时限制
        // iOS 在NativeMessageHandler.ios.cs设置 TimeoutIntervalForRequest 为300s，闲置5分钟重连一次
        //*******************************************

        //static int _heartbeatInterval;

        //static Online()
        //{
        //    // 心跳包间隔
        //    _heartbeatInterval = Kit.GetCfg("HeartbeatInterval", 0) * 1000;
        //    if (_heartbeatInterval > 0)
        //    {
        //        var timer = new Timer(SendHeartbeatPkg, null, _heartbeatInterval, _heartbeatInterval);
        //        GC.KeepAlive(timer);
        //    }
        //}

        //static void SendHeartbeatPkg(object source)
        //{
        //    // 客户端过多时，若总的发送心跳时间超过检查间隔，此处有问题！
        //    DateTime now = DateTime.Now;
        //    foreach (var ci in _all.Values)
        //    {
        //        if ((now - ci.LastMsgTime).TotalMilliseconds >= _heartbeatInterval)
        //            ci.OnHeartbeat();
        //    }
        //}
        #endregion
    }
}
