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
    [Tbl("cm_role_prv")]
    public partial class RolePrvObj : EntityX<RolePrvObj>
    {
        #region 构造方法
        RolePrvObj() { }

        public RolePrvObj(CellList p_cells) : base(p_cells) { }

        public RolePrvObj(
            long RoleID,
            string PrvID)
        {
            AddCell("RoleID", RoleID);
            AddCell("PrvID", PrvID);
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
        public string PrvID
        {
            get { return (string)this["PrvID"]; }
            set { this["PrvID"] = value; }
        }

        new public long ID { get { return -1; } }
    }
}