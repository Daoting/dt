#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.一对多
{
    [Tbl("demo_大儿")]
    public partial class 大儿X : EntityX<大儿X>
    {
        #region 构造方法
        大儿X() { }

        public 大儿X(CellList p_cells) : base(p_cells) { }

        public 大儿X(
            long ID,
            long ParentID = default,
            string 大儿名 = default)
        {
            AddCell("ID", ID);
            AddCell("ParentID", ParentID);
            AddCell("大儿名", 大儿名);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long ParentID
        {
            get { return (long)this["ParentID"]; }
            set { this["ParentID"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string 大儿名
        {
            get { return (string)this["大儿名"]; }
            set { this["大儿名"] = value; }
        }
    }
}