#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 用户参数定义
    /// </summary>
    [Tbl("cm_params")]
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
            AddCell("id", ID);
            AddCell("name", Name);
            AddCell("value", Value);
            AddCell("note", Note);
            AddCell("ctime", Ctime);
            AddCell("mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 参数缺省值
        /// </summary>
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        /// <summary>
        /// 参数描述
        /// </summary>
        public string Note
        {
            get { return (string)this["note"]; }
            set { this["note"] = value; }
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
        /// 修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["mtime"]; }
            set { this["mtime"] = value; }
        }
    }
}