#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using System.Linq;
#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 选择管理类
    /// </summary>
    internal class SelectionManager
    {
        #region 成员变量
        Sketch _owner;
        NodeSelector _nodeSelector;
        Rectangle _selRect;
        // 不包含选中的连线
        readonly List<FrameworkElement> _selection;
        SLine _selLine;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_owner"></param>
        public SelectionManager(Sketch p_owner)
        {
            _owner = p_owner;
            _selection = new List<FrameworkElement>();
        }

        #endregion

        /// <summary>
        /// 获取当前选择元素列表，不包含选中的连线
        /// </summary>
        public List<FrameworkElement> Selection
        {
            get { return _selection; }
        }

        /// <summary>
        /// 获取当前选中的连线
        /// </summary>
        public SLine SelectedLine
        {
            get { return _selLine; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="p_nodeSelector"></param>
        /// <param name="p_selRect"></param>
        public void Init(NodeSelector p_nodeSelector, Rectangle p_selRect)
        {
            _nodeSelector = p_nodeSelector;
            _selRect = p_selRect;
        }

        /// <summary>
        /// 选择UI元素
        /// </summary>
        /// <param name="p_element"></param>
        /// <param name="p_extend">是否扩展选择</param>
        public void Select(FrameworkElement p_element, bool p_extend)
        {
            ClearLine();
            if (p_extend)
            {
                if (_selection.Contains(p_element))
                    _selection.Remove(p_element);
                else
                    _selection.Add(p_element);
            }
            else
            {
                _selection.Clear();
                _selection.Add(p_element);
            }
            UpdateAdorner();
            _owner.OnSelectionChanged();
            BringToFront(p_element);
        }

        /// <summary>
        /// 多选UI元素
        /// </summary>
        /// <param name="p_items"></param>
        public void Select(List<FrameworkElement> p_items)
        {
            //相同时不再添加
            if (_selection != p_items)
                _selection.AddRange(p_items);
            UpdateAdorner();
            _owner.OnSelectionChanged();
            foreach (FrameworkElement elem in p_items)
            {
                BringToFront(elem);
            }
        }

        /// <summary>
        /// 选择连线
        /// </summary>
        /// <param name="p_line"></param>
        public void SelectLine(SLine p_line)
        {
            ClearNode();
            ClearLine();
            _selLine = p_line;
            if (_selLine != null)
                _selLine.ThumbVisibility = Visibility.Visible;
            _owner.OnSelectionChanged();
            BringToFront(p_line);
        }

        /// <summary>
        /// 移动已选择元素
        /// </summary>
        /// <param name="p_deltaX"></param>
        /// <param name="p_deltaY"></param>
        public void Move(double p_deltaX, double p_deltaY)
        {
            int count = _selection.Count;
            if (count == 1)
            {
                var item = _selection[0];
                double left = Canvas.GetLeft(item) + p_deltaX;
                double top = Canvas.GetTop(item) + p_deltaY;
                Canvas.SetLeft(item, left);
                Canvas.SetTop(item, top);
                Canvas.SetLeft(_nodeSelector, left);
                Canvas.SetTop(_nodeSelector, top);
                _owner.RefreshLinkLines(item);
                _owner.LinkClerk.ShowLinkPt(item as SNode, left, top + item.Height);
            }
            else if (count > 1)
            {
                foreach (FrameworkElement item in _selection)
                {
                    Canvas.SetLeft(item, Canvas.GetLeft(item) + p_deltaX);
                    Canvas.SetTop(item, Canvas.GetTop(item) + p_deltaY);
                }
                Canvas.SetLeft(_selRect, Canvas.GetLeft(_selRect) + p_deltaX);
                Canvas.SetTop(_selRect, Canvas.GetTop(_selRect) + p_deltaY);
                _owner.RefreshAllLines();
            }
            _owner.CalcPageSize();
        }

        /// <summary>
        /// 移动结束时对齐到网格
        /// </summary>
        public void AlignGrid()
        {
            if (_owner.AlignGrid)
            {
                if (_selection.Count == 1)
                {
                    var item = _selection[0];
                    Point pt = GetAlignPoint(item);
                    Canvas.SetLeft(item, pt.X);
                    Canvas.SetTop(item, pt.Y);
                    Canvas.SetLeft(_nodeSelector, pt.X);
                    Canvas.SetTop(_nodeSelector, pt.Y);
                    _owner.RefreshLinkLines(item);
                    _owner.LinkClerk.ShowLinkPt(item as SNode, pt.X, pt.Y + item.Height);
                }
                else if (_selection.Count > 1)
                {
                    double top = double.MaxValue;
                    double left = double.MaxValue;
                    double right = 0;
                    double bottom = 0;

                    foreach (FrameworkElement item in _selection)
                    {
                        Point pt = GetAlignPoint(item);
                        double curRight = pt.X + item.ActualWidth;
                        double curBottom = pt.Y + item.ActualHeight;
                        if (pt.X < left)
                            left = pt.X;
                        if (pt.Y < top)
                            top = pt.Y;
                        if (curRight > right)
                            right = curRight;
                        if (curBottom > bottom)
                            bottom = curBottom;

                        Canvas.SetLeft(item, pt.X);
                        Canvas.SetTop(item, pt.Y);
                    }

                    Canvas.SetLeft(_selRect, left);
                    Canvas.SetTop(_selRect, top);
                    _selRect.Width = right - left;
                    _selRect.Height = bottom - top;
                    _owner.RefreshAllLines();
                }
            }
        }

        /// <summary>
        /// 显示所有提示线
        /// </summary>
        public void ShowTipLines()
        {
            MoveTipLines();
            _owner.ShowTipLines();
        }

        /// <summary>
        /// 更新提示线位置
        /// </summary>
        public void MoveTipLines()
        {
            double left, top;
            if (_nodeSelector.Visibility == Visibility.Visible)
            {
                left = Canvas.GetLeft(_nodeSelector);
                top = Canvas.GetTop(_nodeSelector);
                _owner.MoveTipLines(left, top, left + _nodeSelector.ActualWidth, top + _nodeSelector.ActualHeight);
            }
            else if (_selRect.Visibility == Visibility.Visible)
            {
                left = Canvas.GetLeft(_selRect);
                top = Canvas.GetTop(_selRect);
                _owner.MoveTipLines(left, top, left + _selRect.ActualWidth, top + _selRect.ActualHeight);
            }
        }

        /// <summary>
        /// 清空选择状态
        /// </summary>
        public void Clear()
        {
            if (_nodeSelector != null)
            {
                ClearNode();
                ClearLine();
            }
        }

        /// <summary>
        /// 判断点是否多选矩形内部
        /// </summary>
        /// <param name="p_pt"></param>
        /// <returns></returns>
        public bool IsInSelectionRect(Point p_pt)
        {
            if (_selection.Count > 1 && _selRect.Visibility == Visibility.Visible)
            {
                double left = Canvas.GetLeft(_selRect);
                double top = Canvas.GetTop(_selRect);
                return (p_pt.X >= left
                    && p_pt.X <= left + _selRect.ActualWidth
                    && p_pt.Y >= top
                    && p_pt.Y <= top + _selRect.ActualHeight);
            }
            return false;
        }

        /// <summary>
        /// 获取当前选择区域的位置
        /// </summary>
        /// <returns></returns>
        public Point GetCurrentPos()
        {
            if (_selection.Count == 0)
                return new Point();
            if (_selection.Count == 1)
                return new Point(Canvas.GetLeft(_nodeSelector), Canvas.GetTop(_nodeSelector));
            return new Point(Canvas.GetLeft(_selRect), Canvas.GetTop(_selRect));
        }

        /// <summary>
        /// 清空所有图元的选择状态
        /// </summary>
        void ClearNode()
        {
            _nodeSelector.Visibility = Visibility.Collapsed;
            _selRect.Visibility = Visibility.Collapsed;
            _owner.LinkClerk.HideLinkPt();
            _selection.Clear();
        }

        /// <summary>
        /// 清空连线的选择状态
        /// </summary>
        void ClearLine()
        {
            if (_selLine != null)
                _selLine.ThumbVisibility = Visibility.Collapsed;
            _selLine = null;
        }

        /// <summary>
        /// 刷新修饰层内容
        /// </summary>
        void UpdateAdorner()
        {
            int count = _selection.Count;
            _nodeSelector.Visibility = Visibility.Collapsed;
            _selRect.Visibility = Visibility.Collapsed;

            if (count == 1)
            {
                _nodeSelector.Target = _selection[0];
                _nodeSelector.Visibility = Visibility.Visible;
            }
            else if (count > 1)
            {
                // 多选
                double top = double.MaxValue;
                double left = double.MaxValue;
                double right = 0;
                double bottom = 0;

                foreach (var item in _selection)
                {
                    double curLeft = Canvas.GetLeft(item);
                    double curTop = Canvas.GetTop(item);
                    double curRight = curLeft + item.ActualWidth;
                    double curBottom = curTop + item.ActualHeight;

                    if (curLeft < left)
                        left = curLeft;
                    if (curTop < top)
                        top = curTop;
                    if (curRight > right)
                        right = curRight;
                    if (curBottom > bottom)
                        bottom = curBottom;
                }
                Canvas.SetLeft(_selRect, left);
                Canvas.SetTop(_selRect, top);
                _selRect.Width = right - left;
                _selRect.Height = bottom - top;
                _selRect.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 获取对齐点位置
        /// </summary>
        /// <param name="p_elem"></param>
        /// <returns></returns>
        Point GetAlignPoint(FrameworkElement p_elem)
        {
            double left = Canvas.GetLeft(p_elem);
            double top = Canvas.GetTop(p_elem);
            left = (left % 20 > 10) ? Math.Ceiling(left / 20) * 20 : Math.Floor(left / 20) * 20;
            top = (top % 20 > 10) ? Math.Ceiling(top / 20) * 20 : Math.Floor(top / 20) * 20;
            return new Point(left, top);
        }

        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="p_elem"></param>
        void BringToFront(FrameworkElement p_elem)
        {
            int maxZIndex = _owner.Container.Children.Max(itm => Canvas.GetZIndex(itm as FrameworkElement));
            if (Canvas.GetZIndex(p_elem) <= maxZIndex)
            {
                Canvas.SetZIndex(p_elem, maxZIndex + 1);
            }
        }
    }
}
