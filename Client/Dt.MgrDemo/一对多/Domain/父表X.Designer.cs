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

namespace Dt.MgrDemo.一对多
{
    [Tbl("demo_父表")]
    public partial class 父表X : EntityX<父表X>
    {
        #region 构造方法
        父表X() { }

        public 父表X(CellList p_cells) : base(p_cells) { }

        public 父表X(
            long ID,
            string Name = default)
        {
            AddCell("ID", ID);
            AddCell("Name", Name);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }
    }
}