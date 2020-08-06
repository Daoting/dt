#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    internal partial class GcHeaderCornerViewport : GcViewport
    {
        RowPresenter _cornerRow;

        public GcHeaderCornerViewport(SheetView sheet) : base(sheet, SheetArea.CornerHeader)
        {
            _cornerRow = new HeaderCornerRowPresenter(this);
            Children.Add(_cornerRow);
            HorizontalAlignment = HorizontalAlignment.Right;
            VerticalAlignment = VerticalAlignment.Bottom;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _cornerRow.Measure(availableSize);
            return GetViewportSize(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            GetRowLayoutModel();
            double y = Location.Y;
            double x = Location.X;
            double width = finalSize.Width;
            double height = finalSize.Height;
            _cornerRow.Arrange(new Rect(PointToClient(new Point(x, y)), new Size(width, height)));
            return finalSize;
        }

        internal override RowPresenter GetRow(int row)
        {
            return _cornerRow;
        }

        internal override SheetSpanModelBase GetSpanModel()
        {
            return null;
        }

        internal override Size GetViewportSize(Size availableSize)
        {
            double headerWidth = Sheet.GetSheetLayout().HeaderWidth;
            double headerHeight = Sheet.GetSheetLayout().HeaderHeight;
            headerWidth = Math.Min(headerWidth, availableSize.Width);
            return new Size(headerWidth, Math.Min(headerHeight, availableSize.Height));
        }

        internal override bool SupportCellOverflow
        {
            get { return  false; }
        }
    }
}

