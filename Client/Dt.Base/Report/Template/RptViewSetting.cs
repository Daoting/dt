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
    /// 默认报表预览窗口的设置
    /// </summary>
    internal class RptViewSetting
    {
        public RptViewSetting()
        {
            Data = new Row();
            Data.AddCell<string>("script");
            Data.AddCell<bool>("hidesearchform");
            Data.AddCell<bool>("autoquery");
            Data.AddCell<bool>("showcolheader");
            Data.AddCell<bool>("showrowheader");
            Data.AddCell<bool>("showgridline");
            Data.AddCell<bool>("showsearchmi");
            Data.AddCell("showexportmi", true);
            Data.AddCell("showprintmi", true);
        }

        /// <summary>
        /// 预览设置数据源
        /// </summary>
        public Row Data { get; }

        /// <summary>
        /// 报表脚本类型，形如 Dt.Base.MyScript,Dt.Base
        /// </summary>
        public string Script
        {
            get { return Data.Str("script"); }
            set { Data["script"] = value; }
        }

        /// <summary>
        /// 获取设置是否隐藏报表查询面板（报表组时无效），默认false
        /// </summary>
        public bool HideSearchForm
        {
            get { return Data.Bool("hidesearchform"); }
            set { Data["hidesearchform"] = value; }
        }

        /// <summary>
        /// 获取设置初次加载时是否自动执行查询，前提是Params参数值提供完备，默认false
        /// </summary>
        public bool AutoQuery
        {
            get { return Data.Bool("autoquery"); }
            set { Data["autoquery"] = value; }
        }

        /// <summary>
        /// 获取设置Worksheet是否显示列头，默认false
        /// </summary>
        public bool ShowColHeader
        {
            get { return Data.Bool("showcolheader"); }
            set { Data["showcolheader"] = value; }
        }

        /// <summary>
        /// 获取设置Worksheet是否显示行头，默认false
        /// </summary>
        public bool ShowRowHeader
        {
            get { return Data.Bool("showrowheader"); }
            set { Data["showrowheader"] = value; }
        }

        /// <summary>
        /// 获取设置Worksheet是否显示网格，默认false
        /// </summary>
        public bool ShowGridLine
        {
            get { return Data.Bool("showgridline"); }
            set { Data["showgridline"] = value; }
        }

        /// <summary>
        /// 获取设置是否显示查询菜单项，默认false
        /// </summary>
        public bool ShowSearchMi
        {
            get { return Data.Bool("showsearchmi"); }
            set { Data["showsearchmi"] = value; }
        }

        /// <summary>
        /// 获取设置是否显示导出菜单项，默认true
        /// </summary>
        public bool ShowExportMi
        {
            get { return Data.Bool("showexportmi"); }
            set { Data["showexportmi"] = value; }
        }

        /// <summary>
        /// 获取设置是否显示打印菜单项，默认true
        /// </summary>
        public bool ShowPrintMi
        {
            get { return Data.Bool("showprintmi"); }
            set { Data["showprintmi"] = value; }
        }

        public void ReadXml(XmlReader p_reader)
        {
            Data.ReadXml(p_reader);
            p_reader.Read();
        }

        public void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Setting");
            string val = Data.Str("script");
            if (!string.IsNullOrEmpty(val))
                p_writer.WriteAttributeString("script", val);
            if (HideSearchForm)
                p_writer.WriteAttributeString("hidesearchform", "True");
            if (AutoQuery)
                p_writer.WriteAttributeString("autoquery", "True");
            if (ShowColHeader)
                p_writer.WriteAttributeString("showcolheader", "True");
            if (ShowRowHeader)
                p_writer.WriteAttributeString("showrowheader", "True");
            if (ShowGridLine)
                p_writer.WriteAttributeString("showgridline", "True");
            if (ShowSearchMi)
                p_writer.WriteAttributeString("showsearchmi", "True");
            if (!ShowExportMi)
                p_writer.WriteAttributeString("showexportmi", "False");
            if (!ShowPrintMi)
                p_writer.WriteAttributeString("showprintmi", "False");
            p_writer.WriteEndElement();
        }
    }
}
