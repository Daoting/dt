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
        internal const string GCCELL_DisplayTextBlock = "DisplayTextBlock";
        internal const string GCCELL_HighlightRectangle = "HighlightRectangle";
        internal const string GCCELL_HoverRectangle = "HoverRectangle";
        internal const string GCCELL_NormalRectangle = "NormalRectangle";
        internal const string GCCELL_SelectionRectangle = "SelectionRectangle";

        protected HeaderCellPresenter()
        {
            base.DefaultStyleKey = typeof(HeaderCellPresenter);
        }

        internal TextBlock DisplayTextBlock
        {
            get { return _displayTextBlock; }
        }

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
            Action action = null;
            if (HighlightRectangle != null)
            {
                if (action == null)
                {
                    action = delegate
                    {
                        if (IsHightlighted)
                        {
                            TextBlock content = base.Content as TextBlock;
                            if ((content != null) && (DisplayTextBlock != null))
                            {
                                CopyTextBlock(DisplayTextBlock, content);
                                content.Opacity = 0.0;
                                DisplayTextBlock.Opacity = 1.0;
                            }
                            HighlightRectangle.Opacity = 1.0;

                        }
                        else
                        {
                            TextBlock block2 = base.Content as TextBlock;
                            if ((block2 != null) && (DisplayTextBlock != null))
                            {
                                block2.Opacity = 1.0;
                                DisplayTextBlock.Opacity = 0.0;
                            }
                            HighlightRectangle.Opacity = 0.0;
                        }
                    };
                }
                UIAdaptor.InvokeAsync(action);
            }
        }

        internal virtual void ApplyHoverState()
        {
            if (HoverRectangle != null)
            {
                UIAdaptor.InvokeAsync(delegate
                {
                    if (IsMouseOver && (base.OwningRow.OwningPresenter.Sheet.InputDeviceType != InputDeviceType.Touch))
                    {
                        HoverRectangle.Opacity = 1.0;
                    }
                    else
                    {
                        HoverRectangle.Opacity = 0.0;
                    }
                });
            }
        }

        internal virtual void ApplySelectionState()
        {
            if (SelectionRectangle != null)
            {
                UIAdaptor.InvokeAsync(delegate
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
                });

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
            Action action = null;
            Action action2 = null;
            if (base.ShowContent)
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
                if (action == null)
                {
                    action = delegate
                    {
                        b1.Foreground = new SolidColorBrush(Colors.Black);
                    };
                }
                UIAdaptor.InvokeSync(action);
            }
            else if (Application.Current.RequestedTheme == (ApplicationTheme)0)
            {
                if (action2 == null)
                {
                    action2 = delegate
                    {
                        b1.Foreground = new SolidColorBrush(Colors.White);
                    };
                }
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
            _hoverRectangle = base.GetTemplateChild("HoverRectangle") as Rectangle;
            _selectionRectangle = base.GetTemplateChild("SelectionRectangle") as Rectangle;
            _normalRectangle = base.GetTemplateChild("NormalRectangle") as Rectangle;
            _highlightRectangle = base.GetTemplateChild("HighlightRectangle") as Rectangle;
            _displayTextBlock = base.GetTemplateChild("DisplayTextBlock") as TextBlock;
            base.OnApplyTemplate();
        }

        internal override bool TryUpdateVisualTree()
        {
            return false;
        }
        //hdt
        //del
    }
}
