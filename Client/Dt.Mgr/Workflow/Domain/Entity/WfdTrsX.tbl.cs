#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 迁移模板
    /// </summary>
    [Tbl("cm_wfd_trs")]
    public partial class WfdTrsX : EntityX<WfdTrsX>
    {
        #region 构造方法
        WfdTrsX() { }

        public WfdTrsX(CellList p_cells) : base(p_cells) { }

        public WfdTrsX(
            long ID,
            long PrcID = default,
            long SrcAtvID = default,
            long TgtAtvID = default,
            bool IsRollback = default,
            long? TrsID = default)
        {
            AddCell("id", ID);
            AddCell("prc_id", PrcID);
            AddCell("src_atv_id", SrcAtvID);
            AddCell("tgt_atv_id", TgtAtvID);
            AddCell("is_rollback", IsRollback);
            AddCell("trs_id", TrsID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程模板标识
        /// </summary>
        public long PrcID
        {
            get { return (long)this["prc_id"]; }
            set { this["prc_id"] = value; }
        }

        /// <summary>
        /// 起始活动模板标识
        /// </summary>
        public long SrcAtvID
        {
            get { return (long)this["src_atv_id"]; }
            set { this["src_atv_id"] = value; }
        }

        /// <summary>
        /// 目标活动模板标识
        /// </summary>
        public long TgtAtvID
        {
            get { return (long)this["tgt_atv_id"]; }
            set { this["tgt_atv_id"] = value; }
        }

        /// <summary>
        /// 是否为回退迁移
        /// </summary>
        public bool IsRollback
        {
            get { return (bool)this["is_rollback"]; }
            set { this["is_rollback"] = value; }
        }

        /// <summary>
        /// 类别为回退迁移时对应的常规迁移标识
        /// </summary>
        public long? TrsID
        {
            get { return (long?)this["trs_id"]; }
            set { this["trs_id"] = value; }
        }
    }
}