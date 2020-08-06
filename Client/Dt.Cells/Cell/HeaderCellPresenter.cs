using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Dt.Cells.UI
{
    public abstract partial class HeaderCellPresenter : CellPresenterBase
    {
        Rectangle _highlightRectangle;
        Rectangle _hoverRectangle;
        Rectangle _selectionRectangle;

        internal abstract bool IsHightlighted { get; }

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

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _hoverRectangle = GetTemplateChild("HoverRectangle") as Rectangle;
            _selectionRectangle = GetTemplateChild("SelectionRectangle") as Rectangle;
            _highlightRectangle = GetTemplateChild("HighlightRectangle") as Rectangle;
        }

        internal override bool TryUpdateVisualTree()
        {
            return false;
        }
    }
}
