#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 活动授权
    /// </summary>
    [Tbl("CM_WFD_ATV_ROLE")]
    public partial class WfdAtvRoleX : EntityX<WfdAtvRoleX>
    {
        #region 构造方法
        WfdAtvRoleX() { }

        public WfdAtvRoleX(CellList p_cells) : base(p_cells) { }

        public WfdAtvRoleX(
            long AtvID,
            long RoleID)
        {
            Add("ATV_ID", AtvID);
            Add("ROLE_ID", RoleID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 活动标识
        /// </summary>
        public long AtvID
        {
            get { return (long)this["ATV_ID"]; }
            set { this["ATV_ID"] = value; }
        }

        public Cell cAtvID => _cells["ATV_ID"];

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["ROLE_ID"]; }
            set { this["ROLE_ID"] = value; }
        }

        public Cell cRoleID => _cells["ROLE_ID"];

        new public long ID { get { return -1; } }
    }
}