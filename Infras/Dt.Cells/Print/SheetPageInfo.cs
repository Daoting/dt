#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    internal class SheetPageInfo
    {
        public Size GetPageSize()
        {
            return new Size(ColumnPage.ContentSize + ColumnPage.HeaderSize, RowPage.ContentSize + RowPage.HeaderSize);
        }

        public PageInfo ColumnPage { get; set; }

        public int ColumnPageIndex { get; set; }

        public PageInfo RowPage { get; set; }

        public int RowPageIndex { get; set; }
    }
}

