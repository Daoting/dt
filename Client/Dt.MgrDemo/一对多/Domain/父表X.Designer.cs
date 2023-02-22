#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.一对多
{
    [Tbl("demo_父表")]
    public partial class 父表X : EntityX<父表X>
    {
        #region 构造方法
        父表X() { }

        public 父表X(CellList p_cells) : base(p_cells) { }

        public 父表X(
            long ID,
            string 父名 = default)
        {
            AddCell("ID", ID);
            AddCell("父名", 父名);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 父名
        {
            get { return (string)this["父名"]; }
            set { this["父名"] = value; }
        }
    }
}