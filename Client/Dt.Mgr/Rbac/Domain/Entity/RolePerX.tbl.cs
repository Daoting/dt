#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    /// <summary>
    /// 角色一权限多对多
    /// </summary>
    [Tbl("CM_ROLE_PER")]
    public partial class RolePerX : EntityX<RolePerX>
    {
        #region 构造方法
        RolePerX() { }

        public RolePerX(CellList p_cells) : base(p_cells) { }

        public RolePerX(
            long RoleID,
            long PerID)
        {
            Add("ROLE_ID", RoleID);
            Add("PER_ID", PerID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["ROLE_ID"]; }
            set { this["ROLE_ID"] = value; }
        }

        public Cell cRoleID => _cells["ROLE_ID"];

        /// <summary>
        /// 权限标识
        /// </summary>
        public long PerID
        {
            get { return (long)this["PER_ID"]; }
            set { this["PER_ID"] = value; }
        }

        public Cell cPerID => _cells["PER_ID"];

        new public long ID { get { return -1; } }
    }
}