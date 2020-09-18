#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Charts;
using Dt.Core;
using System;
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
            // 调色板
            _data.AddCell("palette", "Default");
            // 图例是否可见
            _data.AddCell("showlegend", "1");
            // 图例标题
            _data.AddCell<string>("legtitle");
            // 图例位置
            _data.AddCell("legpos", "Right");
            // 布局方向
            _data.AddCell("legorientation", "Vertical");
            // 重叠方式
            _data.AddCell("legoverlap", "0");
            // 标题
            _data.AddCell<string>("title");
            // X轴标题
            _data.AddCell<string>("titlex");
            // Y轴标题
            _data.AddCell<string>("titley");
            // 转换XY轴
            _data.AddCell("axisinverted", "0");
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

        /// <summary>
        /// 获取序列化时标签名称
        /// </summary>
        public override string XmlName
        {
            get { return "Chart"; }
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
        public override void Build()
        {
            _part.Inst.Body.AddChild(new RptChartInst(this));
        }

        /// <summary>
        /// 根据参数设置创建图表实例
        /// </summary>
        /// <returns></returns>
        public Chart CreateChart()
        {
            Chart ct = new Chart();
            ct.Width = Width;
            ct.Height = Height;

            ChartType tp = ChartType.Column;
            Enum.TryParse<ChartType>(_data.Str("type"), out tp);
            ct.ChartType = tp;

            Palette pal = Palette.Default;
            Enum.TryParse<Palette>(_data.Str("palette"), out pal);
            ct.Palette = pal;

            if (_data.Bool("showlegend"))
            {
                ChartLegend lg = new ChartLegend();
                lg.Title = _data.Str("legtitle");

                LegendPosition pos = LegendPosition.Right;
                Enum.TryParse<LegendPosition>(_data.Str("legpos"), out pos);
                lg.Position = pos;

                Orientation ori = Orientation.Vertical;
                Enum.TryParse<Orientation>(_data.Str("legorientation"), out ori);
                lg.Orientation = ori;
                lg.OverlapChart = _data.Bool("legoverlap");
                ct.Children.Add(lg);
            }

            string title = _data.Str("title");
            if (!string.IsNullOrEmpty(title))
                ct.Header = title;

            title = _data.Str("titlex");
            if (!string.IsNullOrEmpty(title))
                ct.View.AxisX.Title = title;

            title = _data.Str("titley");
            if (!string.IsNullOrEmpty(title))
                ct.View.AxisY.Title = title;

            ct.View.Inverted = _data.Bool("axisinverted");
            return ct;
        }
    }
}
