#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-24 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    /// <summary>
    /// 系统用户
    /// </summary>
    [Tbl("cm_user")]
    public partial class UserX : EntityX<UserX>
    {
        #region 构造方法
        UserX() { }

        public UserX(CellList p_cells) : base(p_cells) { }

        public UserX(
            long ID,
            string Name = default,
            string Phone = default,
            string Pwd = default,
            string Photo = default,
            bool Expired = false,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            Add("id", ID);
            Add("name", Name);
            Add("phone", Phone);
            Add("pwd", Pwd);
            Add("photo", Photo);
            Add("expired", Expired);
            Add("ctime", Ctime);
            Add("mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 账号，唯一
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public Cell cName => _cells["name"];

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone
        {
            get { return (string)this["phone"]; }
            set { this["phone"] = value; }
        }

        public Cell cPhone => _cells["phone"];

        /// <summary>
        /// 密码的md5
        /// </summary>
        public string Pwd
        {
            get { return (string)this["pwd"]; }
            set { this["pwd"] = value; }
        }

        public Cell cPwd => _cells["pwd"];

        /// <summary>
        /// 头像
        /// </summary>
        public string Photo
        {
            get { return (string)this["photo"]; }
            set { this["photo"] = value; }
        }

        public Cell cPhoto => _cells["photo"];

        /// <summary>
        /// 是否停用
        /// </summary>
        public bool Expired
        {
            get { return (bool)this["expired"]; }
            set { this["expired"] = value; }
        }

        public Cell cExpired => _cells["expired"];

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["ctime"]; }
            set { this["ctime"] = value; }
        }

        public Cell cCtime => _cells["ctime"];

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["mtime"]; }
            set { this["mtime"] = value; }
        }

        public Cell cMtime => _cells["mtime"];
    }
}