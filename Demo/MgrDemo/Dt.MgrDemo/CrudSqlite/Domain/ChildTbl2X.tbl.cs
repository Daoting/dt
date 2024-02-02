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
    public partial class ChildTbl2X : EntityX<ChildTbl2X>
    {
        #region 构造方法
        ChildTbl2X() { }

        public ChildTbl2X(CellList p_cells) : base(p_cells) { }

        public ChildTbl2X(
            long ID,
            long GroupID = default,
            string ItemName = default)
        {
            Add("ID", ID);
            Add("GroupID", GroupID);
            Add("ItemName", ItemName);
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
        /// 
        /// </summary>
        public long GroupID
        {
            get { return (long)this["GroupID"]; }
            set { this["GroupID"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ItemName
        {
            get { return (string)this["ItemName"]; }
            set { this["ItemName"] = value; }
        }
    }
}