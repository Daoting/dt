#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UndoRedo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /> tab strip control.
    /// </summary>
    public partial class TabStrip : Control
    {
        SheetTab _activeTab;
        SheetTab _editingTab;
        bool _hasNewTab;
        SheetTab _newTab;
        Grid _root;
        TabStripPresenter _tabStripPresent;
        bool showTextBoxContextMenus;
        const string TABSTRIP_elementRoot = "Root";

        internal event EventHandler ActiveTabChanged;

        internal event EventHandler ActiveTabChanging;

        internal event EventHandler NewTabNeeded;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public TabStrip()
        {
            base.DefaultStyleKey = typeof(TabStrip);
            _root = null;
            _tabStripPresent = new TabStripPresenter();
            _tabStripPresent.TabPresenter.PropertyChanged += new EventHandler<PropertyChangedEventArgs>(TabPresenter_PropertyChanged);
            _activeTab = null;
            _editingTab = null;
            _hasNewTab = true;
            _newTab = null;
        }

        internal void ActiveNextTab()
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

        internal void ActivePreviousTab()
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

        internal void ActiveSheet(int sheetIndex, bool raiseEvent)
        {
            if (TabsPresenter.Count != 0)
            {
                for (int i = 0; i < TabsPresenter.Children.Count; i++)
                {
                    SheetTab tab = TabsPresenter.Children[i] as SheetTab;
                    if ((tab.SheetIndex == sheetIndex) && (tab != _activeTab))
                    {
                        if (raiseEvent)
                        {
                            CancelEventArgs args = new CancelEventArgs();
                            if (!tab.IsActive)
                            {
                                OnActiveTabChanging((EventArgs) args);
                            }
                            if (!args.Cancel)
                            {
                                if (_activeTab != null)
                                {
                                    _activeTab.IsActive = false;
                                }
                                tab.IsActive = true;
                                _activeTab = tab;
                                UpdateZIndexes();
                                OnActiveTabChanged(EventArgs.Empty);
                            }
                        }
                        else
                        {
                            if (_activeTab != null)
                            {
                                _activeTab.IsActive = false;
                            }
                            tab.IsActive = true;
                            _activeTab = tab;
                            UpdateZIndexes();
                        }
                    }
                }
            }
        }

        internal void AddSheets(WorksheetCollection sheets)
        {
            StopTabEditing(false);
            List<SheetTab> list = new List<SheetTab>();
            List<SheetTab> list2 = new List<SheetTab>();
            int count = TabsPresenter.Count;
            int num2 = sheets.Count;
            if (count < num2)
            {
                for (int j = count; j < num2; j++)
                {
                    SheetTab tab = new SheetTab();
                    list.Add(tab);
                }
            }
            else if (count > num2)
            {
                for (int k = num2; k < count; k++)
                {
                    list2.Add((SheetTab) TabsPresenter.Children[k]);
                }
            }
            UIElementCollection children = TabsPresenter.Children;
            if (list2.Count > 0)
            {
                foreach (SheetTab tab2 in list2)
                {
                    children.Remove(tab2);
                }
            }
            if (list.Count > 0)
            {
                foreach (SheetTab tab3 in list)
                {
                    children.Add(tab3);
                }
            }
            for (int i = 0; i < children.Count; i++)
            {
                SheetTab tab4 = children[i] as SheetTab;
                tab4.OwningStrip = this;
                tab4.SheetIndex = i;
                tab4.Click -= Tab_Click;
                tab4.Click += Tab_Click;
            }
            if (_hasNewTab)
            {
                _newTab = new SheetTab();
                _newTab.OwningStrip = this;
                SheetTab tab5 = _newTab;
                tab5.Click += Tab_Click;
                TabsPresenter.Children.Add(_newTab);
            }
        }

        ButtonBase GetHitNavigatorButton(Windows.Foundation.Point point)
        {
            if (OwningView == null)
            {
                return null;
            }
            List<UIElement> list = Enumerable.ToList<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(TranslatePoint(point, Windows.UI.Xaml.Window.Current.Content), Windows.UI.Xaml.Window.Current.Content));
            if ((list == null) || (list.Count <= 0))
            {
                goto Label_00BD;
            }
            bool flag = false;
            using (List<UIElement>.Enumerator enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is TabStripNavigator)
                    {
                        flag = true;
                        goto Label_007A;
                    }
                }
            }
        Label_007A:
            if (flag)
            {
                foreach (UIElement element2 in list)
                {
                    if (element2 is RepeatButton)
                    {
                        return (RepeatButton) element2;
                    }
                }
            }
        Label_00BD:
            return null;
        }

        SheetTab GetHitSheetTab(PointerRoutedEventArgs mArgs)
        {
            if (TabsPresenter.Count > 0)
            {
                List<UIElement> list = Enumerable.ToList<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(mArgs.GetCurrentPoint(Windows.UI.Xaml.Window.Current.Content).Position, Windows.UI.Xaml.Window.Current.Content));
                if ((list != null) && (list.Count > 0))
                {
                    foreach (UIElement element in list)
                    {
                        if (element is SheetTab)
                        {
                            return (SheetTab) element;
                        }
                    }
                }
            }
            return null;
        }

        internal int GetStartIndexToBringTabIntoView(int tabIndex)
        {
            return _tabStripPresent.GetStartIndexToBringTabIntoView(tabIndex);
        }

        SheetTab GetTouchHitSheetTab(Windows.Foundation.Point point)
        {
            int count = TabsPresenter.Count;
            if ((OwningView != null) && (count > 0))
            {
                List<UIElement> list = Enumerable.ToList<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(TranslatePoint(point, Windows.UI.Xaml.Window.Current.Content), Windows.UI.Xaml.Window.Current.Content));
                if ((list != null) && (list.Count > 0))
                {
                    foreach (UIElement element in list)
                    {
                        if (element is SheetTab)
                        {
                            return (SheetTab) element;
                        }
                    }
                }
            }
            return null;
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

        internal void NewTab(int sheetIndex)
        {
            StopTabEditing(false);
            SheetTab tab = new SheetTab {
                OwningStrip = this,
                SheetIndex = sheetIndex
            };
            tab.Click += Tab_Click;
            int count = TabsPresenter.Count;
            if (HasInsertTab && (TabsPresenter.Count > 0))
            {
                count = TabsPresenter.Count - 1;
            }
            TabsPresenter.Children.Insert(count, tab);
            TabsPresenter.Update();
            TabsPresenter.ReCalculateStartIndex(0, TabsPresenter.Count - 1);
        }

        internal void OnActiveTabChanged(EventArgs e)
        {
            if (ActiveTabChanged != null)
            {
                ActiveTabChanged(this, e);
            }
        }

        internal void OnActiveTabChanging(EventArgs e)
        {
            if (ActiveTabChanging != null)
            {
                ActiveTabChanging(this, e);
            }
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate()" />, when overridden in a derived class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_root != null)
            {
                _root.Children.Clear();
                _root = null;
            }
            _root = base.GetTemplateChild("Root") as Grid;
            if ((_root != null) && !_root.Children.Contains(_tabStripPresent))
            {
                _root.Children.Add(_tabStripPresent);
            }
        }

        void OnEditorContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!showTextBoxContextMenus)
            {
                e.Handled = true;
            }
            showTextBoxContextMenus = true;
        }

        internal virtual void OnNewTabNeeded(EventArgs e)
        {
            EventHandler newTabNeeded = NewTabNeeded;
            if (newTabNeeded != null)
            {
                newTabNeeded(this, e);
            }
        }

        async void PrepareTabForEditing(object sender, RoutedEventArgs e)
        {
            TextBox editor = sender as TextBox;
            if (editor != null)
            {
                await editor.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
                {
                    editor.Focus(FocusState.Programmatic);
                    editor.SelectAll();
                });
            }
        }

        internal void ProcessMouseClickSheetTab(PointerRoutedEventArgs args)
        {
            SheetTab hitSheetTab = GetHitSheetTab(args);
            if (hitSheetTab != null)
            {
                Dt.Cells.UI.TabsPresenter tabsPresenter = TabsPresenter;
                if (hitSheetTab == _newTab)
                {
                    if (!Workbook.Protect)
                    {
                        OwningView.SaveDataForFormulaSelection();
                        OnNewTabNeeded(EventArgs.Empty);
                        if (tabsPresenter.Count > 1)
                        {
                            ActiveSheet(((SheetTab) tabsPresenter.Children[tabsPresenter.Count - 2]).SheetIndex, true);
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
                        OwningView.SaveDataForFormulaSelection();
                        ActiveSheet(hitSheetTab.SheetIndex, true);
                    }
                }
                OwningView.RaiseSheetTabClick(hitSheetTab.SheetIndex);
            }
        }

        void ProcessNavigation(int nextIndex)
        {
            SheetTab tab = TabsPresenter.Children[nextIndex] as SheetTab;
            if (((tab != null) && (tab.SheetIndex != -1)) && (tab != null))
            {
                Dt.Cells.UI.TabsPresenter tabsPresenter = TabsPresenter;
                if (tab == _newTab)
                {
                    if (!Workbook.Protect)
                    {
                        OnNewTabNeeded(EventArgs.Empty);
                        if (tabsPresenter.Count > 1)
                        {
                            ActiveSheet(((SheetTab) tabsPresenter.Children[tabsPresenter.Count - 2]).SheetIndex, true);
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
                OwningView.RaiseSheetTabClick(tab.SheetIndex);
            }
        }

        internal void ProcessTap(Windows.Foundation.Point point)
        {
            SheetTab touchHitSheetTab = GetTouchHitSheetTab(point);
            if (touchHitSheetTab != null)
            {
                Dt.Cells.UI.TabsPresenter tabsPresenter = TabsPresenter;
                if (touchHitSheetTab == _newTab)
                {
                    if (!Workbook.Protect)
                    {
                        if (OwningView.CanSelectFormula)
                        {
                            OwningView.SaveDataForFormulaSelection();
                            OwningView.StopCellEditing(true);
                        }
                        OnNewTabNeeded(EventArgs.Empty);
                        if (tabsPresenter.Count > 1)
                        {
                            ActiveSheet(((SheetTab) tabsPresenter.Children[tabsPresenter.Count - 2]).SheetIndex, true);
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
                        if (OwningView.CanSelectFormula)
                        {
                            OwningView.SaveDataForFormulaSelection();
                            OwningView.StopCellEditing(true);
                        }
                        ActiveSheet(touchHitSheetTab.SheetIndex, true);
                    }
                }
                OwningView.RaiseSheetTabClick(touchHitSheetTab.SheetIndex);
            }
            else
            {
                ButtonBase hitNavigatorButton = GetHitNavigatorButton(point);
                if (hitNavigatorButton != null)
                {
                    string name = hitNavigatorButton.Name;
                    if ("First".Equals(name))
                    {
                        TabsPresenter.NavigateToFirst();
                    }
                    else if ("Previous".Equals(name))
                    {
                        TabsPresenter.NavigateToPrevious();
                    }
                    else if ("Next".Equals(name))
                    {
                        TabsPresenter.NavigateToNext();
                    }
                    else if ("Last".Equals(name))
                    {
                        TabsPresenter.NavigateToLast();
                    }
                }
            }
        }

        internal void Refresh()
        {
            if (TabsPresenter.Children.Count > 0)
            {
                foreach (SheetTab tab in TabsPresenter.Children)
                {
                    tab.PrepareForDisplay();
                }
                _tabStripPresent.InvalidateMeasure();
                _tabStripPresent.InvalidateArrange();
            }
            showTextBoxContextMenus = false;
        }
        
        internal void SetStartSheet(int startSheetIndex)
        {
            if ((TabsPresenter.Count != 0) && (TabsPresenter.Count > startSheetIndex))
            {
                TabsPresenter.SetStartSheet(startSheetIndex);
            }
        }

        internal void StartTabEditing(PointerRoutedEventArgs mouseEventArgs)
        {
            if (((mouseEventArgs != null) && (_activeTab != null)) && ((Workbook != null) && !Workbook.Protect))
            {
                SheetTab hitSheetTab = GetHitSheetTab(mouseEventArgs);
                if (((hitSheetTab != null) && (hitSheetTab.SheetIndex == Workbook.ActiveSheetIndex)) && (hitSheetTab != _editingTab))
                {
                    if (IsEditing)
                    {
                        StopTabEditing(false);
                    }
                    TextBox editingElement = hitSheetTab.GetEditingElement();
                    editingElement.Loaded += PrepareTabForEditing;
                    editingElement.LostFocus += TabEditor_LostFocus;
                    hitSheetTab.PrepareForEditing();
                    editingElement.TextChanged += TabEditor_TextChanged;
                    TabsPresenter.InvalidateMeasure();
                    TabsPresenter.InvalidateArrange();
                    IsEditing = true;
                    _editingTab = hitSheetTab;
                }
            }
        }

        internal void StartTabTouchEditing(Windows.Foundation.Point point)
        {
            if (((_activeTab != null) && (Workbook != null)) && !Workbook.Protect)
            {
                SheetTab touchHitSheetTab = GetTouchHitSheetTab(point);
                if (((touchHitSheetTab != null) && (touchHitSheetTab.SheetIndex == Workbook.ActiveSheetIndex)) && (touchHitSheetTab != _editingTab))
                {
                    if (IsEditing)
                    {
                        StopTabEditing(false);
                    }
                    TextBox editingElement = touchHitSheetTab.GetEditingElement();
                    editingElement.Loaded += PrepareTabForEditing;
                    editingElement.LostFocus += TabEditor_LostFocus;
                    editingElement.ContextMenuOpening += OnEditorContextMenuOpening;
                    touchHitSheetTab.PrepareForEditing();
                    editingElement.TextChanged += TabEditor_TextChanged;
                    TabsPresenter.InvalidateMeasure();
                    TabsPresenter.InvalidateArrange();
                    IsEditing = true;
                    _editingTab = touchHitSheetTab;
                }
            }
        }

        internal bool StayInEditing(Windows.Foundation.Point point)
        {
            return ((IsEditing && (_editingTab != null)) && (GetTouchHitSheetTab(point) == _editingTab));
        }

        internal async void StopTabEditing(bool cancel)
        {
            DispatchedHandler agileCallback = null;
            if ((_editingTab != null) && IsEditing)
            {
                TextBox editingElement = _editingTab.GetEditingElement();
                if (editingElement != null)
                {
                    string text = editingElement.Text;
                    if ((!cancel && !string.IsNullOrEmpty(text)) && IsValidSheetName(text))
                    {
                        SheetRenameUndoAction command = new SheetRenameUndoAction(Workbook.Sheets[_editingTab.SheetIndex], text);
                        OwningView.DoCommand(command);
                    }
                    editingElement.Loaded += PrepareTabForEditing;
                    editingElement.LostFocus += TabEditor_LostFocus; 
                    editingElement.ContextMenuOpening += OnEditorContextMenuOpening;
                    editingElement.TextChanged += TabEditor_TextChanged;
                }
                _editingTab.PrepareForDisplay();
                _editingTab = null;
                TabsPresenter.InvalidateMeasure();
                TabsPresenter.InvalidateArrange();
                if (OwningView != null)
                {
                    if (agileCallback == null)
                    {
                        agileCallback = delegate {
                            OwningView.FocusInternal();
                        };
                    }
                    await OwningView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, agileCallback);
                }
            }
            IsEditing = false;
            showTextBoxContextMenus = false;
        }

        void Tab_Click(object sender, RoutedEventArgs e)
        {
        }

        void TabEditor_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                StopTabEditing(false);
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.Escape)
            {
                StopTabEditing(true);
            }
        }

        void TabEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            StopTabEditing(false);
        }

        void TabEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            TabsPresenter.InvalidateMeasure();
            TabsPresenter.InvalidateArrange();
        }

        void TabPresenter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (((e.PropertyName == "StartIndex") && (OwningView != null)) && ((OwningView.Worksheet != null) && (OwningView.Worksheet.Workbook != null)))
            {
                OwningView.Worksheet.Workbook.StartSheetIndex = TabsPresenter.StartIndex;
            }
        }

        Windows.Foundation.Point TranslatePoint(Windows.Foundation.Point point, UIElement element)
        {
            return OwningView.TransformToVisual(element).TransformPoint(point);
        }

        internal void Update()
        {
            StopTabEditing(false);
            foreach (SheetTab tab in TabsPresenter.Children)
            {
                tab.Click -= Tab_Click;
                tab.SheetIndex = -1;
                tab.OwningStrip = null;
                tab.IsActive = false;
            }
            TabsPresenter.Update();
            _activeTab = null;
            _editingTab = null;
        }

        void UpdateZIndexes()
        {
            int count = TabsPresenter.Count;
            if ((count > 0) && (_activeTab != null))
            {
                for (int i = 0; i < count; i++)
                {
                    if (TabsPresenter.Children[i] != _activeTab)
                    {
                        Canvas.SetZIndex((UIElement)TabsPresenter.Children[i], count - i);
                    }
                }
                Canvas.SetZIndex(_activeTab, count + 1);
            }
        }

        internal SheetTab ActiveTab
        {
            get { return  _activeTab; }
        }

        internal bool HasInsertTab
        {
            get { return  _hasNewTab; }
            set
            {
                Action action = null;
                if (value != _hasNewTab)
                {
                    _hasNewTab = value;
                    if (_hasNewTab)
                    {
                        if (_newTab == null)
                        {
                            _newTab = new SheetTab();
                            _newTab.OwningStrip = this;
                            _newTab.Click += Tab_Click;
                        }
                        if (!TabsPresenter.Children.Contains(_newTab))
                        {
                            TabsPresenter.Children.Add(_newTab);
                        }
                        if (_root != null)
                        {
                            TabsPresenter.Update();
                            TabsPresenter.ReCalculateStartIndex(0, TabsPresenter.Count - 1);
                        }
                    }
                    else
                    {
                        if (action == null)
                        {
                            action = delegate {
                                TabsPresenter.Children.Remove(_newTab);
                                if (_newTab != null)
                                {
                                    _newTab.Click -= Tab_Click;
                                    _newTab = null;
                                }
                            };
                        }
                        Dt.Cells.Data.UIAdaptor.InvokeSync(action);
                    }
                }
            }
        }

        internal bool IsEditing { get; private set; }

        internal SpreadView OwningView { get; set; }

        internal Dt.Cells.UI.TabsPresenter TabsPresenter
        {
            get { return  _tabStripPresent.TabPresenter; }
        }

        internal Dt.Cells.Data.Workbook Workbook
        {
            get { return  OwningView.SpreadSheet.Workbook; }
        }
    }
}

