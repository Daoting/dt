#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
#endregion

namespace Dt.Cm
{
    [Tbl("cm_user", "cm")]
    public partial class User : Entity
    {
        public User()
        { }

        public User(
            long ID,
            string Phone = default,
            string Name = default,
            string Pwd = default,
            bool Sex = true,
            string Photo = default,
            bool Expired = false,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell<long>("ID", ID);
            AddCell<string>("Phone", Phone);
            AddCell<string>("Name", Name);
            AddCell<string>("Pwd", Pwd);
            AddCell<bool>("Sex", Sex);
            AddCell<string>("Photo", Photo);
            AddCell<bool>("Expired", Expired);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<DateTime>("Mtime", Mtime);
            IsAdded = true;
        }

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone
        {
            get { return (string)_cells["Phone"].Val; }
            private set { _cells["Phone"].Val = value; }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return (string)_cells["Name"].Val; }
            private set { _cells["Name"].Val = value; }
        }

        /// <summary>
        /// 密码的md5
        /// </summary>
        public string Pwd
        {
            get { return (string)_cells["Pwd"].Val; }
            private set { _cells["Pwd"].Val = value; }
        }

        /// <summary>
        /// 性别，0女1男
        /// </summary>
        public bool Sex
        {
            get { return (bool)_cells["Sex"].Val; }
            private set { _cells["Sex"].Val = value; }
        }

        /// <summary>
        /// 头像
        /// </summary>
        public string Photo
        {
            get { return (string)_cells["Photo"].Val; }
            private set { _cells["Photo"].Val = value; }
        }

        /// <summary>
        /// 是否停用
        /// </summary>
        public bool Expired
        {
            get { return (bool)_cells["Expired"].Val; }
            private set { _cells["Expired"].Val = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)_cells["Ctime"].Val; }
            private set { _cells["Ctime"].Val = value; }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)_cells["Mtime"].Val; }
            private set { _cells["Mtime"].Val = value; }
        }
    }
}
