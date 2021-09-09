#region 引用命名
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 查询文本框
    /// </summary>
    public partial class SearchBox : DtControl
    {
        public static readonly DependencyProperty IsRealtimeProperty = DependencyProperty.Register(
            "IsRealtime",
            typeof(bool),
            typeof(SearchBox),
            new PropertyMetadata(false, OnIsRealtimeChanged));

        static void OnIsRealtimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = (SearchBox)d;
            if (box._timer == null)
            {
                var timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(300);
                timer.Tick += box.OnTimerTick;
                box._timer = timer;
            }
        }

        DispatcherTimer _timer;
        readonly TextBox _tb;

        public SearchBox()
        {
            DefaultStyleKey = typeof(SearchBox);

            _tb = new TextBox
            {
                BorderThickness = new Thickness(0),
                Padding = new Thickness(10, 7, 10, 7),
                CornerRadius = new CornerRadius(10)
            };
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<string> Search;

        /// <summary>
        /// 获取设置查询框提示内容
        /// </summary>
        public string Placeholder
        {
            get { return _tb.PlaceholderText; }
            set { _tb.PlaceholderText = value; }
        }

        /// <summary>
        /// 获取设置文本内容变化时是否执行查询，默认false
        /// </summary>
        public bool IsRealtime
        {
            get { return (bool)GetValue(IsRealtimeProperty); }
            set { SetValue(IsRealtimeProperty, value); }
        }

        /// <summary>
        /// 获取查询内容
        /// </summary>
        public string Text
        {
            get
            {
                if (_tb != null)
                    return _tb.Text;
                return null;
            }
        }

        protected override void OnLoadTemplate()
        {
            var bd = (Border)GetTemplateChild("Border");
            bd.Child = _tb;
            _tb.TextChanged += OnTextChanged;
            _tb.FirstLoaded(() => _tb.Focus(FocusState.Programmatic));
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                OnSearch();
            }
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsRealtime)
                _timer.Start();
        }

        void OnTimerTick(object sender, object e)
        {
            _timer.Stop();
            OnSearch();
        }

        void OnSearch()
        {
            Search?.Invoke(this, _tb.Text);
        }
    }
}
