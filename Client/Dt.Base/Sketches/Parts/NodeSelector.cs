#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class NodeSelector : Control
    {
        #region 私有变量
        Sketch _owner;
        List<Thumb> _thumbs;
        bool _resized;
        FrameworkElement _target;
        Rect _rectCur;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public NodeSelector()
        {
            DefaultStyleKey = typeof(NodeSelector);
        }

        #region 属性
        /// <summary>
        /// 获取设置节点对象
        /// </summary>
        public FrameworkElement Target
        {
            get { return _target; }
            set
            {
                if (_target != value)
                {
                    _target = value;
                    if (_target != null)
                    {
                        Canvas.SetLeft(this, Canvas.GetLeft(_target));
                        Canvas.SetTop(this, Canvas.GetTop(_target));
                        Width = _target.ActualWidth;
                        Height = _target.ActualHeight;
                    }
                    UpdateSelector(null);
                }
                _owner.LinkClerk.ShowLinkPt(_target as SNode, Canvas.GetLeft(_target), Canvas.GetTop(_target) + Height);
            }
        }

        /// <summary>
        /// 获取设置所属Sketch
        /// </summary>
        internal Sketch Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }
        #endregion

        #region 重写
        /// <summary>
        /// 重写
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_thumbs != null)
            {
                foreach (Thumb thumb in _thumbs)
                {
                    if (thumb != null)
                    {
                        thumb.DragStarted -= OnThumbDragStarted;
                        thumb.DragDelta -= OnThumbDragDelta;
                        thumb.DragCompleted -= OnThumbDragCompleted;
                    }
                }
                _thumbs.Clear();
            }

            Thumb thumb1 = GetTemplateChild("PART_TopLeft") as Thumb;
            Thumb thumb2 = GetTemplateChild("PART_Top") as Thumb;
            Thumb thumb3 = GetTemplateChild("PART_TopRight") as Thumb;
            Thumb thumb4 = GetTemplateChild("PART_Left") as Thumb;
            Thumb thumb5 = GetTemplateChild("PART_Right") as Thumb;
            Thumb thumb6 = GetTemplateChild("PART_BottomLeft") as Thumb;
            Thumb thumb7 = GetTemplateChild("PART_Bottom") as Thumb;
            Thumb thumb8 = GetTemplateChild("PART_BottomRight") as Thumb;
            _thumbs = new List<Thumb> { thumb1, thumb2, thumb3, thumb4, thumb5, thumb6, thumb7, thumb8 };
            foreach (Thumb thumb in _thumbs)
            {
                if (thumb != null)
                {
                    thumb.DragStarted += OnThumbDragStarted;
                    thumb.DragDelta += OnThumbDragDelta;
                    thumb.DragCompleted += OnThumbDragCompleted;
                }
            }
            if(_target != null)
                UpdateSelector(null);
        }
        #endregion

        #region 事件
        void OnThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            _rectCur = new Rect(Canvas.GetLeft(this), Canvas.GetTop(this), Width, Height);
            ShowLine(sender as Thumb);
        }

        void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            UpdateSelector((Thumb)sender);
            ShowLine(sender as Thumb);
            double left = Canvas.GetLeft(_target);
            double top = Canvas.GetTop(_target);
            switch ((Corner)_thumbs.IndexOf(sender as Thumb))
            {
                case Corner.Left:
                    SetLoc(left + e.HorizontalChange, top, Width - e.HorizontalChange, Height);
                    break;
                case Corner.TopLeft:
                    SetLoc(left + e.HorizontalChange, top + e.VerticalChange, Width - e.HorizontalChange, Height - e.VerticalChange);
                    break;
                case Corner.Top:
                    SetLoc(left, top + e.VerticalChange, Width, Height - e.VerticalChange);
                    break;
                case Corner.TopRight:
                    SetLoc(left, top + e.VerticalChange, Width + e.HorizontalChange, Height - e.VerticalChange);
                    break;
                case Corner.Right:
                    SetLoc(left, top, Width + e.HorizontalChange, Height);
                    break;
                case Corner.BottomRight:
                    SetLoc(left, top, Width + e.HorizontalChange, Height + e.VerticalChange);
                    break;
                case Corner.Bottom:
                    SetLoc(left, top, Width, Height + e.VerticalChange);
                    break;
                case Corner.BottomLeft:
                    SetLoc(left + e.HorizontalChange, top, Width - e.HorizontalChange, Height + e.VerticalChange);
                    break;
            }
        }

        void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            OnAlignGrid();
            _owner.HideTipLines();
            Rect newRect = new Rect(Canvas.GetLeft(this), Canvas.GetTop(this), Width, Height);
            _owner.CmdResize.Execute(new ResizeArgs(this, _rectCur, newRect));
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 根据位置显示对齐线
        /// </summary>
        /// <param name="p_thumb"></param>
        void ShowLine(Thumb p_thumb)
        {
            double left = Canvas.GetLeft(_target);
            double top = Canvas.GetTop(_target);
            switch ((Corner)_thumbs.IndexOf(p_thumb as Thumb))
            {
                case Corner.Left:
                    _owner.ShowLeftLine(left);
                    break;
                case Corner.TopLeft:
                    _owner.ShowLeftLine(left);
                    _owner.ShowTopLine(top);
                    break;
                case Corner.Top:
                    _owner.ShowTopLine(top);
                    break;
                case Corner.TopRight:
                    _owner.ShowTopLine(top);
                    _owner.ShowRightLine(left + Width);
                    break;
                case Corner.Right:
                    _owner.ShowRightLine(left + Width);
                    break;
                case Corner.BottomRight:
                    _owner.ShowRightLine(left + Width);
                    _owner.ShowBottomLine(top + Height);
                    break;
                case Corner.Bottom:
                    _owner.ShowBottomLine(top + Height);
                    break;
                case Corner.BottomLeft:
                    _owner.ShowLeftLine(left);
                    _owner.ShowBottomLine(top + Height);
                    break;
            }
        }

        /// <summary>
        /// 设置新位置及大小
        /// </summary>
        /// <param name="p_left"></param>
        /// <param name="p_top"></param>
        /// <param name="p_width"></param>
        /// <param name="p_height"></param>
        void SetLoc(double p_left, double p_top, double p_width, double p_height)
        {
            if (p_left > 0 && p_top > 0
                && p_width > 0 && p_height > 0)
            {
                Canvas.SetLeft(_target, p_left);
                Canvas.SetTop(_target, p_top);
                _target.Width = p_width;
                _target.Height = p_height;

                Canvas.SetLeft(this, p_left);
                Canvas.SetTop(this, p_top);
                Width = p_width;
                Height = p_height;
                _owner.UpdateLayout();
                _owner.RefreshLinkLines(_target);
                _owner.LinkClerk.ShowLinkPt(_target as SNode, p_left, p_top + p_height);
            }
        }

        /// <summary>
        /// 对齐到网格
        /// </summary>
        void OnAlignGrid()
        {
            if (_owner.AlignGrid)
            {
                double left = Canvas.GetLeft(_target);
                double top = Canvas.GetTop(_target);
                double right = left + Width;
                double bottom = top + Height;
                left = (left % 20 > 10) ? Math.Ceiling(left / 20) * 20 : Math.Floor(left / 20) * 20;
                top = (top % 20 > 10) ? Math.Ceiling(top / 20) * 20 : Math.Floor(top / 20) * 20;
                right = (right % 20 > 10) ? Math.Ceiling(right / 20) * 20 : Math.Floor(right / 20) * 20;
                bottom = (bottom % 20 > 10) ? Math.Ceiling(bottom / 20) * 20 : Math.Floor(bottom / 20) * 20;

                Canvas.SetLeft(_target, left);
                Canvas.SetTop(_target, top);
                _target.Width = right - Canvas.GetLeft(_target);
                _target.Height = bottom - Canvas.GetTop(_target);

                Canvas.SetLeft(this, left);
                Canvas.SetTop(this, top);
                Width = _target.Width;
                Height = _target.Height;

                _owner.UpdateLayout();
                _owner.RefreshLinkLines(_target);
            }
        }

        /// <summary>
        /// 更新选择点
        /// </summary>
        /// <param name="p_thumb"></param>
        void UpdateSelector(Thumb p_thumb)
        {
            if (p_thumb == null && _thumbs == null)
                return;

            if (_target.ActualWidth >= 30.0 && _target.ActualHeight >= 30.0)
            {
                if (_target.ActualWidth >= 50.0 && _target.ActualHeight >= 50.0)
                {
                    foreach (Thumb thumb in _thumbs)
                    {
                        thumb.Visibility = Visibility.Visible;
                    }
                    _resized = false;
                }
                else
                {
                    _thumbs[1].Visibility = Visibility.Collapsed;
                    _thumbs[6].Visibility = Visibility.Collapsed;
                    _thumbs[3].Visibility = Visibility.Collapsed;
                    _thumbs[4].Visibility = Visibility.Collapsed;
                    if (p_thumb != null)
                    {
                        switch ((Corner)_thumbs.IndexOf(p_thumb))
                        {
                            case Corner.Top:
                                _thumbs[1].Visibility = Visibility.Visible;
                                break;

                            case Corner.Left:
                                _thumbs[3].Visibility = Visibility.Visible;
                                break;

                            case Corner.Right:
                                _thumbs[4].Visibility = Visibility.Visible;
                                break;

                            case Corner.Bottom:
                                _thumbs[6].Visibility = Visibility.Visible;
                                break;
                        }
                    }
                    if ((p_thumb == null) || _resized)
                    {
                        _thumbs[0].Visibility = Visibility.Visible;
                        _thumbs[2].Visibility = Visibility.Visible;
                        _thumbs[5].Visibility = Visibility.Visible;
                        _thumbs[7].Visibility = Visibility.Visible;
                    }
                    _resized = false;
                }
            }
            else
            {
                foreach (Thumb thumb in _thumbs)
                {
                    if (thumb != p_thumb)
                        thumb.Visibility = Visibility.Collapsed;
                }

                if (p_thumb != null
                    && (_target.ActualWidth >= 30.0 || _target.ActualHeight >= 30.0))
                {
                    switch ((Corner)_thumbs.IndexOf(p_thumb))
                    {
                        case Corner.TopLeft:
                            _thumbs[7].Visibility = Visibility.Visible;
                            break;

                        case Corner.Top:
                        case Corner.Bottom:
                            _thumbs[7].Visibility = Visibility.Visible;
                            _thumbs[0].Visibility = Visibility.Visible;
                            break;

                        case Corner.TopRight:
                            _thumbs[5].Visibility = Visibility.Visible;
                            break;

                        case Corner.Left:
                        case Corner.Right:
                            _thumbs[5].Visibility = Visibility.Visible;
                            _thumbs[2].Visibility = Visibility.Visible;
                            break;

                        case Corner.BottomLeft:
                            _thumbs[2].Visibility = Visibility.Visible;
                            break;

                        case Corner.BottomRight:
                            _thumbs[0].Visibility = Visibility.Visible;
                            break;
                    }
                }
                else if (p_thumb == null)
                {
                    _thumbs[7].Visibility = Visibility.Visible;
                    _thumbs[0].Visibility = Visibility.Visible;
                }
                _resized = true;
            }
        }
        #endregion
    }

    internal enum Corner
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }
}
