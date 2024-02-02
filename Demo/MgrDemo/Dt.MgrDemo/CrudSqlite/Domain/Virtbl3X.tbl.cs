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

namespace Dt.MgrDemo.CrudSqlite
{
    [Sqlite("local")]
    public partial class Virtbl3X : EntityX<Virtbl3X>
    {
        #region 构造方法
        Virtbl3X() { }

        public Virtbl3X(CellList p_cells) : base(p_cells) { }

        public Virtbl3X(
            long ID,
            string Name3 = default)
        {
            Add("ID", ID);
            Add("Name3", Name3);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 主键标识
        /// </summary>
        [PrimaryKey]
        new public long ID
        {
            get { return (long)this["ID"]; }
            set { this["ID"] = value; }
        }

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