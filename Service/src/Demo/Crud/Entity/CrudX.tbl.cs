#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Crud
{
    /// <summary>
    /// 基础增删改
    /// </summary>
    [Tbl("DEMO_CRUD", "demo")]
    public partial class CrudX : EntityX<CrudX>
    {
        #region 构造方法
        CrudX() { }

        public CrudX(CellList p_cells) : base(p_cells) { }

        public CrudX(
            long ID,
            string Name = default,
            int Dispidx = default,
            DateTime Mtime = default,
            bool EnableInsertEvent = default,
            bool EnableNameChangedEvent = default,
            bool EnableDelEvent = default)
        {
            Add("ID", ID);
            Add("NAME", Name);
            Add("DISPIDX", Dispidx);
            Add("MTIME", Mtime);
            Add("ENABLE_INSERT_EVENT", EnableInsertEvent);
            Add("ENABLE_NAME_CHANGED_EVENT", EnableNameChangedEvent);
            Add("ENABLE_DEL_EVENT", EnableDelEvent);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];

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
        /// 最后修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["MTIME"]; }
            set { this["MTIME"] = value; }
        }

        public Cell cMtime => _cells["MTIME"];

        /// <summary>
        /// true时允许发布插入事件
        /// </summary>
        public bool EnableInsertEvent
        {
            get { return (bool)this["ENABLE_INSERT_EVENT"]; }
            set { this["ENABLE_INSERT_EVENT"] = value; }
        }

        public Cell cEnableInsertEvent => _cells["ENABLE_INSERT_EVENT"];

        /// <summary>
        /// true时允许发布Name变化事件
        /// </summary>
        public bool EnableNameChangedEvent
        {
            get { return (bool)this["ENABLE_NAME_CHANGED_EVENT"]; }
            set { this["ENABLE_NAME_CHANGED_EVENT"] = value; }
        }

        public Cell cEnableNameChangedEvent => _cells["ENABLE_NAME_CHANGED_EVENT"];

        /// <summary>
        /// true时允许发布删除事件
        /// </summary>
        public bool EnableDelEvent
        {
            get { return (bool)this["ENABLE_DEL_EVENT"]; }
            set { this["ENABLE_DEL_EVENT"] = value; }
        }

        public Cell cEnableDelEvent => _cells["ENABLE_DEL_EVENT"];
    }
}