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
    /// 流程实例
    /// </summary>
    [Tbl("cm_wfi_prc")]
    public partial class WfiPrcX : EntityX<WfiPrcX>
    {
        #region 构造方法
        WfiPrcX() { }

        public WfiPrcX(CellList p_cells) : base(p_cells) { }

        public WfiPrcX(
            long ID,
            long PrcdID = default,
            string Name = default,
            WfiPrcStatus Status = default,
            int Dispidx = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell("ID", ID);
            AddCell("PrcdID", PrcdID);
            AddCell("Name", Name);
            AddCell("Status", Status);
            AddCell("Dispidx", Dispidx);
            AddCell("Ctime", Ctime);
            AddCell("Mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程模板标识
        /// </summary>
        public long PrcdID
        {
            get { return (long)this["PrcdID"]; }
            set { this["PrcdID"] = value; }
        }

        /// <summary>
        /// 流转单名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 流程实例状态 0活动 1结束 2终止
        /// </summary>
        public WfiPrcStatus Status
        {
            get { return (WfiPrcStatus)this["Status"]; }
            set { this["Status"] = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["Dispidx"]; }
            set { this["Dispidx"] = value; }
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