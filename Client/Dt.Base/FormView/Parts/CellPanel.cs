#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 单元格内部布局面板，为uno节省可视树级数
    /// </summary>
    [ContentProperty(Name = "Child")]
    public partial class CellPanel : Panel
    {
        #region 静态内容
        /// <summary>
        /// 单元格内容
        /// </summary>
        public readonly static DependencyProperty ChildProperty = DependencyProperty.Register(
            "Child",
            typeof(FrameworkElement),
            typeof(CellPanel),
            new PropertyMetadata(null, OnChildPropertyChanged));

        static void OnChildPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CellPanel pnl = (CellPanel)d;
            FrameworkElement con = (FrameworkElement)e.OldValue;
            if (con != null)
            {
                pnl.Children.Remove(con);
                con.KeyDown -= pnl.OnKeyDown;
            }

            con = (FrameworkElement)e.NewValue;
            if (con != null)
            {
                if (pnl.Children.Count == 1)
                    pnl.Children.Insert(0, con);
                else if (pnl.Children.Count == 3)
                    pnl.Children.Insert(2, con);
                con.KeyDown += pnl.OnKeyDown;
            }
        }
        #endregion

        #region 成员变量
        Rectangle _rcTitle;
        TextBlock _tbTitle;
        readonly Rectangle _rcChild;
        FvCell _owner;
        #endregion

        public CellPanel()
        {
            // 内容边框
            _rcChild = new Rectangle
            {
                Stroke = Res.浅灰2,
                IsHitTestVisible = false,
                Margin = new Thickness(0, 0, -1, -1)
            };
            Children.Add(_rcChild);
            PointerPressed += OnPointerPressed;
        }

        /// <summary>
        /// 获取设置单元格内容
        /// </summary>
        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        internal void SetOwner(FvCell p_cell)
        {
            _owner = p_cell;
            OnShowTitleChanged();
        }

        internal void OnShowTitleChanged()
        {
            if (_owner.ShowTitle)
            {
                if (_rcTitle == null)
                {
                    // 标题背景及边框
                    _rcTitle = new Rectangle
                    {
                        Fill = Res.浅灰1,
                        Stroke = Res.浅灰2,
                        IsHitTestVisible = false,
                        Margin = new Thickness(0, 0, -1, -1)
                    };

                    // 标题
                    _tbTitle = new TextBlock
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.NoWrap,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    };
                    Binding bind = new Binding { Path = new PropertyPath("Title"), Source = _owner };
                    _tbTitle.SetBinding(TextBlock.TextProperty, bind);
                }

                Children.Insert(0, _rcTitle);
                Children.Insert(1, _tbTitle);
            }
            else if (Children.Count > 2)
            {
                Children.RemoveAt(0);
                Children.RemoveAt(0);
            }
        }

        internal void OnIsVerticalTitleChanged()
        {
            if (!_owner.ShowTitle || _tbTitle == null)
                return;

            if (_owner.IsVerticalTitle)
            {
                _tbTitle.TextWrapping = TextWrapping.Wrap;
                _tbTitle.Width = 15;
            }
            else
            {
                _tbTitle.TextWrapping = TextWrapping.NoWrap;
                _tbTitle.ClearValue(WidthProperty);
            }
        }

        /// <summary>
        /// Cell的值变化
        /// </summary>
        /// <param name="p_isChanged"></param>
        internal void ToggleIsChanged(bool p_isChanged)
        {
            _rcChild.Fill = p_isChanged ? Res.黄遮罩 : null;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_owner == null || availableSize.Width == 0 || availableSize.Height == 0)
                return base.MeasureOverride(availableSize);

            double width = availableSize.Width;
            double height = availableSize.Height;

            // 内容
            double conWidth = _owner.ShowTitle ? width - _owner.TitleWidth : width;
            var child = Child;
            if (child != null)
            {
                // 左上空出边线
                child.Measure(new Size(conWidth - 1, height - 1));
                // 自动行高
                if (_owner.RowSpan == -1)
                    height = child.DesiredSize.Height > 0 ? child.DesiredSize.Height : Res.RowOuterHeight;
            }

            // 内容外框
            _rcChild.Measure(new Size(conWidth, height));

            // 标题
            if (_owner.ShowTitle)
            {
                Size size = new Size(_owner.TitleWidth, height);
                _rcTitle.Measure(size);
                _tbTitle.Measure(new Size(size.Width - 20, height));
            }
            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_owner == null)
                return finalSize;

            double width = finalSize.Width;
            double height = finalSize.Height;
            double left = 0;
            double conWidth = width;

            // 标题
            if (_owner.ShowTitle)
            {
                _rcTitle.Arrange(new Rect(0, 0, _owner.TitleWidth, height));
                left = _owner.IsVerticalTitle ? (_owner.TitleWidth - _tbTitle.DesiredSize.Width) / 2 : 10;
                _tbTitle.Arrange(new Rect(left, 0, _tbTitle.DesiredSize.Width, height));
                left = _owner.TitleWidth;
                conWidth = width - _owner.TitleWidth;
            }

            // 内容，空出左上边线
            var child = Child;
            if (child != null)
                child.Arrange(new Rect(left + 1, 1, conWidth - 1, height - 1));

            // 内容外框
            _rcChild.Arrange(new Rect(left, 0, conWidth, height));
            return finalSize;
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_owner != null && _owner.Owner != null)
                _owner.Owner.OnCellClick(_owner);
        }

        /// <summary>
        /// 同步处理键盘特殊功能键操作
        /// 需要支持以下编辑键：
        /// Tab：              编辑状态下跳到下一格，未格跳到下行第一格，支持跳过只读格、加shift反跳
        /// Home, End：        首、未单元格跳
        /// Enter：            在非TextBox多行的情况下等同于Tab，Ctrl + Enter 触发编辑结束事件
        /// Escape：           撤消编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (_owner == null || _owner.Owner == null)
                return;

            Fv fv = _owner.Owner;
            switch (e.Key)
            {
                case VirtualKey.Tab:
                    fv.GotoNextCell(_owner);
                    e.Handled = true;
                    break;
                case VirtualKey.Home:
                    fv.GotoFirstCell();
                    e.Handled = true;
                    break;
                case VirtualKey.End:
                    fv.GotoLastCell();
                    e.Handled = true;
                    break;
                case VirtualKey.Enter:
#if IOS || WASM
                    // uno中长文本回车跳格无法输入换行的bug
                    if (e.OriginalSource is TextBox tb && tb.AcceptsReturn == true)
                        return;
#endif
                    fv.GotoNextCell(_owner);
                    e.Handled = true;
                    break;
                case VirtualKey.Escape:
                    _owner.RejectChanges();
                    fv.Focus(FocusState.Programmatic);
                    e.Handled = true;
                    break;
            }
        }
    }
}