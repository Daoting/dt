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
    /// 普通列表的行
    /// </summary>
    public partial class ListRow : LvRow
    {
        Button _btnMenu;

        public ListRow(Lv p_owner, DataTemplate p_temp) : base(p_owner)
        {
            LoadContent(p_temp.LoadContent() as UIElement);
            AttachEvent();
        }

        public ListRow(Lv p_owner, IRowView p_rowView, LvItem p_item) : base(p_owner)
        {
            LoadContent(p_rowView.Create(p_item));
            AttachEvent();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double height = Res.RowOuterHeight;
            double width = 0;
            int index = 0;

            // 选择框
            if (_owner.SelectionMode == SelectionMode.Multiple)
            {
                width = _flagWidth;
                ((UIElement)Children[index++]).Measure(new Size(width, availableSize.Height));
            }

            // 内容
            var elem = (UIElement)Children[index++];
            if (_btnMenu != null)
                width += _flagWidth;
            if (_owner.ItemHeight > 0)
            {
                height = _owner.ItemHeight;
                elem.Measure(new Size(Math.Max(availableSize.Width - width, 0), height));
            }
            else
            {
                // 自动行高
                elem.Measure(new Size(Math.Max(availableSize.Width - width, 0), availableSize.Height));
                if (elem.DesiredSize.Height > height)
                    height = elem.DesiredSize.Height;
            }

            // 上下文菜单
            if (_btnMenu != null)
            {
                index++;
                _btnMenu.Measure(new Size(_flagWidth, _flagWidth));
            }

            // 选择背景
            elem = (UIElement)Children[index++];
            Size size = new Size(availableSize.Width, height);
            elem.Measure(size);

            // 交互背景
            _rcPointer.Measure(size);
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rc = new Rect(new Point(), finalSize);
            double left = 0;
            int index = 0;

            // 选择框
            if (_owner.SelectionMode == SelectionMode.Multiple)
            {
                ((UIElement)Children[index++]).Arrange(new Rect(0, 0, _flagWidth, finalSize.Height));
                left = _flagWidth;
            }

            // 内容
            double right = (_btnMenu != null) ? _flagWidth : 0;
            ((UIElement)Children[index++]).Arrange(new Rect(left, 0, Math.Max(finalSize.Width - left - right, 0), finalSize.Height));

            // 上下文菜单，垂直居中
            if (_btnMenu != null)
            {
                index++;
                _btnMenu.Arrange(new Rect(finalSize.Width - _flagWidth, (finalSize.Height - _flagWidth) / 2, _flagWidth, _flagWidth));
            }

            // 选择背景
            ((UIElement)Children[index++]).Arrange(rc);

            // 交互背景
            _rcPointer.Arrange(rc);
            return finalSize;
        }

        void LoadContent(UIElement p_content)
        {
            Throw.IfNull(p_content);
            // 背景
            SetBinding(BackgroundProperty, new Binding { Path = new PropertyPath("Background") });

            // 普通列表多选时
            if (_owner.SelectionMode == SelectionMode.Multiple)
            {
                var tbCheck = new TextBlock { TextAlignment = Windows.UI.Xaml.TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontFamily = Res.IconFont };
                tbCheck.SetBinding(TextBlock.TextProperty, new Binding
                {
                    Path = new PropertyPath("IsSelected"),
                    Converter = new IsSelectedIconConverter(),
                });
                Children.Add(tbCheck);
            }

            // 内容
            Children.Add(p_content);

            // 上下文菜单
            Menu menu = Ex.GetMenu(_owner);
            if (menu != null)
            {
                _btnMenu = AttachContextMenu(menu);
                if (_btnMenu != null)
                    Children.Add(_btnMenu);
            }

            // 分割线及选择背景
            Border bd;
            if (_owner.ShowItemBorder)
            {
                bd = new Border { BorderBrush = Res.浅灰2, IsHitTestVisible = false };
                // 设置宽度或最大宽度时显示右边框
                if (!Kit.IsPhoneUI && (!double.IsPositiveInfinity(_owner.MaxWidth) || !double.IsNaN(_owner.Width)))
                    bd.BorderThickness = new Thickness(0, 0, 1, 1);
                else
                    bd.BorderThickness = new Thickness(0, 0, 0, 1);
            }
            else
            {
                bd = new Border { IsHitTestVisible = false };
            }

            if (_owner.SelectionMode != SelectionMode.None)
            {
                bd.SetBinding(Border.BackgroundProperty, new Binding
                {
                    Path = new PropertyPath("IsSelected"),
                    Converter = new SelectedBackgroundConverter(),
                });
            }
            Children.Add(bd);

            // 交互背景
            _rcPointer = new Rectangle { IsHitTestVisible = false };
            Children.Add(_rcPointer);
        }
    }
}