#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
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
        uint? _captureID;

        public NotifyItem(NotifyInfo p_info)
        {
            InitializeComponent();

            _info = p_info;
            _grid.Background = _info.NotifyType == NotifyType.Information ? Res.BlackBrush : Res.RedBrush;
            _grid.PointerEntered += OnPointerEntered;
            _grid.PointerPressed += OnPointerPressed;
            _grid.PointerReleased += OnPointerReleased;
            _grid.PointerExited += OnPointerExited;

            _tb.Text = _info.Message;
            _info.Close = CloseInternal;
            if (!string.IsNullOrEmpty(_info.Link))
            {
                Button btn = new Button { Content = _info.Link, Style = Res.浅色按钮, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 10, 0, 0) };
                if (_info.LinkCallback != null)
                    btn.Click += (s, e) => _info.LinkCallback(_info);
                _sp.Children.Add(btn);
            }

            if (_info.DelaySeconds > 0)
                StartAutoClose();

            // 动画，uno暂时未实现
            TransitionCollection tc = new TransitionCollection();
            tc.Add(new EdgeUIThemeTransition { Edge = Kit.IsPhoneUI ? EdgeTransitionLocation.Top : EdgeTransitionLocation.Right });
            _grid.Transitions = tc;
        }

        /// <summary>
        /// 启动自动关闭定时器
        /// </summary>
        void StartAutoClose()
        {
            KillCloseTimer();
            _timerAutoClose = ThreadPoolTimer.CreateTimer(OnTimerHandler, TimeSpan.FromSeconds(_info.DelaySeconds));
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
            KillCloseTimer();
            Height = _grid.ActualHeight;
            Content = null;
            // 保证动画播放完毕
            await Task.Delay(200);
            SysVisual.NotifyList.Remove(_info);
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
                e.Handled = true;
                _captureID = e.Pointer.PointerId;
                _rc.Fill = Res.深亮遮罩;
            }
        }

        /// <summary>
        /// 点击内容关闭提示框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_captureID != e.Pointer.PointerId)
                return;

            _grid.ReleasePointerCapture(e.Pointer);
            e.Handled = true;
            _captureID = null;
            if (_grid.ContainPoint(e.GetCurrentPoint(null).Position))
                CloseInternal();
            else
                _rc.Fill = null;
        }

        /// <summary>
        /// 指针离开时启动自动关闭定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (_info.DelaySeconds > 0)
                StartAutoClose();
            _rc.Fill = null;
        }
    }
}
