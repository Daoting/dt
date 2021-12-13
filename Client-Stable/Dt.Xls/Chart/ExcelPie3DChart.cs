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
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Represents an excel pie 3d chart.
    /// </summary>
    public class ExcelPie3DChart : IExcelPie3DChart, IExcelPieChartBase, IExcelChartBase
    {
        private List<IExcelPieSeries> pieSeries;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelPie3DChart" /> class.
        /// </summary>
        public ExcelPie3DChart() : this(ExcelChartType.Pie3D)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelPie3DChart" /> class.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        /// <exception cref="T:System.ArgumentException">The specified chart type is not compitible with pie 3D chart;chartType</exception>
        public ExcelPie3DChart(ExcelChartType chartType)
        {
            if ((chartType != ExcelChartType.Pie3D) && (chartType != ExcelChartType.ExplodedPie3D))
            {
                throw new ArgumentException("The specified chart type is not compitible with pie 3D chart", "chartType");
            }
            this.ChartType = chartType;
            this.VaryColors = true;
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
                    ExcelPieSeries series = new ExcelPieSeries();
                    series.ReadXml(element, mFolder, xFile);
                    this.PieSeries.Add(series);
                }
                else if (element.Name.LocalName == "dLbls")
                {
                    ExcelDataLabels labels = new ExcelDataLabels();
                    labels.ReadXml(element, mFolder, xFile);
                    this.DataLabels = labels;
                }
            }
            bool flag = true;
            if (this.PieSeries.Count > 0)
            {
                int explosion = this.PieSeries[0].Explosion;
                foreach (IExcelPieSeries series2 in this.PieSeries)
                {
                    if ((series2.Explosion == 0) || (series2.Explosion != explosion))
                    {
                        flag = false;
                        break;
                    }
                }
            }
            this.ChartType = flag ? ExcelChartType.ExplodedPie3D : ExcelChartType.Pie3D;
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("pie3DChart", null, "c"))
            {
                writer.WriteLeafElementWithAttribute("varyColors", null, "c", "val", this.VaryColors ? "1" : "0");
                if (this.ChartType == ExcelChartType.ExplodedPie)
                {
                    foreach (IExcelPieSeries series in this.PieSeries)
                    {
                        if (series.Explosion == 0)
                        {
                            series.Explosion = 0x19;
                        }
                    }
                }
                foreach (ExcelPieSeries series2 in this.PieSeries)
                {
                    series2.WriteXml(writer, mFolder, chartFile);
                }
                if (this.DataLabels != null)
                {
                    (this.DataLabels as ExcelDataLabels).WriteXml(writer, mFolder, chartFile);
                }
            }
        }

        /// <summary>
        /// Speicifies the bar chart type.
        /// </summary>
        public ExcelChartType ChartType { get; internal set; }

        /// <summary>
        /// Specifies the settings for the data labels.
        /// </summary>
        public IExcelDataLabels DataLabels { get; set; }

        /// <summary>
        /// Specifies the series collection on the pie chart.
        /// </summary>
        public List<IExcelPieSeries> PieSeries
        {
            get
            {
                if (this.pieSeries == null)
                {
                    this.pieSeries = new List<IExcelPieSeries>();
                }
                return this.pieSeries;
            }
            set { this.pieSeries = value; }
        }

        /// <summary>
        /// Vary colors by point
        /// </summary>
        public bool VaryColors { get; set; }
    }
}

