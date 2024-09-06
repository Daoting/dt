#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base.Sqlite
{
    [Sqlite("local")]
    public partial class 扩展1X : EntityX<扩展1X>
    {
        #region 构造方法
        扩展1X() { }

        public 扩展1X(CellList p_cells) : base(p_cells) { }

        public 扩展1X(
            long ID,
            string 扩展1名称 = default,
            bool 禁止选中 = default,
            bool 禁止保存 = default)
        {
            Add("id", ID);
            Add("扩展1名称", 扩展1名称);
            Add("禁止选中", 禁止选中);
            Add("禁止保存", 禁止保存);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 主键标识
        /// </summary>
        [PrimaryKey]
        new public long ID
        {
            get { return (long)this["ID"]; }
            set { this["ID"] = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string 扩展1名称
        {
            get { return (string)this["扩展1名称"]; }
            set { this["扩展1名称"] = value; }
        }

        public Cell c扩展1名称 => _cells["扩展1名称"];

        /// <summary>
        /// 始终为false
        /// </summary>
        public bool 禁止选中
        {
            get { return (bool)this["禁止选中"]; }
            set { this["禁止选中"] = value; }
        }

        public Cell c禁止选中 => _cells["禁止选中"];

        /// <summary>
        /// true时保存前校验不通过
        /// </summary>
        public bool 禁止保存
        {
            get { return (bool)this["禁止保存"]; }
            set { this["禁止保存"] = value; }
        }

        public Cell c禁止保存 => _cells["禁止保存"];
    }
}