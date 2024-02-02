#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.OOXml;
using Dt.Xls.Utils;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// 
    /// </summary>
    public class ExcelTrendLine : IExcelTrendLine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelTrendLine" /> class.
        /// </summary>
        public ExcelTrendLine()
        {
            this.Order = -1;
            this.Period = -1;
            this.DisplayEquation = true;
            this.DisplayRSquaredValue = true;
            this.TrendlineType = ExcelTrendLineType.Linear;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "name")
                {
                    this.Name = element.Value;
                }
                else if (element.Name.LocalName == "trendlineType")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "linear"))
                    {
                        case "exp":
                            this.TrendlineType = ExcelTrendLineType.Exponential;
                            break;

                        case "linear":
                            this.TrendlineType = ExcelTrendLineType.Linear;
                            break;

                        case "log":
                            this.TrendlineType = ExcelTrendLineType.Logarithmic;
                            break;

                        case "movingAvg":
                            this.TrendlineType = ExcelTrendLineType.MovingAverage;
                            break;

                        case "poly":
                            this.TrendlineType = ExcelTrendLineType.Polynomial;
                            break;

                        case "power":
                            this.TrendlineType = ExcelTrendLineType.Power;
                            break;
                    }
                }
                else if (element.Name.LocalName == "order")
                {
                    this.Order = element.GetAttributeValueOrDefaultOfInt32Type("val", 2);
                }
                else if (element.Name.LocalName == "period")
                {
                    this.Period = element.GetAttributeValueOrDefaultOfInt32Type("val", 2);
                }
                else if (element.Name.LocalName == "forward")
                {
                    this.Forward = element.GetAttributeValueOrDefaultOfDoubleType("val", 0.0);
                }
                else if (element.Name.LocalName == "backward")
                {
                    this.Backward = element.GetAttributeValueOrDefaultOfDoubleType("val", 0.0);
                }
                else if (element.Name.LocalName == "intercept")
                {
                    this.Intercept = new double?(element.GetAttributeValueOrDefaultOfDoubleType("val", 0.0));
                }
                else if (element.Name.LocalName == "dispRSqr")
                {
                    this.DisplayRSquaredValue = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "dispEq")
                {
                    this.DisplayEquation = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "trendlineLbl")
                {
                    ExcelTrendLineLabel label = new ExcelTrendLineLabel();
                    label.ReadXml(element, mFolder, xFile);
                    this.TrendLineLabel = label;
                }
                else if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format = new ExcelChartFormat();
                    format.ReadXml(element, mFolder, xFile);
                    this.Foramt = format;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "trendline", null);
            if (this.Foramt != null)
            {
                (this.Foramt as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
            }
            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                writer.WriteElementString("c", "name", null, this.Name);
            }
            string str = "linear";
            switch (this.TrendlineType)
            {
                case ExcelTrendLineType.Exponential:
                    str = "exp";
                    break;

                case ExcelTrendLineType.Linear:
                    str = "linear";
                    break;

                case ExcelTrendLineType.Logarithmic:
                    str = "log";
                    break;

                case ExcelTrendLineType.MovingAverage:
                    str = "movingAvg";
                    break;

                case ExcelTrendLineType.Polynomial:
                    str = "poly";
                    break;

                case ExcelTrendLineType.Power:
                    str = "power";
                    break;
            }
            writer.WriteLeafElementWithAttribute("trendlineType", null, "c", "val", str);
            if (this.Order != -1)
            {
                writer.WriteLeafElementWithAttribute("order", null, "c", "val", ((int) this.Order).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (this.Period != -1)
            {
                writer.WriteLeafElementWithAttribute("period", null, "c", "val", ((int) this.Period).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (!this.Forward.IsZero())
            {
                writer.WriteLeafElementWithAttribute("forward", null, "c", "val", ((double) this.Forward).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (!this.Backward.IsZero())
            {
                writer.WriteLeafElementWithAttribute("backward", null, "c", "val", ((double) this.Backward).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if ((((this.TrendlineType == ExcelTrendLineType.Exponential) || (this.TrendlineType == ExcelTrendLineType.Linear)) || (this.TrendlineType == ExcelTrendLineType.Polynomial)) && this.Intercept.HasValue)
            {
                writer.WriteLeafElementWithAttribute("intercept", null, "c", "val", ((double) this.Intercept.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            writer.WriteLeafElementWithAttribute("dispRSqr", null, "c", "val", this.DisplayRSquaredValue ? "1" : "0");
            writer.WriteLeafElementWithAttribute("dispEq", null, "c", "val", this.DisplayEquation ? "1" : "0");
            if (this.TrendLineLabel != null)
            {
                this.TrendLineLabel.WriteXml(writer, mFolder, chartFile);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Specifies the number of categories (or units on a scatter chart) that the trend line extends before the data for the series that is being trended.
        /// On no-scatter charts, the value shall be 0 or 0.5.
        /// </summary>
        public double Backward { get; set; }

        /// <summary>
        /// Specifies that the equation for the trendline is displayed on the chart.
        /// </summary>
        public bool DisplayEquation { get; set; }

        /// <summary>
        /// Specifies that the R-squared value of the trendline is displayed on the chart.
        /// </summary>
        public bool DisplayRSquaredValue { get; set; }

        /// <summary>
        /// Specifies the formatting for the Trendline
        /// </summary>
        public IExcelChartFormat Foramt { get; set; }

        /// <summary>
        /// Specifies the number of categories (or units on a scatter chart) that the trend line extends after the data for the series that is being trended.
        /// </summary>
        public double Forward { get; set; }

        /// <summary>
        /// Specifies the value where the trendline shall cross the y axis.
        /// </summary>
        public double? Intercept { get; set; }

        /// <summary>
        /// Specifies the name of the trendline.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// Specified the period of the trend line for a moving average trend line.
        /// </summary>
        /// <remarks>
        /// It should be ignored for other trend line types.
        /// </remarks>
        public int Period { get; set; }

        /// <summary>
        /// Specifies the label for the trendline
        /// </summary>
        public ExcelTrendLineLabel TrendLineLabel { get; set; }

        /// <summary>
        /// Specifies the style of the trendline.
        /// </summary>
        public ExcelTrendLineType TrendlineType { get; set; }
    }
}

