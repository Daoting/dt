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
    /// Represents the text label element displayed for a data point value of the BubbleDataSeries.
    /// </summary>
    public class BubbleDataLabel : XYDataLabel, IBubbleDataPoint, IXYDataPoint, IDataPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.BubbleDataLabel" /> class.
        /// </summary>
        public BubbleDataLabel()
        {
        }

        internal BubbleDataLabel(SpreadBubbleSeries bubbleSeries, int pointIndex) : base(bubbleSeries, pointIndex)
        {
        }

        SpreadBubbleSeries BubbleDataSeries
        {
            get { return  (base.DataSeries as SpreadBubbleSeries); }
        }

        /// <summary>
        /// Gets the size value.
        /// </summary>
        /// <value>
        /// The size value.
        /// </value>
        public double? SizeValue
        {
            get
            {
                if (((this.BubbleDataSeries != null) && (base.PointIndex >= 0)) && (base.PointIndex < this.BubbleDataSeries.SizeValues.Count))
                {
                    return new double?(this.BubbleDataSeries.SizeValues[base.PointIndex]);
                }
                return null;
            }
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
                string text = base.Text;
                string formatedText = string.Empty;
                DataLabelSettings actualDataLabelSettings = base.ActualDataLabelSettings;
                StringBuilder builder = new StringBuilder();
                if ((actualDataLabelSettings != null) && actualDataLabelSettings.ShowBubbleSize)
                {
                    formatedText = base.GetFormatedText(this.SizeValue);
                    builder.Append(formatedText);
                    builder.Append(actualDataLabelSettings.Separator);
                }
                return (text + actualDataLabelSettings.Separator + formatedText).Trim(actualDataLabelSettings.Separator.ToCharArray());
            }
        }
    }
}

