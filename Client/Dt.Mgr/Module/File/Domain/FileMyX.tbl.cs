﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-11 创建
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
            AddCell("id", ID);
            AddCell("parent_id", ParentID);
            AddCell("name", Name);
            AddCell("is_folder", IsFolder);
            AddCell("ext_name", ExtName);
            AddCell("info", Info);
            AddCell("ctime", Ctime);
            AddCell("user_id", UserID);
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

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 是否为文件夹
        /// </summary>
        public bool IsFolder
        {
            get { return (bool)this["is_folder"]; }
            set { this["is_folder"] = value; }
        }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string ExtName
        {
            get { return (string)this["ext_name"]; }
            set { this["ext_name"] = value; }
        }

        /// <summary>
        /// 文件描述信息
        /// </summary>
        public string Info
        {
            get { return (string)this["info"]; }
            set { this["info"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["ctime"]; }
            set { this["ctime"] = value; }
        }

        /// <summary>
        /// 所属用户
        /// </summary>
        public long UserID
        {
            get { return (long)this["user_id"]; }
            set { this["user_id"] = value; }
        }
    }
}