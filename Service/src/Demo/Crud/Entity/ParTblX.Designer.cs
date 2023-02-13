#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Demo.Crud
{
    [Tbl("demo_par_tbl")]
    public partial class ParTblX : EntityX<ParTblX>
    {
        #region 构造方法
        ParTblX() { }

        public ParTblX(CellList p_cells) : base(p_cells) { }

        public ParTblX(
            long ID,
            string Name = default)
        {
            AddCell("ID", ID);
            AddCell("Name", Name);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }
    }
}