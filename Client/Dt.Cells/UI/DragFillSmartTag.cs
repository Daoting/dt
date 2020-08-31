#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
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
        Windows.UI.Xaml.Controls.Primitives.Popup _autoFitContentMenuPopup;
        DragFillContextMenu _autoFitListBox;
        bool _closedByThis;
        PopupHelper _dragFillPopupManager;
        bool _isHover;
        bool _isPressed;
        Excel _excel;

        /// <summary>
        /// Occurs when the automatic fill type has changed.
        /// </summary>
        public event EventHandler AutoFilterTypeChanged;

        internal DragFillSmartTag(Excel p_excel)
        {
            _excel = p_excel;
            InitializeContextMenu();
            _dragFillPopupManager = new PopupHelper(_autoFitContentMenuPopup);
            base.DefaultStyleKey = typeof(DragFillSmartTag);
            Loaded += DragFillSmartTag_Loaded;
        }

        /// <summary>
        /// XamlTyp用，hdt
        /// </summary>
        public DragFillSmartTag()
        { }

        void _autoFitContentMenu_Closed(object sender, object e)
        {
            _isPressed = false;
            _isHover = _closedByThis;
            _closedByThis = false;
            UpdateVisualState(true);
        }

        void _autoFitListBox_SelectedAutoFitTypeChanged(object sender, EventArgs e)
        {
            _isPressed = false;
            _dragFillPopupManager.Close();
            if (AutoFilterTypeChanged != null)
            {
                AutoFilterTypeChanged(this, EventArgs.Empty);
            }
        }

        internal void CloseDragFillSmartTagPopup()
        {
            if (_dragFillPopupManager != null)
            {
                _dragFillPopupManager.Close();
            }
        }

        void DragFillSmartTag_Loaded(object sender, RoutedEventArgs e)
        {
            DragFillSmartTag tag = this;
            tag.PointerEntered += OnDragFillSmartTagPointerEntered;
            DragFillSmartTag tag2 = this;
            tag2.PointerExited += OnDragFillSmartTagPointerExited;
            DragFillSmartTag tag3 = this;
            tag3.PointerPressed += OnDragFillSmartTagPointerPressed;
            DragFillSmartTag tag4 = this;
            tag4.PointerReleased += OnDragFillSmartTagPointerReleased;
            Windows.UI.Xaml.Controls.Primitives.Popup popup = _autoFitContentMenuPopup;
            popup.Closed += _autoFitContentMenu_Closed;
            _isPressed = false;
            _isHover = false;
            UpdateVisualState(true);
        }

        internal void DragFillSmartTagTap(Point point)
        {
            double num = 0.0;
            double actualHeight = base.ActualHeight;
            Point point2 = new Point(num - 5.0, actualHeight);
            point2 = base.TransformToVisual(_excel).Transform(point2);
            if (!_isPressed)
            {
                _isPressed = true;
                UpdateVisualState(true);
                _dragFillPopupManager.ShowAsModal(_excel, _autoFitListBox, point2, PopupDirection.BottomRight);
            }
            else
            {
                _closedByThis = true;
                _isPressed = false;
                _dragFillPopupManager.ShowAsModal(_excel, _autoFitListBox, point2, PopupDirection.BottomRight);
            }
        }

        internal DragFillContextMenuItem GetTappedDragFillContextMenu(Point point)
        {
            if (IsContextMenuOpened)
            {
                Rect rect = new Rect(_dragFillPopupManager.Location.X, _dragFillPopupManager.Location.Y, _dragFillPopupManager.Size.Width, _dragFillPopupManager.Size.Height);
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
                List<UIElement> list = Enumerable.ToList<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(TranslatePoint(point, content), content));
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

        void InitializeContextMenu()
        {
            _autoFitContentMenuPopup = new Windows.UI.Xaml.Controls.Primitives.Popup();
            AutoFillType[] items = new AutoFillType[4];
            items[1] = AutoFillType.FillSeries;
            items[2] = AutoFillType.FillFormattingOnly;
            items[3] = AutoFillType.FillWithoutFormatting;
            _autoFitListBox = new DragFillContextMenu(items, AutoFillType.CopyCells);
            _autoFitListBox.SelectedAutoFitTypeChanged += new EventHandler(_autoFitListBox_SelectedAutoFitTypeChanged);
        }

        void OnDragFillSmartTagPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _isHover = true;
            UpdateVisualState(true);
        }

        void OnDragFillSmartTagPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isHover = false;
            UpdateVisualState(true);
        }

        void OnDragFillSmartTagPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ProcessPointerDown();
            e.Handled = true;
        }

        void OnDragFillSmartTagPointerReleased(object sender, PointerRoutedEventArgs e)
        {
        }

        void ProcessPointerDown()
        {
            double num = 0.0;
            double actualHeight = base.ActualHeight;
            Point point = new Point(num - 5.0, actualHeight);
            point = base.TransformToVisual(_excel).Transform(point);
            if (!_isPressed)
            {
                _isPressed = true;
                UpdateVisualState(true);
                _dragFillPopupManager.ShowAsModal(_excel, _autoFitListBox, point, PopupDirection.BottomRight);
            }
            else
            {
                _closedByThis = true;
                _isPressed = false;
                _dragFillPopupManager.ShowAsModal(_excel, _autoFitListBox, point, PopupDirection.BottomRight);
            }
        }

        Point TranslatePoint(Point point, UIElement element)
        {
            return _excel.TransformToVisual(element).Transform(point);
        }

        void UpdateVisualState(bool useTransitions)
        {
            if (_isPressed)
            {
                VisualStateManager.GoToState(this, "MouseLeftButtonPressed", useTransitions);
            }
            else if (_isHover)
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
            get { return  _autoFitListBox.SelectedAutoFitType; }
            set { _autoFitListBox.SelectedAutoFitType = value; }
        }

        internal bool IsContextMenuOpened
        {
            get
            {
                if (_autoFitContentMenuPopup == null)
                {
                    return false;
                }
                return _autoFitContentMenuPopup.IsOpen;
            }
        }
    }
}

