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
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    internal partial class FilterButton : Button
    {
        const string ASCEND_STATE = "Ascend";
        const string CHECK_GROUP = "CheckStates";
        const string CHECKED_STATE = "Checked";
        const string DESCEND_STATE = "Descend";
        const string FILTER_ASCEND_STATE = "FilterAscend";
        const string FILTER_DESCEND_STATE = "FilterDescend";
        const string FILTER_STATE = "Filter";
        const string NO_SORTFILTER_STATE = "NoSortFilter";
        const string SORTFILTER_GROUP = "SortFilterStates";
        const string UNCHECKED_STATE = "Unchecked";

        /// <summary>
        /// Creates a new instance of the control.
        /// </summary>
        public FilterButton(CellItem p_cellView)
        {
            DefaultStyleKey = typeof(FilterButton);
            IsHitTestVisible = false;
            CellView = p_cellView;
        }

        internal void ApplyState()
        {
            if ((CellView != null) && (CellView.Excel != null))
            {
                FilterButtonInfo filterButtonInfo = CellView.FilterButtonInfo;
                if (filterButtonInfo == null)
                {
                    VisualStateManager.GoToState(this, "NoSortFilter", true);
                }
                else
                {
                    SortState sortState = filterButtonInfo.GetSortState();
                    if (filterButtonInfo.IsFiltered())
                    {
                        switch (sortState)
                        {
                            case SortState.None:
                                VisualStateManager.GoToState(this, "Filter", true);
                                break;

                            case SortState.Ascending:
                                VisualStateManager.GoToState(this, "FilterAscend", true);
                                break;

                            case SortState.Descending:
                                VisualStateManager.GoToState(this, "FilterDescend", true);
                                break;
                        }
                    }
                    else
                    {
                        switch (sortState)
                        {
                            case SortState.None:
                                VisualStateManager.GoToState(this, "NoSortFilter", true);
                                break;

                            case SortState.Ascending:
                                VisualStateManager.GoToState(this, "Ascend", true);
                                break;

                            case SortState.Descending:
                                VisualStateManager.GoToState(this, "Descend", true);
                                break;
                        }
                    }
                    bool flag = false;
                    if ((Area == SheetArea.ColumnHeader) && (filterButtonInfo.ColumnViewportIndex == CellView.OwnRow.OwnPanel.ColumnViewportIndex))
                    {
                        flag = true;
                    }
                    else if (((Area == SheetArea.Cells) && (filterButtonInfo.RowViewportIndex == CellView.OwnRow.OwnPanel.RowViewportIndex)) && (filterButtonInfo.ColumnViewportIndex == CellView.OwnRow.OwnPanel.ColumnViewportIndex))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        if (CellView.Excel.IsFilterDropDownOpen)
                        {
                            VisualStateManager.GoToState(this, "Checked", true);
                        }
                        else
                        {
                            VisualStateManager.GoToState(this, "Unchecked", true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call <see cref="M:FrameworkElement.ApplyTemplate" /> when overridden in a derived class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ApplyState();
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            e.Handled = false;
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            e.Handled = false;
        }

        internal SheetArea Area { get; set; }

        internal CellItem CellView { get; }
    }
}

