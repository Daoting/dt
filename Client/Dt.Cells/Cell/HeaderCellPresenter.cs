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
        TextBlock _displayTextBlock;
        Rectangle _highlightRectangle;
        Rectangle _hoverRectangle;
        Rectangle _normalRectangle;
        Rectangle _selectionRectangle;

        internal Rectangle HighlightRectangle
        {
            get { return _highlightRectangle; }
        }

        internal Rectangle HoverRectangle
        {
            get { return _hoverRectangle; }
        }

        internal abstract bool IsHightlighted { get; }

        internal Rectangle NormalRectangle
        {
            get { return _normalRectangle; }
        }

        internal Rectangle SelectionRectangle
        {
            get { return _selectionRectangle; }
        }

        internal virtual void ApplyHightlightState()
        {
            if (HighlightRectangle != null)
            {
                if (IsHightlighted)
                {
                    TextBlock content = base.Content as TextBlock;
                    if ((content != null) && (_displayTextBlock != null))
                    {
                        CopyTextBlock(_displayTextBlock, content);
                        content.Opacity = 0.0;
                        _displayTextBlock.Opacity = 1.0;
                    }
                    HighlightRectangle.Opacity = 1.0;

                }
                else
                {
                    TextBlock block2 = base.Content as TextBlock;
                    if ((block2 != null) && (_displayTextBlock != null))
                    {
                        block2.Opacity = 1.0;
                        _displayTextBlock.Opacity = 0.0;
                    }
                    HighlightRectangle.Opacity = 0.0;
                }
            }
        }

        internal virtual void ApplyHoverState()
        {
            if (HoverRectangle != null)
            {
                if (IsMouseOver && (base.OwningRow.OwningPresenter.Sheet.InputDeviceType != InputDeviceType.Touch))
                {
                    HoverRectangle.Opacity = 1.0;
                }
                else
                {
                    HoverRectangle.Opacity = 0.0;
                }
            }
        }

        internal virtual void ApplySelectionState()
        {
            if (SelectionRectangle != null)
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
                    SelectionRectangle.Opacity = 1.0;
                }
                else
                {
                    SelectionRectangle.Opacity = 0.0;
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

        void CopyTextBlock(TextBlock b1, TextBlock b2)
        {
            if (ShowContent)
            {
                b1.Text = b2.Text;
            }
            else
            {
                b1.Text = string.Empty;
            }
            b1.HorizontalAlignment = b2.HorizontalAlignment;
            b1.VerticalAlignment = b2.VerticalAlignment;
            if (Application.Current.RequestedTheme == (ApplicationTheme)1)
            {
                b1.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (Application.Current.RequestedTheme == (ApplicationTheme)0)
            {
                b1.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                b1.Foreground = b2.Foreground;
            }
            b1.FontStyle = b2.FontStyle;
            b1.FontWeight = b2.FontWeight;
            b1.FontStretch = b2.FontStretch;
            b1.FontFamily = b2.FontFamily;
            b1.TextWrapping = b2.TextWrapping;
            b1.FontSize = b2.FontSize;
            b1.Margin = b2.Margin;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _hoverRectangle = GetTemplateChild("HoverRectangle") as Rectangle;
            _selectionRectangle = GetTemplateChild("SelectionRectangle") as Rectangle;
            _normalRectangle = GetTemplateChild("NormalRectangle") as Rectangle;
            _highlightRectangle = GetTemplateChild("HighlightRectangle") as Rectangle;
            _displayTextBlock = GetTemplateChild("DisplayTextBlock") as TextBlock;
        }

        internal override bool TryUpdateVisualTree()
        {
            return false;
        }
    }
}
