#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-03-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FileLists;
using Dt.Core;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 布局面板
    /// </summary>
    public partial class FileListPanel : Panel
    {
        #region 成员变量
        const double PanelMaxHeight = 500;
        readonly List<double> _linesHeight = new List<double>();
        #endregion

        internal FileList Owner { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count == 0
                || availableSize.Width == 0
                || availableSize.Height == 0)
                return base.MeasureOverride(availableSize);

            // 单列不自动填充
            if (Owner.ColCount == 1 && Owner.HorizontalAlignment != HorizontalAlignment.Stretch)
                return MeasureOneCol(availableSize);

            int colCount = Owner.ColCount;
            if (Owner.HorizontalAlignment == HorizontalAlignment.Stretch
                && Owner.ColCount > Children.Count)
            {
                // 水平填充 且 项数不够一行时，按项数平均分配宽度
                colCount = Children.Count;
            }

            double maxWidth = double.IsInfinity(availableSize.Width) ? Kit.ViewWidth : availableSize.Width;
            double colWidth = (maxWidth - (colCount - 1) * Owner.Spacing) / colCount;

            double totalHeight = 0;
            double lineHeight = 0;
            _linesHeight.Clear();
            Size itemSize = new Size(colWidth, PanelMaxHeight);
            Size imgSize = new Size(colWidth, Owner.ImageHeight > 0 ? Owner.ImageHeight : maxWidth / Owner.ColCount);
            for (int i = 0; i < Children.Count; i++)
            {
                var item = Children[i] as FileItem;
                double height;
                if (item.FileType == FileItemType.Image)
                {
                    item.Measure(imgSize);
                    height = imgSize.Height;
                }
                else
                {
                    item.Measure(itemSize);
                    height = item.DesiredSize.Height;
                }

                if (height > lineHeight)
                    lineHeight = height;

                // 行尾或最后一项
                if ((i + 1) % colCount == 0 || i == Children.Count - 1)
                {
                    totalHeight += lineHeight;
                    _linesHeight.Add(lineHeight);
                    lineHeight = 0;
                }
            }
            return new Size(maxWidth, totalHeight + (_linesHeight.Count - 1) * Owner.Spacing);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return base.ArrangeOverride(finalSize);

            if (Owner.ColCount == 1 && Owner.HorizontalAlignment != HorizontalAlignment.Stretch)
                return ArrangeOneCol(finalSize);

            int colCount = Owner.ColCount;
            if (Owner.HorizontalAlignment == HorizontalAlignment.Stretch
                && Owner.ColCount > Children.Count)
            {
                // 水平填充 且 项数不够一行时，按项数平均分配宽度
                colCount = Children.Count;
            }

            double colWidth = (finalSize.Width - (colCount - 1) * Owner.Spacing) / colCount;
            double totalHeight = 0;

            for (int i = 0; i < Children.Count; i++)
            {
                var item = Children[i] as FileItem;
                int col = i % colCount;
                int row = i / colCount;
                item.Arrange(new Rect(col * (colWidth + Owner.Spacing), totalHeight, colWidth, _linesHeight[row]));

                if ((i + 1) % colCount == 0)
                {
                    // 行尾
                    totalHeight += _linesHeight[row] + Owner.Spacing;
                }
            }
            return finalSize;
        }

        /// <summary>
        /// 单列不自动填充
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        Size MeasureOneCol(Size availableSize)
        {
            double maxWidth = double.IsInfinity(availableSize.Width) ? Kit.ViewWidth : availableSize.Width;
            Size itemSize = new Size(maxWidth, PanelMaxHeight);
            Size imgSize = new Size(maxWidth, Owner.ImageHeight > 0 ? Owner.ImageHeight : maxWidth);
            double width = 0;
            double height = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                var item = Children[i] as FileItem;
                if (item.FileType == FileItemType.Image)
                {
                    item.Measure(imgSize);
                    height += imgSize.Height;
                }
                else
                {
                    item.Measure(itemSize);
                    height += item.DesiredSize.Height;
                }
                if (item.DesiredSize.Width > width)
                    width = item.DesiredSize.Width;
            }
            return new Size(width, height + (Children.Count - 1) * Owner.Spacing);
        }

        Size ArrangeOneCol(Size finalSize)
        {
            double height = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                var item = Children[i] as FileItem;
                item.Arrange(new Rect(0, height, finalSize.Width, item.DesiredSize.Height));
                height += item.DesiredSize.Height + Owner.Spacing;
            }
            return finalSize;
        }
    }
}