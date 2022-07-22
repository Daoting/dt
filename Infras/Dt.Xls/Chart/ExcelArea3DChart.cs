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
    /// Represents a excel area 3D chart.
    /// </summary>
    public class ExcelArea3DChart : IExcelArea3DChart, IExcelAreaChartBase, IExcelChartBase
    {
        private List<IExcelAreaSeries> areaSeries;
        private int? xAxisID;
        private int? yAxisID;
        private int? zAxisID;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelAreaChart" /> class.
        /// </summary>
        public ExcelArea3DChart() : this(ExcelChartType.Area3D)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelAreaChart" /> class.
        /// </summary>
        /// <param name="areaChartType">Type of the area chart.</param>
        public ExcelArea3DChart(ExcelChartType areaChartType)
        {
            if (((areaChartType != ExcelChartType.Area3D) && (areaChartType != ExcelChartType.AreaStacked3D)) && (areaChartType != ExcelChartType.AreaStacked100Percent3D))
            {
                throw new ArgumentException("The specified chart type is not compitible with area 3D chart", "chartType");
            }
            this.ChartType = areaChartType;
        }

        private ExcelChartType GetChartType(string areaChart)
        {
            if (areaChart == "standard")
            {
                return ExcelChartType.Area3D;
            }
            if (areaChart == "stacked")
            {
                return ExcelChartType.AreaStacked3D;
            }
            if (areaChart == "percentStacked")
            {
                return ExcelChartType.AreaStacked100Percent3D;
            }
            return ExcelChartType.Area;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "grouping")
                {
                    this.ChartType = this.GetChartType(element.GetAttributeValueOrDefaultOfStringType("val", "standard"));
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
                    ExcelAreaSeries series = new ExcelAreaSeries();
                    series.ReadXml(element, mFolder, xFile);
                    this.AreaSeries.Add(series);
                }
                else if (element.Name.LocalName == "gapDepth")
                {
                    this.GapDepth = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
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
                    else if (!this.ZAxisID.HasValue)
                    {
                        this.ZAxisID = new int?(num);
                    }
                }
                else if (element.Name.LocalName == "dropLines")
                {
                    this.DropLine = new ExcelChartLines();
                    this.DropLine.ReadXml(element, mFolder, xFile);
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            if ((this.AxisX == null) || (this.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null");
            }
            writer.WriteStartElement("c", "area3DChart", null);
            string str = "standard";
            if (this.ChartType == ExcelChartType.AreaStacked3D)
            {
                str = "stacked";
            }
            else if (this.ChartType == ExcelChartType.AreaStacked100Percent3D)
            {
                str = "percentStacked";
            }
            writer.WriteLeafElementWithAttribute("grouping", null, "c", "val", str);
            writer.WriteLeafElementWithAttribute("varyColors", null, "c", "val", this.VaryColors ? "1" : "0");
            foreach (ExcelAreaSeries series in this.AreaSeries)
            {
                series.WriteXml(writer, mFolder, chartFile);
            }
            if (this.DataLabels != null)
            {
                (this.DataLabels as ExcelDataLabels).WriteXml(writer, mFolder, chartFile);
            }
            if (this.DropLine != null)
            {
                this.DropLine.WriteXml(writer, mFolder, chartFile, "dropLines");
            }
            writer.WriteLeafElementWithAttribute("gapDepth", null, "c", "val", ((int) this.GapDepth).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            if (this.AxisX != null)
            {
                writer.WriteLeafElementWithAttribute("axId", null, "c", "val", ((int) this.AxisX.Id).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (this.AxisY != null)
            {
                writer.WriteLeafElementWithAttribute("axId", null, "c", "val", ((int) this.AxisY.Id).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (this.AxisZ != null)
            {
                writer.WriteLeafElementWithAttribute("axId", null, "c", "val", ((int) this.AxisZ.Id).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Specifies the series collection on the area chart.
        /// </summary>
        public List<IExcelAreaSeries> AreaSeries
        {
            get
            {
                if (this.areaSeries == null)
                {
                    this.areaSeries = new List<IExcelAreaSeries>();
                }
                return this.areaSeries;
            }
            set { this.areaSeries = value; }
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
        /// Represeents the series axis info.
        /// </summary>
        public IExcelChartAxis AxisZ { get; set; }

        /// <summary>
        /// Speicifies the area chart type.
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
        /// Specifies the space between series.
        /// </summary>
        public int GapDepth { get; set; }

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

        internal int? ZAxisID
        {
            get
            {
                if (this.AxisZ != null)
                {
                    return new int?(this.AxisZ.Id);
                }
                if (this.zAxisID.HasValue)
                {
                    return new int?(this.zAxisID.Value);
                }
                return null;
            }
            set { this.zAxisID = value; }
        }
    }
}

