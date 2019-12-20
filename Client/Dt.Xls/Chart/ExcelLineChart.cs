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
    /// Represents an excel line chart
    /// </summary>
    public class ExcelLineChart : IExcelLineChart, IExcelLineChartBase, IExcelChartBase
    {
        private ExcelChartType _chartType;
        private List<IExcelLineSeries> lineSeries;
        private int? xAxisID;
        private int? yAxisID;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelLineChart" /> class.
        /// </summary>
        public ExcelLineChart() : this(ExcelChartType.LineWithMarkers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelLineChart" /> class.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        /// <exception cref="T:System.ArgumentException">The specified chart type is not compitible with line chart;chartType</exception>
        public ExcelLineChart(ExcelChartType chartType)
        {
            if ((((chartType != ExcelChartType.Line) && (chartType != ExcelChartType.LineStacked)) && ((chartType != ExcelChartType.LineStacked100Percent) && (chartType != ExcelChartType.LineStacked100PercentWithMarkers))) && ((chartType != ExcelChartType.LineStackedWithMarkers) && (chartType != ExcelChartType.LineWithMarkers)))
            {
                throw new ArgumentException("The specified chart type is not compitible with line chart", "chartType");
            }
            this.ChartType = chartType;
        }

        private string GetChartGrouping()
        {
            if ((this.ChartType == ExcelChartType.LineWithMarkers) || (this.ChartType == ExcelChartType.Line))
            {
                return "standard";
            }
            if ((this.ChartType != ExcelChartType.LineStacked) && (this.ChartType != ExcelChartType.LineStackedWithMarkers))
            {
                return "percentStacked";
            }
            return "stacked";
        }

        private ExcelChartType GetChartType(string areaChart, bool marker)
        {
            if (areaChart == "standard")
            {
                if (!marker)
                {
                    return ExcelChartType.Line;
                }
                return ExcelChartType.LineWithMarkers;
            }
            if (areaChart == "stacked")
            {
                if (!marker)
                {
                    return ExcelChartType.LineStacked;
                }
                return ExcelChartType.LineStackedWithMarkers;
            }
            if (areaChart != "percentStacked")
            {
                return ExcelChartType.Area;
            }
            if (!marker)
            {
                return ExcelChartType.LineStacked100Percent;
            }
            return ExcelChartType.LineStacked100PercentWithMarkers;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            string areaChart = "standard";
            bool marker = false;
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "grouping")
                {
                    areaChart = element.GetAttributeValueOrDefaultOfStringType("val", "standard");
                }
                else if (element.Name.LocalName == "varyColors")
                {
                    this.VaryColors = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "ser")
                {
                    ExcelLineSeries series = new ExcelLineSeries();
                    series.ReadXml(element, mFolder, xFile);
                    this.LineSeries.Add(series);
                }
                else if (element.Name.LocalName == "dLbls")
                {
                    ExcelDataLabels labels = new ExcelDataLabels();
                    labels.ReadXml(element, mFolder, xFile);
                    this.DataLabels = labels;
                }
                else if (element.Name.LocalName == "dropLines")
                {
                    this.DropLine = new ExcelChartLines();
                    this.DropLine.ReadXml(element, mFolder, xFile);
                }
                else if (element.Name.LocalName == "hiLowLines")
                {
                    this.HighLowLine = new ExcelChartLines();
                    this.HighLowLine.ReadXml(element, mFolder, xFile);
                }
                else if (element.Name.LocalName == "upDownBars")
                {
                    ExcelUpDownBars bars = new ExcelUpDownBars();
                    bars.ReadXml(element, mFolder, xFile);
                    this.UpDownBars = bars;
                }
                else if (element.Name.LocalName == "marker")
                {
                    marker = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "smooth")
                {
                    this.Smoothing = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "axId")
                {
                    int num = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                    if (!this.XAxisID.HasValue)
                    {
                        this.XAxisID = new int?(num);
                    }
                    else if (!this.YAxisID.HasValue)
                    {
                        this.YAxisID = new int?(num);
                    }
                }
            }
            this.ChartType = this.GetChartType(areaChart, marker);
        }

        private bool ShowMarkers()
        {
            if (((this.ChartType != ExcelChartType.LineStacked100PercentWithMarkers) && (this.ChartType != ExcelChartType.LineStackedWithMarkers)) && (this.ChartType != ExcelChartType.LineWithMarkers))
            {
                return false;
            }
            return true;
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            if ((this.AxisX == null) || (this.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null");
            }
            using (writer.WriteElement("lineChart", null, "c"))
            {
                writer.WriteLeafElementWithAttribute("grouping", null, "c", "val", this.GetChartGrouping());
                writer.WriteLeafElementWithAttribute("varyColors", null, "c", "val", this.VaryColors ? "1" : "0");
                using (List<IExcelLineSeries>.Enumerator enumerator = this.LineSeries.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        (enumerator.Current as ExcelLineSeries).WriteXml(writer, mFolder, chartFile);
                    }
                }
                if (this.DataLabels != null)
                {
                    (this.DataLabels as ExcelDataLabels).WriteXml(writer, mFolder, chartFile);
                }
                if (this.DropLine != null)
                {
                    this.DropLine.WriteXml(writer, mFolder, chartFile, "dropLines");
                }
                if (this.HighLowLine != null)
                {
                    this.HighLowLine.WriteXml(writer, mFolder, chartFile, "hiLowLines");
                }
                if (this.UpDownBars != null)
                {
                    this.UpDownBars.WriteXml(writer, mFolder, chartFile);
                }
                writer.WriteLeafElementWithAttribute("marker", null, "c", "val", this.ShowMarkers() ? "1" : "0");
                writer.WriteLeafElementWithAttribute("smooth", null, "c", "val", this.Smoothing ? "1" : "0");
                if (this.AxisX != null)
                {
                    writer.WriteLeafElementWithAttribute("axId", null, "c", "val", ((int) this.AxisX.Id).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                if (this.AxisY != null)
                {
                    writer.WriteLeafElementWithAttribute("axId", null, "c", "val", ((int) this.AxisY.Id).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
            }
        }

        /// <summary>
        /// Represents the category axis info.
        /// </summary>
        public IExcelChartAxis AxisX { get; set; }

        /// <summary>
        /// Represents the value axis info.
        /// </summary>
        public IExcelChartAxis AxisY { get; set; }

        /// <summary>
        /// Speicifies the bar chart type.
        /// </summary>
        public ExcelChartType ChartType
        {
            get { return  this._chartType; }
            internal set { this._chartType = value; }
        }

        /// <summary>
        /// Specifies the settings for the data labels.
        /// </summary>
        public IExcelDataLabels DataLabels { get; set; }

        /// <summary>
        /// Specifies the dropLine format.
        /// </summary>
        public ExcelChartLines DropLine { get; set; }

        /// <summary>
        /// Represetns the high low lines
        /// </summary>
        public ExcelChartLines HighLowLine { get; set; }

        /// <summary>
        /// Specifies the series collection on the line chart.
        /// </summary>
        public List<IExcelLineSeries> LineSeries
        {
            get
            {
                if (this.lineSeries == null)
                {
                    this.lineSeries = new List<IExcelLineSeries>();
                }
                return this.lineSeries;
            }
            set { this.lineSeries = value; }
        }

        /// <summary>
        /// Specifies the line connecting the points on the chart shall be smoonthed using Catmull-Rom splines.
        /// </summary>
        public bool Smoothing { get; set; }

        /// <summary>
        /// Specifies the Up/Down bars for the line chart.
        /// </summary>
        public ExcelUpDownBars UpDownBars { get; set; }

        /// <summary>
        /// Vary colors by point
        /// </summary>
        public bool VaryColors { get; set; }

        internal int? XAxisID
        {
            get
            {
                if (this.AxisX != null)
                {
                    return new int?(this.AxisX.Id);
                }
                if (this.xAxisID.HasValue)
                {
                    return new int?(this.xAxisID.Value);
                }
                return null;
            }
            set { this.xAxisID = value; }
        }

        internal int? YAxisID
        {
            get
            {
                if (this.AxisY != null)
                {
                    return new int?(this.AxisY.Id);
                }
                if (this.yAxisID.HasValue)
                {
                    return new int?(this.yAxisID.Value);
                }
                return null;
            }
            set { this.yAxisID = value; }
        }
    }
}

