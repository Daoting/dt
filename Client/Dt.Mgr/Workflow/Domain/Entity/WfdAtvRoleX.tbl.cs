#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 活动授权
    /// </summary>
    [Tbl("cm_wfd_atv_role")]
    public partial class WfdAtvRoleX : EntityX<WfdAtvRoleX>
    {
        #region 构造方法
        WfdAtvRoleX() { }

        public WfdAtvRoleX(CellList p_cells) : base(p_cells) { }

        public WfdAtvRoleX(
            long AtvID,
            long RoleID)
        {
            Add("atv_id", AtvID);
            Add("role_id", RoleID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 活动标识
        /// </summary>
        public long AtvID
        {
            get { return (long)this["atv_id"]; }
            set { this["atv_id"] = value; }
        }

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["role_id"]; }
            set { this["role_id"] = value; }
        }

        new public long ID { get { return -1; } }
    }
}