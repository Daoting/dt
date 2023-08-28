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
    /// 角色
    /// </summary>
    [Tbl("cm_role")]
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
            Add("id", ID);
            Add("name", Name);
            Add("note", Note);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Note
        {
            get { return (string)this["note"]; }
            set { this["note"] = value; }
        }
    }
}