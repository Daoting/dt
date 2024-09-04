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
    /// Represents data points for XYDataSeries.
    /// </summary>
    public class XYDataPoint : DataPoint, IXYDataPoint, IDataPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.XYDataPoint" /> class.
        /// </summary>
        public XYDataPoint()
        {
        }

        internal XYDataPoint(SpreadXYDataSeries xyDataSeries, int pointIndex) : base(xyDataSeries, pointIndex)
        {
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
                if (((this.XYDataSeries != null) && (base.PointIndex >= 0)) && (base.PointIndex < this.XYDataSeries.XValues.Count))
                {
                    return new double?(this.XYDataSeries.XValues[base.PointIndex]);
                }
                return null;
            }
        }

        SpreadXYDataSeries XYDataSeries
        {
            get { return  (base.DataSeries as SpreadXYDataSeries); }
        }
    }
}

