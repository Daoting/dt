#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2026-02-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Maps;
using Mapsui.Extensions;
using Mapsui.Logging;
using Mapsui.Manipulations;
using Mapsui.UI;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.System;
#endregion

namespace Dt.Base;

/// <summary>
/// https://github.com/Mapsui/Mapsui/blob/main/Mapsui.UI.WinUI/MapControl.cs
/// </summary>
public partial class MapView : Grid, IMapControl, IDisposable
{
    readonly RenderControl _renderControl;
    readonly Rectangle _selectRectangle = CreateSelectRectangle();

    bool _shiftPressed;

    public MapView()
    {
        // The commented out code crashes the app when MouseWheelAnimation.Duration > 0. Could be a bug in SKXamlCanvas
        //if (Dispatcher.HasThreadAccess) _canvas?.Invalidate();
        //else RunOnUIThread(() => _canvas?.Invalidate());

        Background = new SolidColorBrush(Colors.White); // DON'T REMOVE! Touch events do not work without a background

        _renderControl = RenderControl.CreateControl(this, canvas => _renderController?.Render(canvas));
        Children.Add(_renderControl);

        // The Canvas needs to be first set before calling the Shared Constructor or else it crashes in the InvalidateCanvas
        SharedConstructor();

        Children.Add(_selectRectangle);

        Loaded += MapControlLoaded;

        SizeChanged += MapControlSizeChanged;

        ManipulationMode = ManipulationModes.Scale | ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.Rotate;

        ManipulationInertiaStarting += OnManipulationInertiaStarting;
        ManipulationDelta += OnManipulationDelta;
        ManipulationCompleted += OnManipulationCompleted;

        PointerPressed += MapControl_PointerPressed;
        PointerMoved += MapControl_PointerMoved;
        PointerReleased += MapControl_PointerReleased;

        PointerWheelChanged += MapControl_PointerWheelChanged;

        KeyDown += MapControl_KeyDown;
        KeyUp += MapControl_KeyUp;

        var orientationSensor = SimpleOrientationSensor.GetDefault();
        if (orientationSensor != null)
            orientationSensor.OrientationChanged += (s, e) => RunOnUIThread(() => Refresh());
    }

    public void InvalidateCanvas()
    {
        RunOnUIThread(_renderControl.InvalidateRender);
    }

    void MapControl_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Shift)
        {
            _shiftPressed = true;
        }
    }

    void MapControl_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Shift)
        {
            _shiftPressed = false;
        }
    }

    void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        RefreshData();
    }

    void MapControl_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var screenPosition = e.GetCurrentPoint(this).Position.ToScreenPosition();

        if (OnPointerPressed([screenPosition]))
            return;
    }

    void MapControl_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        // This is a bit weird. The OnManipulationDelta event fires on both touch and mouse events
        // and deals with both properly, except for mouse hover events. This handler only deals with
        // hover events.
        if (!IsHovering(e))
            return;
        var position = e.GetCurrentPoint(this).Position.ToScreenPosition();

        if (OnPointerMoved([position], true)) // Only for hover events
            return;
    }

    void MapControl_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        var screenPosition = e.GetCurrentPoint(this).Position.ToScreenPosition();
        OnPointerReleased([screenPosition]);
    }

    bool IsHovering(PointerRoutedEventArgs e)
    {
        if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            return false;
        return !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;
    }

    static Rectangle CreateSelectRectangle()
    {
        return new Rectangle
        {
            Fill = new SolidColorBrush(Colors.Red),
            Stroke = new SolidColorBrush(Colors.Black),
            StrokeThickness = 3,
            RadiusX = 0.5,
            RadiusY = 0.5,
            StrokeDashArray = [3.0],
            Opacity = 0.3,
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Visibility = Visibility.Collapsed
        };
    }

    void MapControl_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var mousePointerPoint = e.GetCurrentPoint(this);
        var mouseWheelDelta = mousePointerPoint.Properties.MouseWheelDelta;

        Map.Navigator.MouseWheelZoom(mouseWheelDelta, mousePointerPoint.ToScreenPosition());

        e.Handled = true;
    }

    void MapControlLoaded(object sender, RoutedEventArgs e)
    {
        SharedOnSizeChanged(ActualWidth, ActualHeight);
    }

    void MapControlSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Accessing ActualWidth and ActualHeight before SizeChange results in a com exception.
        Clip = new RectangleGeometry { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };
        SharedOnSizeChanged(ActualWidth, ActualHeight);
    }

    void RunOnUIThread(Action action)
    {
        Catch.TaskRun(() => DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message, ex);
            }
        }));
    }

    static void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
    {
        e.TranslationBehavior.DesiredDeceleration = 25 * 96.0 / (1000.0 * 1000.0);
    }

    void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        var manipulation = ToManipulation(e);

        if (OnPointerMoved([manipulation.Center], false))
            return;

        Map.Navigator.Manipulate(manipulation);
    }

    Manipulation ToManipulation(ManipulationDeltaRoutedEventArgs e)
    {
        // Get the top-left corner of the current control relative to the screen
        var transform = TransformToVisual(null); // null means relative to the root visual
        var topLeft = transform.TransformPoint(new Point(0, 0));

        // Calculate the relative position
        var relativePosition = new Point(e.Position.X - topLeft.X, e.Position.Y - topLeft.Y);

        var previousCenter = relativePosition.ToScreenPosition();
        var center = previousCenter.Offset(e.Delta.Translation.X, e.Delta.Translation.Y);

        return new Manipulation(center, previousCenter, e.Delta.Scale, e.Delta.Rotation, e.Cumulative.Rotation);
    }

    public void OpenInBrowser(string url)
    {
        Catch.TaskRun(async () => await Launcher.LaunchUriAsync(new Uri(url)));
    }

    public float? GetPixelDensity() => _renderControl.GetPixelDensity();

    bool GetShiftPressed() => _shiftPressed;

#if !HAS_UNO
    protected virtual void Dispose(bool disposing)
    {
        SharedDispose(disposing);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
#elif HAS_UNO && __IOS__ // on ios don't dispose _canvas, _canvasGPU, _selectRectangle, base class 
    protected new virtual void Dispose(bool disposing)
    {
        SharedDispose(disposing);
    }

    public new void Dispose()
    {
        GC.SuppressFinalize(this);
    }
#else
    protected virtual void Dispose(bool disposing)
    {
        CommonUnoDispose(disposing);
        SharedDispose(disposing);
        base.Dispose();
    }

    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    void CommonUnoDispose(bool disposing)
    {
        if (disposing)
        {
            _renderControl.Dispose();
            _selectRectangle?.Dispose();
        }
    }
#endif
}