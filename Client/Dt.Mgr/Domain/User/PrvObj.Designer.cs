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
    [Tbl("cm_prv")]
    public partial class PrvObj : EntityX<PrvObj>
    {
        #region 构造方法
        PrvObj() { }

        public PrvObj(CellList p_cells) : base(p_cells) { }

        public PrvObj(
            string ID,
            string Note = default)
        {
            AddCell("ID", ID);
            AddCell("Note", Note);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 权限名称
        /// </summary>
        new public string ID
        {
            get { return (string)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Note
        {
            get { return (string)this["Note"]; }
            set { this["Note"] = value; }
        }
    }
}