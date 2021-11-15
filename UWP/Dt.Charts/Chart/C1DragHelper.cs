using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Dt.Charts
{
    public class C1DragHelper
    {
        bool _actualCaptureElementOnMouseDown;
        double _actualInitialThreshold;
        C1DragHelperMode _actualMode;
        bool _captureElementOnMouseDown;
        Point _cumulativeTranslation;
        double _deceleration = double.NaN;
        C1DragDirection _direction = C1DragDirection.None;
        bool _dragStarted;
        UIElement _element;
        bool _handledEventsToo;
        double _initialThreshold = 16.0;
        DateTime _initialTimeStamp;
        Point _lastPos = new Point();
        DateTime _lastTimeStamp;
        bool _listenManipulationEvents;
        bool _listenPointerEvents;
        bool _manipulationStarted;
        C1DragHelperMode _mode;
        Point _origin;
        bool _pointerPressed;
        C1PointerDeviceType _pointerType;
        Stack<KeyValuePair<DateTime, Point>> _points = new Stack<KeyValuePair<DateTime, Point>>();
        bool _useRightButton;
        Point _velocity;

        public event EventHandler<C1DragCompletedEventArgs> DragCompleted;

        public event EventHandler<C1DragDeltaEventArgs> DragDelta;

        public event EventHandler<C1DragInertiaStartedEventArgs> DragInertiaStarted;

        public event EventHandler<C1DragStartedEventArgs> DragStarted;

        public event EventHandler<C1DragStartingEventArgs> DragStarting;

        public C1DragHelper(UIElement element, C1DragHelperMode mode = C1DragHelperMode.TranslateXY, double initialThreshold = 1.0, bool captureElementOnPointerPressed = true, bool handledEventsToo = false, bool useManipulationEvents = false, bool useRightButton = false)
        {
            _element = element;
            _mode = mode;
            _initialThreshold = initialThreshold;
            _handledEventsToo = handledEventsToo;
            _captureElementOnMouseDown = captureElementOnPointerPressed;
            _listenManipulationEvents = true;
            _listenPointerEvents = !useManipulationEvents;
            _useRightButton = useRightButton;
            InitializeDrag();
        }

        public void Complete()
        {
            Complete(null);
        }

        public void FinalizeDrag()
        {
            if (_handledEventsToo)
            {
                _element.RemoveHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed));
                _element.RemoveHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(OnPointerReleased));
                _element.RemoveHandler(UIElement.PointerMovedEvent, new PointerEventHandler(OnPointerMoved));
                _element.RemoveHandler(UIElement.PointerCaptureLostEvent, new PointerEventHandler(OnPointerCaptureLost));
                _element.RemoveHandler(UIElement.PointerCanceledEvent, new PointerEventHandler(OnPointerCanceled));
            }
            else
            {
                _element.PointerPressed -= OnPointerPressed;
                _element.PointerReleased -= OnPointerReleased;
                _element.PointerMoved -= OnPointerMoved;
                _element.PointerCaptureLost -= OnPointerCaptureLost;
                _element.PointerCanceled -= OnPointerCanceled;
            }
            ReleaseMouseCapture();
            _element.ManipulationInertiaStarting -= OnManipulationInertiaStarting;
            _element.ManipulationStarted -= OnManipulationStarted;
            _element.ManipulationDelta -= OnManipulationDelta;
            _element.ManipulationCompleted -= OnManipulationCompleted;
        }

        internal void Complete(RoutedEventArgs originalArgs = null)
        {
            if (_dragStarted)
            {
                _dragStarted = false;
                _pointerPressed = false;
                ReleaseMouseCapture();
                _manipulationStarted = false;
                _velocity = new Point();
                _direction = C1DragDirection.None;
                CompositionTarget.Rendering -= OnRendering;
                RaiseDragCompleted(originalArgs, _cumulativeTranslation);
            }
        }

        double Decelerate(double velocity, double elapsedTimeMilliseconds)
        {
            double num = double.IsNaN(_deceleration) ? 0.0039780645161290326 : _deceleration;
            double num2 = num * elapsedTimeMilliseconds;
            return (Math.Sign(velocity) * Math.Max((double)0.0, (double)(Math.Abs(velocity) - num2)));
        }

        void InitializeDrag()
        {
            _dragStarted = false;
            if (_listenPointerEvents)
            {
                _lastPos = new Point();
                _pointerPressed = false;
                if (_handledEventsToo)
                {
                    _element.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
                    _element.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
                    _element.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(OnPointerMoved), true);
                    _element.AddHandler(UIElement.PointerCaptureLostEvent, new PointerEventHandler(OnPointerCaptureLost), true);
                    _element.AddHandler(UIElement.PointerCanceledEvent, new PointerEventHandler(OnPointerCanceled), true);
                }
                else
                {
                    _element.PointerPressed += OnPointerPressed;
                    _element.PointerReleased += OnPointerReleased;
                    _element.PointerMoved += OnPointerMoved;
                    _element.PointerCaptureLost += OnPointerCaptureLost;
                    _element.PointerCanceled += OnPointerCanceled;
                }
            }

            if (_listenManipulationEvents)
            {
                UpdateManipulationMode();
                _element.ManipulationInertiaStarting += OnManipulationInertiaStarting;
                _element.ManipulationStarted += OnManipulationStarted;
                _element.ManipulationDelta += OnManipulationDelta;
                _element.ManipulationCompleted += OnManipulationCompleted;
            }
        }

        Point GetFinalVelocities()
        {
            if (_points.Count >= 2)
            {
                KeyValuePair<DateTime, Point> pair = _points.Pop();
                List<Point> list = new List<Point>();
                DateTime time = pair.Key;
                Point point = pair.Value;
                do
                {
                    KeyValuePair<DateTime, Point> pair2 = _points.Pop();
                    TimeSpan span = (TimeSpan)(time - pair2.Key);
                    double totalMilliseconds = span.TotalMilliseconds;
                    if (totalMilliseconds > 150.0)
                    {
                        break;
                    }
                    Point point2 = pair2.Value;
                    if (totalMilliseconds > 0.0)
                    {
                        Point point3 = new Point((point.X - point2.X) / totalMilliseconds, (point.Y - point2.Y) / totalMilliseconds);
                        list.Add(point3);
                    }
                }
                while (_points.Count > 0);
                if (list.Count > 0)
                {
                    return new Point(Enumerable.Average<Point>((IEnumerable<Point>)list, delegate(Point p)
                    {
                        return p.X;
                    }), Enumerable.Average<Point>((IEnumerable<Point>)list, delegate(Point p)
                    {
                        return p.Y;
                    }));
                }
            }
            return new Point();
        }

        bool IsMovingByInertia()
        {
            if (_velocity.X == 0.0)
            {
                return (_velocity.Y != 0.0);
            }
            return true;
        }

        void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (_dragStarted)
            {
                Complete(e);
            }
        }

        void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_manipulationStarted)
            {
                Point translation = e.Delta.Translation;
                Point point2 = e.Cumulative.Translation;
                if (!_dragStarted && (((Math.Abs(point2.X) > _actualInitialThreshold) && _actualMode.TranslateX()) || ((Math.Abs(point2.Y) > _actualInitialThreshold) && _actualMode.TranslateY())))
                {
                    Start(e);
                }
                if (_dragStarted)
                {
                    _cumulativeTranslation = new Point(_cumulativeTranslation.X + translation.X, _cumulativeTranslation.Y + translation.Y);
                    RaiseDragDelta(e, _cumulativeTranslation, translation, e.IsInertial);
                }
            }
        }

        void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            if (_dragStarted)
            {
                Point velocity = new Point(e.Velocities.Linear.X, e.Velocities.Linear.Y);
                double d = RaiseDragInertiaStarted(e, velocity, 0.00096);
                if (!double.IsNaN(d))
                {
                    e.TranslationBehavior.DesiredDeceleration = d;
                }
            }
        }

        void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _pointerType = C1InputEventArgs.GetPointerType(e);
            if ((_pointerType == C1PointerDeviceType.Touch) || !_listenPointerEvents)
            {
                Complete(e);
                _origin = C1InputEventArgs.GetPosition(e, null);
                _manipulationStarted = RaiseDragStarting(e);
                if (_manipulationStarted && (_actualInitialThreshold == 0.0))
                {
                    Start(e);
                }
            }
        }

        void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
        }

        void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (!_useRightButton && (_dragStarted && !IsMovingByInertia()))
            {
                Complete(e);
            }
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_pointerPressed)
            {
                Point position = C1InputEventArgs.GetPosition(e, null);
                if (!_dragStarted)
                {
                    Point point2 = new Point(position.X - _origin.X, position.Y - _origin.Y);
                    if (((Math.Abs(point2.X) > _actualInitialThreshold) && _actualMode.TranslateX()) || ((Math.Abs(point2.Y) > _actualInitialThreshold) && _actualMode.TranslateY()))
                    {
                        if (_actualCaptureElementOnMouseDown || TryCaptureMouse(e))
                        {
                            Start(e);
                        }
                        else
                        {
                            _pointerPressed = false;
                        }
                    }
                }
                if (_dragStarted)
                {
                    Point deltaTranslation = new Point(position.X - _lastPos.X, position.Y - _lastPos.Y);
                    _points.Push(new KeyValuePair<DateTime, Point>(DateTime.Now, position));
                    _cumulativeTranslation = new Point(_cumulativeTranslation.X + deltaTranslation.X, _cumulativeTranslation.Y + deltaTranslation.Y);
                    RaiseDragDelta(e, _cumulativeTranslation, deltaTranslation, false);
                    _lastPos = position;
                }
            }
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _pointerType = C1InputEventArgs.GetPointerType(e);
            if (_pointerType != C1PointerDeviceType.Touch)
            {
                Complete(e);
                _origin = C1InputEventArgs.GetPosition(e, null);
                _lastPos = _origin;
                if (RaiseDragStarting(e))
                {
                    if (!_actualCaptureElementOnMouseDown || TryCaptureMouse(e))
                    {
                        _pointerPressed = true;
                    }
                    if (_pointerPressed && (_actualInitialThreshold == 0.0))
                    {
                        Start(e);
                    }
                    if (_useRightButton)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_pointerPressed)
            {
                _pointerPressed = false;
                if (_dragStarted)
                {
                    Point position = C1InputEventArgs.GetPosition(e, null);
                    _points.Push(new KeyValuePair<DateTime, Point>(DateTime.Now, position));
                    Point finalVelocities = GetFinalVelocities();
                    if ((finalVelocities != new Point()) && _actualMode.IsInertial())
                    {
                        StartInertia(e, finalVelocities);
                    }
                    else
                    {
                        Complete(e);
                    }
                }
                _points.Clear();
                ReleaseMouseCapture();
            }
        }

        void OnRendering(object sender, object e)
        {
            double num = 0.001;
            DateTime time = DateTime.Now;
            TimeSpan span = (TimeSpan)(time - _initialTimeStamp);
            double totalMilliseconds = span.TotalMilliseconds;
            TimeSpan span2 = (TimeSpan)(time - _lastTimeStamp);
            double num3 = span2.TotalMilliseconds;
            _lastTimeStamp = time;
            double introduced10 = Math.Abs(_velocity.X);
            double velocity = Math.Max(introduced10, Math.Abs(_velocity.Y));
            double num5 = Decelerate(velocity, totalMilliseconds);
            Point deltaTranslation = new Point
            {
                X = ((_velocity.X / velocity) * num5) * num3,
                Y = ((_velocity.Y / velocity) * num5) * num3
            };
            _cumulativeTranslation = new Point(_cumulativeTranslation.X + deltaTranslation.X, _cumulativeTranslation.Y + deltaTranslation.Y);
            RaiseDragDelta(null, _cumulativeTranslation, deltaTranslation, true);
            if (Math.Abs(Math.Round(num5, 2)) <= num)
            {
                Complete(null);
            }
        }

        void RaiseDragCompleted(RoutedEventArgs originalArgs, Point cumulativeTranslation)
        {
            if (DragCompleted != null)
            {
                DragCompleted(_element, new C1DragCompletedEventArgs(this, originalArgs, _pointerType, new Point(_actualMode.TranslateX() ? cumulativeTranslation.X : 0.0, _actualMode.TranslateY() ? cumulativeTranslation.Y : 0.0)));
            }
        }

        void RaiseDragDelta(RoutedEventArgs originalArgs, Point cumulativeTranslation, Point deltaTranslation, bool isInertial)
        {
            if (DragDelta != null)
            {
                bool flag = _actualMode.TranslateX() && (!_actualMode.TranslateRailY() || (_direction != C1DragDirection.Vertical));
                bool flag2 = _actualMode.TranslateY() && (!_actualMode.TranslateRailX() || (_direction != C1DragDirection.Horizontal));
                DragDelta(_element, new C1DragDeltaEventArgs(this, originalArgs, _pointerType, new Point(flag ? deltaTranslation.X : 0.0, flag2 ? deltaTranslation.Y : 0.0), new Point(flag ? cumulativeTranslation.X : 0.0, flag2 ? cumulativeTranslation.Y : 0.0), isInertial));
            }
        }

        double RaiseDragInertiaStarted(RoutedEventArgs originalArgs, Point velocity, double desiredDeceleration = 0.00096)
        {
            if (DragInertiaStarted != null)
            {
                bool flag = _actualMode.TranslateX() && (!_actualMode.TranslateRailY() || (_direction != C1DragDirection.Vertical));
                bool flag2 = _actualMode.TranslateY() && (!_actualMode.TranslateRailX() || (_direction != C1DragDirection.Horizontal));
                C1DragInertiaStartedEventArgs args = new C1DragInertiaStartedEventArgs(this, originalArgs, _pointerType, new Point(flag ? velocity.X : 0.0, flag2 ? velocity.Y : 0.0), desiredDeceleration);
                DragInertiaStarted(_element, args);
                return args.DesiredDeceleration;
            }
            return desiredDeceleration;
        }

        void RaiseDragStarted(RoutedEventArgs originalArgs)
        {
            if (DragStarted != null)
            {
                DragStarted(_element, new C1DragStartedEventArgs(this, originalArgs, _pointerType, _origin, _direction));
            }
        }

        bool RaiseDragStarting(RoutedEventArgs e)
        {
            C1DragStartingEventArgs args = new C1DragStartingEventArgs(this, e, _pointerType, _mode, _captureElementOnMouseDown, _initialThreshold);
            if (DragStarting != null)
            {
                DragStarting(this, args);
            }
            _actualMode = args.Mode;
            _actualCaptureElementOnMouseDown = (args.InitialThreshold == 0.0) || args.CaptureElementOnPointerPressed;
            _actualInitialThreshold = args.InitialThreshold;
            return !args.Cancel;
        }

        void ReleaseMouseCapture()
        {
            if (_element != null)
            {
                _element.ReleasePointerCaptures();
            }
        }

        void Start(RoutedEventArgs originalArgs)
        {
            Complete(originalArgs);
            _cumulativeTranslation = new Point();
            _dragStarted = true;
            UpdateGestureDirection(originalArgs);
            RaiseDragStarted(originalArgs);
        }

        void StartInertia(RoutedEventArgs originalArgs, Point velocities)
        {
            _deceleration = RaiseDragInertiaStarted(originalArgs, velocities, 0.00096);
            CompositionTarget.Rendering += OnRendering;
            _velocity = velocities;
            _initialTimeStamp = _lastTimeStamp = DateTime.Now;
        }

        bool TryCaptureMouse(RoutedEventArgs e)
        {
            if (e is PointerRoutedEventArgs)
            {
                return _element.CapturePointer((e as PointerRoutedEventArgs).Pointer);
            }
            return true;
        }

        void UpdateGestureDirection(RoutedEventArgs originalArgs)
        {
            if (_direction == C1DragDirection.None)
            {
                Point position = C1InputEventArgs.GetPosition(originalArgs, null);
                double x = position.X - _origin.X;
                double num2 = position.Y - _origin.Y;
                if ((x != 0.0) || (num2 != 0.0))
                {
                    double num3 = Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(num2, 2.0));
                    double num4 = x / num3;
                    double num5 = num2 / num3;
                    if (Math.Abs(num5) < 0.4)
                    {
                        _direction = C1DragDirection.Horizontal;
                    }
                    else if (Math.Abs(num4) < 0.4)
                    {
                        _direction = C1DragDirection.Vertical;
                    }
                    else
                    {
                        _direction = C1DragDirection.Diagonal;
                    }
                }
            }
        }

        void UpdateManipulationMode()
        {
            ManipulationModes modes = (_element.ManipulationMode == (ManipulationModes.None | ManipulationModes.System)) ? ManipulationModes.None : _element.ManipulationMode;
            if (_mode.TranslateRailX())
            {
                modes |= ManipulationModes.None | ManipulationModes.TranslateRailsX | ManipulationModes.TranslateX;
            }
            if (_mode.TranslateRailY())
            {
                modes |= ManipulationModes.None | ManipulationModes.TranslateRailsY | ManipulationModes.TranslateY;
            }
            if (_mode.TranslateX())
            {
                modes |= ManipulationModes.None | ManipulationModes.TranslateX;
            }
            if (_mode.TranslateY())
            {
                modes |= ManipulationModes.None | ManipulationModes.TranslateY;
            }
            if (_mode.IsInertial())
            {
                modes |= ManipulationModes.None | ManipulationModes.TranslateInertia;
            }
            _element.ManipulationMode = modes;
        }
    }
}
