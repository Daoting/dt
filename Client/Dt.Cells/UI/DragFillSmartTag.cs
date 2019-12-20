#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a drag fill smart tag control to open a drag fill context menu.
    /// </summary>
    [TemplateVisualState(Name="Normal", GroupName="CommonStates")]
    public partial class DragFillSmartTag : Control
    {
        private Windows.UI.Xaml.Controls.Primitives.Popup _autoFitContentMenuPopup;
        private DragFillContextMenu _autoFitListBox;
        private bool _closedByThis;
        private PopupHelper _dragFillPopupManager;
        private bool _isHover;
        private bool _isPressed;
        private SheetView _ownerSheet;

        /// <summary>
        /// Occurs when the automatic fill type has changed.
        /// </summary>
        public event EventHandler AutoFilterTypeChanged;

        internal DragFillSmartTag(SheetView ownerSheet)
        {
            this._ownerSheet = ownerSheet;
            this.InitializeContextMenu();
            this._dragFillPopupManager = new PopupHelper(this._autoFitContentMenuPopup);
            base.DefaultStyleKey = typeof(DragFillSmartTag);
            Loaded += DragFillSmartTag_Loaded;
        }

        /// <summary>
        /// XamlTyp用，hdt
        /// </summary>
        public DragFillSmartTag()
        { }

        private void _autoFitContentMenu_Closed(object sender, object e)
        {
            this._isPressed = false;
            this._isHover = this._closedByThis;
            this._closedByThis = false;
            this.UpdateVisualState(true);
        }

        private void _autoFitListBox_SelectedAutoFitTypeChanged(object sender, EventArgs e)
        {
            this._isPressed = false;
            this._dragFillPopupManager.Close();
            if (this.AutoFilterTypeChanged != null)
            {
                this.AutoFilterTypeChanged(this, EventArgs.Empty);
            }
        }

        internal void CloseDragFillSmartTagPopup()
        {
            if (this._dragFillPopupManager != null)
            {
                this._dragFillPopupManager.Close();
            }
        }

        private void DragFillSmartTag_Loaded(object sender, RoutedEventArgs e)
        {
            DragFillSmartTag tag = this;
            tag.PointerEntered += OnDragFillSmartTagPointerEntered;
            DragFillSmartTag tag2 = this;
            tag2.PointerExited += OnDragFillSmartTagPointerExited;
            DragFillSmartTag tag3 = this;
            tag3.PointerPressed += OnDragFillSmartTagPointerPressed;
            DragFillSmartTag tag4 = this;
            tag4.PointerReleased += OnDragFillSmartTagPointerReleased;
            Windows.UI.Xaml.Controls.Primitives.Popup popup = this._autoFitContentMenuPopup;
            popup.Closed += _autoFitContentMenu_Closed;
            this._isPressed = false;
            this._isHover = false;
            this.UpdateVisualState(true);
        }

        internal void DragFillSmartTagTap(Windows.Foundation.Point point)
        {
            double num = 0.0;
            double actualHeight = base.ActualHeight;
            Windows.Foundation.Point point2 = new Windows.Foundation.Point(num - 5.0, actualHeight);
            point2 = base.TransformToVisual(this._ownerSheet).Transform(point2);
            if (!this._isPressed)
            {
                this._isPressed = true;
                this.UpdateVisualState(true);
                this._dragFillPopupManager.ShowAsModal(this._ownerSheet, this._autoFitListBox, point2, PopupDirection.BottomRight, true, false);
            }
            else
            {
                this._closedByThis = true;
                this._isPressed = false;
                this._dragFillPopupManager.ShowAsModal(this._ownerSheet, this._autoFitListBox, point2, PopupDirection.BottomRight, true, false);
            }
        }

        internal DragFillContextMenuItem GetTappedDragFillContextMenu(Windows.Foundation.Point point)
        {
            if (this.IsContextMenuOpened)
            {
                Windows.Foundation.Rect rect = new Windows.Foundation.Rect(this._dragFillPopupManager.Location.X, this._dragFillPopupManager.Location.Y, this._dragFillPopupManager.Size.Width, this._dragFillPopupManager.Size.Height);
                if (!rect.Contains(point))
                {
                    return null;
                }
                UIElement content = null;
                content = Windows.UI.Xaml.Window.Current.Content;
                if (content == null)
                {
                    return null;
                }
                List<UIElement> list = Enumerable.ToList<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(this.TranslatePoint(point, content), content));
                if ((list != null) && (list.Count > 0))
                {
                    foreach (UIElement element2 in list)
                    {
                        if (element2 is DragFillContextMenuItem)
                        {
                            return (DragFillContextMenuItem) element2;
                        }
                    }
                }
            }
            return null;
        }

        private void InitializeContextMenu()
        {
            this._autoFitContentMenuPopup = new Windows.UI.Xaml.Controls.Primitives.Popup();
            AutoFillType[] items = new AutoFillType[4];
            items[1] = AutoFillType.FillSeries;
            items[2] = AutoFillType.FillFormattingOnly;
            items[3] = AutoFillType.FillWithoutFormatting;
            this._autoFitListBox = new DragFillContextMenu(items, AutoFillType.CopyCells);
            this._autoFitListBox.SelectedAutoFitTypeChanged += new EventHandler(this._autoFitListBox_SelectedAutoFitTypeChanged);
        }

        private void OnDragFillSmartTagPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this._isHover = true;
            this.UpdateVisualState(true);
        }

        private void OnDragFillSmartTagPointerExited(object sender, PointerRoutedEventArgs e)
        {
            this._isHover = false;
            this.UpdateVisualState(true);
        }

        private void OnDragFillSmartTagPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.ProcessPointerDown();
            e.Handled = true;
        }

        private void OnDragFillSmartTagPointerReleased(object sender, PointerRoutedEventArgs e)
        {
        }

        private void ProcessPointerDown()
        {
            double num = 0.0;
            double actualHeight = base.ActualHeight;
            Windows.Foundation.Point point = new Windows.Foundation.Point(num - 5.0, actualHeight);
            point = base.TransformToVisual(this._ownerSheet).Transform(point);
            if (!this._isPressed)
            {
                this._isPressed = true;
                this.UpdateVisualState(true);
                this._dragFillPopupManager.ShowAsModal(this._ownerSheet, this._autoFitListBox, point, PopupDirection.BottomRight, true, false);
            }
            else
            {
                this._closedByThis = true;
                this._isPressed = false;
                this._dragFillPopupManager.ShowAsModal(this._ownerSheet, this._autoFitListBox, point, PopupDirection.BottomRight, true, false);
            }
        }

        private Windows.Foundation.Point TranslatePoint(Windows.Foundation.Point point, UIElement element)
        {
            return this._ownerSheet.TransformToVisual(element).Transform(point);
        }

        private void UpdateVisualState(bool useTransitions)
        {
            if (this._isPressed)
            {
                VisualStateManager.GoToState(this, "MouseLeftButtonPressed", useTransitions);
            }
            else if (this._isHover)
            {
                VisualStateManager.GoToState(this, "MouseOver", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", useTransitions);
            }
        }

        /// <summary>
        /// Gets or sets the type of the automatic fill after drag fill.
        /// </summary>
        /// <value>
        /// The type of the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.AutoFillType" />.
        /// </value>
        public AutoFillType AutoFilterType
        {
            get { return  this._autoFitListBox.SelectedAutoFitType; }
            set { this._autoFitListBox.SelectedAutoFitType = value; }
        }

        internal bool IsContextMenuOpened
        {
            get
            {
                if (this._autoFitContentMenuPopup == null)
                {
                    return false;
                }
                return this._autoFitContentMenuPopup.IsOpen;
            }
        }
    }
}

