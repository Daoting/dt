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
using System.Collections.Generic;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a filter dropdown dialog's text filter item.
    /// </summary>
    public partial class AutoFilterDropDownItemControl : DropDownItemBaseControl
    {
        AutoFilterEditor _autoFilterEditor;
        AutoFilterItemCollection _filterItems;

        /// <summary>
        /// Creates a new instance of the <see cref="T:AutoFilterDropDownItemControl" /> class.
        /// </summary>
        public AutoFilterDropDownItemControl()
        {
            EventHandler handler = null;
            EventHandler handler2 = null;
            base.DefaultStyleKey = typeof(AutoFilterDropDownItemControl);
            _filterItems = new AutoFilterItemCollection();
            _autoFilterEditor = new AutoFilterEditor();
            _autoFilterEditor.AutoFilterItems = _filterItems;
            base.Content = _autoFilterEditor;
            if (handler == null)
            {
                handler = delegate (object param0, EventArgs param1) {
                    if (!FilterItems[0].IsChecked.HasValue || FilterItems[0].IsChecked.Value)
                    {
                        if (base.ParentDropDownList != null)
                        {
                            base.ParentDropDownList.Close();
                        }
                        ExecuteCommand();
                    }
                };
            }
            _autoFilterEditor.OKClick += handler;
            if (handler2 == null)
            {
                handler2 = delegate (object param0, EventArgs param1) {
                    if (base.ParentDropDownList != null)
                    {
                        base.ParentDropDownList.Close();
                    }
                };
            }
            _autoFilterEditor.CancelClick += handler2;
        }

        /// <summary>
        /// Determines whether this the command can be executed on the control.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the command can executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecuteCommand()
        {
            if (((FilterItems.Count > 0) && FilterItems[0].IsChecked.HasValue) && !FilterItems[0].IsChecked.Value)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Executes the command on the control.
        /// </summary>
        public override void ExecuteCommand()
        {
            base.CommandParameter = GetFilterCriterion();
            base.ExecuteCommand();
        }

        object[] GetFilterCriterion()
        {
            if (FilterItems[0].IsChecked.HasValue && FilterItems[0].IsChecked.Value)
            {
                return null;
            }
            List<object> list = new List<object>();
            for (int i = 1; i < FilterItems.Count; i++)
            {
                if (FilterItems[i].IsChecked.HasValue && FilterItems[i].IsChecked.Value)
                {
                    list.Add(FilterItems[i].Criterion);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.UIElement.GotFocus" /> event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }

        internal override void OnIsSelectedChanged()
        {
            if (!base.IsSelected)
            {
                base.ParentDropDownList.Focus(FocusState.Programmatic);
            }
        }

        void ProcessGotFocus()
        {
            int m = base.ParentDropDownList.IndexFromContainer(this);
            int num = base.ParentDropDownList.IndexFromContainer(this);
            if (num != base.ParentDropDownList.SelectedItemIndex)
            {
                base.ParentDropDownList.SelectedItemIndex = num;
            }
        }

        internal override void SelectChild(bool forward)
        {
            base.SelectChild(forward);
            _autoFilterEditor.SelectChildElement(forward);
        }

        internal AutoFilterItemCollection FilterItems
        {
            get { return  _filterItems; }
        }
    }
}

