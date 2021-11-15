#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 记录Fv单元格最后编辑的值
    /// </summary>
    [Sqlite("state")]
    public class CellLastVal : Entity
    {
        #region 构造方法
        CellLastVal() { }

        public CellLastVal(
            string ID,
            string Val = default)
        {
            AddCell("ID", ID);
            AddCell("Val", Val);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        /// <summary>
        /// 单元格唯一标识：BaseUri + Fv.Name + FvCell.ID
        /// </summary>
        [PrimaryKey]
        new public string ID
        {
            get { return (string)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 单元格最后编辑的值
        /// </summary>
        public string Val
        {
            get { return (string)this["Val"]; }
            set { this["Val"] = value; }
        }
    }
}
