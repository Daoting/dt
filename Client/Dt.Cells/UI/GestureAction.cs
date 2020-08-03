#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Helper base class to provide state machine-like behavior for determining which gesture is occurring
    /// </summary>
    internal abstract class GestureAction
    {
        Windows.Foundation.Point _actionCurrentPosition;
        Windows.Foundation.Point _actionPreviousSamplePosition;
        Windows.Foundation.Point _actionStartingPosition;
        TapStatus _actionTapStatus;
        readonly DispatcherTimer _actionTimer = new DispatcherTimer();
        readonly UIElement _gestureElement;
        static readonly TimeSpan DoubleTapTimeout = TimeSpan.FromMilliseconds(250.0);
        static readonly TimeSpan TapAndHoldTimeout = TimeSpan.FromMilliseconds(800.0);

        public event EventHandler ActionCompleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UI.GestureAction" /> class.
        /// </summary>
        /// <param name="gestureElement">The UI element.</param>
        protected GestureAction(UIElement gestureElement)
        {
            if (gestureElement == null)
            {
                throw new ArgumentNullException("gestureElement");
            }
            _gestureElement = gestureElement;
            DispatcherTimer timer = _actionTimer;
            timer.Tick += OnActionTimerTick;
            ActionGestureOrientation = DragOrientation.None;
        }

        /// <summary>
        /// Notifies the machine that manipulation has been completed.
        /// </summary>
        /// <param name="currentPosition">The current position.</param>
        public void HandleManipulationCompleted(Windows.Foundation.Point currentPosition)
        {
            _actionTimer.Stop();
            switch (ActionTapStatus)
            {
                case TapStatus.HoldPending:
                    ProcessManipulationEndForHoldPending();
                    _actionTimer.Interval = DoubleTapTimeout;
                    _actionTimer.Start();
                    return;

                case TapStatus.HoldOcurred:
                    ProcessManipulationEndForHoldOccured();
                    _actionTimer.Interval = DoubleTapTimeout;
                    _actionTimer.Start();
                    return;

                case TapStatus.DragOcurringFirstTime:
                case TapStatus.DragOcurring:
                    ProcessManipulationEndForDragOcurring(currentPosition);
                    _actionTimer.Stop();
                    ActionCompleted(this, EventArgs.Empty);
                    return;

                case TapStatus.PinchOcurringFirstTime:
                case TapStatus.PinchOcurring:
                    ProcessManipulationEndForPinchOccurring(currentPosition);
                    _actionTimer.Stop();
                    ActionCompleted(this, EventArgs.Empty);
                    return;
            }
            _actionTimer.Stop();
            ActionCompleted(this, EventArgs.Empty);
        }

        public void HandleMultipleManipulationDelta(Windows.Foundation.Point currentPosition, Windows.Foundation.Point scaleDelta)
        {
            switch (ActionTapStatus)
            {
                case TapStatus.PinchOcurringFirstTime:
                case TapStatus.PinchOcurring:
                    ProcessManipulationDeltaForPinchOccurring(currentPosition, scaleDelta);
                    return;

                case TapStatus.Pending:
                    ProcessManipulationForPinchStart(currentPosition);
                    return;
            }
            _actionTimer.Stop();
            ActionCompleted(this, EventArgs.Empty);
            ActionTapStatus = TapStatus.Pending;
            HandleMultipleManipulationDelta(currentPosition, scaleDelta);
        }

        /// <summary>
        /// Notifies the machine that a manipulation delta has occurred.
        /// </summary>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="offsetFromOrigin">The offset from origin.</param>
        public void HandleSingleManipulationDelta(Windows.Foundation.Point currentPosition, Windows.Foundation.Point offsetFromOrigin)
        {
            if (IsMoveAboveThreshold(offsetFromOrigin))
            {
                _actionTimer.Stop();
                switch (ActionTapStatus)
                {
                    case TapStatus.HoldPending:
                        ProcessManipulationDeltaForHoldPending(currentPosition);
                        return;

                    case TapStatus.HoldOcurred:
                        ProcessManipulationDeltaForHoldPending(currentPosition);
                        return;

                    case TapStatus.DragOcurringFirstTime:
                    case TapStatus.DragOcurring:
                        ProcessManipulationDeltaForDragOcurring(currentPosition);
                        return;

                    case TapStatus.PinchOcurringFirstTime:
                    case TapStatus.PinchOcurring:
                        ProcessManipulationEndForPinchOccurring(currentPosition);
                        return;
                }
                _actionTimer.Stop();
                ActionCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Notifies the machine that manipulation has started.
        /// </summary>
        /// <param name="currentPosition">The current position.</param>
        public void HandleSingleManipulationStarted(Windows.Foundation.Point currentPosition)
        {
            _actionTimer.Stop();
            TapStatus actionTapStatus = ActionTapStatus;
            if (actionTapStatus == TapStatus.Pending)
            {
                ProcessManipulationStartForPending(currentPosition);
                _actionTimer.Interval = TapAndHoldTimeout;
                _actionTimer.Start();
            }
            else if (actionTapStatus == TapStatus.DoubleTapPending)
            {
                ProcessManipulationStartForDoubleTapPending();
                ActionCompleted(this, EventArgs.Empty);
            }
            else
            {
                ActionCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Initializes the resources for this action source.
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            return OnInitialize();
        }

        bool IsMoveAboveThreshold(Windows.Foundation.Point offsetFromOrigin)
        {
            return (offsetFromOrigin.Offset() > MoveThreshold);
        }

        void OnActionTimerTick(object sender, object e)
        {
            _actionTimer.Stop();
            if (ActionTapStatus == TapStatus.HoldPending)
            {
                ProcessGestureTimerForHoldPending();
            }
            else if (ActionTapStatus == TapStatus.DoubleTapPending)
            {
                ProcessGestureTimerForDoubleTapPending();
                ActionCompleted(this, EventArgs.Empty);
            }
        }

        protected virtual bool OnInitialize()
        {
            return true;
        }

        protected virtual void OnRelease()
        {
        }

        void ProcessGestures(GestureType gestureType)
        {
            if (ActionGestureOrientation == DragOrientation.None
                && (ActionSampleDelta.X != 0.0 || ActionSampleDelta.Y != 0.0))
            {
                double num = Math.Atan(Math.Abs(ActionSampleDelta.Y) / Math.Abs(ActionSampleDelta.X)) * 57.295779513082323;
                if (num > 55.0)
                {
                    ActionGestureOrientation = DragOrientation.Vertical;
                }
                else if (num > 35.0)
                {
                    ActionGestureOrientation = DragOrientation.Horizontal | DragOrientation.Vertical;
                }
                else
                {
                    ActionGestureOrientation = DragOrientation.Horizontal;
                }
            }

            switch (gestureType)
            {
                case GestureType.Pinch:
                case GestureType.DragComplete:
                case GestureType.PinchComplete:
                case GestureType.None:
                case (GestureType.DoubleTap | GestureType.Tap):
                case (GestureType.Hold | GestureType.Tap):
                case (GestureType.Hold | GestureType.DoubleTap):
                case (GestureType.Hold | GestureType.DoubleTap | GestureType.Tap):
                case GestureType.HorizontalDrag:
                case GestureType.VerticalDrag:
                    break;

                case GestureType.Tap:
                    if (View == null)
                    {
                        break;
                    }
                    View.OnTouchTap(ActionCurrentPosition);
                    return;

                case GestureType.DoubleTap:
                    if (View == null)
                    {
                        break;
                    }
                    View.OnTouchTap(ActionCurrentPosition);
                    View.OnTouchDoubleTap(ActionCurrentPosition);
                    return;

                case GestureType.Hold:
                    View.ProcessTouchHold(ActionCurrentPosition);
                    return;

                case GestureType.FreeDrag:
                    if (View != null)
                    {
                        View.ProcessTouchFreeDrag(ActionStartingPosition, ActionCurrentPosition, ActionSampleDelta, ActionGestureOrientation);
                        _actionPreviousSamplePosition = ActionCurrentPosition;
                    }
                    break;

                default:
                    return;
            }
        }

        void ProcessGestureTimerForDoubleTapPending()
        {
            ActionTapStatus = TapStatus.Tap;
            ProcessGestures(GestureType.Tap);
        }

        void ProcessGestureTimerForHoldPending()
        {
            ActionTapStatus = TapStatus.HoldOcurred;
            ProcessGestures(GestureType.Hold);
        }

        void ProcessManipulationDeltaForDragOcurring(Windows.Foundation.Point currentPosition)
        {
            ActionCurrentPosition = currentPosition;
            ActionTapStatus = TapStatus.DragOcurring;
            ProcessGestures(GestureType.FreeDrag);
        }

        void ProcessManipulationDeltaForHoldPending(Windows.Foundation.Point currentPosition)
        {
            ActionCurrentPosition = currentPosition;
            ActionTapStatus = TapStatus.DragOcurringFirstTime;
            ProcessGestures(GestureType.FreeDrag);
        }

        void ProcessManipulationDeltaForPinchOccurring(Windows.Foundation.Point currentPosition, Windows.Foundation.Point scaleDelta)
        {
            ActionCurrentPosition = currentPosition;
            ActionScaleDelta = scaleDelta;
            ActionTapStatus = TapStatus.PinchOcurring;
            ProcessGestures(GestureType.Pinch);
        }

        void ProcessManipulationEndForDragOcurring(Windows.Foundation.Point currentPosition)
        {
            ActionCurrentPosition = currentPosition;
            ActionTapStatus = TapStatus.DragOcurred;
            ProcessGestures(GestureType.DragComplete);
        }

        void ProcessManipulationEndForHoldOccured()
        {
            ActionTapStatus = TapStatus.Pending;
        }

        void ProcessManipulationEndForHoldPending()
        {
            ActionTapStatus = TapStatus.DoubleTapPending;
        }

        void ProcessManipulationEndForPinchOccurring(Windows.Foundation.Point currentPosition)
        {
            ActionCurrentPosition = currentPosition;
            ActionTapStatus = TapStatus.PinchOcurred;
            ProcessGestures(GestureType.PinchComplete);
        }

        void ProcessManipulationForPinchStart(Windows.Foundation.Point currentPosition)
        {
            ActionCurrentPosition = currentPosition;
            ActionTapStatus = TapStatus.PinchOcurringFirstTime;
            ProcessGestures(GestureType.Pinch);
        }

        void ProcessManipulationStartForDoubleTapPending()
        {
            ActionTapStatus = TapStatus.DoubleTapOcurred;
            ProcessGestures(GestureType.DoubleTap);
        }

        void ProcessManipulationStartForPending(Windows.Foundation.Point startingPosition)
        {
            ActionStartingPosition = startingPosition;
            ActionTapStatus = TapStatus.HoldPending;
        }

        /// <summary>
        /// Releases any initialized resources for this action source.
        /// </summary>
        public void Release()
        {
            OnRelease();
        }

        /// <summary>
        /// Gets or sets the current position for this action.
        /// </summary>
        /// <value>The action current position.</value>
        internal Windows.Foundation.Point ActionCurrentPosition
        {
            get { return  _actionCurrentPosition; }
            set
            {
                _actionPreviousSamplePosition = _actionCurrentPosition;
                _actionCurrentPosition = value;
            }
        }

        DragOrientation ActionGestureOrientation { get; set; }

        Windows.Foundation.Point ActionPreviousSamplePosition
        {
            get { return  _actionPreviousSamplePosition; }
            set { _actionPreviousSamplePosition = value; }
        }

        /// <summary>
        /// Gets the action sample offset from the previous sample's position.
        /// </summary>
        /// <value>The action sample delta.</value>
        Windows.Foundation.Point ActionSampleDelta
        {
            get { return  ActionCurrentPosition.Delta(_actionPreviousSamplePosition); }
        }

        Windows.Foundation.Point ActionScaleDelta { get; set; }

        /// <summary>
        /// Gets or sets the starting position for this action.
        /// </summary>
        /// <value>The action starting position.</value>
        Windows.Foundation.Point ActionStartingPosition
        {
            get { return  _actionStartingPosition; }
            set
            {
                _actionStartingPosition = value;
                _actionPreviousSamplePosition = value;
                _actionCurrentPosition = value;
            }
        }

        /// <summary>
        /// Gets the tap status for this action
        /// </summary>
        /// <value>The action tap status.</value>
        TapStatus ActionTapStatus
        {
            get { return  _actionTapStatus; }
            set { _actionTapStatus = value; }
        }

        protected virtual bool HasBeenInitialized
        {
            get { return  true; }
        }

        /// <summary>
        /// Gets a value that indicates whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get { return  HasBeenInitialized; }
        }

        protected abstract double MoveThreshold { get; }

        internal SpreadView View
        {
            get
            {
                if (_gestureElement is SpreadView)
                {
                    return (_gestureElement as SpreadView);
                }
                return null;
            }
        }

        enum TapStatus
        {
            Pending,
            HoldPending,
            HoldOcurred,
            DragOcurringFirstTime,
            DragOcurring,
            DragOcurred,
            PinchOcurringFirstTime,
            PinchOcurring,
            PinchOcurred,
            DoubleTapPending,
            DoubleTapOcurred,
            Tap
        }
    }
}

