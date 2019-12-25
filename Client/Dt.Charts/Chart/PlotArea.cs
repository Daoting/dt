#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public partial class PlotArea : DependencyObject
    {
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background",  typeof(Brush),  typeof(PlotArea), new PropertyMetadata(null));
        public static readonly DependencyProperty ColumnProperty = DependencyProperty.Register("Column",  typeof(int),  typeof(PlotArea), new PropertyMetadata((int) 0));
        Rect plotRect = new Rect();
        Rectangle rect;
        public static readonly DependencyProperty RowProperty = DependencyProperty.Register("Row",  typeof(int),  typeof(PlotArea), new PropertyMetadata((int) 0));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke",  typeof(Brush),  typeof(PlotArea), new PropertyMetadata(null));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness",  typeof(double),  typeof(PlotArea), new PropertyMetadata((double) 1.0));

        internal void SetPlotX(double x, double w)
        {
            plotRect.X = x;
            plotRect.Width = w;
        }

        internal void SetPlotY(double y, double h)
        {
            plotRect.Y = y;
            plotRect.Height = h;
        }

        public Brush Background
        {
            get { return  (Brush) GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public int Column
        {
            get { return  (int) ((int) GetValue(ColumnProperty)); }
            set { SetValue(ColumnProperty, (int) value); }
        }

        public int Row
        {
            get { return  (int) ((int) GetValue(RowProperty)); }
            set { SetValue(RowProperty, (int) value); }
        }

        public Brush Stroke
        {
            get { return  (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return  (double) ((double) GetValue(StrokeThicknessProperty)); }
            set { SetValue(StrokeThicknessProperty, (double) value); }
        }

        internal Windows.UI.Xaml.UIElement UIElement
        {
            get
            {
                if (((plotRect.Width <= 0.0) || (plotRect.Height <= 0.0)) || ((Background == null) && (Stroke == null)))
                {
                    return null;
                }
                if (rect == null)
                {
                    rect = new Rectangle();
                }
                rect.Fill = Background;
                rect.Stroke = Stroke;
                rect.StrokeThickness = StrokeThickness;
                rect.Width = plotRect.Width;
                rect.Height = plotRect.Height;
                Canvas.SetLeft(rect, plotRect.X);
                Canvas.SetTop(rect, plotRect.Y);
                return rect;
            }
        }
    }
}

