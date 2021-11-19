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
    /// Represents an excel surface 3D chart.
    /// </summary>
    public class ExcelSurface3DChart : IExcelSurface3DChart, IExcelSurfaceChartBase, IExcelChartBase
    {
        private List<int> bandFormats;
        private List<ExcelSurfaceSeries> surfaceSeries;
        private int? xAxisID;
        private int? yAxisID;
        private int? zAxisID;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelSurfaceChart" /> class.
        /// </summary>
        public ExcelSurface3DChart() : this(ExcelChartType.SurfaceWireFrame)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelSurfaceChart" /> class.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        /// <exception cref="T:System.ArgumentException">The specified chart type is not compitible with surface chart;chartType</exception>
        public ExcelSurface3DChart(ExcelChartType chartType)
        {
            if ((chartType != ExcelChartType.Surface) && (chartType != ExcelChartType.SurfaceWireFrame))
            {
                throw new ArgumentException("The specified chart type is not compitible with surface 3D chart", "chartType");
            }
            this.ChartType = chartType;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "wireframe")
                {
                    if (element.GetAttributeValueOrDefaultOfBooleanType("val", true))
                    {
                        this.ChartType = ExcelChartType.SurfaceWireFrame;
                    }
                    else
                    {
                        this.ChartType = ExcelChartType.Surface;
                    }
                }
                else if (element.Name.LocalName == "ser")
                {
                    ExcelSurfaceSeries series = new ExcelSurfaceSeries();
                    series.ReadXml(element, mFolder, xFile);
                    this.SurfaceSeries.Add(series);
                }
                else if (element.Name.LocalName == "bandFmts")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "bandFmt")
                        {
                            this.BandFormats.Add((int) ((int) element2.GetChildElementAttributeValueOrDefault<int>("idx", "val")));
                        }
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
                    else if (!this.ZAxisID.HasValue)
                    {
                        this.ZAxisID = new int?(num);
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
            using (writer.WriteElement("surface3DChart", null, "c"))
            {
                writer.WriteLeafElementWithAttribute("wireframe", null, "c", "val", (this.ChartType == ExcelChartType.SurfaceWireFrame) ? "1" : "0");
                using (List<ExcelSurfaceSeries>.Enumerator enumerator = this.SurfaceSeries.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.WriteXml(writer, mFolder, chartFile);
                    }
                }
                using (writer.WriteElement("bandFmts", null, "c"))
                {
                    foreach (int num in this.BandFormats)
                    {
                        using (writer.WriteElement("bandFmt", null, "c"))
                        {
                            writer.WriteLeafElementWithAttribute("idx", null, "c", "val", ((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
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
        /// A collection of formatting bands for a surface chart indexed from low to high.
        /// </summary>
        public List<int> BandFormats
        {
            get
            {
                if (this.bandFormats == null)
                {
                    this.bandFormats = new List<int>();
                }
                return this.bandFormats;
            }
            set { this.bandFormats = value; }
        }

        /// <summary>
        /// Specifies the chart type for the surface 3D chart.
        /// </summary>
        public ExcelChartType ChartType { get; internal set; }

        /// <summary>
        /// Specifies the settings for the data labels.
        /// </summary>
        public IExcelDataLabels DataLabels
        {
            get { return  null; }
            set
            {
            }
        }

        /// <summary>
        /// Specifies the data series for the surface chart.
        /// </summary>
        public List<ExcelSurfaceSeries> SurfaceSeries
        {
            get
            {
                if (this.surfaceSeries == null)
                {
                    this.surfaceSeries = new List<ExcelSurfaceSeries>();
                }
                return this.surfaceSeries;
            }
            set { this.surfaceSeries = value; }
        }

        /// <summary>
        /// Vary colors by point
        /// </summary>
        public bool VaryColors
        {
            get { return  true; }
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

