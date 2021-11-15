#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 单元格面板
    /// </summary>
    internal partial class CellItem : Panel
    {
        #region 成员变量
        const double _textPadding = 4.0;
        static Size _szEmpty = new Size();
        static Rect _rcEmpty = new Rect();
        readonly TextBlock _tb;
        CellOverflowLayout _overflowLayout;
        Rect? _cachedClip;

        ConditionalFormatView _conditionalView;
        CustomDrawingObject _customDrawingObject;
        FrameworkElement _customDrawingObjectView;
        DataBarDrawingObject _dataBarObject;
        IconDrawingObject _iconObject;
        Sparkline _sparkInfo;
        BaseSparklineView _sparklineView;
        StrikethroughView _strikethroughView;
        FilterButton _filterButton;
        bool _lastUnderline;
        #endregion

        #region 构造方法
        public CellItem(RowItem p_rowItem)
        {
            OwnRow = p_rowItem;
            Column = -1;
            _tb = new TextBlock { VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
            Children.Add(_tb);
        }
        #endregion

        #region 属性
        public RowItem OwnRow { get; }

        public int Row
        {
            get { return OwnRow.Row; }
        }

        public int Column { get; set; }

        public CellLayout CellLayout { get; set; }

        public Cell BindingCell { get; private set; }

        public Excel Excel
        {
            get { return OwnRow.OwnPanel.Excel; }
        }

        public double ZoomFactor
        {
            get { return (double)OwnRow.OwnPanel.Excel.ZoomFactor; }
        }

        public CellOverflowLayout CellOverflowLayout
        {
            get { return _overflowLayout; }
            set { _overflowLayout = value; }
        }

        public FilterButtonInfo FilterButtonInfo { get; private set; }
        #endregion

        #region 外部方法
        public void UpdateFilterButtonState()
        {
            _filterButton?.ApplyState();
        }

        public void CleanUpBeforeDiscard()
        {
            if (Children.Count > 1)
            {
                while (Children.Count > 1)
                {
                    Children.RemoveAt(Children.Count - 1);
                }

                _customDrawingObjectView = null;
                if (_sparkInfo != null)
                {
                    _sparkInfo.SparklineChanged -= new EventHandler(sparkline_SparklineChanged);
                    _sparkInfo = null;
                }
            }
        }

        public void Refresh()
        {
            UpdateChildren();
            InvalidateMeasure();
            InvalidateArrange();
        }
        #endregion

        #region 测量布局
        //*** CellsPanel.Measure -> RowsLayer.Measure -> RowItem.UpdateChildren -> 行列改变时 CellItem.UpdateChildren -> RowItem.Measure -> CellItem.Measure ***//

        public void UpdateChildren()
        {
            // 刷新绑定的Cell
            int row = OwnRow.Row;
            int column = Column;
            if (CellLayout != null)
            {
                row = CellLayout.Row;
                column = CellLayout.Column;
            }
            BindingCell = OwnRow.OwnPanel.CellCache.GetCachedCell(row, column);
            if (BindingCell == null)
                return;

            Background = BindingCell.ActualBackground;
            var excel = Excel;
            Worksheet sheet = BindingCell.Worksheet;

            // 迷你图
            Sparkline sparkline = sheet.GetSparkline(row, column);
            if (_sparkInfo != sparkline)
            {
                SparkLine = sparkline;
                SynSparklineView();
            }

            // 收集所有DrawingObject
            List<DrawingObject> list = new List<DrawingObject>();
            DrawingObject[] objArray = sheet.GetDrawingObject(row, column, 1, 1);
            if ((objArray != null) && (objArray.Length > 0))
            {
                list.AddRange(objArray);
            }

            IDrawingObjectProvider drawingObjectProvider = DrawingObjectManager.GetDrawingObjectProvider(excel);
            if (drawingObjectProvider != null)
            {
                DrawingObject[] objArray2 = drawingObjectProvider.GetDrawingObjects(sheet, row, column, 1, 1);
                if ((objArray2 != null) && (objArray2.Length > 0))
                {
                    list.AddRange(objArray2);
                }
            }

            _dataBarObject = null;
            _iconObject = null;
            _customDrawingObject = null;
            if (list.Count > 0)
            {
                foreach (DrawingObject obj in list)
                {
                    if (obj is DataBarDrawingObject bar)
                    {
                        _dataBarObject = bar;
                    }
                    else if (obj is IconDrawingObject icon)
                    {
                        _iconObject = icon;
                    }
                    else if (obj is CustomDrawingObject cust)
                    {
                        _customDrawingObject = cust;
                    }
                }
            }

            bool noBarIcon = SynContitionalView();
            bool noCust = SynCustomDrawingObjectView();

            if (sparkline == null && noBarIcon && noCust && !string.IsNullOrEmpty(BindingCell.Text))
            {
                _tb.Text = BindingCell.Text;
                ApplyStyle();
            }
            else
            {
                _tb.ClearValue(TextBlock.TextProperty);
            }
            SynStrikethroughView();

#if UWP || WASM
            // 手机上体验不好
            if (!excel.IsTouching)
            {
                FilterButtonInfo info = excel.GetFilterButtonInfo(row, column, BindingCell.SheetArea);
                if (info != FilterButtonInfo)
                {
                    FilterButtonInfo = info;
                    SynFilterButton();
                }
            }
#endif
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Column == -1)
            {
                // TextBlock设置Padding时，若Padding左右之和大于Measure时给的Width，uno莫名报错，布局混乱，不易发现！
                foreach (UIElement elem in Children)
                {
                    elem.Measure(_szEmpty);
                }
                return _szEmpty;
            }

            if (_overflowLayout != null && _overflowLayout.ContentWidth > availableSize.Width)
                _tb.Measure(new Size(_overflowLayout.ContentWidth, availableSize.Height));
            else
                _tb.Measure(new Size(Math.Max(availableSize.Width - _textPadding * 2, 0.0), availableSize.Height));

            if (Children.Count > 1)
            {
                for (int i = 1; i < Children.Count; i++)
                {
                    ((UIElement)Children[i]).Measure(availableSize);
                }
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Column == -1 || finalSize.Width < _textPadding * 2)
            {
                foreach (UIElement elem in Children)
                {
                    elem.Arrange(_rcEmpty);
                }
                return _szEmpty;
            }

            Rect? nullable = null;
            if (_overflowLayout != null && _overflowLayout.ContentWidth > finalSize.Width)
            {
                double w;
                double left = 0;
                switch (BindingCell.ToHorizontalAlignment())
                {
                    case HorizontalAlignment.Left:
                    case HorizontalAlignment.Stretch:
                        left = _textPadding;
                        w = _overflowLayout.RightBackgroundWidth;
                        if (w >= 0.0)
                            nullable = new Rect(0.0, 0.0, w, finalSize.Height);
                        break;

                    case HorizontalAlignment.Right:
                        left = finalSize.Width - _overflowLayout.LeftBackgroundWidth;
                        w = _overflowLayout.LeftBackgroundWidth;
                        if (w >= 0.0)
                            nullable = new Rect(left, 0.0, w, finalSize.Height);
                        break;

                    default:
                        left -= (_overflowLayout.ContentWidth - finalSize.Width) / 2.0;
                        double x = 0.0;
                        if (_overflowLayout.LeftBackgroundWidth > 0.0)
                            x = (finalSize.Width / 2.0) - _overflowLayout.LeftBackgroundWidth;

                        w = _overflowLayout.BackgroundWidth;
                        if (w >= 0.0)
                            nullable = new Rect(x, 0.0, w, finalSize.Height);
                        break;
                }

                _tb.Arrange(new Rect(left, 0, _overflowLayout.ContentWidth + _textPadding * 2, finalSize.Height));
            }
            else
            {
                // 文字默认边距：4,0,4,0
                _tb.Arrange(new Rect(_textPadding, 0, finalSize.Width - _textPadding * 2, finalSize.Height));
            }

            if ((_cachedClip.HasValue != nullable.HasValue) || (_cachedClip.HasValue && (_cachedClip.Value != nullable.Value)))
            {
                _cachedClip = nullable;
                if (nullable.HasValue)
                {
                    Clip = new RectangleGeometry { Rect = nullable.Value };
                }
                else
                {
                    ClearValue(ClipProperty);
                }
            }

            if (Children.Count > 1)
            {
                Rect rect = new Rect(new Point(), finalSize);
                for (int i = 1; i < Children.Count; i++)
                {
                    ((UIElement)Children[i]).Arrange(rect);
                }
            }
            return finalSize;
        }
        #endregion

        #region 更新可视树
        bool SynContitionalView()
        {
            bool isContentVisible = true;
            if ((_dataBarObject != null) || (_iconObject != null))
            {
                if (_conditionalView == null)
                {
                    _conditionalView = new ConditionalFormatView(BindingCell);
                    Children.Add(_conditionalView);
                }
                _conditionalView.SetDataBarObject(_dataBarObject);
                if (_iconObject != null)
                {
                    _conditionalView.SetImageContainer();
                    _conditionalView.SetIconObject(_iconObject, Excel.ZoomFactor, BindingCell);
                }
                bool flag = true;
                if (flag && (_dataBarObject != null))
                {
                    flag = !_dataBarObject.ShowBarOnly;
                }
                if (flag && (_iconObject != null))
                {
                    flag = !_iconObject.ShowIconOnly;
                }
                isContentVisible = flag;
            }
            else if (_conditionalView != null)
            {
                Children.Remove(_conditionalView);
                _conditionalView = null;
            }
            return isContentVisible;
        }

        void SynStrikethroughView()
        {
            bool actualStrikethrough = BindingCell.ActualStrikethrough;
            if (_strikethroughView != null)
            {
                Children.Remove(_strikethroughView);
                _strikethroughView = null;
            }

            if (actualStrikethrough)
            {
                _strikethroughView = new StrikethroughView(BindingCell, this);
                Children.Add(_strikethroughView);
            }
        }

        bool SynCustomDrawingObjectView()
        {
            bool isContentVisible = true;
            if (_customDrawingObject != null)
            {
                isContentVisible = !_customDrawingObject.ShowDrawingObjectOnly;
                FrameworkElement rootElement = _customDrawingObject.RootElement;
                if (_customDrawingObjectView != rootElement)
                {
                    if (_customDrawingObjectView != null)
                    {
                        Children.Remove(_customDrawingObjectView);
                    }
                    _customDrawingObjectView = rootElement;
                    if (_customDrawingObjectView != null)
                    {
                        Panel parent = _customDrawingObjectView.Parent as Panel;
                        if ((parent != null) && (parent != this))
                        {
                            parent.Children.Remove(_customDrawingObjectView);
                        }
                        if (!Children.Contains(_customDrawingObjectView))
                        {
                            Children.Add(_customDrawingObjectView);
                        }
                    }
                }
            }
            else if (_customDrawingObjectView != null)
            {
                Children.Remove(_customDrawingObjectView);
                _customDrawingObjectView = null;
            }
            return isContentVisible;
        }

        void SynFilterButton()
        {
            if (FilterButtonInfo != null)
            {
                if (_filterButton == null)
                {
                    FilterButton element = new FilterButton(this);
                    element.HorizontalAlignment = HorizontalAlignment.Right;
                    element.VerticalAlignment = VerticalAlignment.Bottom;
                    _filterButton = element;
                    Children.Add(element);
                }
                else
                {
                    _filterButton.ApplyState();
                }
            }
            else if (_filterButton != null)
            {
                Children.Remove(_filterButton);
                _filterButton = null;
            }
        }
        #endregion

        #region 迷你图
        Sparkline SparkLine
        {
            get { return _sparkInfo; }
            set
            {
                if (_sparkInfo != value)
                {
                    DettachSparklineEvents();
                    if (_sparklineView != null)
                    {
                        Children.Remove(_sparklineView);
                        _sparklineView = null;
                    }
                    _sparkInfo = value;
                    AttachSparklineEvents();
                }
            }
        }

        void AttachSparklineEvents()
        {
            if (_sparkInfo != null)
                _sparkInfo.SparklineChanged += new EventHandler(sparkline_SparklineChanged);
        }

        void DettachSparklineEvents()
        {
            if (_sparkInfo != null)
                _sparkInfo.SparklineChanged -= new EventHandler(sparkline_SparklineChanged);
        }

        void sparkline_SparklineChanged(object sender, EventArgs e)
        {
            Sparkline sparkline = sender as Sparkline;
            if ((_sparklineView == null) || (_sparklineView.SparklineType != sparkline.SparklineType))
            {
                if (_sparklineView != null)
                {
                    Children.Remove(_sparklineView);
                    _sparklineView = null;
                }
                SynSparklineView();
            }
            else if (_sparklineView != null)
            {
                _sparklineView.Update(GetCellSize(), Excel.ZoomFactor);
            }
        }

        void SynSparklineView()
        {
            if (_sparkInfo != null)
            {
                if (_sparklineView == null)
                {
                    _sparklineView = CreateSparkline(_sparkInfo);
                    _sparklineView.ZoomFactor = OwnRow.OwnPanel.Excel.ZoomFactor;
                    ((IThemeContextSupport)_sparklineView).SetContext(OwnRow.OwnPanel.Excel.ActiveSheet);
                    Children.Add(_sparklineView);
                }
            }
            else if (_sparklineView != null)
            {
                DettachSparklineEvents();
                Children.Remove(_sparklineView);
                _sparklineView = null;
            }
        }

        BaseSparklineView CreateSparkline(Sparkline info)
        {
            if (info.SparklineType == SparklineType.Column)
            {
                return new ColumnSparklineView(new ColumnSparklineViewInfo(info));
            }
            if (info.SparklineType == SparklineType.Line)
            {
                return new LineSparklineView(new LineSparklineViewInfo(info));
            }
            return new WinLossSparklineView(new WinLossSparklineViewInfo(info));
        }
        #endregion

        #region 内部方法
        void ApplyStyle()
        {
            HorizontalAlignment horAlignment = BindingCell.ToHorizontalAlignment();
            if (_tb.HorizontalAlignment != horAlignment)
                _tb.HorizontalAlignment = horAlignment;

            // uno绘制Right位置错误，慎用TextAlignment！
            //Windows.UI.Xaml.TextAlignment textAlignment;
            //switch (horAlignment)
            //{
            //    case HorizontalAlignment.Center:
            //        textAlignment = Windows.UI.Xaml.TextAlignment.Center;
            //        break;
            //    case HorizontalAlignment.Right:
            //        textAlignment = Windows.UI.Xaml.TextAlignment.Right;
            //        break;
            //    default:
            //        textAlignment = Windows.UI.Xaml.TextAlignment.Left;
            //        break;
            //}
            //if (_tb.TextAlignment != textAlignment)
            //    _tb.TextAlignment = textAlignment;

            VerticalAlignment verAlignment;
            switch (BindingCell.ActualVerticalAlignment)
            {
                case CellVerticalAlignment.Top:
                    verAlignment = VerticalAlignment.Top;
                    break;
                case CellVerticalAlignment.Bottom:
                    verAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    verAlignment = VerticalAlignment.Center;
                    break;
            }
            if (_tb.VerticalAlignment != verAlignment)
                _tb.VerticalAlignment = verAlignment;

            var foreground = BindingCell.ActualForeground;
            if (foreground == null)
            {
                // 默认黑色
                if (_tb.ReadLocalValue(TextBlock.ForegroundProperty) != DependencyProperty.UnsetValue)
                    _tb.ClearValue(TextBlock.ForegroundProperty);
            }
            else if (foreground != _tb.Foreground)
            {
                _tb.Foreground = foreground;
            }

            var fontStyle = BindingCell.ActualFontStyle;
            if (_tb.FontStyle != fontStyle)
                _tb.FontStyle = fontStyle;

            var fontWeight = BindingCell.ActualFontWeight;
            if (_tb.FontWeight.Weight != fontWeight.Weight)
                _tb.FontWeight = fontWeight;

            var fontFamily = BindingCell.ActualFontFamily;
            if (fontFamily != null && _tb.FontFamily.Source != fontFamily.Source)
                _tb.FontFamily = fontFamily;

            bool wrap = BindingCell.ActualWordWrap;
            TextWrapping textWrap = wrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            if (_tb.TextWrapping != textWrap)
                _tb.TextWrapping = textWrap;

            double fontSize = BindingCell.ActualFontSize * ZoomFactor;
            double fitZoom = -1;
            if (!wrap && BindingCell.ActualShrinkToFit)
            {
                // 自动缩小字体适应单元格宽度
                double textWidth = MeasureHelper.MeasureText(
                    _tb.Text,
                    _tb.FontFamily,
                    fontSize,
                    _tb.FontStretch,
                    _tb.FontStyle,
                    _tb.FontWeight,
                    new Size(double.PositiveInfinity, double.PositiveInfinity),
                    false,
                    null,
                    _tb.UseLayoutRounding,
                    ZoomFactor).Width;
                double cellWidth = BindingCell.Worksheet.GetActualColumnWidth(BindingCell.Column.Index, BindingCell.ColumnSpan, BindingCell.SheetArea) * ZoomFactor;
                cellWidth = MeasureHelper.ConvertExcelCellSizeToTextSize(new Size(cellWidth, double.PositiveInfinity), ZoomFactor).Width;
                cellWidth = Math.Max((double)0.0, (double)(cellWidth - BindingCell.ActualTextIndent * ZoomFactor));
                if (cellWidth < textWidth)
                    fitZoom = cellWidth / textWidth;
            }
            if (fitZoom > 0)
                fontSize *= fitZoom;
            if (_tb.FontSize != fontSize)
                _tb.FontSize = fontSize;

            // TextBlock设置Padding时，若Padding左右之和大于Measure时给的Width，uno莫名报错，布局混乱，不易发现！
            Thickness margin = new Thickness();
            var indent = BindingCell.ActualTextIndent * ZoomFactor;
            if (indent > 0 && _tb.HorizontalAlignment != HorizontalAlignment.Center)
            {
                if (_tb.HorizontalAlignment == HorizontalAlignment.Right)
                    margin.Right += indent;
                else
                    margin.Left += indent;
            }
            if (_tb.Margin != margin)
                _tb.Margin = margin;

            // 未用到
            //var fontStretch = BindingCell.ActualFontStretch;
            //if (_tb.FontStretch != fontStretch)
            //    _tb.FontStretch = fontStretch;

            if (BindingCell.ActualUnderline)
            {
                Underline underline = new Underline();
                Run run = new Run();
                run.Text = _tb.Text;
                underline.Inlines.Add(run);
                _tb.Inlines.Clear();
                _tb.Inlines.Add(underline);
                _lastUnderline = true;
            }
            else if (_lastUnderline)
            {
                string str = _tb.Text;
                _tb.Inlines.Clear();
                _tb.Text = str;
            }

            if (BindingCell.ActualStrikethrough)
            {
                foreach (UIElement element in (_tb.Parent as Panel).Children)
                {
                    if (element is StrikethroughView)
                    {
                        StrikethroughView view = element as StrikethroughView;
                        if (view.LineContainer != null)
                        {
                            foreach (var line in view.LineContainer.Children.OfType<Line>())
                            {
                                line.Stroke = _tb.Foreground;
                            }
                        }
                        break;
                    }
                }
            }
        }

        Size GetCellSize()
        {
            if (CellLayout != null)
                return new Size(CellLayout.Width, CellLayout.Height);

            var colLayout = OwnRow.GetColumnLayoutModel().FindColumn(Column);
            var rowLayout = OwnRow.OwnPanel.GetRowLayoutModel().FindRow(Row);
            if (colLayout != null && rowLayout != null)
                return new Size(colLayout.Width, rowLayout.Height);
            return new Size();
        }
        #endregion
    }
}

