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
        private Button CancelButton { get; set; }
        private ItemsControl ItemsPresenter { get; set; }
        private Button OKButton { get; set; }

        public event EventHandler CancelClick;
        public event EventHandler OKClick;
        private AutoFilterItemCollection _filterItems;
 
        public AutoFilterEditor()
        {
            base.DefaultStyleKey=typeof(AutoFilterEditor);
            base.IsTabStop=false;
        }

        private int GetFocusedItemIndex()
        {
            if (ElementTreeHelper.IsKeyboardFocusWithin(this.ItemsPresenter))
            {
                return 0;
            }
            if (ElementTreeHelper.IsFocused(this.OKButton))
            {
                return 1;
            }
            if (ElementTreeHelper.IsFocused(this.CancelButton))
            {
                return 2;
            }
            return -1;
        }

 
        private void OnAllFilterItemChecked(object sender, EventArgs e)
        {
            if (this.OKButton != null)
            {
                this.OKButton.IsEnabled=true;
            }
        }

        private void OnAllFilterItemUnchecked(object sender, EventArgs e)
        {
            if (this.OKButton != null)
            {
                this.OKButton.IsEnabled=false;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ItemsPresenter = base.GetTemplateChild("ItemsPresenter") as ItemsControl;
            if (this.ItemsPresenter != null)
            {
                this.ItemsPresenter.ItemsSource=this._filterItems;
            }
            this.OKButton = base.GetTemplateChild("OKButton") as Button;
            this.CancelButton = base.GetTemplateChild("CancelButton") as Button;
            if (this.OKButton != null)
            {
                this.OKButton.Content=ResourceStrings.OK;
                if ((this.AutoFilterItems != null) && this.AutoFilterItems.IsAllUnChecked)
                {
                    this.OKButton.IsEnabled = false;
                }
                //hdt
                OKButton.Click += (sender,e)=>
                {
                    OnOKClick();
                };
            }
            if (this.CancelButton != null)
            {
                this.CancelButton.Content = ResourceStrings.Cancel;
                //hdt
                CancelButton.Click +=(sender ,e)=>
                {
                    OnCancelClick();
                };
            }
        }

        private void OnOKClick()
        {
            if (this.OKClick != null)
            {
                this.OKClick(this, EventArgs.Empty);
            }
        }

        private void OnCancelClick()
        {
            if (this.CancelClick != null)
            {
                this.CancelClick(this, EventArgs.Empty);
            }
        } 

        internal void SelectChildElement(bool forward)
        {
            if (forward)
            {
                this.ItemsPresenter.Focus(FocusState.Programmatic);
            }
            else
            {
                this.CancelButton.Focus(FocusState.Programmatic);
            }
        }

        private bool SelectNextItem(bool forward)
        {
            int num = 3;
            int focusedItemIndex = this.GetFocusedItemIndex();
            if ((focusedItemIndex == -1) && !forward)
            {
                focusedItemIndex = num;
            }
            int num3 = focusedItemIndex;
            do
            {
                num3 += forward ? 1 : -1;
                if ((num3 == 0) && this.ItemsPresenter.IsEnabled)
                {
                    this.ItemsPresenter.Focus(FocusState.Programmatic);
                    return true;
                }
                if ((num3 == 1) && this.OKButton.IsEnabled)
                {
                    this.OKButton.Focus(FocusState.Programmatic);
                    return true;
                }
                if ((num3 == 2) && this.CancelButton.IsEnabled)
                {
                    this.CancelButton.Focus(FocusState.Programmatic);
                    return true;
                }
            }
            while ((num3 > 0) && (num3 < num));
            return false;
        }

        internal AutoFilterItemCollection AutoFilterItems
        {
            get { return  this._filterItems; }
            set
            {
                if (!object.Equals(this._filterItems, value))
                {
                    if (this._filterItems != null)
                    {
                        this._filterItems.AllItemUnchecked -= new EventHandler(this.OnAllFilterItemUnchecked);
                        this._filterItems.AllItemChecked -= new EventHandler(this.OnAllFilterItemChecked);
                    }
                    this._filterItems = value;
                    if (this.ItemsPresenter != null)
                    {
                        this.ItemsPresenter.ItemsSource = value;
                    }
                    if (this._filterItems != null)
                    {
                        this._filterItems.AllItemUnchecked += new EventHandler(this.OnAllFilterItemUnchecked);
                        this._filterItems.AllItemChecked += new EventHandler(this.OnAllFilterItemChecked);
                    }
                }
            }
        }
    }
}
