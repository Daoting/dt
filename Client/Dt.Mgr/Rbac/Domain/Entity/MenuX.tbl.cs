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
    /// 业务菜单
    /// </summary>
    [Tbl("CM_MENU")]
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
            bool IsLocked = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            Add("ID", ID);
            Add("PARENT_ID", ParentID);
            Add("NAME", Name);
            Add("IS_GROUP", IsGroup);
            Add("VIEW_NAME", ViewName);
            Add("PARAMS", Params);
            Add("ICON", Icon);
            Add("NOTE", Note);
            Add("DISPIDX", Dispidx);
            Add("IS_LOCKED", IsLocked);
            Add("CTIME", Ctime);
            Add("MTIME", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 父菜单标识
        /// </summary>
        public long? ParentID
        {
            get { return (long?)this["PARENT_ID"]; }
            set { this["PARENT_ID"] = value; }
        }

        public Cell cParentID => _cells["PARENT_ID"];

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];

        /// <summary>
        /// 分组或实例。0表实例，1表分组
        /// </summary>
        public bool IsGroup
        {
            get { return (bool)this["IS_GROUP"]; }
            set { this["IS_GROUP"] = value; }
        }

        public Cell cIsGroup => _cells["IS_GROUP"];

        /// <summary>
        /// 视图名称
        /// </summary>
        public string ViewName
        {
            get { return (string)this["VIEW_NAME"]; }
            set { this["VIEW_NAME"] = value; }
        }

        public Cell cViewName => _cells["VIEW_NAME"];

        /// <summary>
        /// 传递给菜单程序的参数
        /// </summary>
        public string Params
        {
            get { return (string)this["PARAMS"]; }
            set { this["PARAMS"] = value; }
        }

        public Cell cParams => _cells["PARAMS"];

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return (string)this["ICON"]; }
            set { this["ICON"] = value; }
        }

        public Cell cIcon => _cells["ICON"];

        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get { return (string)this["NOTE"]; }
            set { this["NOTE"] = value; }
        }

        public Cell cNote => _cells["NOTE"];

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["DISPIDX"]; }
            set { this["DISPIDX"] = value; }
        }

        public Cell cDispidx => _cells["DISPIDX"];

        /// <summary>
        /// 定义了菜单是否被锁定。0表未锁定，1表锁定不可用
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)this["IS_LOCKED"]; }
            set { this["IS_LOCKED"] = value; }
        }

        public Cell cIsLocked => _cells["IS_LOCKED"];

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["CTIME"]; }
            set { this["CTIME"] = value; }
        }

        public Cell cCtime => _cells["CTIME"];

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["MTIME"]; }
            set { this["MTIME"] = value; }
        }

        public Cell cMtime => _cells["MTIME"];
    }
}