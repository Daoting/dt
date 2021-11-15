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

namespace Dt.Base.Chat
{
    /// <summary>
    /// 
    /// </summary>
    public static class LetterManager
    {
        #region 事件
        /// <summary>
        /// 增加一条聊天信息事件
        /// </summary>
        public static event Action<Letter> NewLetter;

        /// <summary>
        /// 收到撤回消息事件
        /// </summary>
        public static event Action<Letter> UndoLetter;

        /// <summary>
        /// 未读消息状态变化事件，参数为对方的userid
        /// </summary>
        public static event Action<long> StateChanged;
        #endregion

        #region 接收信息
        /// <summary>
        /// 接收服务器推送的聊天信息
        /// </summary>
        /// <param name="p_letter"></param>
        internal static async void ReceiveLetter(LetterInfo p_letter)
        {
            if (p_letter == null || string.IsNullOrEmpty(p_letter.ID))
                return;

            // 撤回消息
            if (p_letter.LetterType == LetterType.Undo)
            {
                var letter = AtState.First<Letter>("select * from Letter where MsgID=@msgid and LoginID=@loginid and IsReceived=1", new Dict { { "msgid", p_letter.ID }, { "loginid", Kit.UserID } });
                if (letter != null)
                {
                    // 删除
                    AtState.Exec($"delete from Letter where ID={letter.ID}");
                    UndoLetter?.Invoke(letter);
                }
                return;
            }

            // 新消息
            Letter l = new Letter(
                LoginID: Kit.UserID,
                MsgID: p_letter.ID,
                OtherID: p_letter.SenderID,
                OtherName: p_letter.SenderName,
                LetterType: p_letter.LetterType,
                Content: p_letter.Content,
                STime: p_letter.SendTime,
                IsReceived: true,
                Unread: true);

            // 自增主键插入后自动赋值
            await AtState.Save(l, false);

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
                await AtState.Save(l, false);
            }
        }

        /// <summary>
        /// 显示未读提示
        /// </summary>
        /// <param name="p_letter"></param>
        static void ShowUnreadNotify(Letter p_letter)
        {
            // 避免过多
            if (SysVisual.NotifyList.Count > 5)
                return;

            var notify = new NotifyInfo();
            // 不自动关闭
            notify.DelaySeconds = 0;
            notify.Link = "查看内容";
            notify.LinkCallback = (e) =>
            {
                Letter l = (Letter)e.Tag;
                // 关闭所有对方为同一人的提示
                foreach (var ni in SysVisual.NotifyList)
                {
                    if (ni.Tag is Letter letter && letter.OtherID == l.OtherID)
                        Kit.CloseNotify(ni);
                }
                Kit.RunAsync(() => ChatDetail.ShowDlg(l.OtherID, l.OtherName));
            };

            switch (p_letter.LetterType)
            {
                case LetterType.Text:
                    string msg;
                    if (p_letter.Content.Length > 9)
                        msg = p_letter.Content.Substring(0, 9) + "…";
                    else
                        msg = p_letter.Content;
                    notify.Message = string.Format("💡 {0}\r\n{1}", p_letter.OtherName, msg);
                    break;
                case LetterType.File:
                    notify.Message = string.Format("🏬 {0}发来文件", p_letter.OtherName);
                    break;
                case LetterType.Image:
                    notify.Message = string.Format("🌄 {0}发来图片", p_letter.OtherName);
                    break;
                case LetterType.Video:
                    notify.Message = string.Format("🌉 {0}发来视频", p_letter.OtherName);
                    break;
                case LetterType.Voice:
                    notify.Message = string.Format("📢 {0}发来语音", p_letter.OtherName);
                    break;
                case LetterType.Link:
                    notify.Message = string.Format("💨 {0}发来链接", p_letter.OtherName);
                    break;
                default:
                    break;
            }
            notify.Tag = p_letter;
            SysVisual.NotifyList.Add(notify);
            //await SysVisual.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() => SysVisual.NotifyList.Add(notify)));
        }

        /// <summary>
        /// 清除和某人的未读消息状态
        /// </summary>
        /// <param name="p_otherid">对方标识</param>
        public static void ClearUnreadFlag(long p_otherid)
        {
            Kit.RunAsync(() =>
            {
                int cnt = AtState.Exec("update Letter set unread=0 where otherid=@otherid and loginid=@loginid and unread=1",
                    new Dict
                    {
                        { "otherid", p_otherid },
                        { "loginid", Kit.UserID }
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
            Throw.IfNullOrEmpty(p_content);

            LetterInfo li = new LetterInfo
            {
                ID = Kit.NewGuid,
                SenderID = Kit.UserID,
                SenderName = Kit.UserName,
                LetterType = p_type,
                Content = p_content,
                SendTime = Kit.Now
            };
            bool isOnline = await AtMsg.SendLetter(p_recvID, li);

            // 本地记录
            Letter l = new Letter(
                LoginID: Kit.UserID,
                MsgID: li.ID,
                OtherID: p_recvID,
                OtherName: p_recvName,
                OtherIsOnline: isOnline,
                IsReceived: false,
                Unread: false,
                LetterType: p_type,
                Content: p_content,
                STime: li.SendTime);

            // 自增主键插入后自动赋值
            await AtState.Save(l, false);

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
        /// <param name="p_letter">待撤消息</param>
        /// <returns></returns>
        public static async Task<bool> SendUndoLetter(Letter p_letter)
        {
            if (p_letter == null)
                return false;

            LetterInfo li = new LetterInfo
            {
                ID = p_letter.MsgID,
                SenderID = Kit.UserID,
                SenderName = Kit.UserName,
                LetterType = LetterType.Undo,
                SendTime = Kit.Now
            };
            await AtMsg.SendLetter(p_letter.OtherID, li);
            AtState.Exec($"delete from Letter where ID={p_letter.ID}");
            return true;
        }
        #endregion

    }
}
