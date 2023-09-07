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
    /// 迁移模板
    /// </summary>
    [Tbl("CM_WFD_TRS")]
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
            Add("ID", ID);
            Add("PRC_ID", PrcID);
            Add("SRC_ATV_ID", SrcAtvID);
            Add("TGT_ATV_ID", TgtAtvID);
            Add("IS_ROLLBACK", IsRollback);
            Add("TRS_ID", TrsID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程模板标识
        /// </summary>
        public long PrcID
        {
            get { return (long)this["PRC_ID"]; }
            set { this["PRC_ID"] = value; }
        }

        public Cell cPrcID => _cells["PRC_ID"];

        /// <summary>
        /// 起始活动模板标识
        /// </summary>
        public long SrcAtvID
        {
            get { return (long)this["SRC_ATV_ID"]; }
            set { this["SRC_ATV_ID"] = value; }
        }

        public Cell cSrcAtvID => _cells["SRC_ATV_ID"];

        /// <summary>
        /// 目标活动模板标识
        /// </summary>
        public long TgtAtvID
        {
            get { return (long)this["TGT_ATV_ID"]; }
            set { this["TGT_ATV_ID"] = value; }
        }

        public Cell cTgtAtvID => _cells["TGT_ATV_ID"];

        /// <summary>
        /// 是否为回退迁移
        /// </summary>
        public bool IsRollback
        {
            get { return (bool)this["IS_ROLLBACK"]; }
            set { this["IS_ROLLBACK"] = value; }
        }

        public Cell cIsRollback => _cells["IS_ROLLBACK"];

        /// <summary>
        /// 类别为回退迁移时对应的常规迁移标识
        /// </summary>
        public long? TrsID
        {
            get { return (long?)this["TRS_ID"]; }
            set { this["TRS_ID"] = value; }
        }

        public Cell cTrsID => _cells["TRS_ID"];
    }
}