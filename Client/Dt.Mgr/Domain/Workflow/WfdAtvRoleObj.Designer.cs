#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    [Tbl("cm_wfd_atv_role")]
    public partial class WfdAtvRoleObj : EntityX<WfdAtvRoleObj>
    {
        #region 构造方法
        WfdAtvRoleObj() { }

        public WfdAtvRoleObj(CellList p_cells) : base(p_cells) { }

        public WfdAtvRoleObj(
            long AtvID,
            long RoleID)
        {
            AddCell("AtvID", AtvID);
            AddCell("RoleID", RoleID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 活动标识
        /// </summary>
        public long AtvID
        {
            get { return (long)this["AtvID"]; }
            set { this["AtvID"] = value; }
        }

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["RoleID"]; }
            set { this["RoleID"] = value; }
        }

        new public long ID { get { return -1; } }
    }
}