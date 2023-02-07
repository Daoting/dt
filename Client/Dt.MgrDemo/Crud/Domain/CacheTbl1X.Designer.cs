#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.Crud
{
    [Tbl("demo_cache_tbl1")]
    public partial class CacheTbl1X : EntityX<CacheTbl1X>
    {
        #region 构造方法
        CacheTbl1X() { }

        public CacheTbl1X(CellList p_cells) : base(p_cells) { }

        public CacheTbl1X(
            long ID,
            string Phone = default,
            string Name = default)
        {
            AddCell("ID", ID);
            AddCell("Phone", Phone);
            AddCell("Name", Name);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string Phone
        {
            get { return (string)this["Phone"]; }
            set { this["Phone"] = value; }
        }

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