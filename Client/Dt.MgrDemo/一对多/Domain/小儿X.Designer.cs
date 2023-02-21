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
    [Tbl("demo_小儿")]
    public partial class 小儿X : EntityX<小儿X>
    {
        #region 构造方法
        小儿X() { }

        public 小儿X(CellList p_cells) : base(p_cells) { }

        public 小儿X(
            long ID,
            long GroupID = default,
            string ItemName = default)
        {
            AddCell("ID", ID);
            AddCell("GroupID", GroupID);
            AddCell("ItemName", ItemName);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long GroupID
        {
            get { return (long)this["GroupID"]; }
            set { this["GroupID"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ItemName
        {
            get { return (string)this["ItemName"]; }
            set { this["ItemName"] = value; }
        }
    }
}