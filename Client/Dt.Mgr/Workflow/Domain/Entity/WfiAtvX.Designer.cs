#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-07 创建
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
            AddCell("ID", ID);
            AddCell("PrciID", PrciID);
            AddCell("AtvdID", AtvdID);
            AddCell("Status", Status);
            AddCell("InstCount", InstCount);
            AddCell("Ctime", Ctime);
            AddCell("Mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程实例标识
        /// </summary>
        public long PrciID
        {
            get { return (long)this["PrciID"]; }
            set { this["PrciID"] = value; }
        }

        /// <summary>
        /// 活动模板标识
        /// </summary>
        public long AtvdID
        {
            get { return (long)this["AtvdID"]; }
            set { this["AtvdID"] = value; }
        }

        /// <summary>
        /// 活动实例的状态 0活动 1结束 2终止 3同步活动
        /// </summary>
        public WfiAtvStatus Status
        {
            get { return (WfiAtvStatus)this["Status"]; }
            set { this["Status"] = value; }
        }

        /// <summary>
        /// 活动实例在流程实例被实例化的次数
        /// </summary>
        public int InstCount
        {
            get { return (int)this["InstCount"]; }
            set { this["InstCount"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
        }

        /// <summary>
        /// 最后一次状态改变的时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["Mtime"]; }
            set { this["Mtime"] = value; }
        }
    }
}