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
    [Tbl("cm_wfd_trs")]
    public partial class WfdTrsObj : Entity
    {
        #region 构造方法
        WfdTrsObj() { }

        public WfdTrsObj(
            long ID,
            long PrcID = default,
            long SrcAtvID = default,
            long TgtAtvID = default,
            bool IsRollback = default,
            long? TrsID = default)
        {
            AddCell("ID", ID);
            AddCell("PrcID", PrcID);
            AddCell("SrcAtvID", SrcAtvID);
            AddCell("TgtAtvID", TgtAtvID);
            AddCell("IsRollback", IsRollback);
            AddCell("TrsID", TrsID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程模板标识
        /// </summary>
        public long PrcID
        {
            get { return (long)this["PrcID"]; }
            set { this["PrcID"] = value; }
        }

        /// <summary>
        /// 起始活动模板标识
        /// </summary>
        public long SrcAtvID
        {
            get { return (long)this["SrcAtvID"]; }
            set { this["SrcAtvID"] = value; }
        }

        /// <summary>
        /// 目标活动模板标识
        /// </summary>
        public long TgtAtvID
        {
            get { return (long)this["TgtAtvID"]; }
            set { this["TgtAtvID"] = value; }
        }

        /// <summary>
        /// 是否为回退迁移
        /// </summary>
        public bool IsRollback
        {
            get { return (bool)this["IsRollback"]; }
            set { this["IsRollback"] = value; }
        }

        /// <summary>
        /// 类别为回退迁移时对应的常规迁移标识
        /// </summary>
        public long? TrsID
        {
            get { return (long?)this["TrsID"]; }
            set { this["TrsID"] = value; }
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，不存在时返回null
        /// </summary>
        /// <param name="p_id">主键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<WfdTrsObj> GetByID(object p_id)
        {
            return GetByID<WfdTrsObj>(_svcName, p_id);
        }

        const string _svcName = "cm";
    }
}