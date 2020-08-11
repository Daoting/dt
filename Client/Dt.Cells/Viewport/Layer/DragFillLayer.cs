#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class DragFillLayer : Panel
    {
        Rectangle _dragClearRectangle;
        DragFillFrame _dragFillFrame;

        public DragFillLayer()
        {
            _dragFillFrame = new DragFillFrame();
            Children.Add(_dragFillFrame);
            Rectangle rectangle = new Rectangle();
            rectangle.Fill = new SolidColorBrush(Color.FromArgb(200, 110, 110, 110));
            _dragClearRectangle = rectangle;
            Children.Add(_dragClearRectangle);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (DragFillFrame != null)
            {
                Rect rect = ParentViewport._cachedDragFillFrameRect;
                if ((rect.Width > 0.0) && (rect.Height > 0.0))
                {
                    DragFillFrame.Visibility = Visibility.Visible;
                    DragFillFrame.Arrange(rect);
                    DragFillFrame.InvalidateArrange();
                }
                else
                {
                    DragFillFrame.Visibility = Visibility.Collapsed;
                }
            }
            if (DragClearRectangle != null)
            {
                Rect rect2 = ParentViewport._cachedDragClearRect;
                if ((rect2.Width > 0.0) && (rect2.Height > 0.0))
                {
                    DragClearRectangle.Visibility = Visibility.Visible;
                    DragClearRectangle.Arrange(rect2);
                    DragClearRectangle.InvalidateArrange();
                    return finalSize;
                }
                DragClearRectangle.Visibility = Visibility.Collapsed;
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (DragFillFrame != null)
            {
                Rect rect = ParentViewport._cachedDragFillFrameRect;
                if ((rect.Width > 0.0) && (rect.Height > 0.0))
                {
                    DragFillFrame.Visibility = Visibility.Visible;
                    DragFillFrame.Measure(new Size(rect.Width, rect.Height));
                }
                else
                {
                    DragFillFrame.Visibility = Visibility.Collapsed;
                }
            }
            if (DragClearRectangle != null)
            {
                Rect rect2 = ParentViewport._cachedDragClearRect;
                if ((rect2.Width > 0.0) && (rect2.Height > 0.0))
                {
                    DragClearRectangle.Visibility = Visibility.Visible;
                    DragClearRectangle.Measure(new Size(rect2.Width, rect2.Height));
                }
                else
                {
                    DragClearRectangle.Visibility = Visibility.Collapsed;
                }
            }
            return ParentViewport.GetViewportSize(availableSize);
        }

        public Rectangle DragClearRectangle
        {
            get { return _dragClearRectangle; }
        }

        public DragFillFrame DragFillFrame
        {
            get { return _dragFillFrame; }
        }

        public CellsPanel ParentViewport { get; set; }
    }

}

