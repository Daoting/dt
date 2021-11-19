#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Base.Chat
{
    /// <summary>
    /// WebRTC处理
    /// </summary>
    public partial class WebRtcApi
    {
        /// <summary>
        /// 收到对方的 WebRTC 连接请求
        /// </summary>
        /// <param name="p_fromUserID"></param>
        public void RequestRtcConnection(long p_fromUserID)
        {
            if (VideoRecver.Inst == null)
            {
                _ = new VideoRecver().ShowDlg(p_fromUserID);
            }
        }

        /// <summary>
        /// 收到对方同意 WebRTC 连接
        /// </summary>
        /// <param name="p_fromUserID"></param>
        public void AcceptRtcConnection(long p_fromUserID)
        {
            VideoCaller.Inst?.OnAcceptConnection();
        }

        /// <summary>
        /// 收到对方拒绝 WebRTC 连接
        /// </summary>
        /// <param name="p_fromUserID"></param>
        public void RefuseRtcConnection(long p_fromUserID)
        {
            VideoCaller.Inst?.OnHangUp();
        }

        /// <summary>
        /// 收到对方的Offer
        /// </summary>
        /// <param name="p_fromUserID"></param>
        /// <param name="p_offer"></param>
        public void RecvRtcOffer(long p_fromUserID, string p_offer)
        {
            VideoRecver.Inst?.OnRecvOffer(p_offer);
        }

        /// <summary>
        /// 收到对方的Answer
        /// </summary>
        /// <param name="p_fromUserID"></param>
        /// <param name="p_answer"></param>
        public void RecvRtcAnswer(long p_fromUserID, string p_answer)
        {
            VideoCaller.Inst?.OnRecvAnswer(p_answer);
        }

        /// <summary>
        /// 收到对方的 IceCandidate 信息
        /// </summary>
        /// <param name="p_fromUserID"></param>
        /// <param name="p_iceCandidate"></param>
        /// <param name="p_toCaller">是否发送给Caller</param>
        public void RecvIceCandidate(long p_fromUserID, string p_iceCandidate, bool p_toCaller)
        {
            if (p_toCaller && VideoCaller.Inst != null)
                VideoCaller.Inst.OnRecvIceCandidate(p_iceCandidate);
            //else if (VideoRecver.Inst != null)
            //    VideoRecver.Inst.OnRecvIceCandidate(p_iceCandidate);
        }

        public void HangUp(long p_fromUserID, bool p_toCaller)
        {
            if (p_toCaller && VideoCaller.Inst != null)
                VideoCaller.Inst.OnHangUp();
            else if (VideoRecver.Inst != null)
                VideoRecver.Inst.OnHangUp();
        }
    }
}
