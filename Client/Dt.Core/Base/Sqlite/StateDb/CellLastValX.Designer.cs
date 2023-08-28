#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 记录Fv单元格最后编辑的值
    /// </summary>
    [Sqlite("state")]
    public partial class CellLastValX : EntityX<CellLastValX>
    {
        #region 构造方法
        CellLastValX() { }

        public CellLastValX(
            string ID,
            string Val = default)
        {
            Add("ID", ID);
            Add("Val", Val);
            IsAdded = true;
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
