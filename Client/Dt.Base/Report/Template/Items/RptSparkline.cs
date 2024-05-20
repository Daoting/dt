#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-16 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 迷你图
    /// </summary>
    public class RptSparkline : RptItem
    {
        public RptSparkline(RptPart p_owner)
            : base(p_owner)
        {
            // 类型
            _data.Add("type", "Line");
            // 数据源名称
            _data.Add<string>("tbl");
            // 数值字段名
            _data.Add<string>("field");
        }

        /// <summary>
        /// 获取数据源名称
        /// </summary>
        public string Tbl
        {
            get { return _data.Str("tbl"); }
        }

        /// <summary>
        /// 获取数值字段名
        /// </summary>
        public string Field
        {
            get { return _data.Str("field"); }
        }

        public override RptItem Clone()
        {
            var newOne = new RptSparkline(_part);
            newOne._data.Copy(_data);
            return newOne;
        }

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
            _part.Inst.Body.AddChild(new RptSparklineInst(this));
        }

        public void Render(Worksheet p_ws, int p_row, int p_col)
        {
            var cell = p_ws[p_row, p_col];
            cell.ColumnSpan = ColSpan;
            cell.RowSpan = RowSpan;

            if (!ValidFilds())
                return;

            RptData data = _part.Inst.Info.GetData(Tbl).Result;

            // 增加隐藏列放置目标数据
            int cntCol = p_ws.Columns.Count;
            p_ws.AddColumns(cntCol, 1);
            // 隐藏列
            p_ws.Columns[cntCol].IsVisible = false;
            int cntRow = p_ws.Rows.Count;
            if (data.Data.Count > cntRow)
            {
                // 补充缺少行
                p_ws.AddRows(p_ws.Rows.Count, data.Data.Count - cntRow);
                for (int i = cntRow; i < p_ws.Rows.Count; i++)
                {
                    // 隐藏行
                    p_ws.Rows[i].IsVisible = false;
                }
            }

            // 隐藏列赋值
            var id = Field;
            for (int i = 0; i < data.Data.Count; i++)
            {
                p_ws[i, cntCol].Value = data.Data[i].Double(id);
            }
            
            p_ws.SetSparkline(
                p_row,
                p_col,
                new CellRange(0, cntCol, data.Data.Count, 1),
                DataOrientation.Vertical,
                GetChartType(),
                new SparklineSetting()
                {
                    // 显示隐藏的数据
                    DisplayHidden = true,
                    // x轴，数值在0上下时可见
                    DisplayXAxis = true,
                    ShowMarkers = true,
                    LineWeight = 3,
                    ShowFirst = true,
                    ShowLast = true,
                    ShowLow = true,
                    ShowHigh = true,
                    ShowNegative = true
                });
        }

        bool ValidFilds()
        {
            if (string.IsNullOrEmpty(Tbl))
            {
                Kit.Msg("数据源不可为空。");
                return false;
            }

            if (string.IsNullOrEmpty(Field))
            {
                Kit.Msg("数值字段不可为空，迷你图生成失败。");
                return false;
            }
            return true;
        }
        
        SparklineType GetChartType()
        {
            if (Enum.TryParse<SparklineType>(_data.Str("type"), out var tp))
            {
                return tp;
            }
            return SparklineType.Line;
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Sparkline");
            WritePosition(p_writer);

            string val = _data.Str("type");
            if (val != "" && val != "Line")
                p_writer.WriteAttributeString("type", val);

            val = _data.Str("tbl");
            if (val != "")
                p_writer.WriteAttributeString("tbl", val);
            val = _data.Str("field");
            if (val != "")
                p_writer.WriteAttributeString("field", val);

            p_writer.WriteEndElement();
        }
    }
}