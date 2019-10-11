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
using Dt.Core.EventBus;
using System;
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
        public static readonly ConcurrentDictionary<long, ClientInfo> All = new ConcurrentDictionary<long, ClientInfo>();

        /// <summary>
        /// 向用户列表中的在线用户推送信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg"></param>
        /// <returns>离线用户列表</returns>
        public static async Task<List<long>> Send(List<long> p_userIDs, MsgInfo p_msg)
        {
            string onlineMsg = p_msg.GetOnlineMsg();
            var busPush = new List<long>();
            foreach (var id in p_userIDs)
            {
                // 会话在当前服务，直接放入推送队列
                if (All.TryGetValue(id, out var ci))
                    ci.AddMsg(onlineMsg);
                else
                    busPush.Add(id);
            }

            var offlines = new List<long>();
            if (busPush.Count > 0)
            {
                // 推送结果的前缀键
                string prefixKey = Guid.NewGuid().ToString().Substring(0, 6);
                // 通知所有副本推送
                Glb.GetSvc<RemoteEventBus>().Multicast(new OnlinePushEvent { PrefixKey = prefixKey, Receivers = busPush, Msg = onlineMsg }, Glb.SvcName);

                // 收集未在线推送的
                // 等待推送完毕，时间？
                await Task.Delay(500);
                StringCache cache = new StringCache(prefixKey);
                foreach (long id in busPush)
                {
                    // 有记录的表示在线推送成功
                    if (!await cache.Remove(id.ToString()))
                        offlines.Add(id);
                }
            }
            return offlines;
        }

        /// <summary>
        /// 只在本地服务注销对指定用户的推送，同一用户在一个服务副本最后注册的有效，在不同副本时都有效
        /// </summary>
        /// <param name="p_userID"></param>
        public static void Unregister(long p_userID)
        {
            if (All.TryRemove(p_userID, out var ci))
            {
                // 会话在当前服务时立即移除
                ci.Close();
            }
        }
    }
}
