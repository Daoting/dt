#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class CornerPanel : Control
    {
        SheetView _view;
        Path _path;

        public CornerPanel(SheetView p_view)
        {
            DefaultStyleKey = typeof(CornerPanel);
            _view = p_view;
        }

        internal void ApplyState()
        {
            bool selectAll = false;
            Worksheet worksheet = _view.ActiveSheet;
            if (!_view.HideSelectionWhenPrinting
                && !_view.HasSelectedFloatingObject()
                && worksheet.Selections.Count == 1)
            {
                CellRange range = worksheet.Selections[0];
                selectAll = range.Column == -1 && range.Row == -1 && range.RowCount == -1 && range.ColumnCount == -1;
            }

            if (selectAll)
            {
                _path.Fill = BrushRes.醒目蓝色;
            }
            else if (_view.HoverManager.IsMouseOverCornerHeaders)
            {
                _path.Fill = BrushRes.主题蓝色;
            }
            else
            {
                _path.Fill = BrushRes.浅灰边框;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _path = (Path)GetTemplateChild("FlagPath");
        }
    }
}

