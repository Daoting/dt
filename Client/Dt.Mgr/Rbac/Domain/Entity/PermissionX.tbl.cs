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
            AddCell("id", ID);
            AddCell("name", Name);
            AddCell("note", Note);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Note
        {
            get { return (string)this["note"]; }
            set { this["note"] = value; }
        }
    }
}