#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a <see cref="T:Dt.Cells.UI.GcSpreadSheet" /> filter button. 
    /// </summary>
    [TemplateVisualState(GroupName="SortFilterStates", Name="FilterAscend"), TemplateVisualState(GroupName="SortFilterStates", Name="FilterAscend"), TemplateVisualState(GroupName="SortFilterStates", Name="Ascend"), TemplateVisualState(GroupName="SortFilterStates", Name="Filter"), TemplateVisualState(GroupName="SortFilterStates", Name="NoSortFilter"), TemplateVisualState(GroupName="SortFilterStates", Name="Descend")]
    public partial class FilterButton : Button
    {
        private const string ASCEND_STATE = "Ascend";
        private const string CHECK_GROUP = "CheckStates";
        private const string CHECKED_STATE = "Checked";
        private const string DESCEND_STATE = "Descend";
        private const string FILTER_ASCEND_STATE = "FilterAscend";
        private const string FILTER_DESCEND_STATE = "FilterDescend";
        private const string FILTER_STATE = "Filter";
        private const string NO_SORTFILTER_STATE = "NoSortFilter";
        private const string SORTFILTER_GROUP = "SortFilterStates";
        private const string UNCHECKED_STATE = "Unchecked";

        /// <summary>
        /// Creates a new instance of the control.
        /// </summary>
        public FilterButton()
        {
            base.DefaultStyleKey = typeof(FilterButton);
            base.IsHitTestVisible = false;
        }

        internal void ApplyState()
        {
            if ((this.CellView != null) && (this.CellView.SheetView != null))
            {
                FilterButtonInfo filterButtonInfo = this.CellView.FilterButtonInfo;
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
                    if ((this.Area == SheetArea.ColumnHeader) && (filterButtonInfo.ColumnViewportIndex == this.CellView.OwningRow.OwningPresenter.ColumnViewportIndex))
                    {
                        flag = true;
                    }
                    else if (((this.Area == SheetArea.Cells) && (filterButtonInfo.RowViewportIndex == this.CellView.OwningRow.OwningPresenter.RowViewportIndex)) && (filterButtonInfo.ColumnViewportIndex == this.CellView.OwningRow.OwningPresenter.ColumnViewportIndex))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        if (this.CellView.SheetView.IsFilterDropDownOpen)
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
            this.ApplyState();
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

        internal CellPresenterBase CellView { get; set; }
    }
}

