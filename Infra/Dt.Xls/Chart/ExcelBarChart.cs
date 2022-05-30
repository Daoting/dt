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
    /// Represent excel column or bar chart.
    /// </summary>
    public class ExcelBarChart : IExcelBarChart, IExcelBarChartBase, IExcelChartBase
    {
        private List<IExcelBarSeries> _barSeries;
        private int? xAxisID;
        private int? yAxisID;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelBarChart" /> class.
        /// </summary>
        public ExcelBarChart() : this(ExcelChartType.ColumnClustered)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelBarChart" /> class.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        public ExcelBarChart(ExcelChartType chartType)
        {
            if ((((chartType != ExcelChartType.ColumnClustered) && (chartType != ExcelChartType.ColumnStacked)) && ((chartType != ExcelChartType.ColumnStacked100Percent) && (chartType != ExcelChartType.BarClustered))) && ((chartType != ExcelChartType.BarStacked) && (chartType != ExcelChartType.BarStacked100Percent)))
            {
                throw new ArgumentException("The specified chart type is not compitible with Bar chart", "chartType");
            }
            this.ChartType = chartType;
            this.VaryColors = false;
            this.Overlap = 0;
            this.GapWidth = 150;
        }

        private ExcelChartType GetBarChartType(string barDir, string barGrouping)
        {
            if (barDir == "col")
            {
                if (barGrouping == "clustered")
                {
                    return ExcelChartType.ColumnClustered;
                }
                if (barGrouping == "stacked")
                {
                    return ExcelChartType.ColumnStacked;
                }
                if (barGrouping == "percentStacked")
                {
                    return ExcelChartType.ColumnStacked100Percent;
                }
            }
            else if (barDir == "bar")
            {
                if (barGrouping == "clustered")
                {
                    return ExcelChartType.BarClustered;
                }
                if (barGrouping == "stacked")
                {
                    return ExcelChartType.BarStacked;
                }
                if (barGrouping == "percentStacked")
                {
                    return ExcelChartType.BarStacked100Percent;
                }
            }
            return ExcelChartType.ColumnClustered;
        }

        private string GetBarDirection(ExcelChartType chartType)
        {
            if (((chartType == ExcelChartType.ColumnClustered) || (chartType == ExcelChartType.ColumnStacked)) || (chartType == ExcelChartType.ColumnStacked100Percent))
            {
                return "col";
            }
            if (((chartType != ExcelChartType.BarClustered) && (chartType != ExcelChartType.BarStacked)) && (chartType != ExcelChartType.BarStacked100Percent))
            {
                return "col";
            }
            return "bar";
        }

        private string GetBarGrouping(ExcelChartType chartType)
        {
            if ((chartType == ExcelChartType.BarClustered) || (chartType == ExcelChartType.ColumnClustered))
            {
                return "clustered";
            }
            if ((chartType == ExcelChartType.ColumnStacked) || (chartType == ExcelChartType.BarStacked))
            {
                return "stacked";
            }
            if ((chartType != ExcelChartType.BarStacked100Percent) && (chartType != ExcelChartType.ColumnStacked100Percent))
            {
                return "clustered";
            }
            return "percentStacked";
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            string barDir = "col";
            string barGrouping = "clustered";
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "barDir")
                {
                    barDir = element.GetAttributeValueOrDefaultOfStringType("val", "col");
                }
                else if (element.Name.LocalName == "grouping")
                {
                    barGrouping = element.GetAttributeValueOrDefaultOfStringType("val", "clustered");
                }
                else if (element.Name.LocalName == "dLbls")
                {
                    ExcelDataLabels labels = new ExcelDataLabels();
                    labels.ReadXml(element, mFolder, xFile);
                    this.DataLabels = labels;
                }
                else if (element.Name.LocalName == "varyColors")
                {
                    this.VaryColors = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "gapWidth")
                {
                    this.GapWidth = element.GetAttributeValueOrDefaultOfInt32Type("val", 150);
                }
                else if (element.Name.LocalName == "overlap")
                {
                    this.Overlap = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if (element.Name.LocalName == "ser")
                {
                    ExcelBarSeries series = new ExcelBarSeries();
                    series.ReadXml(element, mFolder, xFile);
                    this.BarSeries.Add(series);
                }
                else if (element.Name.LocalName == "axId")
                {
                    int num = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                    if (num != 0)
                    {
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
                else if (element.Name.LocalName == "serLines")
                {
                    this.SeriesLines = new ExcelChartLines();
                    this.SeriesLines.ReadXml(element, mFolder, xFile);
                }
            }
            this.ChartType = this.GetBarChartType(barDir, barGrouping);
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            if ((this.AxisX == null) || (this.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null");
            }
            writer.WriteStartElement("c", "barChart", null);
            writer.WriteLeafElementWithAttribute("barDir", null, "c", "val", this.GetBarDirection(this.ChartType));
            writer.WriteLeafElementWithAttribute("grouping", null, "c", "val", this.GetBarGrouping(this.ChartType));
            writer.WriteLeafElementWithAttribute("varyColors", null, "c", "val", this.VaryColors ? "1" : "0");
            foreach (ExcelBarSeries series in this.BarSeries)
            {
                series.WriteXml(writer, mFolder, chartFile);
            }
            if (this.DataLabels != null)
            {
                (this.DataLabels as ExcelDataLabels).WriteXml(writer, mFolder, chartFile);
            }
            if (this.SeriesLines != null)
            {
                this.SeriesLines.WriteXml(writer, mFolder, chartFile, "serLines");
            }
            writer.WriteLeafElementWithAttribute("gapWidth", null, "c", "val", ((int) this.GapWidth).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            if (this.Overlap != 0)
            {
                writer.WriteLeafElementWithAttribute("overlap", null, "c", "val", ((int) this.Overlap).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (this.AxisX != null)
            {
                writer.WriteLeafElementWithAttribute("axId", null, "c", "val", ((int) this.AxisX.Id).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (this.AxisY != null)
            {
                writer.WriteLeafElementWithAttribute("axId", null, "c", "val", ((int) this.AxisY.Id).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            writer.WriteEndElement();
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
        /// Specifies the series collection on the bar chart.
        /// </summary>
        public List<IExcelBarSeries> BarSeries
        {
            get
            {
                if (this._barSeries == null)
                {
                    this._barSeries = new List<IExcelBarSeries>();
                }
                return this._barSeries;
            }
            set { this._barSeries = value; }
        }

        /// <summary>
        /// Speicifies the bar 3d chart type.
        /// </summary>
        public ExcelChartType ChartType { get; internal set; }

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
        public int GapWidth { get; set; }

        /// <summary>
        /// Specifies how much bars and columns shall overlap on 2-D charts.
        /// </summary>
        /// <remarks>
        /// the value should be between -100 and 100
        /// </remarks>
        public int Overlap { get; set; }

        /// <summary>
        /// Specifies series lines for the chart
        /// </summary>
        public ExcelChartLines SeriesLines { get; set; }

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

