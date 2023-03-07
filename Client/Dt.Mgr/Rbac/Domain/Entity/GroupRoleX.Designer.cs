#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Rbac
{
    /// <summary>
    /// 组一角色多对多
    /// </summary>
    [Tbl("cm_group_role")]
    public partial class GroupRoleX : EntityX<GroupRoleX>
    {
        #region 构造方法
        GroupRoleX() { }

        public GroupRoleX(CellList p_cells) : base(p_cells) { }

        public GroupRoleX(
            long GroupID,
            long RoleID)
        {
            AddCell("GroupID", GroupID);
            AddCell("RoleID", RoleID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 组标识
        /// </summary>
        public long GroupID
        {
            get { return (long)this["GroupID"]; }
            set { this["GroupID"] = value; }
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