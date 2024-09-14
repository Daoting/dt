#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-09-13 创建
******************************************************************************/
#endregion

#region 引用命名
using ScottPlot;
using ScottPlot.Grids;
using ScottPlot.Panels;
using ScottPlot.Rendering;
using ScottPlot.Stylers;
using SkiaSharp;
#nullable enable
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Plot公共内容，方便使用
    /// </summary>
    public partial class Chart2
    {
        #region Plot属性
        /// <summary>
        /// 添加绘图元素
        /// </summary>
        public PlottableAdder Add => _plot.Add;

        /// <summary>
        /// 已绘元素列表
        /// </summary>
        public List<IPlottable> PlottableList => _plot.PlottableList;

        public RenderManager RenderManager => _plot.RenderManager;
        public RenderDetails LastRender => _plot.RenderManager.LastRender;
        public LayoutManager Layout => _plot.Layout;

        /// <summary>
        /// 整个图形的背景样式
        /// </summary>
        public BackgroundStyle FigureBackground => _plot.FigureBackground;

        /// <summary>
        /// 数据区的背景样式 (包括坐标轴)
        /// </summary>
        public BackgroundStyle DataBackground => _plot.DataBackground;

        public IZoomRectangle ZoomRectangle { get; set; }
        public double ScaleFactor { get => _plot.ScaleFactor; set => _plot.ScaleFactor = (float)value; }

        public AxisManager Axes => _plot.Axes;


        public FontStyler Font => _plot.Font;
        public Legend Legend => _plot.Legend;

        public DefaultGrid Grid => _plot.Axes.DefaultGrid;

        public IPlottable Benchmark { get => _plot.Benchmark; set => _plot.Benchmark = value; }

        /// <summary>
        /// This object is locked by the Render() methods.
        /// Logic that manipulates the plot (UI inputs or editing data)
        /// can lock this object to prevent rendering artifacts.
        /// </summary>
        public object Sync => _plot.Sync;
        #endregion
        
        #region 标题
        /// <summary>
        /// Shortcut to set text of the <see cref="TitlePanel"/> Label.
        /// Assign properties of <see cref="TitlePanel"/> Label to customize size, color, font, etc.
        /// </summary>
        public void Title(string text, float? size = null)
        {
            Axes.Title.Label.Text = text;
            Axes.Title.IsVisible = !string.IsNullOrWhiteSpace(text);
            if (size.HasValue)
                Axes.Title.Label.FontSize = size.Value;
        }

        /// <summary>
        /// Shortcut to set text of the <see cref="BottomAxis"/> Label
        /// Assign properties of <see cref="BottomAxis"/> Label to customize size, color, font, etc.
        /// </summary>
        public void XLabel(string label, float? size = null)
        {
            Axes.Bottom.Label.Text = label;
            if (size.HasValue)
                Axes.Bottom.Label.FontSize = size.Value;
        }

        /// <summary>
        /// Shortcut to set text of the <see cref="BottomAxis"/> Label
        /// Assign properties of <see cref="BottomAxis"/> Label to customize size, color, font, etc.
        /// </summary>
        public void YLabel(string label, float? size = null)
        {
            Axes.Left.Label.Text = label;
            if (size.HasValue)
                Axes.Left.Label.FontSize = size.Value;
        }
        #endregion
        
        #region 像素坐标转换
        /// <summary>
        /// Return the location on the screen (pixel) for a location on the plot (coordinates) on the default axes.
        /// The figure size and layout referenced will be the one from the last render.
        /// </summary>
        public Pixel GetPixel(Coordinates coordinates) => _plot.GetPixel(coordinates, Axes.Bottom, Axes.Left);

        /// <summary>
        /// Return the location on the screen (pixel) for a location on the plot (coordinates) on the given axes.
        /// The figure size and layout referenced will be the one from the last render.
        /// </summary>
        public Pixel GetPixel(Coordinates coordinates, IXAxis xAxis, IYAxis yAxis) => _plot.GetPixel(coordinates, xAxis, yAxis);
        
        /// <summary>
        /// Return the coordinate for a specific pixel using measurements from the most recent render.
        /// </summary>
        public Coordinates GetCoordinates(Pixel pixel, IXAxis? xAxis = null, IYAxis? yAxis = null)
            => _plot.GetCoordinates(pixel, xAxis, yAxis);
        
        /// <summary>
        /// Return the coordinate for a specific pixel using measurements from the most recent render.
        /// </summary>
        public Coordinates GetCoordinates(float x, float y, IXAxis? xAxis = null, IYAxis? yAxis = null)
            => _plot.GetCoordinates(x, y, xAxis, yAxis);

        /// <summary>
        /// Return a coordinate rectangle centered at a pixel.  Uses measurements
        /// from the most recent render.
        /// <param name="x">Center point pixel's x</param>
        /// <param name="y">Center point pixel's y</param>
        /// <param name="radius">Radius in pixels</param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <returns>The coordinate rectangle</returns>
        /// </summary>
        public CoordinateRect GetCoordinateRect(float x, float y, float radius = 10, IXAxis? xAxis = null, IYAxis? yAxis = null)
            => _plot.GetCoordinateRect(x, y, radius, xAxis, yAxis);

        /// <summary>
        /// Return a coordinate rectangle centered at a pixel.  Uses measurements
        /// from the most recent render.
        /// <param name="pixel">Center point pixel</param>
        /// <param name="radius">Radius in pixels</param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <returns>The coordinate rectangle</returns>
        /// </summary>
        public CoordinateRect GetCoordinateRect(Pixel pixel, float radius = 10, IXAxis? xAxis = null, IYAxis? yAxis = null)
            => _plot.GetCoordinateRect(pixel, radius, xAxis, yAxis);

        /// <summary>
        /// Return a coordinate rectangle centered at a coordinate pair with the
        /// radius specified in pixels.  Uses measurements from the most recent
        /// render.
        /// <param name="coordinates">Center point in coordinate units</param>
        /// <param name="radius">Radius in pixels</param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <returns>The coordinate rectangle</returns>
        /// </summary>
        public CoordinateRect GetCoordinateRect(Coordinates coordinates, float radius = 10, IXAxis? xAxis = null, IYAxis? yAxis = null)
            => _plot.GetCoordinateRect(coordinates, radius, xAxis, yAxis);  

        /// <summary>
        /// Get the axis under a given pixel
        /// </summary>
        /// <param name="pixel">Point</param>
        /// <returns>The axis at <paramref name="pixel" /> (or null)</returns>
        public IAxis? GetAxis(Pixel pixel) => _plot.GetAxis(pixel);

        /// <summary>
        /// Get the panel under a given pixel
        /// </summary>
        /// <param name="pixel">Point</param>
        /// <param name="axesOnly"></param>
        /// <returns>The panel at <paramref name="pixel" /> (or null)</returns>
        public IPanel? GetPanel(Pixel pixel, bool axesOnly) => _plot.GetPanel(pixel, axesOnly);
        #endregion

        #region 绘制
        /// <summary>
        /// Create a new image of the given dimensions, render the plot onto it, and return it.
        /// </summary>
        public void RenderInMemory(int width = 400, int height = 300) => _plot.RenderInMemory(width, height);

        /// <summary>
        /// Render onto an existing canvas
        /// </summary>
        public void Render(SKCanvas canvas, int width, int height) => _plot.Render(canvas, width, height);

        /// <summary>
        /// Render onto an existing canvas inside the given rectangle
        /// </summary>
        public void Render(SKCanvas canvas, PixelRect rect) => _plot.Render(canvas, rect);

        /// <summary>
        /// Render onto an existing canvas of a surface over the local clip bounds
        /// </summary>
        public void Render(SKSurface surface) => _plot.Render(surface);
        #endregion

        #region 创建图片
        public Image GetImage(int width, int height) => _plot.GetImage(width, height);

        /// <summary>
        /// Render the plot and return an HTML img element containing a Base64-encoded PNG
        /// </summary>
        public string GetImageHtml(int width, int height) => _plot.GetImageHtml(width, height);

        public SavedImageInfo SaveJpeg(string filePath, int width, int height, int quality = 85)
            => _plot.SaveJpeg(filePath, width, height, quality);

        public SavedImageInfo SavePng(string filePath, int width, int height)
            => _plot.SavePng(filePath, width, height);

        public SavedImageInfo SaveBmp(string filePath, int width, int height)
            => _plot.SaveBmp(filePath, width, height);

        public SavedImageInfo SaveWebp(string filePath, int width, int height, int quality = 85)
            => _plot.SaveWebp(filePath, width, height, quality);

        public SavedImageInfo SaveSvg(string filePath, int width, int height)
            => _plot.SaveSvg(filePath, width, height);

        public string GetSvgXml(int width, int height)
            => _plot.GetSvgXml(width, height);

        public SavedImageInfo Save(string filePath, int width, int height)
            => _plot.Save(filePath, width, height);

        public SavedImageInfo Save(string filePath, int width, int height, ImageFormat format, int quality = 85)
            => _plot.Save(filePath, width, height, format, quality);

        public byte[] GetImageBytes(int width, int height, ImageFormat format = ImageFormat.Bmp)
            => _plot.GetImageBytes(width, height, format);

        /// <summary>
        /// Returns the content of the legend as a raster image
        /// </summary>
        public Image GetLegendImage() => _plot.GetLegendImage();

        /// <summary>
        /// Returns the content of the legend as SVG (vector) image
        /// </summary>
        public string GetLegendSvgXml() => _plot.GetLegendSvgXml();
        #endregion

        #region 工具方法
        /// <summary>
        /// Return contents of <see cref="PlottableList"/>.
        /// </summary>
        public IEnumerable<IPlottable> GetPlottables()
        {
            return _plot.PlottableList;
        }

        /// <summary>
        /// Return all plottables in <see cref="PlottableList"/> of the given type.
        /// </summary>
        public IEnumerable<T> GetPlottables<T>() where T : IPlottable
        {
            return _plot.PlottableList.OfType<T>();
        }

        /// <summary>
        /// Remove the given plottable from the <see cref="PlottableList"/>.
        /// </summary>
        public void Remove(IPlottable plottable) => _plot.Remove(plottable);

        /// <summary>
        /// Remove the given Panel from the <see cref="Axes"/>.
        /// </summary>
        public void Remove(IPanel panel) => _plot.Remove(panel);

        /// <summary>
        /// Remove the given Axis from the <see cref="Axes"/>.
        /// </summary>
        public void Remove(IAxis axis) => _plot.Remove(axis);

        /// <summary>
        /// Remove all items of a specific type from the <see cref="PlottableList"/>.
        /// </summary>
        public void Remove(Type plotType) => _plot.Remove(plotType);

        /// <summary>
        /// Remove a all instances of a specific type from the <see cref="PlottableList"/>.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IPlottable"/> to be removed</typeparam>
        public void Remove<T>() where T : IPlottable => _plot.Remove<T>();

        /// <summary>
        /// Remove all instances of a specific type from the <see cref="PlottableList"/> 
        /// that meet the <paramref name="predicate"/> criteraia.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="T">Type of <see cref="IPlottable"/> to be removed</typeparam>
        public void Remove<T>(Func<T, bool> predicate) where T : IPlottable => _plot.Remove<T>(predicate);

        /// <summary>
        /// Move the indicated plottable to the end of the list so it is rendered last
        /// </summary>
        public void MoveToFront(IPlottable plottable) => _plot.MoveToFront(plottable);

        /// <summary>
        /// Move the indicated plottable to the start of the list so it is rendered first
        /// </summary>
        public void MoveToBack(IPlottable plottable) => _plot.MoveToBack(plottable);

        /// <summary>
        /// Disable visibility for all axes and grids
        /// </summary>
        public void HideAxesAndGrid(bool showTitle = true) => _plot.HideAxesAndGrid(showTitle);

        /// <summary>
        /// Disable visibility for all grids
        /// </summary>
        public void HideGrid() => _plot.HideGrid();

        /// <summary>
        /// Enable visibility for all grids
        /// </summary>
        public void ShowGrid() => _plot.ShowGrid();

        /// <summary>
        /// Helper method for setting visibility of the <see cref="Legend"/>
        /// </summary>
        public Legend ShowLegend() => _plot.ShowLegend();

        /// <summary>
        /// Helper method for setting visibility of the <see cref="Legend"/>
        /// and setting <see cref="Legend.Location"/> to the provided one.
        /// </summary>
        public Legend ShowLegend(Alignment alignment) => _plot.ShowLegend(alignment);

        /// <summary>
        /// Helper method for setting the Legend's IsVisible, Alignment, and Orientation
        /// properties all at once.
        /// </summary>
        public Legend ShowLegend(Alignment alignment, Orientation orientation) => _plot.ShowLegend(alignment, orientation);

        /// <summary>
        /// Helper method for displaying specific items in the legend
        /// </summary>
        public Legend ShowLegend(IEnumerable<LegendItem> items, Alignment location = Alignment.LowerRight)
            => _plot.ShowLegend(items, location);

        /// <summary>
        /// Hide the default legend (inside the data area) and create a new legend panel 
        /// placed on the edge of the figure outside the data area.
        /// </summary>
        /// <returns></returns>
        public LegendPanel ShowLegend(Edge edge) => _plot.ShowLegend(edge);

        /// <summary>
        /// Helper method for setting visibility of the <see cref="Legend"/>
        /// </summary>
        public Legend HideLegend() => _plot.HideLegend();

        /// <summary>
        /// Clears the <see cref="PlottableList"/> list
        /// </summary>
        public void Clear() => _plot.PlottableList.Clear();

        /// <summary>
        /// Clear a all instances of a specific type from the <see cref="PlottableList"/>.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IPlottable"/> to be cleared</typeparam>
        public void Clear<T>() where T : IPlottable => _plot.Clear<T>();

        /// <summary>
        /// Return the current style settings for this plot
        /// </summary>
        public PlotStyle GetStyle() => _plot.GetStyle();

        /// <summary>
        /// Apply the given style settings to this plot
        /// </summary>
        public void SetStyle(PlotStyle style) => _plot.SetStyle(style);

        /// <summary>
        /// Apply the style settings from the given plot to this plot
        /// </summary>
        public void SetStyle(Plot otherPlot) => _plot.SetStyle(otherPlot);
        #endregion
    }
}