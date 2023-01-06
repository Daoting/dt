#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
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
            AddCell("ID", ID);
            AddCell("TrsdID", TrsdID);
            AddCell("SrcAtviID", SrcAtviID);
            AddCell("TgtAtviID", TgtAtviID);
            AddCell("IsRollback", IsRollback);
            AddCell("Ctime", Ctime);
            IsAdded = true;
        }
        #endregion

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

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，不存在时返回null
        /// </summary>
        /// <param name="p_id">主键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<WfiTrsObj> GetByID(object p_id)
        {
            return GetByID<WfiTrsObj>(_svcName, p_id);
        }

        const string _svcName = "cm";
    }
}