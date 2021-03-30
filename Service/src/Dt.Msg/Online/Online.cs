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
        static readonly ConcurrentDictionary<long, ClientInfo> _all = new ConcurrentDictionary<long, ClientInfo>();

        /// <summary>
        /// 注册新会话，注销同一用户的旧会话，向新会话发送离线信息
        /// </summary>
        /// <param name="p_client"></param>
        /// <returns></returns>
        public static Task Register(ClientInfo p_client)
        {
            Throw.IfNull(p_client);
            long userID = p_client.UserID;

            // 通知已注册的客户端关闭会话
            // 同一用户在一个服务副本最后注册的有效，在不同副本时都有效！！！
            if (_all.TryRemove(userID, out var old))
            {
                // 旧会话
                if (old.DeviceModel == p_client.DeviceModel && old.DeviceName == p_client.DeviceName)
                {
                    // 同一设备多次注册
                    Log.Debug("{0}({1}) 同一设备重复连接", userID, p_client.Context.GetClientIpPort());
                    old.Exit();
                }
                else
                {
                    Log.Debug("{0}({1}) 重复连接", userID, p_client.Context.GetClientIpPort());
                    old.StopPush();
                    old.Close();
                }
            }
            else
            {
                Log.Debug("{0}({1}) 在线", userID, p_client.Context.GetClientIpPort());
            }

            _all[userID] = p_client;
            return p_client.SendOfflineMsg();
        }

        /// <summary>
        /// 获取指定用户的会话对象
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public static ClientInfo GetClient(long p_userID)
        {
            if (_all.TryGetValue(p_userID, out var ci))
                return ci;
            return null;
        }

        /// <summary>
        /// 移除指定用户的会话对象
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public static bool RemoveClient(long p_userID)
        {
            return _all.TryRemove(p_userID, out var ci);
        }

        /// <summary>
        /// 当前服务的所有在线用户，只读
        /// </summary>
        public static IReadOnlyDictionary<long, ClientInfo> All
        {
            get { return _all; }
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
        //    _heartbeatInterval = Glb.GetCfg("HeartbeatInterval", 0) * 1000;
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
