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

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 打开过的文件历史
    /// </summary>
    [Sqlite("lob")]
    public class ReadFileHistoryX : EntityX<ReadFileHistoryX>
    {
        #region 构造方法
        ReadFileHistoryX() { }

        public ReadFileHistoryX(
            long ID,
            string Info = default,
            DateTime LastReadTime = default)
        {
            Add("ID", ID);
            Add("Info", Info);
            Add("LastReadTime", LastReadTime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 文件标识
        /// </summary>
        [PrimaryKey]
        new public long ID
        {
            get { return (long)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 文件描述
        /// </summary>
        public string Info
        {
            get { return (string)this["Info"]; }
            set { this["Info"] = value; }
        }

        /// <summary>
        /// 最后打开时间
        /// </summary>
        public DateTime LastReadTime
        {
            get { return (DateTime)this["LastReadTime"]; }
            set { this["LastReadTime"] = value; }
        }
    }
}
