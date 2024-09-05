#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using Path = Microsoft.UI.Xaml.Shapes.Path;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
#endregion

namespace Dt.Base.TreeDiagrams
{
    /// <summary>
    /// 树节点布局面板
    /// </summary>
    public partial class TdPanel : Panel
    {
        #region 成员变量
        const double PanelMaxHeight = 5000;
        // 连线高度
        const double LinkLineHeight = 40;
        const double ArrowSize = 20;

        TreeDiagram _owner;

        // 层高
        readonly List<double> _levelHeight = new List<double>();

        /// <summary>
        /// 面板最大尺寸，宽高始终不为无穷大！
        /// </summary>
        Size _maxSize = Size.Empty;
        #endregion

        public TdPanel(TreeDiagram p_owner)
        {
            _owner = p_owner;
            Background = Res.TransparentBrush;
            LoaRealRows();
        }

        /// <summary>
        /// 设置面板的最大尺寸，宽高始终不为无穷大！
        /// </summary>
        internal void SetMaxSize(Size p_size)
        {
            // 尺寸变化大于2有效，否则iOS版易造成死循环，每次 p_size 有微小变化！！！
            if (Math.Abs(_maxSize.Width - p_size.Width) > 2 || Math.Abs(_maxSize.Height - p_size.Height) > 2)
            {
                _maxSize = p_size;
            }
        }

        /// <summary>
        /// 切换模板、调整是否采用虚拟化时重新加载
        /// </summary>
        internal void Reload()
        {
            LoaRealRows();
        }

        /// <summary>
        /// 从根节点展开到当前节点，并滚动到可视范围
        /// </summary>
        /// <param name="p_item"></param>
        internal void ScrollIntoItem(TdItem p_item)
        {
        }

        #region 测量
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count == 0)
                return new Size();

            var size = new Size(_maxSize.Width, PanelMaxHeight);
            double rowHeight = 0;
            double sumWidth = 0;
            foreach (var item in _owner.RootItems)
            {
                item.UI.Measure(size);
                if (item.UI.DesiredSize.Height > rowHeight)
                {
                    rowHeight = item.UI.DesiredSize.Height;
                }
                var childWidth = MeasureChildRows(1, size, item);
                item.TotalWidth = childWidth > item.UI.DesiredSize.Width ? childWidth : item.UI.DesiredSize.Width;
                item.LeftWidth = sumWidth;
                sumWidth += item.TotalWidth;
            }
            rowHeight += LinkLineHeight;
            UpdateLevelHeight(0, rowHeight);

            double sumHeight = 0;
            _levelHeight.ForEach(d => sumHeight += d);
            return new Size(sumWidth, sumHeight);
        }

        double MeasureChildRows(int p_level, Size p_size, TdItem p_parent)
        {
            if (p_parent.Children.Count == 0)
                return 0;

            // 左侧已占有的宽度
            double leftWidth = 0;
            if (p_parent.Parent != null && p_parent.Parent.Children.Count > 0)
            {
                leftWidth = p_parent.Parent.Children[0].LeftWidth;
                foreach (var item in p_parent.Parent.Children)
                {
                    if (item == p_parent)
                        break;

                    leftWidth += item.TotalWidth;
                }
            }
            else
            {
                foreach (var item in _owner.RootItems)
                {
                    if (item == p_parent)
                        break;

                    leftWidth += item.TotalWidth;
                }
            }

            if (p_parent.HorLinkLine != null)
                p_parent.HorLinkLine.Measure(p_size);

            double sumWidth = 0;
            double rowHeight = 0;
            foreach (var item in p_parent.Children)
            {
                item.UI.Measure(p_size);
                if (item.UI.DesiredSize.Height > rowHeight)
                {
                    rowHeight = item.UI.DesiredSize.Height;
                }

                item.TopVerLine.Measure(p_size);
                item.TopArrow.Measure(p_size);
                if (item.BottomVerLine != null)
                    item.BottomVerLine.Measure(p_size);

                var childWidth = MeasureChildRows(p_level + 1, p_size, item);
                item.TotalWidth = childWidth > item.UI.DesiredSize.Width ? childWidth : item.UI.DesiredSize.Width;
                item.LeftWidth = leftWidth + sumWidth;
                sumWidth += item.TotalWidth;
            }
            rowHeight += 2 * LinkLineHeight;
            UpdateLevelHeight(p_level, rowHeight);

            return sumWidth;
        }
        #endregion

        #region 布局
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

            Rect rc = new Rect(new Point(), finalSize);
            foreach (var item in _owner.RootItems)
            {
                item.UI.Arrange(new Rect(
                    item.LeftWidth + Math.Floor(item.TotalWidth / 2) - Math.Floor(item.UI.DesiredSize.Width / 2),
                    0,
                    item.UI.DesiredSize.Width,
                    item.UI.DesiredSize.Height));

                if (item.BottomVerLine != null)
                {
                    item.BottomVerLine.X1 = item.BottomVerLine.X2 = item.LeftWidth + Math.Floor(item.TotalWidth / 2);
                    item.BottomVerLine.Y1 = item.UI.DesiredSize.Height;
                    item.BottomVerLine.Y2 = _levelHeight[0];
                    item.BottomVerLine.Arrange(rc);
                }

                ArrangeChildRows(1, rc, item);
            }
            return finalSize;
        }

        void ArrangeChildRows(int p_level, Rect p_rect, TdItem p_parent)
        {
            if (p_parent.Children.Count == 0)
                return;

            double startTop = 0;
            for (int i = 0; i < p_level; i++)
            {
                startTop += _levelHeight[i];
            }

            for (int i = 0; i < p_parent.Children.Count; i++)
            {
                var item = p_parent.Children[i];
                var rc = new Rect(
                    item.LeftWidth + Math.Floor(item.TotalWidth / 2) - Math.Floor(item.UI.DesiredSize.Width / 2),
                    startTop + LinkLineHeight,
                    item.UI.DesiredSize.Width,
                    item.UI.DesiredSize.Height);
                item.UI.Arrange(rc);

                item.TopVerLine.X1 = item.TopVerLine.X2 = item.LeftWidth + Math.Floor(item.TotalWidth / 2);
                item.TopVerLine.Y1 = startTop;
                item.TopVerLine.Y2 = startTop + LinkLineHeight;
                item.TopVerLine.Arrange(p_rect);

                item.TopArrow.Arrange(new Rect(item.TopVerLine.X1 - Math.Floor(ArrowSize / 2), item.TopVerLine.Y2 - ArrowSize, ArrowSize, ArrowSize));

                if (p_parent.HorLinkLine != null)
                {
                    if (i == 0)
                    {
                        // 水平线起点
                        p_parent.HorLinkLine.Y1 = p_parent.HorLinkLine.Y2 = startTop;
                        p_parent.HorLinkLine.X1 = item.TopVerLine.X1;
                    }
                    else if (i == p_parent.Children.Count - 1)
                    {
                        // 水平线终点
                        p_parent.HorLinkLine.X2 = item.TopVerLine.X1;
                        p_parent.HorLinkLine.Arrange(p_rect);
                    }
                }

                if (item.BottomVerLine != null)
                {
                    item.BottomVerLine.X1 = item.BottomVerLine.X2 = item.LeftWidth + Math.Floor(item.TotalWidth / 2);
                    item.BottomVerLine.Y1 = startTop + LinkLineHeight + item.UI.DesiredSize.Height;
                    item.BottomVerLine.Y2 = startTop + _levelHeight[p_level + 1];
                    item.BottomVerLine.Arrange(p_rect);
                }

                ArrangeChildRows(p_level + 1, p_rect, item);
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 加载真实模式的所有行
        /// </summary>
        void LoaRealRows()
        {
            ClearAllRows();
            if (_owner.RootItems.Count > 0)
            {
                foreach (var item in _owner.RootItems)
                {
                    var pi = new TdPanelItem(_owner, item);
                    Children.Add(pi);
                    item.UI = pi;
                    AddChildRows(item);
                }
            }
        }

        void AddChildRows(TdItem p_parent)
        {
            if (p_parent.Children.Count == 0)
                return;

            var line = CreateLine();
            Children.Add(line);
            p_parent.BottomVerLine = line;

            // 多于1个子节点时需要水平连线
            if (p_parent.Children.Count > 1)
            {
                line = CreateLine();
                Children.Add(line);
                p_parent.HorLinkLine = line;
            }

            foreach (var item in p_parent.Children)
            {
                var pi = new TdPanelItem(_owner, item);
                Children.Add(pi);
                item.UI = pi;

                line = CreateLine();
                Children.Add(line);
                item.TopVerLine = line;

                var arrow = new Path
                {
                    Data = Res.ParseGeometry("M0,0 8,0 4,4 Z"),
                    Width = ArrowSize,
                    Height = ArrowSize,
                    Fill = Res.深灰2,
                    Stretch = Stretch.Fill,
                    StrokeThickness = 0,
                };
                Children.Add(arrow);
                item.TopArrow = arrow;

                AddChildRows(item);
            }
        }

        /// <summary>
        /// 清除所有行
        /// </summary>
        void ClearAllRows()
        {
            Children.Clear();
            _owner.RootItems.ClearUI();
            _levelHeight.Clear();
        }

        void UpdateLevelHeight(int p_level, double p_height)
        {
            // 补充
            if (_levelHeight.Count < p_level + 1)
            {
                int cnt = p_level - _levelHeight.Count;
                for (int i = 0; i <= cnt; i++)
                {
                    _levelHeight.Add(0);
                }
            }
            // 记录最大高度
            if (_levelHeight[p_level] < p_height)
                _levelHeight[p_level] = p_height;
        }

        Line CreateLine()
        {
            return new Line
            {
                Stroke = Res.深灰2,
                StrokeThickness = 2
            };
        }
        #endregion
    }
}
