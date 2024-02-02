﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
#endregion

namespace Dt.Mgr.Chat
{
    /// <summary>
    /// 聊天内容
    /// </summary>
    [Sqlite("lob")]
    public partial class LetterX : EntityX<LetterX>
    {
        #region 构造方法
        LetterX() { }

        public LetterX(
            long ID,
            long LoginID = default,
            string MsgID = default,
            long OtherID = default,
            string OtherName = default,
            bool IsReceived = default,
            bool Unread = default,
            LetterType LetterType = default,
            bool OtherIsOnline = default,
            string Content = default,
            DateTime STime = default,
            string Photo = default)
        {
            Add("ID", ID);
            Add("LoginID", LoginID);
            Add("MsgID", MsgID);
            Add("OtherID", OtherID);
            Add("OtherName", OtherName);
            Add("IsReceived", IsReceived);
            Add("Unread", Unread);
            Add("LetterType", LetterType);
            Add("OtherIsOnline", OtherIsOnline);
            Add("Content", Content);
            Add("STime", STime);
            Add("Photo", Photo);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey]
        new public long ID
        {
            get { return (long)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 当前登录者UserID，区分所属聊天人
        /// </summary>
        public long LoginID
        {
            get { return (long)this["LoginID"]; }
            set { this["LoginID"] = value; }
        }

        /// <summary>
        /// 信息标识，撤回时识别用，不同登录人使用同一设备时可能出现重复(自己发给自己)
        /// </summary>
        public string MsgID
        {
            get { return (string)this["MsgID"]; }
            set { this["MsgID"] = value; }
        }

        /// <summary>
        /// 对方UserID
        /// </summary>
        public long OtherID
        {
            get { return (long)this["OtherID"]; }
            set { this["OtherID"] = value; }
        }

        /// <summary>
        /// 对方用户名
        /// </summary>
        public string OtherName
        {
            get { return (string)this["OtherName"]; }
            set { this["OtherName"] = value; }
        }

        /// <summary>
        /// 接收标志
        /// </summary>
        public bool IsReceived
        {
            get { return (bool)this["IsReceived"]; }
            set { this["IsReceived"] = value; }
        }

        /// <summary>
        /// 未读标志
        /// </summary>
        public bool Unread
        {
            get { return (bool)this["Unread"]; }
            set { this["Unread"] = value; }
        }

        /// <summary>
        /// 内容类型
        /// </summary>
        public LetterType LetterType
        {
            get { return (LetterType)this["LetterType"]; }
            set { this["LetterType"] = value; }
        }

        /// <summary>
        /// 对方接收时是否在线，只发送时有效
        /// </summary>
        public bool OtherIsOnline
        {
            get { return (bool)this["OtherIsOnline"]; }
            set { this["OtherIsOnline"] = value; }
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get { return (string)this["Content"]; }
            set { this["Content"] = value; }
        }

        /// <summary>
        /// 收发时间
        /// </summary>
        public DateTime STime
        {
            get { return (DateTime)this["STime"]; }
            set { this["STime"] = value; }
        }

        /// <summary>
        /// 照片
        /// </summary>
        [Ignore]
        public string Photo
        {
            get
            {
                if (!Contains("Photo"))
                    Add("Photo", "");
                return (string)this["Photo"];
            }
            set
            {
                if (!Contains("Photo"))
                    Add("Photo", "");
                this["Photo"] = value;
            }
        }
    }
}
