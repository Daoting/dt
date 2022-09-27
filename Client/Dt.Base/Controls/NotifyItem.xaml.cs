#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System.ComponentModel;
using Windows.Foundation;
using Windows.System.Threading;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 提示信息项
    /// </summary>
    public sealed partial class NotifyItem : UserControl
    {
        NotifyInfo _info;
        ThreadPoolTimer _timerAutoClose;
        Point? _ptStart;
        bool _disposed;

        public NotifyItem(NotifyInfo p_info)
        {
            InitializeComponent();

            _info = p_info;
            _info.PropertyChanged += OnInfoChanged;
            _info.Close = CloseInternal;

            FontSize = 16;
            _grid.PointerEntered += OnPointerEntered;
            _grid.PointerPressed += OnPointerPressed;
            _grid.PointerReleased += OnPointerReleased;
            _grid.PointerExited += OnPointerExited;

            _grid.Background = _info.NotifyType == NotifyType.Information ? Res.BlackBrush : Res.RedBrush;
            _tb.Text = _info.Message;
            CreateLink();

            if (_info.Delay > 0)
                StartAutoClose();

#if WIN
            // 动画，uno暂时未实现
            TransitionCollection tc = new TransitionCollection();
            tc.Add(new EdgeUIThemeTransition { Edge = Kit.IsPhoneUI ? EdgeTransitionLocation.Top : EdgeTransitionLocation.Right });
            _grid.Transitions = tc;
#else
            RenderTransform = new TranslateTransform();
#endif
        }

        void OnInfoChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Message")
            {
                Kit.RunAsync(() => _tb.Text = _info.Message);
            }
            else if (e.PropertyName == "NotifyType")
            {
                Kit.RunAsync(() => _grid.Background = _info.NotifyType == NotifyType.Information ? Res.BlackBrush : Res.RedBrush);
            }
            else if (e.PropertyName == "Link")
            {
                Kit.RunAsync(() =>
                {
                    if (_sp.Children.Count > 1)
                        _sp.Children.RemoveAt(1);
                    CreateLink();
                });
            }
            else if (e.PropertyName == "Delay")
            {
                Kit.RunAsync(() =>
                {
                    KillCloseTimer();
                    if (_info.Delay > 0)
                        StartAutoClose();
                });
            }
        }

        void CreateLink()
        {
            if (!string.IsNullOrEmpty(_info.Link))
            {
                Button btn = new Button { Content = _info.Link, Style = Res.浅色按钮, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 10, 0, 0) };
                btn.Click += (s, e) => _info.LinkCallback?.Invoke(_info);
                _sp.Children.Add(btn);
            }
        }

        /// <summary>
        /// 启动自动关闭定时器
        /// </summary>
        void StartAutoClose()
        {
            KillCloseTimer();
            _timerAutoClose = ThreadPoolTimer.CreateTimer(OnTimerHandler, TimeSpan.FromSeconds(_info.Delay));
        }

        void KillCloseTimer()
        {
            if (_timerAutoClose != null)
            {
                _timerAutoClose.Cancel();
                _timerAutoClose = null;
            }
        }

        void OnTimerHandler(ThreadPoolTimer p_timer)
        {
            Kit.RunAsync(() => CloseInternal());
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        async void CloseInternal()
        {
            if (_disposed)
                return;

            _disposed = true;
            KillCloseTimer();
#if WIN
            Height = _grid.ActualHeight;
            Content = null;
            // 保证动画播放完毕
            await Task.Delay(200);
            Stub.Inst.NotifyList.Remove(_info);
#else
            var trans = (TranslateTransform)RenderTransform;
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            Storyboard.SetTarget(da, trans);
            Storyboard.SetTargetProperty(da, "Y");
            da.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            da.From = 0;
            da.To = -ActualHeight;
            da.EasingFunction = new QuadraticEase();
            da.EnableDependentAnimation = true;
            sb.Children.Add(da);
            sb.Begin();
            sb.Completed += (sender, e) => Stub.Inst.NotifyList.Remove(_info);
            await Task.CompletedTask;
#endif
        }

        /// <summary>
        /// 指针进入时取消自动关闭定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            KillCloseTimer();
            _rc.Fill = Res.亮遮罩;
        }

        /// <summary>
        /// 显示按下状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_grid.CapturePointer(e.Pointer))
            {
                // 因phone不触发PointerEntered，再次关闭定时器
                KillCloseTimer();
                e.Handled = true;
                _rc.Fill = Res.深亮遮罩;
                _ptStart = e.GetCurrentPoint(null).Position;
            }
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _grid.ReleasePointerCapture(e.Pointer);
            e.Handled = true;
            var pt = e.GetCurrentPoint(null).Position;
            if (_info.Delay >= 0
                && Math.Abs(_ptStart.Value.X - pt.X) < 6
                && Math.Abs(_ptStart.Value.Y - pt.Y) < 6)
            {
                CloseInternal();
            }
            else
            {
                _rc.Fill = null;
            }
        }

        /// <summary>
        /// 指针离开时启动自动关闭定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (_info.Delay > 0)
                StartAutoClose();
            _rc.Fill = null;
        }
    }
}
