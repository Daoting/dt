#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class CornerPanel : Control
    {
        SheetView _view;
        Rectangle _hoverRectangle;
        Rectangle _selectionRectangle;

        public CornerPanel(SheetView p_view)
        {
            DefaultStyleKey = typeof(CornerPanel);
            _view = p_view;
        }

        internal void ApplyState()
        {
            if (_selectionRectangle != null)
            {
                if (_view.HideSelectionWhenPrinting || _view.HasSelectedFloatingObject())
                {
                    _selectionRectangle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Worksheet worksheet = _view.ActiveSheet;
                    if (worksheet.Selections.Count != 1)
                    {
                        _selectionRectangle.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        CellRange range = worksheet.Selections[0];
                        _selectionRectangle.Visibility = (range.Column == -1 && range.Row == -1 && range.RowCount == -1 && range.ColumnCount == -1) ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }

            if (_hoverRectangle != null)
                _hoverRectangle.Visibility = _view.HoverManager.IsMouseOverCornerHeaders ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _hoverRectangle = GetTemplateChild("HoverRectangle") as Rectangle;
            _selectionRectangle = GetTemplateChild("SelectionRectangle") as Rectangle;
        }
    }
}

