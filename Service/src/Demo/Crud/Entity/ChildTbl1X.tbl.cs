#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-19 创建
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

        /// <summary>
        /// 
        /// </summary>
        public string ItemName
        {
            get { return (string)this["item_name"]; }
            set { this["item_name"] = value; }
        }
    }
}