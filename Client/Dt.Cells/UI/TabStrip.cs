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
        private SheetTab _activeTab;
        private SheetTab _editingTab;
        private bool _hasNewTab;
        private SheetTab _newTab;
        private Grid _root;
        private TabStripPresenter _tabStripPresent;
        private bool showTextBoxContextMenus;
        private const string TABSTRIP_elementRoot = "Root";

        internal event EventHandler ActiveTabChanged;

        internal event EventHandler ActiveTabChanging;

        internal event EventHandler NewTabNeeded;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public TabStrip()
        {
            base.DefaultStyleKey = typeof(TabStrip);
            this._root = null;
            this._tabStripPresent = new TabStripPresenter();
            this._tabStripPresent.TabPresenter.PropertyChanged += new EventHandler<PropertyChangedEventArgs>(this.TabPresenter_PropertyChanged);
            this._activeTab = null;
            this._editingTab = null;
            this._hasNewTab = true;
            this._newTab = null;
        }

        internal void ActiveNextTab()
        {
            if (this._activeTab != null)
            {
                int nextVisibleIndex = this.TabsPresenter.GetNextVisibleIndex(this.TabsPresenter.Children.IndexOf(this._activeTab));
                if (nextVisibleIndex != -1)
                {
                    this.ProcessNavigation(nextVisibleIndex);
                }
            }
        }

        internal void ActivePreviousTab()
        {
            if (this._activeTab != null)
            {
                int preVisibleIndex = this.TabsPresenter.GetPreVisibleIndex(this.TabsPresenter.Children.IndexOf(this._activeTab));
                if (preVisibleIndex != -1)
                {
                    this.ProcessNavigation(preVisibleIndex);
                }
            }
        }

        internal void ActiveSheet(int sheetIndex, bool raiseEvent)
        {
            if (this.TabsPresenter.Count != 0)
            {
                for (int i = 0; i < this.TabsPresenter.Children.Count; i++)
                {
                    SheetTab tab = this.TabsPresenter.Children[i] as SheetTab;
                    if ((tab.SheetIndex == sheetIndex) && (tab != this._activeTab))
                    {
                        if (raiseEvent)
                        {
                            CancelEventArgs args = new CancelEventArgs();
                            if (!tab.IsActive)
                            {
                                this.OnActiveTabChanging((EventArgs) args);
                            }
                            if (!args.Cancel)
                            {
                                if (this._activeTab != null)
                                {
                                    this._activeTab.IsActive = false;
                                }
                                tab.IsActive = true;
                                this._activeTab = tab;
                                this.UpdateZIndexes();
                                this.OnActiveTabChanged(EventArgs.Empty);
                            }
                        }
                        else
                        {
                            if (this._activeTab != null)
                            {
                                this._activeTab.IsActive = false;
                            }
                            tab.IsActive = true;
                            this._activeTab = tab;
                            this.UpdateZIndexes();
                        }
                    }
                }
            }
        }

        internal void AddSheets(WorksheetCollection sheets)
        {
            this.StopTabEditing(false);
            List<SheetTab> list = new List<SheetTab>();
            List<SheetTab> list2 = new List<SheetTab>();
            int count = this.TabsPresenter.Count;
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
                    list2.Add((SheetTab) this.TabsPresenter.Children[k]);
                }
            }
            UIElementCollection children = this.TabsPresenter.Children;
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
            if (this._hasNewTab)
            {
                this._newTab = new SheetTab();
                this._newTab.OwningStrip = this;
                SheetTab tab5 = this._newTab;
                tab5.Click += Tab_Click;
                this.TabsPresenter.Children.Add(this._newTab);
            }
        }

        private ButtonBase GetHitNavigatorButton(Windows.Foundation.Point point)
        {
            if (this.OwningView == null)
            {
                return null;
            }
            List<UIElement> list = Enumerable.ToList<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(this.TranslatePoint(point, Windows.UI.Xaml.Window.Current.Content), Windows.UI.Xaml.Window.Current.Content));
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

        private SheetTab GetHitSheetTab(PointerRoutedEventArgs mArgs)
        {
            if (this.TabsPresenter.Count > 0)
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
            return this._tabStripPresent.GetStartIndexToBringTabIntoView(tabIndex);
        }

        private SheetTab GetTouchHitSheetTab(Windows.Foundation.Point point)
        {
            int count = this.TabsPresenter.Count;
            if ((this.OwningView != null) && (count > 0))
            {
                List<UIElement> list = Enumerable.ToList<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(this.TranslatePoint(point, Windows.UI.Xaml.Window.Current.Content), Windows.UI.Xaml.Window.Current.Content));
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

        private bool IsValidSheetName(string sheetName)
        {
            if (string.IsNullOrEmpty(sheetName))
            {
                return false;
            }
            foreach (Worksheet worksheet in this.Workbook.Sheets)
            {
                if ((worksheet != this.Workbook.ActiveSheet) && (sheetName == worksheet.Name))
                {
                    return false;
                }
            }
            return true;
        }

        internal void NewTab(int sheetIndex)
        {
            this.StopTabEditing(false);
            SheetTab tab = new SheetTab {
                OwningStrip = this,
                SheetIndex = sheetIndex
            };
            tab.Click += Tab_Click;
            int count = this.TabsPresenter.Count;
            if (this.HasInsertTab && (this.TabsPresenter.Count > 0))
            {
                count = this.TabsPresenter.Count - 1;
            }
            this.TabsPresenter.Children.Insert(count, tab);
            this.TabsPresenter.Update();
            this.TabsPresenter.ReCalculateStartIndex(0, this.TabsPresenter.Count - 1);
        }

        internal void OnActiveTabChanged(EventArgs e)
        {
            if (this.ActiveTabChanged != null)
            {
                this.ActiveTabChanged(this, e);
            }
        }

        internal void OnActiveTabChanging(EventArgs e)
        {
            if (this.ActiveTabChanging != null)
            {
                this.ActiveTabChanging(this, e);
            }
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate()" />, when overridden in a derived class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this._root != null)
            {
                this._root.Children.Clear();
                this._root = null;
            }
            this._root = base.GetTemplateChild("Root") as Grid;
            if ((this._root != null) && !this._root.Children.Contains(this._tabStripPresent))
            {
                this._root.Children.Add(this._tabStripPresent);
            }
        }

        private void OnEditorContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!this.showTextBoxContextMenus)
            {
                e.Handled = true;
            }
            this.showTextBoxContextMenus = true;
        }

        internal virtual void OnNewTabNeeded(EventArgs e)
        {
            EventHandler newTabNeeded = this.NewTabNeeded;
            if (newTabNeeded != null)
            {
                newTabNeeded(this, e);
            }
        }

        private async void PrepareTabForEditing(object sender, RoutedEventArgs e)
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
            SheetTab hitSheetTab = this.GetHitSheetTab(args);
            if (hitSheetTab != null)
            {
                Dt.Cells.UI.TabsPresenter tabsPresenter = this.TabsPresenter;
                if (hitSheetTab == this._newTab)
                {
                    if (!this.Workbook.Protect)
                    {
                        this.OwningView.SaveDataForFormulaSelection();
                        this.OnNewTabNeeded(EventArgs.Empty);
                        if (tabsPresenter.Count > 1)
                        {
                            this.ActiveSheet(((SheetTab) tabsPresenter.Children[tabsPresenter.Count - 2]).SheetIndex, true);
                        }
                    }
                }
                else
                {
                    int index = this.TabsPresenter.Children.IndexOf(hitSheetTab);
                    if (index == (this.TabsPresenter.StartIndex - 1))
                    {
                        this.TabsPresenter.NavigateToPrevious();
                    }
                    else if (this.TabsPresenter.ReCalculateStartIndex(this.TabsPresenter.StartIndex, index))
                    {
                        this.TabsPresenter.InvalidateMeasure();
                        this.TabsPresenter.InvalidateArrange();
                    }
                    if (hitSheetTab != this._activeTab)
                    {
                        this.OwningView.SaveDataForFormulaSelection();
                        this.ActiveSheet(hitSheetTab.SheetIndex, true);
                    }
                }
                this.OwningView.RaiseSheetTabClick(hitSheetTab.SheetIndex);
            }
        }

        private void ProcessNavigation(int nextIndex)
        {
            SheetTab tab = this.TabsPresenter.Children[nextIndex] as SheetTab;
            if (((tab != null) && (tab.SheetIndex != -1)) && (tab != null))
            {
                Dt.Cells.UI.TabsPresenter tabsPresenter = this.TabsPresenter;
                if (tab == this._newTab)
                {
                    if (!this.Workbook.Protect)
                    {
                        this.OnNewTabNeeded(EventArgs.Empty);
                        if (tabsPresenter.Count > 1)
                        {
                            this.ActiveSheet(((SheetTab) tabsPresenter.Children[tabsPresenter.Count - 2]).SheetIndex, true);
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
                    if (tab != this._activeTab)
                    {
                        this.ActiveSheet(tab.SheetIndex, true);
                    }
                }
                this.OwningView.RaiseSheetTabClick(tab.SheetIndex);
            }
        }

        internal void ProcessTap(Windows.Foundation.Point point)
        {
            SheetTab touchHitSheetTab = this.GetTouchHitSheetTab(point);
            if (touchHitSheetTab != null)
            {
                Dt.Cells.UI.TabsPresenter tabsPresenter = this.TabsPresenter;
                if (touchHitSheetTab == this._newTab)
                {
                    if (!this.Workbook.Protect)
                    {
                        if (this.OwningView.CanSelectFormula)
                        {
                            this.OwningView.SaveDataForFormulaSelection();
                            this.OwningView.StopCellEditing(true);
                        }
                        this.OnNewTabNeeded(EventArgs.Empty);
                        if (tabsPresenter.Count > 1)
                        {
                            this.ActiveSheet(((SheetTab) tabsPresenter.Children[tabsPresenter.Count - 2]).SheetIndex, true);
                        }
                    }
                }
                else
                {
                    int index = this.TabsPresenter.Children.IndexOf(touchHitSheetTab);
                    if (index == (this.TabsPresenter.StartIndex - 1))
                    {
                        this.TabsPresenter.NavigateToPrevious();
                    }
                    else if (this.TabsPresenter.ReCalculateStartIndex(this.TabsPresenter.StartIndex, index))
                    {
                        this.TabsPresenter.InvalidateMeasure();
                        this.TabsPresenter.InvalidateArrange();
                    }
                    if (touchHitSheetTab != this._activeTab)
                    {
                        if (this.OwningView.CanSelectFormula)
                        {
                            this.OwningView.SaveDataForFormulaSelection();
                            this.OwningView.StopCellEditing(true);
                        }
                        this.ActiveSheet(touchHitSheetTab.SheetIndex, true);
                    }
                }
                this.OwningView.RaiseSheetTabClick(touchHitSheetTab.SheetIndex);
            }
            else
            {
                ButtonBase hitNavigatorButton = this.GetHitNavigatorButton(point);
                if (hitNavigatorButton != null)
                {
                    string name = hitNavigatorButton.Name;
                    if ("First".Equals(name))
                    {
                        this.TabsPresenter.NavigateToFirst();
                    }
                    else if ("Previous".Equals(name))
                    {
                        this.TabsPresenter.NavigateToPrevious();
                    }
                    else if ("Next".Equals(name))
                    {
                        this.TabsPresenter.NavigateToNext();
                    }
                    else if ("Last".Equals(name))
                    {
                        this.TabsPresenter.NavigateToLast();
                    }
                }
            }
        }

        internal void Refresh()
        {
            if (this.TabsPresenter.Children.Count > 0)
            {
                foreach (SheetTab tab in this.TabsPresenter.Children)
                {
                    tab.PrepareForDisplay();
                }
                this._tabStripPresent.InvalidateMeasure();
                this._tabStripPresent.InvalidateArrange();
            }
            this.showTextBoxContextMenus = false;
        }
        
        internal void SetStartSheet(int startSheetIndex)
        {
            if ((this.TabsPresenter.Count != 0) && (this.TabsPresenter.Count > startSheetIndex))
            {
                this.TabsPresenter.SetStartSheet(startSheetIndex);
            }
        }

        internal void StartTabEditing(PointerRoutedEventArgs mouseEventArgs)
        {
            if (((mouseEventArgs != null) && (this._activeTab != null)) && ((this.Workbook != null) && !this.Workbook.Protect))
            {
                SheetTab hitSheetTab = this.GetHitSheetTab(mouseEventArgs);
                if (((hitSheetTab != null) && (hitSheetTab.SheetIndex == this.Workbook.ActiveSheetIndex)) && (hitSheetTab != this._editingTab))
                {
                    if (this.IsEditing)
                    {
                        this.StopTabEditing(false);
                    }
                    TextBox editingElement = hitSheetTab.GetEditingElement();
                    editingElement.Loaded += PrepareTabForEditing;
                    editingElement.LostFocus += TabEditor_LostFocus;
                    hitSheetTab.PrepareForEditing();
                    editingElement.TextChanged += TabEditor_TextChanged;
                    this.TabsPresenter.InvalidateMeasure();
                    this.TabsPresenter.InvalidateArrange();
                    this.IsEditing = true;
                    this._editingTab = hitSheetTab;
                }
            }
        }

        internal void StartTabTouchEditing(Windows.Foundation.Point point)
        {
            if (((this._activeTab != null) && (this.Workbook != null)) && !this.Workbook.Protect)
            {
                SheetTab touchHitSheetTab = this.GetTouchHitSheetTab(point);
                if (((touchHitSheetTab != null) && (touchHitSheetTab.SheetIndex == this.Workbook.ActiveSheetIndex)) && (touchHitSheetTab != this._editingTab))
                {
                    if (this.IsEditing)
                    {
                        this.StopTabEditing(false);
                    }
                    TextBox editingElement = touchHitSheetTab.GetEditingElement();
                    editingElement.Loaded += PrepareTabForEditing;
                    editingElement.LostFocus += TabEditor_LostFocus;
                    editingElement.ContextMenuOpening += OnEditorContextMenuOpening;
                    touchHitSheetTab.PrepareForEditing();
                    editingElement.TextChanged += TabEditor_TextChanged;
                    this.TabsPresenter.InvalidateMeasure();
                    this.TabsPresenter.InvalidateArrange();
                    this.IsEditing = true;
                    this._editingTab = touchHitSheetTab;
                }
            }
        }

        internal bool StayInEditing(Windows.Foundation.Point point)
        {
            return ((this.IsEditing && (this._editingTab != null)) && (this.GetTouchHitSheetTab(point) == this._editingTab));
        }

        internal async void StopTabEditing(bool cancel)
        {
            DispatchedHandler agileCallback = null;
            if ((this._editingTab != null) && this.IsEditing)
            {
                TextBox editingElement = this._editingTab.GetEditingElement();
                if (editingElement != null)
                {
                    string text = editingElement.Text;
                    if ((!cancel && !string.IsNullOrEmpty(text)) && this.IsValidSheetName(text))
                    {
                        SheetRenameUndoAction command = new SheetRenameUndoAction(this.Workbook.Sheets[this._editingTab.SheetIndex], text);
                        this.OwningView.DoCommand(command);
                    }
                    editingElement.Loaded += PrepareTabForEditing;
                    editingElement.LostFocus += TabEditor_LostFocus; 
                    editingElement.ContextMenuOpening += OnEditorContextMenuOpening;
                    editingElement.TextChanged += TabEditor_TextChanged;
                }
                this._editingTab.PrepareForDisplay();
                this._editingTab = null;
                this.TabsPresenter.InvalidateMeasure();
                this.TabsPresenter.InvalidateArrange();
                if (this.OwningView != null)
                {
                    if (agileCallback == null)
                    {
                        agileCallback = delegate {
                            this.OwningView.FocusInternal();
                        };
                    }
                    await this.OwningView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, agileCallback);
                }
            }
            this.IsEditing = false;
            this.showTextBoxContextMenus = false;
        }

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
        }

        private void TabEditor_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                this.StopTabEditing(false);
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.Escape)
            {
                this.StopTabEditing(true);
            }
        }

        private void TabEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            this.StopTabEditing(false);
        }

        private void TabEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.TabsPresenter.InvalidateMeasure();
            this.TabsPresenter.InvalidateArrange();
        }

        private void TabPresenter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (((e.PropertyName == "StartIndex") && (this.OwningView != null)) && ((this.OwningView.Worksheet != null) && (this.OwningView.Worksheet.Workbook != null)))
            {
                this.OwningView.Worksheet.Workbook.StartSheetIndex = this.TabsPresenter.StartIndex;
            }
        }

        private Windows.Foundation.Point TranslatePoint(Windows.Foundation.Point point, UIElement element)
        {
            return this.OwningView.TransformToVisual(element).TransformPoint(point);
        }

        internal void Update()
        {
            this.StopTabEditing(false);
            foreach (SheetTab tab in this.TabsPresenter.Children)
            {
                tab.Click -= Tab_Click;
                tab.SheetIndex = -1;
                tab.OwningStrip = null;
                tab.IsActive = false;
            }
            this.TabsPresenter.Update();
            this._activeTab = null;
            this._editingTab = null;
        }

        private void UpdateZIndexes()
        {
            int count = this.TabsPresenter.Count;
            if ((count > 0) && (this._activeTab != null))
            {
                for (int i = 0; i < count; i++)
                {
                    if (this.TabsPresenter.Children[i] != this._activeTab)
                    {
                        Canvas.SetZIndex((UIElement)TabsPresenter.Children[i], count - i);
                    }
                }
                Canvas.SetZIndex(this._activeTab, count + 1);
            }
        }

        internal SheetTab ActiveTab
        {
            get { return  this._activeTab; }
        }

        internal bool HasInsertTab
        {
            get { return  this._hasNewTab; }
            set
            {
                Action action = null;
                if (value != this._hasNewTab)
                {
                    this._hasNewTab = value;
                    if (this._hasNewTab)
                    {
                        if (this._newTab == null)
                        {
                            this._newTab = new SheetTab();
                            this._newTab.OwningStrip = this;
                            _newTab.Click += Tab_Click;
                        }
                        if (!this.TabsPresenter.Children.Contains(this._newTab))
                        {
                            this.TabsPresenter.Children.Add(this._newTab);
                        }
                        if (this._root != null)
                        {
                            this.TabsPresenter.Update();
                            this.TabsPresenter.ReCalculateStartIndex(0, this.TabsPresenter.Count - 1);
                        }
                    }
                    else
                    {
                        if (action == null)
                        {
                            action = delegate {
                                this.TabsPresenter.Children.Remove(this._newTab);
                                if (this._newTab != null)
                                {
                                    _newTab.Click -= Tab_Click;
                                    this._newTab = null;
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
            get { return  this._tabStripPresent.TabPresenter; }
        }

        internal Dt.Cells.Data.Workbook Workbook
        {
            get { return  this.OwningView.SpreadSheet.Workbook; }
        }
    }
}

