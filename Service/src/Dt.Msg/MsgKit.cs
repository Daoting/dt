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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    public static class MsgKit
    {
        const int _maxRetry = 12;
        const int _delayMilli = 100;

        /// <summary>
        /// 用户未推送消息列表 msg:Queue:userid = list(msginfo)
        /// </summary>
        public const string MsgQueueKey = "msg:Queue";

        /// <summary>
        /// 向用户列表中的在线用户推送信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <returns>在线推送列表</returns>
        public static async Task<List<long>> Push(List<long> p_userIDs, MsgInfo p_msg)
        {
            if (p_userIDs == null || p_userIDs.Count == 0 || p_msg == null)
                throw new Exception("待推送的用户或信息不可为空！");

            var onlines = new List<long>();
            var offlines = new List<long>();
            string onlineMsg = p_msg.GetOnlineMsg();
            int cnt = await Kit.GetSvcReplicaCount();
            if (cnt > 1)
            {
                // Msg服务多副本推送
                await PushMultipleReplicas(p_userIDs, onlineMsg, onlines, offlines, cnt);
            }
            else
            {
                // 本地单副本推送
                PushLocal(p_userIDs, onlineMsg, onlines, offlines);
            }

            // 离线
            if (offlines.Count > 0)
            {
                // 将消息保存到用户的未推送列表
                var lc = new ListCache<string>(MsgQueueKey);
                foreach (long id in offlines)
                {
                    await lc.RightPush(id, onlineMsg);
                }

                // 推送离线提醒
                if (!string.IsNullOrEmpty(p_msg.Title))
                    Offline.Add(offlines, p_msg);
            }
            return onlines;
        }

        /// <summary>
        /// 若用户在线则推送消息，不在线返回false
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_msg"></param>
        /// <returns>true 已在线推送，false不在线</returns>
        public static async Task<bool> PushIfOnline(long p_userID, MsgInfo p_msg)
        {
            if (Online.All.TryGetValue(p_userID, out var ls)
                && ls != null
                && ls.Count > 0)
            {
                // 本地在线推送
                ls[0].AddMsg(p_msg.GetOnlineMsg());
                return true;
            }

            // 查询所有其他副本
            int cnt = await Kit.GetSvcReplicaCount();
            if (cnt > 1)
            {
                string key = $"msg:PushIfOnline:{p_userID}:{Guid.NewGuid().ToString().Substring(0, 6)}";
                Kit.RemoteMulticast(new OnlinePushEvent
                {
                    PrefixKey = key,
                    Receivers = new List<long> { p_userID },
                    Msg = p_msg.GetOnlineMsg(),
                    PushFirstSession = true
                });

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
        /// 向所有副本的所有在线用户广播信息
        /// </summary>
        /// <param name="p_msg"></param>
        public static async Task BroadcastAllOnline(MsgInfo p_msg)
        {
            Throw.IfNull(p_msg);

            var msg = p_msg.GetOnlineMsg();
            int cnt = await Kit.GetSvcReplicaCount();
            if (cnt > 1)
            {
                // 多副本推送
                Kit.RemoteMulticast(new BroadcastEvent { Msg = msg });
            }
            else
            {
                // 本地单副本推送
                foreach (var ls in Online.All.Values)
                {
                    foreach (var ci in ls)
                    {
                        ci.AddMsg(msg);
                    }
                }
            }
        }

        /// <summary>
        /// 多副本推送
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <param name="p_onlines"></param>
        /// <param name="p_offlines"></param>
        /// <param name="p_cnt"></param>
        static async Task PushMultipleReplicas(List<long> p_userIDs, string p_msg, List<long> p_onlines, List<long> p_offlines, int p_cnt)
        {
            /* 
             * Msg服务多副本运行时，有以下两种处理方案：
             * 1. 由当前副本通知所有副本，每个副本都检查会话是否存在，存在时推送并做已推送标志，再由当前副本汇总离线列表
             * 2. 将所有副本的会话信息缓存在redis，当前副本查询缓存获取会话所在的副本，再通过事件总线推送到指定副本
             * 当前采用方案1，因方案2在某个副本非正常退出时，缓存的会话未能与实际同步，造成冗余的推送！
             * 方案1的缺点是所有副本都需要检查会话是否存在
            */

            // 推送结果的前缀键
            string prefixKey = "msg:Push:" + Guid.NewGuid().ToString().Substring(0, 6);
            // 通知所有副本推送
            Kit.RemoteMulticast(new OnlinePushEvent { PrefixKey = prefixKey, Receivers = p_userIDs, Msg = p_msg });

            // 等待收集
            int total, retry = 0;
            var sc = new StringCache(prefixKey);
            do
            {
                await Task.Delay(_delayMilli);
                total = await sc.Get<int>("cnt");
                retry++;
            }
            while (total < p_cnt && retry < _maxRetry);

            // 删除统计总数
            await sc.Delete("cnt");

            foreach (long id in p_userIDs)
            {
                // 有记录的表示在线推送成功
                if (await sc.Delete(id))
                    p_onlines.Add(id);
                else
                    p_offlines.Add(id);
            }
        }

        /// <summary>
        /// 本地单副本推送
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <param name="p_onlines"></param>
        /// <param name="p_offlines"></param>
        static void PushLocal(List<long> p_userIDs, string p_msg, List<long> p_onlines, List<long> p_offlines)
        {
            foreach (var id in p_userIDs)
            {
                if (Online.All.TryGetValue(id, out var ls)
                    && ls != null
                    && ls.Count > 0)
                {
                    // 在线推送，直接放入推送队列
                    foreach (var ci in ls)
                    {
                        ci.AddMsg(p_msg);
                    }
                    p_onlines.Add(id);
                }
                else
                {
                    p_offlines.Add(id);
                }
            }
        }
    }
}
