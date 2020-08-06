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
    internal class BubbleChartSettings : INotifyPropertyChanged
    {
        double _bubbleScale = 1.0;
        bool _showNegativeBubble;
        Dt.Cells.Data.BubbleSizeRepresents _sizeRepresents = Dt.Cells.Data.BubbleSizeRepresents.Width;

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
        /// Gets or sets the bubble scale.
        /// </summary>
        /// <value>
        /// The bubble scale.
        /// </value>
        /// <exception cref="T:System.ArgumentException">value must be between 0 and 3!</exception>
        public double BubbleScale
        {
            get { return  this._bubbleScale; }
            set
            {
                if ((value < 0.0) || (value > 3.0))
                {
                    throw new ArgumentException("value must be between 0 and 3!");
                }
                if (value != this.BubbleScale)
                {
                    this._bubbleScale = value;
                    this.RaisePropertyChanged("BubbleScale");
                }
            }
        }

        /// <summary>
        /// Gets or sets the bubble size representation.
        /// </summary>
        /// <value>
        /// The bubble size representation.
        /// </value>
        public Dt.Cells.Data.BubbleSizeRepresents BubbleSizeRepresents
        {
            get { return  this._sizeRepresents; }
            set
            {
                if (value != this.BubbleSizeRepresents)
                {
                    this._sizeRepresents = value;
                    this.RaisePropertyChanged("BubbleSizeRepresents");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to show a negative bubble.
        /// </summary>
        /// <value>
        /// <c>true</c> to show a negative bubble; otherwise, <c>false</c>.
        /// </value>
        public bool ShowNegativeBubble
        {
            get { return  this._showNegativeBubble; }
            set
            {
                if (value != this.ShowNegativeBubble)
                {
                    this._showNegativeBubble = value;
                    this.RaisePropertyChanged("ShowNegativeBubble");
                }
            }
        }
    }
}

