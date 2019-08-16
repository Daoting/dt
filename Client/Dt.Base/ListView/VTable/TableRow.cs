#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 表格视图的行
    /// </summary>
    public partial class TableRow : LvRow
    {
        #region 成员变量
        internal static Thickness TextMargin = new Thickness(10, 4, 10, 4);
        readonly Dictionary<string, UIElement> _cells;
        #endregion

        #region 构造方法
        public TableRow(Lv p_owner) : base(p_owner)
        {
            _cells = new Dictionary<string, UIElement>();
            LoadCells();
            AttachEvent();
        }
        #endregion

        protected override Size MeasureOverride(Size availableSize)
        {
            // 行最小高度41
            double height = AtRes.RowOuterHeight;

            // 行单元格
            Cols cols = _owner.Cols;
            if (double.IsNaN(_owner.ItemHeight))
            {
                // 自动行高
                for (int i = 0; i < cols.Count; i++)
                {
                    Col col = cols[i];
                    var elem = _cells[col.ID];
                    elem.Measure(new Size(cols[i].Width, availableSize.Height));
                    if (elem.DesiredSize.Height > height)
                        height = elem.DesiredSize.Height;
                }
            }
            else
            {
                if (_owner.ItemHeight > 0)
                    height = _owner.ItemHeight;

                for (int i = 0; i < cols.Count; i++)
                {
                    Col col = cols[i];
                    _cells[col.ID].Measure(new Size(cols[i].Width, height));
                }
            }

            // 行头
            Grid header = (Grid)Children[cols.Count];
            header.Measure(new Size(header.Width, height));

            // 选择背景
            if (_owner.SelectionMode != SelectionMode.None)
                ((UIElement)Children[cols.Count + 1]).Measure(new Size(cols.TotalWidth, height));

            // 交互背景
            Size size = new Size(cols.TotalWidth + header.Width, height);
            _rcPointer.Measure(size);
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Cols cols = _owner.Cols;

            // 行头
            Grid header = (Grid)Children[cols.Count];
            double headerWidth = header.Width;
            header.Arrange(new Rect(_owner.Scroll.HorizontalOffset, 0, headerWidth, finalSize.Height));

            // 行单元格
            for (int i = 0; i < cols.Count; i++)
            {
                Col col = cols[i];
                _cells[col.ID].Arrange(new Rect(col.Left + headerWidth, 0, col.Width, finalSize.Height));
            }

            // 选择背景
            if (_owner.SelectionMode != SelectionMode.None)
                ((UIElement)Children[cols.Count + 1]).Arrange(new Rect(headerWidth, 0, cols.TotalWidth, finalSize.Height));

            // 交互背景
            _rcPointer.Arrange(new Rect(new Point(), finalSize));
            return finalSize;
        }

        void LoadCells()
        {
            Cols cols = _owner.Cols;
            cols.Update += (s, e) => InvalidateMeasure();
            _owner.Scroll.ViewChanged += (s, e) =>
            {
                _rcPointer.Fill = null;
                InvalidateArrange();
            };

            // 背景
            SetBinding(BackgroundProperty, new Binding { Path = new PropertyPath("Background") });

            // 单元格
            Thickness borderLine = _owner.ShowItemBorder ? new Thickness(0, 0, 1, 1) : new Thickness(0, 0, 1, 0);
            foreach (var col in cols)
            {
                ContentPresenter pre = new ContentPresenter { Padding = TextMargin, BorderBrush = AtRes.浅灰边框, BorderThickness = borderLine, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                pre.SetBinding(ContentPresenter.ContentProperty, new Binding { Path = new PropertyPath($"[{col.ID}]"), Mode = BindingMode.OneTime });
                pre.SetBinding(ContentPresenter.ForegroundProperty, new Binding { Path = new PropertyPath("Foreground") });
                pre.SetBinding(ContentPresenter.FontWeightProperty, new Binding { Path = new PropertyPath("FontWeight") });
                pre.SetBinding(ContentPresenter.FontStyleProperty, new Binding { Path = new PropertyPath("FontStyle") });
                pre.SetBinding(ContentPresenter.FontSizeProperty, new Binding { Path = new PropertyPath("FontSize") });
                Children.Add(pre);
                _cells[col.ID] = pre;
            }

            // 行头
            Grid header = new Grid { Background = AtRes.浅灰背景 };
            if (_owner.SelectionMode != SelectionMode.None)
            {
                header.SetBinding(BackgroundProperty, new Binding
                {
                    Path = new PropertyPath("IsSelected"),
                    Converter = new HeaderBackgroundConverter(),
                });
            }
            var bd = new Border { BorderBrush = AtRes.浅灰边框, BorderThickness = borderLine, IsHitTestVisible = false };
            header.Children.Add(bd);
            TextBlock tb = new TextBlock { TextAlignment = Windows.UI.Xaml.TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            tb.SetBinding(TextBlock.TextProperty, new Binding { Path = new PropertyPath("Index") });
            header.Children.Add(tb);

            if (_owner.SelectionMode == SelectionMode.Multiple)
            {
                header.Width = 81;
                header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                TextBlock tbCheck = new TextBlock { VerticalAlignment = VerticalAlignment.Center, TextAlignment = Windows.UI.Xaml.TextAlignment.Center, FontFamily = AtRes.IconFont };
                tbCheck.SetBinding(TextBlock.TextProperty, new Binding
                {
                    Path = new PropertyPath("IsSelected"),
                    Converter = new IsSelectedIconConverter(),
                });
                Grid.SetColumn(tbCheck, 1);
                Grid.SetColumnSpan(bd, 2);
                header.Children.Add(tbCheck);
            }
            else
            {
                header.Width = 40;
            }
            Children.Add(header);

            // 选择背景
            if (_owner.SelectionMode != SelectionMode.None)
            {
                var rc = new Rectangle { IsHitTestVisible = false };
                rc.SetBinding(Rectangle.FillProperty, new Binding
                {
                    Path = new PropertyPath("IsSelected"),
                    Converter = new SelectedBackgroundConverter(),
                });
                Children.Add(rc);
            }

            // 交互背景
            _rcPointer = new Rectangle { IsHitTestVisible = false };
            Children.Add(_rcPointer);

            // 上下文菜单
            Menu menu = Ex.GetMenu(_owner);
            if (menu != null)
            {
                // 只支持右键或长按！
                if (AtSys.IsTouchMode)
                {
                    Holding += (s, e) =>
                    {
                        if (e.HoldingState == HoldingState.Started)
                            OpenContextMenu(e.GetPosition(null));
                    };
                }
                if (AtSys.System == TargetSystem.Windows)
                    RightTapped += (s, e) => OpenContextMenu(e.GetPosition(null));
            }
        }
    }
}
