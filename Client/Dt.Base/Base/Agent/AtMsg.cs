#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 消息服务Api代理类（自动生成）
    /// </summary>
    public static class AtMsg
    {
        #region Pusher
        /// <summary>
        /// 客户端注册在线推送
        /// </summary>
        /// <param name="p_deviceInfo">客户端设备信息</param>
        /// <returns></returns>
        public static Task<ResponseReader> Register(Dict p_deviceInfo)
        {
            return new ServerStreamRpc(
                "msg",
                "Pusher.Register",
                p_deviceInfo
            ).Call();
        }

        /// <summary>
        /// 注销客户端
        /// 1. 早期版本在客户端关闭时会造成多个无关的ClientInfo收到Abort，只能从服务端Abort，升级到.net 5.0后不再出现该现象！！！
        /// 2. 使用客户端 response.Dispose() 主动关闭时，不同平台现象不同，服务端能同步收到uwp关闭消息，但android ios上不行，
        /// 直到再次推送时才发现客户端已关闭，为了保证客户端在线状态实时更新，客户端只能调用该方法！！！
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_sessionID">会话标识，区分同一账号多个登录的情况</param>
        /// <returns></returns>
        public static Task<bool> Unregister(long p_userID, string p_sessionID)
        {
            return new UnaryRpc(
                "msg",
                "Pusher.Unregister",
                p_userID,
                p_sessionID
            ).Call<bool>();
        }

        /// <summary>
        /// 判断用户是否在线，查询所有副本
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>false 不在线</returns>
        public static Task<bool> IsOnline(long p_userID)
        {
            return new UnaryRpc(
                "msg",
                "Pusher.IsOnline",
                p_userID
            ).Call<bool>();
        }

        /// <summary>
        /// 查询所有副本，获取某账号的所有会话信息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>会话信息列表</returns>
        public static Task<List<Dict>> GetAllSessions(long p_userID)
        {
            return new UnaryRpc(
                "msg",
                "Pusher.GetAllSessions",
                p_userID
            ).Call<List<Dict>>();
        }

        /// <summary>
        /// 实时获取所有副本的在线用户总数
        /// </summary>
        /// <returns>Dict结构：key为副本id，value为副本会话总数</returns>
        public static Task<Dict> GetOnlineCount()
        {
            return new UnaryRpc(
                "msg",
                "Pusher.GetOnlineCount"
            ).Call<Dict>();
        }
        #endregion

        #region InstantMsg
        /// <summary>
        /// 向某用户的客户端推送系统消息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_msg"></param>
        /// <returns>true 在线推送</returns>
        public static Task<bool> SendMsg(long p_userID, string p_msg)
        {
            return new UnaryRpc(
                "msg",
                "InstantMsg.SendMsg",
                p_userID,
                p_msg
            ).Call<bool>();
        }

        /// <summary>
        /// 向用户列表的所有客户端推送系统消息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <returns>在线推送列表</returns>
        public static Task<List<long>> BatchSendMsg(List<long> p_userIDs, string p_msg)
        {
            return new UnaryRpc(
                "msg",
                "InstantMsg.BatchSendMsg",
                p_userIDs,
                p_msg
            ).Call<List<long>>();
        }

        /// <summary>
        /// 向所有副本的所有在线用户广播信息
        /// </summary>
        /// <param name="p_msg"></param>
        public static Task SendMsgToOnline(string p_msg)
        {
            return new UnaryRpc(
                "msg",
                "InstantMsg.SendMsgToOnline",
                p_msg
            ).Call<object>();
        }

        /// <summary>
        /// 向某用户的客户端推送聊天信息，可通过指定LetterInfo.LetterType为Undo撤回信息
        /// </summary>
        /// <param name="p_userID">目标用户</param>
        /// <param name="p_letter">聊天信息</param>
        /// <returns>true 在线推送</returns>
        public static Task<bool> SendLetter(long p_userID, LetterInfo p_letter)
        {
            return new UnaryRpc(
                "msg",
                "InstantMsg.SendLetter",
                p_userID,
                p_letter
            ).Call<bool>();
        }

        /// <summary>
        /// 向用户列表的所有客户端推送聊天信息，可通过指定LetterInfo.LetterType为Undo撤回信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_letter">聊天信息</param>
        /// <returns>在线推送列表</returns>
        public static Task<List<long>> BatchSendLetter(List<long> p_userIDs, LetterInfo p_letter)
        {
            return new UnaryRpc(
                "msg",
                "InstantMsg.BatchSendLetter",
                p_userIDs,
                p_letter
            ).Call<List<long>>();
        }
        #endregion

        #region CmdMsg
        /// <summary>
        /// 向某用户的客户端推送指令信息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_msg"></param>
        /// <returns>true 在线推送</returns>
        public static Task<bool> SendCmd(long p_userID, MsgInfo p_msg)
        {
            return new UnaryRpc(
                "msg",
                "CmdMsg.SendCmd",
                p_userID,
                p_msg
            ).Call<bool>();
        }

        /// <summary>
        /// 向用户列表的所有客户端推送指令信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <returns>在线推送列表</returns>
        public static Task<List<long>> BatchSendCmd(List<long> p_userIDs, MsgInfo p_msg)
        {
            return new UnaryRpc(
                "msg",
                "CmdMsg.BatchSendCmd",
                p_userIDs,
                p_msg
            ).Call<List<long>>();
        }
        #endregion

        #region WebRtcMsg
        /// <summary>
        /// 向某在线用户的发送 WebRTC 连接请求
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <returns>true 对方在线，false对方不在线</returns>
        public static Task<bool> RequestRtcConnection(long p_fromUserID, long p_toUserID)
        {
            return new UnaryRpc(
                "msg",
                "WebRtcMsg.RequestRtcConnection",
                p_fromUserID,
                p_toUserID
            ).Call<bool>();
        }

        /// <summary>
        /// 向某在线用户的发送同意 WebRTC 连接
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <returns>true 在线发送成功，false对方不在线</returns>
        public static Task<bool> AcceptRtcConnection(long p_fromUserID, long p_toUserID)
        {
            return new UnaryRpc(
                "msg",
                "WebRtcMsg.AcceptRtcConnection",
                p_fromUserID,
                p_toUserID
            ).Call<bool>();
        }

        /// <summary>
        /// 拒绝接受某用户的 WebRTC 连接请求
        /// </summary>
        /// <param name="p_fromUserID">发送者</param>
        /// <param name="p_toUserID">接收者</param>
        /// <returns></returns>
        public static Task<bool> RefuseRtcConnection(long p_fromUserID, long p_toUserID)
        {
            return new UnaryRpc(
                "msg",
                "WebRtcMsg.RefuseRtcConnection",
                p_fromUserID,
                p_toUserID
            ).Call<bool>();
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
            return new UnaryRpc(
                "msg",
                "WebRtcMsg.SendRtcOffer",
                p_fromUserID,
                p_toUserID,
                p_offer
            ).Call<bool>();
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
            return new UnaryRpc(
                "msg",
                "WebRtcMsg.SendRtcAnswer",
                p_fromUserID,
                p_toUserID,
                p_answer
            ).Call<bool>();
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
            return new UnaryRpc(
                "msg",
                "WebRtcMsg.SendIceCandidate",
                p_fromUserID,
                p_toUserID,
                p_iceCandidate,
                p_toCaller
            ).Call<bool>();
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
            return new UnaryRpc(
                "msg",
                "WebRtcMsg.HangUp",
                p_fromUserID,
                p_toUserID,
                p_toCaller
            ).Call<bool>();
        }
        #endregion

        #region SubscribeMsg
        /// <summary>
        /// 发布订阅信息
        /// </summary>
        /// <param name="p_subscribeID">订阅号标识</param>
        /// <param name="p_msg">信息内容</param>
        /// <param name="p_offlineTip">离线推送时的提示信息</param>
        /// <returns>在线收到的人数</returns>
        public static Task<int> Publish(long p_subscribeID, string p_msg, string p_offlineTip)
        {
            return new UnaryRpc(
                "msg",
                "SubscribeMsg.Publish",
                p_subscribeID,
                p_msg,
                p_offlineTip
            ).Call<int>();
        }
        #endregion
    }
}
