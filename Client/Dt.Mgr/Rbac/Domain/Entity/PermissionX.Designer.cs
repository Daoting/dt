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
    /// 权限
    /// </summary>
    [Tbl("cm_permission")]
    public partial class PermissionX : EntityX<PermissionX>
    {
        #region 构造方法
        PermissionX() { }

        public PermissionX(CellList p_cells) : base(p_cells) { }

        public PermissionX(
            long ID,
            string Name = default,
            string Note = default)
        {
            AddCell("ID", ID);
            AddCell("Name", Name);
            AddCell("Note", Note);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Note
        {
            get { return (string)this["Note"]; }
            set { this["Note"] = value; }
        }
    }
}