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
    /// Implement the IAreaSeries interface
    /// </summary>
    public class ExcelAreaSeries : IExcelAreaSeries, IExcelChartSeriesBase
    {
        private List<IExcelDataPoint> _dataPoints;
        private List<IExcelTrendLine> _trendlines;

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
                    if (this.FirstErrorBars == null)
                    {
                        this.FirstErrorBars = bars;
                    }
                    else
                    {
                        this.SecondErrorBars = bars;
                    }
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
                else if (element.Name.LocalName == "pictureOptions")
                {
                    Dt.Xls.Chart.PictureOptions options = new Dt.Xls.Chart.PictureOptions();
                    options.ReadXml(element, mFolder, xFile);
                    this.PictureOptions = options;
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
            if (this.FirstErrorBars != null)
            {
                (this.FirstErrorBars as ExcelErrorBars).WriteXml(writer, mFolder, chartFile);
            }
            if (this.SecondErrorBars != null)
            {
                (this.SecondErrorBars as ExcelErrorBars).WriteXml(writer, mFolder, chartFile);
            }
            if ((this.Trendlines != null) && (this.Trendlines.Count > 0))
            {
                foreach (ExcelTrendLine line in this.Trendlines)
                {
                    line.WriteXml(writer, mFolder, chartFile);
                }
            }
            if (this.CategoryAxisData != null)
            {
                (this.CategoryAxisData as ExcelCategoryAxisData).WriteXml(writer, mFolder, chartFile);
            }
            if (this.SeriesValue != null)
            {
                (this.SeriesValue as ExcelSeriesValue).WriteXml(writer, mFolder, chartFile);
            }
            if (this.PictureOptions != null)
            {
                this.PictureOptions.WriteXml(writer, mFolder, chartFile);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Specifies the data used for the category axis
        /// </summary>
        public IExcelChartCategoryAxisData CategoryAxisData { get; set; }

        /// <summary>
        /// Represents the data labels of this series.
        /// </summary>
        public IExcelDataLabels DataLabels { get; set; }

        /// <summary>
        /// Specifies collection od data points
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
        /// Represents the error bars.
        /// </summary>
        public IExcelErrorBars FirstErrorBars { get; set; }

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
        /// Specifies the order of the series on the collection. It's 0 based
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Represents the picture options when the fill is blip fill.
        /// </summary>
        public Dt.Xls.Chart.PictureOptions PictureOptions { get; set; }

        /// <summary>
        /// Represents the second error bars.
        /// </summary>
        public IExcelErrorBars SecondErrorBars { get; set; }

        /// <summary>
        /// Specifies text for a series name.
        /// </summary>
        public IExcelSeriesName SeriesName { get; set; }

        /// <summary>
        /// Specifies the data values which shall be used to define the locatin of the data markers on a charts
        /// </summary>
        public IExcelSeriesValue SeriesValue { get; set; }

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

