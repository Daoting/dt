#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
using System;
#endregion

namespace Dt.Base.Chat
{
    /// <summary>
    /// 聊天人员信息
    /// </summary>
    [Sqlite("state")]
    public class ChatMember : Entity
    {
        #region 构造方法
        ChatMember() { }

        public ChatMember(
            long ID,
            string Name = default,
            string Phone = default,
            Gender Sex = default,
            string Photo = default,
            DateTime Mtime = default)
        {
            AddCell("ID", ID);
            AddCell("Name", Name);
            AddCell("Phone", Phone);
            AddCell("Sex", Sex);
            AddCell("Photo", Photo);
            AddCell("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
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
