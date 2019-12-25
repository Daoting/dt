using System;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Dt.Charts
{
    internal enum C1DragDirection
    {
        Vertical,
        Horizontal,
        Diagonal,
        None
    }

    public enum C1DragHelperMode : byte
    {
        Inertia = 4,
        TranslateRailX = 8,
        TranslateRailY = 0x10,
        TranslateX = 1,
        TranslateXY = 3,
        TranslateY = 2
    }

    public enum C1PointerDeviceType
    {
        Mouse,
        Touch,
        Pen
    }

    internal static class C1DragHelperModeExtensions
    {
        // Methods
        public static bool IsInertial(this C1DragHelperMode mode)
        {
            return (((byte)(mode & C1DragHelperMode.Inertia)) == 4);
        }

        public static bool TranslateRailX(this C1DragHelperMode mode)
        {
            return (((byte)(mode & C1DragHelperMode.TranslateRailX)) == 8);
        }

        public static bool TranslateRailY(this C1DragHelperMode mode)
        {
            return (((byte)(mode & C1DragHelperMode.TranslateRailY)) == 0x10);
        }

        public static bool TranslateX(this C1DragHelperMode mode)
        {
            return (((byte)(mode & C1DragHelperMode.TranslateX)) == 1);
        }

        public static bool TranslateY(this C1DragHelperMode mode)
        {
            return (((byte)(mode & C1DragHelperMode.TranslateY)) == 2);
        }
    }

    public class C1DragCompletedEventArgs : C1DragEventArgs
    {
        // Methods
        internal C1DragCompletedEventArgs(C1DragHelper helper, RoutedEventArgs originalArgs, C1PointerDeviceType pointerType, Point cumulativeTranslation)
            : base(helper, originalArgs, pointerType)
        {
            CumulativeTranslation = cumulativeTranslation;
        }

        // Properties
        public Point CumulativeTranslation { get; internal set; }
    }

    public class C1DragDeltaEventArgs : C1DragEventArgs
    {
        // Methods
        internal C1DragDeltaEventArgs(C1DragHelper helper, RoutedEventArgs originalArgs, C1PointerDeviceType pointerType, Point deltaTranslation, Point cumulativeTranslation, bool isInertial)
            : base(helper, originalArgs, pointerType)
        {
            DeltaTranslation = deltaTranslation;
            CumulativeTranslation = cumulativeTranslation;
            IsInertial = isInertial;
        }

        public void Complete()
        {
            base.DragHelper.Complete(null);
        }

        // Properties
        public Point CumulativeTranslation { get; set; }

        public Point DeltaTranslation { get; set; }

        public bool IsInertial { get; set; }
    }

    public class C1DragInertiaStartedEventArgs : C1DragEventArgs
    {
        // Methods
        internal C1DragInertiaStartedEventArgs(C1DragHelper helper, RoutedEventArgs originalArgs, C1PointerDeviceType pointerType, Point finalVelocity, double desiredDeceleration)
            : base(helper, originalArgs, pointerType)
        {
            Velocity = finalVelocity;
            DesiredDeceleration = desiredDeceleration;
        }

        // Properties
        public double DesiredDeceleration { get; set; }

        public Point Velocity { get; set; }
    }

    public class C1DragStartedEventArgs : C1DragEventArgs
    {
        // Methods
        internal C1DragStartedEventArgs(C1DragHelper helper, RoutedEventArgs originalArgs, C1PointerDeviceType pointerType, Point origin, C1DragDirection direction)
            : base(helper, originalArgs, pointerType)
        {
            Origin = origin;
            Direction = direction;
        }

        // Properties
        internal C1DragDirection Direction { get; set; }

        public Point Origin { get; set; }
    }

    public class C1DragStartingEventArgs : C1DragEventArgs
    {
        // Methods
        internal C1DragStartingEventArgs(C1DragHelper helper, RoutedEventArgs originalArgs, C1PointerDeviceType pointerType, C1DragHelperMode mode, bool captureElementOnPointerPressed, double initialThreshold)
            : base(helper, originalArgs, pointerType)
        {
            Mode = mode;
            CaptureElementOnPointerPressed = captureElementOnPointerPressed;
            InitialThreshold = initialThreshold;
        }

        // Properties
        public bool Cancel { get; set; }

        public bool CaptureElementOnPointerPressed { get; set; }

        public double InitialThreshold { get; set; }

        public C1DragHelperMode Mode { get; set; }
    }


    public abstract class C1DragEventArgs : C1InputEventArgs
    {
        // Methods
        internal C1DragEventArgs(C1DragHelper dragHelper, RoutedEventArgs originalArgs, C1PointerDeviceType pointerType)
            : base(originalArgs, pointerType)
        {
            DragHelper = dragHelper;
        }

        // Properties
        public C1DragHelper DragHelper { get; set; }
    }

    public abstract class C1InputEventArgs : EventArgs
    {
        // Methods
        internal C1InputEventArgs(RoutedEventArgs originalArgs)
        {
            OriginalEventArgs = originalArgs;
            PointerDeviceType = GetPointerType(originalArgs);
        }

        internal C1InputEventArgs(RoutedEventArgs originalArgs, C1PointerDeviceType pointerType)
        {
            OriginalEventArgs = originalArgs;
            PointerDeviceType = pointerType;
        }

        internal static bool GetIsHandled(RoutedEventArgs e)
        {
            if (e is PointerRoutedEventArgs)
            {
                return ((PointerRoutedEventArgs)e).Handled;
            }
            if (e is ManipulationStartingRoutedEventArgs)
            {
                return ((ManipulationStartingRoutedEventArgs)e).Handled;
            }
            if (e is ManipulationStartedRoutedEventArgs)
            {
                return ((ManipulationStartedRoutedEventArgs)e).Handled;
            }
            if (e is ManipulationDeltaRoutedEventArgs)
            {
                return ((ManipulationDeltaRoutedEventArgs)e).Handled;
            }
            if (e is ManipulationInertiaStartingRoutedEventArgs)
            {
                return ((ManipulationInertiaStartingRoutedEventArgs)e).Handled;
            }
            if (e is ManipulationCompletedRoutedEventArgs)
            {
                return ((ManipulationCompletedRoutedEventArgs)e).Handled;
            }
            if (e is TappedRoutedEventArgs)
            {
                return ((TappedRoutedEventArgs)e).Handled;
            }
            if (e is DoubleTappedRoutedEventArgs)
            {
                return ((DoubleTappedRoutedEventArgs)e).Handled;
            }
            if (e is RightTappedRoutedEventArgs)
            {
                return ((RightTappedRoutedEventArgs)e).Handled;
            }
            return ((e is HoldingRoutedEventArgs) && ((HoldingRoutedEventArgs)e).Handled);
        }

        public static C1PointerDeviceType GetPointerType(RoutedEventArgs originalArgs)
        {
            PointerDeviceType mouse = Windows.Devices.Input.PointerDeviceType.Mouse;
            if (originalArgs is PointerRoutedEventArgs)
            {
                mouse = ((PointerRoutedEventArgs)originalArgs).Pointer.PointerDeviceType;
            }
            else if (originalArgs is ManipulationStartedRoutedEventArgs)
            {
                mouse = ((ManipulationStartedRoutedEventArgs)originalArgs).PointerDeviceType;
            }
            else if (originalArgs is ManipulationDeltaRoutedEventArgs)
            {
                mouse = ((ManipulationDeltaRoutedEventArgs)originalArgs).PointerDeviceType;
            }
            else if (originalArgs is ManipulationInertiaStartingRoutedEventArgs)
            {
                mouse = ((ManipulationInertiaStartingRoutedEventArgs)originalArgs).PointerDeviceType;
            }
            else if (originalArgs is ManipulationCompletedRoutedEventArgs)
            {
                mouse = ((ManipulationCompletedRoutedEventArgs)originalArgs).PointerDeviceType;
            }
            else if (originalArgs is TappedRoutedEventArgs)
            {
                mouse = ((TappedRoutedEventArgs)originalArgs).PointerDeviceType;
            }
            else if (originalArgs is DoubleTappedRoutedEventArgs)
            {
                mouse = ((DoubleTappedRoutedEventArgs)originalArgs).PointerDeviceType;
            }
            else if (originalArgs is RightTappedRoutedEventArgs)
            {
                mouse = ((RightTappedRoutedEventArgs)originalArgs).PointerDeviceType;
            }
            else if (originalArgs is HoldingRoutedEventArgs)
            {
                mouse = ((HoldingRoutedEventArgs)originalArgs).PointerDeviceType;
            }
            switch (mouse)
            {
                case Windows.Devices.Input.PointerDeviceType.Touch:
                    return C1PointerDeviceType.Touch;

                case Windows.Devices.Input.PointerDeviceType.Pen:
                    return C1PointerDeviceType.Pen;
            }
            return C1PointerDeviceType.Mouse;
        }

        public Point GetPosition(UIElement relativeTo)
        {
            return GetPosition(OriginalEventArgs, relativeTo);
        }

        public static Point GetPosition(RoutedEventArgs args, UIElement relativeTo)
        {
            if (args is PointerRoutedEventArgs)
            {
                return ((PointerRoutedEventArgs)args).GetCurrentPoint(relativeTo).Position;
            }
            if (args is ManipulationStartedRoutedEventArgs)
            {
                ManipulationStartedRoutedEventArgs args2 = args as ManipulationStartedRoutedEventArgs;
                return DoTransform(args2.Container, relativeTo).TransformPoint(args2.Position);
            }
            if (args is ManipulationDeltaRoutedEventArgs)
            {
                ManipulationDeltaRoutedEventArgs args3 = args as ManipulationDeltaRoutedEventArgs;
                return DoTransform(args3.Container, relativeTo).TransformPoint(args3.Position);
            }
            if (args is ManipulationCompletedRoutedEventArgs)
            {
                ManipulationCompletedRoutedEventArgs args4 = args as ManipulationCompletedRoutedEventArgs;
                return DoTransform(args4.Container, relativeTo).TransformPoint(args4.Position);
            }
            if (args is TappedRoutedEventArgs)
            {
                return ((TappedRoutedEventArgs)args).GetPosition(relativeTo);
            }
            if (args is DoubleTappedRoutedEventArgs)
            {
                return ((DoubleTappedRoutedEventArgs)args).GetPosition(relativeTo);
            }
            if (args is RightTappedRoutedEventArgs)
            {
                return ((RightTappedRoutedEventArgs)args).GetPosition(relativeTo);
            }
            if (args is HoldingRoutedEventArgs)
            {
                return ((HoldingRoutedEventArgs)args).GetPosition(relativeTo);
            }
            return new Point();
        }

        internal static void SetIsHandled(RoutedEventArgs e, bool handled)
        {
            if (e is PointerRoutedEventArgs)
            {
                ((PointerRoutedEventArgs)e).Handled = handled;
            }
            else if (e is ManipulationStartingRoutedEventArgs)
            {
                ((ManipulationStartingRoutedEventArgs)e).Handled = handled;
            }
            else if (e is ManipulationStartedRoutedEventArgs)
            {
                ((ManipulationStartedRoutedEventArgs)e).Handled = handled;
            }
            else if (e is ManipulationDeltaRoutedEventArgs)
            {
                ((ManipulationDeltaRoutedEventArgs)e).Handled = handled;
            }
            else if (e is ManipulationInertiaStartingRoutedEventArgs)
            {
                ((ManipulationInertiaStartingRoutedEventArgs)e).Handled = handled;
            }
            else if (e is ManipulationCompletedRoutedEventArgs)
            {
                ((ManipulationCompletedRoutedEventArgs)e).Handled = handled;
            }
            else if (e is TappedRoutedEventArgs)
            {
                ((TappedRoutedEventArgs)e).Handled = handled;
            }
            else if (e is DoubleTappedRoutedEventArgs)
            {
                ((DoubleTappedRoutedEventArgs)e).Handled = handled;
            }
            else if (e is RightTappedRoutedEventArgs)
            {
                ((RightTappedRoutedEventArgs)e).Handled = handled;
            }
            else if (e is HoldingRoutedEventArgs)
            {
                ((HoldingRoutedEventArgs)e).Handled = handled;
            }
        }

        // Properties
        public bool Handled
        {
            get { return  GetIsHandled(OriginalEventArgs); }
            set { SetIsHandled(OriginalEventArgs, value); }
        }

        public RoutedEventArgs OriginalEventArgs { get; set; }

        public object OriginalSource
        {
            get { return  null; }
        }

        public C1PointerDeviceType PointerDeviceType { get; set; }

        static GeneralTransform DoTransform(UIElement element, UIElement visual)
        {
            try
            {
                return element.TransformToVisual(visual);
            }
            catch
            {
                MatrixTransform transform = new MatrixTransform();
                transform.Matrix = Matrix.Identity;
                return transform;
            }
        }
    }


}
