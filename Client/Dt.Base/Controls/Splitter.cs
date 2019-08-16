﻿using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dt.Base
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Splitter : Control
    {
        #region Template Part and Visual State names
        const string OrientationStatesGroupName = "OrientationStates";
        const string VerticalOrientationStateName = "VerticalOrientation";
        const string HorizontalOrientationStateName = "HorizontalOrientation";
        #endregion

        #region 成员变量
        const double DefaultKeyboardIncrement = 1d;
        Point lastPosition;
        Point previewDraggingStartPosition;
        bool isDragging;
        bool isDraggingPreview;
        GridResizeDirection effectiveResizeDirection;
        Grid parentGrid;

        Grid previewPopupHostGrid;
        Grid previewGrid;
        Splitter previewGridSplitter;
        uint? dragPointer;

        #endregion

        public event EventHandler DraggingCompleted;

        #region ResizeBehavior
        /// <summary>
        /// ResizeBehavior Dependency Property
        /// </summary>
        public static readonly DependencyProperty ResizeBehaviorProperty =
            DependencyProperty.Register(
                "ResizeBehavior",
                typeof(GridResizeBehavior),
                typeof(Splitter),
                new PropertyMetadata(GridResizeBehavior.BasedOnAlignment));

        /// <summary>
        /// Gets or sets the ResizeBehavior property. This dependency property 
        /// indicates which columns or rows are resized relative
        /// to the column or row for which the GridSplitter control is defined.
        /// </summary>
        public GridResizeBehavior ResizeBehavior
        {
            get { return (GridResizeBehavior)GetValue(ResizeBehaviorProperty); }
            set { SetValue(ResizeBehaviorProperty, value); }
        }
        #endregion

        #region ResizeDirection
        /// <summary>
        /// ResizeDirection Dependency Property
        /// </summary>
        public static readonly DependencyProperty ResizeDirectionProperty =
            DependencyProperty.Register(
                "ResizeDirection",
                typeof(GridResizeDirection),
                typeof(Splitter),
                new PropertyMetadata(GridResizeDirection.Auto, OnResizeDirectionChanged));

        /// <summary>
        /// Gets or sets the ResizeDirection property. This dependency property 
        /// indicates whether the CustomGridSplitter control resizes rows or columns.
        /// </summary>
        public GridResizeDirection ResizeDirection
        {
            get { return (GridResizeDirection)GetValue(ResizeDirectionProperty); }
            set { SetValue(ResizeDirectionProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ResizeDirection property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        static void OnResizeDirectionChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (Splitter)d;
            GridResizeDirection oldResizeDirection = (GridResizeDirection)e.OldValue;
            GridResizeDirection newResizeDirection = target.ResizeDirection;
            target.OnResizeDirectionChanged(oldResizeDirection, newResizeDirection);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the ResizeDirection property.
        /// </summary>
        /// <param name="oldResizeDirection">The old ResizeDirection value</param>
        /// <param name="newResizeDirection">The new ResizeDirection value</param>
        protected virtual void OnResizeDirectionChanged(
            GridResizeDirection oldResizeDirection, GridResizeDirection newResizeDirection)
        {
            this.DetermineResizeCursor();
            this.UpdateOrientationState();
        }
        #endregion

        #region KeyboardIncrement
        /// <summary>
        /// KeyboardIncrement Dependency Property
        /// </summary>
        public static readonly DependencyProperty KeyboardIncrementProperty =
            DependencyProperty.Register(
                "KeyboardIncrement",
                typeof(double),
                typeof(Splitter),
                new PropertyMetadata(DefaultKeyboardIncrement));

        /// <summary>
        /// Gets or sets the KeyboardIncrement property. This dependency property 
        /// indicates the distance that each press of an arrow key moves
        /// a CustomGridSplitter control.
        /// </summary>
        public double KeyboardIncrement
        {
            get { return (double)GetValue(KeyboardIncrementProperty); }
            set { SetValue(KeyboardIncrementProperty, value); }
        }
        #endregion

        #region ShowsPreview
        /// <summary>
        /// ShowsPreview Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowsPreviewProperty =
            DependencyProperty.Register(
                "ShowsPreview",
                typeof(bool),
                typeof(Splitter),
                new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the ShowsPreview property. This dependency property
        /// indicates whether the preview control should be shown when dragged
        /// instead of directly updating the grid.
        /// </summary>
        public bool ShowsPreview
        {
            get { return (bool)GetValue(ShowsPreviewProperty); }
            set { SetValue(ShowsPreviewProperty, value); }
        }
        #endregion

        #region DetermineEffectiveResizeDirection()
        GridResizeDirection DetermineEffectiveResizeDirection()
        {
            if (ResizeDirection == GridResizeDirection.Columns)
            {
                return GridResizeDirection.Columns;
            }

            if (ResizeDirection == GridResizeDirection.Rows)
            {
                return GridResizeDirection.Rows;
            }

            // Based on GridResizeDirection Enumeration documentation from
            // http://msdn.microsoft.com/en-us/library/WinRTXamlToolkit.Controls.gridresizedirection(v=VS.110).aspx

            // Space is redistributed based on the values of the HorizontalAlignment, VerticalAlignment, ActualWidth, and ActualHeight properties of the CustomGridSplitter.

            // * If the HorizontalAlignment is not set to Stretch, space is redistributed between columns.
            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return GridResizeDirection.Columns;
            }

            // * If the HorizontalAlignment is set to Stretch and the VerticalAlignment is not set to Stretch, space is redistributed between rows.
            if (this.HorizontalAlignment == HorizontalAlignment.Stretch &&
                this.VerticalAlignment != VerticalAlignment.Stretch)
            {
                return GridResizeDirection.Rows;
            }

            // * If the following conditions are true, space is redistributed between columns:
            //   * The HorizontalAlignment is set to Stretch.
            //   * The VerticalAlignment is set to Stretch.
            //   * The ActualWidth is less than or equal to the ActualHeight.
            if (this.HorizontalAlignment == HorizontalAlignment.Stretch &&
                this.VerticalAlignment == VerticalAlignment.Stretch &&
                this.ActualWidth <= this.ActualHeight)
            {
                return GridResizeDirection.Columns;
            }

            // * If the following conditions are true, space is redistributed between rows:
            //   * HorizontalAlignment is set to Stretch.
            //   * VerticalAlignment is set to Stretch.
            //   * ActualWidth is greater than the ActualHeight.
            //if (this.HorizontalAlignment == HorizontalAlignment.Stretch &&
            //    this.VerticalAlignment == VerticalAlignment.Stretch &&
            //    this.ActualWidth > this.ActualHeight)
            {
                return GridResizeDirection.Rows;
            }
        }
        #endregion

        #region DetermineEffectiveResizeBehavior()
        GridResizeBehavior DetermineEffectiveResizeBehavior()
        {
            if (ResizeBehavior == GridResizeBehavior.CurrentAndNext)
            {
                return GridResizeBehavior.CurrentAndNext;
            }

            if (ResizeBehavior == GridResizeBehavior.PreviousAndCurrent)
            {
                return GridResizeBehavior.PreviousAndCurrent;
            }

            if (ResizeBehavior == GridResizeBehavior.PreviousAndNext)
            {
                return GridResizeBehavior.PreviousAndNext;
            }

            // Based on GridResizeBehavior Enumeration documentation from
            // http://msdn.microsoft.com/en-us/library/WinRTXamlToolkit.Controls.gridresizebehavior(v=VS.110).aspx

            // Space is redistributed based on the value of the
            // HorizontalAlignment and VerticalAlignment properties.

            var effectiveResizeDirection =
                DetermineEffectiveResizeDirection();

            // If the value of the ResizeDirection property specifies
            // that space is redistributed between rows,
            // the redistribution follows these guidelines:

            if (effectiveResizeDirection == GridResizeDirection.Rows)
            {
                // * When the VerticalAlignment property is set to Top,
                //   space is redistributed between the row that is specified
                //   for the GridSplitter and the row that is above that row.
                if (this.VerticalAlignment == VerticalAlignment.Top)
                {
                    return GridResizeBehavior.PreviousAndCurrent;
                }

                // * When the VerticalAlignment property is set to Bottom,
                //   space is redistributed between the row that is specified
                //   for the GridSplitter and the row that is below that row.
                if (this.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    return GridResizeBehavior.CurrentAndNext;
                }

                // * When the VerticalAlignment property is set to Center,
                //   space is redistributed between the row that is above and
                //   the row that is below the row that is specified
                //   for the GridSplitter.
                // * When the VerticalAlignment property is set to Stretch,
                //   space is redistributed between the row that is above
                //   and the row that is below the row that is specified
                //   for the GridSplitter.
                return GridResizeBehavior.PreviousAndNext;
            }

            // If the value of the ResizeDirection property specifies
            // that space is redistributed between columns,
            // the redistribution follows these guidelines:

            // * When the HorizontalAlignment property is set to Left,
            //   space is redistributed between the column that is specified
            //   for the GridSplitter and the column that is to the left.
            if (this.HorizontalAlignment == HorizontalAlignment.Left)
            {
                return GridResizeBehavior.PreviousAndCurrent;
            }

            // * When the HorizontalAlignment property is set to Right,
            //   space is redistributed between the column that is specified
            //   for the GridSplitter and the column that is to the right.
            if (this.HorizontalAlignment == HorizontalAlignment.Right)
            {
                return GridResizeBehavior.CurrentAndNext;
            }

            // * When the HorizontalAlignment property is set to Center,
            //   space is redistributed between the columns that are to the left
            //   and right of the column that is specified for the GridSplitter.
            // * When the HorizontalAlignment property is set to Stretch,
            //   space is redistributed between the columns that are to the left
            //   and right of the column that is specified for the GridSplitter.
            return GridResizeBehavior.PreviousAndNext;
        }
        #endregion

        #region DetermineResizeCursor()
        void DetermineResizeCursor()
        {
            var effectiveResizeDirection =
                this.DetermineEffectiveResizeDirection();

            if (effectiveResizeDirection == GridResizeDirection.Columns)
            {
                this.SetCursor(CoreCursorType.SizeWestEast);
            }
            else
            {
                this.SetCursor(CoreCursorType.SizeNorthSouth);
            }
        }
        #endregion

        #region CTOR
        public Splitter()
        {
            this.DefaultStyleKey = typeof(Splitter);
            this.DetermineResizeCursor();
            Loaded += OnSplitterLoaded;
            this.LayoutUpdated += OnLayoutUpdated;
        }

        #endregion

        #region 重写方法
        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.DetermineResizeCursor();
        }

        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (this.dragPointer != null)
                return;

            this.dragPointer = e.Pointer.PointerId;
            this.effectiveResizeDirection = this.DetermineEffectiveResizeDirection();
            this.parentGrid = GetGrid();
            this.previewDraggingStartPosition = e.GetCurrentPoint(this.parentGrid).Position;
            this.lastPosition = this.previewDraggingStartPosition;
            this.isDragging = true;

            if (ShowsPreview)
            {
                StartPreviewDragging(e);
            }
            else
            {
                StartDirectDragging(e);
            }
        }

        void StartPreviewDragging(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
            this.isDraggingPreview = true;
            Windows.UI.Xaml.Controls.Primitives.Popup previewPopup = new Windows.UI.Xaml.Controls.Primitives.Popup();

            this.previewPopupHostGrid = new Grid
            {
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch
            };
            this.parentGrid.Children.Add(this.previewPopupHostGrid);
            if (this.parentGrid.RowDefinitions.Count > 0)
                Grid.SetRowSpan(this.previewPopupHostGrid, this.parentGrid.RowDefinitions.Count);
            if (this.parentGrid.ColumnDefinitions.Count > 0)
                Grid.SetColumnSpan(this.previewPopupHostGrid, this.parentGrid.ColumnDefinitions.Count);
            this.previewPopupHostGrid.Children.Add(previewPopup);

            this.previewGrid = new Grid
            {
                Width = this.parentGrid.ActualWidth,
                Height = this.parentGrid.ActualHeight
            };
            previewPopup.Child = this.previewGrid;

            foreach (var definition in this.parentGrid.RowDefinitions)
            {
                var definitionCopy = new RowDefinition
                {
                    Height = definition.Height,
                    MaxHeight = definition.MaxHeight,
                    MinHeight = definition.MinHeight
                };

                this.previewGrid.RowDefinitions.Add(definitionCopy);
            }

            foreach (var definition in this.parentGrid.ColumnDefinitions)
            {
                var mxw = definition.MaxWidth;
                var mnw = definition.MinWidth;

                var definitionCopy = new ColumnDefinition();

                // hdt Auto情况调整
                if (definition.Width.IsAuto)
                    definitionCopy.Width = new GridLength(definition.ActualWidth);
                else
                    definitionCopy.Width = definition.Width;

                definition.MinWidth = mnw;
                if (!double.IsInfinity(definition.MaxWidth))
                {
                    definition.MaxWidth = mxw;
                }

                this.previewGrid.ColumnDefinitions.Add(definitionCopy);
            }

            this.previewGridSplitter = new Splitter
            {
                ShowsPreview = false,
                Width = this.Width,
                Height = this.Height,
                Margin = this.Margin,
                VerticalAlignment = this.VerticalAlignment,
                HorizontalAlignment = this.HorizontalAlignment,
                ResizeBehavior = this.ResizeBehavior,
                ResizeDirection = this.ResizeDirection,
                KeyboardIncrement = this.KeyboardIncrement
            };

            Grid.SetColumn(this.previewGridSplitter, Grid.GetColumn(this));
            var cs = Grid.GetColumnSpan(this);
            if (cs > 0)
                Grid.SetColumnSpan(this.previewGridSplitter, cs);
            Grid.SetRow(this.previewGridSplitter, Grid.GetRow(this));
            var rs = Grid.GetRowSpan(this);
            if (rs > 0)
                Grid.SetRowSpan(this.previewGridSplitter, rs);
            this.previewGrid.Children.Add(this.previewGridSplitter);
            previewPopup.Child = this.previewGrid;
            previewPopup.IsOpen = true;

            this.previewGridSplitter.dragPointer = e.Pointer.PointerId;
            this.previewGridSplitter.effectiveResizeDirection = this.DetermineEffectiveResizeDirection();
            this.previewGridSplitter.parentGrid = this.previewGrid;
            this.previewGridSplitter.lastPosition = e.GetCurrentPoint(this.previewGrid).Position;
            this.previewGridSplitter.isDragging = true;
            this.previewGridSplitter.StartDirectDragging(e);
            this.previewGridSplitter.DraggingCompleted += previewGridSplitter_DraggingCompleted;
        }

        void previewGridSplitter_DraggingCompleted(object sender, EventArgs e)
        {
            for (int i = 0; i < this.previewGrid.RowDefinitions.Count; i++)
            {
                this.parentGrid.RowDefinitions[i].Height =
                    this.previewGrid.RowDefinitions[i].Height;
            }

            for (int i = 0; i < this.previewGrid.ColumnDefinitions.Count; i++)
            {
                this.parentGrid.ColumnDefinitions[i].Width =
                    this.previewGrid.ColumnDefinitions[i].Width;
            }

            this.previewGridSplitter.DraggingCompleted -= previewGridSplitter_DraggingCompleted;
            this.parentGrid.Children.Remove(previewPopupHostGrid);

            this.isDragging = false;
            this.isDraggingPreview = false;
            this.dragPointer = null;
            this.parentGrid = null;
            this.DraggingCompleted?.Invoke(this, EventArgs.Empty);
        }

        void StartDirectDragging(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.isDraggingPreview = false;
            this.CapturePointer(e.Pointer);
            this.Focus(FocusState.Pointer);
            
            // hdt
            e.Handled = true;
        }

        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!this.isDragging ||
                this.dragPointer != e.Pointer.PointerId)
            {
                return;
            }

            var position = e.GetCurrentPoint(this.parentGrid).Position;

            if (!isDraggingPreview)
                ContinueDirectDragging(position);

            this.lastPosition = position;
        }

        void ContinueDirectDragging(Point position)
        {
            if (this.effectiveResizeDirection == GridResizeDirection.Columns)
            {
                var deltaX = position.X - this.lastPosition.X;
                this.ResizeColumns(this.parentGrid, deltaX);
            }
            else
            {
                var deltaY = position.Y - this.lastPosition.Y;
                this.ResizeRows(this.parentGrid, deltaY);
            }
        }

        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!this.isDragging ||
                this.dragPointer != e.Pointer.PointerId)
            {
                return;
            }

            VisualStateManager.GoToState(this, "Normal", true);
            this.ReleasePointerCapture(e.Pointer);
            this.isDragging = false;
            this.isDraggingPreview = false;
            this.dragPointer = null;
            this.parentGrid = null;
            this.DraggingCompleted?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!this.isDragging)
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            Splitter splitter = e.OriginalSource as Splitter;
            // 避免非拖拽时获得焦点改变样式
            if (splitter == null || !splitter.isDragging)
                return;

            if (this.effectiveResizeDirection == GridResizeDirection.Columns)
            {
                VisualStateManager.GoToState(this, "VerticalFocused", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "HorizontalFocused", true);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        protected override void OnKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            GridResizeDirection effectiveResizeDirection =
                this.DetermineEffectiveResizeDirection();

            if (effectiveResizeDirection == GridResizeDirection.Columns)
            {
                if (e.Key == VirtualKey.Left)
                {
                    this.ResizeColumns(this.GetGrid(), -KeyboardIncrement);
                    e.Handled = true;
                }
                else if (e.Key == VirtualKey.Right)
                {
                    this.ResizeColumns(this.GetGrid(), KeyboardIncrement);
                    e.Handled = true;
                }
            }
            else
            {
                if (e.Key == VirtualKey.Up)
                {
                    this.ResizeRows(this.GetGrid(), -KeyboardIncrement);
                    e.Handled = true;
                }
                else if (e.Key == VirtualKey.Down)
                {
                    this.ResizeRows(this.GetGrid(), KeyboardIncrement);
                    e.Handled = true;
                }
            }
        }

        #endregion

        #region ResizeColumns()
        void ResizeColumns(Grid grid, double deltaX)
        {
            GridResizeBehavior effectiveGridResizeBehavior =
                this.DetermineEffectiveResizeBehavior();

            int column = Grid.GetColumn(this);
            int leftColumn;
            int rightColumn;

            switch (effectiveGridResizeBehavior)
            {
                case GridResizeBehavior.PreviousAndCurrent:
                    leftColumn = column - 1;
                    rightColumn = column;
                    break;
                case GridResizeBehavior.PreviousAndNext:
                    leftColumn = column - 1;
                    rightColumn = column + 1;
                    break;
                default:
                    leftColumn = column;
                    rightColumn = column + 1;
                    break;
            }

            if (rightColumn >= grid.ColumnDefinitions.Count)
            {
                return;
            }

            var leftColumnDefinition = grid.ColumnDefinitions[leftColumn];
            var rightColumnDefinition = grid.ColumnDefinitions[rightColumn];
            var leftColumnGridUnitType = leftColumnDefinition.Width.GridUnitType;
            var rightColumnGridUnitType = rightColumnDefinition.Width.GridUnitType;
            var leftColumnActualWidth = leftColumnDefinition.ActualWidth;
            var rightColumnActualWidth = rightColumnDefinition.ActualWidth;
            var leftColumnMaxWidth = leftColumnDefinition.MaxWidth;
            var rightColumnMaxWidth = rightColumnDefinition.MaxWidth;
            var leftColumnMinWidth = leftColumnDefinition.MinWidth;
            var rightColumnMinWidth = rightColumnDefinition.MinWidth;

            //deltaX = 200;
            if (leftColumnActualWidth + deltaX > leftColumnMaxWidth)
            {
                deltaX = Math.Max(
                    0,
                    leftColumnDefinition.MaxWidth - leftColumnActualWidth);
            }

            if (leftColumnActualWidth + deltaX < leftColumnMinWidth)
            {
                deltaX = Math.Min(
                    0,
                    leftColumnDefinition.MinWidth - leftColumnActualWidth);
            }

            if (rightColumnActualWidth - deltaX > rightColumnMaxWidth)
            {
                deltaX = -Math.Max(
                    0,
                    rightColumnDefinition.MaxWidth - rightColumnActualWidth);
            }

            if (rightColumnActualWidth - deltaX < rightColumnMinWidth)
            {
                deltaX = -Math.Min(
                    0,
                    rightColumnDefinition.MinWidth - rightColumnActualWidth);
            }

            var newLeftColumnActualWidth = leftColumnActualWidth + deltaX;
            var newRightColumnActualWidth = rightColumnActualWidth - deltaX;

            double totalStarColumnsWidth = 0;
            double starColumnsAvailableWidth = grid.ActualWidth;

            if (leftColumnGridUnitType ==
                    GridUnitType.Star ||
                rightColumnGridUnitType ==
                    GridUnitType.Star)
            {
                foreach (var columnDefinition in grid.ColumnDefinitions)
                {
                    if (columnDefinition.Width.GridUnitType ==
                        GridUnitType.Star)
                    {
                        totalStarColumnsWidth +=
                            columnDefinition.Width.Value;
                    }
                    else
                    {
                        starColumnsAvailableWidth -=
                            columnDefinition.ActualWidth;
                    }
                }
            }

            if (leftColumnGridUnitType == GridUnitType.Star)
            {
                if (rightColumnGridUnitType == GridUnitType.Star)
                {
                    // If both columns are star columns
                    // - totalStarColumnsWidth won't change and
                    // as much as one of the columns grows
                    // - the other column will shrink by the same value.

                    // If there is no width available to star columns
                    // - we can't resize two of them.
                    if (starColumnsAvailableWidth < 1)
                    {
                        return;
                    }

                    var oldStarWidth = leftColumnDefinition.Width.Value;
                    var newStarWidth = Math.Max(
                        0,
                        totalStarColumnsWidth * newLeftColumnActualWidth /
                            starColumnsAvailableWidth);
                    leftColumnDefinition.Width =
                        new GridLength(newStarWidth, GridUnitType.Star);

                    rightColumnDefinition.Width =
                        new GridLength(
                            Math.Max(
                                0,
                                rightColumnDefinition.Width.Value -
                                    newStarWidth + oldStarWidth),
                            GridUnitType.Star);
                }
                else
                {
                    var newStarColumnsAvailableWidth =
                        starColumnsAvailableWidth +
                        rightColumnActualWidth -
                        newRightColumnActualWidth;

                    if (newStarColumnsAvailableWidth - newLeftColumnActualWidth >= 1)
                    {
                        var newStarWidth = Math.Max(
                            0,
                            (totalStarColumnsWidth -
                             leftColumnDefinition.Width.Value) *
                            newLeftColumnActualWidth /
                            (newStarColumnsAvailableWidth - newLeftColumnActualWidth));

                        leftColumnDefinition.Width =
                            new GridLength(newStarWidth, GridUnitType.Star);
                    }
                }
            }
            else
            {
                leftColumnDefinition.Width =
                    new GridLength(
                        newLeftColumnActualWidth, GridUnitType.Pixel);
            }

            if (rightColumnGridUnitType ==
                GridUnitType.Star)
            {
                if (leftColumnGridUnitType !=
                    GridUnitType.Star)
                {
                    var newStarColumnsAvailableWidth =
                        starColumnsAvailableWidth +
                        leftColumnActualWidth -
                        newLeftColumnActualWidth;

                    if (newStarColumnsAvailableWidth - newRightColumnActualWidth >= 1)
                    {
                        var newStarWidth = Math.Max(
                            0,
                            (totalStarColumnsWidth -
                             rightColumnDefinition.Width.Value) *
                            newRightColumnActualWidth /
                            (newStarColumnsAvailableWidth - newRightColumnActualWidth));
                        rightColumnDefinition.Width =
                            new GridLength(newStarWidth, GridUnitType.Star);
                    }
                }
                // else handled in the left column width calculation block
            }
            else
            {
                rightColumnDefinition.Width =
                    new GridLength(
                        newRightColumnActualWidth, GridUnitType.Pixel);
            }
        }
        #endregion

        #region ResizeRows()
        void ResizeRows(Grid grid, double deltaX)
        {
            GridResizeBehavior effectiveGridResizeBehavior =
                this.DetermineEffectiveResizeBehavior();

            int row = Grid.GetRow(this);
            int upperRow;
            int lowerRow;

            switch (effectiveGridResizeBehavior)
            {
                case GridResizeBehavior.PreviousAndCurrent:
                    upperRow = row - 1;
                    lowerRow = row;
                    break;
                case GridResizeBehavior.PreviousAndNext:
                    upperRow = row - 1;
                    lowerRow = row + 1;
                    break;
                default:
                    upperRow = row;
                    lowerRow = row + 1;
                    break;
            }

            if (lowerRow >= grid.RowDefinitions.Count)
            {
                return;
            }

            var upperRowDefinition = grid.RowDefinitions[upperRow];
            var lowerRowDefinition = grid.RowDefinitions[lowerRow];
            var upperRowGridUnitType = upperRowDefinition.Height.GridUnitType;
            var lowerRowGridUnitType = lowerRowDefinition.Height.GridUnitType;
            var upperRowActualHeight = upperRowDefinition.ActualHeight;
            var lowerRowActualHeight = lowerRowDefinition.ActualHeight;
            var upperRowMaxHeight = upperRowDefinition.MaxHeight;
            var lowerRowMaxHeight = lowerRowDefinition.MaxHeight;
            var upperRowMinHeight = upperRowDefinition.MinHeight;
            var lowerRowMinHeight = lowerRowDefinition.MinHeight;

            //deltaX = 200;
            if (upperRowActualHeight + deltaX > upperRowMaxHeight)
            {
                deltaX = Math.Max(
                    0,
                    upperRowDefinition.MaxHeight - upperRowActualHeight);
            }

            if (upperRowActualHeight + deltaX < upperRowMinHeight)
            {
                deltaX = Math.Min(
                    0,
                    upperRowDefinition.MinHeight - upperRowActualHeight);
            }

            if (lowerRowActualHeight - deltaX > lowerRowMaxHeight)
            {
                deltaX = -Math.Max(
                    0,
                    lowerRowDefinition.MaxHeight - lowerRowActualHeight);
            }

            if (lowerRowActualHeight - deltaX < lowerRowMinHeight)
            {
                deltaX = -Math.Min(
                    0,
                    lowerRowDefinition.MinHeight - lowerRowActualHeight);
            }

            var newUpperRowActualHeight = upperRowActualHeight + deltaX;
            var newLowerRowActualHeight = lowerRowActualHeight - deltaX;

            //grid.BeginInit();

            double totalStarRowsHeight = 0;
            double starRowsAvailableHeight = grid.ActualHeight;

            if (upperRowGridUnitType ==
                    GridUnitType.Star ||
                lowerRowGridUnitType ==
                    GridUnitType.Star)
            {
                foreach (var rowDefinition in grid.RowDefinitions)
                {
                    if (rowDefinition.Height.GridUnitType ==
                        GridUnitType.Star)
                    {
                        totalStarRowsHeight +=
                            rowDefinition.Height.Value;
                    }
                    else
                    {
                        starRowsAvailableHeight -=
                            rowDefinition.ActualHeight;
                    }
                }
            }

            if (upperRowGridUnitType == GridUnitType.Star)
            {
                if (lowerRowGridUnitType == GridUnitType.Star)
                {
                    // If both rows are star rows
                    // - totalStarRowsHeight won't change and
                    // as much as one of the rows grows
                    // - the other row will shrink by the same value.

                    // If there is no width available to star rows
                    // - we can't resize two of them.
                    if (starRowsAvailableHeight < 1)
                    {
                        return;
                    }

                    var oldStarHeight = upperRowDefinition.Height.Value;
                    var newStarHeight = Math.Max(
                        0,
                        totalStarRowsHeight * newUpperRowActualHeight /
                            starRowsAvailableHeight);
                    upperRowDefinition.Height =
                        new GridLength(newStarHeight, GridUnitType.Star);

                    lowerRowDefinition.Height =
                        new GridLength(
                            Math.Max(
                                0,
                                lowerRowDefinition.Height.Value -
                                    newStarHeight + oldStarHeight),
                            GridUnitType.Star);
                }
                else
                {
                    var newStarRowsAvailableHeight =
                        starRowsAvailableHeight +
                        lowerRowActualHeight -
                        newLowerRowActualHeight;

                    if (newStarRowsAvailableHeight - newUpperRowActualHeight >= 1)
                    {
                        var newStarHeight = Math.Max(
                            0,
                            (totalStarRowsHeight -
                             upperRowDefinition.Height.Value) *
                            newUpperRowActualHeight /
                            (newStarRowsAvailableHeight - newUpperRowActualHeight));

                        upperRowDefinition.Height =
                            new GridLength(newStarHeight, GridUnitType.Star);
                    }
                }
            }
            else
            {
                upperRowDefinition.Height =
                    new GridLength(
                        newUpperRowActualHeight, GridUnitType.Pixel);
            }

            if (lowerRowGridUnitType ==
                GridUnitType.Star)
            {
                if (upperRowGridUnitType !=
                    GridUnitType.Star)
                {
                    var newStarRowsAvailableHeight =
                        starRowsAvailableHeight +
                        upperRowActualHeight -
                        newUpperRowActualHeight;

                    if (newStarRowsAvailableHeight - newLowerRowActualHeight >= 1)
                    {
                        var newStarHeight = Math.Max(
                            0,
                            (totalStarRowsHeight -
                             lowerRowDefinition.Height.Value) *
                            newLowerRowActualHeight /
                            (newStarRowsAvailableHeight - newLowerRowActualHeight));
                        lowerRowDefinition.Height =
                            new GridLength(newStarHeight, GridUnitType.Star);
                    }
                }
                // else handled in the upper row width calculation block
            }
            else
            {
                lowerRowDefinition.Height =
                    new GridLength(
                        newLowerRowActualHeight, GridUnitType.Pixel);
            }

            //grid.EndInit();
        }
        #endregion

        #region GetGrid()
        Grid GetGrid()
        {
            var grid = this.Parent as Grid;

            if (grid == null)
            {
                throw new InvalidOperationException(
                    "CustomGridSplitter only works when hosted in a Grid.");
            }
            return grid;
        }
        #endregion

        #region 内部方法
        void UpdateOrientationState()
        {
            var resizeDirection = DetermineEffectiveResizeDirection();

            if (resizeDirection == GridResizeDirection.Columns)
            {
                VisualStateManager.GoToState(this, VerticalOrientationStateName, true);
            }
            else
            {
                VisualStateManager.GoToState(this, HorizontalOrientationStateName, true);
            }
        }

        void OnLayoutUpdated(object sender, object e)
        {
            UpdateOrientationState();
        }

        /// <summary>
        /// 确保在设计时显示正常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSplitterLoaded(object sender, RoutedEventArgs e)
        {
            ApplyTemplate();
            UpdateOrientationState();
        }
        #endregion
    }

    // Summary:
    //     Specifies the rows or columns that are resized by a WinRTXamlToolkit.Controls.CustomGridSplitter
    //     control.
    public enum GridResizeBehavior
    {
        // Summary:
        //     Space is redistributed based on the value of the Windows.UI.Xaml.FrameworkElement.HorizontalAlignment
        //     and Windows.UI.Xaml.FrameworkElement.VerticalAlignment properties.
        BasedOnAlignment = 0,
        //
        // Summary:
        //     For a horizontal WinRTXamlToolkit.Controls.CustomGridSplitter, space is redistributed
        //     between the row that is specified for the WinRTXamlToolkit.Controls.CustomGridSplitter
        //     and the next row that is below it. For a vertical WinRTXamlToolkit.Controls.CustomGridSplitter,
        //     space is redistributed between the column that is specified for the WinRTXamlToolkit.Controls.CustomGridSplitter
        //     and the next column that is to the right.
        CurrentAndNext = 1,
        //
        // Summary:
        //     For a horizontal WinRTXamlToolkit.Controls.CustomGridSplitter, space is redistributed
        //     between the row that is specified for the WinRTXamlToolkit.Controls.CustomGridSplitter
        //     and the next row that is above it. For a vertical WinRTXamlToolkit.Controls.CustomGridSplitter,
        //     space is redistributed between the column that is specified for the WinRTXamlToolkit.Controls.CustomGridSplitter
        //     and the next column that is to the left.
        PreviousAndCurrent = 2,
        //
        // Summary:
        //     For a horizontal WinRTXamlToolkit.Controls.CustomGridSplitter, space is redistributed
        //     between the rows that are above and below the row that is specified for the
        //     WinRTXamlToolkit.Controls.CustomGridSplitter. For a vertical WinRTXamlToolkit.Controls.CustomGridSplitter,
        //     space is redistributed between the columns that are to the left and right
        //     of the column that is specified for the WinRTXamlToolkit.Controls.CustomGridSplitter.
        PreviousAndNext = 3,
    }

    // Summary:
    //     Specifies whether a WinRTXamlToolkit.Controls.CustomGridSplitter control redistributes
    //     space between rows or between columns.
    public enum GridResizeDirection
    {
        // Summary:
        //     Space is redistributed based on the values of the Windows.UI.Xaml.FrameworkElement.HorizontalAlignment,
        //     Windows.UI.Xaml.FrameworkElement.VerticalAlignment, Windows.UI.Xaml.FrameworkElement.ActualWidth,
        //     and Windows.UI.Xaml.FrameworkElement.ActualHeight properties of the WinRTXamlToolkit.Controls.CustomGridSplitter.
        Auto = 0,
        //
        // Summary:
        //     Space is redistributed between columns.
        Columns = 1,
        //
        // Summary:
        //     Space is redistributed between rows.
        Rows = 2,
    }

}
