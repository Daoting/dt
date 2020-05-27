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
            long userID = p_client.Context.UserID;

            // 通知已注册的客户端关闭会话
            // 同一用户在一个服务副本最后注册的有效，在不同副本时都有效！！！
            if (_all.TryRemove(userID, out var old))
            {
                // 旧会话
                if (old.DeviceModel == p_client.DeviceModel && old.DeviceName == p_client.DeviceName)
                {
                    // 同一设备多次注册
                    old.Exit();
                    Log.Debug($"重复注册：{userID}  旧连接({old.Context.Context.Connection.Id})  新连接({p_client.Context.Context.Connection.Id})");
                }
                else
                {
                    old.StopPush();
                    old.Close();
                    Log.Debug($"替换：{userID}  旧连接({old.Context.Context.Connection.Id})  新连接({p_client.Context.Context.Connection.Id})");
                }
            }
            else
            {
                Log.Debug($"+{p_client.Context.Context.Connection.Id}：{userID}  ");
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
    }
}
