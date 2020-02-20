#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Dt.Core.Sqlite;
using System.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 
    /// </summary>
    public static class LetterManager
    {
        /// <summary>
        /// 增加一条聊天信息事件
        /// </summary>
        public static event Action<Letter> NewLetter;

        /// <summary>
        /// 未读消息状态变化事件，参数为对方的userid
        /// </summary>
        public static event Action<long> StateChanged;

        #region 接收信息
        /// <summary>
        /// 接收服务器推送的聊天信息
        /// </summary>
        /// <param name="p_letter"></param>
        internal static void ReceiveLetter(LetterInfo p_letter)
        {
            if (p_letter == null || string.IsNullOrEmpty(p_letter.ID))
                return;

            // 撤回消息
            if (p_letter.LetterType == LetterType.Undo)
            {
                var letter = AtLocal.GetFirst<Letter>("select * from Letter where MsgID=@msgid and LoginID=@loginid and IsReceived=1", new Dict { { "msgid", p_letter.ID }, { "loginid", AtUser.ID } });
                if (letter != null)
                {
                    letter.LetterType = LetterType.Undo;
                    AtLocal.Save(letter);

                    NewLetter?.Invoke(letter);
                }
                return;
            }

            // 新消息
            Letter l = new Letter();
            l.LoginID = AtUser.ID;

            l.MsgID = p_letter.ID;
            l.OtherID = p_letter.SenderID;
            l.OtherName = p_letter.SenderName;
            l.LetterType = p_letter.LetterType;
            l.Content = p_letter.Content;
            l.STime = p_letter.SendTime;

            l.IsReceived = true;
            l.Unread = true;

            // 自增主键插入后自动赋值
            AtLocal.Insert(l);

            // 外部可修改 Unread 状态
            NewLetter?.Invoke(l);

            if (l.Unread)
            {
                // 外部未读提示
                StateChanged?.Invoke(l.OtherID);
                ShowUnreadNotify(l);
            }
            else
            {
                // Unread状态被修改
                AtLocal.Save(l);
            }
        }

        static NotifyInfo _notify;

        /// <summary>
        /// 显示未读提示
        /// </summary>
        /// <param name="p_letter"></param>
        static void ShowUnreadNotify(Letter p_letter)
        {
            if (_notify == null)
            {
                _notify = new NotifyInfo();
                // 不自动关闭
                _notify.DelaySeconds = 0;
                _notify.Link = "查看内容";
                _notify.LinkCallback = (e) =>
                {
                    AtKit.CloseNotify(_notify);
                    AtKit.RunAsync(() =>
                    {
                        //Type tp = AtSys.GetViewType(AtKit.ChatView);
                        //if (tp != null)
                        //{
                        //    IView viewer = Activator.CreateInstance(tp) as IView;
                        //    viewer.Run(e.Tag);
                        //}
                    });
                };
            }

            switch (p_letter.LetterType)
            {
                case LetterType.Text:
                    string msg;
                    if (p_letter.Content.Length > 9)
                        msg = p_letter.Content.Substring(0, 9) + "…";
                    else
                        msg = p_letter.Content;
                    _notify.Message = string.Format("💡 {0}\r\n{1}", p_letter.OtherName, msg);
                    break;
                case LetterType.File:
                    _notify.Message = string.Format("🏬 {0}发来文件", p_letter.OtherName);
                    break;
                case LetterType.Image:
                    _notify.Message = string.Format("🌄 {0}发来图片", p_letter.OtherName);
                    break;
                case LetterType.Video:
                    _notify.Message = string.Format("🌉 {0}发来视频", p_letter.OtherName);
                    break;
                case LetterType.Voice:
                    _notify.Message = string.Format("📢 {0}发来语音", p_letter.OtherName);
                    break;
                case LetterType.Link:
                    _notify.Message = string.Format("💨 {0}发来链接", p_letter.OtherName);
                    break;
                default:
                    break;
            }
            _notify.Tag = p_letter;
            if (!SysVisual.NotifyList.Contains(_notify))
                AtKit.Notify(_notify);
        }

        /// <summary>
        /// 清除和某人的未读消息状态
        /// </summary>
        /// <param name="p_otherid">对方标识</param>
        public static void ClearUnreadFlag(long p_otherid)
        {
            AtKit.RunAsync(() =>
            {
                int cnt = AtLocal.Execute("update Letter set unread=0 where otherid=@otherid and loginid=@loginid and unread=1",
                    new Dict
                    {
                        { "otherid", p_otherid },
                        { "loginid", AtUser.ID }
                    });

                if (cnt > 0)
                {
                    StateChanged?.Invoke(p_otherid);
                }
            });
        }
        #endregion

        #region 发送信息
        /// <summary>
        /// 发送聊天信息
        /// </summary>
        /// <param name="p_recvID"></param>
        /// <param name="p_recvName"></param>
        /// <param name="p_content"></param>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public static async Task<Letter> SendLetter(
            long p_recvID,
            string p_recvName,
            string p_content,
            LetterType p_type)
        {
            Check.NotNullOrEmpty(p_content);

            LetterInfo li = new LetterInfo
            {
                ID = AtKit.NewID,
                SenderID = AtUser.ID,
                SenderName = AtUser.Name,
                LetterType = p_type,
                Content = p_content,
                SendTime = AtSys.Now
            };
            bool isOnline = await AtMsg.SendLetter(p_recvID, li);

            // 本地记录
            Letter l = new Letter();
            l.LoginID = AtUser.ID;
            l.MsgID = li.ID;
            l.OtherID = p_recvID;
            l.OtherName = p_recvName;
            l.OtherIsOnline = isOnline;
            l.IsReceived = false;
            l.Unread = false;
            l.LetterType = p_type;
            l.Content = p_content;
            l.STime = li.SendTime;

            // 自增主键插入后自动赋值
            AtLocal.Insert(l);

            NewLetter?.Invoke(l);
            return l;
        }

        /// <summary>
        /// 发送链接
        /// </summary>
        /// <param name="p_recvID"></param>
        /// <param name="p_recvName"></param>
        /// <param name="p_title"></param>
        /// <param name="p_desc"></param>
        /// <param name="p_icon"></param>
        /// <param name="p_action"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public static Task<Letter> SendLink(
            long p_recvID,
            string p_recvName,
            string p_title,
            string p_desc,
            string p_icon,
            Type p_action,
            string p_params)
        {
            string msg;
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, JsonOptions.UnsafeWriter))
                {
                    writer.WriteStartArray();
                    writer.WriteStringValue(p_title);
                    writer.WriteStringValue(p_desc);
                    writer.WriteStringValue(p_icon);
                    writer.WriteStringValue(p_action.AssemblyQualifiedName);
                    writer.WriteStringValue(p_params);
                    writer.WriteEndArray();
                }
                msg = Encoding.UTF8.GetString(stream.ToArray());
            }
            return SendLetter(p_recvID, p_recvName, msg, LetterType.Link);
        }

        /// <summary>
        /// 撤回发出的消息
        /// </summary>
        /// <param name="p_id">待撤消息主键</param>
        /// <returns></returns>
        public static async Task SendUndoLetter(int p_id)
        {
            Letter l = AtLocal.GetFirst<Letter>($"select * from Letter where ID={p_id}");
            if (l == null)
                return;

            LetterInfo li = new LetterInfo
            {
                ID = l.MsgID,
                SenderID = AtUser.ID,
                SenderName = AtUser.Name,
                LetterType = LetterType.Undo,
                SendTime = AtSys.Now
            };
            bool isOnline = await AtMsg.SendLetter(l.OtherID, li);

            l.LetterType = LetterType.Undo;
            AtLocal.Save(l);
        }

        #endregion

    }
}
