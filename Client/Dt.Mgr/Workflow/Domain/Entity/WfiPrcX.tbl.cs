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
    /// 流程实例
    /// </summary>
    [Tbl("CM_WFI_PRC")]
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
            Add("ID", ID);
            Add("PRCD_ID", PrcdID);
            Add("NAME", Name);
            Add("STATUS", Status);
            Add("DISPIDX", Dispidx);
            Add("CTIME", Ctime);
            Add("MTIME", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程模板标识
        /// </summary>
        public long PrcdID
        {
            get { return (long)this["PRCD_ID"]; }
            set { this["PRCD_ID"] = value; }
        }

        public Cell cPrcdID => _cells["PRCD_ID"];

        /// <summary>
        /// 流转单名称
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];

        /// <summary>
        /// 流程实例状态 0活动 1结束 2终止
        /// </summary>
        public WfiPrcStatus Status
        {
            get { return (WfiPrcStatus)this["STATUS"]; }
            set { this["STATUS"] = value; }
        }

        public Cell cStatus => _cells["STATUS"];

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["DISPIDX"]; }
            set { this["DISPIDX"] = value; }
        }

        public Cell cDispidx => _cells["DISPIDX"];

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