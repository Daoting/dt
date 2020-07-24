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
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal abstract partial class GcGroupBase : Panel
    {
        protected Rectangle _borderBottom;
        protected Rectangle _borderRight;
        protected SheetView _sheetView;
        Brush borderBrush;
        internal const double PADDING = 2.0;

        public GcGroupBase(SheetView sheetView)
        {
            Action action = null;
            Action action2 = null;
            _sheetView = sheetView;
            if ((_sheetView != null) && (_sheetView.RangeGroupBackground != null))
            {
                base.Background = _sheetView.RangeGroupBackground;
            }
            else if (Application.Current.RequestedTheme == ApplicationTheme.Light)
            {
                if (action == null)
                {
                    action = delegate {
                        base.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xb1, 0xab, 0xab));
                    };
                }
                Dt.Cells.Data.UIAdaptor.InvokeSync(action);
            }
            else
            {
                if (action2 == null)
                {
                    action2 = delegate {
                        base.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xd1, 0xd1, 0xd1));
                    };
                }
                Dt.Cells.Data.UIAdaptor.InvokeSync(action2);
            }
        }

        protected virtual void ArrangeBorderLines(Windows.Foundation.Size finalSize)
        {
            double x;
            double y;
            if (_borderRight != null)
            {
                x = (Location.X + finalSize.Width) - 1.0;
                y = Location.Y;
                _borderRight.Arrange(new Windows.Foundation.Rect(PointToClient(new Windows.Foundation.Point(x, y)), new Windows.Foundation.Size(1.0, finalSize.Height)));
            }
            if (_borderBottom != null)
            {
                x = Location.X;
                y = (Location.Y + finalSize.Height) - 1.0;
                _borderBottom.Arrange(new Windows.Foundation.Rect(PointToClient(new Windows.Foundation.Point(x, y)), new Windows.Foundation.Size(finalSize.Width, 1.0)));
            }
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            ArrangeBorderLines(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        protected virtual double CalcMinWidthOrHeight(Windows.Foundation.Size finalSize, Orientation orientation)
        {
            double num = 0.0;
            int maxLevel = GetMaxLevel(orientation);
            if (orientation == Orientation.Horizontal)
            {
                num = (finalSize.Width - 4.0) / ((double) (maxLevel + 2));
            }
            else if (orientation == Orientation.Vertical)
            {
                num = (finalSize.Height - 4.0) / ((double) (maxLevel + 2));
            }
            return Math.Max(0.0, num);
        }

        protected virtual int GetMaxLevel(Orientation orientation)
        {
            int maxLevel = -1;
            if (_sheetView.Worksheet != null)
            {
                if (orientation == Orientation.Horizontal)
                {
                    return _sheetView.Worksheet.RowRangeGroup.GetMaxLevel();
                }
                if (orientation == Orientation.Vertical)
                {
                    maxLevel = _sheetView.Worksheet.ColumnRangeGroup.GetMaxLevel();
                }
            }
            return maxLevel;
        }

        protected virtual void MeasureBorderLines(Windows.Foundation.Size availableSize)
        {
            MeasureRightBorder(availableSize);
            MeasureBottomBorder(availableSize);
        }

        protected void MeasureBottomBorder(Windows.Foundation.Size availableSize)
        {
            if (_borderBottom == null)
            {
                _borderBottom = new Rectangle();
            }
            Dt.Cells.Data.UIAdaptor.InvokeSync(delegate {
                _borderBottom.Fill = BorderBrush;
            });
            if (!base.Children.Contains(_borderBottom))
            {
                base.Children.Add(_borderBottom);
            }
            _borderBottom.Measure(new Windows.Foundation.Size(availableSize.Width, 1.0));
        }

        protected void MeasureRightBorder(Windows.Foundation.Size availableSize)
        {
            if (_borderRight == null)
            {
                _borderRight = new Rectangle();
            }
            Dt.Cells.Data.UIAdaptor.InvokeSync(delegate {
                _borderRight.Fill = BorderBrush;
            });
            if (!base.Children.Contains(_borderRight))
            {
                base.Children.Add(_borderRight);
            }
            _borderRight.Measure(new Windows.Foundation.Size(1.0, availableSize.Height));
        }

        public Windows.Foundation.Point PointToClient(Windows.Foundation.Point point)
        {
            return new Windows.Foundation.Point(point.X - Location.X, point.Y - Location.Y);
        }

#if !UWP
        new
#endif
        internal Brush BorderBrush
        {
            get
            {
                Action action = null;
                if ((_sheetView != null) && (_sheetView.RangeGroupBorderBrush != null))
                {
                    return _sheetView.RangeGroupBorderBrush;
                }
                if (borderBrush == null)
                {
                    if (action == null)
                    {
                        action = delegate {
                            borderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 160, 160, 160));
                        };
                    }
                    Dt.Cells.Data.UIAdaptor.InvokeSync(action);
                }
                return borderBrush;
            }
        }

        public Windows.Foundation.Point Location { get; set; }
    }
}

