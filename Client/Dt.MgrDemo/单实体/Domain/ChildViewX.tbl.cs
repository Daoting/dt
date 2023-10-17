#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo
{
    [Tbl("DEMO_CHILD_VIEW")]
    public partial class ChildViewX : EntityX<ChildViewX>
    {
        #region 构造方法
        ChildViewX() { }

        public ChildViewX(CellList p_cells) : base(p_cells) { }

        public ChildViewX(
            long ID,
            long ParentID = default,
            string ItemName = default,
            string Name = default)
        {
            Add("ID", ID);
            Add("PARENT_ID", ParentID);
            Add("ITEM_NAME", ItemName);
            Add("NAME", Name);
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

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];
    }
}