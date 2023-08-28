#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    /// <summary>
    /// 角色一权限多对多
    /// </summary>
    [Tbl("cm_role_per")]
    public partial class RolePerX : EntityX<RolePerX>
    {
        #region 构造方法
        RolePerX() { }

        public RolePerX(CellList p_cells) : base(p_cells) { }

        public RolePerX(
            long RoleID,
            long PerID)
        {
            Add("role_id", RoleID);
            Add("per_id", PerID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["role_id"]; }
            set { this["role_id"] = value; }
        }

        /// <summary>
        /// 权限标识
        /// </summary>
        public long PerID
        {
            get { return (long)this["per_id"]; }
            set { this["per_id"] = value; }
        }

        new public long ID { get { return -1; } }
    }
}