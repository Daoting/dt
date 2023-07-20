#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-19 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.Crud
{
    [Tbl("demo_virtbl1")]
    public partial class Virtbl1X : EntityX<Virtbl1X>
    {
        #region 构造方法
        Virtbl1X() { }

        public Virtbl1X(CellList p_cells) : base(p_cells) { }

        public Virtbl1X(
            long ID,
            string Name1 = default)
        {
            AddCell("id", ID);
            AddCell("name1", Name1);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称1
        /// </summary>
        public string Name1
        {
            get { return (string)this["name1"]; }
            set { this["name1"] = value; }
        }
    }
}