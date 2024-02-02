﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Specialized;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 顶部的列头面板
    /// </summary>
    public partial class ColHeader : Panel
    {
        #region 成员变量
        TextBlock _tbDrag;
        int _lastDrag = -1;
        #endregion

        #region 构造方法
        public ColHeader(Lv p_owner)
        {
            Lv = p_owner;
            Cols cols = p_owner.Cols;
            cols.LockCols();
            cols.ColWidthChanged += (s, e) => InvalidateMeasure();
            cols.Reloading += OnColsReloading;
            LoadAllCols();
        }
        #endregion

        internal Lv Lv { get; }

        internal ColHeaderCell GetCellByID(string p_id)
        {
            foreach (var cell in Children.OfType<ColHeaderCell>())
            {
                if (cell.Col.ID.Equals(p_id, StringComparison.OrdinalIgnoreCase))
                    return cell;
            }
            return null;
        }

        internal Col GetDragTargetCol(Col p_col, double p_pos)
        {
            int index = -1;
            Col col;
            Cols cols = Lv.Cols;
            for (int i = 0; i < cols.Count; i++)
            {
                col = (Col)cols[i];
                if (p_pos >= col.Left && p_pos <= col.Left + col.ActualWidth)
                {
                    index = i;
                    break;
                }
            }

            // 未找到或还在原来列
            if (index == -1 || (col = (Col)cols[index]) == p_col)
            {
                if (_lastDrag > -1)
                {
                    _lastDrag = -1;
                    Children.Remove(_tbDrag);
                    _tbDrag = null;
                }
                return null;
            }

            int toIndex = index > cols.IndexOf(p_col) ? index + 1 : index;
            if (toIndex != _lastDrag)
            {
                _lastDrag = toIndex;
                if (_tbDrag == null)
                {
                    _tbDrag = new TextBlock
                    {
                        Text = "\uE018",
                        FontFamily = Res.IconFont,
                        FontSize = 20,
                        Foreground = Res.RedBrush,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    Children.Add(_tbDrag);
                }
                else
                {
                    InvalidateArrange();
                }
            }
            return (Col)cols[index];
        }

        internal void FinishedDrag()
        {
            if (_lastDrag > -1)
            {
                _lastDrag = -1;
                Children.Remove(_tbDrag);
                _tbDrag = null;
            }
        }

        internal void SyncSortIcon()
        {
            foreach (var cell in Children.OfType<ColHeaderCell>())
            {
                cell.SyncSortIcon();
            }
        }

        #region 测量布局
        protected override Size MeasureOverride(Size availableSize)
        {
            double height = Res.RowOuterHeight;
            foreach (var cell in Children.OfType<ColHeaderCell>())
            {
                cell.Measure(new Size(cell.Col.ActualWidth, availableSize.Height));
                if (cell.DesiredSize.Height > height)
                    height = cell.DesiredSize.Height;
            }

            // 大于默认行高时，上下留边距
            if (height > Res.RowOuterHeight)
                height += 18;

            if (_lastDrag > -1)
                _tbDrag.Measure(new Size(20, height));
            return new Size(Lv.Cols.TotalWidth, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var cell in Children.OfType<ColHeaderCell>())
            {
                cell.Arrange(new Rect(cell.Col.Left, 0, cell.Col.ActualWidth, finalSize.Height));
            }
            if (_lastDrag > -1)
            {
                Cols cols = Lv.Cols;
                double left = -10;
                if (_lastDrag > 0 && _lastDrag < cols.Count)
                    left = ((Col)Lv.Cols[_lastDrag]).Left - 10;
                else if (_lastDrag == cols.Count)
                    left = cols.TotalWidth - 10;
                _tbDrag.Arrange(new Rect(left, 0, 20, finalSize.Height));
            }
            return finalSize;
        }
        #endregion

        void LoadAllCols()
        {
            foreach (var col in Lv.Cols.OfType<Col>())
            {
                Children.Add(new ColHeaderCell(col, this));
            }
        }

        void OnColsReloading(object sender, EventArgs e)
        {
            Children.Clear();
            LoadAllCols();
        }
    }
}
