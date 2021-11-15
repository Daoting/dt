#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 浮动面板容器
    /// </summary>
    [ContentProperty(Name = "Child")]
    public partial class Fly : Flyout
    {
        #region 静态内容
        /// <summary>
        /// 浮动面板内容
        /// </summary>
        public readonly static DependencyProperty ChildProperty = DependencyProperty.Register(
            "Child",
            typeof(UIElement),
            typeof(Fly),
            new PropertyMetadata(null));

        /// <summary>
        /// 是否可调节大小
        /// </summary>
        public static readonly DependencyProperty ResizeableProperty = DependencyProperty.Register(
            "Resizeable",
            typeof(bool),
            typeof(Fly),
            new PropertyMetadata(false, OnResizeableChanged));

        /// <summary>
        /// 是否可改变大小属性改变触发事件。
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        static void OnResizeableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Fly)d).OrganizeThumb();
        }
        #endregion

        #region 成员变量
        Grid _rootGrid;
        ScrollViewer _sv;
        ContentPresenter _pre;
        Grid _grid;
        #endregion

        #region 构造方法
        public Fly()
        {
            Init();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 尺寸调节结束事件
        /// </summary>
        public event EventHandler AfterResized;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置浮动面板内容
        /// </summary>
        public UIElement Child
        {
            get { return (UIElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        /// <summary>
        /// 获取设置边距
        /// </summary>
        public Thickness Padding
        {
            get { return _pre.Margin; }
            set { _pre.Margin = value; }
        }

        /// <summary>
        /// 获取设置面板宽度
        /// </summary>
        public double Width
        {
            get { return _rootGrid.Width; }
            set { _rootGrid.Width = value; }
        }

        /// <summary>
        /// 获取设置面板最小宽度
        /// </summary>
        public double MinWidth
        {
            get { return _rootGrid.MinWidth; }
            set { _rootGrid.MinWidth = value; }
        }

        /// <summary>
        /// 获取设置面板高度
        /// </summary>
        public double Height
        {
            get { return _rootGrid.Height; }
            set { _rootGrid.Height = value; }
        }

        /// <summary>
        /// 获取设置面板最小高度
        /// </summary>
        public double MinHeight
        {
            get { return _rootGrid.MinHeight; }
            set { _rootGrid.MinHeight = value; }
        }

        /// <summary>
        /// 获取设置面板背景颜色
        /// </summary>
        public Brush Background
        {
            get { return _rootGrid.Background; }
            set { _rootGrid.Background = value; }
        }

        /// <summary>
        /// 获取设置是否可调节大小，默认为false
        /// </summary>
        public bool Resizeable
        {
            get { return (bool)GetValue(ResizeableProperty); }
            set { SetValue(ResizeableProperty, value); }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 初始化，创建简易结构
        /// </summary>
        void Init()
        {
            _rootGrid = new Grid()
            {
                MinWidth = 40d,
                MinHeight = 40d,
                Background = Res.默认背景,
                BorderThickness = new Thickness(1d),
                BorderBrush = Res.浅灰2
            };

            _sv = new ScrollViewer
            {
                ZoomMode = ZoomMode.Disabled,
                HorizontalScrollMode = ScrollMode.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollMode = ScrollMode.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            };

            _pre = new ContentPresenter { FontSize = 16 };
            _pre.SetBinding(ContentPresenter.ContentProperty, new Binding { Path = new PropertyPath("Child"), Source = this });

            _sv.Content = _pre;
            _rootGrid.Children.Add(_sv);
            Content = _rootGrid;

            Opening += OnOpening;
        }

        /// <summary>
        /// 打开时确定最大宽高。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnOpening(object sender, object e)
        {
            _rootGrid.MaxWidth = Kit.ViewWidth;
            _rootGrid.MaxHeight = Kit.ViewHeight;

            Rect rect = Target.GetBounds();
            switch (Placement)
            {
                case FlyoutPlacementMode.Left:
                    _rootGrid.MaxWidth = rect.X;
                    break;
                case FlyoutPlacementMode.Top:
                    _rootGrid.MaxHeight = rect.Y;
                    break;
                case FlyoutPlacementMode.Right:
                    _rootGrid.MaxWidth = _rootGrid.MaxWidth - rect.Right;
                    break;
                case FlyoutPlacementMode.Bottom:
                    _rootGrid.MaxHeight = _rootGrid.MaxHeight - rect.Bottom;
                    break;
            }
        }
        #endregion

        #region 调节大小
        /// <summary>
        /// 根据是否可调节大小改变可视树内容。
        /// </summary>
        void OrganizeThumb()
        {
            if (Resizeable)
            {
                if (_grid == null)
                    CreateResizeParts();
                AddResizeParts();
            }
            else
            {
                RemoveResizeParts();
            }
        }

        /// <summary>
        /// 创建调节大小所需的控件
        /// </summary>
        void CreateResizeParts()
        {
            // 创建拖动手柄容器
            _grid = new Grid();
            _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            _grid.RowDefinitions.Add(new RowDefinition());
            _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            _grid.ColumnDefinitions.Add(new ColumnDefinition());
            _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // 创建拖动手柄
            Thumb topThumb = new Thumb() { Margin = new Thickness(0, -10, 0, 0) };
            topThumb.SetCursor(CoreCursorType.SizeNorthSouth);
            topThumb.DragStarted += OnResizeStarted;
            topThumb.DragDelta += OnTopDragDelta;
            topThumb.DragCompleted += OnResizeCompleted;
            Grid.SetRow(topThumb, 0);
            Grid.SetColumn(topThumb, 1);
            _grid.Children.Add(topThumb);

            Thumb leftThumb = new Thumb() { Margin = new Thickness(-10, 0, 0, 0) };
            leftThumb.SetCursor(CoreCursorType.SizeWestEast);
            leftThumb.DragStarted += OnResizeStarted;
            leftThumb.DragDelta += OnLeftDragDelta;
            leftThumb.DragCompleted += OnResizeCompleted;
            Grid.SetRow(leftThumb, 1);
            Grid.SetColumn(leftThumb, 0);
            _grid.Children.Add(leftThumb);

            Thumb rightThumb = new Thumb() { Margin = new Thickness(0, 0, -10, 5) };
            rightThumb.SetCursor(CoreCursorType.SizeWestEast);
            rightThumb.DragStarted += OnResizeStarted;
            rightThumb.DragDelta += OnRightDragDelta;
            rightThumb.DragCompleted += OnResizeCompleted;
            Grid.SetRow(rightThumb, 1);
            Grid.SetColumn(rightThumb, 2);
            _grid.Children.Add(rightThumb);

            Thumb bottomThumb = new Thumb() { Margin = new Thickness(0, 0, 5, -10) };
            bottomThumb.SetCursor(CoreCursorType.SizeNorthSouth);
            bottomThumb.DragStarted += OnResizeStarted;
            bottomThumb.DragDelta += OnBottomDragDelta;
            bottomThumb.DragCompleted += OnResizeCompleted;
            Grid.SetRow(bottomThumb, 2);
            Grid.SetColumn(bottomThumb, 1);
            _grid.Children.Add(bottomThumb);

            Thumb rightBottomThumb = new Thumb() { Margin = new Thickness(0, 0, -5, -5) };
            rightBottomThumb.SetCursor(CoreCursorType.SizeNorthwestSoutheast);
            rightBottomThumb.DragStarted += OnResizeStarted;
            rightBottomThumb.DragDelta += OnRightBottomDragDelta;
            rightBottomThumb.DragCompleted += OnResizeCompleted;
            Grid.SetRow(rightBottomThumb, 2);
            Grid.SetColumn(rightBottomThumb, 2);
            _grid.Children.Add(rightBottomThumb);
        }

        /// <summary>
        /// 向可视树中增加调节大小用到的控件
        /// </summary>
        void AddResizeParts()
        {
            _rootGrid.Children.Add(_grid);
        }

        /// <summary>
        /// 从可视树中移除调节大小用到的控件
        /// </summary>
        void RemoveResizeParts()
        {
            _rootGrid.Children.Remove(_grid);
        }

        /// <summary>
        /// 改变大小时显示红色边框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnResizeStarted(object sender, DragStartedEventArgs e)
        {
            _rootGrid.BorderBrush = Res.亮红;
        }

        void OnTopDragDelta(object sender, DragDeltaEventArgs e)
        {
            double height = _rootGrid.ActualHeight;
            height -= e.VerticalChange;
            if (height < _rootGrid.MinHeight)
                height = _rootGrid.MinHeight;
            if (height > _rootGrid.MaxHeight)
                height = _rootGrid.MaxHeight;

            _rootGrid.Height = height;
        }

        void OnLeftDragDelta(object sender, DragDeltaEventArgs e)
        {
            double width = _rootGrid.ActualWidth;
            width -= e.HorizontalChange;
            if (width < _rootGrid.MinWidth)
                width = _rootGrid.MinWidth;
            if (width > _rootGrid.MaxWidth)
                width = _rootGrid.MaxWidth;

            _rootGrid.Width = width;
        }

        void OnRightDragDelta(object sender, DragDeltaEventArgs e)
        {
            double width = _rootGrid.ActualWidth;
            width += e.HorizontalChange;
            if (width < _rootGrid.MinWidth)
                width = _rootGrid.MinWidth;
            if (width > _rootGrid.MaxWidth)
                width = _rootGrid.MaxWidth;

            _rootGrid.Width = width;
        }

        void OnBottomDragDelta(object sender, DragDeltaEventArgs e)
        {
            double height = _rootGrid.ActualHeight;
            height += e.VerticalChange;
            if (height < _rootGrid.MinHeight)
                height = _rootGrid.MinHeight;
            if (height > _rootGrid.MaxHeight)
                height = _rootGrid.MaxHeight;

            _rootGrid.Height = height;
        }

        void OnRightBottomDragDelta(object sender, DragDeltaEventArgs e)
        {
            double height = _rootGrid.ActualHeight;
            double width = _rootGrid.ActualWidth;
            width += e.HorizontalChange;
            if (width < _rootGrid.MinWidth)
                width = _rootGrid.MinWidth;
            if (width > _rootGrid.MaxWidth)
                width = _rootGrid.MaxWidth;
            height += e.VerticalChange;
            if (height < _rootGrid.MinHeight)
                height = _rootGrid.MinHeight;
            if (height > _rootGrid.MaxHeight)
                height = _rootGrid.MaxHeight;

            _rootGrid.Width = width;
            _rootGrid.Height = height;
        }

        /// <summary>
        /// 改变大小结束，恢复边框颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnResizeCompleted(object sender, DragCompletedEventArgs e)
        {
            _rootGrid.BorderBrush = Res.浅灰2;
            AfterResized?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
