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
    [Tbl("DEMO_VIRTBL1")]
    public partial class Virtbl1X : EntityX<Virtbl1X>
    {
        #region 构造方法
        Virtbl1X() { }

        public Virtbl1X(CellList p_cells) : base(p_cells) { }

        public Virtbl1X(
            long ID,
            string Name1 = default)
        {
            Add("ID", ID);
            Add("NAME1", Name1);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称1
        /// </summary>
        public string Name1
        {
            get { return (string)this["NAME1"]; }
            set { this["NAME1"] = value; }
        }

        public Cell cName1 => _cells["NAME1"];
    }
}