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
    /// Represents the text label element displayed for a data point value of the XYDataSeries.
    /// </summary>
    public class XYDataLabel : DataLabel, IXYDataPoint, IDataPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.XYDataLabel" /> class.
        /// </summary>
        public XYDataLabel()
        {
        }

        internal XYDataLabel(SpreadXYDataSeries xyDataSeries, int pointIndex) : base(xyDataSeries, pointIndex)
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

