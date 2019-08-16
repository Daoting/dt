#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 尺寸调节器，在Win内部容器之间调节大小
    /// </summary>
    public partial class GridResizer : Control
    {
        #region 静态内容
        /// <summary>
        /// 开始调整事件
        /// </summary>
        public static BaseRoutedEvent PreviewResizeStartEvent;

        internal static BaseRoutedEvent LayoutChangeEndedEvent;

        /// <summary>
        /// 放置位置
        /// </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(
            "Placement", 
            typeof(ItemPlacement?), 
            typeof(GridResizer), 
            new PropertyMetadata(null, OnPlacementChanged));

        static void OnPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GridResizer)d).ChangeVisualState();
        }

#if UWP
        static GridResizer()
        {
            PreviewResizeStartEvent = EventManager.RegisterRoutedEvent(
                "PreviewResizeStart",
                RoutingStrategy.Tunnel,
                typeof(EventHandler<ResizeEventArgs>),
                typeof(GridResizer));

            LayoutChangeEndedEvent = EventManager.RegisterRoutedEvent(
                "LayoutChangeEnded",
                RoutingStrategy.Bubble,
                typeof(EventHandler<BaseRoutedEventArgs>),
                typeof(GridResizer));
        }
#endif
        #endregion

        #region 成员变量
        internal const double ResizerSize = 4;
        PreviewControl _preview;
        Panel _previewLayer;
        ResizeData _resizeData;
        bool _dragging;
        Point _start;
        #endregion

        /// <summary>
        ///
        /// </summary>
        public GridResizer()
        {
            DefaultStyleKey = typeof(GridResizer);
        }

        #region 属性
        /// <summary>
        /// 获取设置放置位置
        /// </summary>
        /// <value>The placement.</value>
        public ItemPlacement? Placement
        {
            get { return (ItemPlacement?)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        internal ResizeData Data
        {
            get { return _resizeData; }
        }

        bool HasKeyboardFocus
        {
            get { return (FocusManager.GetFocusedElement() == this); }
        }

        /// <summary>
        /// 拖动过程中的位置预览
        /// </summary>
        internal PreviewControl Preview
        {
            get
            {
                if (_preview == null)
                {
                    _preview = new PreviewControl();
                    if (_previewLayer == null)
                    {
                        CreatePreviewLayer();
                    }
                    _previewLayer.Children.Add(_preview);
                }
                return _preview;
            }
        }
        #endregion

        #region 重写方法
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            _dragging = CapturePointer(e.Pointer);
            if (_dragging)
            {
                _start = e.GetCurrentPoint(null).Position;
                Focus(FocusState.Programmatic);
                InitializeData();
                SetupPreview();
            }
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            if (_dragging)
            {
                Point position = e.GetCurrentPoint(null).Position;
                double offsetX = position.X - _start.X;
                double offsetY = position.Y - _start.Y;
                offsetX = (FlowDirection == FlowDirection.RightToLeft) ? -offsetX : offsetX;
                if (_resizeData != null)
                {
                    MoveSplitter(offsetX, offsetY, true);
                }
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (_dragging)
            {
                _dragging = false;
                ReleasePointerCapture(e.Pointer);

                Point position = e.GetCurrentPoint(null).Position;
                Point point = new Point(position.X - _start.X, position.Y - _start.Y);

                if (_resizeData != null)
                {
                    Canvas.SetZIndex(_resizeData.ResizedTgt, 0);
                    MoveSplitter(_resizeData.Preview.OffsetX, _resizeData.Preview.OffsetY, false);
                    RemovePreview();
                    _resizeData.ClearReferences();
                    _resizeData = null;
                }
                ChangeVisualState();
                OnLayoutChangeEnded();
            }
        }

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            base.OnPointerCaptureLost(e);
            if (_dragging)
            {
                _dragging = false;
                if (_resizeData != null)
                {
                    CancelResize();
                }
                ChangeVisualState();
                OnLayoutChangeEnded();
            }
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case VirtualKey.Left:
                    e.Handled = KeyboardMoveSplitter(-10.0, 0.0);
                    return;

                case VirtualKey.Up:
                    e.Handled = KeyboardMoveSplitter(0.0, -10.0);
                    return;

                case VirtualKey.Right:
                    e.Handled = KeyboardMoveSplitter(10.0, 0.0);
                    return;

                case VirtualKey.Down:
                    e.Handled = KeyboardMoveSplitter(0.0, 10.0);
                    return;

                case VirtualKey.Escape:
                    if (_resizeData != null)
                    {
                        CancelResize();
                        e.Handled = true;
                    }
                    return;
            }
        }

        #endregion

        #region 内部方法
        /// <summary>
        /// 通过事件参数设置开始移动的相关信息
        /// </summary>
        void InitializeData()
        {
            ResizeEventArgs args = new ResizeEventArgs(PreviewResizeStartEvent, this);
            this.RaiseEvent(args);
            Panel parent = Parent as Panel;
            if (parent != null)
            {
                _resizeData = new ResizeData();
                _resizeData.ResizedTgt = args.ResizedTgt ?? parent;
                _resizeData.Placement = Placement ?? ItemPlacement.Left;
                _resizeData.MaxSize = args.AvailableSize;
                _resizeData.MinSize = args.MinSize;
                if (args.AffectedTgt != null)
                {
                    _resizeData.AffectedTgt = args.AffectedTgt;
                    _resizeData.ResizeBehavior = ResizeBehavior.Split;
                }
                _resizeData.Init();
            }
        }

        /// <summary>
        /// 移动分隔栏
        /// </summary>
        /// <param name="p_horChange">水平移动距离</param>
        /// <param name="p_verChange">垂直移动距离</param>
        /// <param name="p_isPreview">移动预览栏还是真实调整</param>
        void MoveSplitter(double p_horChange, double p_verChange, bool p_isPreview)
        {
            if (!Placement.HasValue)
                return;

            double change = 0.0;
            switch (Placement)
            {
                case ItemPlacement.Left:
                case ItemPlacement.Right:
                    change = p_horChange;
                    break;
                case ItemPlacement.Top:
                case ItemPlacement.Bottom:
                    change = p_verChange;
                    break;
            }
            if ((_resizeData != null) && (change != 0.0))
            {
                _resizeData.Resize(change, p_isPreview);
            }
        }

        void CancelResize()
        {
            RemovePreview();
            _resizeData.CancelResize();
            _resizeData = null;
        }

        bool KeyboardMoveSplitter(double p_horChange, double p_verChange)
        {
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                p_horChange *= -1.0;
            }
            if (HasKeyboardFocus && IsEnabled)
            {
                if (_resizeData != null)
                {
                    return false;
                }
                InitializeData();
                if (_resizeData == null)
                {
                    return false;
                }
                MoveSplitter(p_horChange, p_verChange, false);
                _resizeData = null;
                return true;
            }
            return false;
        }

        void CreatePreviewLayer()
        {
            _previewLayer = new Canvas();
            Panel parent = Parent as Panel;
            Grid parentGrid = parent as Grid;
            if (parentGrid != null)
            {
                if (parentGrid.RowDefinitions.Count > 0)
                {
                    _previewLayer.SetValue(Grid.RowSpanProperty, parentGrid.RowDefinitions.Count);
                }
                if (parentGrid.ColumnDefinitions.Count > 0)
                {
                    _previewLayer.SetValue(Grid.ColumnSpanProperty, parentGrid.ColumnDefinitions.Count);
                }
            }
            if (parent != null)
            {
                parent.Children.Add(_previewLayer);
            }
        }

        void SetupPreview()
        {
            _resizeData.Preview = Preview;
            _resizeData.Preview.Bind(this);
            _resizeData.Preview.Visibility = Visibility.Visible;
        }

        void RemovePreview()
        {
            if (_resizeData.Preview != null)
            {
                _resizeData.Preview.Visibility = Visibility.Collapsed;
            }
        }

        void ChangeVisualState()
        {
            if (Placement.HasValue)
            {
                Visibility = Visibility.Visible;
                if (Placement == ItemPlacement.Left)
                {
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Stretch;
                    Width = ResizerSize;
                    Height = double.NaN;
                    this.SetCursor(CoreCursorType.SizeWestEast);
                }
                else if (Placement == ItemPlacement.Right)
                {
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Stretch;
                    Width = ResizerSize;
                    Height = double.NaN;
                    this.SetCursor(CoreCursorType.SizeWestEast);
                }
                else if (Placement == ItemPlacement.Top)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                    VerticalAlignment = VerticalAlignment.Top;
                    Width = double.NaN;
                    Height = ResizerSize;
                    this.SetCursor(CoreCursorType.SizeNorthSouth);
                }
                else if (Placement == ItemPlacement.Bottom)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    Width = double.NaN;
                    Height = ResizerSize;
                    this.SetCursor(CoreCursorType.SizeNorthSouth);
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        void OnLayoutChangeEnded()
        {
            this.RaiseEvent(new BaseRoutedEventArgs(LayoutChangeEndedEvent, this));
        }
        #endregion

        #region 内部类型
        internal enum ResizeBehavior
        {
            /// <summary>
            /// 调整停靠区域大小
            /// </summary>
            Resize,

            /// <summary>
            /// 调整停靠区域内部各控件大小
            /// </summary>
            Split
        }

        internal class ResizeData
        {
            int _zIndex;
            Size _affectedTgtOriginalSize;
            double _affectedTgtOriginalChange;
            Size _resizedTgtOriginalSize;
            double _resizedTgtOriginalChange;

            /// <summary>
            /// 保存目标控件的原始位置
            /// </summary>
            internal void Init()
            {
                _zIndex = Canvas.GetZIndex(ResizedTgt);
                Canvas.SetZIndex(ResizedTgt, 0xf423f);
                if (ResizeBehavior == GridResizer.ResizeBehavior.Resize)
                {
                    _resizedTgtOriginalSize = new Size(ResizedTgt.ActualWidth, ResizedTgt.ActualHeight);
                }
                else
                {
                    _resizedTgtOriginalChange = TabItemPanel.GetSplitterChange(ResizedTgt);
                    _affectedTgtOriginalChange = TabItemPanel.GetSplitterChange(AffectedTgt);
                    _resizedTgtOriginalSize = ResizedTgt.RenderSize;
                    _affectedTgtOriginalSize = AffectedTgt.RenderSize;
                }
            }

            /// <summary>
            /// 调整大小
            /// </summary>
            /// <param name="p_change">变化量</param>
            /// <param name="p_isPreview">移动预览栏还是真实调整</param>
            public void Resize(double p_change, bool p_isPreview)
            {
                double[] deltaConstraints = GetDeltaConstraints();
                double minSize = deltaConstraints[0];
                double maxSize = deltaConstraints[1];
                p_change = Math.Min(Math.Max(p_change, minSize), maxSize);
                if (p_isPreview)
                {
                    if (IsPlacementHorizontal)
                    {
                        Preview.OffsetX = p_change;
                    }
                    else
                    {
                        Preview.OffsetY = p_change;
                    }
                }
                else
                {
                    if (Placement == ItemPlacement.Left || Placement == ItemPlacement.Top)
                        p_change = -p_change;
                    
                    if (ResizeBehavior == GridResizer.ResizeBehavior.Split)
                    {
                        SplitItems(p_change);
                    }
                    else if (IsPlacementHorizontal)
                    {
                        ResizedTgt.Width = ResizedTgtOriginalLength + p_change;
                    }
                    else
                    {
                        ResizedTgt.Height = ResizedTgtOriginalLength + p_change;
                    }
                    InvalidatePanelMeasure();
                }
            }

            /// <summary>
            /// 取消调整
            /// </summary>
            internal void CancelResize()
            {
                if (ResizeBehavior == GridResizer.ResizeBehavior.Resize)
                {
                    if (IsPlacementHorizontal)
                    {
                        ResizedTgt.Width = ResizedTgtOriginalLength;
                    }
                    else
                    {
                        ResizedTgt.Height = ResizedTgtOriginalLength;
                    }
                }
                else
                {
                    TabItemPanel.SetSplitterChange(ResizedTgt, _resizedTgtOriginalChange);
                    TabItemPanel.SetSplitterChange(AffectedTgt, _affectedTgtOriginalChange);
                }
                InvalidatePanelMeasure();
                ClearReferences();
            }

            internal void ClearReferences()
            {
                Canvas.SetZIndex(ResizedTgt, _zIndex);
                Preview = null;
                ResizedTgt = null;
                AffectedTgt = null;
            }

            double[] GetDeltaConstraints()
            {
                double minSize = 0.0;
                double maxSize = 0.0;
                if (ResizeBehavior == GridResizer.ResizeBehavior.Split)
                {
                    switch (Placement)
                    {
                        case ItemPlacement.Left:
                            minSize = AffectedTgt.MinWidth - AffectedTgtOriginalLength;
                            maxSize = ResizedTgtOriginalLength - ResizedTgt.MinWidth;
                            break;

                        case ItemPlacement.Top:
                            minSize = AffectedTgt.MinHeight - AffectedTgtOriginalLength;
                            maxSize = ResizedTgtOriginalLength - ResizedTgt.MinHeight;
                            break;

                        case ItemPlacement.Right:
                            minSize = ResizedTgtOriginalLength - ResizedTgt.MinWidth;
                            maxSize = AffectedTgtOriginalLength - AffectedTgt.MinWidth;
                            break;

                        case ItemPlacement.Bottom:
                            minSize = ResizedTgtOriginalLength - ResizedTgt.MinHeight;
                            maxSize = AffectedTgtOriginalLength - AffectedTgt.MinHeight;
                            break;
                    }
                }
                else
                {
                    Size maximum = MaxSize.IsEmpty ? new Size(AtUI.ViewWidth, AtUI.ViewHeight) : MaxSize;
                    switch (Placement)
                    {
                        case ItemPlacement.Left:
                            minSize = MinSize.Width - MaxSize.Width;
                            maxSize = ResizedTgtOriginalLength - ResizedTgt.MinWidth;
                            break;

                        case ItemPlacement.Top:
                            minSize = MinSize.Height - MaxSize.Height;
                            maxSize = ResizedTgtOriginalLength - ResizedTgt.MinHeight;
                            break;

                        case ItemPlacement.Right:
                            minSize = ResizedTgt.MinWidth - ResizedTgtOriginalLength;
                            maxSize = Math.Min((double)(maximum.Width - MinSize.Width), (double)(ResizedTgt.MaxWidth - ResizedTgtOriginalLength));
                            break;

                        case ItemPlacement.Bottom:
                            minSize = ResizedTgt.MinHeight - ResizedTgtOriginalLength;
                            maxSize = Math.Min((double)(maximum.Height - MinSize.Height), (double)(ResizedTgt.MaxHeight - ResizedTgtOriginalLength));
                            break;
                    }
                }
                return new double[] { minSize, maxSize };
            }

            void InvalidatePanelMeasure()
            {
                if (Panel != null)
                {
                    Panel.InvalidateMeasure();
                }
            }

            void SplitItems(double change)
            {
                TabItemPanel.SetSplitterChange(ResizedTgt, ResizedTgtOriginalLength + change);
                TabItemPanel.SetSplitterChange(AffectedTgt, AffectedTgtOriginalLength - change);
            }

            public FrameworkElement ResizedTgt { get; set; }

            public FrameworkElement AffectedTgt { get; set; }

            public Size MaxSize { get; set; }

            public Size MinSize { get; set; }

            public ItemPlacement Placement { get; set; }

            public PreviewControl Preview { get; set; }

            internal ResizeBehavior ResizeBehavior { get; set; }

            Panel Panel
            {
                get
                {
                    if (ResizedTgt == null)
                    {
                        return null;
                    }
                    return (VisualTreeHelper.GetParent(ResizedTgt) as Panel);
                }
            }

            double ResizedTgtOriginalLength
            {
                get
                {
                    if (IsPlacementHorizontal)
                    {
                        return _resizedTgtOriginalSize.Width;
                    }
                    return _resizedTgtOriginalSize.Height;
                }
            }

            double AffectedTgtOriginalLength
            {
                get
                {
                    if (IsPlacementHorizontal)
                    {
                        return _affectedTgtOriginalSize.Width;
                    }
                    return _affectedTgtOriginalSize.Height;
                }
            }

            bool IsPlacementHorizontal
            {
                get
                {
                    if (Placement != ItemPlacement.Left)
                    {
                        return (Placement == ItemPlacement.Right);
                    }
                    return true;
                }
            }

        }
        #endregion
    }
}

