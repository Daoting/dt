#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Dt.Base.ListView;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 下拉刷新相关
    /// </summary>
    public partial class Lv
    {
        #region 静态内容
        public static readonly DependencyProperty PullToRefreshProperty = DependencyProperty.Register(
            "PullToRefresh",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(false, OnReloadPullToRefresh));

        static void OnReloadPullToRefresh(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if IOS || ANDROID
            ((Lv)d).TogglePullToRefresh();
#endif
        }
        #endregion

        #region 成员变量
        // 阈值
        const double _threshold = 80;
        double _deltaY;
        bool _isPullPressed;
        Point _lastPullPoint;
        PullToRefreshDlg _pullDlg;
        PullToRefreshState _pullState;
        #endregion

        /// <summary>
        /// 下拉刷新事件
        /// </summary>
        public event EventHandler<AsyncEventArgs> RefreshRequested;

        /// <summary>
        /// 获取设置是否允许下拉以刷新，默认false
        /// </summary>
        public bool PullToRefresh
        {
            get { return (bool)GetValue(PullToRefreshProperty); }
            set { SetValue(PullToRefreshProperty, value); }
        }

        /// <summary>
        /// 请求下拉刷新过程
        /// </summary>
        public async Task RequestRefresh()
        {
            if (_pullState != PullToRefreshState.Refreshing)
            {
                await DoRequestRefresh();
            }
        }

        void LoadPullToRefresh()
        {
            if (PullToRefresh)
                AttachPullToRefresh();
        }

        void TogglePullToRefresh()
        {
            if (_root != null)
            {
                if (PullToRefresh)
                    AttachPullToRefresh();
                else
                    DetachPullToRefresh();
            }
        }

        void AttachPullToRefresh()
        {
            _root.AddHandler(PointerPressedEvent, new PointerEventHandler(OnRootPointerPressed), true);
            _root.AddHandler(PointerMovedEvent, new PointerEventHandler(OnRootPointerMoved), true);
            _root.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnRootPointerReleased), true);
        }

        void DetachPullToRefresh()
        {
            _root.RemoveHandler(PointerPressedEvent, new PointerEventHandler(OnRootPointerPressed));
            _root.RemoveHandler(PointerMovedEvent, new PointerEventHandler(OnRootPointerMoved));
            _root.RemoveHandler(PointerReleasedEvent, new PointerEventHandler(OnRootPointerReleased));
        }

        void OnRootPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_pullState == PullToRefreshState.Idle)
            {
                _isPullPressed = true;
                _lastPullPoint = e.GetCurrentPoint(null).Position;
                _pullState = PullToRefreshState.Interacting;
            }
        }

        void OnRootPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isPullPressed)
                return;

            var pt = e.GetCurrentPoint(null).Position;
            double y = pt.Y - _lastPullPoint.Y;
            _lastPullPoint = pt;

            if (y < 0 || Scroll.VerticalOffset > 0)
            {
                _deltaY = 0;
                ClosePullDlg();
            }
            else
            {
                _deltaY += y;
                GetPullDlg().ShowInteracting(_deltaY);
            }
        }

        async void OnRootPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_isPullPressed)
                return;

            _pullState = _deltaY < _threshold ? PullToRefreshState.CancelRefresh : PullToRefreshState.Refreshing;
            if (_pullState == PullToRefreshState.Refreshing)
            {
                await DoRequestRefresh();
            }
            else
            {
                ClosePullDlg();
            }

            _pullState = PullToRefreshState.Idle;
            _isPullPressed = false;
            _deltaY = 0;
        }

        async Task DoRequestRefresh()
        {
            GetPullDlg().ShowRefreshing();
            await OnRefreshRequested();
            ClosePullDlg();
        }

        void ClosePullDlg()
        {
            if (_pullDlg != null)
            {
                _pullDlg.Close();
                _pullDlg = null;
            }
        }

        PullToRefreshDlg GetPullDlg()
        {
            if (_pullDlg == null)
                _pullDlg = new PullToRefreshDlg(this);
            return _pullDlg;
        }

        /// <summary>
        /// 触发下拉刷新事件
        /// </summary>
        /// <returns></returns>
        async Task<bool> OnRefreshRequested()
        {
            if (RefreshRequested != null)
            {
                var args = new AsyncEventArgs();
                RefreshRequested(this, args);
                await args.EnsureAllCompleted();
                return true;
            }
            return false;
        }
    }
}