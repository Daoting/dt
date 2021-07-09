#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 即时消息Api
    /// </summary>
    [Api]
    public class InstantMsg
    {
        #region 系统消息
        /// <summary>
        /// 向某用户的客户端推送系统消息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_msg"></param>
        /// <returns>true 在线推送</returns>
        public async Task<bool> SendMsg(long p_userID, string p_msg)
        {
            var result = await MsgKit.Push(new List<long> { p_userID }, WrapperMsg(p_msg));
            return result.Count > 0;
        }

        /// <summary>
        /// 向用户列表的所有客户端推送系统消息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <returns>在线推送列表</returns>
        public Task<List<long>> BatchSendMsg(List<long> p_userIDs, string p_msg)
        {
            return MsgKit.Push(p_userIDs, WrapperMsg(p_msg));
        }

        /// <summary>
        /// 向所有副本的所有在线用户广播信息
        /// </summary>
        /// <param name="p_msg"></param>
        public void SendMsgToOnline(string p_msg)
        {
            _ = MsgKit.BroadcastAllOnline(WrapperMsg(p_msg));
        }
        #endregion

        #region 聊天
        /// <summary>
        /// 向某用户的客户端推送聊天信息，可通过指定LetterInfo.LetterType为Undo撤回信息
        /// </summary>
        /// <param name="p_userID">目标用户</param>
        /// <param name="p_letter">聊天信息</param>
        /// <returns>true 在线推送</returns>
        public async Task<bool> SendLetter(long p_userID, LetterInfo p_letter)
        {
            var result = await MsgKit.Push(new List<long> { p_userID }, WrapperLetter(p_letter));
            return result.Count > 0;
        }

        /// <summary>
        /// 向用户列表的所有客户端推送聊天信息，可通过指定LetterInfo.LetterType为Undo撤回信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_letter">聊天信息</param>
        /// <returns>在线推送列表</returns>
        public Task<List<long>> BatchSendLetter(List<long> p_userIDs, LetterInfo p_letter)
        {
            return MsgKit.Push(p_userIDs, WrapperLetter(p_letter));
        }
        #endregion

        #region 包装成MsgInfo
        MsgInfo WrapperMsg(string p_msg)
        {
            LetterInfo li = new LetterInfo
            {
                ID = Kit.NewGuid,
                SenderID = 0,
                SenderName = "系统",
                LetterType = LetterType.Text,
                Content = p_msg,
                SendTime = DateTime.Now
            };
            return WrapperLetter(li);
        }

        MsgInfo WrapperLetter(LetterInfo p_letter)
        {
            var mi = new MsgInfo
            {
                MethodName = "SysPushApi.ReceiveLetter",
                Params = new List<object> { p_letter },
            };

            if (p_letter.LetterType != LetterType.Undo)
            {
                mi.Title = p_letter.SenderName;
                mi.Content = GetToastMsg(p_letter.LetterType, p_letter.Content);
            }
            return mi;
        }

        string GetToastMsg(LetterType p_type, string p_content)
        {
            string msg = string.Empty;
            switch (p_type)
            {
                case LetterType.Text:
                    if (p_content.Length > 70)
                        msg = p_content.Substring(0, 70) + "…";
                    else
                        msg = p_content;
                    break;
                case LetterType.File:
                    msg = "🏬 给您发来文件";
                    break;
                case LetterType.Image:
                    msg = "🌄 给您发来图片";
                    break;
                case LetterType.Video:
                    msg = "🌉 给您发来视频";
                    break;
                case LetterType.Voice:
                    msg = "📢 给您发来语音";
                    break;
                case LetterType.Link:
                    msg = "💨 给您发来链接";
                    break;
                default:
                    break;
            }
            return msg;
        }
        #endregion
    }
}
