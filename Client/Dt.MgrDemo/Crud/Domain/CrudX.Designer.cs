#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.Crud
{
    /// <summary>
    /// 基础增删改
    /// </summary>
    [Tbl("demo_crud", "cm")]
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
            AddCell("ID", ID);
            AddCell("Name", Name);
            AddCell("Dispidx", Dispidx);
            AddCell("Mtime", Mtime);
            AddCell("EnableInsertEvent", EnableInsertEvent);
            AddCell("EnableNameChangedEvent", EnableNameChangedEvent);
            AddCell("EnableDelEvent", EnableDelEvent);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["Dispidx"]; }
            set { this["Dispidx"] = value; }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["Mtime"]; }
            set { this["Mtime"] = value; }
        }

        /// <summary>
        /// true时允许发布插入事件
        /// </summary>
        public bool EnableInsertEvent
        {
            get { return (bool)this["EnableInsertEvent"]; }
            set { this["EnableInsertEvent"] = value; }
        }

        /// <summary>
        /// true时允许发布Name变化事件
        /// </summary>
        public bool EnableNameChangedEvent
        {
            get { return (bool)this["EnableNameChangedEvent"]; }
            set { this["EnableNameChangedEvent"] = value; }
        }

        /// <summary>
        /// true时允许发布删除事件
        /// </summary>
        public bool EnableDelEvent
        {
            get { return (bool)this["EnableDelEvent"]; }
            set { this["EnableDelEvent"] = value; }
        }
    }
}