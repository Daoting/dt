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
            _line1 = new InnerLine();
            _line2 = new InnerLine();
            _line3 = new InnerLine();
            base.Children.Add(_line1.Element);
            base.Children.Add(_line2.Element);
            base.Children.Add(_line3.Element);
        }

        static void ApplyDashArray(InnerLine line, DoubleCollection value, int offset)
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
            get { return  _line2.X1; }
            set
            {
                _line1.X1 = value;
                _line2.X1 = value - 0.5;
                _line3.X1 = value;
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
            get { return  _line2.X2; }
            set
            {
                _line1.X2 = value;
                _line2.X2 = value - 0.5;
                _line3.X2 = value;
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
            get { return  _line2.Y1; }
            set
            {
                _line1.Y1 = value;
                _line2.Y1 = value - 0.5;
                _line3.Y1 = value;
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
            get { return  _line2.Y2; }
            set
            {
                _line1.Y2 = value;
                _line2.Y2 = value - 0.5;
                _line3.Y2 = value;
            }
        }

        internal class InnerLine
        {
            Windows.UI.Color _cachedColor = Colors.Transparent;
            List<double> _dashArray = new List<double>();
            Line _line = new Line();
            SolidColorBrush _stroke;
            double _strokeDashOffset;
            double _strokeThickness = 1.0;
            Windows.UI.Xaml.Visibility _visibility;
            double _x1 = 1.0;
            double _x2 = 1.0;
            double _y1 = 1.0;
            double _y2 = 1.0;

            public void AddDashArray(double value)
            {
                if (_dashArray.Count == 0)
                {
                    _line.StrokeDashOffset = _strokeDashOffset;
                }
                _dashArray.Add(value);
                if (_line.StrokeDashArray == null)
                    _line.StrokeDashArray = new DoubleCollection();
                _line.StrokeDashArray.Add(value);
            }

            public void ClearDashArray()
            {
                if (_dashArray.Count > 0)
                {
                    _dashArray.Clear();
                    _line.StrokeDashArray?.Clear();
                }
            }

            public void SetColor(SolidColorBrush brush, Windows.UI.Color color)
            {
                if (color != _cachedColor)
                {
                    _cachedColor = color;
                    _line.Stroke = brush;
                }
            }

            internal Line Element
            {
                get { return  _line; }
            }

            public SolidColorBrush Stroke
            {
                get { return  _stroke; }
                set
                {
                    if (_stroke != value)
                    {
                        _stroke = value;
                        _line.Stroke = value;
                    }
                }
            }

            public double StrokeDashOffset
            {
                get { return  _strokeDashOffset; }
                set
                {
                    if (_strokeDashOffset != value)
                    {
                        _strokeDashOffset = value;
                        if (_dashArray.Count > 0)
                        {
                            _line.StrokeDashOffset = value;
                        }
                    }
                }
            }

            public double StrokeThickness
            {
                get { return  _strokeThickness; }
                set
                {
                    if (_strokeThickness != value)
                    {
                        _strokeThickness = value;
                        _line.StrokeThickness = value;
                    }
                }
            }

            public Windows.UI.Xaml.Visibility Visibility
            {
                get { return  _visibility; }
                set
                {
                    if (_visibility != value)
                    {
                        _visibility = value;
                        _line.Visibility = value;
                    }
                }
            }

            public double X1
            {
                get { return  _x1; }
                set
                {
                    if (_x1 != value)
                    {
                        _x1 = value;
                        _line.X1 = value;
                    }
                }
            }

            public double X2
            {
                get { return  _x2; }
                set
                {
                    if (_x2 != value)
                    {
                        _x2 = value;
                        _line.X2 = value;
                    }
                }
            }

            public double Y1
            {
                get { return  _y1; }
                set
                {
                    if (_y1 != value)
                    {
                        _y1 = value;
                        _line.Y1 = value;
                    }
                }
            }

            public double Y2
            {
                get { return  _y2; }
                set
                {
                    if (_y2 != value)
                    {
                        _y2 = value;
                        _line.Y2 = value;
                    }
                }
            }
        }
    }
}

