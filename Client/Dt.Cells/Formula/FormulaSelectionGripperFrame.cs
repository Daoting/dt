#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class FormulaSelectionGripperFrame : Panel
    {
        Ellipse _bottomRightGripper;
        Visibility _bottomRightVisibility;
        bool _canChangeBoundsByUI = true;
        FormulaSelectionItem _selectionItem;
        Ellipse _topLeftGripper;
        Visibility _topLeftVisibility;

        public FormulaSelectionGripperFrame(FormulaSelectionItem selectionItem)
        {
            _selectionItem = selectionItem;
            _selectionItem.PropertyChanged += new PropertyChangedEventHandler(SelectionItemPropertyChanged);
            _canChangeBoundsByUI = selectionItem.CanChangeBoundsByUI;
            CreateTouchGrippers(selectionItem);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _topLeftGripper.Arrange(new Rect(-_topLeftGripper.Width / 2.0, -_topLeftGripper.Height / 2.0, _topLeftGripper.Width, _topLeftGripper.Height));
            _bottomRightGripper.Arrange(new Rect(finalSize.Width - (_topLeftGripper.Width / 2.0), finalSize.Height - (_topLeftGripper.Height / 2.0), _topLeftGripper.Width, _topLeftGripper.Height));
            return base.ArrangeOverride(finalSize);
        }

        void CreateTouchGrippers(FormulaSelectionItem selectionItem)
        {
            SolidColorBrush brush = new SolidColorBrush(selectionItem.Color);
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 16.0;
            ellipse.Height = 16.0;
            ellipse.Fill = new SolidColorBrush(Colors.White);
            ellipse.StrokeThickness = 2.0;
            ellipse.Stroke = brush;
            _topLeftGripper = ellipse;
            Ellipse ellipse2 = new Ellipse();
            ellipse2.Width = 16.0;
            ellipse2.Height = 16.0;
            ellipse2.Fill = new SolidColorBrush(Colors.White);
            ellipse2.StrokeThickness = 2.0;
            ellipse2.Stroke = brush;
            _bottomRightGripper = ellipse2;
            UpdateGripperVisibility();
            base.Children.Add(_topLeftGripper);
            base.Children.Add(_bottomRightGripper);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (FrameworkElement element in base.Children)
            {
                element.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        void SelectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Range")
            {
                UIElement parent = VisualTreeHelper.GetParent(this) as UIElement;
                if (parent != null)
                {
                    parent.InvalidateMeasure();
                }
            }
            else if (e.PropertyName == "Color")
            {
                SolidColorBrush brush = new SolidColorBrush(_selectionItem.Color);
                _topLeftGripper.Stroke = brush;
                _bottomRightGripper.Stroke = brush;
            }
            else if (e.PropertyName == "CanChangeBoundsByUI")
            {
                CanChangeBoundsByUI = _selectionItem.CanChangeBoundsByUI;
            }
            else if (e.PropertyName == "IsResizing")
            {
                if (_selectionItem.IsResizing)
                {
                    _topLeftGripper.Visibility = Visibility.Collapsed;
                    _bottomRightGripper.Visibility = Visibility.Collapsed;
                }
                else
                {
                    UpdateGripperVisibility();
                }
            }
        }

        void UpdateGripperVisibility()
        {
            if (Excel.FormulaSelectionFeature.IsTouching && _canChangeBoundsByUI)
            {
                _topLeftGripper.Visibility = _topLeftVisibility;
                _bottomRightGripper.Visibility = _bottomRightVisibility;
            }
            else
            {
                _topLeftGripper.Visibility = Visibility.Collapsed;
                _bottomRightGripper.Visibility = Visibility.Collapsed;
            }
        }

        public Visibility BottomRightVisibility
        {
            get { return _bottomRightVisibility; }
            set
            {
                if (_bottomRightVisibility != value)
                {
                    _bottomRightVisibility = value;
                    UpdateGripperVisibility();
                }
            }
        }

        public bool CanChangeBoundsByUI
        {
            get { return _canChangeBoundsByUI; }
            set
            {
                if (_canChangeBoundsByUI != value)
                {
                    _canChangeBoundsByUI = value;
                    UpdateGripperVisibility();
                }
            }
        }

        public FormulaSelectionItem SelectionItem
        {
            get { return _selectionItem; }
        }

        public Visibility TopLeftVisibility
        {
            get { return _topLeftVisibility; }
            set
            {
                if (_topLeftVisibility != value)
                {
                    _topLeftVisibility = value;
                    UpdateGripperVisibility();
                }
            }
        }
    }
}

