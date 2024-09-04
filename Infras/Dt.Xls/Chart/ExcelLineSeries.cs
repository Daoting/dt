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
    /// Specifies a series on a line chart.
    /// </summary>
    public class ExcelLineSeries : IExcelLineSeries, IExcelChartSeriesBase
    {
        private List<IExcelDataPoint> _dataPoints;
        private List<IExcelTrendLine> _trendlines;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelLineSeries" /> class.
        /// </summary>
        public ExcelLineSeries()
        {
            this.Smoothing = true;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "cat")
                {
                    ExcelCategoryAxisData data = new ExcelCategoryAxisData();
                    data.ReadXml(element);
                    this.CategoryAxisData = data;
                }
                else if (element.Name.LocalName == "dLbls")
                {
                    ExcelDataLabels labels = new ExcelDataLabels();
                    labels.ReadXml(element, mFolder, xFile);
                    this.DataLabels = labels;
                }
                else if (element.Name.LocalName == "dPt")
                {
                    ExcelDataPoint point = new ExcelDataPoint();
                    point.ReadXml(element, mFolder, xFile);
                    this.DataPoints.Add(point);
                }
                else if (element.Name.LocalName == "errBars")
                {
                    ExcelErrorBars bars = new ExcelErrorBars();
                    bars.ReadXml(element, mFolder, xFile);
                    this.ErrorBars = bars;
                }
                else if (element.Name.LocalName == "idx")
                {
                    this.Index = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if (element.Name.LocalName == "order")
                {
                    this.Order = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format = new ExcelChartFormat();
                    format.ReadXml(element, mFolder, xFile);
                    this.Format = format;
                }
                else if (element.Name.LocalName == "trendline")
                {
                    ExcelTrendLine line = new ExcelTrendLine();
                    line.ReadXml(element, mFolder, xFile);
                    this.Trendlines.Add(line);
                }
                else if (element.Name.LocalName == "tx")
                {
                    ExcelSeriesName name = new ExcelSeriesName();
                    name.ReadXml(element);
                    this.SeriesName = name;
                }
                else if (element.Name.LocalName == "val")
                {
                    ExcelSeriesValue value2 = new ExcelSeriesValue();
                    value2.ReadXml(element);
                    this.SeriesValue = value2;
                }
                else if (element.Name.LocalName == "marker")
                {
                    ExcelDataMarker marker = new ExcelDataMarker();
                    marker.ReadXml(element, mFolder, xFile);
                    this.Marker = marker;
                }
                else if (element.Name.LocalName == "smooth")
                {
                    this.Smoothing = element.GetAttributeValueOrDefaultOfBooleanType("val", false);
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "ser", null);
            writer.WriteLeafElementWithAttribute("idx", null, "c", "val", ((int) this.Index).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            writer.WriteLeafElementWithAttribute("order", null, "c", "val", ((int) this.Order).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            if (this.SeriesName != null)
            {
                (this.SeriesName as ExcelSeriesName).WriteXml(writer, mFolder, chartFile);
            }
            if (this.Format != null)
            {
                (this.Format as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
            }
            if (this.Marker != null)
            {
                this.Marker.WriteXml(writer, mFolder, chartFile);
            }
            if ((this.DataPoints != null) && (this.DataPoints.Count > 0))
            {
                foreach (ExcelDataPoint point in this.DataPoints)
                {
                    point.WriteXml(writer, mFolder, chartFile);
                }
            }
            if (this.DataLabels != null)
            {
                (this.DataLabels as ExcelDataLabels).WriteXml(writer, mFolder, chartFile);
            }
            if ((this.Trendlines != null) && (this.Trendlines.Count > 0))
            {
                foreach (ExcelTrendLine line in this.Trendlines)
                {
                    line.WriteXml(writer, mFolder, chartFile);
                }
            }
            if (this.ErrorBars != null)
            {
                this.ErrorBars.WriteXml(writer, mFolder, chartFile);
            }
            if (this.CategoryAxisData != null)
            {
                (this.CategoryAxisData as ExcelCategoryAxisData).WriteXml(writer, mFolder, chartFile);
            }
            if (this.SeriesValue != null)
            {
                (this.SeriesValue as ExcelSeriesValue).WriteXml(writer, mFolder, chartFile);
            }
            if (!this.Smoothing)
            {
                writer.WriteLeafElementWithAttribute("smooth", null, "c", "val", "0");
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Specifies the category axis data
        /// </summary>
        public IExcelChartCategoryAxisData CategoryAxisData { get; set; }

        /// <summary>
        /// Specifies the data labels
        /// </summary>
        public IExcelDataLabels DataLabels { get; set; }

        /// <summary>
        /// Represents a data point collections.
        /// </summary>
        public List<IExcelDataPoint> DataPoints
        {
            get
            {
                if (this._dataPoints == null)
                {
                    this._dataPoints = new List<IExcelDataPoint>();
                }
                return this._dataPoints;
            }
            set { this._dataPoints = value; }
        }

        /// <summary>
        /// Specifies the  error bars.
        /// </summary>
        public ExcelErrorBars ErrorBars { get; set; }

        /// <summary>
        /// Represents the formatting options of this series.
        /// </summary>
        public IExcelChartFormat Format { get; set; }

        /// <summary>
        /// Specifies the index of the containing element. It shall determine which of the parent's children
        /// collection this elemeht applies to.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Represents the data marker settings;
        /// </summary>
        public ExcelDataMarker Marker { get; set; }

        /// <summary>
        /// Specifies the order of the series on the collection. It's 0 based
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Specifies text for a series name.
        /// </summary>
        public IExcelSeriesName SeriesName { get; set; }

        /// <summary>
        /// Specifies series value
        /// </summary>
        public IExcelSeriesValue SeriesValue { get; set; }

        /// <summary>
        /// Specifies the line connection the points on the chart shall be smoothed using Catmull-Rom splines.
        /// </summary>
        public bool Smoothing { get; set; }

        /// <summary>
        /// Represents collection of trend lines.
        /// </summary>
        public List<IExcelTrendLine> Trendlines
        {
            get
            {
                if (this._trendlines == null)
                {
                    this._trendlines = new List<IExcelTrendLine>();
                }
                return this._trendlines;
            }
            set { this._trendlines = value; }
        }
    }
}

