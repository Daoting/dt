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
    /// <summary>
    /// 
    /// </summary>
    internal class ArrowSettings : INotifyPropertyChanged
    {
        ArrowSize _length = ArrowSize.Small;
        ArrowType _type;
        ArrowSize _width = ArrowSize.Small;

        /// <summary>
        /// Occurs when [property changed].
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
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public ArrowSize Length
        {
            get { return  this._length; }
            set
            {
                if (value != this.Length)
                {
                    this._length = value;
                    this.RaisePropertyChanged("Length");
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the arrow.
        /// </summary>
        /// <value>
        /// The type of the arrow.
        /// </value>
        public ArrowType Type
        {
            get { return  this._type; }
            set
            {
                if (value != this.Type)
                {
                    this._type = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public ArrowSize Width
        {
            get { return  this._width; }
            set
            {
                if (value != this.Width)
                {
                    this._width = value;
                    this.RaisePropertyChanged("Width");
                }
            }
        }
    }
}

