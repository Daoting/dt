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
        /// 判断用户是否在线，查询所有副本
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>null 不在线</returns>
        public static Task<Dict> IsOnline(long p_userID)
        {
            return new UnaryRpc(
                "msg",
                "Pusher.IsOnline",
                p_userID
            ).Call<Dict>();
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
