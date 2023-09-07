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
    /// 活动实例
    /// </summary>
    [Tbl("CM_WFI_ATV")]
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
            Add("ID", ID);
            Add("PRCI_ID", PrciID);
            Add("ATVD_ID", AtvdID);
            Add("STATUS", Status);
            Add("INST_COUNT", InstCount);
            Add("CTIME", Ctime);
            Add("MTIME", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程实例标识
        /// </summary>
        public long PrciID
        {
            get { return (long)this["PRCI_ID"]; }
            set { this["PRCI_ID"] = value; }
        }

        public Cell cPrciID => _cells["PRCI_ID"];

        /// <summary>
        /// 活动模板标识
        /// </summary>
        public long AtvdID
        {
            get { return (long)this["ATVD_ID"]; }
            set { this["ATVD_ID"] = value; }
        }

        public Cell cAtvdID => _cells["ATVD_ID"];

        /// <summary>
        /// 活动实例的状态 0活动 1结束 2终止 3同步活动
        /// </summary>
        public WfiAtvStatus Status
        {
            get { return (WfiAtvStatus)this["STATUS"]; }
            set { this["STATUS"] = value; }
        }

        public Cell cStatus => _cells["STATUS"];

        /// <summary>
        /// 活动实例在流程实例被实例化的次数
        /// </summary>
        public int InstCount
        {
            get { return (int)this["INST_COUNT"]; }
            set { this["INST_COUNT"] = value; }
        }

        public Cell cInstCount => _cells["INST_COUNT"];

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["CTIME"]; }
            set { this["CTIME"] = value; }
        }

        public Cell cCtime => _cells["CTIME"];

        /// <summary>
        /// 最后一次状态改变的时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["MTIME"]; }
            set { this["MTIME"] = value; }
        }

        public Cell cMtime => _cells["MTIME"];
    }
}