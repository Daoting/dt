#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
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
    /// Represents IExcel chart instance.
    /// </summary>
    public class ExcelChart : IExcelChart
    {
        private List<int> _alternateContentChoiceList;
        private List<int> _alternateFallbackList;
        private static Dictionary<int, IExcelChartAxis> _axisCollection = new Dictionary<int, IExcelChartAxis>();
        private int _defaultStyleIndex = -1;
        private HashSet<int> firstChartAxisCollection = new HashSet<int>();
        private HashSet<int> secondaryChartAxisCollection = new HashSet<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelChart" /> class.
        /// </summary>
        public ExcelChart()
        {
            this.PlotVisibleOnly = true;
            this.DisplayBlanksAs = DisplayBlankAs.Zero;
            this.ShowDataLabelsOverMaximun = true;
            this.ShowAutoTitle = true;
            this.IsDate1904 = true;
            this.RoundedCorners = true;
            this.Locked = true;
            this.Hidden = false;
            this._defaultStyleIndex = -1;
        }

        private void EnsureAxisId(IExcelChartAxis axis)
        {
            if (axis.Id == 0)
            {
                axis.Id = this.GenerateAxisId();
                _axisCollection.Add(axis.Id, axis);
            }
            else if (_axisCollection.ContainsKey(axis.Id))
            {
                if (_axisCollection[axis.Id] == null)
                {
                    axis.Id = this.GenerateAxisId();
                    _axisCollection.Add(axis.Id, axis);
                }
            }
            else
            {
                _axisCollection.Add(axis.Id, axis);
            }
        }

        private int GenerateAxisId()
        {
            Random random = new Random();
            int num = 0;
            do
            {
                num = random.Next(0x989680, 0x5f5e0ff);
            }
            while (_axisCollection.ContainsKey(num));
            return num;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            this.secondaryChartAxisCollection.Clear();
            this.firstChartAxisCollection.Clear();
            if (node != null)
            {
                foreach (XElement element in node.Elements())
                {
                    if (element.Name.LocalName == "plotVisOnly")
                    {
                        this.PlotVisibleOnly = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                    }
                    else if (element.Name.LocalName == "dispBlanksAs")
                    {
                        switch (element.GetAttributeValueOrDefaultOfStringType("val", "zero"))
                        {
                            case "zero":
                                this.DisplayBlanksAs = DisplayBlankAs.Zero;
                                break;

                            case "gap":
                                this.DisplayBlanksAs = DisplayBlankAs.Gap;
                                break;

                            case "span":
                                this.DisplayBlanksAs = DisplayBlankAs.Span;
                                break;
                        }
                    }
                    else if (element.Name.LocalName == "title")
                    {
                        ExcelChartTitle title = new ExcelChartTitle();
                        title.ReadXml(element, mFolder, xFile);
                        this.ChartTitle = title;
                        this.ShowAutoTitle = false;
                    }
                    else if (element.Name.LocalName == "showDLblsOverMax")
                    {
                        this.ShowDataLabelsOverMaximun = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                    }
                    else if (element.Name.LocalName == "autoTitleDeleted")
                    {
                        this.ShowAutoTitle = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                    }
                    else if (element.Name.LocalName == "view3D")
                    {
                        Dt.Xls.Chart.ViewIn3D ind = new Dt.Xls.Chart.ViewIn3D();
                        ind.ReadXml(element);
                        this.ViewIn3D = ind;
                    }
                    else if (element.Name.LocalName == "floor")
                    {
                        ExcelWall wall = new ExcelWall();
                        wall.ReadXml(element, mFolder, xFile);
                        this.FloorWall = wall;
                    }
                    else if (element.Name.LocalName == "sideWall")
                    {
                        ExcelWall wall2 = new ExcelWall();
                        wall2.ReadXml(element, mFolder, xFile);
                        this.SideWall = wall2;
                    }
                    else if (element.Name.LocalName == "backWall")
                    {
                        ExcelWall wall3 = new ExcelWall();
                        wall3.ReadXml(element, mFolder, xFile);
                        this.BackWall = wall3;
                    }
                    else if (element.Name.LocalName == "legend")
                    {
                        ExcelChartLegend legend = new ExcelChartLegend();
                        legend.ReadXml(element, mFolder, xFile);
                        this.Legend = legend;
                    }
                    else if (element.Name.LocalName == "plotArea")
                    {
                        Dictionary<int, IExcelChartAxis> axises = new Dictionary<int, IExcelChartAxis>();
                        foreach (XElement element2 in element.Elements())
                        {
                            if (element2.Name.LocalName == "layout")
                            {
                                Layout layout = new Layout();
                                layout.ReadXml(element2, mFolder, xFile);
                                this.PlotAreaLayout = layout;
                            }
                            else if (element2.Name.LocalName == "dTable")
                            {
                                ExcelChartDataTable table = new ExcelChartDataTable();
                                table.ReadXml(element2, mFolder, xFile);
                                this.DataTable = table;
                            }
                            else if (element2.Name.LocalName == "catAx")
                            {
                                ExcelChartCategoryAxis axis = new ExcelChartCategoryAxis();
                                axis.ReadXml(element2, mFolder, xFile);
                                axises.Add(axis.Id, axis);
                            }
                            else if (element2.Name.LocalName == "valAx")
                            {
                                ExcelChartValueAxis axis2 = new ExcelChartValueAxis();
                                axis2.ReadXml(element2, mFolder, xFile);
                                axises.Add(axis2.Id, axis2);
                            }
                            else if (element2.Name.LocalName == "dateAx")
                            {
                                ExcelChartDateAxis axis3 = new ExcelChartDateAxis();
                                axis3.ReadXml(element2, mFolder, xFile);
                                axises.Add(axis3.Id, axis3);
                            }
                            else if (element2.Name.LocalName == "serAx")
                            {
                                ExcelChartSeriesAxis axis4 = new ExcelChartSeriesAxis();
                                axis4.ReadXml(element2, mFolder, xFile);
                                axises.Add(axis4.Id, axis4);
                            }
                            if (element2.Name.LocalName == "barChart")
                            {
                                ExcelBarChart chart = new ExcelBarChart();
                                chart.ReadXml(element2, mFolder, xFile);
                                if (this.secondaryChartAxisCollection.Contains(chart.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart.YAxisID.Value))
                                {
                                    this.SecondaryChart.BarChart = chart;
                                }
                                else if (this.BarChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.BarChart = chart;
                                        this.firstChartAxisCollection.Add(chart.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart.YAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart.YAxisID.Value))
                                    {
                                        this.BarChart = chart;
                                        this.firstChartAxisCollection.Add(chart.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart.YAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.BarChart = chart;
                                        this.secondaryChartAxisCollection.Add(chart.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart.YAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.BarChart = chart;
                                    this.secondaryChartAxisCollection.Add(chart.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart.YAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "bar3DChart")
                            {
                                ExcelBar3DChart chart2 = new ExcelBar3DChart();
                                chart2.ReadXml(element2, mFolder, xFile);
                                if (this.secondaryChartAxisCollection.Contains(chart2.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart2.YAxisID.Value))
                                {
                                    this.SecondaryChart.Bar3DChart = chart2;
                                }
                                else if (this.Bar3DChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.Bar3DChart = chart2;
                                        this.firstChartAxisCollection.Add(chart2.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart2.YAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart2.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart2.YAxisID.Value))
                                    {
                                        this.Bar3DChart = chart2;
                                        this.firstChartAxisCollection.Add(chart2.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart2.YAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.Bar3DChart = chart2;
                                        this.secondaryChartAxisCollection.Add(chart2.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart2.YAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.Bar3DChart = chart2;
                                    this.secondaryChartAxisCollection.Add(chart2.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart2.YAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "areaChart")
                            {
                                ExcelAreaChart chart3 = new ExcelAreaChart();
                                chart3.ReadXml(element2, mFolder, xFile);
                                if (this.secondaryChartAxisCollection.Contains(chart3.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart3.YAxisID.Value))
                                {
                                    this.SecondaryChart.AreaChart = chart3;
                                }
                                else if (this.AreaChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.AreaChart = chart3;
                                        this.firstChartAxisCollection.Add(chart3.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart3.YAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart3.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart3.YAxisID.Value))
                                    {
                                        this.AreaChart = chart3;
                                        this.firstChartAxisCollection.Add(chart3.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart3.YAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.AreaChart = chart3;
                                        this.secondaryChartAxisCollection.Add(chart3.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart3.YAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.AreaChart = chart3;
                                    this.secondaryChartAxisCollection.Add(chart3.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart3.YAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "area3DChart")
                            {
                                ExcelArea3DChart chart4 = new ExcelArea3DChart();
                                chart4.ReadXml(element2, mFolder, xFile);
                                if (this.secondaryChartAxisCollection.Contains(chart4.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart4.YAxisID.Value))
                                {
                                    this.SecondaryChart.Area3DChart = chart4;
                                }
                                else if (this.Area3DChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.Area3DChart = chart4;
                                        this.firstChartAxisCollection.Add(chart4.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart4.YAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart4.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart4.YAxisID.Value))
                                    {
                                        this.Area3DChart = chart4;
                                        this.firstChartAxisCollection.Add(chart4.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart4.YAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.Area3DChart = chart4;
                                        this.secondaryChartAxisCollection.Add(chart4.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart4.YAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.Area3DChart = chart4;
                                    this.secondaryChartAxisCollection.Add(chart4.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart4.YAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "pieChart")
                            {
                                ExcelPieChart chart5 = new ExcelPieChart();
                                chart5.ReadXml(element2, mFolder, xFile);
                                if (this.PieChart == null)
                                {
                                    this.PieChart = chart5;
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.PieChart = chart5;
                                }
                            }
                            if (element2.Name.LocalName == "pie3DChart")
                            {
                                ExcelPie3DChart chart6 = new ExcelPie3DChart();
                                chart6.ReadXml(element2, mFolder, xFile);
                                if (this.Pie3DChart == null)
                                {
                                    this.Pie3DChart = chart6;
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.Pie3DChart = chart6;
                                }
                            }
                            if (element2.Name.LocalName == "ofPieChart")
                            {
                                ExcelOfPieChart chart7 = new ExcelOfPieChart();
                                chart7.ReadXml(element2, mFolder, xFile);
                                this.OfPieChart = chart7;
                            }
                            if (element2.Name.LocalName == "doughnutChart")
                            {
                                ExcelDoughnutChart chart8 = new ExcelDoughnutChart();
                                chart8.ReadXml(element2, mFolder, xFile);
                                this.DoughnutChart = chart8;
                            }
                            if (element2.Name.LocalName == "bubbleChart")
                            {
                                ExcelBubbleChart chart9 = new ExcelBubbleChart();
                                chart9.ReadXml(element2, mFolder, xFile);
                                if (this.secondaryChartAxisCollection.Contains(chart9.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart9.YAxisID.Value))
                                {
                                    this.SecondaryChart.BubbleChart = chart9;
                                }
                                else if (this.BubbleChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.BubbleChart = chart9;
                                        this.firstChartAxisCollection.Add(chart9.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart9.YAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart9.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart9.YAxisID.Value))
                                    {
                                        this.BubbleChart = chart9;
                                        this.firstChartAxisCollection.Add(chart9.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart9.YAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.BubbleChart = chart9;
                                        this.secondaryChartAxisCollection.Add(chart9.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart9.YAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.BubbleChart = chart9;
                                    this.secondaryChartAxisCollection.Add(chart9.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart9.YAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "surfaceChart")
                            {
                                ExcelSurfaceChart chart10 = new ExcelSurfaceChart();
                                chart10.ReadXml(element2, mFolder, xFile);
                                if (this.secondaryChartAxisCollection.Contains(chart10.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart10.YAxisID.Value))
                                {
                                    this.SecondaryChart.SurfaceChart = chart10;
                                }
                                else if (this.SurfaceChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.SurfaceChart = chart10;
                                        this.firstChartAxisCollection.Add(chart10.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart10.YAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart10.ZAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart10.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart10.YAxisID.Value))
                                    {
                                        this.SurfaceChart = chart10;
                                        this.firstChartAxisCollection.Add(chart10.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart10.YAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart10.ZAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.SurfaceChart = chart10;
                                        this.secondaryChartAxisCollection.Add(chart10.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart10.YAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart10.ZAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.SurfaceChart = chart10;
                                    this.secondaryChartAxisCollection.Add(chart10.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart10.YAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart10.ZAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "surface3DChart")
                            {
                                ExcelSurface3DChart chart11 = new ExcelSurface3DChart();
                                chart11.ReadXml(element2, mFolder, xFile);
                                if ((this.secondaryChartAxisCollection.Contains(chart11.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart11.YAxisID.Value)) && this.secondaryChartAxisCollection.Contains(chart11.ZAxisID.Value))
                                {
                                    this.SecondaryChart.Surface3DChart = chart11;
                                }
                                else if (this.Surface3DChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.Surface3DChart = chart11;
                                        this.firstChartAxisCollection.Add(chart11.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart11.YAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart11.ZAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart11.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart11.YAxisID.Value))
                                    {
                                        this.Surface3DChart = chart11;
                                        this.firstChartAxisCollection.Add(chart11.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart11.YAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart11.ZAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.Surface3DChart = chart11;
                                        this.secondaryChartAxisCollection.Add(chart11.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart11.YAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart11.ZAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.Surface3DChart = chart11;
                                    this.secondaryChartAxisCollection.Add(chart11.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart11.YAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart11.ZAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "lineChart")
                            {
                                ExcelLineChart chart12 = new ExcelLineChart();
                                chart12.ReadXml(element2, mFolder, xFile);
                                if (this.secondaryChartAxisCollection.Contains(chart12.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart12.YAxisID.Value))
                                {
                                    this.SecondaryChart.LineChart = chart12;
                                }
                                else if (this.LineChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.LineChart = chart12;
                                        this.firstChartAxisCollection.Add(chart12.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart12.YAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart12.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart12.YAxisID.Value))
                                    {
                                        this.LineChart = chart12;
                                        this.firstChartAxisCollection.Add(chart12.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart12.YAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.LineChart = chart12;
                                        this.secondaryChartAxisCollection.Add(chart12.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart12.YAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.LineChart = chart12;
                                    this.secondaryChartAxisCollection.Add(chart12.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart12.YAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "line3DChart")
                            {
                                ExcelLine3DChart chart13 = new ExcelLine3DChart();
                                chart13.ReadXml(element2, mFolder, xFile);
                                if ((this.secondaryChartAxisCollection.Contains(chart13.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart13.YAxisID.Value)) && this.secondaryChartAxisCollection.Contains(chart13.ZAxisID.Value))
                                {
                                    this.SecondaryChart.Line3DChart = chart13;
                                }
                                else if (this.Line3DChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.Line3DChart = chart13;
                                        this.firstChartAxisCollection.Add(chart13.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart13.YAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart13.ZAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart13.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart13.YAxisID.Value))
                                    {
                                        this.Line3DChart = chart13;
                                        this.firstChartAxisCollection.Add(chart13.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart13.YAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart13.ZAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.Line3DChart = chart13;
                                        this.secondaryChartAxisCollection.Add(chart13.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart13.YAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart13.ZAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.Line3DChart = chart13;
                                    this.secondaryChartAxisCollection.Add(chart13.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart13.YAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart13.ZAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "stockChart")
                            {
                                ExcelStockChart chart14 = new ExcelStockChart();
                                chart14.ReadXml(element2, mFolder, xFile);
                                if (this.secondaryChartAxisCollection.Contains(chart14.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart14.YAxisID.Value))
                                {
                                    this.SecondaryChart.StockChart = chart14;
                                }
                                else if (this.StockChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.StockChart = chart14;
                                        this.firstChartAxisCollection.Add(chart14.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart14.YAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart14.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart14.YAxisID.Value))
                                    {
                                        this.StockChart = chart14;
                                        this.firstChartAxisCollection.Add(chart14.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart14.YAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.StockChart = chart14;
                                        this.secondaryChartAxisCollection.Add(chart14.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart14.YAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.StockChart = chart14;
                                    this.secondaryChartAxisCollection.Add(chart14.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart14.YAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "scatterChart")
                            {
                                ExcelScatterChart chart15 = new ExcelScatterChart();
                                chart15.ReadXml(element2, mFolder, xFile);
                                if (this.secondaryChartAxisCollection.Contains(chart15.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart15.YAxisID.Value))
                                {
                                    this.SecondaryChart.ScatterChart = chart15;
                                }
                                else if (this.ScatterChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.ScatterChart = chart15;
                                        this.firstChartAxisCollection.Add(chart15.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart15.YAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart15.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart15.YAxisID.Value))
                                    {
                                        this.ScatterChart = chart15;
                                        this.firstChartAxisCollection.Add(chart15.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart15.YAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.ScatterChart = chart15;
                                        this.secondaryChartAxisCollection.Add(chart15.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart15.YAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.ScatterChart = chart15;
                                    this.secondaryChartAxisCollection.Add(chart15.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart15.YAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "radarChart")
                            {
                                ExcelRadarChart chart16 = new ExcelRadarChart();
                                chart16.ReadXml(element2, mFolder, xFile);
                                if (this.secondaryChartAxisCollection.Contains(chart16.XAxisID.Value) && this.secondaryChartAxisCollection.Contains(chart16.YAxisID.Value))
                                {
                                    this.SecondaryChart.RadarChart = chart16;
                                }
                                else if (this.RadarChart == null)
                                {
                                    if (this.firstChartAxisCollection.Count == 0)
                                    {
                                        this.RadarChart = chart16;
                                        this.firstChartAxisCollection.Add(chart16.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart16.YAxisID.Value);
                                    }
                                    else if (this.firstChartAxisCollection.Contains(chart16.XAxisID.Value) && this.firstChartAxisCollection.Contains(chart16.YAxisID.Value))
                                    {
                                        this.RadarChart = chart16;
                                        this.firstChartAxisCollection.Add(chart16.XAxisID.Value);
                                        this.firstChartAxisCollection.Add(chart16.YAxisID.Value);
                                    }
                                    else
                                    {
                                        if (this.SecondaryChart == null)
                                        {
                                            this.SecondaryChart = new ExcelChart();
                                        }
                                        this.SecondaryChart.RadarChart = chart16;
                                        this.secondaryChartAxisCollection.Add(chart16.XAxisID.Value);
                                        this.secondaryChartAxisCollection.Add(chart16.YAxisID.Value);
                                    }
                                }
                                else
                                {
                                    if (this.SecondaryChart == null)
                                    {
                                        this.SecondaryChart = new ExcelChart();
                                    }
                                    this.SecondaryChart.RadarChart = chart16;
                                    this.secondaryChartAxisCollection.Add(chart16.XAxisID.Value);
                                    this.secondaryChartAxisCollection.Add(chart16.YAxisID.Value);
                                }
                            }
                            if (element2.Name.LocalName == "spPr")
                            {
                                ExcelChartFormat format = new ExcelChartFormat();
                                format.ReadXml(element2, mFolder, xFile);
                                this.PlotAreaFormat = format;
                            }
                        }
                        this.UpdateAxises(this, axises);
                        this.UpdateAxises(this.SecondaryChart, axises);
                    }
                }
            }
        }

        private void UpdateAxises(IExcelChart chart, Dictionary<int, IExcelChartAxis> axises)
        {
            if ((chart != null) && (axises != null))
            {
                if (chart.BarChart != null)
                {
                    chart.BarChart.AxisX = axises[(chart.BarChart as ExcelBarChart).XAxisID.Value];
                    chart.BarChart.AxisY = axises[(chart.BarChart as ExcelBarChart).YAxisID.Value];
                }
                if (chart.AreaChart != null)
                {
                    chart.AreaChart.AxisX = axises[(chart.AreaChart as ExcelAreaChart).XAxisID.Value];
                    chart.AreaChart.AxisY = axises[(chart.AreaChart as ExcelAreaChart).YAxisID.Value];
                }
                if (chart.Bar3DChart != null)
                {
                    chart.Bar3DChart.AxisX = axises[(chart.Bar3DChart as ExcelBar3DChart).XAxisID.Value];
                    chart.Bar3DChart.AxisY = axises[(chart.Bar3DChart as ExcelBar3DChart).YAxisID.Value];
                    if ((chart.Bar3DChart as ExcelBar3DChart).ZAxisID.HasValue)
                    {
                        int num = (chart.Bar3DChart as ExcelBar3DChart).ZAxisID.Value;
                        if (axises.ContainsKey(num))
                        {
                            chart.Bar3DChart.AxisZ = axises[num];
                        }
                    }
                }
                if (chart.Area3DChart != null)
                {
                    chart.Area3DChart.AxisX = axises[(chart.Area3DChart as ExcelArea3DChart).XAxisID.Value];
                    chart.Area3DChart.AxisY = axises[(chart.Area3DChart as ExcelArea3DChart).YAxisID.Value];
                    if ((chart.Area3DChart as ExcelArea3DChart).ZAxisID.HasValue)
                    {
                        int num2 = (chart.Area3DChart as ExcelArea3DChart).ZAxisID.Value;
                        if (axises.ContainsKey(num2))
                        {
                            chart.Area3DChart.AxisZ = axises[num2];
                        }
                    }
                }
                if (chart.BubbleChart != null)
                {
                    chart.BubbleChart.AxisX = axises[(chart.BubbleChart as ExcelBubbleChart).XAxisID.Value];
                    chart.BubbleChart.AxisY = axises[(chart.BubbleChart as ExcelBubbleChart).YAxisID.Value];
                }
                if (chart.SurfaceChart != null)
                {
                    chart.SurfaceChart.AxisX = axises[(chart.SurfaceChart as ExcelSurfaceChart).XAxisID.Value];
                    chart.SurfaceChart.AxisY = axises[(chart.SurfaceChart as ExcelSurfaceChart).YAxisID.Value];
                    if ((chart.SurfaceChart as ExcelSurfaceChart).ZAxisID.HasValue)
                    {
                        int num3 = (chart.SurfaceChart as ExcelSurfaceChart).ZAxisID.Value;
                        if (axises.ContainsKey(num3))
                        {
                            chart.SurfaceChart.AxisZ = axises[num3];
                        }
                    }
                }
                if (chart.Surface3DChart != null)
                {
                    chart.Surface3DChart.AxisX = axises[(chart.Surface3DChart as ExcelSurface3DChart).XAxisID.Value];
                    chart.Surface3DChart.AxisY = axises[(chart.Surface3DChart as ExcelSurface3DChart).YAxisID.Value];
                    chart.Surface3DChart.AxisZ = axises[(chart.Surface3DChart as ExcelSurface3DChart).ZAxisID.Value];
                }
                if (chart.LineChart != null)
                {
                    chart.LineChart.AxisX = axises[(chart.LineChart as ExcelLineChart).XAxisID.Value];
                    chart.LineChart.AxisY = axises[(chart.LineChart as ExcelLineChart).YAxisID.Value];
                }
                if (chart.Line3DChart != null)
                {
                    chart.Line3DChart.AxisX = axises[(chart.Line3DChart as ExcelLine3DChart).XAxisID.Value];
                    chart.Line3DChart.AxisY = axises[(chart.Line3DChart as ExcelLine3DChart).YAxisID.Value];
                    chart.Line3DChart.AxisZ = axises[(chart.Line3DChart as ExcelLine3DChart).ZAxisID.Value];
                }
                if (chart.StockChart != null)
                {
                    chart.StockChart.AxisX = axises[(chart.StockChart as ExcelStockChart).XAxisID.Value];
                    chart.StockChart.AxisY = axises[(chart.StockChart as ExcelStockChart).YAxisID.Value];
                }
                if (chart.ScatterChart != null)
                {
                    chart.ScatterChart.AxisX = axises[(chart.ScatterChart as ExcelScatterChart).XAxisID.Value];
                    chart.ScatterChart.AxisY = axises[(chart.ScatterChart as ExcelScatterChart).YAxisID.Value];
                }
                if (chart.RadarChart != null)
                {
                    chart.RadarChart.AxisX = axises[(chart.RadarChart as ExcelRadarChart).XAxisID.Value];
                    chart.RadarChart.AxisY = axises[(chart.RadarChart as ExcelRadarChart).YAxisID.Value];
                }
            }
        }

        private void WriteArea3DChart(XmlWriter writer, IExcelArea3DChart area3DChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if ((area3DChart.AxisX == null) || (area3DChart.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null for AreaChart");
            }
            this.EnsureAxisId(area3DChart.AxisX);
            this.EnsureAxisId(area3DChart.AxisY);
            if (area3DChart.AxisZ != null)
            {
                this.EnsureAxisId(area3DChart.AxisZ);
            }
            if (area3DChart.AxisZ != null)
            {
                area3DChart.AxisX.CrossAx = area3DChart.AxisY.Id;
                area3DChart.AxisZ.CrossAx = area3DChart.AxisY.Id;
                area3DChart.AxisY.CrossAx = area3DChart.AxisX.Id;
            }
            else
            {
                area3DChart.AxisX.CrossAx = area3DChart.AxisY.Id;
                area3DChart.AxisY.CrossAx = area3DChart.AxisX.Id;
            }
            (area3DChart as ExcelArea3DChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(area3DChart.AxisX.Id, area3DChart.AxisX);
            axises.Add(area3DChart.AxisY.Id, area3DChart.AxisY);
            if (area3DChart.AxisZ != null)
            {
                axises.Add(area3DChart.AxisZ.Id, area3DChart.AxisZ);
            }
        }

        private void WriteAreaChart(XmlWriter writer, IExcelAreaChart areaChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if ((areaChart.AxisX == null) || (areaChart.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null for AreaChart");
            }
            this.EnsureAxisId(areaChart.AxisX);
            this.EnsureAxisId(areaChart.AxisY);
            areaChart.AxisX.CrossAx = areaChart.AxisY.Id;
            areaChart.AxisY.CrossAx = areaChart.AxisX.Id;
            (areaChart as ExcelAreaChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(areaChart.AxisX.Id, areaChart.AxisX);
            axises.Add(areaChart.AxisY.Id, areaChart.AxisY);
        }

        private void WriteBar3DChart(XmlWriter writer, IExcelBar3DChart bar3DChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if ((bar3DChart.AxisX == null) || (bar3DChart.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null for Bar Chart");
            }
            this.EnsureAxisId(bar3DChart.AxisX);
            this.EnsureAxisId(bar3DChart.AxisY);
            if (bar3DChart.AxisZ != null)
            {
                this.EnsureAxisId(bar3DChart.AxisZ);
            }
            if (bar3DChart.AxisZ != null)
            {
                bar3DChart.AxisX.CrossAx = bar3DChart.AxisY.Id;
                bar3DChart.AxisZ.CrossAx = bar3DChart.AxisY.Id;
                bar3DChart.AxisY.CrossAx = bar3DChart.AxisX.Id;
            }
            else
            {
                bar3DChart.AxisX.CrossAx = bar3DChart.AxisY.Id;
                bar3DChart.AxisY.CrossAx = bar3DChart.AxisX.Id;
            }
            (bar3DChart as ExcelBar3DChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(bar3DChart.AxisX.Id, bar3DChart.AxisX);
            axises.Add(bar3DChart.AxisY.Id, bar3DChart.AxisY);
            if (bar3DChart.AxisZ != null)
            {
                axises.Add(bar3DChart.AxisZ.Id, bar3DChart.AxisZ);
            }
        }

        private void WriteBarChart(XmlWriter writer, IExcelBarChart barChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if ((barChart.AxisX == null) || (barChart.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null for BarChart");
            }
            this.EnsureAxisId(barChart.AxisX);
            this.EnsureAxisId(barChart.AxisY);
            barChart.AxisX.CrossAx = barChart.AxisY.Id;
            barChart.AxisY.CrossAx = barChart.AxisX.Id;
            (barChart as ExcelBarChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(barChart.AxisX.Id, barChart.AxisX);
            axises.Add(barChart.AxisY.Id, barChart.AxisY);
        }

        private void WriteBubbleChart(XmlWriter writer, IExcelBubbleChart bubbleChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if ((bubbleChart.AxisX == null) || (bubbleChart.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null for BubbleChart");
            }
            this.EnsureAxisId(bubbleChart.AxisX);
            this.EnsureAxisId(bubbleChart.AxisY);
            bubbleChart.AxisX.CrossAx = bubbleChart.AxisY.Id;
            bubbleChart.AxisY.CrossAx = bubbleChart.AxisX.Id;
            (bubbleChart as ExcelBubbleChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(bubbleChart.AxisX.Id, bubbleChart.AxisX);
            axises.Add(bubbleChart.AxisY.Id, bubbleChart.AxisY);
        }

        private void WriteChart(XmlWriter writer, IExcelChart chart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if (chart != null)
            {
                if (chart.PieChart != null)
                {
                    (chart.PieChart as ExcelPieChart).WriteXml(writer, mFolder, chartFile);
                }
                if (chart.Pie3DChart != null)
                {
                    (chart.Pie3DChart as ExcelPie3DChart).WriteXml(writer, mFolder, chartFile);
                }
                if (chart.OfPieChart != null)
                {
                    (chart.OfPieChart as ExcelOfPieChart).WriteXml(writer, mFolder, chartFile);
                }
                if (chart.DoughnutChart != null)
                {
                    (chart.DoughnutChart as ExcelDoughnutChart).WriteXml(writer, mFolder, chartFile);
                }
                if (chart.BarChart != null)
                {
                    this.WriteBarChart(writer, chart.BarChart, mFolder, chartFile, axises);
                }
                if (chart.Bar3DChart != null)
                {
                    this.WriteBar3DChart(writer, chart.Bar3DChart, mFolder, chartFile, axises);
                }
                if (chart.AreaChart != null)
                {
                    this.WriteAreaChart(writer, chart.AreaChart, mFolder, chartFile, axises);
                }
                if (chart.Area3DChart != null)
                {
                    this.WriteArea3DChart(writer, chart.Area3DChart, mFolder, chartFile, axises);
                }
                if (chart.BubbleChart != null)
                {
                    this.WriteBubbleChart(writer, chart.BubbleChart, mFolder, chartFile, axises);
                }
                if (chart.SurfaceChart != null)
                {
                    this.WriteSurfaceChart(writer, chart.SurfaceChart, mFolder, chartFile, axises);
                }
                if (chart.Surface3DChart != null)
                {
                    this.WriteSurface3DChart(writer, chart.Surface3DChart, mFolder, chartFile, axises);
                }
                if (chart.LineChart != null)
                {
                    this.WriteLineChart(writer, chart.LineChart, mFolder, chartFile, axises);
                }
                if (chart.Line3DChart != null)
                {
                    this.WriteLine3DChart(writer, chart.Line3DChart, mFolder, chartFile, axises);
                }
                if (chart.StockChart != null)
                {
                    this.WriteStockChart(writer, chart.StockChart, mFolder, chartFile, axises);
                }
                if (chart.ScatterChart != null)
                {
                    this.WriteScatterChart(writer, chart.ScatterChart, mFolder, chartFile, axises);
                }
                if (chart.RadarChart != null)
                {
                    this.WriteRadarChart(writer, chart.RadarChart, mFolder, chartFile, axises);
                }
            }
        }

        private void WriteLine3DChart(XmlWriter writer, IExcelLine3DChart line3DChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if (((line3DChart.AxisX == null) || (line3DChart.AxisY == null)) || (line3DChart.AxisZ == null))
            {
                throw new InvalidOperationException("Both XAxis, YAxis and XAxis cannot be null for Line3D Chart.");
            }
            this.EnsureAxisId(line3DChart.AxisX);
            this.EnsureAxisId(line3DChart.AxisY);
            this.EnsureAxisId(line3DChart.AxisZ);
            line3DChart.AxisX.CrossAx = line3DChart.AxisZ.Id;
            line3DChart.AxisZ.CrossAx = line3DChart.AxisY.Id;
            line3DChart.AxisY.CrossAx = line3DChart.AxisX.Id;
            (line3DChart as ExcelLine3DChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(line3DChart.AxisX.Id, line3DChart.AxisX);
            axises.Add(line3DChart.AxisY.Id, line3DChart.AxisY);
            axises.Add(line3DChart.AxisZ.Id, line3DChart.AxisZ);
        }

        private void WriteLineChart(XmlWriter writer, IExcelLineChart lineChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if ((lineChart.AxisX == null) || (lineChart.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null for Line Chart");
            }
            this.EnsureAxisId(lineChart.AxisX);
            this.EnsureAxisId(lineChart.AxisY);
            lineChart.AxisX.CrossAx = lineChart.AxisY.Id;
            lineChart.AxisY.CrossAx = lineChart.AxisX.Id;
            (lineChart as ExcelLineChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(lineChart.AxisX.Id, lineChart.AxisX);
            axises.Add(lineChart.AxisY.Id, lineChart.AxisY);
        }

        private void WriteRadarChart(XmlWriter writer, IExcelRadarChart radarChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if ((radarChart.AxisX == null) || (radarChart.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null for RadarChart");
            }
            this.EnsureAxisId(radarChart.AxisX);
            this.EnsureAxisId(radarChart.AxisY);
            radarChart.AxisX.CrossAx = radarChart.AxisY.Id;
            radarChart.AxisY.CrossAx = radarChart.AxisX.Id;
            (radarChart as ExcelRadarChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(radarChart.AxisX.Id, radarChart.AxisX);
            axises.Add(radarChart.AxisY.Id, radarChart.AxisY);
        }

        private void WriteScatterChart(XmlWriter writer, IExcelScatterChart scatterChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if ((scatterChart.AxisX == null) || (scatterChart.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null for ScatterChart");
            }
            this.EnsureAxisId(scatterChart.AxisX);
            this.EnsureAxisId(scatterChart.AxisY);
            scatterChart.AxisX.CrossAx = scatterChart.AxisY.Id;
            scatterChart.AxisY.CrossAx = scatterChart.AxisX.Id;
            (scatterChart as ExcelScatterChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(scatterChart.AxisX.Id, scatterChart.AxisX);
            axises.Add(scatterChart.AxisY.Id, scatterChart.AxisY);
        }

        private void WriteStockChart(XmlWriter writer, IExcelStockChart stockChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if ((stockChart.AxisX == null) || (stockChart.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null for StockChart");
            }
            this.EnsureAxisId(stockChart.AxisX);
            this.EnsureAxisId(stockChart.AxisY);
            stockChart.AxisX.CrossAx = stockChart.AxisY.Id;
            stockChart.AxisY.CrossAx = stockChart.AxisX.Id;
            (stockChart as ExcelStockChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(stockChart.AxisX.Id, stockChart.AxisX);
            axises.Add(stockChart.AxisY.Id, stockChart.AxisY);
        }

        private void WriteSurface3DChart(XmlWriter writer, IExcelSurface3DChart surface3DChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if (((surface3DChart.AxisX == null) || (surface3DChart.AxisY == null)) || (surface3DChart.AxisZ == null))
            {
                throw new InvalidOperationException("Both XAxis ,YAxis and ZAxis cannot be null for Surface3DChart");
            }
            this.EnsureAxisId(surface3DChart.AxisX);
            this.EnsureAxisId(surface3DChart.AxisY);
            this.EnsureAxisId(surface3DChart.AxisZ);
            surface3DChart.AxisX.CrossAx = surface3DChart.AxisZ.Id;
            surface3DChart.AxisZ.CrossAx = surface3DChart.AxisY.Id;
            surface3DChart.AxisY.CrossAx = surface3DChart.AxisX.Id;
            (surface3DChart as ExcelSurface3DChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(surface3DChart.AxisX.Id, surface3DChart.AxisX);
            axises.Add(surface3DChart.AxisY.Id, surface3DChart.AxisY);
            axises.Add(surface3DChart.AxisZ.Id, surface3DChart.AxisZ);
        }

        private void WriteSurfaceChart(XmlWriter writer, IExcelSurfaceChart surfaceChart, MemoryFolder mFolder, XFile chartFile, Dictionary<int, IExcelChartAxis> axises)
        {
            if ((surfaceChart.AxisX == null) || (surfaceChart.AxisY == null))
            {
                throw new InvalidOperationException("Both XAxis and YAxis cannot be null for SurfaceChart");
            }
            this.EnsureAxisId(surfaceChart.AxisX);
            this.EnsureAxisId(surfaceChart.AxisY);
            if (surfaceChart.AxisZ != null)
            {
                this.EnsureAxisId(surfaceChart.AxisZ);
            }
            if (surfaceChart.AxisZ != null)
            {
                surfaceChart.AxisX.CrossAx = surfaceChart.AxisZ.Id;
                surfaceChart.AxisZ.CrossAx = surfaceChart.AxisY.Id;
                surfaceChart.AxisY.CrossAx = surfaceChart.AxisX.Id;
            }
            else
            {
                surfaceChart.AxisX.CrossAx = surfaceChart.AxisY.Id;
                surfaceChart.AxisY.CrossAx = surfaceChart.AxisX.Id;
            }
            (surfaceChart as ExcelSurfaceChart).WriteXml(writer, mFolder, chartFile);
            axises.Add(surfaceChart.AxisX.Id, surfaceChart.AxisX);
            axises.Add(surfaceChart.AxisY.Id, surfaceChart.AxisY);
            if (surfaceChart.AxisZ != null)
            {
                axises.Add(surfaceChart.AxisZ.Id, surfaceChart.AxisZ);
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            _axisCollection.Clear();
            writer.WriteStartElement("c", "chart", null);
            if (this.ChartTitle != null)
            {
                (this.ChartTitle as ExcelChartTitle).WriteXml(writer, mFolder, chartFile);
            }
            if (!this.ShowAutoTitle)
            {
                writer.WriteLeafElementWithAttribute("autoTitleDeleted", null, "c", "val", "0");
            }
            if (this.ViewIn3D != null)
            {
                this.ViewIn3D.WriteXml(writer, mFolder, chartFile);
            }
            if (this.FloorWall != null)
            {
                (this.FloorWall as ExcelWall).WriteXml(writer, mFolder, chartFile, "floor");
            }
            if (this.SideWall != null)
            {
                (this.SideWall as ExcelWall).WriteXml(writer, mFolder, chartFile, "sideWall");
            }
            if (this.BackWall != null)
            {
                (this.BackWall as ExcelWall).WriteXml(writer, mFolder, chartFile, "backWall");
            }
            writer.WriteStartElement("c", "plotArea", null);
            if (this.PlotAreaLayout != null)
            {
                this.PlotAreaLayout.WriteXml(writer, mFolder, chartFile);
            }
            Dictionary<int, IExcelChartAxis> axises = new Dictionary<int, IExcelChartAxis>();
            this.WriteChart(writer, this, mFolder, chartFile, axises);
            this.WriteChart(writer, this.SecondaryChart, mFolder, chartFile, axises);
            foreach (IExcelChartAxis axis in axises.Values)
            {
                if (axis is IExcelChartCategoryAxis)
                {
                    (axis as ExcelChartCategoryAxis).WriteXml(writer, mFolder, chartFile);
                }
                else if (axis is IExcelChartValueAxis)
                {
                    (axis as ExcelChartValueAxis).WriteXml(writer, mFolder, chartFile);
                }
                else if (axis is IExcelChartDateAxis)
                {
                    (axis as ExcelChartDateAxis).WriteXml(writer, mFolder, chartFile);
                }
                else if (axis is IExcelChartSeriesAxis)
                {
                    (axis as ExcelChartSeriesAxis).WriteXml(writer, mFolder, chartFile);
                }
            }
            if (this.DataTable != null)
            {
                (this.DataTable as ExcelChartDataTable).WriteXml(writer, mFolder, chartFile);
            }
            if (this.PlotAreaFormat != null)
            {
                (this.PlotAreaFormat as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
            }
            writer.WriteEndElement();
            if (this.Legend != null)
            {
                (this.Legend as ExcelChartLegend).WriteXml(writer, mFolder, chartFile);
            }
            writer.WriteLeafElementWithAttribute("plotVisOnly", null, "c", "val", this.PlotVisibleOnly ? "1" : "0");
            string str = "zero";
            switch (this.DisplayBlanksAs)
            {
                case DisplayBlankAs.Span:
                    str = "span";
                    break;

                case DisplayBlankAs.Gap:
                    str = "gap";
                    break;

                default:
                    str = "zero";
                    break;
            }
            writer.WriteLeafElementWithAttribute("dispBlanksAs", null, "c", "val", str);
            writer.WriteLeafElementWithAttribute("showDLblsOverMax", null, "c", "val", this.ShowDataLabelsOverMaximun ? "1" : "0");
            writer.WriteEndElement();
        }

        /// <summary>
        /// Represents alternate enhanced functionality content the chart will use if the application understand it.
        /// </summary>
        /// <remarks>
        /// There may be multiple choices in the lest and the first understandable one will be used.
        /// </remarks>
        public List<int> AlternateContentChoiceStyleList
        {
            get
            {
                if (this._alternateContentChoiceList == null)
                {
                    this._alternateContentChoiceList = new List<int>();
                }
                return this._alternateContentChoiceList;
            }
            set { this._alternateContentChoiceList = value; }
        }

        /// <summary>
        /// Represents fall back choice if all alternate choice are not recognized by cosuming application.
        /// </summary>
        public List<int> AlternateFallbackStyleList
        {
            get
            {
                if (this._alternateFallbackList == null)
                {
                    this._alternateFallbackList = new List<int>();
                }
                return this._alternateFallbackList;
            }
            set { this._alternateFallbackList = value; }
        }

        /// <summary>
        /// Specifies the chart location.
        /// </summary>
        public IAnchor Anchor { get; set; }

        /// <summary>
        /// Specifies there is a area 3D chart.
        /// </summary>
        public IExcelArea3DChart Area3DChart { get; set; }

        /// <summary>
        /// Specifies there is a area chart
        /// </summary>
        public IExcelAreaChart AreaChart { get; set; }

        /// <summary>
        /// Specifies the back wall of the chart
        /// </summary>
        public IExcelWall BackWall { get; set; }

        /// <summary>
        /// Specifies there is a 3D Bar or column series on the chart.
        /// </summary>
        public IExcelBar3DChart Bar3DChart { get; set; }

        /// <summary>
        /// Specifies there is a Bar or column series on the chart.
        /// </summary>
        public IExcelBarChart BarChart { get; set; }

        /// <summary>
        /// Specifies there is a bubble chart.
        /// </summary>
        public IExcelBubbleChart BubbleChart { get; set; }

        /// <summary>
        /// Represents the format settings for the chart area.
        /// </summary>
        public IExcelChartFormat ChartFormat { get; set; }

        /// <summary>
        /// Specifies a title.
        /// </summary>
        public IExcelChartTitle ChartTitle { get; set; }

        /// <summary>
        /// Represents chart table settings
        /// </summary>
        public IExcelChartDataTable DataTable { get; set; }

        /// <summary>
        /// Specifies the default formatting for all chart elements.
        /// </summary>
        /// <remarks>
        /// The valid value should be between 1 to 48 and 0 means not set.
        /// </remarks>
        public int DefaultStyleIndex
        {
            get { return  this._defaultStyleIndex; }
            set
            {
                if ((value < 1) || (value > 0x30))
                {
                    throw new ArgumentOutOfRangeException();
                }
                this._defaultStyleIndex = value;
            }
        }

        /// <summary>
        /// Specifies how blank cells shall be plotted on a chart.
        /// </summary>
        public DisplayBlankAs DisplayBlanksAs { get; set; }

        /// <summary>
        /// Specifies there is a doughnut chart.
        /// </summary>
        public IExcelDoughnutChart DoughnutChart { get; set; }

        /// <summary>
        /// Specifies the floor of the chart.
        /// </summary>
        public IExcelWall FloorWall { get; set; }

        /// <summary>
        /// Represents whether the chart is visible or hidden
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Specifies that the chart use the 1904 date system.
        /// </summary>
        public bool IsDate1904 { get; set; }

        /// <summary>
        /// Specifies the legend.
        /// </summary>
        public IExcelChartLegend Legend { get; set; }

        /// <summary>
        /// Specifies there is a surface 3D chart.
        /// </summary>
        public IExcelLine3DChart Line3DChart { get; set; }

        /// <summary>
        /// Specifies there is a surface chart
        /// </summary>
        public IExcelLineChart LineChart { get; set; }

        /// <summary>
        /// Represents whether the chart is locked
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Represents the chart name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specifies there is a bar of pie or pie of pie chart
        /// </summary>
        public IExcelOfPieChart OfPieChart { get; set; }

        /// <summary>
        /// Specifies there is a pie 3d chart.
        /// </summary>
        public IExcelPie3DChart Pie3DChart { get; set; }

        /// <summary>
        /// Specifies there is a pie chart
        /// </summary>
        public IExcelPieChart PieChart { get; set; }

        /// <summary>
        /// Represents the format settings for the plot area.
        /// </summary>
        public IExcelChartFormat PlotAreaFormat { get; set; }

        /// <summary>
        /// Represents the layout of the  plot area
        /// </summary>
        public Layout PlotAreaLayout { get; set; }

        /// <summary>
        /// Specifies that only visible cells shall be plotted on the chart
        /// </summary>
        /// <remarks>
        /// The default value is true
        /// </remarks>
        public bool PlotVisibleOnly { get; set; }

        /// <summary>
        /// Specifies there is a radar chart.
        /// </summary>
        public IExcelRadarChart RadarChart { get; set; }

        /// <summary>
        /// Specifies the chart area shll have rounded corners.
        /// </summary>
        public bool RoundedCorners { get; set; }

        /// <summary>
        /// Specifies there is a scatter chart.
        /// </summary>
        public IExcelScatterChart ScatterChart { get; set; }

        /// <summary>
        /// Represents the second chart in the same plot area.
        /// </summary>
        public IExcelChart SecondaryChart { get; set; }

        /// <summary>
        /// Specifies whether shown chart title or not.
        /// </summary>
        public bool ShowAutoTitle { get; set; }

        /// <summary>
        /// Specifies data labels over the maximun of the chart shall be shown.
        /// </summary>
        public bool ShowDataLabelsOverMaximun { get; set; }

        /// <summary>
        /// Specifies the side wall of the chart.
        /// </summary>
        public IExcelWall SideWall { get; set; }

        /// <summary>
        /// Specifies there is a stock chart.
        /// </summary>
        public IExcelStockChart StockChart { get; set; }

        /// <summary>
        /// Specifies there is a surface 3D chart.
        /// </summary>
        public IExcelSurface3DChart Surface3DChart { get; set; }

        /// <summary>
        /// Specifies there is a surface chart
        /// </summary>
        public IExcelSurfaceChart SurfaceChart { get; set; }

        /// <summary>
        /// Represents the text format settings for the chart area
        /// </summary>
        public IExcelTextFormat TextFormat { get; set; }

        /// <summary>
        /// Specifies the 3-D view of the chart.
        /// </summary>
        public Dt.Xls.Chart.ViewIn3D ViewIn3D { get; set; }
    }
}

