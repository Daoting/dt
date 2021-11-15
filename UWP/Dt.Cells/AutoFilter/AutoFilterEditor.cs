using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dt.Cells.UI
{
    public partial class AutoFilterEditor :Control
    {
        Button CancelButton { get; set; }
        ItemsControl ItemsPresenter { get; set; }
        Button OKButton { get; set; }

        public event EventHandler CancelClick;
        public event EventHandler OKClick;
        AutoFilterItemCollection _filterItems;
 
        public AutoFilterEditor()
        {
            base.DefaultStyleKey=typeof(AutoFilterEditor);
            base.IsTabStop=false;
        }

        int GetFocusedItemIndex()
        {
            if (ElementTreeHelper.IsKeyboardFocusWithin(ItemsPresenter))
            {
                return 0;
            }
            if (ElementTreeHelper.IsFocused(OKButton))
            {
                return 1;
            }
            if (ElementTreeHelper.IsFocused(CancelButton))
            {
                return 2;
            }
            return -1;
        }

 
        void OnAllFilterItemChecked(object sender, EventArgs e)
        {
            if (OKButton != null)
            {
                OKButton.IsEnabled=true;
            }
        }

        void OnAllFilterItemUnchecked(object sender, EventArgs e)
        {
            if (OKButton != null)
            {
                OKButton.IsEnabled=false;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemsPresenter = base.GetTemplateChild("ItemsPresenter") as ItemsControl;
            if (ItemsPresenter != null)
            {
                ItemsPresenter.ItemsSource=_filterItems;
            }
            OKButton = base.GetTemplateChild("OKButton") as Button;
            CancelButton = base.GetTemplateChild("CancelButton") as Button;
            if (OKButton != null)
            {
                OKButton.Content=ResourceStrings.OK;
                if ((AutoFilterItems != null) && AutoFilterItems.IsAllUnChecked)
                {
                    OKButton.IsEnabled = false;
                }
                //hdt
                OKButton.Click += (sender,e)=>
                {
                    OnOKClick();
                };
            }
            if (CancelButton != null)
            {
                CancelButton.Content = ResourceStrings.Cancel;
                //hdt
                CancelButton.Click +=(sender ,e)=>
                {
                    OnCancelClick();
                };
            }
        }

        void OnOKClick()
        {
            if (OKClick != null)
            {
                OKClick(this, EventArgs.Empty);
            }
        }

        void OnCancelClick()
        {
            if (CancelClick != null)
            {
                CancelClick(this, EventArgs.Empty);
            }
        } 

        internal void SelectChildElement(bool forward)
        {
            if (forward)
            {
                ItemsPresenter.Focus(FocusState.Programmatic);
            }
            else
            {
                CancelButton.Focus(FocusState.Programmatic);
            }
        }

        bool SelectNextItem(bool forward)
        {
            int num = 3;
            int focusedItemIndex = GetFocusedItemIndex();
            if ((focusedItemIndex == -1) && !forward)
            {
                focusedItemIndex = num;
            }
            int num3 = focusedItemIndex;
            do
            {
                num3 += forward ? 1 : -1;
                if ((num3 == 0) && ItemsPresenter.IsEnabled)
                {
                    ItemsPresenter.Focus(FocusState.Programmatic);
                    return true;
                }
                if ((num3 == 1) && OKButton.IsEnabled)
                {
                    OKButton.Focus(FocusState.Programmatic);
                    return true;
                }
                if ((num3 == 2) && CancelButton.IsEnabled)
                {
                    CancelButton.Focus(FocusState.Programmatic);
                    return true;
                }
            }
            while ((num3 > 0) && (num3 < num));
            return false;
        }

        internal AutoFilterItemCollection AutoFilterItems
        {
            get { return  _filterItems; }
            set
            {
                if (!object.Equals(_filterItems, value))
                {
                    if (_filterItems != null)
                    {
                        _filterItems.AllItemUnchecked -= new EventHandler(OnAllFilterItemUnchecked);
                        _filterItems.AllItemChecked -= new EventHandler(OnAllFilterItemChecked);
                    }
                    _filterItems = value;
                    if (ItemsPresenter != null)
                    {
                        ItemsPresenter.ItemsSource = value;
                    }
                    if (_filterItems != null)
                    {
                        _filterItems.AllItemUnchecked += new EventHandler(OnAllFilterItemUnchecked);
                        _filterItems.AllItemChecked += new EventHandler(OnAllFilterItemChecked);
                    }
                }
            }
        }
    }
}
