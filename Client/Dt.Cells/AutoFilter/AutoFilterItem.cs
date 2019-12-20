#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Threading;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents filter item information for dropdown filter's text filter.
    /// </summary>
    public class AutoFilterItem : INotifyPropertyChanged
    {
        private object _criterion;
        private string _displayText;
        private bool? _isChecked = true;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="E:Dt.Cells.UI.AutoFilterItem.PropertyChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets the filter item's value.
        /// </summary>
        public virtual object Criterion
        {
            get { return  this._criterion; }
            set
            {
                if (!object.Equals(value, this._criterion))
                {
                    this._criterion = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Criterion"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter item's displaytext.
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (this.Criterion != null)
                {
                    return this.Criterion.ToString();
                }
                return string.Empty;
            }
            set
            {
                if (this._displayText != value)
                {
                    this._displayText = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("DisplayText"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the value indicates the filter item take effect or not.
        /// </summary>
        public bool? IsChecked
        {
            get { return  this._isChecked; }
            set
            {
                bool? nullable = value;
                bool? nullable2 = this._isChecked;
                if ((nullable.GetValueOrDefault() != nullable2.GetValueOrDefault()) || (nullable.HasValue != nullable2.HasValue))
                {
                    this._isChecked = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
                }
            }
        }
    }
}

