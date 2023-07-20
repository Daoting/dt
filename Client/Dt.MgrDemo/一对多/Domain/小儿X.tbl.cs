#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.一对多
{
    [Tbl("demo_小儿")]
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
            AddCell("id", ID);
            AddCell("group_id", GroupID);
            AddCell("小儿名", 小儿名);
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

        /// <summary>
        /// 
        /// </summary>
        public string 小儿名
        {
            get { return (string)this["小儿名"]; }
            set { this["小儿名"] = value; }
        }
    }
}