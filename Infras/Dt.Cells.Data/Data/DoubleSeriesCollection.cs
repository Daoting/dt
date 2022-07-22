#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Provides a container for a SeriesDataCollection of double.
    /// </summary>
    public sealed class DoubleSeriesCollection : SeriesDataCollection<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.DoubleSeriesCollection" /> class.
        /// </summary>
        public DoubleSeriesCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.DoubleSeriesCollection" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public DoubleSeriesCollection(IEnumerable<double> values) : base(values)
        {
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="valueIndex">Index of the value.</param>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        protected override double ConvertValue(int valueIndex, object obj)
        {
            if (obj == null)
            {
                if (base.DataSeries.EmptyValueStyle == EmptyValueStyle.Zero)
                {
                    return 0.0;
                }
                return double.NaN;
            }
            double? nullable = FormatConverter.TryDouble(obj, false);
            if (!nullable.HasValue)
            {
                return double.NaN;
            }
            return nullable.Value;
        }
    }
}

