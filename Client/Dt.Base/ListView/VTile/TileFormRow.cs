#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 表单磁贴的行
    /// </summary>
    public partial class TileFormRow : LvRow
    {
        public TileFormRow(Lv p_owner) : base(p_owner)
        {
            LoadCols();
            AttachEvent();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // 行最小高度41
            double height = Res.RowOuterHeight;
            UIElement elem = (UIElement)Children[0];
            elem.Measure(availableSize);
            if (elem.DesiredSize.Height > height)
                height = elem.DesiredSize.Height;
            ((UIElement)Children[1]).Measure(availableSize);
            ((UIElement)Children[2]).Measure(availableSize);
            return new Size(availableSize.Width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rc = new Rect(new Point(), finalSize);
            foreach (var elem in Children)
            {
                ((UIElement)elem).Arrange(rc);
            }
            return finalSize;
        }

        /// <summary>
        /// 加载表单列表
        /// </summary>
        /// <returns></returns>
        void LoadCols()
        {
            // 背景
            SetBinding(BackgroundProperty, new Binding { Path = new PropertyPath("Background") });

            Cols cols = _owner.Cols;
            if (cols == null || cols.Count == 0)
                throw new Exception("列定义不可为空！");

            var bdThickness = new Thickness(0, 0, 0, 1);
            StackPanel root = new StackPanel();

            // 功能行
            if (!cols.HideIndex || _owner.SelectionMode == SelectionMode.Multiple)
            {
                Grid grid = new Grid { Height = Res.RowOuterHeight };
                var bd = new Border
                {
                    Background = Res.浅灰1,
                    BorderBrush = Res.浅灰2,
                    BorderThickness = bdThickness,
                    IsHitTestVisible = false,
                };
                grid.Children.Add(bd);

                // 行号
                if (!cols.HideIndex)
                {
                    StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };
                    TextBlock tb = new TextBlock { Text = "\uE013", Margin = TableRow.TextMargin, VerticalAlignment = VerticalAlignment.Center, FontFamily = Res.IconFont };
                    sp.Children.Add(tb);
                    tb = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
                    tb.SetBinding(TextBlock.TextProperty, new Binding { Path = new PropertyPath("FullIndex") });
                    sp.Children.Add(tb);
                    grid.Children.Add(sp);
                }

                // 多选时的选择框
                if (_owner.SelectionMode == SelectionMode.Multiple)
                {
                    var tbCheck = new TextBlock { TextAlignment = Windows.UI.Xaml.TextAlignment.Right, VerticalAlignment = VerticalAlignment.Center, Margin = TableRow.TextMargin, FontFamily = Res.IconFont };
                    var bind = new Binding
                    {
                        Path = new PropertyPath("IsSelected"),
                        Converter = new IsSelectedIconConverter(),
                    };
                    tbCheck.SetBinding(TextBlock.TextProperty, bind);
                    grid.Children.Add(tbCheck);
                }
                root.Children.Add(grid);
            }

            for (int i = 0; i < cols.Count; i++)
            {
                Col col = cols[i];
                Grid grid = new Grid
                {
                    Height = col.RowSpan * Res.RowOuterHeight,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(140) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                };

                // 标题背景及边框
                var bd = new Border
                {
                    Background = Res.浅灰1,
                    BorderBrush = Res.浅灰2,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    IsHitTestVisible = false,
                };
                grid.Children.Add(bd);

                // 标题
                var tb = new TextBlock
                {
                    Text = col.Title,
                    Margin = TableRow.TextMargin,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.NoWrap,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                grid.Children.Add(tb);

                // 内容
                ContentPresenter pre = new ContentPresenter { Padding = TableRow.TextMargin, BorderBrush = Res.浅灰2, BorderThickness = bdThickness, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                SetContentBinding(col, pre);
                Grid.SetColumn(pre, 1);
                grid.Children.Add(pre);

                root.Children.Add(grid);
            }
            Children.Add(root);

            // 选择状态背景
            var bdSelected = new Border { IsHitTestVisible = false };
            if (_owner.SelectionMode != SelectionMode.None)
            {
                bdSelected.SetBinding(Border.BackgroundProperty, new Binding
                {
                    Path = new PropertyPath("IsSelected"),
                    Converter = new SelectedBackgroundConverter(),
                });
            }
            Children.Add(bdSelected);

            // 交互背景
            _rcPointer = new Rectangle { IsHitTestVisible = false };
            Children.Add(_rcPointer);
        }
    }
}