﻿#region 文件描述
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
    /// 用户参数定义
    /// </summary>
    [Tbl("CM_PARAMS")]
    public partial class ParamsX : EntityX<ParamsX>
    {
        #region 构造方法
        ParamsX() { }

        public ParamsX(CellList p_cells) : base(p_cells) { }

        public ParamsX(
            long ID,
            string Name = default,
            string Value = default,
            string Note = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            Add("ID", ID);
            Add("NAME", Name);
            Add("VALUE", Value);
            Add("NOTE", Note);
            Add("CTIME", Ctime);
            Add("MTIME", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];

        /// <summary>
        /// 参数缺省值
        /// </summary>
        public string Value
        {
            get { return (string)this["VALUE"]; }
            set { this["VALUE"] = value; }
        }

        public Cell cValue => _cells["VALUE"];

        /// <summary>
        /// 参数描述
        /// </summary>
        public string Note
        {
            get { return (string)this["NOTE"]; }
            set { this["NOTE"] = value; }
        }

        public Cell cNote => _cells["NOTE"];

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