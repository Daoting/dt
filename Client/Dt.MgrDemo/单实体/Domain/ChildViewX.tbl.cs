#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-27 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.单实体
{
    /// <summary>
    /// VIEW
    /// </summary>
    [Tbl("demo_child_view")]
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
            AddCell("id", ID);
            AddCell("parent_id", ParentID);
            AddCell("item_name", ItemName);
            AddCell("name", Name);
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

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
    }
}