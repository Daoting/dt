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
                MethodName = "SysPushApi.RecvRtcOffer",
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
                MethodName = "SysPushApi.RecvRtcAnswer",
                Params = new List<object> { p_fromUserID, p_answer },
            };
            return MsgKit.PushIfOnline(p_toUserID, mi);
        }

        /// <summary>
        /// 拒绝接受某用户的WebRTC视频通话请求
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <returns></returns>
        public Task<bool> RefuseRtcOffer(long p_fromUserID, long p_toUserID)
        {
            var mi = new MsgInfo
            {
                MethodName = "SysPushApi.RefuseRtcOffer",
                Params = new List<object> { p_fromUserID },
            };
            return MsgKit.PushIfOnline(p_toUserID, mi);
        }
    }
}
