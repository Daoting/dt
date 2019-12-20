#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
#endregion

namespace Dt.Cells.UI
{
    internal class AutoFilterItemCollection : Collection<AutoFilterItem>
    {
        private bool _suspendItemPropertyChanged;

        internal event EventHandler AllItemChecked;

        internal event EventHandler AllItemUnchecked;

        protected override void InsertItem(int index, AutoFilterItem item)
        {
            base.InsertItem(index, item);
            item.PropertyChanged += new PropertyChangedEventHandler(this.OnItemPropertyChanged);
        }

        private void OnAllItemChecked()
        {
            if (this.AllItemChecked != null)
            {
                this.AllItemChecked(this, EventArgs.Empty);
            }
        }

        private void OnAllItemUnchecked()
        {
            if (this.AllItemUnchecked != null)
            {
                this.AllItemUnchecked(this, EventArgs.Empty);
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!this._suspendItemPropertyChanged)
            {
                try
                {
                    this._suspendItemPropertyChanged = true;
                    if (e.PropertyName == "IsChecked")
                    {
                        if (base.IndexOf(sender as AutoFilterItem) == 0)
                        {
                            for (int i = 1; i < base.Count; i++)
                            {
                                base.Items[i].IsChecked = base.Items[0].IsChecked;
                            }
                            if (base.Items[0].IsChecked.HasValue && !base.Items[0].IsChecked.Value)
                            {
                                this.OnAllItemUnchecked();
                            }
                            else
                            {
                                this.OnAllItemChecked();
                            }
                        }
                        else
                        {
                            bool flag = false;
                            bool flag2 = false;
                            for (int j = 1; j < base.Count; j++)
                            {
                                if (base.Items[j].IsChecked.Value)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    flag2 = true;
                                }
                                if (flag && flag2)
                                {
                                    break;
                                }
                            }
                            if (flag && flag2)
                            {
                                base.Items[0].IsChecked = true;
                                base.Items[0].IsChecked = null;
                                this.OnAllItemChecked();
                            }
                            else
                            {
                                if (flag)
                                {
                                    base.Items[0].IsChecked = false;
                                    base.Items[0].IsChecked = true;
                                    this.OnAllItemChecked();
                                }
                                if (flag2)
                                {
                                    base.Items[0].IsChecked = true;
                                    base.Items[0].IsChecked = false;
                                    this.OnAllItemUnchecked();
                                }
                            }
                        }
                    }
                }
                finally
                {
                    this._suspendItemPropertyChanged = false;
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            base.Items[index].PropertyChanged -= new PropertyChangedEventHandler(this.OnItemPropertyChanged);
            base.RemoveItem(index);
        }

        internal void ResumeItemPropertyChanged()
        {
            this._suspendItemPropertyChanged = false;
        }

        protected override void SetItem(int index, AutoFilterItem item)
        {
            base.Items[index].PropertyChanged -= new PropertyChangedEventHandler(this.OnItemPropertyChanged);
            base.SetItem(index, item);
            base.Items[index].PropertyChanged += new PropertyChangedEventHandler(this.OnItemPropertyChanged);
        }

        internal void SuspendItemPropertyChanged()
        {
            this._suspendItemPropertyChanged = true;
        }

        internal bool IsAllUnChecked
        {
            get
            {
                if ((base.Count > 0) && base.Items[0].IsChecked.HasValue)
                {
                    return !base.Items[0].IsChecked.Value;
                }
                return false;
            }
        }
    }
}

