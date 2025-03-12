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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;
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
#if WIN || WASM || SKIA
                con.KeyDown -= pnl.OnKeyDown;
#else
                con.KeyUp -= pnl.OnKeyDown;
#endif
                pnl.Children.Remove(con);
            }

            con = (FrameworkElement)e.NewValue;
            if (con != null)
            {
                // win上KeyUp事件有怪异：Tab跳两格、CList选择后跳两格
                // 手机上KeyDown事件不触发！！！
#if WIN || WASM || SKIA
                con.KeyDown += pnl.OnKeyDown;
#else
                con.KeyUp += pnl.OnKeyDown;
#endif
                if (pnl.Children.Count > 0)
                    pnl.Children.Insert(pnl.Children.Count - 1, con);
            }
        }
        #endregion

        #region 成员变量
        Rectangle _rcTitle;
        TextBlock _tbTitle;
        Rectangle _rcQuery;
        Button _btnQuery;
        Rectangle _rcChild;
        FvCell _owner;
        #endregion

        public CellPanel()
        {
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
            InitChildren();
        }

        internal void UpdateChildren()
        {
            _rcChild = null;
            _rcTitle = null;
            _tbTitle = null;
            _rcQuery = null;
            if (_btnQuery != null)
            {
                _btnQuery.Click -= OnQueryClick;
                _btnQuery = null;
            }

            Children.Clear();
            InitChildren();
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

            // 标题
            if (_owner.ShowTitle)
            {
                Size size = new Size(_owner.TitleWidth, height);
                _rcTitle.Measure(size);
                _tbTitle.Measure(new Size(size.Width > 20 ? size.Width - 20 : 0, height));
            }

            // 查询比较符
            if (_owner.Query != QueryType.Disable)
            {
                Size size = new Size(_queryWidth, height);
                _rcQuery.Measure(size);
                _btnQuery.Measure(size);
            }

            // 内容
            double conWidth = width;
            if (_owner.ShowTitle)
                conWidth -= _owner.TitleWidth;
            if (_owner.Query != QueryType.Disable)
                conWidth -= _queryWidth;
            var child = Child;
            if (child != null)
            {
                // 左上空出边线
                child.Measure(new Size(conWidth > 1 ? conWidth - 1 : 0, height - 1));
                // 自动行高
                if (_owner.RowSpan == -1)
                    height = child.DesiredSize.Height > 0 ? child.DesiredSize.Height : Res.RowOuterHeight;
            }

            // 内容外框
            _rcChild?.Measure(new Size(conWidth > 0 ? conWidth : 0, height));
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
                conWidth -= _owner.TitleWidth;
            }

            // 查询比较符
            if (_owner.Query != QueryType.Disable)
            {
                var rc = new Rect(left, 0, _queryWidth, height);
                _rcQuery.Arrange(rc);
                _btnQuery.Arrange(rc);
                left += _queryWidth;
                conWidth -= _queryWidth;
            }

            // 内容，空出左上边线
            var child = Child;
            if (child != null)
                child.Arrange(conWidth > 1 ? new Rect(left + 1, 1, conWidth - 1, height - 1) : Res.HideRect);

            // 内容外框
            _rcChild?.Arrange(conWidth > 0 ? new Rect(left, 0, conWidth, height) : Res.HideRect);
            return finalSize;
        }

        void InitChildren()
        {
            // 标题
            if (_owner.ShowTitle)
            {
                // 背景及边框
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

#if WIN
                _tbTitle.IsTextTrimmedChanged += OnIsTextTrimmedChanged;
#endif

                Children.Add(_rcTitle);
                Children.Add(_tbTitle);
            }

            // 查询比较符
            if (_owner.Query != QueryType.Disable)
            {
                // 背景及边框
                _rcQuery = new Rectangle
                {
                    Fill = Res.浅灰1,
                    Stroke = Res.浅灰2,
                    IsHitTestVisible = false,
                    Margin = new Thickness(0, 0, -1, -1)
                };

                _btnQuery = new Button
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                _btnQuery.Click += OnQueryClick;
                OnQueryFlagChanged();

                Children.Add(_rcQuery);
                Children.Add(_btnQuery);
            }

            // 内容
            if (Child != null)
            {
                Children.Add(Child);
            }

            // 内容边框
            _rcChild = new Rectangle
            {
                Stroke = Res.浅灰2,
                IsHitTestVisible = false,
                Margin = new Thickness(0, 0, -1, -1)
            };
            Children.Add(_rcChild);
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_owner != null && _owner.Owner != null)
                _owner.Owner.OnCellClick(_owner, e);
        }

        /// <summary>
        /// 同步处理键盘特殊功能键操作
        /// 需要支持以下编辑键：
        /// Tab：              编辑状态下跳到下一格，支持跳过只读格、加 Ctrl 反跳
        /// Home, End：        首、未单元格跳
        /// Enter：            在非TextBox多行的情况下等同于 Tab
        /// Escape：           撤消编辑
        /// Ctrl + S           保存
        /// Ctrl + N           新建
        /// Ctrl + Delete      删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (_owner == null || _owner.Owner == null)
                return;

            // 外部处理输入，若 e.Handled = true 不再处理
            _owner.PostKeyUp(e);
            if (e.Handled)
                return;

            Fv fv = _owner.Owner;
            switch (e.Key)
            {
                case VirtualKey.Tab:
                    if (InputKit.IsCtrlPressed)
                    {
                        // 加 Ctrl 反跳
                        fv.GotoPreviousCell(_owner);
                    }
                    else
                    {
                        fv.GotoNextCell(_owner);
                    }
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
                    // 长文本回车跳格无法输入换行的bug
                    if (e.OriginalSource is TextBox tb
                        && tb.AcceptsReturn == true)
                        return;

                    if (InputKit.IsCtrlPressed)
                    {
                        // 加 Ctrl 反跳
                        fv.GotoPreviousCell(_owner);
                    }
                    else if (fv.IsLastCell(_owner))
                    {
                        if (!fv.DoLastCellEnter())
                            fv.GotoNextCell(_owner);
                    }
                    else
                    {
                        fv.GotoNextCell(_owner);
                    }
                    e.Handled = true;
                    break;

                case VirtualKey.S:
                    if (InputKit.IsCtrlPressed
                        && sender is UIElement con)
                    {
                        // Ctrl + S 保存
                        e.Handled = true;
                        // 必须在失去焦点事件中TextBox值才更新到数据源
                        con.LostFocus += OnConLostFocus;
                        fv.Focus(FocusState.Programmatic);
                    }
                    break;

                case VirtualKey.Escape:
                    _owner.RejectChanges();
                    fv.Focus(FocusState.Programmatic);
                    e.Handled = true;
                    break;
            }
        }

        void OnConLostFocus(object sender, RoutedEventArgs e)
        {
            ((UIElement)sender).LostFocus -= OnConLostFocus;
            _owner.Owner.OnSave();
        }

        /// <summary>
        /// 提示被截断的长文本
        /// </summary>
        /// <param name="p_tb"></param>
        /// <param name="e"></param>
        static void OnIsTextTrimmedChanged(TextBlock p_tb, IsTextTrimmedChangedEventArgs e)
        {
            p_tb.IsTextTrimmedChanged -= OnIsTextTrimmedChanged;
            ToolTipService.SetToolTip(p_tb, p_tb.Text);
        }
    }
}