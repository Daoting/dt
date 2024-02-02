#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 个人文件
    /// </summary>
    [Tbl("cm_file_my")]
    public partial class FileMyX : EntityX<FileMyX>
    {
        #region 构造方法
        FileMyX() { }

        public FileMyX(CellList p_cells) : base(p_cells) { }

        public FileMyX(
            long ID,
            long? ParentID = default,
            string Name = default,
            bool IsFolder = default,
            string ExtName = default,
            string Info = default,
            DateTime Ctime = default,
            long UserID = default)
        {
            Add("id", ID);
            Add("parent_id", ParentID);
            Add("name", Name);
            Add("is_folder", IsFolder);
            Add("ext_name", ExtName);
            Add("info", Info);
            Add("ctime", Ctime);
            Add("user_id", UserID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 上级目录，根目录的parendid为空
        /// </summary>
        public long? ParentID
        {
            get { return (long?)this["parent_id"]; }
            set { this["parent_id"] = value; }
        }

        public Cell cParentID => _cells["parent_id"];

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public Cell cName => _cells["name"];

        /// <summary>
        /// 是否为文件夹
        /// </summary>
        public bool IsFolder
        {
            get { return (bool)this["is_folder"]; }
            set { this["is_folder"] = value; }
        }

        public Cell cIsFolder => _cells["is_folder"];

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string ExtName
        {
            get { return (string)this["ext_name"]; }
            set { this["ext_name"] = value; }
        }

        public Cell cExtName => _cells["ext_name"];

        /// <summary>
        /// 文件描述信息
        /// </summary>
        public string Info
        {
            get { return (string)this["info"]; }
            set { this["info"] = value; }
        }

        public Cell cInfo => _cells["info"];

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
        /// 所属用户
        /// </summary>
        public long UserID
        {
            get { return (long)this["user_id"]; }
            set { this["user_id"] = value; }
        }

        public Cell cUserID => _cells["user_id"];
    }
}