#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-26 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    [Tbl("cm_permission_func")]
    public partial class PermissionFuncX : EntityX<PermissionFuncX>
    {
        #region 构造方法
        PermissionFuncX() { }

        public PermissionFuncX(CellList p_cells) : base(p_cells) { }

        public PermissionFuncX(
            long ID,
            long ModuleID = default,
            string Name = default,
            string Note = default)
        {
            Add("id", ID);
            Add("module_id", ModuleID);
            Add("name", Name);
            Add("note", Note);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 所属模块
        /// </summary>
        public long ModuleID
        {
            get { return (long)this["module_id"]; }
            set { this["module_id"] = value; }
        }

        public Cell cModuleID => _cells["module_id"];

        /// <summary>
        /// 功能名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public Cell cName => _cells["name"];

        /// <summary>
        /// 功能描述
        /// </summary>
        public string Note
        {
            get { return (string)this["note"]; }
            set { this["note"] = value; }
        }

        public Cell cNote => _cells["note"];
    }
}