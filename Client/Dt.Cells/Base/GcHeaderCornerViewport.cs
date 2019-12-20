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
        private RowPresenter _cornerRow;

        public GcHeaderCornerViewport(SheetView sheet) : base(sheet, SheetArea.CornerHeader, false)
        {
            base._sheetArea = SheetArea.CornerHeader;
            this._cornerRow = new HeaderCornerRowPresenter(this);
            base.Children.Clear();
            base.Children.Add(this._cornerRow);
            base.HorizontalAlignment = HorizontalAlignment.Right;
            base.VerticalAlignment = VerticalAlignment.Bottom;
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            this.GetRowLayoutModel();
            double y = base.Location.Y;
            double x = base.Location.X;
            double width = finalSize.Width;
            double height = finalSize.Height;
            this._cornerRow.Arrange(new Windows.Foundation.Rect(base.PointToClient(new Windows.Foundation.Point(x, y)), new Windows.Foundation.Size(width, height)));
            return finalSize;
        }

        internal override RowPresenter GetRow(int row)
        {
            return this._cornerRow;
        }

        internal override SheetSpanModelBase GetSpanModel()
        {
            return null;
        }

        internal override Windows.Foundation.Size GetViewportSize(Windows.Foundation.Size availableSize)
        {
            double headerWidth = base.Sheet.GetSheetLayout().HeaderWidth;
            double headerHeight = base.Sheet.GetSheetLayout().HeaderHeight;
            headerWidth = Math.Min(headerWidth, availableSize.Width);
            return new Windows.Foundation.Size(headerWidth, Math.Min(headerHeight, availableSize.Height));
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            this._cornerRow.Measure(availableSize);
            return this.GetViewportSize(availableSize);
        }

        internal override bool SupportCellOverflow
        {
            get { return  false; }
        }

        protected override bool SupportSelection
        {
            get { return  false; }
        }
    }
}

