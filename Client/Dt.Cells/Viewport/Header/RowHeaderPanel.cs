#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 行头视口
    /// </summary>
    internal partial class RowHeaderPanel : HeaderPanel
    {
        public RowHeaderPanel(Excel p_excel)
            : base(p_excel, SheetArea.CornerHeader | SheetArea.RowHeader)
        {
        }
    }
}

