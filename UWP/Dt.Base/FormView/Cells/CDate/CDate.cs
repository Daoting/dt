#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using Dt.Core.Mask;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 时间格
    /// </summary>
    public partial class CDate : FvCell
    {
        #region 静态内容
        /// <summary>
        /// 格式串
        /// </summary>
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
            "Format",
            typeof(string),
            typeof(CDate),
            new PropertyMetadata("yyyy-MM-dd", OnFormatChanged));

        /// <summary>
        /// 触摸模式选择器
        /// </summary>
        public static readonly DependencyProperty AlwaysTouchPickerProperty = DependencyProperty.Register(
            "AlwaysTouchPicker",
            typeof(bool),
            typeof(CDate),
            new PropertyMetadata(false));

        /// <summary>
        /// 日期时间
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(DateTime),
            typeof(CDate),
            new PropertyMetadata(DateTime.MinValue));

        static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CDate c = (CDate)d;

            // 确定格式
            string format = ((string)e.NewValue).ToLower();
            int dateIndex = format.IndexOf("yy");
            int timeIndex = format.IndexOf("hh");
            if (dateIndex > -1)
            {
                if (timeIndex > -1)
                {
                    c._format = DateFormatType.DateTime;
                    c._dateInTail = timeIndex < dateIndex;
                }
                else
                {
                    c._format = DateFormatType.Date;
                }
            }
            else if (timeIndex > -1)
            {
                c._format = DateFormatType.Time;
            }
            else
            {
                throw new Exception("日期时间格式串错误！");
            }
        }
        #endregion

        #region 成员变量
        DateFormatType _format = DateFormatType.Date;
        // 格式为日期时间时，日期是否在尾部
        bool _dateInTail;
        Grid _grid;
        CalendarDlg _dlg;
        #endregion

        #region 构造方法
        public CDate()
        {
            DefaultStyleKey = typeof(CDate);
            ValConverter = new DateValConverter();
        }
        #endregion

        /// <summary>
        /// 获取设置格式串，默认 yyyy-MM-dd，完整如：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [CellParam("格式串")]
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        /// <summary>
        /// 获取设置是否始终为触摸模式选择器，默认 false
        /// </summary>
        [CellParam("触摸模式选择器")]
        public bool AlwaysTouchPicker
        {
            get { return (bool)GetValue(AlwaysTouchPickerProperty); }
            set { SetValue(AlwaysTouchPickerProperty, value); }
        }

        /// <summary>
        /// 获取设置日期时间
        /// </summary>
        public DateTime Value
        {
            get { return (DateTime)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }


        #region 重写方法
        protected override void OnApplyCellTemplate()
        {
            LoadContent();
        }

        protected override void SetValBinding()
        {
            SetBinding(ValueProperty, ValBinding);
        }

        protected override void OnReadOnlyChanged()
        {
            if (_grid != null && _grid.Background == null)
            {
                foreach (var elem in _grid.Children)
                {
                    if (elem is MaskBox mb)
                        mb.Box.IsReadOnly = ReadOnlyBinding;
                    else if (elem is Button btn)
                        btn.Visibility = ReadOnlyBinding ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        protected override bool SetFocus()
        {
            if (_grid != null && _grid.Background == null)
                return ((MaskBox)_grid.Children[0]).Focus(FocusState.Programmatic);
            return true;
        }
        #endregion

        void LoadContent()
        {
            if (_panel == null)
                return;

            _panel.Child = null;
            _grid = new Grid();
            if (Kit.IsPhoneUI || AlwaysTouchPicker)
            {
                // 触摸模式
                _grid.Background = Res.TransparentBrush;
                TextBlock tb = new TextBlock { Margin = new Thickness(10, 0, 10, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
                var bind = new Binding
                {
                    Path = new PropertyPath("Value"),
                    Converter = new DateValUIConverter(this),
                    Source = this
                };
                tb.SetBinding(TextBlock.TextProperty, bind);
                _grid.Children.Add(tb);

                if (_format == DateFormatType.Date)
                    _grid.Tapped += OnDateFly;
                else if (_format == DateFormatType.Time)
                    _grid.Tapped += OnTimeFly;
                else
                    _grid.Tapped += OnDateTimeFly;
            }
            else
            {
                // 非触摸模式
                MaskBox box = new MaskBox { MaskType = MaskType.DateTimeAdvancingCaret, AllowNullInput = true, UseAsDisplayFormat = true };
                var bind = new Binding
                {
                    Path = new PropertyPath("Format"),
                    Source = this
                };
                box.SetBinding(MaskBox.MaskProperty, bind);
                bind = new Binding
                {
                    Path = new PropertyPath("Value"),
                    Mode = BindingMode.TwoWay,
                    Converter = new ValMaskConverter(),
                    Source = this
                };
                box.SetBinding(MaskBox.ValueProperty, bind);
                _grid.Children.Add(box);

                if (_format != DateFormatType.Time)
                {
                    _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) });

                    Button btn = new Button { Style = Res.字符按钮, Content = "\uE047" };
                    Grid.SetColumn(btn, 1);
                    btn.Click += OnShowCalendar;
                    _grid.Children.Add(btn);
                }
            }
            _panel.Child = _grid;
        }

        #region 触摸模式
        void OnDateFly(object sender, TappedRoutedEventArgs e)
        {
            if (ReadOnlyBinding)
                return;

            // 日期选择器
            DatePickerFlyout fly = new DatePickerFlyout();
            fly.Date = Value;
            fly.DatePicked += (s, args) => Value = args.NewDate.DateTime;
            fly.ShowAt(_grid);
        }

        void OnTimeFly(object sender, TappedRoutedEventArgs e)
        {
            if (ReadOnlyBinding)
                return;

            // 时间选择器
            TimePickerFlyout tf = new TimePickerFlyout();
            tf.Time = Value - Value.Date;
            tf.TimePicked += (s, args) => Value = Value.Date + args.NewTime;
            tf.ShowAt(_grid);
        }

        void OnDateTimeFly(object sender, TappedRoutedEventArgs e)
        {
            if (ReadOnlyBinding)
                return;

            // uno无法给出相对位置
            TextBlock tb = _grid.Children[0] as TextBlock;
            var pt = e.GetPosition(null);

            // 无文本内容时按Grid计算
            MatrixTransform trans = string.IsNullOrEmpty(tb.Text) ? _grid.TransformToVisual(null) as MatrixTransform : tb.TransformToVisual(null) as MatrixTransform;
            if (trans == null)
                return;

            bool showDateFly = (pt.X <= trans.Matrix.OffsetX + tb.ActualWidth / 2 + 10) && !_dateInTail;
            if (showDateFly)
            {
                // 日期选择器
                DatePickerFlyout fly = new DatePickerFlyout();
                fly.Date = Value;
                fly.DatePicked += (s, args) => Value = args.NewDate.DateTime.Date + (Value - Value.Date);
                fly.ShowAt(_grid);
            }
            else
            {
                // 时间选择器
                TimePickerFlyout tf = new TimePickerFlyout();
                tf.Time = Value - Value.Date;
                tf.TimePicked += (s, args) => Value = Value.Date + args.NewTime;
                tf.ShowAt(_grid);
            }
        }
        #endregion

        void OnShowCalendar(object sender, RoutedEventArgs e)
        {
            if (_dlg != null && _dlg.IsOpened)
                return;

            if (_dlg == null)
            {
                _dlg = new CalendarDlg
                {
                    Owner = this,
                    WinPlacement = DlgPlacement.TargetBottomLeft,
                    PlacementTarget = _grid,
                    ClipElement = (Button)sender,
                    HideTitleBar = true,
                    Resizeable = false,
                    // 不向下层对话框传递Press事件
                    AllowRelayPress = false,
                };
            }
            _dlg.ShowDlg();
        }

        enum DateFormatType
        {
            DateTime,
            Date,
            Time
        }
    }
}