#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 水平垂直尺子控件
    /// </summary>
    public partial class Ruler : Control
    {
        #region 静态内容
        /// <summary>
        /// 布局方向
        /// </summary>
        public readonly static DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(Ruler),
            new PropertyMetadata(Orientation.Horizontal, Invalidate));

        /// <summary>
        /// 偏移量
        /// </summary>
        public readonly static DependencyProperty OffsetProperty = DependencyProperty.Register(
            "Offset",
            typeof(double),
            typeof(Ruler),
            new PropertyMetadata((double)0, Invalidate));

        /// <summary>
        /// 比例尺
        /// </summary>
        public readonly static DependencyProperty ScaleProperty = DependencyProperty.Register(
            "Scale",
            typeof(double),
            typeof(Ruler),
            new PropertyMetadata((double)1, Invalidate));

        /// <summary>
        /// 水平时的高度或垂直时的宽度
        /// </summary>
        public readonly static DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness",
            typeof(double),
            typeof(Ruler),
            new PropertyMetadata((double)25.0, Invalidate));

        static void Invalidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Ruler).InvalidateMeasure();
        }

        #endregion

        #region 成员变量
        Panel _canvas;
        readonly CompositeTransform _rulerTransform;
        internal double _segmentWidth;
        internal LengthUnit _unit;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public Ruler()
        {
            DefaultStyleKey = typeof(Ruler);

            _unit = new LengthUnit();
            _rulerTransform = new CompositeTransform();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置布局方向
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// 获取设置偏移量
        /// </summary>
        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, (double)value); }
        }

        /// <summary>
        /// 获取设置比例尺
        /// </summary>
        new public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, (double)value); }
        }

        /// <summary>
        /// 获取设置尺子水平时的高度或垂直时的宽度
        /// </summary>
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, (double)value); }
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = GetTemplateChild("Part_RulerPanel") as Panel;
            _canvas.RenderTransform = _rulerTransform;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateSegmentWidth();
            double d = 0.0;
            if (Orientation == Orientation.Horizontal)
            {
                d = availableSize.Width;
                availableSize.Height = Thickness;
            }
            else
            {
                d = availableSize.Height;
                availableSize.Width = Thickness;
            }
            if (!double.IsNaN(d) && !double.IsInfinity(d))
            {
                double num2 = _unit.ToUnit(d);
                double start = _unit.ToUnit(Offset);
                UpdateSegments(start, num2 + start);
                if (_canvas.Children.Count != 0)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        _rulerTransform.TranslateX = _unit.ToPixel((_canvas.Children[0] as RulerSegment).StartValue) * Scale - Offset;
                    }
                    else
                    {
                        _rulerTransform.TranslateY = (_unit.ToPixel((_canvas.Children[0] as RulerSegment).StartValue) * Scale) - Offset;
                    }
                }
            }
            base.MeasureOverride(availableSize);
            if (double.IsNaN(d) || double.IsInfinity(d))
                d = 0;
            if (Orientation == Orientation.Horizontal)
                return new Size(d, Thickness);
            return new Size(Thickness, d);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            finalSize = base.ArrangeOverride(finalSize);
            return new Size(finalSize.Width + 0.1, finalSize.Height + 0.1);
        }
        #endregion

        #region 内部方法
        static double BestRound(double value)
        {
            int digits = 0;
            for (double i = value; i < 1.0; i *= 10.0)
            {
                digits++;
            }
            return Math.Round(value, digits);
        }

        static double GetSegmentWidth(LengthUnit _unit, double scale)
        {
            double num = BestRound(_unit.ToUnit((double)100.0));
            double num2 = 25.0;
            double num3 = Math.Pow(2.0, Math.Round((double)(Math.Log(scale) / Math.Log(2.0))));
            double num4 = num;
            num4 = num / num3;
            double num5 = 1.0;
            while (num4 > 100.0)
            {
                num5 /= 10.0;
                num4 /= 10.0;
            }
            while (num4 < 25.0)
            {
                num5 *= 10.0;
                num4 *= 10.0;
            }
            if ((num4 >= num2) && ((num4 % num2) != 0.0))
            {
                num4 = Math.Round((double)(num4 / num2)) * num2;
            }
            return ((num4 * scale) / num5);
        }

        double UpdateSegment(double start, double end, RulerSegment rulerSegment, double run, ref double trans)
        {
            double segmentWidth = _segmentWidth;
            if (run == start)
            {
                rulerSegment.StartValue = (Math.Floor((double)(start / segmentWidth)) * segmentWidth) / Scale;
                run = rulerSegment.StartValue * Scale;
            }
            else if (run <= end)
            {
                rulerSegment.StartValue = run / Scale;
            }
            rulerSegment.SegmentWidth = segmentWidth;
            if (Orientation == Orientation.Horizontal)
            {
                rulerSegment._mTranslateTransform.X = trans;
            }
            else
            {
                rulerSegment._mTranslateTransform.Y = trans;
            }
            trans += rulerSegment.PxSegmentWidth;
            run += segmentWidth;
            return run;
        }

        void UpdateSegments(double start, double end)
        {
            double run = start;
            double trans = 0.0;
            foreach (RulerSegment segment in _canvas.Children)
            {
                run = UpdateSegment(start, end, segment, run, ref trans);
            }
            while (run < end)
            {
                RulerSegment newSegment = new RulerSegment();
                newSegment.Ruler = this;
                _canvas.Children.Add(newSegment);
                run = UpdateSegment(start, end, newSegment, run, ref trans);
            }
        }

        void UpdateSegmentWidth()
        {
            _segmentWidth = GetSegmentWidth(_unit, Scale);
        }
        #endregion
    }
}
