#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 迁移实例
    /// </summary>
    [Tbl("CM_WFI_TRS")]
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
            Add("ID", ID);
            Add("TRSD_ID", TrsdID);
            Add("SRC_ATVI_ID", SrcAtviID);
            Add("TGT_ATVI_ID", TgtAtviID);
            Add("IS_ROLLBACK", IsRollback);
            Add("CTIME", Ctime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 迁移模板标识
        /// </summary>
        public long TrsdID
        {
            get { return (long)this["TRSD_ID"]; }
            set { this["TRSD_ID"] = value; }
        }

        public Cell cTrsdID => _cells["TRSD_ID"];

        /// <summary>
        /// 起始活动实例标识
        /// </summary>
        public long SrcAtviID
        {
            get { return (long)this["SRC_ATVI_ID"]; }
            set { this["SRC_ATVI_ID"] = value; }
        }

        public Cell cSrcAtviID => _cells["SRC_ATVI_ID"];

        /// <summary>
        /// 目标活动实例标识
        /// </summary>
        public long TgtAtviID
        {
            get { return (long)this["TGT_ATVI_ID"]; }
            set { this["TGT_ATVI_ID"] = value; }
        }

        public Cell cTgtAtviID => _cells["TGT_ATVI_ID"];

        /// <summary>
        /// 是否为回退迁移，1表回退
        /// </summary>
        public bool IsRollback
        {
            get { return (bool)this["IS_ROLLBACK"]; }
            set { this["IS_ROLLBACK"] = value; }
        }

        public Cell cIsRollback => _cells["IS_ROLLBACK"];

        /// <summary>
        /// 迁移时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["CTIME"]; }
            set { this["CTIME"] = value; }
        }

        public Cell cCtime => _cells["CTIME"];
    }
}