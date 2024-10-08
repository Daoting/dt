﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    /// <summary>
    /// 角色关联的权限
    /// </summary>
    [Tbl("crud_角色权限")]
    public partial class 角色权限X : EntityX<角色权限X>
    {
        #region 构造方法
        角色权限X() { }

        public 角色权限X(CellList p_cells) : base(p_cells) { }

        public 角色权限X(
            long RoleID,
            long PrvID)
        {
            Add("role_id", RoleID);
            Add("prv_id", PrvID);
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

        public Cell cRoleID => _cells["role_id"];

        /// <summary>
        /// 权限标识
        /// </summary>
        public long PrvID
        {
            get { return (long)this["prv_id"]; }
            set { this["prv_id"] = value; }
        }

        public Cell cPrvID => _cells["prv_id"];

        new public long ID { get { return -1; } }
    }
}