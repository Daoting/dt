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
    /// Represents the text label element displayed for a data point value of the XYZDataSeries.
    /// </summary>
    public class XYZDataLabel : XYDataLabel, IXYZDataPoint, IXYDataPoint, IDataPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.XYDataLabel" /> class.
        /// </summary>
        public XYZDataLabel()
        {
        }

        internal XYZDataLabel(SpreadXYZDataSeries xyzDataSeries, int pointIndex) : base(xyzDataSeries, pointIndex)
        {
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public override string Text
        {
            get { return  base.Text; }
        }

        SpreadXYZDataSeries XYZDataSeries
        {
            get { return  (base.DataSeries as SpreadXYZDataSeries); }
        }

        /// <summary>
        /// Gets the Z value.
        /// </summary>
        /// <value>
        /// The Z value.
        /// </value>
        public double? ZValue
        {
            get
            {
                if (((this.XYZDataSeries != null) && (base.PointIndex >= 0)) && (base.PointIndex < this.XYZDataSeries.ZValues.Count))
                {
                    return new double?(this.XYZDataSeries.ZValues[base.PointIndex]);
                }
                return null;
            }
        }
    }
}

