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
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class TextPanel : Panel
    {
        #region 成员变量
        bool _isStrikeThrough;
        bool _isUnderLine;
        Line _strikeLine;
        TextBlock _tb;
        Line _underLine;
        #endregion

        public TextPanel()
        {
            _tb = new TextBlock();
            Children.Add(_tb);

            _strikeLine = new Line();
            _strikeLine.StrokeThickness = 1.0;
            _strikeLine.Stroke = new SolidColorBrush(Colors.Black);
            _isStrikeThrough = false;
            Children.Add(_strikeLine);

            _underLine = new Line();
            _underLine.StrokeThickness = 1.0;
            _underLine.Stroke = new SolidColorBrush(Colors.Black);
            _isUnderLine = false;
            Children.Add(_underLine);
        }

        public FontFamily FontFamily
        {
            get { return _tb.FontFamily; }
            set { _tb.FontFamily = value; }
        }

        public double FontSize
        {
            get { return _tb.FontSize; }
            set { _tb.FontSize = value; }
        }

        public FontStyle FontStyle
        {
            get { return _tb.FontStyle; }
            set { _tb.FontStyle = value; }
        }

        public FontWeight FontWeight
        {
            get { return _tb.FontWeight; }
            set { _tb.FontWeight = value; }
        }

#if ANDROID
        new
#endif
        public Brush Foreground
        {
            get { return _tb.Foreground; }
            set { _tb.Foreground = value; }
        }

        public bool IsStrikeThrough
        {
            get { return _isStrikeThrough; }
            set { _isStrikeThrough = value; }
        }

        public bool IsUnderLine
        {
            get { return _isUnderLine; }
            set { _isUnderLine = value; }
        }

        public string Text
        {
            get { return _tb.Text; }
            set { _tb.Text = value; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _tb.Measure(constraint);
            _strikeLine.Measure(constraint);
            _underLine.Measure(constraint);
            return _tb.DesiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            double width = _tb.DesiredSize.Width;
            double height = _tb.DesiredSize.Height;
            if (!double.IsInfinity(width) && !double.IsInfinity(arrangeBounds.Width))
                width = Math.Min(width, arrangeBounds.Width);
            if (!double.IsInfinity(height) && !double.IsInfinity(arrangeBounds.Height))
                height = Math.Min(height, arrangeBounds.Height);

            _tb.Arrange(new Rect(0.0, 0.0, width, height));
            if (_isStrikeThrough)
            {
                _strikeLine.X1 = 0.0;
                _strikeLine.Y1 = height / 2.0;
                _strikeLine.X2 = width;
                _strikeLine.Y2 = height / 2.0;
                _strikeLine.Arrange(new Rect(0.0, 0.0, width, height));
            }
            if (_isUnderLine)
            {
                _underLine.X1 = 0.0;
                _underLine.Y1 = height;
                _underLine.X2 = width;
                _underLine.Y2 = height;
                _underLine.Arrange(new Rect(0.0, 0.0, width, height));
            }
            return base.ArrangeOverride(arrangeBounds);
        }
    }
}

