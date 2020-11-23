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
    [Tbl("cm_wfi_atv")]
    public partial class WfiAtv : Entity
    {
        #region 构造方法
        WfiAtv() { }

        public WfiAtv(
            long ID,
            long PrciID = default,
            long AtvdID = default,
            WfiAtvStatus Status = default,
            int InstCount = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell<long>("ID", ID);
            AddCell<long>("PrciID", PrciID);
            AddCell<long>("AtvdID", AtvdID);
            AddCell<byte>("Status", (byte)Status);
            AddCell<int>("InstCount", InstCount);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<DateTime>("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
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
            set { this["Status"] = (byte)value; }
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
        #endregion
    }
    #endregion
}