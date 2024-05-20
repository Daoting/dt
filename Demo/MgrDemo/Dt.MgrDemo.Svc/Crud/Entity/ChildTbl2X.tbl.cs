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
    [Tbl("DEMO_CHILD_TBL2")]
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
            Add("GROUP_ID", GroupID);
            Add("ITEM_NAME", ItemName);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long GroupID
        {
            get { return (long)this["GROUP_ID"]; }
            set { this["GROUP_ID"] = value; }
        }

        public Cell cGroupID => _cells["GROUP_ID"];

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