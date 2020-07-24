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
        private Brush borderBrush;
        internal const double PADDING = 2.0;

        public GcGroupBase(SheetView sheetView)
        {
            Action action = null;
            Action action2 = null;
            this._sheetView = sheetView;
            if ((this._sheetView != null) && (this._sheetView.RangeGroupBackground != null))
            {
                base.Background = this._sheetView.RangeGroupBackground;
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
            if (this._borderRight != null)
            {
                x = (this.Location.X + finalSize.Width) - 1.0;
                y = this.Location.Y;
                this._borderRight.Arrange(new Windows.Foundation.Rect(this.PointToClient(new Windows.Foundation.Point(x, y)), new Windows.Foundation.Size(1.0, finalSize.Height)));
            }
            if (this._borderBottom != null)
            {
                x = this.Location.X;
                y = (this.Location.Y + finalSize.Height) - 1.0;
                this._borderBottom.Arrange(new Windows.Foundation.Rect(this.PointToClient(new Windows.Foundation.Point(x, y)), new Windows.Foundation.Size(finalSize.Width, 1.0)));
            }
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            this.ArrangeBorderLines(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        protected virtual double CalcMinWidthOrHeight(Windows.Foundation.Size finalSize, Orientation orientation)
        {
            double num = 0.0;
            int maxLevel = this.GetMaxLevel(orientation);
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
            if (this._sheetView.Worksheet != null)
            {
                if (orientation == Orientation.Horizontal)
                {
                    return this._sheetView.Worksheet.RowRangeGroup.GetMaxLevel();
                }
                if (orientation == Orientation.Vertical)
                {
                    maxLevel = this._sheetView.Worksheet.ColumnRangeGroup.GetMaxLevel();
                }
            }
            return maxLevel;
        }

        protected virtual void MeasureBorderLines(Windows.Foundation.Size availableSize)
        {
            this.MeasureRightBorder(availableSize);
            this.MeasureBottomBorder(availableSize);
        }

        protected void MeasureBottomBorder(Windows.Foundation.Size availableSize)
        {
            if (this._borderBottom == null)
            {
                this._borderBottom = new Rectangle();
            }
            Dt.Cells.Data.UIAdaptor.InvokeSync(delegate {
                this._borderBottom.Fill = this.BorderBrush;
            });
            if (!base.Children.Contains(this._borderBottom))
            {
                base.Children.Add(this._borderBottom);
            }
            this._borderBottom.Measure(new Windows.Foundation.Size(availableSize.Width, 1.0));
        }

        protected void MeasureRightBorder(Windows.Foundation.Size availableSize)
        {
            if (this._borderRight == null)
            {
                this._borderRight = new Rectangle();
            }
            Dt.Cells.Data.UIAdaptor.InvokeSync(delegate {
                this._borderRight.Fill = this.BorderBrush;
            });
            if (!base.Children.Contains(this._borderRight))
            {
                base.Children.Add(this._borderRight);
            }
            this._borderRight.Measure(new Windows.Foundation.Size(1.0, availableSize.Height));
        }

        public Windows.Foundation.Point PointToClient(Windows.Foundation.Point point)
        {
            return new Windows.Foundation.Point(point.X - this.Location.X, point.Y - this.Location.Y);
        }

#if !UWP
        new
#endif
        internal Brush BorderBrush
        {
            get
            {
                Action action = null;
                if ((this._sheetView != null) && (this._sheetView.RangeGroupBorderBrush != null))
                {
                    return this._sheetView.RangeGroupBorderBrush;
                }
                if (this.borderBrush == null)
                {
                    if (action == null)
                    {
                        action = delegate {
                            this.borderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 160, 160, 160));
                        };
                    }
                    Dt.Cells.Data.UIAdaptor.InvokeSync(action);
                }
                return this.borderBrush;
            }
        }

        public Windows.Foundation.Point Location { get; set; }
    }
}

