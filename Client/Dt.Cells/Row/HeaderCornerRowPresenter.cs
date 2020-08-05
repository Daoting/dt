#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    internal partial class HeaderCornerRowPresenter : RowPresenter
    {
        CellPresenterBase _cornerCell;
        GcHeaderCornerViewport _owningPresenter;

        public HeaderCornerRowPresenter(GcHeaderCornerViewport cornerHeader) : base(cornerHeader)
        {
            _cornerCell = new CornerHeaderCellPresenter();
            _cornerCell.OwningRow = this;
            Children.Add(_cornerCell);
            _owningPresenter = cornerHeader;
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            _cornerCell.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            return finalSize;
        }

        public override CellPresenterBase GetCell(int column)
        {
            return _cornerCell;
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            double width = availableSize.Width;
            double height = availableSize.Height;
            _cornerCell.Measure(new Windows.Foundation.Size(width, height));
            GcViewport parent = base.Parent as GcViewport;
            if (parent != null)
            {
                return parent.GetViewportSize(availableSize);
            }
            if (!double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
            {
                return availableSize;
            }
            return _cornerCell.DesiredSize;
        }

        public override GcViewport OwningPresenter
        {
            get { return  _owningPresenter; }
        }
    }
}

