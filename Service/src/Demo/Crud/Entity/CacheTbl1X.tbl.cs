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
    [Tbl("DEMO_CACHE_TBL1")]
    public partial class CacheTbl1X : EntityX<CacheTbl1X>
    {
        #region 构造方法
        CacheTbl1X() { }

        public CacheTbl1X(CellList p_cells) : base(p_cells) { }

        public CacheTbl1X(
            long ID,
            string Phone = default,
            string Name = default)
        {
            AddCell("id", ID);
            AddCell("phone", Phone);
            AddCell("name", Name);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string Phone
        {
            get { return (string)this["phone"]; }
            set { this["phone"] = value; }
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