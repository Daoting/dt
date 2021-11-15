#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Charts;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Sample
{
    public partial class LoadAnimation : Win
    {
        Random _rnd = new Random();

        public LoadAnimation()
        {
            InitializeComponent();

            _cbTrans.ItemsSource = EnumDataSource.FromType<AnimationTransform>();
            _cbTrans.SelectedIndex = 1;

            _cbOrigin.ItemsSource = EnumDataSource.FromType<AnimationOrigin>();
            _cbOrigin.SelectedIndex = 1;

            _cbEasing.ItemsSource = EnumDataSource.FromType<Easing>();
            _cbEasing.SelectedIndex = 0;

            _chart.Tapped += (s, e) => NewData();
            _chart.Loaded += (s, e) => NewData();
        }

        void NewData()
        {
            _chart.BeginUpdate();

            // uwp release版出错，ios android wasm无动画
            //AnimationTransform at = (AnimationTransform)_cbTrans.SelectedIndex;
            //AnimationOrigin ao = (AnimationOrigin)_cbOrigin.SelectedIndex;
            //Easing ea = (Easing)_cbEasing.SelectedIndex;
            //_chart.Data.LoadAnimation = CreateAnimation(at, ao, _cbDelay.IsChecked == true, ea);

            int nser = _rnd.Next(2, 6);
            int npts = _rnd.Next(5, 10);

            _chart.Data.Children.Clear();
            for (int i = 0; i < nser; i++)
                _chart.Data.Children.Add(ChartSampleData.CreateDataSeries(npts));
            _chart.EndUpdate();
        }

        public PlotElementAnimation CreateAnimation(AnimationTransform transform, AnimationOrigin origin, bool indexDelay, Easing easing)
        {
            var sb = new Storyboard();
            var duration = new Duration(TimeSpan.FromSeconds(0.5));

            var style = new Style();
            style.TargetType = typeof(Windows.UI.Xaml.Shapes.Path);
            if (transform == AnimationTransform.Scale)
                style.Setters.Add(new Setter(Windows.UI.Xaml.Shapes.Path.RenderTransformProperty, new ScaleTransform() { ScaleX = 0, ScaleY = 0 }));
            else if (transform == AnimationTransform.Rotation)
                style.Setters.Add(new Setter(Windows.UI.Xaml.Shapes.Path.RenderTransformProperty, new RotateTransform() { Angle = 180 }));
            else if (transform == AnimationTransform.Opacity)
                style.Setters.Add(new Setter(Windows.UI.Xaml.Shapes.Path.OpacityProperty, 0));

            var point = new Point(0.5, 0.5);
            switch (origin)
            {
                case AnimationOrigin.Bottom:
                    point = new Point(0.5, 2);
                    break;
                case AnimationOrigin.Top:
                    point = new Point(0.5, -2);
                    break;
                case AnimationOrigin.Left:
                    point = new Point(-2, 0.5);
                    break;
                case AnimationOrigin.Right:
                    point = new Point(2, 0.5);
                    break;
                case AnimationOrigin.TopRight:
                    point = new Point(2, -2);
                    break;
                case AnimationOrigin.TopLeft:
                    point = new Point(-2, -2);
                    break;
                case AnimationOrigin.BottomRight:
                    point = new Point(2, 2);
                    break;
                case AnimationOrigin.BottomLeft:
                    point = new Point(-2, 2);
                    break;
                default:
                    break;
            }

            style.Setters.Add(new Setter(Windows.UI.Xaml.Shapes.Path.RenderTransformOriginProperty, point));

            var da = new DoubleAnimation() { From = 0, To = 1, Duration = duration };
            Storyboard.SetTargetProperty(da, "Opacity");
            sb.Children.Add(da);

            if (transform == AnimationTransform.Scale)
            {
                var da2 = new DoubleAnimation() { From = 0, To = 1, Duration = duration };
                Storyboard.SetTargetProperty(da2, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");

                var da3 = new DoubleAnimation() { From = 0, To = 1, Duration = duration };
                Storyboard.SetTargetProperty(da3, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");

                sb.Children.Add(da2);
                sb.Children.Add(da3);
            }
            else if (transform == AnimationTransform.Rotation)
            {
                var da2 = new DoubleAnimation() { To = 0, Duration = duration };
                Storyboard.SetTargetProperty(da2, "(UIElement.RenderTransform).(RotateTransform.Angle)");
                sb.Children.Add(da2);
            }

            if (indexDelay)
            {
                foreach (var anim in sb.Children)
                    PlotElementAnimation.SetIndexDelay(anim, 0.5);
            }

            if (easing != Easing.None)
            {
                EasingFunctionBase ef = null;

                switch (easing)
                {
                    case Easing.BackEase:
                        ef = new BackEase();
                        break;
                    case Easing.BounceEase:
                        ef = new BounceEase();
                        break;
                    case Easing.CircleEase:
                        ef = new CircleEase();
                        break;
                    case Easing.CubicEase:
                        ef = new CubicEase();
                        break;
                    case Easing.ElasticEase:
                        ef = new ElasticEase();
                        break;
                    case Easing.ExponentialEase:
                        ef = new ExponentialEase();
                        break;
                    case Easing.PowerEase:
                        ef = new PowerEase();
                        break;
                    case Easing.QuadraticEase:
                        ef = new QuadraticEase();
                        break;
                    case Easing.QuarticEase:
                        ef = new QuarticEase();
                        break;
                    case Easing.QuinticEase:
                        ef = new QuinticEase();
                        break;
                    case Easing.SineEase:
                        ef = new SineEase();
                        break;
                    default:
                        break;
                }

                foreach (DoubleAnimation anim in sb.Children)
                    anim.EasingFunction = ef;
            }

            return new PlotElementAnimation() { Storyboard = sb, SymbolStyle = style };
        }

        void OnPieAnimation(object sender, RoutedEventArgs e)
        {
            //GoTo(new PieAnimation(), "饼图动画");
        }
    }

    public enum AnimationOrigin
    {
        Center,
        Top,
        Left,
        Bottom,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public enum Easing
    {
        None,
        BackEase,
        BounceEase,
        CircleEase,
        CubicEase,
        ElasticEase,
        ExponentialEase,
        PowerEase,
        QuadraticEase,
        QuarticEase,
        QuinticEase,
        SineEase
    }

    public enum AnimationTransform
    {
        Opacity,
        Scale,
        Rotation
    }
}