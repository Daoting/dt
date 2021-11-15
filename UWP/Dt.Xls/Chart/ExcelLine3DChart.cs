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
    /// Represents an excel line 3d chart.
    /// </summary>
    public class ExcelLine3DChart : IExcelLine3DChart, IExcelLineChartBase, IExcelChartBase
    {
        private List<IExcelLineSeries> lineSeries;
        private int? xAxisID;
        private int? yAxisID;
        private int? zAxisID;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelLine3DChart" /> class.
        /// </summary>
        public ExcelLine3DChart()
        {
            this.GapDepth = 150;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName != "grouping")
                {
                    if (element.Name.LocalName == "varyColors")
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
                    else if (element.Name.LocalName == "gapDepth")
                    {
                        this.GapDepth = element.GetAttributeValueOrDefaultOfInt32Type("val", 150);
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
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            if (((this.AxisX == null) || (this.AxisY == null)) || (this.AxisZ == null))
            {
                throw new InvalidOperationException("Both XAxis, YAxis and ZAxis cannot be null");
            }
            using (writer.WriteElement("line3DChart", null, "c"))
            {
                writer.WriteLeafElementWithAttribute("grouping", null, "c", "val", "standard");
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
                if (this.GapDepth != 150)
                {
                    writer.WriteLeafElementWithAttribute("gapDepth", null, "c", "val", ((int) this.GapDepth).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
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
        /// Represeents the series axis info.
        /// </summary>
        public IExcelChartAxis AxisZ { get; set; }

        /// <summary>
        /// Speicifies the bar chart type.
        /// </summary>
        public ExcelChartType ChartType
        {
            get { return  ExcelChartType.Line3D; }
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
        /// Specifies the space between bar or column clusters, as a percentage of the bar or column width.
        /// </summary>
        public int GapDepth { get; set; }

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

