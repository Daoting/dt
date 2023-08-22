#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-08-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.Crud
{
    /// <summary>
    /// 基础增删改
    /// </summary>
    [Tbl("demo_crud", "demo")]
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
            AddCell("id", ID);
            AddCell("name", Name);
            AddCell("dispidx", Dispidx);
            AddCell("mtime", Mtime);
            AddCell("enable_insert_event", EnableInsertEvent);
            AddCell("enable_name_changed_event", EnableNameChangedEvent);
            AddCell("enable_del_event", EnableDelEvent);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public Cell cName => _cells["name"];

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["dispidx"]; }
            set { this["dispidx"] = value; }
        }

        public Cell cDispidx => _cells["dispidx"];

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["mtime"]; }
            set { this["mtime"] = value; }
        }

        public Cell cMtime => _cells["mtime"];

        /// <summary>
        /// true时允许发布插入事件
        /// </summary>
        public bool EnableInsertEvent
        {
            get { return (bool)this["enable_insert_event"]; }
            set { this["enable_insert_event"] = value; }
        }

        public Cell cEnableInsertEvent => _cells["enable_insert_event"];

        /// <summary>
        /// true时允许发布Name变化事件
        /// </summary>
        public bool EnableNameChangedEvent
        {
            get { return (bool)this["enable_name_changed_event"]; }
            set { this["enable_name_changed_event"] = value; }
        }

        public Cell cEnableNameChangedEvent => _cells["enable_name_changed_event"];

        /// <summary>
        /// true时允许发布删除事件
        /// </summary>
        public bool EnableDelEvent
        {
            get { return (bool)this["enable_del_event"]; }
            set { this["enable_del_event"] = value; }
        }

        public Cell cEnableDelEvent => _cells["enable_del_event"];
    }
}