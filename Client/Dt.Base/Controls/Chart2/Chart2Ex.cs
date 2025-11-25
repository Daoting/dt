#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-09-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using ScottPlot;
using ScottPlot.Interactivity;
using ScottPlot.Interactivity.UserActions;
using Windows.Foundation;
using Windows.System;
#endregion

namespace Dt.Base
{
    internal static class Chart2Ex
    {
        internal static Pixel Pixel(this PointerRoutedEventArgs e, Chart2 plotControl)
        {
            Point position = e.GetCurrentPoint(plotControl).Position;
            position.X *= plotControl.DisplayScale;
            position.Y *= plotControl.DisplayScale;
            return position.ToPixel();
        }

        internal static Pixel ToPixel(this Point p)
        {
            return new Pixel((float)p.X, (float)p.Y);
        }

        internal static void ProcessMouseDown(this UserInputProcessor processor, Chart2 plotControl, PointerRoutedEventArgs e)
        {
            Pixel pixel = e.Pixel(plotControl);
            PointerUpdateKind kind = e.GetCurrentPoint(plotControl).Properties.PointerUpdateKind;
            IUserAction action = kind switch
            {
                PointerUpdateKind.LeftButtonPressed => new LeftMouseDown(pixel),
                PointerUpdateKind.MiddleButtonPressed => new MiddleMouseDown(pixel),
                PointerUpdateKind.RightButtonPressed => new RightMouseDown(pixel),
                _ => new Unknown(kind.ToString(), "pressed"),
            };
            processor.Process(action);
        }

        internal static void ProcessMouseUp(this UserInputProcessor processor, Chart2 plotControl, PointerRoutedEventArgs e)
        {
            Pixel pixel = e.Pixel(plotControl);
            PointerUpdateKind kind = e.GetCurrentPoint(plotControl).Properties.PointerUpdateKind;
            IUserAction action = kind switch
            {
                PointerUpdateKind.LeftButtonReleased => new LeftMouseUp(pixel),
                PointerUpdateKind.MiddleButtonReleased => new MiddleMouseUp(pixel),
                PointerUpdateKind.RightButtonReleased => new RightMouseUp(pixel),
                _ => new Unknown(kind.ToString(), "released"),
            };
            processor.Process(action);
        }

        internal static void ProcessMouseMove(this UserInputProcessor processor, Chart2 plotControl, PointerRoutedEventArgs e)
        {
            Pixel pixel = e.Pixel(plotControl);
            IUserAction action = new MouseMove(pixel);
            processor.Process(action);
        }

        internal static void ProcessMouseWheel(this UserInputProcessor processor, Chart2 plotControl, PointerRoutedEventArgs e)
        {
            Pixel pixel = e.Pixel(plotControl);

            IUserAction action = e.GetCurrentPoint(plotControl).Properties.MouseWheelDelta > 0
                ? new MouseWheelUp(pixel)
                : new MouseWheelDown(pixel);

            processor.Process(action);
        }

        internal static void ProcessKeyDown(this UserInputProcessor processor, Chart2 plotControl, KeyRoutedEventArgs e)
        {
            Key key = e.ToKey();
            IUserAction action = new KeyDown(key);
            processor.Process(action);
        }

        internal static void ProcessKeyUp(this UserInputProcessor processor, Chart2 plotControl, KeyRoutedEventArgs e)
        {
            Key key = e.ToKey();
            IUserAction action = new KeyUp(key);
            processor.Process(action);
        }
        
        internal static Key ToKey(this KeyRoutedEventArgs e)
        {
            return e.Key switch
            {
                VirtualKey.Control => StandardKeys.Control,
                VirtualKey.LeftControl => StandardKeys.Control,
                VirtualKey.RightControl => StandardKeys.Control,

                VirtualKey.Menu => StandardKeys.Alt,
                VirtualKey.LeftMenu => StandardKeys.Alt,
                VirtualKey.RightMenu => StandardKeys.Alt,

                VirtualKey.Shift => StandardKeys.Shift,
                VirtualKey.LeftShift => StandardKeys.Shift,
                VirtualKey.RightShift => StandardKeys.Shift,
                _ => new Key(e.Key.ToString()),
            };
        }
    }
}



