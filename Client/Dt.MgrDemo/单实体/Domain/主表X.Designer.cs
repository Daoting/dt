#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.单实体
{
    [Tbl("demo_主表")]
    public partial class 主表X : EntityX<主表X>
    {
        #region 构造方法
        主表X() { }

        public 主表X(CellList p_cells) : base(p_cells) { }

        public 主表X(
            long ID,
            string 主表名称 = default,
            string 限长4 = default,
            string 不重复 = default)
        {
            AddCell("ID", ID);
            AddCell("主表名称", 主表名称);
            AddCell("限长4", 限长4);
            AddCell("不重复", 不重复);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 主表名称
        {
            get { return (string)this["主表名称"]; }
            set { this["主表名称"] = value; }
        }

        /// <summary>
        /// 限制最大长度4
        /// </summary>
        public string 限长4
        {
            get { return (string)this["限长4"]; }
            set { this["限长4"] = value; }
        }

        /// <summary>
        /// 列值无重复
        /// </summary>
        public string 不重复
        {
            get { return (string)this["不重复"]; }
            set { this["不重复"] = value; }
        }
    }
}