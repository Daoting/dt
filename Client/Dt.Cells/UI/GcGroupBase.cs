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
        protected Excel _excel;
        Brush borderBrush;
        internal const double PADDING = 2.0;

        public GcGroupBase(Excel p_excel)
        {
            _excel = p_excel;
            if ((_excel != null) && (_excel.RangeGroupBackground != null))
            {
                Background = _excel.RangeGroupBackground;
            }
            else if (Application.Current.RequestedTheme == ApplicationTheme.Light)
            {
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xb1, 0xab, 0xab));
            }
            else
            {
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xd1, 0xd1, 0xd1));
            }
        }

        protected virtual void ArrangeBorderLines(Size finalSize)
        {
            double x;
            double y;
            if (_borderRight != null)
            {
                x = (Location.X + finalSize.Width) - 1.0;
                y = Location.Y;
                _borderRight.Arrange(new Rect(PointToClient(new Point(x, y)), new Size(1.0, finalSize.Height)));
            }
            if (_borderBottom != null)
            {
                x = Location.X;
                y = (Location.Y + finalSize.Height) - 1.0;
                _borderBottom.Arrange(new Rect(PointToClient(new Point(x, y)), new Size(finalSize.Width, 1.0)));
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeBorderLines(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        protected virtual double CalcMinWidthOrHeight(Size finalSize, Orientation orientation)
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
            if (_excel.ActiveSheet != null)
            {
                if (orientation == Orientation.Horizontal)
                {
                    return _excel.ActiveSheet.RowRangeGroup.GetMaxLevel();
                }
                if (orientation == Orientation.Vertical)
                {
                    maxLevel = _excel.ActiveSheet.ColumnRangeGroup.GetMaxLevel();
                }
            }
            return maxLevel;
        }

        protected virtual void MeasureBorderLines(Size availableSize)
        {
            MeasureRightBorder(availableSize);
            MeasureBottomBorder(availableSize);
        }

        protected void MeasureBottomBorder(Size availableSize)
        {
            if (_borderBottom == null)
            {
                _borderBottom = new Rectangle();
            }
            _borderBottom.Fill = BorderBrush;
            if (!Children.Contains(_borderBottom))
            {
                Children.Add(_borderBottom);
            }
            _borderBottom.Measure(new Size(availableSize.Width, 1.0));
        }

        protected void MeasureRightBorder(Size availableSize)
        {
            if (_borderRight == null)
            {
                _borderRight = new Rectangle();
            }
            _borderRight.Fill = BorderBrush;
            if (!Children.Contains(_borderRight))
            {
                Children.Add(_borderRight);
            }
            _borderRight.Measure(new Size(1.0, availableSize.Height));
        }

        public Point PointToClient(Point point)
        {
            return new Point(point.X - Location.X, point.Y - Location.Y);
        }

#if !UWP
        new
#endif
        internal Brush BorderBrush
        {
            get
            {
                if ((_excel != null) && (_excel.RangeGroupBorderBrush != null))
                {
                    return _excel.RangeGroupBorderBrush;
                }
                if (borderBrush == null)
                    borderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 160, 160, 160));
                return borderBrush;
            }
        }

        public Point Location { get; set; }
    }
}

