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
    [Tbl("demo_par_tbl")]
    public partial class ParTblX : EntityX<ParTblX>
    {
        #region 构造方法
        ParTblX() { }

        public ParTblX(CellList p_cells) : base(p_cells) { }

        public ParTblX(
            long ID,
            string Name = default)
        {
            Add("id", ID);
            Add("name", Name);
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

        public Cell cName => _cells["name"];
    }
}