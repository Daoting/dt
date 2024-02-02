#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-14 创建
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
            long FuncID = default,
            string Name = default,
            string Note = default)
        {
            Add("id", ID);
            Add("func_id", FuncID);
            Add("name", Name);
            Add("note", Note);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 所属功能
        /// </summary>
        public long FuncID
        {
            get { return (long)this["func_id"]; }
            set { this["func_id"] = value; }
        }

        public Cell cFuncID => _cells["func_id"];

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public Cell cName => _cells["name"];

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Note
        {
            get { return (string)this["note"]; }
            set { this["note"] = value; }
        }

        public Cell cNote => _cells["note"];
    }
}