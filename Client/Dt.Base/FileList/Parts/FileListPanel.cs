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

            double maxWidth = double.IsInfinity(availableSize.Width) ? SysVisual.ViewWidth : availableSize.Width;
            double colWidth = maxWidth / Owner.ColCount;

            double totalHeight = 0;
            double lineHeight = 0;
            _linesHeight.Clear();
            Size itemSize = new Size(colWidth, PanelMaxHeight);
            Size imgSize = new Size(colWidth, Owner.ImageHeight > 0 ? Owner.ImageHeight : colWidth);
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
                if ((i + 1) % Owner.ColCount == 0 || i == Children.Count - 1)
                {
                    totalHeight += lineHeight;
                    _linesHeight.Add(lineHeight);
                    lineHeight = 0;
                }
            }
            return new Size(maxWidth, totalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return base.ArrangeOverride(finalSize);

            if (Owner.ColCount == 1 && Owner.HorizontalAlignment != HorizontalAlignment.Stretch)
                return ArrangeOneCol(finalSize);

            double totalHeight = 0;
            double colWidth = finalSize.Width / Owner.ColCount;
            for (int i = 0; i < Children.Count; i++)
            {
                var item = Children[i] as FileItem;
                int col = i % Owner.ColCount;
                int row = i / Owner.ColCount;
                item.Arrange(new Rect(col * colWidth, totalHeight, colWidth, _linesHeight[row]));

                if ((i + 1) % Owner.ColCount == 0)
                {
                    // 行尾
                    totalHeight += _linesHeight[row];
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
            double maxWidth = double.IsInfinity(availableSize.Width) ? SysVisual.ViewWidth : availableSize.Width;
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
            return new Size(width, height);
        }

        Size ArrangeOneCol(Size finalSize)
        {
            double height = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                var item = Children[i] as FileItem;
                item.Arrange(new Rect(0, height, finalSize.Width, item.DesiredSize.Height));
                height += item.DesiredSize.Height;
            }
            return finalSize;
        }
    }
}