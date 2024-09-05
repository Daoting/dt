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
    /// 报表模板定义
    /// </summary>
    [Tbl("cm_rpt")]
    public partial class RptX : EntityX<RptX>
    {
        #region 构造方法
        RptX() { }

        public RptX(CellList p_cells) : base(p_cells) { }

        public RptX(
            long ID,
            string Name = default,
            string Define = default,
            string Note = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            Add("id", ID);
            Add("name", Name);
            Add("define", Define);
            Add("note", Note);
            Add("ctime", Ctime);
            Add("mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 报表名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public Cell cName => _cells["name"];

        /// <summary>
        /// 报表模板定义
        /// </summary>
        public string Define
        {
            get { return (string)this["define"]; }
            set { this["define"] = value; }
        }

        public Cell cDefine => _cells["define"];

        /// <summary>
        /// 报表描述
        /// </summary>
        public string Note
        {
            get { return (string)this["note"]; }
            set { this["note"] = value; }
        }

        public Cell cNote => _cells["note"];

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