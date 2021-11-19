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
    internal class DataSeriesSettings : INotifyPropertyChanged
    {
        double _gapDepth = double.NaN;
        double _gapWidth = double.NaN;
        double _seriesOverlap = double.NaN;

        /// <summary>
        /// Occurs when the property has changed.
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
        /// Gets or sets the width of the gap.
        /// </summary>
        /// <value>
        /// The width of the gap.
        /// </value>
        public double GapDepth
        {
            get { return  this._gapDepth; }
            set
            {
                if (value != this.GapDepth)
                {
                    if ((value < 0.0) || (value > 5.0))
                    {
                        throw new ArgumentOutOfRangeException("The value must between 0 and 5!");
                    }
                    this._gapDepth = value;
                    this.RaisePropertyChanged("GapDepth");
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the gap.
        /// </summary>
        /// <value>
        /// The width of the gap.
        /// </value>
        public double GapWidth
        {
            get { return  this._gapWidth; }
            set
            {
                if (value != this.GapWidth)
                {
                    if ((value < 0.0) || (value > 5.0))
                    {
                        throw new ArgumentOutOfRangeException("The value must between 0 and 5!");
                    }
                    this._gapWidth = value;
                    this.RaisePropertyChanged("GapWidth");
                }
            }
        }

        /// <summary>
        /// Gets or sets the series overlap.
        /// </summary>
        /// <value>
        /// The series overlap.
        /// </value>
        public double SeriesOverlap
        {
            get { return  this._seriesOverlap; }
            set
            {
                if (value != this.SeriesOverlap)
                {
                    if ((value < -1.0) || (value > 1.0))
                    {
                        throw new ArgumentOutOfRangeException("The value must between -1 and 1!");
                    }
                    this._seriesOverlap = value;
                    this.RaisePropertyChanged("SeriesOverlap");
                }
            }
        }
    }
}

