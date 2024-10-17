#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-09-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using ScottPlot;
using ScottPlot.Control;
using ScottPlot.Interactivity;
using SkiaSharp.Views.Windows;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 开源跨平台Chart https://github.com/ScottPlot/ScottPlot
    /// 高性能、交互性强
    /// </summary>
    public partial class Chart2 : UserControl, IPlotControl, IDisposable
    {
        #region 变量
        readonly SKXamlCanvas _canvas;
        PlotX _plot;
        UserInputProcessor _inputProcessor;
        IPlotInteraction _interaction;
        #endregion

        public Chart2()
        {
            _plot = new PlotX { PlotControl = this };
            _inputProcessor = new UserInputProcessor(_plot);

            _interaction = new Interaction(this);
            Menu = new ChartMenu(this);

            Background = new SolidColorBrush(Microsoft.UI.Colors.White);

            _canvas = CreateRenderTarget();
            _canvas.PaintSurface += OnPaintSurface;

            _canvas.PointerWheelChanged += OnPointerWheelChanged;
            _canvas.PointerReleased += OnPointerReleased;
            _canvas.PointerPressed += OnPointerPressed;
            _canvas.PointerMoved += OnPointerMoved;
            _canvas.DoubleTapped += OnDoubleTapped;
            _canvas.KeyDown += OnKeyDown;
            _canvas.KeyUp += OnKeyUp;
            Loaded += OnLoaded;

            Content = _canvas;
        }

        #region IPlotControl
#nullable enable
        public IPlotMenu? Menu { get; set; }

        public float DisplayScale { get; set; } = 1;
        
        Plot IPlotControl.Plot => _plot;
        SkiaSharp.GRContext? IPlotControl.GRContext => null;
        IPlotInteraction IPlotControl.Interaction
        {
            get => _interaction;
            set => _interaction = value;
        }
        UserInputProcessor IPlotControl.UserInputProcessor => _inputProcessor;

        public void Reset()
        {
            ResetInternal(new PlotX());
        }

        void IPlotControl.Reset(Plot plot)
        {
            if (plot is PlotX p)
            {
                ResetInternal(p);
            }
        }

        void ResetInternal(PlotX p_plot)
        {
            _plot.Dispose();
            p_plot.PlotControl = this;
            _plot = p_plot;
            _inputProcessor = new(_plot);
        }

        public void Refresh()
        {
            _canvas.Invalidate();
        }

        public void ShowContextMenu(Pixel position)
        {
            Menu?.ShowContextMenu(position);
        }

        public float DetectDisplayScale()
        {
            if (XamlRoot is not null)
            {
                _plot.ScaleFactor = XamlRoot.RasterizationScale;
                DisplayScale = (float)XamlRoot.RasterizationScale;
            }

            return DisplayScale;
        }
        #endregion

        #region 初始化
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (XamlRoot is null)
                return;
            
            Loaded -= OnLoaded;
            XamlRoot.Changed += OnRootChanged;
            _plot.ScaleFactor = XamlRoot.RasterizationScale;
            DisplayScale = (float)XamlRoot.RasterizationScale;
        }

        void OnRootChanged(XamlRoot sender, XamlRootChangedEventArgs args)
        {
            DetectDisplayScale();
        }

        static SKXamlCanvas CreateRenderTarget()
        {
            return new SKXamlCanvas
            {
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent)
            };
        }
        #endregion

        #region 交互
        void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            _plot.Render(e.Surface.Canvas, (int)e.Surface.Canvas.LocalClipBounds.Width, (int)e.Surface.Canvas.LocalClipBounds.Height);
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Focus(FocusState.Pointer);

            ((IPlotControl)this).Interaction.MouseDown(e.Pixel(this), e.OldToButton(this));
            _inputProcessor.ProcessMouseDown(this, e);

            (sender as UIElement)?.CapturePointer(e.Pointer);

            base.OnPointerPressed(e);
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _interaction.MouseUp(e.Pixel(this), e.OldToButton(this));
            _inputProcessor.ProcessMouseUp(this, e);

            (sender as UIElement)?.ReleasePointerCapture(e.Pointer);

            base.OnPointerReleased(e);
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _interaction.OnMouseMove(e.Pixel(this));
            _inputProcessor.ProcessMouseMove(this, e);
            base.OnPointerMoved(e);
        }

        void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _interaction.DoubleClick();
            base.OnDoubleTapped(e);
        }

        void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            _interaction.MouseWheelVertical(e.Pixel(this), e.GetCurrentPoint(this).Properties.MouseWheelDelta);
            _inputProcessor.ProcessMouseWheel(this, e);
            base.OnPointerWheelChanged(e);
        }

        void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            _interaction.KeyDown(e.OldToKey());
            _inputProcessor.ProcessKeyDown(this, e);
            base.OnKeyDown(e);
        }

        void OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"KEY UP {e.Key}");
            _interaction.KeyUp(e.OldToKey());
            _inputProcessor.ProcessKeyUp(this, e);
            base.OnKeyUp(e);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (XamlRoot != null)
                XamlRoot.Changed -= OnRootChanged;

            _canvas.PaintSurface -= OnPaintSurface;
            _canvas.PointerWheelChanged -= OnPointerWheelChanged;
            _canvas.PointerReleased -= OnPointerReleased;
            _canvas.PointerPressed -= OnPointerPressed;
            _canvas.PointerMoved -= OnPointerMoved;
            _canvas.DoubleTapped -= OnDoubleTapped;
            _canvas.KeyDown -= OnKeyDown;
            _canvas.KeyUp -= OnKeyUp;
            Content = null;
        }
        #endregion
    }
}