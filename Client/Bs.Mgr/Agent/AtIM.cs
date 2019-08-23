#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Bs.Mgr
{
    /// <summary>
    /// 实时通信代理类，客户端之间互操作时用
    /// </summary>
    public static class AtIM
    {
        /*
        #region 发送信息
        /// <summary>
        /// 增加一条聊天信息事件
        /// </summary>
        public static event Action<Letter> AddedLetter;

        /// <summary>
        /// 接收到离线信息事件，主要Resume或Toast启动时外部需要处理
        /// </summary>
        public static event Action RecvOffline;

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
            string p_recvID,
            string p_recvName,
            string p_title,
            string p_desc,
            string p_icon,
            Type p_action,
            string p_params)
        {
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, AtKit.WriterSettings))
            {
                writer.WriteStartElement("Lk");
                if (!string.IsNullOrEmpty(p_title))
                    writer.WriteAttributeString("Title", p_title);
                if (!string.IsNullOrEmpty(p_desc))
                    writer.WriteAttributeString("Desc", p_desc);
                if (!string.IsNullOrEmpty(p_icon))
                    writer.WriteAttributeString("Icon", p_icon);

                if (p_action != null)
                    writer.WriteAttributeString("Action", p_action.AssemblyQualifiedName);

                if (!string.IsNullOrEmpty(p_params))
                    writer.WriteAttributeString("Params", p_params);
                writer.WriteEndElement();
                writer.Flush();
            }
            return SendLetter(LetterOtherType.User, p_recvID, p_recvName, sb.ToString(), LetterType.Link);
        }

        /// <summary>
        /// 发送聊天信息，传递顺序：
        /// <para>AtIM.SaveLetter -> 服务端 -> 在线时直接发送，不在线时先保存等待接收者登录后收取</para>
        /// </summary>
        /// <param name="p_otherType"></param>
        /// <param name="p_recvID">批量群发时接收者逗号隔开，*表全部</param>
        /// <param name="p_recvName"></param>
        /// <param name="p_content"></param>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public static async Task<Letter> SendLetter(
            LetterOtherType p_otherType,
            string p_recvID,
            string p_recvName,
            string p_content,
            LetterType p_type)
        {
            if (string.IsNullOrEmpty(p_recvID)
                || string.IsNullOrEmpty(p_recvName)
                || string.IsNullOrEmpty(p_content))
                AtKit.Throw("接收者或内容不可为空！");

            string msgID = AtKit.NewID;
            bool isOnline = false;
            DateTime now = AtSys.Now;
            if (p_otherType == LetterOtherType.User)
            {
                // 一对一
                isOnline = await SaveLetter(p_recvID, msgID, (int)p_type, p_content, now);
            }
            else if (p_otherType == LetterOtherType.Group)
            {
                // 群聊
                Dict dt = await SaveGroupLetter(p_recvID, p_recvName, msgID, (int)p_type, p_content, now);
                if (!(bool)dt["suc"])
                {
                    AtKit.Msg((string)dt["msg"]);
                    return null;
                }
            }
            else
            {
                // 批量群发，接收者逗号隔开，*表全部
                await SaveBatchLetter(p_recvID, msgID, (int)p_type, p_content, now);
            }

            // 本地记录
            Letter pl = new Letter();
            pl.LoginID = AtUser.ID;
            pl.MsgID = msgID;
            pl.OtherType = p_otherType;
            if (p_otherType == LetterOtherType.Batch)
            {
                pl.OtherID = pl.OtherName = "群发";
                pl.OtherUser = p_recvName;
                pl.BatchID = p_recvID;
            }
            else
            {
                pl.OtherID = p_recvID;
                pl.OtherName = p_recvName;
            }
            pl.IsReceived = false;
            pl.Unread = false;
            pl.LetterType = p_type;
            pl.IsOnline = isOnline;
            pl.Content = p_content;
            pl.STime = now;
            // 自增主键插入后自动赋值
            AtLocal.Insert(pl);

            AddedLetter?.Invoke(pl);
            return pl;
        }

        /// <summary>
        /// 撤回发出的消息
        /// </summary>
        /// <param name="p_id">待撤消息主键</param>
        public static async Task UndoLetter(int p_id)
        {
            Letter pl = AtLocal.GetFirst<Letter>($"select * from Letter where ID=\"{p_id}\"");
            // 服务器端撤回消息时两种情况
            if (pl.OtherType == LetterOtherType.User)
                await UndoLetter(pl.MsgID, pl.OtherID);
            else if (pl.OtherType == LetterOtherType.Group)
                await UndoGroupLetter(pl.MsgID, pl.OtherID);
            else
                await UndoBatchLetter(pl.MsgID, pl.BatchID);
            pl.LetterType = LetterType.Undo;
            AtLocal.Save(pl);
        }

        /// <summary>
        /// 本地保存一条信息
        /// </summary>
        /// <param name="p_recvID"></param>
        /// <param name="p_recvName"></param>
        /// <param name="p_isReceived"></param>
        /// <param name="p_unread"></param>
        /// <param name="p_content"></param>
        /// <param name="p_type"></param>
        /// <param name="p_date"></param>
        /// <param name="p_otherType"></param>
        public static void InsertLetter(
            string p_recvID,
            string p_recvName,
            bool p_isReceived,
            bool p_unread,
            string p_content,
            LetterType p_type,
            DateTime p_date,
            LetterOtherType p_otherType = LetterOtherType.User)
        {
            // 本地记录
            Letter pl = new Letter();
            pl.LoginID = AtUser.ID;
            pl.MsgID = AtKit.NewID;
            pl.OtherType = p_otherType;
            pl.OtherID = p_recvID;
            pl.OtherName = p_recvName;
            pl.IsReceived = p_isReceived;
            pl.Unread = p_unread;
            pl.Content = p_content;
            pl.LetterType = p_type;
            pl.STime = p_date;
            pl.IsOnline = true;
            AtLocal.Insert(pl);
        }
        #endregion

        #region 接收信息
        static NotifyInfo _notify;

        /// <summary>
        /// 获取离线聊天信息，登录后自动获取
        /// </summary>
        /// <param name="p_msg">离线信息json，null时重新获取</param>
        public static async Task LoadOfflines(string p_msg = null)
        {
            if (string.IsNullOrEmpty(p_msg))
            {
                p_msg = await GetOfflines();
                if (string.IsNullOrEmpty(p_msg))
                    return;
            }

            Table tbl;
            using (StringReader sr = new StringReader(p_msg))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                try
                {
                    // [
                    reader.Read();
                    tbl = (Table)JsonRpcSerializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    AtKit.Warn("Json解析错误：" + ex.Message);
                    return;
                }
            }
            if (tbl != null && tbl.Count > 0)
            {
                List<Letter> ls = new List<Letter>();
                int unct = 0;
                foreach (var row in tbl)
                {
                    LetterType type = (LetterType)row.Int("type");
                    // 撤回处理
                    if (type == LetterType.Undo)
                    {
                        ReceiveUndoLetter(row.Str("id"));
                        unct++;
                        continue;
                    }

                    string grpID = row.Str("grpid");
                    Letter pl = new Letter();
                    pl.LoginID = AtUser.ID;
                    pl.MsgID = row.Str("id");
                    if (string.IsNullOrEmpty(grpID))
                    {
                        pl.OtherType = LetterOtherType.User;
                        pl.OtherID = row.Str("senderid");
                        pl.OtherName = row.Str("sendername");
                    }
                    else
                    {
                        pl.OtherType = LetterOtherType.Group;
                        pl.OtherID = grpID;
                        pl.OtherName = row.Str("grpname");
                        pl.OtherUser = row.Str("sendername");
                    }
                    pl.IsReceived = true;
                    pl.Unread = true;
                    pl.LetterType = type;
                    pl.IsOnline = false;
                    pl.Content = row.Str("content");
                    pl.STime = row.Date("stime");
                    ls.Add(pl);
                }
                int count = AtLocal.InsertAll(ls);
                // 触发离线接收事件
                RecvOffline?.Invoke();
                // 移除服务器的离线信息
                if ((count + unct) == tbl.Count)
                    await DeleteOfflines();
            }
            OnUnreadChanged(GetUnreadCount());
        }

        /// <summary>
        /// 接收服务器推送的聊天信息
        /// </summary>
        /// <param name="p_id"></param>
        /// <param name="p_grpID"></param>
        /// <param name="p_grpName"></param>
        /// <param name="p_senderID"></param>
        /// <param name="p_senderName"></param>
        /// <param name="p_type"></param>
        /// <param name="p_content"></param>
        /// <param name="p_stime"></param>
        public static void ReceiveLetter(
            string p_id,
            string p_grpID,
            string p_grpName,
            string p_senderID,
            string p_senderName,
            LetterType p_type,
            string p_content,
            DateTime p_stime)
        {
            Letter pl = new Letter();
            pl.LoginID = AtUser.ID;
            pl.MsgID = p_id;
            if (string.IsNullOrEmpty(p_grpID))
            {
                pl.OtherType = LetterOtherType.User;
                pl.OtherID = p_senderID;
                pl.OtherName = p_senderName;
            }
            else
            {
                pl.OtherType = LetterOtherType.Group;
                pl.OtherID = p_grpID;
                pl.OtherName = p_grpName;
                pl.OtherUser = p_senderName;
            }
            pl.IsReceived = true;
            pl.Unread = true;
            pl.LetterType = p_type;
            pl.IsOnline = true;
            pl.Content = p_content;
            pl.STime = p_stime;
            // 自增主键插入后自动赋值
            AtLocal.Insert(pl);

            AddedLetter?.Invoke(pl);

            // 提示
            if (pl.Unread)
            {
                OnUnreadChanged(GetUnreadCount());
                ShowUnreadNotify(pl, p_type, p_senderName, p_content);
            }
            else
            {
                // Unread状态被修改
                AtLocal.Save(pl);
            }
        }

        /// <summary>
        /// 实时撤回消息
        /// </summary>
        /// <param name="p_msgID">聊天信息ID，主要撤回时使用</param>
        public static void ReceiveUndoLetter(string p_msgID)
        {
            var letter = AtLocal.GetFirst<Letter>(
                "select * from Letter where MsgID=:MsgID and LoginID=:LoginID and IsReceived=1",
                new Dict { { "MsgID", p_msgID }, { "LoginID", AtUser.ID } });
            if (letter != null)
            {
                letter.LetterType = LetterType.Undo;
                AtLocal.Save(letter);

                AddedLetter?.Invoke(letter);
            }
        }

        /// <summary>
        /// 清除和某人的未读消息状态
        /// </summary>
        /// <param name="p_otherid">对方</param>
        public static void ClearUnreadFlag(string p_otherid)
        {
            AtKit.RunAsync(() =>
            {
                int cnt = AtLocal.Execute(
                    "update Letter set unread=0 where otherid=:otherid and loginid=:loginid and unread=1",
                    new Dict { { "otherid", p_otherid }, { "loginid", AtUser.ID } });
                if (cnt > 0)
                {
                    cnt = AtLocal.GetScalar<int>(
                        "select count(*) from Letter where loginid=:loginid and unread=1",
                        new Dict { { "loginid", AtUser.ID } });
                    if (cnt == 0)
                        OnUnreadChanged(0);
                }
            });
        }

        /// <summary>
        /// 显示未读提示
        /// </summary>
        /// <param name="p_letter"></param>
        /// <param name="p_type"></param>
        /// <param name="p_sender"></param>
        /// <param name="p_msg"></param>
        internal static void ShowUnreadNotify(object p_letter, LetterType p_type, string p_sender, string p_msg)
        {
            if (_notify == null)
            {
                _notify = new NotifyInfo();
                _notify.AutoClose = false;
                _notify.Link = "查看内容";
                _notify.LinkCallback = (e) =>
                {
                    AtKit.CloseNotify(_notify);
                    AtKit.RunAsync(() =>
                    {
                        Type tp = AtSys.GetViewType(AtKit.ChatView);
                        if (tp != null)
                        {
                            IView viewer = Activator.CreateInstance(tp) as IView;
                            viewer.Run(e.Tag);
                        }
                    });
                };
            }

            switch (p_type)
            {
                case LetterType.Text:
                    string msg;
                    if (p_msg.Length > 9)
                        msg = p_msg.Substring(0, 9) + "…";
                    else
                        msg = p_msg;
                    _notify.Message = string.Format("💡 {0}\r\n{1}", p_sender, msg);
                    break;
                case LetterType.File:
                    _notify.Message = string.Format("🏬 {0}发来文件", p_sender);
                    break;
                case LetterType.Image:
                    _notify.Message = string.Format("🌄 {0}发来图片", p_sender);
                    break;
                case LetterType.Video:
                    _notify.Message = string.Format("🌉 {0}发来视频", p_sender);
                    break;
                case LetterType.Voice:
                    _notify.Message = string.Format("📢 {0}发来语音", p_sender);
                    break;
                case LetterType.Link:
                    _notify.Message = string.Format("💨 {0}发来链接", p_sender);
                    break;
                default:
                    break;
            }
            _notify.Tag = p_letter;
            if (!SysVisual.NotifyList.Contains(_notify))
                AtKit.Notify(_notify);
        }

        /// <summary>
        /// 触发未读消息状态变化事件
        /// </summary>
        /// <param name="p_existUnread"></param>
        internal static void OnUnreadChanged(int p_cnt)
        {
            var menus = AtSys.Stub.FixedMenus;
            if (menus != null && menus.Count > 0)
            {
                var chatItem = (from om in menus
                                where om.ViewName == AtKit.ChatView
                                select om).FirstOrDefault();
                if (chatItem != null)
                    chatItem.SetWarningNum(p_cnt);
            }
        }

        /// <summary>
        /// 获取未读消息个数
        /// </summary>
        /// <returns></returns>
        static int GetUnreadCount()
        {
            return AtLocal.GetScalar<int>("select count(*) from Letter where loginid=:loginid and unread=1", new Dict { { "loginid", AtUser.ID } });
        }
        #endregion
        */
    }
}
