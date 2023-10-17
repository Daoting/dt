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
    [Tbl("DEMO_VIRTBL3")]
    public partial class Virtbl3X : EntityX<Virtbl3X>
    {
        #region 构造方法
        Virtbl3X() { }

        public Virtbl3X(CellList p_cells) : base(p_cells) { }

        public Virtbl3X(
            long ID,
            string Name3 = default)
        {
            Add("ID", ID);
            Add("NAME3", Name3);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称3
        /// </summary>
        public string Name3
        {
            get { return (string)this["NAME3"]; }
            set { this["NAME3"] = value; }
        }

        public Cell cName3 => _cells["NAME3"];
    }
}