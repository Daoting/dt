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
        private Windows.Foundation.Point _actionCurrentPosition;
        private Windows.Foundation.Point _actionPreviousSamplePosition;
        private Windows.Foundation.Point _actionStartingPosition;
        private TapStatus _actionTapStatus;
        private readonly DispatcherTimer _actionTimer = new DispatcherTimer();
        private readonly UIElement _gestureElement;
        private static readonly TimeSpan DoubleTapTimeout = TimeSpan.FromMilliseconds(250.0);
        private static readonly TimeSpan TapAndHoldTimeout = TimeSpan.FromMilliseconds(800.0);

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
            this._gestureElement = gestureElement;
            DispatcherTimer timer = this._actionTimer;
            timer.Tick += OnActionTimerTick;
            this.ActionGestureOrientation = DragOrientation.None;
        }

        /// <summary>
        /// Notifies the machine that manipulation has been completed.
        /// </summary>
        /// <param name="currentPosition">The current position.</param>
        public void HandleManipulationCompleted(Windows.Foundation.Point currentPosition)
        {
            this._actionTimer.Stop();
            switch (this.ActionTapStatus)
            {
                case TapStatus.HoldPending:
                    this.ProcessManipulationEndForHoldPending();
                    this._actionTimer.Interval = DoubleTapTimeout;
                    this._actionTimer.Start();
                    return;

                case TapStatus.HoldOcurred:
                    this.ProcessManipulationEndForHoldOccured();
                    this._actionTimer.Interval = DoubleTapTimeout;
                    this._actionTimer.Start();
                    return;

                case TapStatus.DragOcurringFirstTime:
                case TapStatus.DragOcurring:
                    this.ProcessManipulationEndForDragOcurring(currentPosition);
                    this._actionTimer.Stop();
                    this.ActionCompleted(this, EventArgs.Empty);
                    return;

                case TapStatus.PinchOcurringFirstTime:
                case TapStatus.PinchOcurring:
                    this.ProcessManipulationEndForPinchOccurring(currentPosition);
                    this._actionTimer.Stop();
                    this.ActionCompleted(this, EventArgs.Empty);
                    return;
            }
            this._actionTimer.Stop();
            this.ActionCompleted(this, EventArgs.Empty);
        }

        public void HandleMultipleManipulationDelta(Windows.Foundation.Point currentPosition, Windows.Foundation.Point scaleDelta)
        {
            switch (this.ActionTapStatus)
            {
                case TapStatus.PinchOcurringFirstTime:
                case TapStatus.PinchOcurring:
                    this.ProcessManipulationDeltaForPinchOccurring(currentPosition, scaleDelta);
                    return;

                case TapStatus.Pending:
                    this.ProcessManipulationForPinchStart(currentPosition);
                    return;
            }
            this._actionTimer.Stop();
            this.ActionCompleted(this, EventArgs.Empty);
            this.ActionTapStatus = TapStatus.Pending;
            this.HandleMultipleManipulationDelta(currentPosition, scaleDelta);
        }

        /// <summary>
        /// Notifies the machine that a manipulation delta has occurred.
        /// </summary>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="offsetFromOrigin">The offset from origin.</param>
        public void HandleSingleManipulationDelta(Windows.Foundation.Point currentPosition, Windows.Foundation.Point offsetFromOrigin)
        {
            if (this.IsMoveAboveThreshold(offsetFromOrigin))
            {
                this._actionTimer.Stop();
                switch (this.ActionTapStatus)
                {
                    case TapStatus.HoldPending:
                        this.ProcessManipulationDeltaForHoldPending(currentPosition);
                        return;

                    case TapStatus.HoldOcurred:
                        this.ProcessManipulationDeltaForHoldPending(currentPosition);
                        return;

                    case TapStatus.DragOcurringFirstTime:
                    case TapStatus.DragOcurring:
                        this.ProcessManipulationDeltaForDragOcurring(currentPosition);
                        return;

                    case TapStatus.PinchOcurringFirstTime:
                    case TapStatus.PinchOcurring:
                        this.ProcessManipulationEndForPinchOccurring(currentPosition);
                        return;
                }
                this._actionTimer.Stop();
                this.ActionCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Notifies the machine that manipulation has started.
        /// </summary>
        /// <param name="currentPosition">The current position.</param>
        public void HandleSingleManipulationStarted(Windows.Foundation.Point currentPosition)
        {
            this._actionTimer.Stop();
            TapStatus actionTapStatus = this.ActionTapStatus;
            if (actionTapStatus == TapStatus.Pending)
            {
                this.ProcessManipulationStartForPending(currentPosition);
                this._actionTimer.Interval = TapAndHoldTimeout;
                this._actionTimer.Start();
            }
            else if (actionTapStatus == TapStatus.DoubleTapPending)
            {
                this.ProcessManipulationStartForDoubleTapPending();
                this.ActionCompleted(this, EventArgs.Empty);
            }
            else
            {
                this.ActionCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Initializes the resources for this action source.
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            return this.OnInitialize();
        }

        private bool IsMoveAboveThreshold(Windows.Foundation.Point offsetFromOrigin)
        {
            return (offsetFromOrigin.Offset() > this.MoveThreshold);
        }

        private void OnActionTimerTick(object sender, object e)
        {
            this._actionTimer.Stop();
            if (this.ActionTapStatus == TapStatus.HoldPending)
            {
                this.ProcessGestureTimerForHoldPending();
            }
            else if (this.ActionTapStatus == TapStatus.DoubleTapPending)
            {
                this.ProcessGestureTimerForDoubleTapPending();
                this.ActionCompleted(this, EventArgs.Empty);
            }
        }

        protected virtual bool OnInitialize()
        {
            return true;
        }

        protected virtual void OnRelease()
        {
        }

        private void ProcessGestures(GestureType gestureType)
        {
            if (((this.ActionGestureOrientation == DragOrientation.None) && ((this.ActionSampleDelta.X != 0.0) || (this.ActionSampleDelta.Y != 0.0))) && ((this.ActionGestureOrientation == DragOrientation.None) && ((this.ActionSampleDelta.X != 0.0) || (this.ActionSampleDelta.Y != 0.0))))
            {
                double num = Math.Atan(Math.Abs(this.ActionSampleDelta.Y) / Math.Abs(this.ActionSampleDelta.X)) * 57.295779513082323;
                if (num > 55.0)
                {
                    this.ActionGestureOrientation = DragOrientation.Vertical;
                }
                else if (num > 35.0)
                {
                    this.ActionGestureOrientation = DragOrientation.Horizontal | DragOrientation.Vertical;
                }
                else
                {
                    this.ActionGestureOrientation = DragOrientation.Horizontal;
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
                    if (this.View == null)
                    {
                        break;
                    }
                    this.View.OnTouchTap(this.ActionCurrentPosition);
                    return;

                case GestureType.DoubleTap:
                    if (this.View == null)
                    {
                        break;
                    }
                    this.View.OnTouchTap(this.ActionCurrentPosition);
                    this.View.OnTouchDoubleTap(this.ActionCurrentPosition);
                    return;

                case GestureType.Hold:
                    this.View.ProcessTouchHold(this.ActionCurrentPosition);
                    return;

                case GestureType.FreeDrag:
                    if (this.View != null)
                    {
                        this.View.ProcessTouchFreeDrag(this.ActionStartingPosition, this.ActionCurrentPosition, this.ActionSampleDelta, this.ActionGestureOrientation);
                        this._actionPreviousSamplePosition = this.ActionCurrentPosition;
                    }
                    break;

                default:
                    return;
            }
        }

        private void ProcessGestureTimerForDoubleTapPending()
        {
            this.ActionTapStatus = TapStatus.Tap;
            this.ProcessGestures(GestureType.Tap);
        }

        private void ProcessGestureTimerForHoldPending()
        {
            this.ActionTapStatus = TapStatus.HoldOcurred;
            this.ProcessGestures(GestureType.Hold);
        }

        private void ProcessManipulationDeltaForDragOcurring(Windows.Foundation.Point currentPosition)
        {
            this.ActionCurrentPosition = currentPosition;
            this.ActionTapStatus = TapStatus.DragOcurring;
            this.ProcessGestures(GestureType.FreeDrag);
        }

        private void ProcessManipulationDeltaForHoldPending(Windows.Foundation.Point currentPosition)
        {
            this.ActionCurrentPosition = currentPosition;
            this.ActionTapStatus = TapStatus.DragOcurringFirstTime;
            this.ProcessGestures(GestureType.FreeDrag);
        }

        private void ProcessManipulationDeltaForPinchOccurring(Windows.Foundation.Point currentPosition, Windows.Foundation.Point scaleDelta)
        {
            this.ActionCurrentPosition = currentPosition;
            this.ActionScaleDelta = scaleDelta;
            this.ActionTapStatus = TapStatus.PinchOcurring;
            this.ProcessGestures(GestureType.Pinch);
        }

        private void ProcessManipulationEndForDragOcurring(Windows.Foundation.Point currentPosition)
        {
            this.ActionCurrentPosition = currentPosition;
            this.ActionTapStatus = TapStatus.DragOcurred;
            this.ProcessGestures(GestureType.DragComplete);
        }

        private void ProcessManipulationEndForHoldOccured()
        {
            this.ActionTapStatus = TapStatus.Pending;
        }

        private void ProcessManipulationEndForHoldPending()
        {
            this.ActionTapStatus = TapStatus.DoubleTapPending;
        }

        private void ProcessManipulationEndForPinchOccurring(Windows.Foundation.Point currentPosition)
        {
            this.ActionCurrentPosition = currentPosition;
            this.ActionTapStatus = TapStatus.PinchOcurred;
            this.ProcessGestures(GestureType.PinchComplete);
        }

        private void ProcessManipulationForPinchStart(Windows.Foundation.Point currentPosition)
        {
            this.ActionCurrentPosition = currentPosition;
            this.ActionTapStatus = TapStatus.PinchOcurringFirstTime;
            this.ProcessGestures(GestureType.Pinch);
        }

        private void ProcessManipulationStartForDoubleTapPending()
        {
            this.ActionTapStatus = TapStatus.DoubleTapOcurred;
            this.ProcessGestures(GestureType.DoubleTap);
        }

        private void ProcessManipulationStartForPending(Windows.Foundation.Point startingPosition)
        {
            this.ActionStartingPosition = startingPosition;
            this.ActionTapStatus = TapStatus.HoldPending;
        }

        /// <summary>
        /// Releases any initialized resources for this action source.
        /// </summary>
        public void Release()
        {
            this.OnRelease();
        }

        /// <summary>
        /// Gets or sets the current position for this action.
        /// </summary>
        /// <value>The action current position.</value>
        internal Windows.Foundation.Point ActionCurrentPosition
        {
            get { return  this._actionCurrentPosition; }
            private set
            {
                this._actionPreviousSamplePosition = this._actionCurrentPosition;
                this._actionCurrentPosition = value;
            }
        }

        private DragOrientation ActionGestureOrientation { get; set; }

        private Windows.Foundation.Point ActionPreviousSamplePosition
        {
            get { return  this._actionPreviousSamplePosition; }
            set { this._actionPreviousSamplePosition = value; }
        }

        /// <summary>
        /// Gets the action sample offset from the previous sample's position.
        /// </summary>
        /// <value>The action sample delta.</value>
        private Windows.Foundation.Point ActionSampleDelta
        {
            get { return  this.ActionCurrentPosition.Delta(this._actionPreviousSamplePosition); }
        }

        private Windows.Foundation.Point ActionScaleDelta { get; set; }

        /// <summary>
        /// Gets or sets the starting position for this action.
        /// </summary>
        /// <value>The action starting position.</value>
        private Windows.Foundation.Point ActionStartingPosition
        {
            get { return  this._actionStartingPosition; }
            set
            {
                this._actionStartingPosition = value;
                this._actionPreviousSamplePosition = value;
                this._actionCurrentPosition = value;
            }
        }

        /// <summary>
        /// Gets the tap status for this action
        /// </summary>
        /// <value>The action tap status.</value>
        private TapStatus ActionTapStatus
        {
            get { return  this._actionTapStatus; }
            set { this._actionTapStatus = value; }
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
            get { return  this.HasBeenInitialized; }
        }

        protected abstract double MoveThreshold { get; }

        internal SpreadView View
        {
            get
            {
                if (this._gestureElement is SpreadView)
                {
                    return (this._gestureElement as SpreadView);
                }
                return null;
            }
        }

        private enum TapStatus
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

