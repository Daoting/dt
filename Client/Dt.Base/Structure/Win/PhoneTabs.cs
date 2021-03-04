﻿#region 文件描述
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
        bool _isDragging;
        #endregion

        public PhoneTabs()
        {
            DefaultStyleKey = typeof(PhoneTabs);

            _grid = new Grid { BorderThickness = new Thickness(0, 1, 0, 0), BorderBrush = AtRes.浅灰边框, Background = AtRes.浅灰背景 };
            Grid.SetRow(_grid, 1);
            ManipulationMode = ManipulationModes.System | ManipulationModes.TranslateX | ManipulationModes.TranslateInertia;
        }

        /// <summary>
        /// 只有是首页时才有值
        /// </summary>
        internal Win OwnWin { get; set; }

        /// <summary>
        /// 导航时的标识，所有Tab标题逗号隔开
        /// </summary>
        internal string NaviID { get; set; }

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
                ((Tab)btn.DataContext).PinButtonVisibility = Visibility.Collapsed;
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
            if (OwnWin != null)
                return OwnWin.AllowClose();
            return Task.FromResult(true);
        }

        /// <summary>
        /// 关闭或后退之后
        /// </summary>
        void IPhonePage.OnClosed()
        {
            // 只在首页时处理
            if (OwnWin != null)
                OwnWin.AfterClosed();
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
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);
            if (!e.Handled && _selected != null)
            {
                // 不可在此处重置RenderTransform，uno中当内部包含ScollViewer且内嵌面板有背景色时会造成delta莫名变大！很难发现问题原因
                _isDragging = true;
                e.Handled = true;
            }
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            base.OnManipulationDelta(e);
            if (_isDragging)
            {
                e.Handled = true;
                var trans = (TranslateTransform)((UIElement)_selected.DataContext).RenderTransform;
                trans.X += e.Delta.Translation.X;
                if (e.IsInertial)
                {
                    e.Complete();
                    SwitchPage();
                    _isDragging = false;
                }
            }
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            if (_isDragging)
            {
                e.Handled = true;
                var con = (FrameworkElement)_selected.DataContext;
                if (Math.Abs(((TranslateTransform)con.RenderTransform).X) > con.ActualWidth / 2)
                    SwitchPage();
                else
                    CancelPaging();
                _isDragging = false;
            }
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
        #endregion
    }
}