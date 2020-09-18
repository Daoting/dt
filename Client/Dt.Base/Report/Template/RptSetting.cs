#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 页面设置
    /// </summary>
    internal class RptSetting
    {
        Row _data;

        public RptSetting()
        {
            Table tbl = new Table();
            var cols = tbl.Columns;
            cols.Add(new Column("PaperName", typeof(string)));
            cols.Add(new Column("Height", typeof(double)));
            cols.Add(new Column("Width", typeof(double)));
            cols.Add(new Column("LeftMargin", typeof(double)));
            cols.Add(new Column("TopMargin", typeof(double)));
            cols.Add(new Column("RightMargin", typeof(double)));
            cols.Add(new Column("BottomMargin", typeof(double)));
            cols.Add(new Column("Landscape", typeof(string)));

            _data = tbl.NewRow(new 
            {
                PaperName = "IsoA4",
                Height = 1122,
                Width = 793,
                LeftMargin = 36,
                TopMargin = 36,
                RightMargin = 36,
                BottomMargin = 36,
                Landscape = "0",
            });
            tbl.Add(_data);
        }

        /// <summary>
        /// 获取页面设置数据源
        /// </summary>
        public Row Data
        {
            get { return _data; }
        }

        /// <summary>
        /// 获取纸张名称
        /// </summary>
        public string PaperName
        {
            set { _data["PaperName"] = value; }
            get { return _data.Str("PaperName"); }
        }

        /// <summary>
        /// 获取高度
        /// </summary>
        public double Height
        {
            set { _data["Height"] = value; }
            get { return _data.Int("Height"); }
        }

        /// <summary>
        /// 获取宽度
        /// </summary>
        public double Width
        {
            set { _data["Width"] = value; }
            get { return _data.Int("Width"); }
        }

        /// <summary>
        /// 获取左边距
        /// </summary>
        public double LeftMargin
        {
            set { _data["LeftMargin"] = value; }
            get { return _data.Int("LeftMargin"); }
        }

        /// <summary>
        /// 获取上边距
        /// </summary>
        public double TopMargin
        {
            set { _data["TopMargin"] = value; }
            get { return _data.Int("TopMargin"); }
        }

        /// <summary>
        /// 获取右边距
        /// </summary>
        public double RightMargin
        {
            set { _data["RightMargin"] = value; }
            get { return _data.Int("RightMargin"); }
        }

        /// <summary>
        /// 获取下边距
        /// </summary>
        public double BottomMargin
        {
            set { _data["BottomMargin"] = value; }
            get { return _data.Int("BottomMargin"); }
        }

        /// <summary>
        /// 获取是否横向
        /// </summary>
        public bool Landscape
        {
            set { _data["Landscape"] = value ? "1" : "0"; }
            get { return _data.Bool("Landscape"); }
        }

        /// <summary>
        /// 获取有效高度
        /// </summary>
        public double ValidHeight
        {
            get { return Landscape ? Width - TopMargin - BottomMargin : Height - TopMargin - BottomMargin; }
        }

        /// <summary>
        /// 获取有效宽度
        /// </summary>
        public double ValidWidth
        {
            get { return Landscape ? Height - LeftMargin - RightMargin : Width - LeftMargin - RightMargin; }
        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public void ReadXml(XmlReader p_reader)
        {
            _data.ReadXml(p_reader);
            p_reader.Read();
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        public void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Setting");
            _data.WriteXml(p_writer);
            p_writer.WriteEndElement();
        }
    }
}
