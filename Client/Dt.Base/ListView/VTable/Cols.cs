#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Markup;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列集合
    /// </summary>
    public class Cols : Nl<object>
    {
        /// <summary>
        /// 列宽变化事件
        /// </summary>
        public event EventHandler ColWidthChanged;

        /// <summary>
        /// 列集合变化、Col.ID变化等引起的重新加载事件
        /// </summary>
        public event EventHandler Reloading;

        /// <summary>
        /// 获取设置点击列头是否可以排序
        /// </summary>
        public bool AllowSorting { get; set; } = true;

        /// <summary>
        /// 获取设置是否隐藏行号，默认false
        /// </summary>
        public bool HideIndex { get; set; }

        /// <summary>
        /// 列总宽
        /// </summary>
        internal double TotalWidth { get; set; }

        /// <summary>
        /// 列宽失效，触发重新测量布局
        /// </summary>
        internal void OnColWidthChanged()
        {
            FixWidth(_maxWidth);
            ColWidthChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 列集合变化、Col.ID变化等引起的重新加载
        /// </summary>
        internal void OnReloading()
        {
            FixWidth(_maxWidth);
            Reloading?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 更新水平位置及总宽
        /// </summary>
        internal void FixWidth(double p_maxWidth)
        {
            _maxWidth = p_maxWidth;
            TotalWidth = 0;
            double totalStar = 0;

            foreach (Col col in this.OfType<Col>())
            {
                var str = col.Width;
                double w = 0;

                if (string.IsNullOrEmpty(str)
                    || str.Equals("auto", StringComparison.OrdinalIgnoreCase))
                {
                    // auto 或 未指定时
                    w = _defaultWidth;
                }
                else if (double.TryParse(str, out var width))
                {
                    w = width;
                }
                else if (str.EndsWith('*'))
                {
                    // star
                    if (str.Length == 1)
                    {
                        totalStar += 1;
                    }
                    else if (double.TryParse(str.TrimEnd('*'), out var per))
                    {
                        totalStar += per;
                    }
                    else
                    {
                        w = _defaultWidth;
                    }
                }
                else
                {
                    w = _defaultWidth;
                }

                col.Left = TotalWidth;
                col.ActualWidth = w;
                TotalWidth += w;
            }

            // 列宽中有*，重新计算 ActualWidth Left
            if (totalStar > 0)
            {
                double unit = p_maxWidth > TotalWidth ? (p_maxWidth - TotalWidth) / totalStar : 0;
                // *按最小120宽
                unit = Math.Max(unit, _defaultWidth);
                TotalWidth = 0;

                foreach (Col col in this.OfType<Col>())
                {
                    if (col.ActualWidth > 0)
                    {
                        col.Left = TotalWidth;
                        TotalWidth += col.ActualWidth;
                    }
                    else
                    {
                        // star
                        var str = col.Width;
                        double per = 0;
                        if (str.Length == 1)
                        {
                            per = 1;
                        }
                        else if (double.TryParse(str.TrimEnd('*'), out per))
                        {
                        }

                        col.Left = TotalWidth;
                        col.ActualWidth = Math.Floor(per * unit);
                        TotalWidth += col.ActualWidth;
                    }
                }
            }
        }

        /// <summary>
        /// 列头最多占用的行数
        /// </summary>
        internal int GetLineCount()
        {
            int cnt;
            int maxCnt = 1;
            foreach (Col col in this.OfType<Col>())
            {
                if (!string.IsNullOrEmpty(col.Title)
                    && (cnt = col.Title.Split('\u000A').Length) > maxCnt)
                {
                    maxCnt = cnt;
                }
            }
            return maxCnt;
        }

        /// <summary>
        /// 锁定列集合，集合变化Reloading
        /// </summary>
        internal void LockCols()
        {
            CollectionChanged -= OnColsCollectionChanged;
            CollectionChanged += OnColsCollectionChanged;

            foreach (var col in this.OfType<Col>())
            {
                col.Owner = this;
            }
        }

        void OnColsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var col in e.NewItems.OfType<Col>())
                {
                    col.Owner = this;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var col in this.OfType<Col>())
                {
                    col.Owner = this;
                }
            }
            OnReloading();
        }

        // 6个中文字
        const double _defaultWidth = 120d;
        double _maxWidth;
    }
}
