﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
#endregion

namespace Dt.App.File
{
    #region 自动生成
    [Tbl("cm_myfile", "cm")]
    public partial class Myfile : Entity
    {
        #region 构造方法
        Myfile() { }

        public Myfile(
            long ID,
            long? ParentID = default,
            string Name = default,
            bool IsFolder = default,
            string Info = default,
            DateTime Ctime = default,
            long UserID = default)
        {
            AddCell<long>("ID", ID);
            AddCell<long?>("ParentID", ParentID);
            AddCell<string>("Name", Name);
            AddCell<bool>("IsFolder", IsFolder);
            AddCell<string>("Info", Info);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<long>("UserID", UserID);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 上级目录，根目录的parendid为空
        /// </summary>
        public long? ParentID
        {
            get { return (long?)this["ParentID"]; }
            set { this["ParentID"] = value; }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 是否为文件夹
        /// </summary>
        public bool IsFolder
        {
            get { return (bool)this["IsFolder"]; }
            set { this["IsFolder"] = value; }
        }

        /// <summary>
        /// 文件描述信息
        /// </summary>
        public string Info
        {
            get { return (string)this["Info"]; }
            set { this["Info"] = value; }
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
        /// 所属用户
        /// </summary>
        public long UserID
        {
            get { return (long)this["UserID"]; }
            set { this["UserID"] = value; }
        }
        #endregion

        #region 可复制
        /*
        void OnSaving()
        {
        }

        void OnDeleting()
        {
        }

        void SetID(long p_value)
        {
        }

        void SetParentID(long? p_value)
        {
        }

        void SetName(string p_value)
        {
        }

        void SetIsFolder(bool p_value)
        {
        }

        void SetInfo(string p_value)
        {
        }

        void SetCtime(DateTime p_value)
        {
        }

        void SetUserID(long p_value)
        {
        }
        */
        #endregion
    }
    #endregion
}
