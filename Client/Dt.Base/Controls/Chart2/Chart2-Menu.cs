#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-09-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Controls;
using ScottPlot;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 图表上下文菜单
    /// </summary>
    public class ChartMenu : IPlotMenu
    {
        readonly Chart2 _chart;
        readonly List<ContextMenuItem> _menuItems;

        public ChartMenu(Chart2 p_chart)
        {
            _chart = p_chart;
            _menuItems = new List<ContextMenuItem>();
            Reset();
        }

        public MenuFlyout GetContextMenu(Plot plot)
        {
            MenuFlyout menu = new();
            foreach (var curr in _menuItems)
            {
                if (curr.IsSeparator)
                {
                    menu.Items.Add(new MenuFlyoutSeparator());
                }
                else
                {
                    var menuItem = new MenuFlyoutItem { Text = curr.Label };
                    menuItem.Click += (s, e) => curr.OnInvoke(plot);
                    menu.Items.Add(menuItem);
                }
            }
            return menu;
        }

        public void ShowContextMenu(Pixel pixel)
        {
            var plot = _chart.GetPlotAtPixel(pixel);
            if (plot is null)
                return;

            MenuFlyout flyout = GetContextMenu(plot);
            Windows.Foundation.Point pt = new(pixel.X, pixel.Y);
            flyout.ShowAt(_chart, pt);
        }

        public void Reset()
        {
            Clear();
            AddDefaultItems();
        }

        public void Clear()
        {
            _menuItems.Clear();
        }

        public void Add(string Label, Action<Plot> action)
        {
            _menuItems.Add(new ContextMenuItem() { Label = Label, OnInvoke = action });
        }

        public void AddSeparator()
        {
            _menuItems.Add(new ContextMenuItem() { IsSeparator = true });
        }

        void AddDefaultItems()
        {
            _menuItems.Add(new() { Label = "保存图像", OnInvoke = OpenSaveImageDialog });
            _menuItems.Add(new() { Label = "复制图像", OnInvoke = CopyImageToClipboard });
            _menuItems.Add(new() { Label = "自动缩放", OnInvoke = Autoscale });
        }

        async void OpenSaveImageDialog(Plot plot)
        {
            FileSavePicker dialog = new()
            {
                SuggestedFileName = "图表.png"
            };
            dialog.FileTypeChoices.Add("PNG 文件", new List<string>() { ".png" });
            dialog.FileTypeChoices.Add("JPEG 文件", new List<string>() { ".jpg", ".jpeg" });
            dialog.FileTypeChoices.Add("BMP 文件", new List<string>() { ".bmp" });
            dialog.FileTypeChoices.Add("WebP 文件", new List<string>() { ".webp" });
            dialog.FileTypeChoices.Add("SVG 文件", new List<string>() { ".svg" });

#if WIN
            // 绑定Window句柄，操
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(Kit.MainWin);
            WinRT.Interop.InitializeWithWindow.Initialize(dialog, hWnd);
#endif

            var file = await dialog.PickSaveFileAsync();

            if (file != null)
            {
                ImageFormat format = ImageFormats.FromFilename(file.Name);
                PixelSize lastRenderSize = plot.RenderManager.LastRender.FigureRect.Size;
                plot.Save(file.Path, (int)lastRenderSize.Width, (int)lastRenderSize.Height, format);
                Kit.Msg("保存成功！");
            }
        }

        void CopyImageToClipboard(Plot plot)
        {
            PixelSize lastRenderSize = plot.RenderManager.LastRender.FigureRect.Size;
            byte[] bytes = plot.GetImage((int)lastRenderSize.Width, (int)lastRenderSize.Height).GetImageBytes();

            var stream = new InMemoryRandomAccessStream();
            stream.AsStreamForWrite().Write(bytes);

            var content = new DataPackage();
            content.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));

            Clipboard.SetContent(content);
            Kit.Msg("图像已复制到剪贴板！");
        }

        void Autoscale(Plot plot)
        {
            plot.Axes.AutoScale();
            _chart.Refresh();
        }
    }
}