#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 基础选项
    /// </summary>
    [Tbl("CM_OPTION")]
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
            Add("ID", ID);
            Add("NAME", Name);
            Add("DISPIDX", Dispidx);
            Add("GROUP_ID", GroupID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 选项名称
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["DISPIDX"]; }
            set { this["DISPIDX"] = value; }
        }

        public Cell cDispidx => _cells["DISPIDX"];

        /// <summary>
        /// 所属分组
        /// </summary>
        public long GroupID
        {
            get { return (long)this["GROUP_ID"]; }
            set { this["GROUP_ID"] = value; }
        }

        public Cell cGroupID => _cells["GROUP_ID"];
    }
}