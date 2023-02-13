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
    [Tbl("demo_virtbl1")]
    public partial class Virtbl1X : EntityX<Virtbl1X>
    {
        #region 构造方法
        Virtbl1X() { }

        public Virtbl1X(CellList p_cells) : base(p_cells) { }

        public Virtbl1X(
            long ID,
            string Name1 = default)
        {
            AddCell("ID", ID);
            AddCell("Name1", Name1);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称1
        /// </summary>
        public string Name1
        {
            get { return (string)this["Name1"]; }
            set { this["Name1"] = value; }
        }
    }
}