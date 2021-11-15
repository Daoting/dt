#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using Dt.Cells.UndoRedo;
using System;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /> tab strip control.
    /// </summary>
    internal partial class TabStrip : Control
    {
        SheetTab _activeTab;
        SheetTab _editingTab;
        SheetTab _newTab;

        public event EventHandler ActiveTabChanged;

        public event EventHandler ActiveTabChanging;

        public event EventHandler NewTabNeeded;

        public TabStrip(Excel p_excel)
        {
            DefaultStyleKey = typeof(TabStrip);
            Excel = p_excel;
            TabsPresenter = new TabsPresenter(this);
            LoadTabs();
        }

        public SheetTab ActiveTab
        {
            get { return _activeTab; }
        }

        public bool IsEditing { get; private set; }

        public Excel Excel { get; }

        public TabsPresenter TabsPresenter { get; }

        public Dt.Cells.Data.Workbook Workbook
        {
            get { return Excel.Workbook; }
        }

        void LoadTabs()
        {
            for (int i = 0; i < Excel.Sheets.Count; i++)
            {
                SheetTab tab = new SheetTab(this);
                tab.SheetIndex = i;
                TabsPresenter.Children.Add(tab);
            }

            if (Excel.TabStripInsertTab)
            {
                _newTab = new SheetTab(this);
                TabsPresenter.Children.Add(_newTab);
            }
            ActiveSheet(Excel.ActiveSheetIndex, false);
        }

        /// <summary>
        /// Tab项个数不同时重新加载，相同时刷新名称
        /// </summary>
        public void UpdateTabs()
        {
            int cnt = Excel.Sheets.Count;
            if (_newTab != null)
                cnt += 1;
            if (cnt != TabsPresenter.Children.Count)
            {
                TabsPresenter.Children.Clear();
                _newTab = null;
                LoadTabs();
            }
            else if(TabsPresenter.Children.Count > 0)
            {
                foreach (SheetTab tab in TabsPresenter.Children)
                {
                    tab.PrepareForDisplay();
                }
            }
        }

        /// <summary>
        /// 刷新标签名称
        /// </summary>
        public void Refresh()
        {
            if (TabsPresenter.Children.Count > 0)
            {
                foreach (SheetTab tab in TabsPresenter.Children)
                {
                    tab.PrepareForDisplay();
                }
            }
        }

        public void NewTab(int sheetIndex)
        {
            StopTabEditing(false);
            SheetTab tab = new SheetTab(this) { SheetIndex = sheetIndex };
            int count = TabsPresenter.Children.Count;
            if (Excel.TabStripInsertTab && (TabsPresenter.Children.Count > 0))
            {
                count = TabsPresenter.Children.Count - 1;
            }
            TabsPresenter.Children.Insert(count, tab);
            TabsPresenter.ReCalculateStartIndex(0, TabsPresenter.Children.Count - 1);
        }

        public void ActiveNextTab()
        {
            if (_activeTab != null)
            {
                int nextVisibleIndex = TabsPresenter.GetNextVisibleIndex(TabsPresenter.Children.IndexOf(_activeTab));
                if (nextVisibleIndex != -1)
                {
                    ProcessNavigation(nextVisibleIndex);
                }
            }
        }

        public void ActivePreviousTab()
        {
            if (_activeTab != null)
            {
                int preVisibleIndex = TabsPresenter.GetPreVisibleIndex(TabsPresenter.Children.IndexOf(_activeTab));
                if (preVisibleIndex != -1)
                {
                    ProcessNavigation(preVisibleIndex);
                }
            }
        }

        public void UpdateInsertNewTab()
        {
            if (Excel.TabStripInsertTab)
            {
                if (_newTab == null)
                {
                    _newTab = new SheetTab(this);
                }
                if (!TabsPresenter.Children.Contains(_newTab))
                {
                    TabsPresenter.Children.Add(_newTab);
                }
                if (IsLoaded)
                {
                    TabsPresenter.ReCalculateStartIndex(0, TabsPresenter.Children.Count - 1);
                }
            }
            else if (_newTab != null)
            {
                TabsPresenter.Children.Remove(_newTab);
                _newTab = null;
            }
        }

        void ActiveSheet(int p_sheetIndex, bool raiseEvent)
        {
            if (TabsPresenter.Children.Count == 0
                || p_sheetIndex < 0
                || p_sheetIndex >= TabsPresenter.Children.Count)
                return;

            SheetTab tab = TabsPresenter.Children[p_sheetIndex] as SheetTab;
            if (tab.SheetIndex == p_sheetIndex && tab != _activeTab)
            {
                if (raiseEvent)
                {
                    CancelEventArgs args = new CancelEventArgs();
                    if (!tab.IsActive)
                    {
                        OnActiveTabChanging((EventArgs)args);
                    }
                    if (!args.Cancel)
                    {
                        if (_activeTab != null)
                            _activeTab.IsActive = false;
                        tab.IsActive = true;
                        _activeTab = tab;
                        OnActiveTabChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    if (_activeTab != null)
                        _activeTab.IsActive = false;
                    tab.IsActive = true;
                    _activeTab = tab;
                }
            }
        }

        public int GetStartIndexToBringTabIntoView(int tabIndex)
        {
            return TabsPresenter.GetStartIndexToBringTabIntoView(tabIndex);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var root = (Grid)GetTemplateChild("Root");
            if (root != null)
                root.Children.Add(TabsPresenter);
        }

        public void ProcessMouseClickSheetTab(PointerRoutedEventArgs args)
        {
            SheetTab hitSheetTab = GetHitSheetTab(args.GetCurrentPoint(null).Position);
            if (hitSheetTab != null)
            {
                TabsPresenter tabsPresenter = TabsPresenter;
                if (hitSheetTab == _newTab)
                {
                    if (!Workbook.Protect)
                    {
                        OnNewTabNeeded(EventArgs.Empty);
                        if (tabsPresenter.Children.Count > 1)
                        {
                            ActiveSheet(((SheetTab)tabsPresenter.Children[tabsPresenter.Children.Count - 2]).SheetIndex, true);
                        }
                    }
                }
                else
                {
                    int index = TabsPresenter.Children.IndexOf(hitSheetTab);
                    if (index == (TabsPresenter.StartIndex - 1))
                    {
                        TabsPresenter.NavigateToPrevious();
                    }
                    else if (TabsPresenter.ReCalculateStartIndex(TabsPresenter.StartIndex, index))
                    {
                        TabsPresenter.InvalidateMeasure();
                        TabsPresenter.InvalidateArrange();
                    }
                    if (hitSheetTab != _activeTab)
                    {
                        ActiveSheet(hitSheetTab.SheetIndex, true);
                    }
                }
                Excel.RaiseSheetTabClick(hitSheetTab.SheetIndex);
            }
        }

        void ProcessNavigation(int nextIndex)
        {
            SheetTab tab = TabsPresenter.Children[nextIndex] as SheetTab;
            if (((tab != null) && (tab.SheetIndex != -1)) && (tab != null))
            {
                TabsPresenter tabsPresenter = TabsPresenter;
                if (tab == _newTab)
                {
                    if (!Workbook.Protect)
                    {
                        OnNewTabNeeded(EventArgs.Empty);
                        if (tabsPresenter.Children.Count > 1)
                        {
                            ActiveSheet(((SheetTab)tabsPresenter.Children[tabsPresenter.Children.Count - 2]).SheetIndex, true);
                        }
                    }
                }
                else
                {
                    int index = tabsPresenter.Children.IndexOf(tab);
                    if (index == (tabsPresenter.StartIndex - 1))
                    {
                        tabsPresenter.NavigateToPrevious();
                    }
                    else if (tabsPresenter.ReCalculateStartIndex(tabsPresenter.StartIndex, index))
                    {
                        tabsPresenter.InvalidateMeasure();
                        tabsPresenter.InvalidateArrange();
                    }
                    if (tab != _activeTab)
                    {
                        ActiveSheet(tab.SheetIndex, true);
                    }
                }
                Excel.RaiseSheetTabClick(tab.SheetIndex);
            }
        }

        public void ProcessTap(Point point)
        {
            SheetTab touchHitSheetTab = GetHitSheetTab(point);
            if (touchHitSheetTab == null)
                return;

            TabsPresenter tabsPresenter = TabsPresenter;
            if (touchHitSheetTab == _newTab)
            {
                if (!Workbook.Protect)
                {
                    OnNewTabNeeded(EventArgs.Empty);
                    if (tabsPresenter.Children.Count > 1)
                    {
                        ActiveSheet(((SheetTab)tabsPresenter.Children[tabsPresenter.Children.Count - 2]).SheetIndex, true);
                    }
                }
            }
            else
            {
                int index = TabsPresenter.Children.IndexOf(touchHitSheetTab);
                if (index == (TabsPresenter.StartIndex - 1))
                {
                    TabsPresenter.NavigateToPrevious();
                }
                else if (TabsPresenter.ReCalculateStartIndex(TabsPresenter.StartIndex, index))
                {
                    TabsPresenter.InvalidateMeasure();
                    TabsPresenter.InvalidateArrange();
                }
                if (touchHitSheetTab != _activeTab)
                {
                    ActiveSheet(touchHitSheetTab.SheetIndex, true);
                }
            }
            Excel.RaiseSheetTabClick(touchHitSheetTab.SheetIndex);
        }

        public void SetStartSheet(int startSheetIndex)
        {
            if ((TabsPresenter.Children.Count != 0) && (TabsPresenter.Children.Count > startSheetIndex))
            {
                TabsPresenter.SetStartSheet(startSheetIndex);
            }
        }

        SheetTab GetHitSheetTab(Point p_point)
        {
            if (TabsPresenter.Children.Count > 0)
            {
                var pt = Windows.UI.Xaml.Window.Current.Content.TransformToVisual(TabsPresenter).TransformPoint(p_point);
                if (pt.X > 0)
                {
                    double x = TabsPresenter.StartIndex > 0 ? 10 : 0.0;
                    for (int i = TabsPresenter.StartIndex; i < TabsPresenter.Children.Count; i++)
                    {
                        var tab = (SheetTab)TabsPresenter.Children[i];
                        if (pt.X > x && pt.X <= x + tab.DesiredSize.Width)
                            return tab;
                        x += tab.DesiredSize.Width;
                    }
                }
            }
            return null;
        }

        #region 编辑
        public void StartTabEditing(PointerRoutedEventArgs e)
        {
            if (_activeTab != null && !Workbook.Protect)
                StartTabEditing(e.GetCurrentPoint(null).Position);
        }

        public void StartTabTouchEditing(Point point)
        {
            if (_activeTab != null && !Workbook.Protect)
                StartTabEditing(point);
        }

        void StartTabEditing(Point point)
        {
            SheetTab tab = GetHitSheetTab(point);
            if (tab != null && tab.SheetIndex == Workbook.ActiveSheetIndex && tab != _editingTab)
            {
                if (IsEditing)
                    StopTabEditing(false);

                tab.PrepareForEditing();
                IsEditing = true;
                _editingTab = tab;
            }
        }

        public void StopTabEditing(bool cancel)
        {
            if (!IsEditing)
                return;

            string text = _editingTab.GetEditText();
            if (!cancel && !string.IsNullOrEmpty(text) && IsValidSheetName(text))
            {
                SheetRenameUndoAction command = new SheetRenameUndoAction(Workbook.Sheets[_editingTab.SheetIndex], text);
                Excel.DoCommand(command);
            }
            _editingTab.PrepareForDisplay();
            _editingTab = null;
            IsEditing = false;
        }

        public bool StayInEditing(Point point)
        {
            return ((IsEditing && (_editingTab != null)) && (GetHitSheetTab(point) == _editingTab));
        }

        bool IsValidSheetName(string sheetName)
        {
            if (string.IsNullOrEmpty(sheetName))
            {
                return false;
            }
            foreach (Worksheet worksheet in Workbook.Sheets)
            {
                if ((worksheet != Workbook.ActiveSheet) && (sheetName == worksheet.Name))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        void OnActiveTabChanged(EventArgs e)
        {
            ActiveTabChanged?.Invoke(this, e);
        }

        void OnActiveTabChanging(EventArgs e)
        {
            ActiveTabChanging?.Invoke(this, e);
        }

        void OnNewTabNeeded(EventArgs e)
        {
            NewTabNeeded?.Invoke(this, e);
        }

    }
}

