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
            AddCell("RoleID", RoleID);
            AddCell("PerID", PerID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["RoleID"]; }
            set { this["RoleID"] = value; }
        }

        /// <summary>
        /// 权限标识
        /// </summary>
        public long PerID
        {
            get { return (long)this["PerID"]; }
            set { this["PerID"] = value; }
        }

        new public long ID { get { return -1; } }
    }
}