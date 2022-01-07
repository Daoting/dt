namespace Dt.Agent
{
    /// <summary>
    /// WebRTC信令服务器Api
    /// </summary>
    public partial class AtMsg
    {
        /// <summary>
        /// 向某在线用户的发送 WebRTC 连接请求
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <returns>true 对方在线，false对方不在线</returns>
        public static Task<bool> RequestRtcConnection(long p_fromUserID, long p_toUserID)
        {
            return Kit.Rpc<bool>(
                "msg",
                "WebRtcMsg.RequestRtcConnection",
                p_fromUserID,
                p_toUserID
            );
        }

        /// <summary>
        /// 向某在线用户的发送同意 WebRTC 连接
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <returns>true 在线发送成功，false对方不在线</returns>
        public static Task<bool> AcceptRtcConnection(long p_fromUserID, long p_toUserID)
        {
            return Kit.Rpc<bool>(
                "msg",
                "WebRtcMsg.AcceptRtcConnection",
                p_fromUserID,
                p_toUserID
            );
        }

        /// <summary>
        /// 拒绝接受某用户的 WebRTC 连接请求
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <returns></returns>
        public static Task<bool> RefuseRtcConnection(long p_fromUserID, long p_toUserID)
        {
            return Kit.Rpc<bool>(
                "msg",
                "WebRtcMsg.RefuseRtcConnection",
                p_fromUserID,
                p_toUserID
            );
        }

        /// <summary>
        /// 向某在线用户的发送WebRTC的offer信息
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <param name="p_offer">offer内容</param>
        /// <returns>true 在线发送成功，false对方不在线</returns>
        public static Task<bool> SendRtcOffer(long p_fromUserID, long p_toUserID, string p_offer)
        {
            return Kit.Rpc<bool>(
                "msg",
                "WebRtcMsg.SendRtcOffer",
                p_fromUserID,
                p_toUserID,
                p_offer
            );
        }

        /// <summary>
        /// 向某用户的发送WebRTC的answer信息
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <param name="p_answer">answer内容</param>
        /// <returns>true 在线发送成功，false对方不在线</returns>
        public static Task<bool> SendRtcAnswer(long p_fromUserID, long p_toUserID, string p_answer)
        {
            return Kit.Rpc<bool>(
                "msg",
                "WebRtcMsg.SendRtcAnswer",
                p_fromUserID,
                p_toUserID,
                p_answer
            );
        }

        /// <summary>
        /// 向某在线用户的发送 WebRTC 的 IceCandidate 信息
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <param name="p_iceCandidate"></param>
        /// <param name="p_toCaller">是否发送给Caller</param>
        /// <returns>true 在线发送成功，false对方不在线</returns>
        public static Task<bool> SendIceCandidate(long p_fromUserID, long p_toUserID, string p_iceCandidate, bool p_toCaller)
        {
            return Kit.Rpc<bool>(
                "msg",
                "WebRtcMsg.SendIceCandidate",
                p_fromUserID,
                p_toUserID,
                p_iceCandidate,
                p_toCaller
            );
        }

        /// <summary>
        /// 挂断电话
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <param name="p_toCaller">是否发送给Caller</param>
        /// <returns>true 在线发送成功，false对方不在线</returns>
        public static Task<bool> HangUp(long p_fromUserID, long p_toUserID, bool p_toCaller)
        {
            return Kit.Rpc<bool>(
                "msg",
                "WebRtcMsg.HangUp",
                p_fromUserID,
                p_toUserID,
                p_toCaller
            );
        }
    }
}
