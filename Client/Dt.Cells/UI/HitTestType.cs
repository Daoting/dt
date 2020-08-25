#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Specifies the locations in the component for the HitTest method.
    /// </summary>
    public enum HitTestType
    {
        Empty,
        Corner,
        TabStrip,
        RowHeader,
        ColumnHeader,
        Viewport,
        RowSplitBar,
        ColumnSplitBar,
        RowSplitBox,
        ColumnSplitBox,
        // 已删，怕有数字转类型的造成串位，未移除！
        TabSplitBoxRemove,
        HorizontalScrollBar,
        VerticalScrollBar,
        CornerRangeGroup,
        RowRangeGroup,
        ColumnRangeGroup,
        FloatingObject,
        FormulaSelection
    }
}

