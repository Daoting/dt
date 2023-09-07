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
    /// 基础选项分组
    /// </summary>
    [Tbl("CM_OPTION_GROUP")]
    public partial class OptionGroupX : EntityX<OptionGroupX>
    {
        #region 构造方法
        OptionGroupX() { }

        public OptionGroupX(CellList p_cells) : base(p_cells) { }

        public OptionGroupX(
            long ID,
            string Name = default)
        {
            Add("ID", ID);
            Add("NAME", Name);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];
    }
}