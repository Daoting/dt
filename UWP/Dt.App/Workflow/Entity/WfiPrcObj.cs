#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Workflow
{
    #region 自动生成
    [Tbl("cm_wfi_prc")]
    public partial class WfiPrcObj : Entity
    {
        #region 构造方法
        WfiPrcObj() { }

        public WfiPrcObj(
            long ID,
            long PrcdID = default,
            string Name = default,
            WfiAtvStatus Status = default,
            int Dispidx = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell<long>("ID", ID);
            AddCell<long>("PrcdID", PrcdID);
            AddCell<string>("Name", Name);
            AddCell<byte>("Status", (byte)Status);
            AddCell<int>("Dispidx", Dispidx);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<DateTime>("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
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
        public WfiAtvStatus Status
        {
            get { return (WfiAtvStatus)((byte)this["Status"]); }
            set { this["Status"] = (byte)value; }
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
        #endregion
    }
    #endregion
}