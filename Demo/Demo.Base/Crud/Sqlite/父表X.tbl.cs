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
    public partial class 父表X : EntityX<父表X>
    {
        #region 构造方法
        父表X() { }

        public 父表X(CellList p_cells) : base(p_cells) { }

        public 父表X(
            long ID,
            string 父名 = default)
        {
            Add("id", ID);
            Add("父名", 父名);
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
        public string 父名
        {
            get { return (string)this["父名"]; }
            set { this["父名"] = value; }
        }

        public Cell c父名 => _cells["父名"];
    }
}