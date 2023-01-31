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
    [Tbl("cm_rpt")]
    public partial class RptObj : EntityX<RptObj>
    {
        #region 构造方法
        RptObj() { }

        public RptObj(CellList p_cells) : base(p_cells) { }

        public RptObj(
            long ID,
            string Name = default,
            string Define = default,
            string Note = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell("ID", ID);
            AddCell("Name", Name);
            AddCell("Define", Define);
            AddCell("Note", Note);
            AddCell("Ctime", Ctime);
            AddCell("Mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 报表名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 报表模板定义
        /// </summary>
        public string Define
        {
            get { return (string)this["Define"]; }
            set { this["Define"] = value; }
        }

        /// <summary>
        /// 报表描述
        /// </summary>
        public string Note
        {
            get { return (string)this["Note"]; }
            set { this["Note"] = value; }
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