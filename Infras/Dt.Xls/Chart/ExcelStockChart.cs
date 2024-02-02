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
    /// Represents an excel stock chart.
    /// </summary>
    public class ExcelStockChart : IExcelStockChart, IExcelChartBase
    {
        private List<IExcelLineSeries> lineSeries;
        private int? xAxisID;
        private int? yAxisID;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelStockChart" /> class.
        /// </summary>
        public ExcelStockChart() : this(ExcelChartType.StockHighLowClose)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelStockChart" /> class.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        public ExcelStockChart(ExcelChartType chartType)
        {
            if ((chartType != ExcelChartType.StockHighLowClose) && (chartType != ExcelChartType.StockOpenHighLowClose))
            {
                throw new ArgumentException("The specified chart type is not compitible with stock chart", "chartType");
            }
            this.ChartType = chartType;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "ser")
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
            this.ChartType = (this.LineSeries.Count == 3) ? ExcelChartType.StockHighLowClose : ExcelChartType.StockOpenHighLowClose;
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            if ((this.AxisX == null) || (this.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null");
            }
            if ((this.ChartType == ExcelChartType.StockHighLowClose) && (this.LineSeries.Count != 3))
            {
                throw new InvalidOperationException("StockHighLowClose chart must have 3 series");
            }
            if ((this.ChartType == ExcelChartType.StockOpenHighLowClose) && (this.LineSeries.Count != 4))
            {
                throw new InvalidOperationException("StockOpenHighLowClose chart must have 4 series");
            }
            using (writer.WriteElement("stockChart", null, "c"))
            {
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
        /// Represents the excel stock chart type.
        /// </summary>
        public ExcelChartType ChartType { get; internal set; }

        /// <summary>
        /// Specifies the settings for the data labels.
        /// </summary>
        public IExcelDataLabels DataLabels { get; set; }

        /// <summary>
        /// specifies drop lines.
        /// </summary>
        public ExcelChartLines DropLine { get; set; }

        /// <summary>
        /// Represents the high/low line format 
        /// </summary>
        public ExcelChartLines HighLowLine { get; set; }

        /// <summary>
        /// Specifies the series collection on the stock chart.
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
        /// Specifies the Up/Down bars for the line chart.
        /// </summary>
        public ExcelUpDownBars UpDownBars { get; set; }

        /// <summary>
        /// Vary colors by point
        /// </summary>
        public bool VaryColors
        {
            get { return  false; }
            set
            {
            }
        }

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

