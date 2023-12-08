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
using Microsoft.UI.Xaml.Controls;
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
            ((Lv)d).TogglePullToRefresh();
        }
        #endregion

        /// <summary>
        /// 下拉刷新事件
        /// </summary>
        public event Action<AsyncArgs> RefreshRequested;

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
        public void RequestRefresh()
        {
            if (IsInnerScroll
                && PullToRefresh
                && _root.Child is RefreshContainer rc)
            {
                rc.RequestRefresh();
            }
        }

        void TogglePullToRefresh()
        {
            // 只有滚动栏在内部时支持下拉刷新
            if (!IsInnerScroll)
                return;

            if (PullToRefresh)
            {
                if (_root.Child is ScrollViewer sv)
                {
                    _root.Child = null;

#if WIN
                    // 不可反复切换
                    //sv.Content = null;
                    //var svNew = new ScrollViewer { Content = _panel };
                    //svNew.HorizontalScrollMode = sv.HorizontalScrollMode;
                    //svNew.HorizontalScrollBarVisibility = sv.HorizontalScrollBarVisibility;
                    //svNew.VerticalScrollMode = sv.VerticalScrollMode;
                    //svNew.VerticalScrollBarVisibility = sv.VerticalScrollBarVisibility;
                    //sv = svNew;
#endif

                    var rc = new RefreshContainer { Content = sv };
                    _root.Child = rc;
                    rc.RefreshRequested += OnRefreshRequested;
                }
            }
            else if (_root.Child is RefreshContainer rc)
            {
                rc.RefreshRequested -= OnRefreshRequested;
                var sv = rc.Content as ScrollViewer;
                rc.Content = null;
                _root.Child = sv;
            }
        }

        /// <summary>
        /// 触发下拉刷新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnRefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs e)
        {
            using (var RefreshCompletionDeferral = e.GetDeferral())
            {
                if (RefreshRequested != null)
                {
                    var args = new AsyncArgs();
                    RefreshRequested(args);
                    await args.EnsureAllCompleted();
                }
            }
        }
    }
}