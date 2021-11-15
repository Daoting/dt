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
    /// 
    /// </summary>
    public class ExcelBubbleChart : IExcelBubbleChart, IExcelChartBase
    {
        private int bubbleScale;
        private List<ExcelBubbleSeries> bubbleSeries;
        private int? xAxisID;
        private int? yAxisID;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelBubbleChart" /> class.
        /// </summary>
        public ExcelBubbleChart() : this(ExcelChartType.Bubble3D)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelBubbleChart" /> class.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        public ExcelBubbleChart(ExcelChartType chartType)
        {
            this.bubbleScale = 100;
            if ((chartType == ExcelChartType.Bubble) || (chartType == ExcelChartType.Bubble3D))
            {
                this.ChartType = chartType;
                this.SizeRepresents = BubbleSizeRepresents.Area;
                this.ShowNegativeBubbles = false;
                this.BubbleScale = 100;
                this.VaryColors = true;
            }
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "varyColors")
                {
                    this.VaryColors = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "ser")
                {
                    ExcelBubbleSeries series = new ExcelBubbleSeries();
                    series.ReadXml(element, mFolder, xFile);
                    this.BubbleSeries.Add(series);
                }
                else if (element.Name.LocalName == "dLbls")
                {
                    ExcelDataLabels labels = new ExcelDataLabels();
                    labels.ReadXml(element, mFolder, xFile);
                    this.DataLabels = labels;
                }
                else if (element.Name.LocalName == "bubble3D")
                {
                    if (element.GetAttributeValueOrDefaultOfBooleanType("val", true))
                    {
                        this.ChartType = ExcelChartType.Bubble3D;
                    }
                    else
                    {
                        this.ChartType = ExcelChartType.Bubble;
                    }
                }
                else if (element.Name.LocalName == "bubbleScale")
                {
                    this.BubbleScale = element.GetAttributeValueOrDefaultOfInt32Type("val", 100);
                }
                else if (element.Name.LocalName == "showNegBubbles")
                {
                    this.ShowNegativeBubbles = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "sizeRepresents")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "area"))
                    {
                        case "area":
                            this.SizeRepresents = BubbleSizeRepresents.Area;
                            break;

                        case "w":
                            this.SizeRepresents = BubbleSizeRepresents.Width;
                            break;
                    }
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
            using (writer.WriteElement("bubbleChart", null, "c"))
            {
                writer.WriteLeafElementWithAttribute("varyColors", null, "c", "val", this.VaryColors ? "1" : "0");
                using (List<ExcelBubbleSeries>.Enumerator enumerator = this.BubbleSeries.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.WriteXml(writer, mFolder, chartFile);
                    }
                }
                if (this.DataLabels != null)
                {
                    (this.DataLabels as ExcelDataLabels).WriteXml(writer, mFolder, chartFile);
                }
                if (this.ChartType == ExcelChartType.Bubble)
                {
                    writer.WriteLeafElementWithAttribute("bubble3D", null, "c", "val", "0");
                }
                writer.WriteLeafElementWithAttribute("bubbleScale", null, "c", "val", ((int) this.BubbleScale).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                writer.WriteLeafElementWithAttribute("showNegBubbles", null, "c", "val", this.ShowNegativeBubbles ? "1" : "0");
                string str = "area";
                if (this.SizeRepresents == BubbleSizeRepresents.Width)
                {
                    str = "w";
                }
                writer.WriteLeafElementWithAttribute("sizeRepresents", null, "c", "val", str);
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
        /// Specifies the scale factor for the bubble chart.
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">BubbleScale</exception>
        public int BubbleScale
        {
            get { return  this.bubbleScale; }
            set
            {
                if ((value < 0) || (value > 300))
                {
                    throw new ArgumentOutOfRangeException("BubbleScale");
                }
                this.bubbleScale = value;
            }
        }

        /// <summary>
        /// Specifies the series of the bubble chart.
        /// </summary>
        public List<ExcelBubbleSeries> BubbleSeries
        {
            get
            {
                if (this.bubbleSeries == null)
                {
                    this.bubbleSeries = new List<ExcelBubbleSeries>();
                }
                return this.bubbleSeries;
            }
            set { this.bubbleSeries = value; }
        }

        /// <summary>
        /// Specifies the bubble chart type.
        /// </summary>
        public ExcelChartType ChartType { get; internal set; }

        /// <summary>
        /// Specifies the settings for the data labels.
        /// </summary>
        public IExcelDataLabels DataLabels { get; set; }

        /// <summary>
        /// Specifies negative sized bubble shall be shown on a bubble chart.
        /// </summary>
        public bool ShowNegativeBubbles { get; set; }

        /// <summary>
        /// Specifies how the bubble size values are represented on the chart.
        /// </summary>
        public BubbleSizeRepresents SizeRepresents { get; set; }

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

