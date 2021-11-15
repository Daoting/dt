#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class FloatingObjectMovingLayer : Panel
    {
        static Rect _rcEmpty = new Rect();
        static Size _szEmpty = new Size();
        int _recycledStart;

        public FloatingObjectMovingLayer(CellsPanel parentViewport)
        {
            ParentViewport = parentViewport;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _recycledStart = 0;
            Rect[] layouts = OperatingFrameLayouts;
            if (layouts == null || layouts.Length == 0)
            {
                if (Children.Count > 0)
                {
                    foreach (UIElement elem in Children)
                    {
                        elem.Measure(_szEmpty);
                    }
                }
                return availableSize;
            }

            for (int i = 0; i < layouts.Length; i++)
            {
                var rc = PopFrame();
                rc.Measure(new Size(layouts[i].Width, layouts[i].Height));
            }

            if (_recycledStart < Children.Count)
            {
                for (int i = _recycledStart; i < Children.Count; i++)
                {
                    ((Rectangle)Children[i]).Measure(_szEmpty);
                }
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect[] layouts = OperatingFrameLayouts;
            if (layouts == null || layouts.Length == 0)
            {
                if (Children.Count > 0)
                {
                    foreach (UIElement elem in Children)
                    {
                        elem.Arrange(_rcEmpty);
                    }
                }
                return finalSize;
            }

            for (int i = 0; i < layouts.Length; i++)
            {
                ((Rectangle)Children[i]).Arrange(layouts[i]);
            }

            if (layouts.Length < Children.Count)
            {
                for (int i = layouts.Length; i < Children.Count; i++)
                {
                    ((Rectangle)Children[i]).Arrange(_rcEmpty);
                }
            }
            return finalSize;
        }

        Rectangle PopFrame()
        {
            Rectangle rect;
            if (_recycledStart >= Children.Count)
            {
                rect = new Rectangle
                {
                    StrokeThickness = 1.0,
                    Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(50, 110, 110, 110)),
                    Stroke = BrushRes.BlackBrush,
                };
                Children.Add(rect);
            }
            else
            {
                rect = (Rectangle)Children[_recycledStart];
            }
            _recycledStart++;
            return rect;
        }

        Rect[] OperatingFrameLayouts
        {
            get
            {
                if (ParentViewport._cachedChartShapeMovingRects != null && ParentViewport._cachedChartShapeMovingRects.Length > 0)
                {
                    return ParentViewport._cachedChartShapeMovingRects;
                }
                return ParentViewport._cachedChartShapeResizingRects;
            }
        }

        public CellsPanel ParentViewport { get; set; }
    }
}

