#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-14 创建
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
            Add("id", ID);
            Add("prc_id", PrcID);
            Add("src_atv_id", SrcAtvID);
            Add("tgt_atv_id", TgtAtvID);
            Add("is_rollback", IsRollback);
            Add("trs_id", TrsID);
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

        public Cell cPrcID => _cells["prc_id"];

        /// <summary>
        /// 起始活动模板标识
        /// </summary>
        public long SrcAtvID
        {
            get { return (long)this["src_atv_id"]; }
            set { this["src_atv_id"] = value; }
        }

        public Cell cSrcAtvID => _cells["src_atv_id"];

        /// <summary>
        /// 目标活动模板标识
        /// </summary>
        public long TgtAtvID
        {
            get { return (long)this["tgt_atv_id"]; }
            set { this["tgt_atv_id"] = value; }
        }

        public Cell cTgtAtvID => _cells["tgt_atv_id"];

        /// <summary>
        /// 是否为回退迁移
        /// </summary>
        public bool IsRollback
        {
            get { return (bool)this["is_rollback"]; }
            set { this["is_rollback"] = value; }
        }

        public Cell cIsRollback => _cells["is_rollback"];

        /// <summary>
        /// 类别为回退迁移时对应的常规迁移标识
        /// </summary>
        public long? TrsID
        {
            get { return (long?)this["trs_id"]; }
            set { this["trs_id"] = value; }
        }

        public Cell cTrsID => _cells["trs_id"];
    }
}