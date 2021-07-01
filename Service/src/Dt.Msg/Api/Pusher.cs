#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Dt.Core.EventBus;
using Dt.Core.Rpc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 消息推送Api
    /// </summary>
    [Api]
    public class Pusher
    {
        const int _maxRetry = 12;
        const int _delayMilli = 100;

        /// <summary>
        /// 客户端注册在线推送
        /// </summary>
        /// <param name="p_deviceInfo">客户端设备信息</param>
        /// <param name="p_writer"></param>
        /// <returns></returns>
        public async Task Register(Dict p_deviceInfo, ResponseWriter p_writer)
        {
            var ci = new ClientInfo(p_deviceInfo, p_writer);
            // 注册新会话(同一账号支持多个会话)，并向新会话发送离线信息
            await Online.Register(ci);

            // 推送
            while (await ci.SendMsg())
            { }

            // 推送结束，删除会话
            Online.RemoveSession(ci.UserID, ci.SessionID);
            Log.Debug("{0} 离线", ci.UserID);
        }

        /// <summary>
        /// 注销客户端
        /// 1. 早期版本在客户端关闭时会造成多个无关的ClientInfo收到Abort，只能从服务端Abort，升级到.net 5.0后不再出现该现象！！！
        /// 2. 使用客户端 response.Dispose() 主动关闭时，不同平台现象不同，服务端能同步收到uwp关闭消息，但android ios上不行，
        ///    直到再次推送时才发现客户端已关闭，为了保证客户端在线状态实时更新，客户端只能调用该方法！！！
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_sessionID">会话标识，区分同一账号多个登录的情况</param>
        /// <returns></returns>
        public async Task<bool> Unregister(long p_userID, string p_sessionID)
        {
            var ci = Online.GetSession(p_userID, p_sessionID);
            if (ci != null)
            {
                await ci.Close();
                return true;
            }

            // 查询所有其他副本
            int cnt = await Kit.GetSvcReplicaCount();
            if (cnt > 1)
            {
                string key = $"msg:Unregister:{p_userID}:{Guid.NewGuid().ToString().Substring(0, 6)}";
                Kit.RemoteMulticast(new UnregisterEvent { CacheKey = key, UserID = p_userID, SessionID = p_sessionID });

                // 等待收集
                int total, retry = 0;
                var sc = new StringCache(key);
                do
                {
                    await Task.Delay(_delayMilli);
                    total = await sc.Get<int>("cnt");
                    retry++;
                }
                while (total < cnt && retry < _maxRetry);

                // 删除统计总数
                await sc.Delete("cnt");

                // 存在键值表示在线
                if (await sc.Delete(null))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判断用户是否在线，查询所有副本
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>false 不在线</returns>
        public async Task<bool> IsOnline(long p_userID)
        {
            var ls = Online.GetSessions(p_userID);
            if (ls != null && ls.Count > 0)
                return true;

            // 查询所有其他副本
            int cnt = await Kit.GetSvcReplicaCount();
            if (cnt > 1)
            {
                string key = $"msg:IsOnline:{p_userID}:{Guid.NewGuid().ToString().Substring(0, 6)}";
                Kit.RemoteMulticast(new IsOnlineEvent { CacheKey = key, UserID = p_userID });

                // 等待收集
                int total, retry = 0;
                var sc = new StringCache(key);
                do
                {
                    await Task.Delay(_delayMilli);
                    total = await sc.Get<int>("cnt");
                    retry++;
                }
                while (total < cnt && retry < _maxRetry);

                // 删除统计总数
                await sc.Delete("cnt");

                // 存在键值表示在线
                if (await sc.Delete(null))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 查询所有副本，获取某账号的所有会话信息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>会话信息列表</returns>
        public async Task<List<Dict>> GetAllSessions(long p_userID)
        {
            List<Dict> result = new List<Dict>();
            int cnt = await Kit.GetSvcReplicaCount();
            if (cnt > 1)
            {
                // 查询所有副本
                string key = $"msg:Sessions:{p_userID}:{Guid.NewGuid().ToString().Substring(0, 6)}";
                Kit.RemoteMulticast(new UserSessionsEvent { CacheKey = key, UserID = p_userID });

                // 等待收集
                int total, retry = 0;
                var sc = new StringCache(key);
                do
                {
                    await Task.Delay(_delayMilli);
                    total = await sc.Get<int>("cnt");
                    retry++;
                }
                while (total < cnt && retry < _maxRetry);

                // 删除统计总数
                await sc.Delete("cnt");

                var hc = new HashCache(key);
                var hash = await hc.GetAll(null);
                if (hash != null && hash.Length > 0)
                {
                    await hc.Delete(null);

                    var dt = hash.ToDict();
                    foreach (var item in dt)
                    {
                        var ss = Kit.Deserialize<List<Dict>>((string)item.Value);
                        if (ss != null && ss.Count > 0)
                            result.AddRange(ss);
                    }
                }
            }
            else
            {
                // 当前单副本
                var ls = Online.GetSessions(p_userID);
                if (ls != null && ls.Count > 0)
                {
                    foreach (var ci in ls)
                    {
                        result.Add(new Dict
                        {
                            { "userid", ci.UserID },
                            { "svcid", Kit.SvcID },
                            { "starttime", ci.StartTime.ToString() },
                            { "platform", ci.Platform },
                            { "version", ci.Version },
                            { "devicename", ci.DeviceName },
                            { "devicemodel", ci.DeviceModel },
                        });
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 实时获取所有副本的在线用户总数
        /// </summary>
        /// <returns>Dict结构：key为副本id，value为副本会话总数</returns>
        public async Task<Dict> GetOnlineCount()
        {
            Dict result = null;
            int cnt = await Kit.GetSvcReplicaCount();
            if (cnt > 1)
            {
                // 所有副本
                string key = "msg:OnlineCount:" + Guid.NewGuid().ToString().Substring(0, 6);
                Kit.RemoteMulticast(new OnlineCountEvent { CacheKey = key });

                // 等待收集
                int total, retry = 0;
                var sc = new StringCache(key);
                do
                {
                    await Task.Delay(_delayMilli);
                    total = await sc.Get<int>("cnt");
                    retry++;
                }
                while (total < cnt && retry < _maxRetry);

                // 删除统计总数
                await sc.Delete("cnt");

                var hc = new HashCache(key);
                var hash = await hc.GetAll(null);
                if (hash != null && hash.Length > 0)
                {
                    await hc.Delete(null);
                    result = hash.ToDict();
                }
            }
            else
            {
                // 当前单副本
                result = new Dict { { Kit.SvcID, Online.TotalCount } };
            }
            return result;
        }
    }
}
