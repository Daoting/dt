#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 流程模板
    /// </summary>
    [Tbl("cm_wfd_prc")]
    public partial class WfdPrcX : EntityX<WfdPrcX>
    {
        #region 构造方法
        WfdPrcX() { }

        public WfdPrcX(CellList p_cells) : base(p_cells) { }

        public WfdPrcX(
            long ID,
            string Name = default,
            string Diagram = default,
            bool IsLocked = default,
            bool Singleton = default,
            string Note = default,
            int Dispidx = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            Add("id", ID);
            Add("name", Name);
            Add("diagram", Diagram);
            Add("is_locked", IsLocked);
            Add("singleton", Singleton);
            Add("note", Note);
            Add("dispidx", Dispidx);
            Add("ctime", Ctime);
            Add("mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public Cell cName => _cells["name"];

        /// <summary>
        /// 流程图
        /// </summary>
        public string Diagram
        {
            get { return (string)this["diagram"]; }
            set { this["diagram"] = value; }
        }

        public Cell cDiagram => _cells["diagram"];

        /// <summary>
        /// 锁定标志，0表未锁定；1表锁定，不能创建流程实例，已启动的流程实例继续执行
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)this["is_locked"]; }
            set { this["is_locked"] = value; }
        }

        public Cell cIsLocked => _cells["is_locked"];

        /// <summary>
        /// 同一时刻只允许有一个激活的流程实例，0表非单实例，1表单实例
        /// </summary>
        public bool Singleton
        {
            get { return (bool)this["singleton"]; }
            set { this["singleton"] = value; }
        }

        public Cell cSingleton => _cells["singleton"];

        /// <summary>
        /// 描述
        /// </summary>
        public string Note
        {
            get { return (string)this["note"]; }
            set { this["note"] = value; }
        }

        public Cell cNote => _cells["note"];

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
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["ctime"]; }
            set { this["ctime"] = value; }
        }

        public Cell cCtime => _cells["ctime"];

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["mtime"]; }
            set { this["mtime"] = value; }
        }

        public Cell cMtime => _cells["mtime"];
    }
}