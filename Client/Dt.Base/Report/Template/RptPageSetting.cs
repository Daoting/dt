#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System.ComponentModel;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 页面设置，所有尺寸单位：0.01英寸
    /// </summary>
    public class RptPageSetting
    {
        const double _dpi = 0.96;
        
        // 默认A4纸张，单位：0.01英寸
        const double _defaultHeight = 1169;
        const double _defaultWidth = 827;
        
        // 默认边距为word打印时的"适中"边距
        const int _defaultLeft = 70;
        const int _defaultRight = 70;
        const int _defaultTop = 100;
        const int _defaultBottom = 100;

        public RptPageSetting(RptRoot p_root)
        {
            Root = p_root;
            Data = new Row();
            // 纸张名称不保存
            Data.Add<string>("papername");
            Data.Add<bool>("autopapersize").PropertyChanged += OnAutoPaperSizeChanged;
            Data.Add("height", _defaultHeight);
            Data.Add("width", _defaultWidth);
            Data.Add("leftmargin", _defaultLeft);
            Data.Add("topmargin", _defaultTop);
            Data.Add("rightmargin", _defaultRight);
            Data.Add("bottommargin", _defaultBottom);
            Data.Add<bool>("landscape");
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
        /// 是否自动调整纸张大小，确保只一页，默认false
        /// </summary>
        public bool AutoPaperSize
        {
            set { Data["autopapersize"] = value; }
            get { return Data.Bool("autopapersize"); }
        }
        
        /// <summary>
        /// 获取纸张名称
        /// </summary>
        public string PaperName
        {
            set { Data["PaperName"] = value; }
            get { return Data.Str("PaperName"); }
        }

        /// <summary>
        /// 获取高度，单位：0.01英寸
        /// </summary>
        public double Height
        {
            set { Data["Height"] = value; }
            get { return AutoPaperSize ? double.MaxValue : Data.Double("Height"); }
        }

        /// <summary>
        /// 获取宽度，单位：0.01英寸
        /// </summary>
        public double Width
        {
            set { Data["Width"] = value; }
            get { return AutoPaperSize ? double.MaxValue : Data.Double("Width"); }
        }

        /// <summary>
        /// 获取左边距，单位：0.01英寸
        /// </summary>
        public int LeftMargin
        {
            set { Data["LeftMargin"] = value; }
            get { return Data.Int("LeftMargin"); }
        }

        /// <summary>
        /// 获取上边距，单位：0.01英寸
        /// </summary>
        public int TopMargin
        {
            set { Data["TopMargin"] = value; }
            get { return Data.Int("TopMargin"); }
        }

        /// <summary>
        /// 获取右边距，单位：0.01英寸
        /// </summary>
        public int RightMargin
        {
            set { Data["RightMargin"] = value; }
            get { return Data.Int("RightMargin"); }
        }

        /// <summary>
        /// 获取下边距，单位：0.01英寸
        /// </summary>
        public int BottomMargin
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
        /// 页面大小，单位：0.01英寸
        /// </summary>
        public Dt.Cells.Data.PaperSize PaperSize => new(Width, Height);

        /// <summary>
        /// 页边距，单位：0.01英寸
        /// </summary>
        public Dt.Cells.Data.Margins PageMargins => new(TopMargin, BottomMargin, LeftMargin, RightMargin, 0, 0);

        /// <summary>
        /// 获取有效高度，像素
        /// </summary>
        public double ValidHeight
        {
            get
            {
                // 不可提取统一 Math.Round，和Excel内部算法一致，否则出现误差！！！
                return Landscape ?
                    Math.Round(Width * _dpi) - Math.Round(TopMargin * _dpi) - Math.Round(BottomMargin * _dpi)
                    : Math.Round(Height * _dpi) - Math.Round(TopMargin * _dpi) - Math.Round(BottomMargin * _dpi);
            }
        }

        /// <summary>
        /// 获取有效宽度，像素
        /// </summary>
        public double ValidWidth
        {
            get
            {
                // 不可提取统一 Math.Round，和Excel内部算法一致，否则出现误差！！！
                return Landscape ?
                    Math.Round(Height * _dpi) - Math.Round(LeftMargin * _dpi) - Math.Round(RightMargin * _dpi)
                    : Math.Round(Width * _dpi) - Math.Round(LeftMargin * _dpi) - Math.Round(RightMargin * _dpi);
            }
        }

        public bool IsValid()
        {
            if (AutoPaperSize)
                return true;
            
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
            if (AutoPaperSize)
                p_writer.WriteAttributeString("autopapersize", "True");
            if (Height != _defaultHeight && !AutoPaperSize)
                p_writer.WriteAttributeString("height", Height.ToString());
            if (Width != _defaultWidth && !AutoPaperSize)
                p_writer.WriteAttributeString("width", Width.ToString());
            if (LeftMargin != _defaultLeft)
                p_writer.WriteAttributeString("leftmargin", LeftMargin.ToString());
            if (TopMargin != _defaultTop)
                p_writer.WriteAttributeString("topmargin", TopMargin.ToString());
            if (RightMargin != _defaultRight)
                p_writer.WriteAttributeString("rightmargin", RightMargin.ToString());
            if (BottomMargin != _defaultBottom)
                p_writer.WriteAttributeString("bottommargin", BottomMargin.ToString());
            if (Landscape)
                p_writer.WriteAttributeString("landscape", "True");
            p_writer.WriteEndElement();
        }

        void OnAutoPaperSizeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChanged")
            {
                if (AutoPaperSize)
                {
                    Width = double.MaxValue;
                    Height = double.MaxValue;
                }
                else
                {
                    Width = _defaultWidth;
                    Height = _defaultHeight;
                }
            }
        }
    }
}
