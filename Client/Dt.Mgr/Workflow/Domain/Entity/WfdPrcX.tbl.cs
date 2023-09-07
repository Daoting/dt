#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 流程模板
    /// </summary>
    [Tbl("CM_WFD_PRC")]
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
            Add("ID", ID);
            Add("NAME", Name);
            Add("DIAGRAM", Diagram);
            Add("IS_LOCKED", IsLocked);
            Add("SINGLETON", Singleton);
            Add("NOTE", Note);
            Add("DISPIDX", Dispidx);
            Add("CTIME", Ctime);
            Add("MTIME", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];

        /// <summary>
        /// 流程图
        /// </summary>
        public string Diagram
        {
            get { return (string)this["DIAGRAM"]; }
            set { this["DIAGRAM"] = value; }
        }

        public Cell cDiagram => _cells["DIAGRAM"];

        /// <summary>
        /// 锁定标志，0表未锁定；1表锁定，不能创建流程实例，已启动的流程实例继续执行
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)this["IS_LOCKED"]; }
            set { this["IS_LOCKED"] = value; }
        }

        public Cell cIsLocked => _cells["IS_LOCKED"];

        /// <summary>
        /// 同一时刻只允许有一个激活的流程实例，0表非单实例，1表单实例
        /// </summary>
        public bool Singleton
        {
            get { return (bool)this["SINGLETON"]; }
            set { this["SINGLETON"] = value; }
        }

        public Cell cSingleton => _cells["SINGLETON"];

        /// <summary>
        /// 描述
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