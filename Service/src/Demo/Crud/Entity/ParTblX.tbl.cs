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
    [Tbl("DEMO_PAR_TBL")]
    public partial class ParTblX : EntityX<ParTblX>
    {
        #region 构造方法
        ParTblX() { }

        public ParTblX(CellList p_cells) : base(p_cells) { }

        public ParTblX(
            long ID,
            string Name = default)
        {
            AddCell("id", ID);
            AddCell("name", Name);
            IsAdded = true;
        }
        #endregion

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