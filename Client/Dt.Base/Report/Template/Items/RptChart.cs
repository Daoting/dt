#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Charts;
using System;
using System.Threading.Tasks;
using System.Xml;
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
            _data.AddCell("showlegend", true);
            // 图例标题
            _data.AddCell<string>("legtitle");
            // 图例位置
            _data.AddCell("legpos", "Right");
            // 布局方向
            _data.AddCell("legorientation", "Vertical");
            // 重叠方式
            _data.AddCell<bool>("legoverlap");
            // 标题
            _data.AddCell<string>("title");
            // X轴标题
            _data.AddCell<string>("titlex");
            // Y轴标题
            _data.AddCell<string>("titley");
            // 转换XY轴
            _data.AddCell<bool>("axisinverted");
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

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Chart");
            WritePosition(p_writer);

            string val = _data.Str("type");
            if (val != string.Empty && val != "Column")
                p_writer.WriteAttributeString("type", val);
            val = _data.Str("palette");
            if (val != string.Empty && val != "Default")
                p_writer.WriteAttributeString("palette", val);

            if (!_data.Bool("showlegend"))
                p_writer.WriteAttributeString("showlegend", "False");
            val = _data.Str("legtitle");
            if (val != string.Empty)
                p_writer.WriteAttributeString("legtitle", val);
            val = _data.Str("legpos");
            if (val != string.Empty && val != "Right")
                p_writer.WriteAttributeString("legpos", val);
            val = _data.Str("legorientation");
            if (val != string.Empty && val != "Vertical")
                p_writer.WriteAttributeString("legorientation", val);
            if (_data.Bool("legoverlap"))
                p_writer.WriteAttributeString("legoverlap", "True");
            val = _data.Str("title");
            if (val != string.Empty)
                p_writer.WriteAttributeString("title", val);
            val = _data.Str("titlex");
            if (val != string.Empty)
                p_writer.WriteAttributeString("titlex", val);
            val = _data.Str("titley");
            if (val != string.Empty)
                p_writer.WriteAttributeString("titley", val);
            if (_data.Bool("axisinverted"))
                p_writer.WriteAttributeString("axisinverted", "True");

            val = _data.Str("tbl");
            if (val != string.Empty)
                p_writer.WriteAttributeString("tbl", val);
            val = _data.Str("fieldseries");
            if (val != string.Empty)
                p_writer.WriteAttributeString("fieldseries", val);
            val = _data.Str("fieldx");
            if (val != string.Empty)
                p_writer.WriteAttributeString("fieldx", val);
            val = _data.Str("fieldy");
            if (val != string.Empty)
                p_writer.WriteAttributeString("fieldy", val);
            val = _data.Str("fieldz");
            if (val != string.Empty)
                p_writer.WriteAttributeString("fieldz", val);

            p_writer.WriteEndElement();
        }
    }
}
