#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// PhoneUI时的TabControl，内部用，为uno节省一级ContentPresenter！
    /// </summary>
    public partial class PhoneTabs : Control, IPhonePage
    {
        #region 成员变量
        Grid _root;
        readonly Grid _grid;
        Button _selected;
        #endregion

        public PhoneTabs()
        {
            DefaultStyleKey = typeof(PhoneTabs);

            _grid = new Grid { BorderThickness = new Thickness(0, 1, 0, 0), BorderBrush = Res.浅灰2, Background = Res.浅灰1 };
            Grid.SetRow(_grid, 1);
            //ManipulationMode = ManipulationModes.System | ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
        }

        /// <summary>
        /// 所属Win
        /// </summary>
        internal Win OwnWin
        {
            get
            {
                if (_grid == null || _grid.Children.Count == 0)
                    return null;

                return ((Tab)(((Button)_grid.Children[0]).DataContext)).OwnWin;
            }
        }

        /// <summary>
        /// 导航时的标识，所有Tab标题逗号隔开
        /// </summary>
        internal string NaviID { get; set; }

        /// <summary>
        /// 是否为首页
        /// </summary>
        internal bool IsHome { get; set; }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="p_tab"></param>
        internal void AddItem(Tab p_tab)
        {
            _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Button btn = new Button();
            btn.Style = (Style)Application.Current.Resources["PhoneTabsBtn"];
            btn.DataContext = p_tab;
            btn.Click += OnBtnClick;
            btn.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetColumn(btn, _grid.ColumnDefinitions.Count - 1);
            _grid.Children.Add(btn);
        }

        /// <summary>
        /// 隐藏所有标签的返回按钮
        /// </summary>
        internal void HideBackButton()
        {
            foreach (var btn in _grid.Children.OfType<Button>())
            {
                ((Tab)btn.DataContext).BackButtonVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 选择指定索引的标签
        /// </summary>
        /// <param name="p_index"></param>
        public void Select(int p_index)
        {
            if (p_index >= 0 && p_index < _grid.Children.Count)
                SelectItem((Button)_grid.Children[p_index]);
        }

        /// <summary>
        /// 选择指定标题的标签
        /// </summary>
        /// <param name="p_title"></param>
        public void Select(string p_title)
        {
            foreach (var btn in _grid.Children.OfType<Button>())
            {
                if (((Tab)btn.DataContext).Title == p_title)
                {
                    SelectItem(btn);
                    break;
                }
            }
        }

        #region 实现接口
        /// <summary>
        /// 关闭或后退之前，返回false表示禁止关闭
        /// </summary>
        /// <returns>true 表允许关闭</returns>
        Task<bool> IPhonePage.OnClosing()
        {
            if (IsHome)
            {
                var win = OwnWin;
                if (win != null)
                    return win.AllowClose();
            }
            return Task.FromResult(true);
        }

        /// <summary>
        /// 关闭或后退之后
        /// </summary>
        void IPhonePage.OnClosed()
        {
            // 只在首页时处理
            if (IsHome)
                OwnWin?.AfterClosed();
        }
        #endregion

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 为uno节省一级ContentPresenter！
            _root = (Grid)GetTemplateChild("RootGrid");
            _root.Children.Add(_grid);
            if (_selected != null)
                SelectItem(_selected);
        }

        void OnBtnClick(object sender, RoutedEventArgs e)
        {
            SelectItem((Button)sender);
        }

        void SelectItem(Button p_btn)
        {
            _selected = p_btn;
            if (_root != null)
            {
                if (_root.Children.Count > 1)
                    _root.Children.RemoveAt(1);

                var elem = (UIElement)p_btn.DataContext;
                elem.RenderTransform = new TranslateTransform();
                _root.Children.Add(elem);
                foreach (var btn in _grid.Children.OfType<Button>())
                {
                    btn.IsEnabled = (btn != p_btn);
                }
            }
        }

        #region 左右滑动
        /*  交互效果不好，暂时停用
        SlideState _state;
        ScrollViewer _innerScroll;

        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Touch && !e.Handled && _selected != null)
            {
                // 不可在此处重置RenderTransform，uno中当内部包含ScollViewer且内嵌面板有背景色时会造成delta莫名变大！很难发现问题原因
                e.Handled = true;
                _innerScroll = this.FindChildByType<ScrollViewer>();
                _state = SlideState.Start;
            }
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch
                || e.Delta.Translation.X == 0.0
                || _state == SlideState.LockY
                || _state == SlideState.None)
                return;

            if (_innerScroll != null)
            {
                // 滚动栏垂直方向有移动时不算！保证垂直正常滚动
                if (e.Delta.Translation.Y != 0
                    && _state != SlideState.LockX
                    && _innerScroll.ScrollableHeight > 0
                    && Math.Abs(e.Delta.Translation.X) < Math.Abs(e.Delta.Translation.Y) * 3
                    && ((e.Delta.Translation.Y < 0 && _innerScroll.VerticalOffset < _innerScroll.ScrollableHeight)
                        || (e.Delta.Translation.Y > 0 && _innerScroll.VerticalOffset > 0)))
                {
                    _state = SlideState.LockY;
                    return;
                }

                if (_innerScroll.ScrollableWidth == 0
                    || (e.Delta.Translation.X < 0 && _innerScroll.HorizontalOffset >= _innerScroll.ScrollableWidth)
                    || (e.Delta.Translation.X > 0 && _innerScroll.HorizontalOffset <= 0))
                {
                    SetLockX(e);
                }
            }
            else
            {
                SetLockX(e);
            }
        }

        void SetLockX(ManipulationDeltaRoutedEventArgs e)
        {
            // 水平滑动距离是垂直的n倍
            if (_state == SlideState.Start
                && Math.Abs(e.Delta.Translation.X) > Math.Abs(e.Delta.Translation.Y) * 3)
            {
                _state = SlideState.LockX;
            }

            if (_state == SlideState.LockX)
                ((TranslateTransform)((UIElement)_selected.DataContext).RenderTransform).X += e.Delta.Translation.X;
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Touch && _state == SlideState.LockX)
            {
                e.Handled = true;
                var con = (FrameworkElement)_selected.DataContext;
                double deltaX = ((TranslateTransform)((UIElement)_selected.DataContext).RenderTransform).X;
                if (Math.Abs(deltaX) > con.ActualWidth / 2)
                    SwitchPage();
                else if (deltaX != 0)
                    CancelPaging();
            }
            _state = SlideState.None;
        }

        void SwitchPage()
        {
            var trans = (TranslateTransform)((UIElement)_selected.DataContext).RenderTransform;
            int index = _grid.Children.IndexOf(_selected);
            if (trans.X > 0)
                index = (index <= 0) ? _grid.Children.Count - 1 : index - 1;
            else
                index = (index == _grid.Children.Count - 1) ? 0 : index + 1;

            // 增加动画，平衡切换时间
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            Storyboard.SetTarget(da, trans);
            Storyboard.SetTargetProperty(da, "X");
            da.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            da.From = trans.X;
            var con = (FrameworkElement)_selected.DataContext;
            da.To = (trans.X > 0) ? con.ActualWidth : -con.ActualWidth;
            da.EasingFunction = new QuadraticEase();
            da.EnableDependentAnimation = true;
            sb.Children.Add(da);
            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                ((UIElement)_selected.DataContext).RenderTransform = null;
                SelectItem((Button)_grid.Children[index]);
            };
        }

        /// <summary>
        /// 不换页移动回原位置
        /// </summary>
        void CancelPaging()
        {
            var trans = (TranslateTransform)((UIElement)_selected.DataContext).RenderTransform;
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            Storyboard.SetTarget(da, trans);
            Storyboard.SetTargetProperty(da, "X");
            da.Duration = new Duration(TimeSpan.FromSeconds(0.1));
            da.From = trans.X;
            da.To = 0;
            da.EasingFunction = new QuadraticEase();
            da.EnableDependentAnimation = true;
            sb.Children.Add(da);
            sb.Begin();
            sb.Completed += (sender, e) => ((UIElement)_selected.DataContext).RenderTransform = new TranslateTransform();
        }

        enum SlideState
        {
            None,

            Start,

            /// <summary>
            /// 锁定水平滑动，内部有滚动栏时也可以同步垂直滑动
            /// </summary>
            LockX,

            /// <summary>
            /// 锁定只可内部有滚动栏垂直滑动
            /// </summary>
            LockY
        }
        */
        #endregion
    }
}