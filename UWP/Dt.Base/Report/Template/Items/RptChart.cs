#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using Dt.Charts;
using Dt.Core;
using System;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 图表
    /// </summary>
    internal class RptChart : RptItem
    {
        public RptChart(RptPart p_owner)
            : base(p_owner)
        {
            // 图表类型
            _data.AddCell("type", "Column");
            // 图例是否可见
            _data.AddCell("showlegend", true);
            // 图例标题
            _data.AddCell<string>("legtitle");
            // 图例位置
            _data.AddCell("legpos", "Right");
            // 布局方向
            _data.AddCell("legorientation", "Vertical");
            // 标题
            _data.AddCell<string>("title");
            // X轴标题
            _data.AddCell<string>("titlex");
            // Y轴标题
            _data.AddCell<string>("titley");
            // 数据源名称
            _data.AddCell<string>("tbl");
            // 系列字段名
            _data.AddCell<string>("fieldseries");
            // x轴字段名
            _data.AddCell<string>("fieldx");
            // y轴字段名
            _data.AddCell<string>("fieldy");
            //  扩展字段
            _data.AddCell<string>("fieldz");
        }

        /// <summary>
        /// 获取数据源名称
        /// </summary>
        public string Tbl
        {
            get { return _data.Str("tbl"); }
        }

        /// <summary>
        /// 获取x轴字段名
        /// </summary>
        public string FieldX
        {
            get { return _data.Str("fieldx"); }
        }

        /// <summary>
        /// 获取y轴字段名
        /// </summary>
        public string FieldY
        {
            get { return _data.Str("fieldy"); }
        }

        public string FieldZ
        {
            get { return _data.Str("fieldz"); }
        }

        /// <summary>
        /// 获取系列字段名
        /// </summary>
        public string FieldSeries
        {
            get { return _data.Str("fieldseries"); }
        }

        public bool ShowLegend
        {
            get { return _data.Bool("showlegend"); }
        }

        /// <summary>
        /// 拷贝对象方法
        /// </summary>
        /// <returns></returns>
        public override RptItem Clone()
        {
            RptChart newOne = new RptChart(_part);
            newOne._data.Copy(_data);
            return newOne;
        }

        /// <summary>
        /// 构造报表项实例
        /// </summary>
        public override async Task Build()
        {
            RptRootInst inst = _part.Inst;
            string tblName = _data.Str("tbl");
            if (string.IsNullOrEmpty(tblName))
                return;

            // 使用时再加载数据
            var rptData = await inst.Info.GetData(tblName);
            if (rptData == null)
                return;

            // 无数据不加载
            _part.Inst.Body.AddChild(new RptChartInst(this));
        }

        /// <summary>
        /// 输出图表
        /// </summary>
        /// <param name="p_ws"></param>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        public void Render(Worksheet p_ws, int p_row, int p_col)
        {
            if (!ValidFilds())
                return;

            Rect rc = p_ws.GetRangeLocation(new CellRange(p_row, p_col, RowSpan, ColSpan));
            var chartType = GetChartType();
            SpreadChart c = p_ws.AddChart("chart" + p_ws.Charts.Count.ToString(), chartType, rc.Left, rc.Top, rc.Width, rc.Height);
            // 锁定图表，禁止拖动缩放
            c.Locked = true;

            string title = _data.Str("title");
            if (!string.IsNullOrEmpty(title))
                c.ChartTitle = new ChartTitle { Text = title };

            title = _data.Str("titlex");
            if (!string.IsNullOrEmpty(title))
                c.AxisX.Title = new ChartTitle { Text = title };

            title = _data.Str("titley");
            if (!string.IsNullOrEmpty(title))
                c.AxisY.Title = new ChartTitle { Text = title };

            if (ShowLegend)
            {
                c.Legend.Text = _data.Str("legtitle");
                if (Enum.TryParse<Dt.Charts.LegendPosition>(_data.Str("legpos"), out var pos))
                    c.Legend.Alignment = GetLegendAlignment(pos);
                if (Enum.TryParse<Orientation>(_data.Str("legorientation"), out var ori))
                    c.Legend.Orientation = ori;
            }
            else
            {
                c.Legend = null;
            }

            if (string.IsNullOrEmpty(FieldSeries))
                LoadTable(c);
            else
                LoadMatrix(c);
        }

        void LoadMatrix(SpreadChart p_chart)
        {
            // Build()中已判断空的情况
            RptData data = _part.Inst.Info.GetData(Tbl).Result;
            var tbl = data.Data.CreateMatrix(FieldX, FieldSeries, FieldY);
            for (int i = 0; i < tbl.Columns.Count; i++)
            {
                string colName = tbl.Columns[i].ID;
                if (colName == FieldX)
                    continue;

                SpreadDataSeries ser = new SpreadDataSeries();
                ser.Name = colName;
                DoubleSeriesCollection vals = new DoubleSeriesCollection();
                foreach (var row in tbl)
                {
                    try
                    {
                        double val = row.Double(i);
                        vals.Add(val);
                    }
                    catch
                    {
                        vals.Add(0);
                    }
                }
                ser.Values = vals;
                p_chart.DataSeries.Add(ser);
            }

            // 添加系列数据时已创建x轴Items，只能重新设置x轴
            p_chart.AxisX.Items.Clear();
            for (int i = 0; i < tbl.Count; i++)
            {
                p_chart.AxisX.Items.Add(tbl[i].Str(FieldX));
            }
        }

        void LoadTable(SpreadChart p_chart)
        {
            RptData data = _part.Inst.Info.GetData(Tbl).Result;
            SpreadDataSeries ser = new SpreadDataSeries();
            DoubleSeriesCollection vals = new DoubleSeriesCollection();
            foreach (var row in data.Data)
            {
                try
                {
                    double val = row.Double(FieldY);
                    vals.Add(val);
                }
                catch
                {
                    vals.Add(0);
                }
            }
            ser.Values = vals;
            p_chart.DataSeries.Add(ser);

            p_chart.AxisX.Items.Clear();
            foreach (var row in data.Data)
            {
                p_chart.AxisX.Items.Add(row.Str(FieldX));
            }
        }

        /// <summary>
        /// 判断数据表字段是否完整
        /// </summary>
        /// <returns></returns>
        bool ValidFilds()
        {
            if (string.IsNullOrEmpty(Tbl))
            {
                Kit.Msg("数据源不可为空。");
                return false;
            }

            string type = _data.Str("type");
            if (type == "Gantt")
            {
                if (string.IsNullOrEmpty(FieldZ)
                    || string.IsNullOrEmpty(FieldX)
                    || string.IsNullOrEmpty(FieldY))
                {
                    Kit.Msg("任务字段、起始时间字段及终止时间字段均不可为空，图表生成失败。");
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(FieldX)
                    || string.IsNullOrEmpty(FieldY))
                {
                    Kit.Msg("分类字段和值字段不可为空，图表生成失败。");
                    return false;
                }
            }
            return true;
        }

        SpreadChartType GetChartType()
        {
            if (Enum.TryParse<ChartType>(_data.Str("type"), out var tp))
            {
                switch (tp)
                {
                    case ChartType.Bar:
                        return SpreadChartType.BarClustered;

                    case ChartType.BarStacked:
                        return SpreadChartType.BarStacked;

                    case ChartType.Column:
                        return SpreadChartType.ColumnClustered;

                    case ChartType.ColumnStacked:
                        return SpreadChartType.ColumnStacked;

                    case ChartType.Line:
                        return SpreadChartType.Line;

                    case ChartType.LineSmoothed:
                        return SpreadChartType.LineSmoothed;

                    case ChartType.LineStacked:
                        return SpreadChartType.LineStacked;

                    case ChartType.LineSymbols:
                        return SpreadChartType.LineWithMarkers;

                    case ChartType.LineSymbolsSmoothed:
                        return SpreadChartType.LineWithMarkersSmoothed;

                    case ChartType.LineSymbolsStacked:
                        return SpreadChartType.LineStackedWithMarkers;

                    case ChartType.Pie:
                        return SpreadChartType.Pie;

                    case ChartType.PieExploded:
                        return SpreadChartType.PieExploded;

                    case ChartType.PieDoughnut:
                        return SpreadChartType.PieDoughnut;

                    case ChartType.PieExplodedDoughnut:
                        return SpreadChartType.PieExplodedDoughnut;

                    case ChartType.Area:
                        return SpreadChartType.Area;

                    case ChartType.AreaStacked:
                        return SpreadChartType.AreaStacked;

                    case ChartType.AreaStacked100pc:
                        return SpreadChartType.AreaStacked;

                    case ChartType.Radar:
                        return SpreadChartType.Radar;

                    case ChartType.RadarSymbols:
                        return SpreadChartType.RadarWithMarkers;

                    case ChartType.RadarFilled:
                        return SpreadChartType.RadarFilled;

                    case ChartType.XYPlot:
                        return SpreadChartType.Scatter;

                    case ChartType.Bubble:
                        return SpreadChartType.Bubble;

                    case ChartType.Candle:
                        return SpreadChartType.StockHighLowOpenClose;
                }
            }
            return SpreadChartType.ColumnClustered;
        }

        LegendAlignment GetLegendAlignment(Charts.LegendPosition pos)
        {
            switch (pos)
            {
                case Charts.LegendPosition.TopLeft:
                    return LegendAlignment.TopLeft;

                case Charts.LegendPosition.TopRight:
                    return LegendAlignment.TopRight;

                case Charts.LegendPosition.TopCenter:
                    return LegendAlignment.TopCenter;

                case Charts.LegendPosition.Left:
                    return LegendAlignment.MiddleLeft;

                case Charts.LegendPosition.Right:
                    return LegendAlignment.MiddleRight;

                case Charts.LegendPosition.BottomLeft:
                    return LegendAlignment.BottomLeft;

                case Charts.LegendPosition.BottomCenter:
                    return LegendAlignment.BottomCenter;

                case Charts.LegendPosition.BottomRight:
                    return LegendAlignment.BottomRight;
            }
            return LegendAlignment.BottomCenter;
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Chart");
            WritePosition(p_writer);

            string val = _data.Str("type");
            if (val != "" && val != "Column")
                p_writer.WriteAttributeString("type", val);

            if (!_data.Bool("showlegend"))
                p_writer.WriteAttributeString("showlegend", "False");
            val = _data.Str("legtitle");
            if (val != "")
                p_writer.WriteAttributeString("legtitle", val);
            val = _data.Str("legpos");
            if (val != "" && val != "Right")
                p_writer.WriteAttributeString("legpos", val);
            val = _data.Str("legorientation");
            if (val != "" && val != "Vertical")
                p_writer.WriteAttributeString("legorientation", val);
            val = _data.Str("title");
            if (val != "")
                p_writer.WriteAttributeString("title", val);
            val = _data.Str("titlex");
            if (val != "")
                p_writer.WriteAttributeString("titlex", val);
            val = _data.Str("titley");
            if (val != "")
                p_writer.WriteAttributeString("titley", val);

            val = _data.Str("tbl");
            if (val != "")
                p_writer.WriteAttributeString("tbl", val);
            val = _data.Str("fieldseries");
            if (val != "")
                p_writer.WriteAttributeString("fieldseries", val);
            val = _data.Str("fieldx");
            if (val != "")
                p_writer.WriteAttributeString("fieldx", val);
            val = _data.Str("fieldy");
            if (val != "")
                p_writer.WriteAttributeString("fieldy", val);
            val = _data.Str("fieldz");
            if (val != "")
                p_writer.WriteAttributeString("fieldz", val);

            p_writer.WriteEndElement();
        }
    }
}
