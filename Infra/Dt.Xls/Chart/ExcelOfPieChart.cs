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
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Represents an excel bar of pie or pie of pie chart.
    /// </summary>
    public class ExcelOfPieChart : IExcelOfPieChart, IExcelPieChartBase, IExcelChartBase
    {
        private List<int> customSplitPoints;
        private int gapWidth;
        private List<IExcelPieSeries> pieSeries;
        private int secondPieSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelOfPieChart" /> class.
        /// </summary>
        public ExcelOfPieChart() : this(ExcelChartType.PieOfPie)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelOfPieChart" /> class.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        /// <exception cref="T:System.ArgumentException">The specified chart type is not compitible with pie of pie or bar of pie chart.;chartType</exception>
        public ExcelOfPieChart(ExcelChartType chartType)
        {
            this.gapWidth = 150;
            this.secondPieSize = 0x4b;
            if ((chartType != ExcelChartType.PieOfPie) && (chartType != ExcelChartType.BarOfPie))
            {
                throw new ArgumentException("The specified chart type is not compitible with pie of pie or bar of pie chart.", "chartType");
            }
            this.ChartType = chartType;
            this.VaryColors = true;
            this.GapWidth = 150;
            this.SecondPieSize = 0x4b;
            this.SplitType = OfPieChartSplitType.Auto;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "ofPieType")
                {
                    if (element.GetAttributeValueOrDefaultOfStringType("val", "pie") == "pie")
                    {
                        this.ChartType = ExcelChartType.PieOfPie;
                    }
                    else
                    {
                        this.ChartType = ExcelChartType.BarOfPie;
                    }
                }
                else if (element.Name.LocalName == "varyColors")
                {
                    this.VaryColors = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "ser")
                {
                    ExcelPieSeries series = new ExcelPieSeries();
                    series.ReadXml(element, mFolder, xFile);
                    this.PieSeries.Add(series);
                }
                else if (element.Name.LocalName == "dLbls")
                {
                    ExcelDataLabels labels = new ExcelDataLabels();
                    labels.ReadXml(element, mFolder, xFile);
                    this.DataLabels = labels;
                }
                else if (element.Name.LocalName == "gapWidth")
                {
                    this.GapWidth = element.GetAttributeValueOrDefaultOfInt32Type("val", 150);
                }
                else if (element.Name.LocalName == "splitType")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "auto"))
                    {
                        case "auto":
                            this.SplitType = OfPieChartSplitType.Auto;
                            break;

                        case "cust":
                            this.SplitType = OfPieChartSplitType.Custom;
                            break;

                        case "percent":
                            this.SplitType = OfPieChartSplitType.Percent;
                            break;

                        case "pos":
                            this.SplitType = OfPieChartSplitType.Position;
                            break;

                        case "val":
                            this.SplitType = OfPieChartSplitType.Value;
                            break;
                    }
                }
                else if (element.Name.LocalName == "splitPos")
                {
                    this.SplitPosition = element.GetAttributeValueOrDefaultOfDoubleType("val", 0.0);
                }
                else if (element.Name.LocalName == "custSplit")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        this.CustomSplitPoints.Add(element2.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                    }
                }
                else if (element.Name.LocalName == "secondPieSize")
                {
                    this.SecondPieSize = element.GetAttributeValueOrDefaultOfInt32Type("val", 0x4b);
                }
                else if (element.Name.LocalName == "serLines")
                {
                    this.SeriesLines = new ExcelChartLines();
                    this.SeriesLines.ReadXml(element, mFolder, xFile);
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("ofPieChart", null, "c"))
            {
                if (this.ChartType == ExcelChartType.PieOfPie)
                {
                    writer.WriteLeafElementWithAttribute("ofPieType", null, "c", "val", "pie");
                }
                else
                {
                    writer.WriteLeafElementWithAttribute("ofPieType", null, "c", "val", "bar");
                }
                writer.WriteLeafElementWithAttribute("varyColors", null, "c", "val", this.VaryColors ? "1" : "0");
                foreach (ExcelPieSeries series in this.PieSeries)
                {
                    series.WriteXml(writer, mFolder, chartFile);
                }
                if (this.DataLabels != null)
                {
                    (this.DataLabels as ExcelDataLabels).WriteXml(writer, mFolder, chartFile);
                }
                writer.WriteLeafElementWithAttribute("gapWidth", null, "c", "val", ((int) this.GapWidth).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                string str = "auto";
                switch (this.SplitType)
                {
                    case OfPieChartSplitType.Custom:
                        str = "cust";
                        break;

                    case OfPieChartSplitType.Percent:
                        str = "percent";
                        break;

                    case OfPieChartSplitType.Position:
                        str = "pos";
                        break;

                    case OfPieChartSplitType.Value:
                        str = "val";
                        break;

                    default:
                        str = "auto";
                        break;
                }
                writer.WriteLeafElementWithAttribute("splitType", null, "c", "val", str);
                if (!this.SplitPosition.IsZero())
                {
                    writer.WriteLeafElementWithAttribute("splitPos", null, "c", "val", ((double) this.SplitPosition).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                if (((this.SplitType == OfPieChartSplitType.Custom) && (this.CustomSplitPoints != null)) && (this.CustomSplitPoints.Count > 0))
                {
                    using (writer.WriteElement("custSplit", null, "c"))
                    {
                        foreach (int num in this.CustomSplitPoints)
                        {
                            writer.WriteLeafElementWithAttribute("secondPiePt", null, "c", "val", ((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                }
                if (this.SecondPieSize != 0x4b)
                {
                    writer.WriteLeafElementWithAttribute("secondPieSize", null, "c", "val", ((int) this.SecondPieSize).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                if (this.SeriesLines != null)
                {
                    this.SeriesLines.WriteXml(writer, mFolder, chartFile, "serLines");
                }
            }
        }

        /// <summary>
        /// Speicifies the bar chart type.
        /// </summary>
        public ExcelChartType ChartType { get; internal set; }

        /// <summary>
        /// Specifies the custom split information for a pie of pie or bar of pie chart with a custom split.
        /// </summary>
        public List<int> CustomSplitPoints
        {
            get
            {
                if (this.customSplitPoints == null)
                {
                    this.customSplitPoints = new List<int>();
                }
                return this.customSplitPoints;
            }
            set { this.customSplitPoints = value; }
        }

        /// <summary>
        /// Specifies the settings for the data labels.
        /// </summary>
        public IExcelDataLabels DataLabels { get; set; }

        /// <summary>
        /// Specifies the space between bar or column clusters, as a percentage of the bar or column width.
        /// </summary>
        /// <remarks>
        /// the value should be between 0 to 500
        /// </remarks>
        public int GapWidth
        {
            get { return  this.gapWidth; }
            set
            {
                if ((value < 0) || (value > 500))
                {
                    throw new ArgumentOutOfRangeException("GapWidth");
                }
                this.gapWidth = value;
            }
        }

        /// <summary>
        /// Specifies the series collection on the pie chart.
        /// </summary>
        public List<IExcelPieSeries> PieSeries
        {
            get
            {
                if (this.pieSeries == null)
                {
                    this.pieSeries = new List<IExcelPieSeries>();
                }
                return this.pieSeries;
            }
            set { this.pieSeries = value; }
        }

        /// <summary>
        /// Specifes the size of the second pie or bar of a pie chart, as a percentage of the size of the first pie.
        /// </summary>
        public int SecondPieSize
        {
            get { return  this.secondPieSize; }
            set
            {
                if ((value < 5) || (value > 500))
                {
                    throw new ArgumentOutOfRangeException("SecondPieSize");
                }
                this.secondPieSize = value;
            }
        }

        /// <summary>
        /// Specifies series lines for the chart
        /// </summary>
        public ExcelChartLines SeriesLines { get; set; }

        /// <summary>
        /// Specifies a value that shall be used to determine which data points are in the second pie or bar on a
        /// pie of pie or bar of pie chart.
        /// </summary>
        public double SplitPosition { get; set; }

        /// <summary>
        /// Specifies the possible ways to split a pie of pie or bar of pie chart.
        /// </summary>
        public OfPieChartSplitType SplitType { get; set; }

        /// <summary>
        /// Vary colors by point
        /// </summary>
        public bool VaryColors { get; set; }
    }
}

