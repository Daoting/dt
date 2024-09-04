#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the text label element displayed for a data point value of the OpenHighLowCloseDataSeries.
    /// </summary>
    public class OpenHighLowCloseDataLabel : DataLabel, IXYDataPoint, IDataPoint
    {
        SpreadOpenHighLowCloseSeries _openHighLowCloseSeries;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.OpenHighLowCloseDataLabel" /> class.
        /// </summary>
        public OpenHighLowCloseDataLabel()
        {
        }

        internal OpenHighLowCloseDataLabel(SpreadDataSeries dataSeries, SpreadOpenHighLowCloseSeries openHighLowCloseSeries, int pointIndex) : base(dataSeries, pointIndex)
        {
            this._openHighLowCloseSeries = openHighLowCloseSeries;
        }

        /// <summary>
        /// Gets the OpenHighLowCloseSeries.
        /// </summary>
        /// <value>
        /// The OpenHighLowCloseSeries.
        /// </value>
        public SpreadOpenHighLowCloseSeries OpenHighLowCloseSeries
        {
            get { return  this._openHighLowCloseSeries; }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public override string Text
        {
            get
            {
                string name = string.Empty;
                string formatedText = string.Empty;
                string str3 = string.Empty;
                DataLabelSettings actualDataLabelSettings = base.ActualDataLabelSettings;
                StringBuilder builder = new StringBuilder();
                if (actualDataLabelSettings != null)
                {
                    if (actualDataLabelSettings.ShowSeriesName)
                    {
                        name = base.DataSeries.Name;
                        builder.Append(name);
                        builder.Append(actualDataLabelSettings.Separator);
                    }
                    if (actualDataLabelSettings.ShowCategoryName)
                    {
                        formatedText = base.GetFormatedText(this.XValue);
                        builder.Append(formatedText);
                        builder.Append(actualDataLabelSettings.Separator);
                    }
                    if (actualDataLabelSettings.ShowValue)
                    {
                        str3 = base.GetFormatedText(base.Value);
                        builder.Append(str3);
                        builder.Append(actualDataLabelSettings.Separator);
                    }
                }
                return builder.ToString().Trim(actualDataLabelSettings.Separator.ToCharArray());
            }
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

