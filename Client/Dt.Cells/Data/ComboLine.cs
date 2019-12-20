#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a combo border line.
    /// </summary>
    internal partial class ComboLine : Canvas
    {
        internal InnerLine _line1;
        internal InnerLine _line2;
        internal InnerLine _line3;

        internal ComboLine()
        {
            base.HorizontalAlignment = HorizontalAlignment.Left;
            base.VerticalAlignment = VerticalAlignment.Top;
            this._line1 = new InnerLine();
            this._line2 = new InnerLine();
            this._line3 = new InnerLine();
            base.Children.Add(this._line1.Element);
            base.Children.Add(this._line2.Element);
            base.Children.Add(this._line3.Element);
        }

        private static void ApplyDashArray(InnerLine line, DoubleCollection value, int offset)
        {
            line.ClearDashArray();
            if (value != null)
            {
                foreach (double num in value)
                {
                    line.AddDashArray(num);
                }
                line.StrokeDashOffset = offset;
            }
        }

        /// <summary>
        /// Creates a border line element or gets a border line element from BorderLinesPool"/&gt;.
        /// </summary>
        /// <param name="linePool">The line pool.</param>
        /// <param name="borderLine">The border line.</param>
        /// <returns>The border line</returns>
        public static ComboLine Create(BorderLinesPool linePool, BorderLine borderLine)
        {
            ComboLine line;
            if (linePool != null)
            {
                line = linePool.Pop() as ComboLine;
            }
            else
            {
                line = new ComboLine();
            }
            if (borderLine != null)
            {
                if (borderLine.Style == BorderLineStyle.Double)
                {
                    line._line1.Visibility = Visibility.Visible;
                    line._line2.Visibility = Visibility.Collapsed;
                    line._line3.Visibility = Visibility.Visible;
                    line._line1.StrokeThickness = 1.0;
                    line._line3.StrokeThickness = 1.0;
                    if (linePool == null)
                    {
                        line._line1.Stroke = new SolidColorBrush(borderLine.Color);
                        line._line3.Stroke = new SolidColorBrush(borderLine.Color);
                    }
                    else
                    {
                        line._line1.SetColor(linePool.GetSolidBrush(borderLine.Color), borderLine.Color);
                        line._line3.SetColor(linePool.GetSolidBrush(borderLine.Color), borderLine.Color);
                    }
                    ApplyDashArray(line._line1, borderLine.StyleData.FarDash, borderLine.StyleData.StrokeDashOffset);
                    ApplyDashArray(line._line3, borderLine.StyleData.NearDash, borderLine.StyleData.StrokeDashOffset);
                    return line;
                }
                if (borderLine.Style == BorderLineStyle.SlantedDashDot)
                {
                    line._line1.Visibility = Visibility.Visible;
                    line._line2.Visibility = Visibility.Visible;
                    line._line3.Visibility = Visibility.Collapsed;
                    line._line1.StrokeThickness = 1.0;
                    line._line2.StrokeThickness = 1.0;
                    if (linePool == null)
                    {
                        line._line1.Stroke = new SolidColorBrush(borderLine.Color);
                        line._line2.Stroke = new SolidColorBrush(borderLine.Color);
                    }
                    else
                    {
                        line._line1.SetColor(linePool.GetSolidBrush(borderLine.Color), borderLine.Color);
                        line._line2.SetColor(linePool.GetSolidBrush(borderLine.Color), borderLine.Color);
                    }
                    ApplyDashArray(line._line1, borderLine.StyleData.FarDash, borderLine.StyleData.StrokeDashOffset - 1);
                    ApplyDashArray(line._line2, borderLine.StyleData.MiddleDash, borderLine.StyleData.StrokeDashOffset);
                    return line;
                }
                line._line1.Visibility = Visibility.Collapsed;
                line._line2.Visibility = Visibility.Visible;
                line._line3.Visibility = Visibility.Collapsed;
                line._line2.StrokeThickness = borderLine.StyleData.DrawingThickness;
                if (linePool == null)
                {
                    line._line2.Stroke = new SolidColorBrush(borderLine.Color);
                }
                else
                {
                    line._line2.SetColor(linePool.GetSolidBrush(borderLine.Color), borderLine.Color);
                }
                ApplyDashArray(line._line2, borderLine.StyleData.MiddleDash, borderLine.StyleData.StrokeDashOffset);
            }
            return line;
        }

        /// <summary>
        /// Layouts the specified border line.
        /// </summary>
        /// <param name="lineItem">The LineItem to indicate line info.</param>
        /// <param name="hOffset">The horizontal offset.</param>
        /// <param name="vOffset">The vertical offset.</param>
        public void Layout(LineItem lineItem, double hOffset, double vOffset)
        {
            if (lineItem.Line != null)
            {
                if (lineItem.Line.Style == BorderLineStyle.Double)
                {
                    DoubleLineLayout.Layout(this, lineItem, hOffset, vOffset);
                }
                else if (lineItem.Line.Style == BorderLineStyle.SlantedDashDot)
                {
                    SlantedLinelayout.Layout(this, lineItem, hOffset, vOffset);
                }
                else
                {
                    DefaultLineLayout.Layout(this, lineItem, hOffset, vOffset);
                }
            }
            else
            {
                DefaultLineLayout.Layout(this, lineItem, hOffset, vOffset);
            }
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the line start point.
        /// </summary>
        /// <value>
        /// The x-coordinate for the start point of the line.
        /// </value>
        public double X1
        {
            get { return  this._line2.X1; }
            set
            {
                this._line1.X1 = value;
                this._line2.X1 = value - 0.5;
                this._line3.X1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the line end point.
        /// </summary>
        /// <value>
        /// The x-coordinate for the end point of the line.
        /// </value>
        public double X2
        {
            get { return  this._line2.X2; }
            set
            {
                this._line1.X2 = value;
                this._line2.X2 = value - 0.5;
                this._line3.X2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the y-coordinate of the line start point.
        /// </summary>
        /// <value>
        /// The y-coordinate for the start point of the line.
        /// </value>
        public double Y1
        {
            get { return  this._line2.Y1; }
            set
            {
                this._line1.Y1 = value;
                this._line2.Y1 = value - 0.5;
                this._line3.Y1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the y-coordinate of the line end point.
        /// </summary>
        /// <value>
        /// The y-coordinate for the end point of the line.
        /// </value>
        public double Y2
        {
            get { return  this._line2.Y2; }
            set
            {
                this._line1.Y2 = value;
                this._line2.Y2 = value - 0.5;
                this._line3.Y2 = value;
            }
        }

        internal class InnerLine
        {
            private Windows.UI.Color _cachedColor = Colors.Transparent;
            private List<double> _dashArray = new List<double>();
            private Windows.UI.Xaml.Shapes.Line _line = new Windows.UI.Xaml.Shapes.Line();
            private SolidColorBrush _stroke;
            private double _strokeDashOffset;
            private double _strokeThickness = 1.0;
            private Windows.UI.Xaml.Visibility _visibility;
            private double _x1 = 1.0;
            private double _x2 = 1.0;
            private double _y1 = 1.0;
            private double _y2 = 1.0;

            public void AddDashArray(double value)
            {
                if (this._dashArray.Count == 0)
                {
                    this._line.StrokeDashOffset = this._strokeDashOffset;
                }
                this._dashArray.Add(value);
                this._line.StrokeDashArray.Add(value);
            }

            public void ClearDashArray()
            {
                if (this._dashArray.Count > 0)
                {
                    this._dashArray.Clear();
                    this._line.StrokeDashArray.Clear();
                }
            }

            public void SetColor(SolidColorBrush brush, Windows.UI.Color color)
            {
                if (color != this._cachedColor)
                {
                    this._cachedColor = color;
                    this._line.Stroke = brush;
                }
            }

            internal Windows.UI.Xaml.Shapes.Line Element
            {
                get { return  this._line; }
            }

            public SolidColorBrush Stroke
            {
                get { return  this._stroke; }
                set
                {
                    if (this._stroke != value)
                    {
                        this._stroke = value;
                        this._line.Stroke = value;
                    }
                }
            }

            public double StrokeDashOffset
            {
                get { return  this._strokeDashOffset; }
                set
                {
                    if (this._strokeDashOffset != value)
                    {
                        this._strokeDashOffset = value;
                        if (this._dashArray.Count > 0)
                        {
                            this._line.StrokeDashOffset = value;
                        }
                    }
                }
            }

            public double StrokeThickness
            {
                get { return  this._strokeThickness; }
                set
                {
                    if (this._strokeThickness != value)
                    {
                        this._strokeThickness = value;
                        this._line.StrokeThickness = value;
                    }
                }
            }

            public Windows.UI.Xaml.Visibility Visibility
            {
                get { return  this._visibility; }
                set
                {
                    if (this._visibility != value)
                    {
                        this._visibility = value;
                        this._line.Visibility = value;
                    }
                }
            }

            public double X1
            {
                get { return  this._x1; }
                set
                {
                    if (this._x1 != value)
                    {
                        this._x1 = value;
                        this._line.X1 = value;
                    }
                }
            }

            public double X2
            {
                get { return  this._x2; }
                set
                {
                    if (this._x2 != value)
                    {
                        this._x2 = value;
                        this._line.X2 = value;
                    }
                }
            }

            public double Y1
            {
                get { return  this._y1; }
                set
                {
                    if (this._y1 != value)
                    {
                        this._y1 = value;
                        this._line.Y1 = value;
                    }
                }
            }

            public double Y2
            {
                get { return  this._y2; }
                set
                {
                    if (this._y2 != value)
                    {
                        this._y2 = value;
                        this._line.Y2 = value;
                    }
                }
            }
        }
    }
}

