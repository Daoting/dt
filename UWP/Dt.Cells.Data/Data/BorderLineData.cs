#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents data for the border line.
    /// </summary>
    public sealed class BorderLineData
    {
        int _drawingThickness;
        DoubleCollection _farDash;
        int _farWidth;
        DoubleCollection _middleDash;
        int _middleWidth;
        DoubleCollection _nearDash;
        int _nearWidth;
        int _strokeDashOffset;
        int _weight;

        internal BorderLineData(int weight, int widthFar, DoubleCollection dashFar, int widthMiddle, DoubleCollection dashMiddle, int widthNear, DoubleCollection dashNear)
        {
            this._weight = weight;
            this._farWidth = widthFar;
            this._farDash = dashFar;
            this._middleWidth = widthMiddle;
            this._middleDash = dashMiddle;
            this._nearWidth = widthNear;
            this._nearDash = dashNear;
            this._strokeDashOffset = 0;
            this._drawingThickness = 1;
            this.Linkable = true;
        }

        /// <summary>
        /// Gets the border line count. 
        /// </summary>
        public int DrawingThickness
        {
            get { return  this._drawingThickness; }
            internal set { this._drawingThickness = value; }
        }

        /// <summary>
        /// Gets dash information of the far line.
        /// </summary>
        public DoubleCollection FarDash
        {
            get { return  this._farDash; }
        }

        /// <summary>
        /// Gets the line width of the far line.
        /// </summary>
        public int FarWidth
        {
            get { return  this._farWidth; }
        }

        /// <summary>
        /// Gets a value that indicates whether the border line has a far line.
        /// </summary>
        public bool HasFar
        {
            get { return  (this._farWidth > 0); }
        }

        /// <summary>
        /// Gets a value that indicates whether the border line has a middle line.
        /// </summary>
        public bool HasMiddle
        {
            get { return  (this._middleWidth > 0); }
        }

        /// <summary>
        /// Gets a value that indicates whether the border line has a near line.
        /// </summary>
        public bool HasNear
        {
            get { return  (this._nearWidth > 0); }
        }

        /// <summary>
        /// Gets a value that indicates whether the border can be linked when drawing.
        /// </summary>
        /// <value>
        /// <c>true</c> if the border can be linked; otherwise, <c>false</c>.
        /// </value>
        public bool Linkable { get; internal set; }

        /// <summary>
        /// Gets dash information of the middle line.
        /// </summary>
        public DoubleCollection MiddleDash
        {
            get { return  this._middleDash; }
        }

        /// <summary>
        /// Gets the line width of the middle line.
        /// </summary>
        public int MiddleWidth
        {
            get { return  this._middleWidth; }
        }

        /// <summary>
        /// Gets dash information of the near line.
        /// </summary>
        public DoubleCollection NearDash
        {
            get { return  this._nearDash; }
        }

        /// <summary>
        /// Gets the line width of the near line.
        /// </summary>
        public int NearWidth
        {
            get { return  this._nearWidth; }
        }

        /// <summary>
        /// Gets the stroke dash offset in the border line data.
        /// </summary>
        public int StrokeDashOffset
        {
            get { return  this._strokeDashOffset; }
            set { this._strokeDashOffset = value; }
        }

        /// <summary>
        /// Gets the border thickness. 
        /// </summary>
        public int Thickness
        {
            get { return  ((this._farWidth + this._middleWidth) + this._nearWidth); }
        }

        /// <summary>
        /// Gets the border weight, the smaller is covered by the larger.
        /// </summary>
        public int Weight
        {
            get { return  this._weight; }
        }
    }
}

