#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [Tbl("crud_小儿")]
    public partial class 小儿X : EntityX<小儿X>
    {
        #region 构造方法
        小儿X() { }

        public 小儿X(CellList p_cells) : base(p_cells) { }

        public 小儿X(
            long ID,
            long GroupID = default,
            string 小儿名 = default)
        {
            Add("id", ID);
            Add("group_id", GroupID);
            Add("小儿名", 小儿名);
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
        public string 小儿名
        {
            get { return (string)this["小儿名"]; }
            set { this["小儿名"] = value; }
        }

        public Cell c小儿名 => _cells["小儿名"];
    }
}