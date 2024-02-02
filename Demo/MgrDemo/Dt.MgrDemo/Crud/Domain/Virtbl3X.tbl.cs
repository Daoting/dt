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
    [Tbl("demo_virtbl3")]
    public partial class Virtbl3X : EntityX<Virtbl3X>
    {
        #region 构造方法
        Virtbl3X() { }

        public Virtbl3X(CellList p_cells) : base(p_cells) { }

        public Virtbl3X(
            long ID,
            string Name3 = default)
        {
            Add("id", ID);
            Add("name3", Name3);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称3
        /// </summary>
        public string Name3
        {
            get { return (string)this["name3"]; }
            set { this["name3"] = value; }
        }

        public Cell cName3 => _cells["name3"];
    }
}