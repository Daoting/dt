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
    /// 用户一角色多对多
    /// </summary>
    [Tbl("cm_user_role")]
    public partial class UserRoleX : EntityX<UserRoleX>
    {
        #region 构造方法
        UserRoleX() { }

        public UserRoleX(CellList p_cells) : base(p_cells) { }

        public UserRoleX(
            long UserID,
            long RoleID)
        {
            AddCell("UserID", UserID);
            AddCell("RoleID", RoleID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 用户标识
        /// </summary>
        public long UserID
        {
            get { return (long)this["UserID"]; }
            set { this["UserID"] = value; }
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