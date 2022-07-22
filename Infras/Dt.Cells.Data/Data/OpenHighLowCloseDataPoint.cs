#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents data points for OpenHighLowCloseDataSeries.
    /// </summary>
    public class OpenHighLowCloseDataPoint : DataPoint, IXYDataPoint, IDataPoint
    {
        SpreadOpenHighLowCloseSeries _openHighLowCloseSeries;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.OpenHighLowCloseDataMarker" /> class.
        /// </summary>
        public OpenHighLowCloseDataPoint()
        {
        }

        internal OpenHighLowCloseDataPoint(SpreadDataSeries dataSeries, SpreadOpenHighLowCloseSeries openHighLowCloseSeries, int pointIndex) : base(dataSeries, pointIndex)
        {
            this._openHighLowCloseSeries = openHighLowCloseSeries;
        }

        /// <summary>
        /// Gets the OpenHighLowCloseSeries series.
        /// </summary>
        /// <value>
        /// The OpenHighLowCloseSeries series.
        /// </value>
        public SpreadOpenHighLowCloseSeries OpenHighLowCloseSeries
        {
            get { return  this._openHighLowCloseSeries; }
        }

        /// <summary>
        /// Gets the X value.
        /// </summary>
        /// <value>
        /// The X value.
        /// </value>
        public double? XValue
        {
            get
            {
                if (((this.OpenHighLowCloseSeries != null) && (base.PointIndex >= 0)) && (base.PointIndex < this.OpenHighLowCloseSeries.XValues.Count))
                {
                    return new double?(this.OpenHighLowCloseSeries.XValues[base.PointIndex]);
                }
                return null;
            }
        }
    }
}

