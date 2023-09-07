#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    /// <summary>
    /// 系统用户
    /// </summary>
    [Tbl("CM_USER")]
    public partial class UserX : EntityX<UserX>
    {
        #region 构造方法
        UserX() { }

        public UserX(CellList p_cells) : base(p_cells) { }

        public UserX(
            long ID,
            string Phone = default,
            string Name = default,
            string Pwd = default,
            Gender Sex = (Gender)1,
            string Photo = default,
            bool Expired = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            Add("ID", ID);
            Add("PHONE", Phone);
            Add("NAME", Name);
            Add("PWD", Pwd);
            Add("SEX", Sex);
            Add("PHOTO", Photo);
            Add("EXPIRED", Expired);
            Add("CTIME", Ctime);
            Add("MTIME", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone
        {
            get { return (string)this["PHONE"]; }
            set { this["PHONE"] = value; }
        }

        public Cell cPhone => _cells["PHONE"];

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];

        /// <summary>
        /// 密码的md5
        /// </summary>
        public string Pwd
        {
            get { return (string)this["PWD"]; }
            set { this["PWD"] = value; }
        }

        public Cell cPwd => _cells["PWD"];

        /// <summary>
        /// 性别
        /// </summary>
        public Gender Sex
        {
            get { return (Gender)this["SEX"]; }
            set { this["SEX"] = value; }
        }

        public Cell cSex => _cells["SEX"];

        /// <summary>
        /// 头像
        /// </summary>
        public string Photo
        {
            get { return (string)this["PHOTO"]; }
            set { this["PHOTO"] = value; }
        }

        public Cell cPhoto => _cells["PHOTO"];

        /// <summary>
        /// 是否停用
        /// </summary>
        public bool Expired
        {
            get { return (bool)this["EXPIRED"]; }
            set { this["EXPIRED"] = value; }
        }

        public Cell cExpired => _cells["EXPIRED"];

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["CTIME"]; }
            set { this["CTIME"] = value; }
        }

        public Cell cCtime => _cells["CTIME"];

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["MTIME"]; }
            set { this["MTIME"] = value; }
        }

        public Cell cMtime => _cells["MTIME"];
    }
}