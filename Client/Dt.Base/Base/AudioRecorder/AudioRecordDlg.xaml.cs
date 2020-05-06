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
    public sealed partial class AudioRecordDlg : Dlg
    {
        public AudioRecordDlg()
        {
            InitializeComponent();
            HideTitleBar = true;
            IsPinned = true;
            PhonePlacement = DlgPlacement.CenterScreen;
            WinPlacement = DlgPlacement.TargetCenter;
            StartTimer();
        }

        /// <summary>
        /// 录音时长
        /// </summary>
        public string Duration
        {
            get { return _tbTimer.Text; }
        }

        void OnSend(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Close(true);
        }

        void OnCancel(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Close();
        }

        #region 定时器
        DispatcherTimer _timer;
        int _seconds;

        void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        void OnTimerTick(object sender, object e)
        {
            TimeSpan ts = TimeSpan.FromSeconds(++_seconds);
            _tbTimer.Text = $"{ts.Minutes.ToString("D2")}:{ts.Seconds.ToString("D2")}";
        }
        #endregion
    }
}
