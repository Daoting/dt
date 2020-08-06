#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    internal class DataTableSettings : INotifyPropertyChanged
    {
        bool _showHorizontalBorder = true;
        bool _showLegendKeys;
        bool _showOutlineBorder = true;
        bool _showVerticalBorder = true;

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Indicates whether to show the horizontal border in the data table.
        /// </summary>
        public bool ShowHorizontalBorder
        {
            get { return  this._showHorizontalBorder; }
            set
            {
                if (this._showHorizontalBorder != value)
                {
                    this._showHorizontalBorder = value;
                    this.RaisePropertyChanged("ShowHorizontalBorder");
                }
            }
        }

        /// <summary>
        /// Indicates whether to show the legend keys in the data table.
        /// </summary>
        public bool ShowLegendKeys
        {
            get { return  this._showLegendKeys; }
            set
            {
                if (this._showLegendKeys != value)
                {
                    this._showLegendKeys = value;
                    this.RaisePropertyChanged("ShowLegendKeys");
                }
            }
        }

        /// <summary>
        /// Indicates whether to show the outline border in the data table.
        /// </summary>
        public bool ShowOutlineBorder
        {
            get { return  this._showOutlineBorder; }
            set
            {
                if (this._showOutlineBorder != value)
                {
                    this._showOutlineBorder = value;
                    this.RaisePropertyChanged("ShowOutlineBorder");
                }
            }
        }

        /// <summary>
        /// Indicates whether to show the vertical border in the data table.
        /// </summary>
        public bool ShowVerticalBorder
        {
            get { return  this._showVerticalBorder; }
            set
            {
                if (this._showVerticalBorder != value)
                {
                    this._showVerticalBorder = value;
                    this.RaisePropertyChanged("ShowVerticalBorder");
                }
            }
        }
    }
}

