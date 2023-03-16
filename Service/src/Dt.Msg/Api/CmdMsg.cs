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
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 指令消息Api
    /// </summary>
    [Api]
    public class CmdMsg : DomainSvc
    {
        /// <summary>
        /// 向某用户的客户端推送指令信息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_msg"></param>
        /// <returns>true 在线推送</returns>
        public async Task<bool> SendCmd(long p_userID, MsgInfo p_msg)
        {
            var result = await BatchSendCmd(new List<long> { p_userID }, p_msg);
            return result.Count > 0;
        }

        /// <summary>
        /// 向用户列表的所有客户端推送指令信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <param name="p_checkReplica">多副本实例时是否检查其他副本</param>
        /// <returns>在线推送列表</returns>
        public async Task<List<long>> BatchSendCmd(List<long> p_userIDs, MsgInfo p_msg, bool p_checkReplica = true)
        {
            if (p_userIDs == null || p_userIDs.Count == 0 || p_msg == null)
                throw new Exception("待推送的用户或信息不可为空！");

            var onlines = new List<long>();
            var offlines = new List<long>();
            string onlineMsg = p_msg.GetOnlineMsg();

            // 本地单副本推送
            foreach (var id in p_userIDs)
            {
                if (Online.All.TryGetValue(id, out var ls)
                    && ls != null
                    && ls.Count > 0)
                {
                    // 在线推送，直接放入推送队列
                    foreach (var ci in ls)
                    {
                        ci.AddMsg(onlineMsg);
                    }
                    onlines.Add(id);
                }
                else
                {
                    offlines.Add(id);
                }
            }

            // 查询所有其他副本
            if (p_checkReplica)
            {
                int cnt = Kit.GetSvcReplicaCount();
                if (cnt > 1)
                {
                    foreach (var svcID in Kit.GetOtherReplicaIDs())
                    {
                        var ls = await Kit.RpcInst<List<long>>(svcID, "CmdMsg.BatchSendCmd", offlines, p_msg, false);
                        if (ls?.Count > 0)
                        {
                            onlines.AddRange(ls);
                            foreach (var ol in ls)
                            {
                                offlines.Remove(ol);
                            }
                        }

                        if (offlines.Count == 0)
                            break;
                    }
                }

            }

            // 离线
            if (offlines.Count > 0)
            {
                // 将消息保存到用户的未推送列表
                var lc = new ListCache<string>(ClientInfo.MsgQueueKey);
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
    }
}
