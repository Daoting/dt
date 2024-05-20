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
    [Tbl("DEMO_VIRTBL2")]
    public partial class Virtbl2X : EntityX<Virtbl2X>
    {
        #region 构造方法
        Virtbl2X() { }

        public Virtbl2X(CellList p_cells) : base(p_cells) { }

        public Virtbl2X(
            long ID,
            string Name2 = default)
        {
            Add("ID", ID);
            Add("NAME2", Name2);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称2
        /// </summary>
        public string Name2
        {
            get { return (string)this["NAME2"]; }
            set { this["NAME2"] = value; }
        }

        public Cell cName2 => _cells["NAME2"];
    }
}