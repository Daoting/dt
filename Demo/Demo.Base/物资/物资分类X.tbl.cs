#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-19 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [Tbl("物资分类")]
    public partial class 物资分类X : EntityX<物资分类X>
    {
        #region 构造方法
        物资分类X() { }

        public 物资分类X(CellList p_cells) : base(p_cells) { }

        public 物资分类X(
            long ID,
            string 名称 = default)
        {
            Add("id", ID);
            Add("名称", 名称);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 名称
        {
            get { return (string)this["名称"]; }
            set { this["名称"] = value; }
        }

        public Cell c名称 => _cells["名称"];
    }
}