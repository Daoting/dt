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
    [Tbl("demo_virtbl3")]
    public partial class Virtbl3X : EntityX<Virtbl3X>
    {
        #region 构造方法
        Virtbl3X() { }

        public Virtbl3X(CellList p_cells) : base(p_cells) { }

        public Virtbl3X(
            long ID,
            string Name3 = default)
        {
            AddCell("ID", ID);
            AddCell("Name3", Name3);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称3
        /// </summary>
        public string Name3
        {
            get { return (string)this["Name3"]; }
            set { this["Name3"] = value; }
        }
    }
}