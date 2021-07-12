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
    public class WebRtcMsg
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
            return MsgKit.PushIfOnline(p_toUserID, mi);
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
            return MsgKit.PushIfOnline(p_toUserID, mi);
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
            return MsgKit.PushIfOnline(p_toUserID, mi);
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
            return MsgKit.PushIfOnline(p_toUserID, mi);
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
            return MsgKit.PushIfOnline(p_toUserID, mi);
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
            return MsgKit.PushIfOnline(p_toUserID, mi);
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
            return MsgKit.PushIfOnline(p_toUserID, mi);
        }
    }
}
