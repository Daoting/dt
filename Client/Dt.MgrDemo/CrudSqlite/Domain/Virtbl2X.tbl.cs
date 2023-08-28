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
    public partial class Virtbl2X : EntityX<Virtbl2X>
    {
        #region 构造方法
        Virtbl2X() { }

        public Virtbl2X(CellList p_cells) : base(p_cells) { }

        public Virtbl2X(
            long ID,
            string Name2 = default)
        {
            Add("ID", ID);
            Add("Name2", Name2);
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
        /// 名称2
        /// </summary>
        public string Name2
        {
            get { return (string)this["Name2"]; }
            set { this["Name2"] = value; }
        }
    }
}