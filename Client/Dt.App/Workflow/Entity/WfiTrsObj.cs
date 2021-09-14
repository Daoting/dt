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
    [Tbl("cm_wfi_trs")]
    public partial class WfiTrsObj : Entity
    {
        #region 构造方法
        WfiTrsObj() { }

        public WfiTrsObj(
            long ID,
            long TrsdID = default,
            long SrcAtviID = default,
            long TgtAtviID = default,
            bool IsRollback = default,
            DateTime Ctime = default)
        {
            AddCell<long>("ID", ID);
            AddCell<long>("TrsdID", TrsdID);
            AddCell<long>("SrcAtviID", SrcAtviID);
            AddCell<long>("TgtAtviID", TgtAtviID);
            AddCell<bool>("IsRollback", IsRollback);
            AddCell<DateTime>("Ctime", Ctime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 迁移模板标识
        /// </summary>
        public long TrsdID
        {
            get { return (long)this["TrsdID"]; }
            set { this["TrsdID"] = value; }
        }

        /// <summary>
        /// 起始活动实例标识
        /// </summary>
        public long SrcAtviID
        {
            get { return (long)this["SrcAtviID"]; }
            set { this["SrcAtviID"] = value; }
        }

        /// <summary>
        /// 目标活动实例标识
        /// </summary>
        public long TgtAtviID
        {
            get { return (long)this["TgtAtviID"]; }
            set { this["TgtAtviID"] = value; }
        }

        /// <summary>
        /// 是否为回退迁移，1表回退
        /// </summary>
        public bool IsRollback
        {
            get { return (bool)this["IsRollback"]; }
            set { this["IsRollback"] = value; }
        }

        /// <summary>
        /// 迁移时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
        }
        #endregion
    }
    #endregion
}