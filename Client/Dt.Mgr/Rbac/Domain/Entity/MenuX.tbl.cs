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
    /// 业务菜单
    /// </summary>
    [Tbl("cm_menu")]
    public partial class MenuX : EntityX<MenuX>
    {
        #region 构造方法
        MenuX() { }

        public MenuX(CellList p_cells) : base(p_cells) { }

        public MenuX(
            long ID,
            long? ParentID = default,
            string Name = default,
            bool IsGroup = default,
            string ViewName = default,
            string Params = default,
            string Icon = default,
            string Note = default,
            int Dispidx = default,
            bool IsLocked = false,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell("id", ID);
            AddCell("parent_id", ParentID);
            AddCell("name", Name);
            AddCell("is_group", IsGroup);
            AddCell("view_name", ViewName);
            AddCell("params", Params);
            AddCell("icon", Icon);
            AddCell("note", Note);
            AddCell("dispidx", Dispidx);
            AddCell("is_locked", IsLocked);
            AddCell("ctime", Ctime);
            AddCell("mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 父菜单标识
        /// </summary>
        public long? ParentID
        {
            get { return (long?)this["parent_id"]; }
            set { this["parent_id"] = value; }
        }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 分组或实例。0表实例，1表分组
        /// </summary>
        public bool IsGroup
        {
            get { return (bool)this["is_group"]; }
            set { this["is_group"] = value; }
        }

        /// <summary>
        /// 视图名称
        /// </summary>
        public string ViewName
        {
            get { return (string)this["view_name"]; }
            set { this["view_name"] = value; }
        }

        /// <summary>
        /// 传递给菜单程序的参数
        /// </summary>
        public string Params
        {
            get { return (string)this["params"]; }
            set { this["params"] = value; }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return (string)this["icon"]; }
            set { this["icon"] = value; }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get { return (string)this["note"]; }
            set { this["note"] = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["dispidx"]; }
            set { this["dispidx"] = value; }
        }

        /// <summary>
        /// 定义了菜单是否被锁定。0表未锁定，1表锁定不可用
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)this["is_locked"]; }
            set { this["is_locked"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["ctime"]; }
            set { this["ctime"] = value; }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["mtime"]; }
            set { this["mtime"] = value; }
        }
    }
}