#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-08-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.Crud
{
    [Tbl("demo_child_tbl1")]
    public partial class ChildTbl1X : EntityX<ChildTbl1X>
    {
        #region 构造方法
        ChildTbl1X() { }

        public ChildTbl1X(CellList p_cells) : base(p_cells) { }

        public ChildTbl1X(
            long ID,
            long ParentID = default,
            string ItemName = default)
        {
            Add("id", ID);
            Add("parent_id", ParentID);
            Add("item_name", ItemName);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long ParentID
        {
            get { return (long)this["parent_id"]; }
            set { this["parent_id"] = value; }
        }

        public Cell cParentID => _cells["parent_id"];

        /// <summary>
        /// 
        /// </summary>
        public string ItemName
        {
            get { return (string)this["item_name"]; }
            set { this["item_name"] = value; }
        }

        public Cell cItemName => _cells["item_name"];
    }
}