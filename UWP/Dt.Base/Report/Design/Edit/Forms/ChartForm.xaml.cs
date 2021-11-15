#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Charts;
using Dt.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class ChartForm : UserControl
    {
        RptDesignInfo _info;
        RptChart _item;
        ChartData _defaultData;
        ChartData _ganttData;

        public ChartForm(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
        }

        internal void LoadItem(RptChart p_item)
        {
            if (_item == p_item)
                return;

            _item = p_item;
            Kit.RunAsync(() =>
            {
                _item.Data.Changed -= OnValueChanged;
                _fv.Data = _item.Data;
                _item.Data.Changed += OnValueChanged;
                InitChart();

                string dataSourceName = _item.Data.Str("tbl");
                if (string.IsNullOrEmpty(dataSourceName))
                {
                    // 如果未选中唯一的数据源，默认选中
                    var ls = _item.Root.Data.DataSet;
                    if (ls != null && ls.Count == 1)
                    {
                        // 使用initVal 避免重做撤消命令产生冗余操作。
                        _item.Data.InitVal("tbl", ls[0].Str("name"));
                        DataDropBox(ls[0].Str("name"));
                    }
                }
                else
                {
                    for (int i = 0; i < _item.Root.Data.DataSet.Count; i++)
                    {
                        if (_item.Root.Data.DataSet[i].Str("name") == dataSourceName)
                        {
                            DataDropBox(dataSourceName);
                            break;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 数据项发生变化事件响应函数。
        /// 注意：字段值变化未校验数据类型。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnValueChanged(object sender, Cell e)
        {
            _chart.BeginUpdate();

            switch (e.ID)
            {
                case "type":
                    Enum.TryParse<ChartType>(_item.Data.Str("type"), out var tp);
                    _chart.ChartType = tp;
                    _chart.Data = GetData(tp);
                    ChgFldByType(tp);
                    break;

                case "title":
                    _chart.Header = e.GetVal<string>();
                    break;

                case "tbl":
                    _item.Data["fieldseries"] = "";
                    _item.Data["fieldx"] = "";
                    _item.Data["fieldy"] = "";
                    _item.Data["fieldz"] = "";
                    DataDropBox(e.GetVal<string>());
                    break;

                case "showlegend":
                    for (int i = 0; i < _chart.Children.Count; i++)
                    {
                        if (_chart.Children[i] is ChartLegend)
                        {
                            _chart.Children[i].Visibility = e.GetVal<bool>() ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                    break;

                case "legtitle":
                    for (int i = 0; i < _chart.Children.Count; i++)
                    {
                        if (_chart.Children[i] is ChartLegend)
                        {
                            (_chart.Children[i] as ChartLegend).Title = e.Val;
                        }
                    }
                    break;

                case "legpos":
                    LegendPosition pos = LegendPosition.Right;
                    Enum.TryParse<LegendPosition>(e.GetVal<string>(), out pos);
                    for (int i = 0; i < _chart.Children.Count; i++)
                    {
                        if (_chart.Children[i] is ChartLegend)
                        {
                            (_chart.Children[i] as ChartLegend).Position = pos;
                        }
                    }
                    break;

                case "legorientation":
                    Orientation ori;
                    Enum.TryParse<Orientation>(e.GetVal<string>(), out ori);
                    for (int i = 0; i < _chart.Children.Count; i++)
                    {
                        if (_chart.Children[i] is ChartLegend)
                        {
                            (_chart.Children[i] as ChartLegend).Orientation = ori;
                        }
                    }
                    break;

                case "titlex":
                    _chart.View.AxisX.Title = e.GetVal<string>();
                    break;

                case "titley":
                    _chart.View.AxisY.Title = e.GetVal<string>();
                    break;

                default:
                    break;
            }

            _chart.EndUpdate();
        }

        /// <summary>
        /// 根据数据源名称，初始化字段的数据源项目。
        /// </summary>
        /// <param name="p_dsName"></param>
        void DataDropBox(string p_dsName)
        {
            // 数据源改变，关系到列的内容全部改变。
            var dtCols = _item.Root.Data.GetColsData(p_dsName);
            ((CList)_fv["fieldseries"]).Data = dtCols;
            ((CList)_fv["fieldx"]).Data = dtCols;
            ((CList)_fv["fieldy"]).Data = dtCols;
            ((CList)_fv["fieldz"]).Data = dtCols;
        }

        /// <summary>
        /// 根据图表类型，改变数据字段项表现。
        /// </summary>
        /// <param name="p_chartType"></param>
        void ChgFldByType(ChartType p_chartType)
        {
            // 常规图
            _fv["fieldseries"].Title = "系列字段";
            _fv["fieldX"].Title = "分类字段";
            _fv["fieldY"].Title = "值字段";
            _fv["fieldX"].Visibility = Visibility.Visible;
            _fv["fieldY"].Visibility = Visibility.Visible;
            _fv["fieldZ"].Visibility = Visibility.Collapsed;
            // 甘特图
            if (p_chartType == ChartType.Gantt)
            {
                _fv["fieldseries"].Title = "任务分类字段";
                _fv["fieldX"].Title = "任务字段";
                _fv["fieldY"].Title = "起始时间字段";
                _fv["fieldZ"].Title = "终止时间字段";
                _fv["fieldZ"].Visibility = Visibility.Visible;
            }
        }

        #region Chart
        /// <summary>
        /// 初始化报表
        /// </summary>
        void InitChart()
        {
            // 数据源下拉列表初始化
            ((CList)_fv["tbl"]).Data = _item.Root.Data.DataSet;

            _chart.BeginUpdate();
            Enum.TryParse<ChartType>(_item.Data.Str("type"), out var tp);
            Enum.TryParse<Orientation>(_item.Data.Str("legorientation"), out var ori);
            Enum.TryParse<LegendPosition>(_item.Data.Str("legpos"), out var pos);
            _chart.ChartType = tp;
            ChgFldByType(tp);
            _chart.Data = GetData(tp);

            _chart.Header = _item.Data.Str("title");
            for (int i = 0; i < _chart.Children.Count; i++)
            {
                if (_chart.Children[i] is ChartLegend)
                {
                    // 加载初始数据值，改变legend表现样式。
                    ChartLegend legend = _chart.Children[i] as ChartLegend;
                    legend.Visibility = _item.Data.Bool("showlegend") ? Visibility.Visible : Visibility.Collapsed;
                    legend.Title = _item.Data.Str("legtitle");
                    legend.Position = pos;
                    legend.Orientation = ori;
                }
            }
            _chart.View.AxisX.Title = _item.Data.Str("titlex");
            _chart.View.AxisY.Title = _item.Data.Str("titley");
            _chart.EndUpdate();
        }

        ChartData GetData(ChartType ct)
        {
            if (ct == ChartType.Gantt)
                return GanttData;

            return DefaultData;
        }

        /// <summary>
        /// 适合于大部分图表使用
        /// </summary>
        ChartData DefaultData
        {
            get
            {
                if (_defaultData == null)
                {
                    _defaultData = new ChartData();
                    _defaultData.ItemNames = "语文 数学 英语 物理 化学";
                    DataSeries ds1 = new DataSeries() { Label = "张平" };
                    ds1.ValuesSource = new double[] { 93, 82, 97, 74, 88 };
                    _defaultData.Children.Add(ds1);

                    DataSeries ds2 = new DataSeries() { Label = "李华强" };
                    ds2.ValuesSource = new double[] { 91, 92, 93, 94, 96 };
                    _defaultData.Children.Add(ds2);

                    DataSeries ds3 = new DataSeries() { Label = "王力国" };
                    ds3.ValuesSource = new double[] { 100, 91, 76, 62, 63 };
                    _defaultData.Children.Add(ds3);
                }

                return _defaultData;
            }
        }

        ChartData GanttData
        {
            get
            {
                if (_ganttData == null)
                {
                    _ganttData = new ChartData();
                    _ganttData.ItemNames = new string[] { "Task1", "Task2", "Task3", "Task4" };

                    HighLowSeries ds1 = new HighLowSeries() { Label = "Task1" };
                    ds1.XValuesSource = new double[] { 0 };
                    ds1.LowValuesSource = new DateTime[] { new DateTime(2008, 10, 1) };
                    ds1.HighValuesSource = new DateTime[] { new DateTime(2008, 10, 5) };
                    _ganttData.Children.Add(ds1);

                    HighLowSeries ds2 = new HighLowSeries() { Label = "Task2" };
                    ds2.XValuesSource = new double[] { 1 };
                    ds2.LowValuesSource = new DateTime[] { new DateTime(2008, 10, 3) };
                    ds2.HighValuesSource = new DateTime[] { new DateTime(2008, 10, 4) };
                    _ganttData.Children.Add(ds2);

                    HighLowSeries ds3 = new HighLowSeries() { Label = "Task3" };
                    ds3.XValuesSource = new double[] { 2 };
                    ds3.LowValuesSource = new DateTime[] { new DateTime(2008, 10, 2) };
                    ds3.HighValuesSource = new DateTime[] { new DateTime(2008, 10, 8) };
                    _ganttData.Children.Add(ds3);

                    HighLowSeries ds4 = new HighLowSeries() { Label = "Task4" };
                    ds4.XValuesSource = new double[] { 3 };
                    ds4.LowValuesSource = new DateTime[] { new DateTime(2008, 10, 4) };
                    ds4.HighValuesSource = new DateTime[] { new DateTime(2008, 10, 7) };
                    _ganttData.Children.Add(ds4);

                    foreach (DataSeries ds in _ganttData.Children)
                    {
                        ds.SymbolSize = new Size(30, 30);
                    }
                }

                return _ganttData;
            }
        }
        #endregion
    }
}
