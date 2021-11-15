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
    /// Defines how you want the 3D chart to look.
    /// </summary>
    public class View3DSettings : INotifyPropertyChanged
    {
        double _depthPercent = 100.0;
        double _heightPercent = 100.0;
        double _perspective = 30.0;
        bool _rightAngleAxes = true;
        double _rotationX;
        double _rotationY;
        double _rotationZ;

        /// <summary>
        /// Occurs when the property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets or sets the depth percent.
        /// </summary>
        /// <value>
        /// The depth percent.
        /// </value>
        internal double DepthPercent
        {
            get { return  this._depthPercent; }
            set
            {
                if ((value < 20.0) || (value > 2000.0))
                {
                    throw new ArgumentOutOfRangeException("DepthPercent");
                }
                if (value != this.DepthPercent)
                {
                    this._depthPercent = value;
                    this.RaisePropertyChanged("DepthPercent");
                }
            }
        }

        /// <summary>
        /// Gets or sets the height percent.
        /// </summary>
        /// <value>
        /// The height percent.
        /// </value>
        internal double HeightPercent
        {
            get { return  this._heightPercent; }
            set
            {
                if ((value < 5.0) || (value > 500.0))
                {
                    throw new ArgumentOutOfRangeException("HeightPercent");
                }
                if (value != this.HeightPercent)
                {
                    this._heightPercent = value;
                    this.RaisePropertyChanged("HeightPercent");
                }
            }
        }

        /// <summary>
        /// Gets or sets the perspective.
        /// </summary>
        /// <value>
        /// The perspective.
        /// </value>
        public double Perspective
        {
            get { return  this._perspective; }
            set
            {
                if ((value < 0.0) || (value > 100.0))
                {
                    throw new ArgumentOutOfRangeException("Perspective");
                }
                if (value != this.Perspective)
                {
                    this._perspective = value;
                    this.RaisePropertyChanged("Perspective");
                }
            }
        }

        /// <summary>
        /// specifies that the chart axes are at the right angle, rather than drawn in perspective. applies only to 3-D charts.
        /// </summary>
        internal bool RightAngleAxes
        {
            get { return  this._rightAngleAxes; }
            set
            {
                if (this._rightAngleAxes != value)
                {
                    this._rightAngleAxes = value;
                    this.RaisePropertyChanged("RightAngleAxes");
                }
            }
        }

        /// <summary>
        /// Gets or sets the x rotation.
        /// </summary>
        /// <value>
        /// The rotation X.
        /// </value>
        public double RotationX
        {
            get { return  this._rotationX; }
            set
            {
                if ((value > 360.0) || (value < 0.0))
                {
                    throw new ArgumentOutOfRangeException("RotationX");
                }
                if (value != this.RotationX)
                {
                    this._rotationX = value;
                    this.RaisePropertyChanged("RotationX");
                }
            }
        }

        /// <summary>
        /// Gets or sets the y rotation.
        /// </summary>
        /// <value>
        /// The rotation Y.
        /// </value>
        public double RotationY
        {
            get { return  this._rotationY; }
            set
            {
                if ((value > 90.0) || (value < -90.0))
                {
                    throw new ArgumentOutOfRangeException("YRotation");
                }
                if (value != this.RotationY)
                {
                    this._rotationY = value;
                    this.RaisePropertyChanged("RotationY");
                }
            }
        }

        /// <summary>
        /// Gets or sets the z rotation.
        /// </summary>
        /// <value>
        /// The rotation Z.
        /// </value>
        public double RotationZ
        {
            get { return  this._rotationZ; }
            set
            {
                if ((value < 0.0) || (value > 100.0))
                {
                    throw new ArgumentOutOfRangeException("RotationZ");
                }
                if (value != this.RotationZ)
                {
                    this._rotationZ = value;
                    this.RaisePropertyChanged("RotationZ");
                }
            }
        }
    }
}

