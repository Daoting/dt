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
        /// <summary>
        /// 用户未推送消息列表 msg:userid = list(msginfo)
        /// </summary>
        public const string MsgQueueKey = "msg:";

        /// <summary>
        /// Msg服务是否正在运行多个副本，当前从service.json中取，可否使用KubeClient?
        /// </summary>
        public static readonly bool IsMultipleReplicas = Glb.IsInDocker ? Glb.GetCfg("IsMultipleReplicas", false) : false;

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

            // 在线推送
            List<long> result = new List<long>();
            string onlineMsg = p_msg.GetOnlineMsg();
            var offlines = new List<long>();
            foreach (var id in p_userIDs)
            {
                // 会话在当前服务，直接放入推送队列
                if (Online.All.TryGetValue(id, out var ci) && ci.AddMsg(onlineMsg))
                {
                    result.Add(id);
                    continue;
                }
                offlines.Add(id);
            }

            /* Msg服务多副本运行时，有以下两种处理方案：
             * 1. 由当前副本通知其他所有副本，每个副本都检查会话是否存在，存在时推送并做已推送标志，再由当前副本汇总离线列表
             * 2. 将所有副本的会话信息缓存在redis，当前副本查询缓存获取会话所在的副本，再通过事件总线推送到指定副本
             * 当前采用方案1，因方案2在某个副本非正常退出时，缓存的会话未能与实际同步，造成冗余的推送！
             * 方案1的缺点是所有副本都需要检查会话是否存在
            */
            if (offlines.Count > 0 && IsMultipleReplicas)
            {
                // 推送结果的前缀键
                string prefixKey = Guid.NewGuid().ToString().Substring(0, 6);
                // 通知所有副本推送
                Glb.GetSvc<RemoteEventBus>().Multicast(new OnlinePushEvent { PrefixKey = prefixKey, Receivers = offlines, Msg = onlineMsg }, Glb.SvcName);

                // 收集未在线推送的
                // 等待推送完毕，时间？
                await Task.Delay(500);

                StringCache cache = new StringCache(prefixKey);
                var temp = new List<long>();
                foreach (long id in offlines)
                {
                    // 有记录的表示在线推送成功
                    if (await cache.Remove(id.ToString()))
                        result.Add(id);
                    else
                        temp.Add(id);
                }
                offlines = temp;
            }

            // 离线
            if (offlines.Count > 0)
            {
                // 将消息保存到用户的未推送列表
                var db = Redis.Db;
                foreach (long id in offlines)
                {
                    string key = MsgQueueKey + id.ToString();
                    await db.ListRightPushAsync(key, onlineMsg);
                }

                // 推送离线提醒
                if (!string.IsNullOrEmpty(p_msg.Title))
                    Offline.Add(offlines, p_msg);
            }
            return result;
        }

        /// <summary>
        /// 向所有副本的所有在线用户广播信息
        /// </summary>
        /// <param name="p_msg"></param>
        public static void PushToOnline(MsgInfo p_msg)
        {
            Throw.IfNull(p_msg);

            // 单副本也统一走 RemoteEventBus
            Glb.GetSvc<RemoteEventBus>().Multicast(new BroadcastEvent { Msg = p_msg.GetOnlineMsg() }, Glb.SvcName);
        }
    }
}
