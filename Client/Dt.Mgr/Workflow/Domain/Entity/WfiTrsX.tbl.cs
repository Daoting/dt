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
    /// 迁移实例
    /// </summary>
    [Tbl("cm_wfi_trs")]
    public partial class WfiTrsX : EntityX<WfiTrsX>
    {
        #region 构造方法
        WfiTrsX() { }

        public WfiTrsX(CellList p_cells) : base(p_cells) { }

        public WfiTrsX(
            long ID,
            long TrsdID = default,
            long SrcAtviID = default,
            long TgtAtviID = default,
            bool IsRollback = default,
            DateTime Ctime = default)
        {
            Add("id", ID);
            Add("trsd_id", TrsdID);
            Add("src_atvi_id", SrcAtviID);
            Add("tgt_atvi_id", TgtAtviID);
            Add("is_rollback", IsRollback);
            Add("ctime", Ctime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 迁移模板标识
        /// </summary>
        public long TrsdID
        {
            get { return (long)this["trsd_id"]; }
            set { this["trsd_id"] = value; }
        }

        public Cell cTrsdID => _cells["trsd_id"];

        /// <summary>
        /// 起始活动实例标识
        /// </summary>
        public long SrcAtviID
        {
            get { return (long)this["src_atvi_id"]; }
            set { this["src_atvi_id"] = value; }
        }

        public Cell cSrcAtviID => _cells["src_atvi_id"];

        /// <summary>
        /// 目标活动实例标识
        /// </summary>
        public long TgtAtviID
        {
            get { return (long)this["tgt_atvi_id"]; }
            set { this["tgt_atvi_id"] = value; }
        }

        public Cell cTgtAtviID => _cells["tgt_atvi_id"];

        /// <summary>
        /// 是否为回退迁移，1表回退
        /// </summary>
        public bool IsRollback
        {
            get { return (bool)this["is_rollback"]; }
            set { this["is_rollback"] = value; }
        }

        public Cell cIsRollback => _cells["is_rollback"];

        /// <summary>
        /// 迁移时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["ctime"]; }
            set { this["ctime"] = value; }
        }

        public Cell cCtime => _cells["ctime"];
    }
}