#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 尺子段，如100px为一段，包括5个刻度线一个数字
    /// </summary>
    public partial class RulerSegment : ContentControl
    {
        #region 静态内容
        /// <summary>
        /// 刻度线个数
        /// </summary>
        public static readonly DependencyProperty IntervalsProperty = DependencyProperty.Register(
            "Intervals",
            typeof(int),
            typeof(RulerSegment),
            new PropertyMetadata(5));

        public static readonly DependencyProperty StartValueProperty = DependencyProperty.Register(
            "StartValue",
            typeof(double),
            typeof(RulerSegment),
            new PropertyMetadata(0.0));
        #endregion

        #region 成员变量
        TickAlignment _align;
        TextBlock _label;
        readonly CompositeTransform _labelPosition = new CompositeTransform();
        Panel _mCanvas;
        readonly List<Tick> _mTicks;
        internal TranslateTransform _mTranslateTransform = new TranslateTransform();
        double _segmentWidth;
        double _thickness;
        #endregion

        #region 构造方法
        public RulerSegment()
        {
            DefaultStyleKey = typeof(RulerSegment);
            _mTicks = new List<Tick>();
            RenderTransform = _mTranslateTransform;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置刻度线个数
        /// </summary>
        public int Intervals
        {
            get { return (int)((int)base.GetValue(IntervalsProperty)); }
            set { base.SetValue(IntervalsProperty, (int)value); }
        }

        /// <summary>
        /// 获取设置起始刻度值
        /// </summary>
        public double StartValue
        {
            get { return (double)((double)base.GetValue(StartValueProperty)); }
            internal set { base.SetValue(StartValueProperty, (double)value); }
        }

        internal double PxSegmentWidth
        {
            get { return Ruler._unit.ToPixel(SegmentWidth); }
        }

        public Ruler Ruler { get; internal set; }

        public double SegmentWidth
        {
            get { return _segmentWidth; }
            internal set
            {
                if (_segmentWidth != value)
                {
                    _segmentWidth = value;
                    base.InvalidateArrange();
                }
            }
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _mCanvas = base.GetTemplateChild("Part_RulerSegmentPanel") as Panel;
            _label = base.GetTemplateChild("PART_Label") as TextBlock;
            _label.RenderTransform = _labelPosition;
            _label.SizeChanged -= OnLabelSizeChanged;
            _label.SizeChanged += OnLabelSizeChanged;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _thickness = Ruler.Thickness;
            CreateUpdateTicks();
            base.MeasureOverride(availableSize);

            double width = availableSize.Width;
            if (double.IsNaN(width) || double.IsInfinity(width))
                width = 0;
            double height = availableSize.Height;
            if (double.IsNaN(height) || double.IsInfinity(height))
                height = 0;
            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _thickness = Ruler.Thickness;
            double num = 0.0;
            for (int i = 1; i <= Intervals; i++)
            {
                ArrangeLine(_mCanvas.Children[i] as Line, num);
                num += PxSegmentWidth / ((double)Intervals);
            }
            base.ArrangeOverride(finalSize);
            return finalSize;
        }
        #endregion

        #region 内部方法
        void ArrangeLine(Line tick, double value)
        {
            if (Ruler.Orientation == Orientation.Horizontal)
            {
                (tick.RenderTransform as TranslateTransform).X = Math.Abs(value);
            }
            else
            {
                (tick.RenderTransform as TranslateTransform).Y = Math.Abs(value);
            }
        }

        void CreateUpdateTicks()
        {
            for (int i = _mTicks.Count; i < Intervals; i++)
            {
                Tick newTick = new Tick();
                newTick.Segment = this;
                _mTicks.Add(newTick);
                Line line2 = new Line();
                line2.RenderTransform = new TranslateTransform();
                Line line = line2;
                Binding binding = new Binding();
                binding.Path = new PropertyPath("Foreground");
                binding.Source = this;
                line.SetBinding(Shape.StrokeProperty, binding);
                _mCanvas.Children.Add(line);
            }
            double num2 = 0.0;
            int num3 = 1;
            foreach (Tick tick2 in _mTicks)
            {
                Line line3 = _mCanvas.Children[num3] as Line;
                tick2.Value = num2;
                Point point = tick2.GetLinePoint(PxSegmentWidth, _thickness, out _align);
                if (Ruler.Orientation == Orientation.Horizontal)
                {
                    line3.Y1 = point.X;
                    line3.Y2 = point.Y;
                }
                else
                {
                    line3.X1 = point.X;
                    line3.X2 = point.Y;
                }
                num3++;
                num2 += PxSegmentWidth / ((double)Intervals);
            }
        }

        void UpdateLabel(TextBlock label)
        {
            Point point = new Point(0.0, 0.0);

            Size size = new Size(Math.Ceiling(label.DesiredSize.Width), Math.Ceiling(label.DesiredSize.Height));
            if (Ruler.Orientation == Orientation.Horizontal)
            {
                _labelPosition.Rotation = 0.0;
                if (_align == TickAlignment.LeftOrTop)
                {
                    point.X = 3.0;
                    point.Y = (_thickness - size.Height) - 2.0;
                }
                else if (_align == TickAlignment.RightOrBottom)
                {
                    point.X = 3.0;
                    point.Y = 2.0;
                }
                else
                {
                    point.X = (PxSegmentWidth - size.Width) / 2.0;
                    point.Y = (_thickness - size.Height) / 2.0;
                }
            }
            else
            {
                _labelPosition.Rotation = -90.0;
                if (_align == TickAlignment.LeftOrTop)
                {
                    point.X = (_thickness - size.Height) - 2.0;
                    point.Y = size.Width + 3.0;
                }
                else if (_align == TickAlignment.RightOrBottom)
                {
                    point.X = 2.0;
                    point.Y = size.Width + 3.0;
                }
                else
                {
                    point.X = (_thickness - size.Height) / 2.0;
                    point.Y = (PxSegmentWidth + size.Width) / 2.0;
                }
            }
            _labelPosition.TranslateX = point.X;
            _labelPosition.TranslateY = point.Y;
        }

        void OnLabelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLabel(_label);
        }
        #endregion
    }

    public class RulerLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return ((double)Math.Round((double)value, 3)).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return value;
        }
    }
}
