﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Mgr.Chat
{
    /// <summary>
    /// 
    /// </summary>
    class ChatDs : DomainSvc<ChatDs, AtLob>
    {
        ChatDs() { }

        #region 事件
        /// <summary>
        /// 增加一条聊天信息事件
        /// </summary>
        public event Action<LetterObj> NewLetter;

        /// <summary>
        /// 收到撤回消息事件
        /// </summary>
        public event Action<LetterObj> UndoLetter;

        /// <summary>
        /// 未读消息状态变化事件，参数为对方的userid
        /// </summary>
        public event Action<long> StateChanged;
        #endregion

        #region 接收信息
        /// <summary>
        /// 接收服务器推送的聊天信息
        /// </summary>
        /// <param name="p_letter"></param>
        public async void ReceiveLetter(LetterInfo p_letter)
        {
            if (p_letter == null || string.IsNullOrEmpty(p_letter.ID))
                return;

            // 撤回消息
            if (p_letter.LetterType == LetterType.Undo)
            {
                var letter = await LetterObj.First("MsgID=@msgid and LoginID=@loginid and IsReceived=1", new Dict { { "msgid", p_letter.ID }, { "loginid", Kit.UserID } });
                if (letter != null)
                {
                    // 删除
                    await letter.Delete(false);
                    UndoLetter?.Invoke(letter);
                }
                return;
            }

            // 新消息
            var l = new LetterObj(
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
            await l.Save(false);

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
                await l.Save(false);
            }
        }

        /// <summary>
        /// 清除和某人的未读消息状态
        /// </summary>
        /// <param name="p_otherid">对方标识</param>
        public void ClearUnreadFlag(long p_otherid)
        {
            Kit.RunAsync(async () =>
            {
                int cnt = await _ea.Exec("update Letter set unread=0 where otherid=@otherid and loginid=@loginid and unread=1",
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
        public async Task<LetterObj> SendLetter(
            long p_recvID,
            string p_recvName,
            string p_content,
            LetterType p_type)
        {
            Throw.IfEmpty(p_content);

            var li = new LetterInfo
            {
                ID = Kit.NewGuid,
                SenderID = Kit.UserID,
                SenderName = Kit.UserName,
                LetterType = p_type,
                Content = p_content,
                SendTime = Kit.Now
            };
            bool isOnline = await SendLetter(p_recvID, li);

            // 本地记录
            LetterObj l = new LetterObj(
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
            await l.Save(false);

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
        public Task<LetterObj> SendLink(
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
        public async Task<bool> SendUndoLetter(LetterObj p_letter)
        {
            if (p_letter == null)
                return false;

            var li = new LetterInfo
            {
                ID = p_letter.MsgID,
                SenderID = Kit.UserID,
                SenderName = Kit.UserName,
                LetterType = LetterType.Undo,
                SendTime = Kit.Now
            };
            await SendLetter(p_letter.OtherID, li);
            await LetterObj.DelByID(p_letter.ID, true, false);
            return true;
        }

        /// <summary>
        /// 向某用户的客户端推送聊天信息，可通过指定LetterInfo.LetterType为Undo撤回信息
        /// </summary>
        /// <param name="p_userID">目标用户</param>
        /// <param name="p_letter">聊天信息</param>
        /// <returns>true 在线推送</returns>
        public static Task<bool> SendLetter(long p_userID, LetterInfo p_letter)
        {
            return Kit.Rpc<bool>(
                "msg",
                "InstantMsg.SendLetter",
                p_userID,
                p_letter
            );
        }
        #endregion

        #region 刷新好友列表
        const string _refreshKey = "LastRefreshChatMember";

        /// <summary>
        /// 更新好友列表，默认超过10小时需要刷新
        /// </summary>
        /// <returns></returns>
        public async Task Refresh()
        {
            if (!await NeedRefresh())
                return;

            // 暂时取所有，后续增加好友功能
            var tbl = await AtCm.Query<ChatMemberObj>("select id,name,phone,sex,(case photo when '' then 'photo/profilephoto.jpg' else photo end) as photo, mtime from cm_user");

            // 将新列表缓存到本地库
            await _ea.Exec("delete from ChatMember");
            if (tbl != null && tbl.Count > 0)
            {
                foreach (var r in tbl)
                {
                    r.IsAdded = true;
                }
                await tbl.Save(false);
            }

            // 记录刷新时间
            await new ClientCookie(_refreshKey, Kit.Now.ToString()).Save(false);
        }

        async Task<bool> NeedRefresh()
        {
            // 超过10小时需要刷新
            bool refresh = true;
            string val = await ClientCookie.GetCookie(_refreshKey);
            if (!string.IsNullOrEmpty(val) && DateTime.TryParse(val, out var last))
                refresh = (Kit.Now - last).TotalHours >= 10;
            return refresh;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 显示未读提示
        /// </summary>
        /// <param name="p_letter"></param>
        static void ShowUnreadNotify(LetterObj p_letter)
        {
            // 避免过多
            if (Kit.NotifyList.Count > 5)
                return;

            var notify = new NotifyInfo();
            // 不自动关闭
            notify.Delay = 0;
            notify.Link = "查看内容";
            notify.LinkCallback = (e) =>
            {
                LetterObj l = (LetterObj)e.Tag;
                Kit.RunAsync(() => ChatDetail.ShowDlg(l.OtherID, l.OtherName));

                // 关闭所有对方为同一人的提示
                var list = new List<NotifyInfo>();
                foreach (var ni in Kit.NotifyList)
                {
                    if (ni.Tag is LetterObj letter && letter.OtherID == l.OtherID)
                        list.Add(ni);
                }
                if (list.Count > 0)
                    list.ForEach((ni) => Kit.CloseNotify(ni));
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
            Kit.Notify(notify);
        }
        #endregion
    }
}