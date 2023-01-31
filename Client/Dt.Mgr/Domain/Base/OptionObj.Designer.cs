#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    [Tbl("cm_option")]
    public partial class OptionObj : EntityX<OptionObj>
    {
        #region 构造方法
        OptionObj() { }

        public OptionObj(CellList p_cells) : base(p_cells) { }

        public OptionObj(
            string Name,
            string Category,
            int Dispidx = default)
        {
            AddCell("Name", Name);
            AddCell("Category", Category);
            AddCell("Dispidx", Dispidx);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 选项名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 所属分类
        /// </summary>
        public string Category
        {
            get { return (string)this["Category"]; }
            set { this["Category"] = value; }
        }

        new public long ID { get { return -1; } }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["Dispidx"]; }
            set { this["Dispidx"] = value; }
        }
    }
}