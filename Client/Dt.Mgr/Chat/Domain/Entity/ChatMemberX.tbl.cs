#region 文件描述
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
    /// 聊天人员信息
    /// </summary>
    [Sqlite("lob")]
    public partial class ChatMemberX : EntityX<ChatMemberX>
    {
        #region 构造方法
        ChatMemberX() { }

        public ChatMemberX(
            long ID,
            string Name = default,
            string Phone = default,
            Gender Sex = default,
            string Photo = default,
            DateTime Mtime = default)
        {
            Add("ID", ID);
            Add("Name", Name);
            Add("Phone", Phone);
            Add("Sex", Sex);
            Add("Photo", Photo);
            Add("Mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 用户标识
        /// </summary>
        [PrimaryKey]
        new public long ID
        {
            get { return (long)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone
        {
            get { return (string)this["Phone"]; }
            set { this["Phone"] = value; }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender Sex
        {
            get { return (Gender)this["Sex"]; }
            set { this["Sex"] = value; }
        }

        /// <summary>
        /// 头像
        /// </summary>
        public string Photo
        {
            get { return (string)this["Photo"]; }
            set { this["Photo"] = value; }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["Mtime"]; }
            set { this["Mtime"] = value; }
        }
    }
}
