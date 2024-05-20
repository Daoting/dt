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
            Add("ID", ID);
            Add("PHONE", Phone);
            Add("NAME", Name);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string Phone
        {
            get { return (string)this["PHONE"]; }
            set { this["PHONE"] = value; }
        }

        public Cell cPhone => _cells["PHONE"];

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