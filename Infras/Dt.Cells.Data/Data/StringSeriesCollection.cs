#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Provides a container for a SeriesDataCollection of string.
    /// </summary>
    public sealed class StringSeriesCollection : SeriesDataCollection<string>
    {
        IFormatter _formatter;

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="valueIndex">Index of the value.</param>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        protected override string ConvertValue(int valueIndex, object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            if (this.Formatter != null)
            {
                return this.Formatter.Format(obj);
            }
            IFormatter underlyingFormatter = base.GetUnderlyingFormatter(valueIndex);
            if (underlyingFormatter != null)
            {
                return underlyingFormatter.Format(obj);
            }
            return Convert.ToString(obj, (IFormatProvider) CultureInfo.CurrentCulture);
        }

        internal override WorksheetSeriesDataProvider CreateSeriesDataProvider(IDataSeries dataSeries)
        {
            WorksheetSeriesDataProvider provider = base.CreateSeriesDataProvider(dataSeries);
            IDataSeries series1 = base.DataSeries;
            return provider;
        }

        internal IFormatter Formatter
        {
            get { return  this._formatter; }
            set
            {
                if (value != this.Formatter)
                {
                    this._formatter = value;
                    this.UpdateCollection();
                }
            }
        }
    }
}

