#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.Crud
{
    [Tbl("demo_crud")]
    public partial class CrudObj : EntityX<CrudObj>
    {
        #region 构造方法
        CrudObj() { }

        public CrudObj(CellList p_cells) : base(p_cells) { }

        public CrudObj(
            long ID,
            string Name = default,
            int Dispidx = default,
            DateTime Mtime = default)
        {
            AddCell("ID", ID);
            AddCell("Name", Name);
            AddCell("Dispidx", Dispidx);
            AddCell("Mtime", Mtime);
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
    }
}