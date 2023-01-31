#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    [Tbl("cm_user")]
    public partial class UserObj : EntityX<UserObj>
    {
        #region 构造方法
        UserObj() { }

        public UserObj(CellList p_cells) : base(p_cells) { }

        public UserObj(
            long ID,
            string Phone = default,
            string Name = default,
            string Pwd = default,
            Gender Sex = (Gender)1,
            string Photo = default,
            bool Expired = false,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell("ID", ID);
            AddCell("Phone", Phone);
            AddCell("Name", Name);
            AddCell("Pwd", Pwd);
            AddCell("Sex", Sex);
            AddCell("Photo", Photo);
            AddCell("Expired", Expired);
            AddCell("Ctime", Ctime);
            AddCell("Mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone
        {
            get { return (string)this["Phone"]; }
            set { this["Phone"] = value; }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 密码的md5
        /// </summary>
        public string Pwd
        {
            get { return (string)this["Pwd"]; }
            set { this["Pwd"] = value; }
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
        /// 是否停用
        /// </summary>
        public bool Expired
        {
            get { return (bool)this["Expired"]; }
            set { this["Expired"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
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