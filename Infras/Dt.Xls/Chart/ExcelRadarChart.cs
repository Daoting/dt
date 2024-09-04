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
    /// Represents an excel scatter chart.
    /// </summary>
    public class ExcelRadarChart : IExcelRadarChart, IExcelChartBase
    {
        private List<IExcelRadarSeries> radarSeries;
        private int? xAxisID;
        private int? yAxisID;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelRadarChart" /> class.
        /// </summary>
        public ExcelRadarChart() : this(ExcelChartType.Radar)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelRadarChart" /> class.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        /// <exception cref="T:System.ArgumentException">The specified chart type is not compitible with radar chart;chartType</exception>
        public ExcelRadarChart(ExcelChartType chartType)
        {
            if (((chartType != ExcelChartType.Radar) && (chartType != ExcelChartType.RadarWithMarkers)) && (chartType != ExcelChartType.FilledRadar))
            {
                throw new ArgumentException("The specified chart type is not compitible with radar chart", "chartType");
            }
            this.ChartType = chartType;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "radarStyle")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "standard"))
                    {
                        case "standard":
                            this.ChartType = ExcelChartType.Radar;
                            break;

                        case "marker":
                            this.ChartType = ExcelChartType.RadarWithMarkers;
                            break;

                        case "filled":
                            this.ChartType = ExcelChartType.FilledRadar;
                            break;
                    }
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
                else if (element.Name.LocalName == "ser")
                {
                    ExcelRadarSeries series = new ExcelRadarSeries();
                    series.ReadXml(element, mFolder, xFile);
                    this.RadarSeries.Add(series);
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
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            if ((this.AxisX == null) || (this.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null");
            }
            writer.WriteStartElement("c", "radarChart", null);
            string str = "standard";
            if (this.ChartType == ExcelChartType.Radar)
            {
                str = "standard";
            }
            else if (this.ChartType == ExcelChartType.RadarWithMarkers)
            {
                str = "marker";
            }
            else if (this.ChartType == ExcelChartType.FilledRadar)
            {
                str = "filled";
            }
            writer.WriteLeafElementWithAttribute("radarStyle", null, "c", "val", str);
            writer.WriteLeafElementWithAttribute("varyColors", null, "c", "val", this.VaryColors ? "1" : "0");
            foreach (ExcelRadarSeries series in this.RadarSeries)
            {
                series.WriteXml(writer, mFolder, chartFile);
            }
            if (this.DataLabels != null)
            {
                (this.DataLabels as ExcelDataLabels).WriteXml(writer, mFolder, chartFile);
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
        /// Speicifies the radar chart type.
        /// </summary>
        public ExcelChartType ChartType { get; internal set; }

        /// <summary>
        /// Specifies the settings for the data labels.
        /// </summary>
        public IExcelDataLabels DataLabels { get; set; }

        /// <summary>
        /// Specifies the series collection on the radar chart.
        /// </summary>
        public List<IExcelRadarSeries> RadarSeries
        {
            get
            {
                if (this.radarSeries == null)
                {
                    this.radarSeries = new List<IExcelRadarSeries>();
                }
                return this.radarSeries;
            }
            set { this.radarSeries = value; }
        }

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

