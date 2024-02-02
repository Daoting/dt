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
    /// Represents the excel bar 3D chart.
    /// </summary>
    public class ExcelBar3DChart : IExcelBar3DChart, IExcelBarChartBase, IExcelChartBase
    {
        private List<IExcelBarSeries> barSeries;
        private int? xAxisID;
        private int? yAxisID;
        private int? zAxisID;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelBar3DChart" /> class.
        /// </summary>
        public ExcelBar3DChart() : this(ExcelChartType.ColumnClustered3D)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelBar3DChart" /> class.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        /// <exception cref="T:System.ArgumentException">The specified chart type is not compitible with Bar 3D chart;chartType</exception>
        public ExcelBar3DChart(ExcelChartType chartType)
        {
            if ((((((chartType != ExcelChartType.ColumnClustered3D) && (chartType != ExcelChartType.ColumnStacked3D)) && ((chartType != ExcelChartType.ColumnStacked100Percent3D) && (chartType != ExcelChartType.BarClustered3D))) && (((chartType != ExcelChartType.BarStacked3D) && (chartType != ExcelChartType.BarStacked100Percent3D)) && ((chartType != ExcelChartType.Column3D) && (chartType != ExcelChartType.CylinderColumnClustered)))) && ((((chartType != ExcelChartType.CylinderColumnStacked) && (chartType != ExcelChartType.CylinderColumnStacked100Percent)) && ((chartType != ExcelChartType.CylinderBarClustered) && (chartType != ExcelChartType.CylinderBarStacked))) && (((chartType != ExcelChartType.CylinderBarStacked100Percent) && (chartType != ExcelChartType.CylinderColumn3D)) && ((chartType != ExcelChartType.ConeColumnClustered) && (chartType != ExcelChartType.ConeColumnStacked))))) && (((((chartType != ExcelChartType.ConeColumnStacked100Percent) && (chartType != ExcelChartType.ConeBarClustered)) && ((chartType != ExcelChartType.ConeBarStacked) && (chartType != ExcelChartType.ConeBarStacked100Percent))) && (((chartType != ExcelChartType.ConeColumn3D) && (chartType != ExcelChartType.PyramidColumnClustered)) && ((chartType != ExcelChartType.PyramidColumnStacked) && (chartType != ExcelChartType.PyramidColumnStacked100Percent)))) && (((chartType != ExcelChartType.PyramidBarClustered) && (chartType != ExcelChartType.PyramidBarStacked)) && ((chartType != ExcelChartType.PyramidBarStacked100Percent) && (chartType != ExcelChartType.PyramidColumn3D)))))
            {
                throw new ArgumentException("The specified chart type is not compitible with Bar 3D chart", "chartType");
            }
            this.ChartType = chartType;
            this.VaryColors = false;
            this.GapDepth = 150;
            this.GapWidth = 150;
        }

        private ExcelChartType GetBarChartType(string barDir, string barGrouping, string shape)
        {
            if ((shape == null) || (shape == "box"))
            {
                if (barDir != "col")
                {
                    if (barDir == "bar")
                    {
                        if (barGrouping == "clustered")
                        {
                            return ExcelChartType.BarClustered3D;
                        }
                        if (barGrouping == "stacked")
                        {
                            return ExcelChartType.BarStacked3D;
                        }
                        if (barGrouping == "percentStacked")
                        {
                            return ExcelChartType.BarStacked100Percent3D;
                        }
                    }
                }
                else
                {
                    if (barGrouping == "clustered")
                    {
                        return ExcelChartType.ColumnClustered3D;
                    }
                    if (barGrouping == "stacked")
                    {
                        return ExcelChartType.ColumnStacked3D;
                    }
                    if (barGrouping == "percentStacked")
                    {
                        return ExcelChartType.ColumnStacked100Percent3D;
                    }
                    if (barGrouping == "standard")
                    {
                        return ExcelChartType.Column3D;
                    }
                }
            }
            else if (shape == "pyramid")
            {
                if (barDir != "col")
                {
                    if (barDir == "bar")
                    {
                        if (barGrouping == "clustered")
                        {
                            return ExcelChartType.PyramidBarClustered;
                        }
                        if (barGrouping == "stacked")
                        {
                            return ExcelChartType.PyramidBarStacked;
                        }
                        if (barGrouping == "percentStacked")
                        {
                            return ExcelChartType.PyramidBarStacked100Percent;
                        }
                    }
                }
                else
                {
                    if (barGrouping == "clustered")
                    {
                        return ExcelChartType.PyramidColumnClustered;
                    }
                    if (barGrouping == "stacked")
                    {
                        return ExcelChartType.PyramidColumnStacked;
                    }
                    if (barGrouping == "percentStacked")
                    {
                        return ExcelChartType.PyramidColumnStacked100Percent;
                    }
                    if (barGrouping == "standard")
                    {
                        return ExcelChartType.PyramidColumn3D;
                    }
                }
            }
            else if (shape == "cone")
            {
                if (barDir != "col")
                {
                    if (barDir == "bar")
                    {
                        if (barGrouping == "clustered")
                        {
                            return ExcelChartType.ConeBarClustered;
                        }
                        if (barGrouping == "stacked")
                        {
                            return ExcelChartType.ConeBarStacked;
                        }
                        if (barGrouping == "percentStacked")
                        {
                            return ExcelChartType.ConeBarStacked100Percent;
                        }
                    }
                }
                else
                {
                    if (barGrouping == "clustered")
                    {
                        return ExcelChartType.ConeColumnClustered;
                    }
                    if (barGrouping == "stacked")
                    {
                        return ExcelChartType.ConeColumnStacked;
                    }
                    if (barGrouping == "percentStacked")
                    {
                        return ExcelChartType.ConeColumnStacked100Percent;
                    }
                    if (barGrouping == "standard")
                    {
                        return ExcelChartType.ConeColumn3D;
                    }
                }
            }
            else if (shape == "cylinder")
            {
                if (barDir == "col")
                {
                    if (barGrouping == "clustered")
                    {
                        return ExcelChartType.CylinderColumnClustered;
                    }
                    if (barGrouping == "stacked")
                    {
                        return ExcelChartType.CylinderColumnStacked;
                    }
                    if (barGrouping == "percentStacked")
                    {
                        return ExcelChartType.CylinderColumnStacked100Percent;
                    }
                    if (barGrouping == "standard")
                    {
                        return ExcelChartType.CylinderColumn3D;
                    }
                }
                else if (barDir == "bar")
                {
                    if (barGrouping == "clustered")
                    {
                        return ExcelChartType.CylinderBarClustered;
                    }
                    if (barGrouping == "stacked")
                    {
                        return ExcelChartType.CylinderBarStacked;
                    }
                    if (barGrouping == "percentStacked")
                    {
                        return ExcelChartType.CylinderBarStacked100Percent;
                    }
                }
            }
            return ExcelChartType.ColumnClustered;
        }

        private string GetBarDirection(ExcelChartType chartType)
        {
            if (((((chartType != ExcelChartType.ColumnClustered3D) && (chartType != ExcelChartType.ColumnStacked3D)) && ((chartType != ExcelChartType.ColumnStacked100Percent3D) && (chartType != ExcelChartType.Column3D))) && (((chartType != ExcelChartType.CylinderColumnClustered) && (chartType != ExcelChartType.CylinderColumnStacked)) && ((chartType != ExcelChartType.CylinderColumnStacked100Percent) && (chartType != ExcelChartType.CylinderColumn3D)))) && ((((chartType != ExcelChartType.ConeColumnClustered) && (chartType != ExcelChartType.ConeColumnStacked)) && ((chartType != ExcelChartType.ConeColumnStacked100Percent) && (chartType != ExcelChartType.ConeColumn3D))) && (((chartType != ExcelChartType.PyramidColumnClustered) && (chartType != ExcelChartType.PyramidColumnStacked)) && ((chartType != ExcelChartType.PyramidColumnStacked100Percent) && (chartType != ExcelChartType.PyramidColumn3D)))))
            {
                return "bar";
            }
            return "col";
        }

        private string GetBarGrouping(ExcelChartType chartType)
        {
            if ((((chartType == ExcelChartType.BarClustered3D) || (chartType == ExcelChartType.ColumnClustered3D)) || ((chartType == ExcelChartType.PyramidBarClustered) || (chartType == ExcelChartType.PyramidColumnClustered))) || (((chartType == ExcelChartType.ConeColumnClustered) || (chartType == ExcelChartType.ConeBarClustered)) || ((chartType == ExcelChartType.CylinderBarClustered) || (chartType == ExcelChartType.CylinderColumnClustered))))
            {
                return "clustered";
            }
            if ((((chartType == ExcelChartType.ColumnStacked3D) || (chartType == ExcelChartType.BarStacked3D)) || ((chartType == ExcelChartType.PyramidBarStacked) || (chartType == ExcelChartType.PyramidColumnStacked))) || (((chartType == ExcelChartType.ConeBarStacked) || (chartType == ExcelChartType.ConeColumnStacked)) || ((chartType == ExcelChartType.CylinderBarStacked) || (chartType == ExcelChartType.CylinderColumnStacked))))
            {
                return "stacked";
            }
            if ((((chartType == ExcelChartType.BarStacked100Percent3D) || (chartType == ExcelChartType.ColumnStacked100Percent3D)) || ((chartType == ExcelChartType.PyramidColumnStacked100Percent) || (chartType == ExcelChartType.PyramidBarStacked100Percent))) || (((chartType == ExcelChartType.ConeColumnStacked100Percent) || (chartType == ExcelChartType.ConeBarStacked100Percent)) || ((chartType == ExcelChartType.CylinderColumnStacked100Percent) || (chartType == ExcelChartType.CylinderBarStacked100Percent))))
            {
                return "percentStacked";
            }
            if (((chartType != ExcelChartType.Column3D) && (chartType != ExcelChartType.CylinderColumn3D)) && ((chartType != ExcelChartType.PyramidColumn3D) && (chartType != ExcelChartType.ConeColumn3D)))
            {
                return "clustered";
            }
            return "standard";
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            string barDir = "col";
            string barGrouping = "clustered";
            string shape = null;
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
                else if (element.Name.LocalName == "gapDepth")
                {
                    this.GapDepth = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
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
                else if (element.Name.LocalName == "shape")
                {
                    shape = element.GetAttributeValueOrDefaultOfStringType("val", "box");
                }
            }
            this.ChartType = this.GetBarChartType(barDir, barGrouping, shape);
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            if ((this.AxisX == null) || (this.AxisY == null))
            {
                throw new InvalidOperationException("XAxis and YAxis cannot be null");
            }
            writer.WriteStartElement("c", "bar3DChart", null);
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
            writer.WriteLeafElementWithAttribute("gapWidth", null, "c", "val", ((int) this.GapWidth).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            writer.WriteLeafElementWithAttribute("gapDepth", null, "c", "val", ((int) this.GapDepth).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            string str = "box";
            string str2 = this.ChartType.ToString().ToLowerInvariant();
            if (str2.StartsWith("cylinder"))
            {
                str = "cylinder";
            }
            else if (str2.StartsWith("cone"))
            {
                str = "cone";
            }
            else if (str2.StartsWith("pyramid"))
            {
                str = "pyramid";
            }
            if (!string.IsNullOrWhiteSpace(str))
            {
                writer.WriteLeafElementWithAttribute("shape", null, "c", "val", str);
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
            if ((this.AxisZ == null) && !string.IsNullOrWhiteSpace(str))
            {
                writer.WriteLeafElementWithAttribute("axId", null, "c", "val", "0");
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
        /// Represeents the series axis info.
        /// </summary>
        public IExcelChartAxis AxisZ { get; set; }

        /// <summary>
        /// Specifies the series collection on the bar chart.
        /// </summary>
        public List<IExcelBarSeries> BarSeries
        {
            get
            {
                if (this.barSeries == null)
                {
                    this.barSeries = new List<IExcelBarSeries>();
                }
                return this.barSeries;
            }
            set { this.barSeries = value; }
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
        public int GapDepth { get; set; }

        /// <summary>
        /// Specifies the space between bar or column clusters, as a percentage of the bar or column width.
        /// </summary>
        /// <remarks>
        /// the value should be between 0 to 500
        /// </remarks>
        public int GapWidth { get; set; }

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

