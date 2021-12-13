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
    internal class PieChartSettings : INotifyPropertyChanged
    {
        double _explosion = 0.25;
        int _firstSliceAngle;
        double _holeSize = 0.1;

        /// <summary>
        /// Occurs when the property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets or sets the explosion.
        /// </summary>
        /// <value>
        /// The explosion.
        /// </value>
        /// <exception cref="T:System.ArgumentException">value must be between 0 and 4!</exception>
        public double Explosion
        {
            get { return  this._explosion; }
            set
            {
                if ((value < 0.0) || (value > 4.0))
                {
                    throw new ArgumentException("value must be between 0 and 4!");
                }
                if (value != this.Explosion)
                {
                    this._explosion = value;
                    this.RaisePropertyChanged("Explosion");
                }
            }
        }

        /// <summary>
        /// Gets or sets the first slice angle.
        /// </summary>
        /// <value>
        /// The first slice angle.
        /// </value>
        /// <exception cref="T:System.ArgumentException">value must be between 0 and 360!</exception>
        public int FirstSliceAngle
        {
            get { return  this._firstSliceAngle; }
            set
            {
                if ((value < 0) || (value > 360))
                {
                    throw new ArgumentException("value must be between 0 and 360!");
                }
                if (value != this.FirstSliceAngle)
                {
                    this._firstSliceAngle = value;
                    this.RaisePropertyChanged("FirstSliceAngle");
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the hole.
        /// </summary>
        /// <value>
        /// The size of the hole.
        /// </value>
        public double HoleSize
        {
            get { return  this._holeSize; }
            set
            {
                if (value != this.HoleSize)
                {
                    this._holeSize = value;
                    this.RaisePropertyChanged("HoleSize");
                }
            }
        }
    }
}

