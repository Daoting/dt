#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 基础选项
    /// </summary>
    [Tbl("cm_option")]
    public partial class OptionX : EntityX<OptionX>
    {
        #region 构造方法
        OptionX() { }

        public OptionX(CellList p_cells) : base(p_cells) { }

        public OptionX(
            long ID,
            string Name = default,
            int Dispidx = default,
            long GroupID = default)
        {
            AddCell("ID", ID);
            AddCell("Name", Name);
            AddCell("Dispidx", Dispidx);
            AddCell("GroupID", GroupID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 选项名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["Dispidx"]; }
            set { this["Dispidx"] = value; }
        }

        /// <summary>
        /// 所属分组
        /// </summary>
        public long GroupID
        {
            get { return (long)this["GroupID"]; }
            set { this["GroupID"] = value; }
        }
    }
}