#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Crud
{
    [Tbl("crud_主表")]
    public partial class 主表X : EntityX<主表X>
    {
        #region 构造方法
        主表X() { }

        public 主表X(CellList p_cells) : base(p_cells) { }

        public 主表X(
            long ID,
            string 主表名称 = default,
            string 限长4 = default,
            string 不重复 = default)
        {
            Add("id", ID);
            Add("主表名称", 主表名称);
            Add("限长4", 限长4);
            Add("不重复", 不重复);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 主表名称
        {
            get { return (string)this["主表名称"]; }
            set { this["主表名称"] = value; }
        }

        public Cell c主表名称 => _cells["主表名称"];

        /// <summary>
        /// 限制最大长度4
        /// </summary>
        public string 限长4
        {
            get { return (string)this["限长4"]; }
            set { this["限长4"] = value; }
        }

        public Cell c限长4 => _cells["限长4"];

        /// <summary>
        /// 列值无重复
        /// </summary>
        public string 不重复
        {
            get { return (string)this["不重复"]; }
            set { this["不重复"] = value; }
        }

        public Cell c不重复 => _cells["不重复"];
    }
}