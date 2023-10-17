#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo
{
    /// <summary>
    /// 角色关联的权限
    /// </summary>
    [Tbl("DEMO_角色权限")]
    public partial class 角色权限X : EntityX<角色权限X>
    {
        #region 构造方法
        角色权限X() { }

        public 角色权限X(CellList p_cells) : base(p_cells) { }

        public 角色权限X(
            long RoleID,
            long PrvID)
        {
            Add("ROLE_ID", RoleID);
            Add("PRV_ID", PrvID);
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
        public long PrvID
        {
            get { return (long)this["PRV_ID"]; }
            set { this["PRV_ID"] = value; }
        }

        public Cell cPrvID => _cells["PRV_ID"];

        new public long ID { get { return -1; } }
    }
}