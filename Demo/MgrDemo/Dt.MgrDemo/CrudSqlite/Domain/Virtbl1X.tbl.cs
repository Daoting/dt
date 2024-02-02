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
    public partial class Virtbl1X : EntityX<Virtbl1X>
    {
        #region 构造方法
        Virtbl1X() { }

        public Virtbl1X(CellList p_cells) : base(p_cells) { }

        public Virtbl1X(
            long ID,
            string Name1 = default)
        {
            Add("ID", ID);
            Add("Name1", Name1);
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
        /// 名称1
        /// </summary>
        public string Name1
        {
            get { return (string)this["Name1"]; }
            set { this["Name1"] = value; }
        }
    }
}