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
            Add("id", ID);
            Add("prcd_id", PrcdID);
            Add("name", Name);
            Add("status", Status);
            Add("dispidx", Dispidx);
            Add("ctime", Ctime);
            Add("mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程模板标识
        /// </summary>
        public long PrcdID
        {
            get { return (long)this["prcd_id"]; }
            set { this["prcd_id"] = value; }
        }

        /// <summary>
        /// 流转单名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 流程实例状态 0活动 1结束 2终止
        /// </summary>
        public WfiPrcStatus Status
        {
            get { return (WfiPrcStatus)this["status"]; }
            set { this["status"] = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["dispidx"]; }
            set { this["dispidx"] = value; }
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