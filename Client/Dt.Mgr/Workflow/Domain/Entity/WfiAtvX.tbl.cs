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
    /// 活动实例
    /// </summary>
    [Tbl("cm_wfi_atv")]
    public partial class WfiAtvX : EntityX<WfiAtvX>
    {
        #region 构造方法
        WfiAtvX() { }

        public WfiAtvX(CellList p_cells) : base(p_cells) { }

        public WfiAtvX(
            long ID,
            long PrciID = default,
            long AtvdID = default,
            WfiAtvStatus Status = default,
            int InstCount = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            Add("id", ID);
            Add("prci_id", PrciID);
            Add("atvd_id", AtvdID);
            Add("status", Status);
            Add("inst_count", InstCount);
            Add("ctime", Ctime);
            Add("mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程实例标识
        /// </summary>
        public long PrciID
        {
            get { return (long)this["prci_id"]; }
            set { this["prci_id"] = value; }
        }

        /// <summary>
        /// 活动模板标识
        /// </summary>
        public long AtvdID
        {
            get { return (long)this["atvd_id"]; }
            set { this["atvd_id"] = value; }
        }

        /// <summary>
        /// 活动实例的状态 0活动 1结束 2终止 3同步活动
        /// </summary>
        public WfiAtvStatus Status
        {
            get { return (WfiAtvStatus)this["status"]; }
            set { this["status"] = value; }
        }

        /// <summary>
        /// 活动实例在流程实例被实例化的次数
        /// </summary>
        public int InstCount
        {
            get { return (int)this["inst_count"]; }
            set { this["inst_count"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["ctime"]; }
            set { this["ctime"] = value; }
        }

        /// <summary>
        /// 最后一次状态改变的时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["mtime"]; }
            set { this["mtime"] = value; }
        }
    }
}