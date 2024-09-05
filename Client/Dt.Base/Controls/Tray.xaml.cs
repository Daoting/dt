#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-06-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 托盘区域按钮
    /// </summary>
    public partial class Tray : UserControl
    {
        #region 静态内容
        public readonly static DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(Icons),
            typeof(Tray),
            new PropertyMetadata(default(Icons), OnIconChanged));

        public readonly static DependencyProperty DigitProperty = DependencyProperty.Register(
            "Digit",
            typeof(int),
            typeof(Tray),
            new PropertyMetadata(0, OnDigitChanged));

        static void OnDigitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Tray)d).OnDigitChanged();
        }

        static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tray = (Tray)d;
            tray._tbIcon.Text = Res.GetIconChar((Icons)e.NewValue);
        }
        #endregion

        public Tray()
        {
            InitializeComponent();
            MinHeight = 40;
        }

        /// <summary>
        /// 点击事件
        /// </summary>
#if ANDROID
        new
#endif
        public event Action<Tray> Click;

        /// <summary>
        /// 获取设置按钮图标
        /// </summary>
        public Icons Icon
        {
            get { return (Icons)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 醒目提示的数字
        /// </summary>
        public int Digit
        {
            get { return (int)GetValue(DigitProperty); }
            set { SetValue(DigitProperty, value); }
        }

        /// <summary>
        /// 提醒
        /// </summary>
        public void Notice()
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromMilliseconds(400),
            };

            Storyboard.SetTarget(animation, _rotate);
            Storyboard.SetTargetProperty(animation, "Angle");

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        void OnDigitChanged()
        {
            if (Digit > 0)
            {
                var grid = new Grid { Children = { new Ellipse { Fill = Res.RedBrush, Width = 14, Height = 14 } } };
                var tb = new TextBlock
                {
                    Text = Digit > 99 ? "┅" : Digit.ToString(),
                    Foreground = Res.WhiteBrush,
                    FontSize = 10,
                    TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                grid.Children.Add(tb);
                _pre.Content = grid;
            }
            else
            {
                _pre.Content = null;
            }
            Notice();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            _grid.Background = Res.中黄;
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            _grid.Background = Res.深黄;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            _grid.Background = Res.中黄;
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            _grid.Background = Res.TransparentBrush;
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            Click?.Invoke(this);
        }
    }
}
