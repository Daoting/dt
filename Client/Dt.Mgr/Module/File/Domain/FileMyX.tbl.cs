#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 个人文件
    /// </summary>
    [Tbl("CM_FILE_MY")]
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
            Add("ID", ID);
            Add("PARENT_ID", ParentID);
            Add("NAME", Name);
            Add("IS_FOLDER", IsFolder);
            Add("EXT_NAME", ExtName);
            Add("INFO", Info);
            Add("CTIME", Ctime);
            Add("USER_ID", UserID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 上级目录，根目录的parendid为空
        /// </summary>
        public long? ParentID
        {
            get { return (long?)this["PARENT_ID"]; }
            set { this["PARENT_ID"] = value; }
        }

        public Cell cParentID => _cells["PARENT_ID"];

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];

        /// <summary>
        /// 是否为文件夹
        /// </summary>
        public bool IsFolder
        {
            get { return (bool)this["IS_FOLDER"]; }
            set { this["IS_FOLDER"] = value; }
        }

        public Cell cIsFolder => _cells["IS_FOLDER"];

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string ExtName
        {
            get { return (string)this["EXT_NAME"]; }
            set { this["EXT_NAME"] = value; }
        }

        public Cell cExtName => _cells["EXT_NAME"];

        /// <summary>
        /// 文件描述信息
        /// </summary>
        public string Info
        {
            get { return (string)this["INFO"]; }
            set { this["INFO"] = value; }
        }

        public Cell cInfo => _cells["INFO"];

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
        /// 所属用户
        /// </summary>
        public long UserID
        {
            get { return (long)this["USER_ID"]; }
            set { this["USER_ID"] = value; }
        }

        public Cell cUserID => _cells["USER_ID"];
    }
}