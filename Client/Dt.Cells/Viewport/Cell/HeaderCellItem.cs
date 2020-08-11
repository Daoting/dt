using Dt.Cells.Data;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Dt.Cells.UI
{
    public abstract partial class HeaderCellItem : CellItemBase
    {
        Rectangle _highlightRectangle;
        Rectangle _hoverRectangle;
        Rectangle _selectionRectangle;
        TextBlock _content;

        internal override void ApplyState()
        {
            _highlightRectangle.Visibility = IsHightlighted ? Visibility.Visible : Visibility.Collapsed;

            SheetView sheet = OwningRow.OwningPresenter.Sheet;
            bool allowSelect = !sheet.HideSelectionWhenPrinting && !sheet.HasSelectedFloatingObject();
            if ((IsSelected && allowSelect) || IsCurrent)
            {
                _selectionRectangle.Visibility = Visibility.Visible;
            }
            else
            {
                _selectionRectangle.Visibility = Visibility.Collapsed;
            }

            _hoverRectangle.Visibility = IsMouseOver ? Visibility.Visible : Visibility.Collapsed;
        }

        internal override void CleanUpBeforeDiscard()
        {
        }

        internal override bool TryUpdateVisualTree()
        {
            return false;
        }

        internal abstract bool IsHightlighted { get; }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _hoverRectangle = (Rectangle)GetTemplateChild("HoverRectangle");
            _selectionRectangle = (Rectangle)GetTemplateChild("SelectionRectangle");
            _highlightRectangle = (Rectangle)GetTemplateChild("HighlightRectangle");
            _content = (TextBlock)GetTemplateChild("DisplayTextBlock");
            UpdataContent();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (_isInvalidating)
            {
                UpdataContent();
                _isInvalidating = false;
            }
            return base.MeasureOverride(constraint);
        }

        void UpdataContent()
        {
            Cell cell = BindingCell;
            if (cell == null || _content == null)
                return;

            string text = cell.Text;
            if (string.IsNullOrEmpty(text) || !ShowContent)
            {
                _content.ClearValue(TextBlock.TextProperty);
            }
            else
            {
                _content.Text = text;
                ApplyStyle(_content);
            }
            ApplyState();
        }
    }
}
