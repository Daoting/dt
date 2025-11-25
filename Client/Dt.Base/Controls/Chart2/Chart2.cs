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
using ScottPlot.Interactivity;
using SkiaSharp.Views.Windows;
using Windows.Storage;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 开源跨平台Chart https://github.com/ScottPlot/ScottPlot
    /// 高性能、交互性强
    /// </summary>
    public partial class Chart2 : UserControl, IPlotControl, IDestroy
    {
        #region 变量
        readonly SKXamlCanvas _canvas;
        Plot _plot;
        UserInputProcessor _inputProcessor;
        #endregion

        #region 静态构造
        static Chart2()
        {
            // 设置支持中文的默认字体，ScottPlot中默认字体乱码
#if WIN
            // 采用windows默认中文字体：微软雅黑
            ScottPlot.Fonts.Default = "Microsoft YaHei UI";
#else
            _ = AddFontFile();
#endif
        }
        
        static async Task AddFontFile()
        {
            // 默认字体和uno中相同，手动指定粗体、斜体等样式的ttf文件
            string fontName = "HarmonySans";
            try
            {
                // 静态构造方法中若使用GetResults()同步方法获取文件时，android初次会异常！
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Fonts/HarmonySans.ttf"));
                ScottPlot.Fonts.AddFontFile(fontName, file.Path, false, false);
                ScottPlot.Fonts.AddFontFile(fontName, file.Path, false, true);

                // 仍需要获取文件路径，通过上述路径计算的路径在android中报文件不存在，可能android中StorageFile内部有获取访问权限的异步操作
                file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Fonts/HarmonySans_Bold.ttf"));
                ScottPlot.Fonts.AddFontFile(fontName, file.Path, true, true);
                ScottPlot.Fonts.AddFontFile(fontName, file.Path, true, false);
            }
            catch { }
            ScottPlot.Fonts.Default = fontName;
        }
        #endregion

        #region 构造方法
        public Chart2()
        {
            _plot = new Plot { PlotControl = this };
            Multiplot = new Multiplot(_plot);
            _inputProcessor = new UserInputProcessor(this);
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
        #endregion

        #region IPlotControl
#nullable enable
        public IPlotMenu? Menu { get; set; }

        public float DisplayScale { get; set; } = 1;

        Plot IPlotControl.Plot => _plot;
        public IMultiplot Multiplot { get; set; }
        SkiaSharp.GRContext? IPlotControl.GRContext => null;
        
        UserInputProcessor IPlotControl.UserInputProcessor => _inputProcessor;

        public void Reset()
        {
            Reset(new Plot());
        }

        public void Reset(Plot p_plot)
        {
            _plot = p_plot;
            _plot.PlotControl = this;
            Multiplot.Reset(_plot);
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
        public void SetCursor(Cursor cursor)
        {
            // TODO: How do we set the cursor in WinUI?
            //InputSystemCursor.Create(cursor.GetCursor());
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
            Multiplot.Render(e.Surface);
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Focus(FocusState.Pointer);
            _inputProcessor.ProcessMouseDown(this, e);
            (sender as UIElement)?.CapturePointer(e.Pointer);
            base.OnPointerPressed(e);
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _inputProcessor.ProcessMouseUp(this, e);
            (sender as UIElement)?.ReleasePointerCapture(e.Pointer);
            base.OnPointerReleased(e);
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _inputProcessor.ProcessMouseMove(this, e);
            base.OnPointerMoved(e);
        }

        void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            base.OnDoubleTapped(e);
        }

        void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            _inputProcessor.ProcessMouseWheel(this, e);
            base.OnPointerWheelChanged(e);
        }

        void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            _inputProcessor.ProcessKeyDown(this, e);
            base.OnKeyDown(e);
        }

        void OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"KEY UP {e.Key}");
            _inputProcessor.ProcessKeyUp(this, e);
            base.OnKeyUp(e);
        }
        #endregion

        #region IDestroy
        public void Destroy()
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