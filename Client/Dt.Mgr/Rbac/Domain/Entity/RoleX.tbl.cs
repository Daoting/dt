#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    /// <summary>
    /// 角色
    /// </summary>
    [Tbl("CM_ROLE")]
    public partial class RoleX : EntityX<RoleX>
    {
        #region 构造方法
        RoleX() { }

        public RoleX(CellList p_cells) : base(p_cells) { }

        public RoleX(
            long ID,
            string Name = default,
            string Note = default)
        {
            Add("ID", ID);
            Add("NAME", Name);
            Add("NOTE", Note);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Note
        {
            get { return (string)this["NOTE"]; }
            set { this["NOTE"] = value; }
        }

        public Cell cNote => _cells["NOTE"];
    }
}