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
            AddCell("id", ID);
            AddCell("name2", Name2);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称2
        /// </summary>
        public string Name2
        {
            get { return (string)this["name2"]; }
            set { this["name2"] = value; }
        }
    }
}