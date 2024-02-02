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
    [Tbl("demo_child_tbl2")]
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
            Add("id", ID);
            Add("group_id", GroupID);
            Add("item_name", ItemName);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long GroupID
        {
            get { return (long)this["group_id"]; }
            set { this["group_id"] = value; }
        }

        public Cell cGroupID => _cells["group_id"];

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