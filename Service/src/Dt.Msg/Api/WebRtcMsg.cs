#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// WebRTC信令服务器Api
    /// </summary>
    [Api]
    public class WebRtcMsg : DomainSvc
    {
        /// <summary>
        /// 向某在线用户的发送 WebRTC 连接请求
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <returns>true 对方在线，false对方不在线</returns>
        public Task<bool> RequestRtcConnection(long p_fromUserID, long p_toUserID)
        {
            var mi = new MsgInfo
            {
                MethodName = "WebRtcApi.RequestRtcConnection",
                Params = new List<object> { p_fromUserID },
            };
            return PushIfOnline(p_toUserID, mi);
        }

        /// <summary>
        /// 向某在线用户的发送同意 WebRTC 连接
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <returns>true 在线发送成功，false对方不在线</returns>
        public Task<bool> AcceptRtcConnection(long p_fromUserID, long p_toUserID)
        {
            var mi = new MsgInfo
            {
                MethodName = "WebRtcApi.AcceptRtcConnection",
                Params = new List<object> { p_fromUserID },
            };
            return PushIfOnline(p_toUserID, mi);
        }

        /// <summary>
        /// 拒绝接受某用户的 WebRTC 连接请求
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <returns></returns>
        public Task<bool> RefuseRtcConnection(long p_fromUserID, long p_toUserID)
        {
            var mi = new MsgInfo
            {
                MethodName = "WebRtcApi.RefuseRtcConnection",
                Params = new List<object> { p_fromUserID },
            };
            return PushIfOnline(p_toUserID, mi);
        }

        /// <summary>
        /// 向某在线用户的发送WebRTC的offer信息
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <param name="p_offer">offer内容</param>
        /// <returns>true 在线发送成功，false对方不在线</returns>
        public Task<bool> SendRtcOffer(long p_fromUserID, long p_toUserID, string p_offer)
        {
            var mi = new MsgInfo
            {
                MethodName = "WebRtcApi.RecvRtcOffer",
                Params = new List<object> { p_fromUserID, p_offer },
            };
            return PushIfOnline(p_toUserID, mi);
        }

        /// <summary>
        /// 向某用户的发送WebRTC的answer信息
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <param name="p_answer">answer内容</param>
        /// <returns>true 在线发送成功，false对方不在线</returns>
        public Task<bool> SendRtcAnswer(long p_fromUserID, long p_toUserID, string p_answer)
        {
            var mi = new MsgInfo
            {
                MethodName = "WebRtcApi.RecvRtcAnswer",
                Params = new List<object> { p_fromUserID, p_answer },
            };
            return PushIfOnline(p_toUserID, mi);
        }

        /// <summary>
        /// 向某在线用户的发送 WebRTC 的 IceCandidate 信息
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <param name="p_iceCandidate"></param>
        /// <param name="p_toCaller">是否发送给Caller</param>
        /// <returns>true 在线发送成功，false对方不在线</returns>
        public Task<bool> SendIceCandidate(long p_fromUserID, long p_toUserID, string p_iceCandidate, bool p_toCaller)
        {
            var mi = new MsgInfo
            {
                MethodName = "WebRtcApi.RecvIceCandidate",
                Params = new List<object> { p_fromUserID, p_iceCandidate, p_toCaller },
            };
            return PushIfOnline(p_toUserID, mi);
        }

        /// <summary>
        /// 挂断电话
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <param name="p_toCaller">是否发送给Caller</param>
        /// <returns>true 在线发送成功，false对方不在线</returns>
        public Task<bool> HangUp(long p_fromUserID, long p_toUserID, bool p_toCaller)
        {
            var mi = new MsgInfo
            {
                MethodName = "WebRtcApi.HangUp",
                Params = new List<object> { p_fromUserID, p_toCaller },
            };
            return PushIfOnline(p_toUserID, mi);
        }


        /// <summary>
        /// 若用户在线则推送消息，不在线返回false
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_msg"></param>
        /// <param name="p_checkReplica">多副本实例时是否检查其他副本</param>
        /// <returns>true 已在线推送，false不在线</returns>
        public async Task<bool> PushIfOnline(long p_userID, MsgInfo p_msg, bool p_checkReplica = true)
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
            if (p_checkReplica)
            {
                int cnt = Kit.GetSvcReplicaCount();
                if (cnt > 1)
                {
                    foreach (var svcID in Kit.GetOtherReplicaIDs())
                    {
                        var suc = await Kit.RpcInst<bool>(svcID, "WebRtcMsg.PushIfOnline", p_userID, p_msg, false);
                        if (suc)
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
