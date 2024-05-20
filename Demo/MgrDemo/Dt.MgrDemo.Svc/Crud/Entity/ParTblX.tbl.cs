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
            Add("ID", ID);
            Add("NAME", Name);
            IsAdded = true;
        }
        #endregion

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