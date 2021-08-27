#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-05-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 只读信息格，始终只读
    /// </summary>
    public partial class CTip : FvCell
    {
        #region 静态内容
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
            "Format",
            typeof(string),
            typeof(CTip),
            new PropertyMetadata(null, OnFormatChanged));

        public readonly static DependencyProperty ChildProperty = DependencyProperty.Register(
            "Child",
            typeof(FrameworkElement),
            typeof(CTip),
            new PropertyMetadata(null));

        static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CTip tip = (CTip)d;
            if (tip._isLoaded)
            {
                var tb = (TextBlock)tip.GetTemplateChild("Block");
                if (tb != null)
                {
                    tb.ClearValue(TextBlock.TextProperty);
                    tb.SetBinding(TextBlock.TextProperty, tip.ValBinding);
                }
            }
        }
        #endregion

        TextBlock _tb;

        #region 构造方法
        public CTip()
        {
            DefaultStyleKey = typeof(CTip);
            ValConverter = new TipValConverter(this);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 点击事件
        /// </summary>
#if ANDROID
        new
#endif
        public event TappedEventHandler Click
        {
            add
            {
                Grid g = Child as Grid;
                if (g == null)
                {
                    g = LoadInteractiveChild();
                }
                g.AddHandler(TappedEvent, value, true);
            }
            remove
            {
                if (Child is Grid g)
                {
                    g.RemoveHandler(TappedEvent, value);
                    LoadTextChild();
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取设置格式串，时间格式如：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [CellParam("格式串")]
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        /// <summary>
        /// 获取设置单元格内容
        /// </summary>
        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        #region 重写方法
        protected override void OnApplyCellTemplate()
        {
            if (Child == null)
                LoadTextChild();
        }

        protected override void SetValBinding()
        {
            _tb?.SetBinding(TextBlock.TextProperty, ValBinding);
        }
        #endregion

        #region 动态内容
        void LoadTextChild()
        {
            Child = CreateTextBlock();
            if (_isLoaded)
                SetValBinding();
        }

        TextBlock CreateTextBlock()
        {
            _tb = new TextBlock
            {
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center,
                IsTextSelectionEnabled = true,
                TextWrapping = TextWrapping.Wrap,
            };
            return _tb;
        }

        Grid LoadInteractiveChild()
        {
            Grid g = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                Background = Res.TransparentBrush,
            };
            g.Children.Add(CreateTextBlock());

            var tb = new TextBlock
            {
                Text = "\uE011",
                FontFamily = Res.IconFont,
                FontSize = 20,
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            Grid.SetColumn(tb, 1);
            g.Children.Add(tb);

            g.PointerEntered += OnGridPointerEntered;
            g.PointerPressed += OnGridPointerPressed;
            g.PointerReleased += OnGridPointerReleased;
            g.PointerExited += OnGridPointerExited;
            g.PointerCaptureLost += OnGridPointerExited;
            Child = g;

            if (_isLoaded)
                SetValBinding();
            return g;
        }

        void OnGridPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (Child is Grid g)
                g.Background = Res.暗遮罩;
        }

        void OnGridPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var props = e.GetCurrentPoint(null).Properties;
            if (props.IsLeftButtonPressed)
            {
                if (Child is Grid g)
                    g.Background = Res.深暗遮罩;
            }
        }

        void OnGridPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (Child is Grid g)
                g.Background = Res.暗遮罩;
        }

        void OnGridPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (Child is Grid g)
                g.Background = Res.TransparentBrush;
        }
        #endregion
    }

    /// <summary>
    /// 源CTip.Data，目标TextBlock.Text
    /// </summary>
    class TipValConverter : IValueConverter
    {
        CTip _owner;

        public TipValConverter(CTip p_owner)
        {
            _owner = p_owner;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "";

            if (value is DateTime dt)
            {
                try
                {
                    if (string.IsNullOrEmpty(_owner.Format))
                        return dt.ToString("yyyy-MM-dd");
                    return dt.ToString(_owner.Format);
                }
                catch { }
            }
            else if (!string.IsNullOrEmpty(_owner.Format) && value is IFormattable f)
            {
                try
                {
                    return f.ToString(_owner.Format, null);
                }
                catch { }
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}