#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Crud
{
    [Tbl("DEMO_CHILD_TBL1")]
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
            Add("ID", ID);
            Add("PARENT_ID", ParentID);
            Add("ITEM_NAME", ItemName);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long ParentID
        {
            get { return (long)this["PARENT_ID"]; }
            set { this["PARENT_ID"] = value; }
        }

        public Cell cParentID => _cells["PARENT_ID"];

        /// <summary>
        /// 
        /// </summary>
        public string ItemName
        {
            get { return (string)this["ITEM_NAME"]; }
            set { this["ITEM_NAME"] = value; }
        }

        public Cell cItemName => _cells["ITEM_NAME"];
    }
}