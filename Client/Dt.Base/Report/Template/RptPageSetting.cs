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
    internal class RptPageSetting
    {
        const double _defaultMargin = 36;
        const double _defaultHeight = 1122;
        const double _defaultWidth = 793;

        public RptPageSetting(RptRoot p_root)
        {
            Root = p_root;
            Data = new Row();
            // 纸张名称不保存
            Data.AddCell<string>("papername");
            Data.AddCell("height", _defaultHeight);
            Data.AddCell("width", _defaultWidth);
            Data.AddCell("leftmargin", _defaultMargin);
            Data.AddCell("topmargin", _defaultMargin);
            Data.AddCell("rightmargin", _defaultMargin);
            Data.AddCell("bottommargin", _defaultMargin);
            Data.AddCell<bool>("landscape");
            Data.Changed += Root.OnCellValueChanged;
        }

        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public RptRoot Root { get; }

        /// <summary>
        /// 获取页面设置数据源
        /// </summary>
        public Row Data { get; }

        /// <summary>
        /// 获取纸张名称
        /// </summary>
        public string PaperName
        {
            set { Data["PaperName"] = value; }
            get { return Data.Str("PaperName"); }
        }

        /// <summary>
        /// 获取高度
        /// </summary>
        public double Height
        {
            set { Data["Height"] = value; }
            get { return Data.Int("Height"); }
        }

        /// <summary>
        /// 获取宽度
        /// </summary>
        public double Width
        {
            set { Data["Width"] = value; }
            get { return Data.Int("Width"); }
        }

        /// <summary>
        /// 获取左边距
        /// </summary>
        public double LeftMargin
        {
            set { Data["LeftMargin"] = value; }
            get { return Data.Int("LeftMargin"); }
        }

        /// <summary>
        /// 获取上边距
        /// </summary>
        public double TopMargin
        {
            set { Data["TopMargin"] = value; }
            get { return Data.Int("TopMargin"); }
        }

        /// <summary>
        /// 获取右边距
        /// </summary>
        public double RightMargin
        {
            set { Data["RightMargin"] = value; }
            get { return Data.Int("RightMargin"); }
        }

        /// <summary>
        /// 获取下边距
        /// </summary>
        public double BottomMargin
        {
            set { Data["BottomMargin"] = value; }
            get { return Data.Int("BottomMargin"); }
        }

        /// <summary>
        /// 获取是否横向
        /// </summary>
        public bool Landscape
        {
            set { Data["Landscape"] = value; }
            get { return Data.Bool("Landscape"); }
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

        public bool IsValid()
        {
            if (ValidHeight - Root.Header.ActualHeight - Root.Footer.ActualHeight <= 0)
            {
                Kit.Warn("报表的可用页面高度不足，请确认！");
                return false;
            }
            if (ValidWidth <= 0)
            {
                Kit.Warn("报表的可用页面宽度不足，请确认！");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public void ReadXml(XmlReader p_reader)
        {
            Data.ReadXml(p_reader);
            p_reader.Read();
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        public void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Page");
            // 不保存纸张名称
            //string val = Data.Str("papername");
            //if (val != "IsoA4")
            //    p_writer.WriteAttributeString("papername", val);
            if (Height != _defaultHeight)
                p_writer.WriteAttributeString("height", Height.ToString());
            if (Width != _defaultWidth)
                p_writer.WriteAttributeString("width", Width.ToString());
            if (LeftMargin != _defaultMargin)
                p_writer.WriteAttributeString("leftmargin", LeftMargin.ToString());
            if (TopMargin != _defaultMargin)
                p_writer.WriteAttributeString("topmargin", TopMargin.ToString());
            if (RightMargin != _defaultMargin)
                p_writer.WriteAttributeString("rightmargin", RightMargin.ToString());
            if (BottomMargin != _defaultMargin)
                p_writer.WriteAttributeString("bottommargin", BottomMargin.ToString());
            if (Landscape)
                p_writer.WriteAttributeString("landscape", "True");
            p_writer.WriteEndElement();
        }
    }
}
