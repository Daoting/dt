using Dt.Cells.Data;
using Windows.UI.Xaml.Shapes;

namespace Dt.Cells.UI
{
    public abstract partial class HeaderCellPresenter : CellPresenterBase
    {
        Rectangle _highlightRectangle;
        Rectangle _hoverRectangle;
        Rectangle _selectionRectangle;

        internal abstract bool IsHightlighted { get; }

        protected override void UpdataContent()
        {
            Cell cell = BindingCell;
            if (cell == null)
                return;

            string text = cell.Text;
        }

        internal virtual void ApplyHightlightState()
        {
            if (_highlightRectangle != null)
                _highlightRectangle.Opacity = IsHightlighted ? 1.0 : 0.0;
        }

        internal virtual void ApplyHoverState()
        {
            if (_hoverRectangle != null)
                _hoverRectangle.Opacity = IsMouseOver ? 1.0 : 0.0;
        }

        internal virtual void ApplySelectionState()
        {
            if (_selectionRectangle != null)
            {
                SheetView sheet = base.OwningRow.OwningPresenter.Sheet;
                bool flag = true;
                if (sheet.HideSelectionWhenPrinting)
                {
                    flag = false;
                }
                if (sheet.HasSelectedFloatingObject())
                {
                    flag = false;
                }
                if ((IsSelected && flag) || IsCurrent)
                {
                    _selectionRectangle.Opacity = 1.0;
                }
                else
                {
                    _selectionRectangle.Opacity = 0.0;
                }
            }

        }

        internal override void ApplyState()
        {
            base.ApplyState();
            ApplyHightlightState();
            ApplySelectionState();
            ApplyHoverState();
        }

        internal override bool TryUpdateVisualTree()
        {
            return false;
        }

        protected override void OnApplyTemplate()
        {
            _hoverRectangle = GetTemplateChild("HoverRectangle") as Rectangle;
            _selectionRectangle = GetTemplateChild("SelectionRectangle") as Rectangle;
            _highlightRectangle = GetTemplateChild("HighlightRectangle") as Rectangle;

            base.OnApplyTemplate();
        }
    }
}
