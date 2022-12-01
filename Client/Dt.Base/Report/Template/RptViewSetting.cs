#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 默认报表预览窗口的设置
    /// </summary>
    internal class RptViewSetting
    {
        public RptViewSetting(RptRoot p_root)
        {
            Root = p_root;
            var row = new Row();
            row.AddCell<string>("script");
            row.AddCell("showsearchform", true);
            row.AddCell<bool>("autoquery");

            row.AddCell<bool>("showcolheader");
            row.AddCell<bool>("showrowheader");
            row.AddCell<bool>("showgridline");

            row.AddCell("showmenu", true);
            row.AddCell<bool>("showcontextmenu");
            row.AddCell<bool>("showselectionmenu");

            row.AddCell<bool>("showquery");
            row.AddCell("showexport", true);
            row.AddCell("showprint", true);
            row.AddCell<bool>("colheaderitem");
            row.AddCell<bool>("rowheaderitem");
            row.AddCell<bool>("gridlineitem");

            row.Changed += Root.OnCellValueChanged;
            Data = row;
        }

        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public RptRoot Root { get; }

        /// <summary>
        /// 预览设置数据源
        /// </summary>
        public Row Data { get; }

        /// <summary>
        /// 报表脚本的类型名称，继承RptScript且带有 [RptScript] 标签的类型
        /// </summary>
        public string Script
        {
            get { return Data.Str("script"); }
            set { Data["script"] = value; }
        }

        /// <summary>
        /// 获取设置是否显示报表查询面板，默认true
        /// </summary>
        public bool ShowSearchForm
        {
            get { return Data.Bool("showsearchform"); }
            set { Data["showsearchform"] = value; }
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
        /// 获取设置是否显示工具栏菜单，默认true
        /// </summary>
        public bool ShowMenu
        {
            get { return Data.Bool("showmenu"); }
            set { Data["showmenu"] = value; }
        }

        /// <summary>
        /// 获取设置是否显示上下文菜单，默认false
        /// </summary>
        public bool ShowContextMenu
        {
            get { return Data.Bool("showcontextmenu"); }
            set { Data["showcontextmenu"] = value; }
        }

        /// <summary>
        /// 是否显示显示选中区域的上下文菜单，默认false
        /// </summary>
        public bool ShowSelectionMenu
        {
            get { return Data.Bool("showselectionmenu"); }
            set { Data["showselectionmenu"] = value; }
        }

        /// <summary>
        /// 是否显示查询菜单项，默认false
        /// </summary>
        public bool ShowQuery
        {
            get { return Data.Bool("showquery"); }
            set { Data["showquery"] = value; }
        }

        /// <summary>
        /// 是否显示导出菜单项，默认true
        /// </summary>
        public bool ShowExport
        {
            get { return Data.Bool("showexport"); }
            set { Data["showexport"] = value; }
        }

        /// <summary>
        /// 是否显示打印菜单项，默认true
        /// </summary>
        public bool ShowPrint
        {
            get { return Data.Bool("showprint"); }
            set { Data["showprint"] = value; }
        }

        /// <summary>
        /// 是否显示列头菜单项，默认false
        /// </summary>
        public bool ShowColHeaderItem
        {
            get { return Data.Bool("colheaderitem"); }
            set { Data["contextcolheader"] = value; }
        }

        /// <summary>
        /// 是否显示行头菜单项，默认false
        /// </summary>
        public bool ShowRowHeaderItem
        {
            get { return Data.Bool("rowheaderitem"); }
            set { Data["rowheaderitem"] = value; }
        }

        /// <summary>
        /// 是否显示网格菜单项，默认false
        /// </summary>
        public bool ShowGridLineItem
        {
            get { return Data.Bool("gridlineitem"); }
            set { Data["gridlineitem"] = value; }
        }

        public void ReadXml(XmlReader p_reader)
        {
            Data.ReadXml(p_reader);
            p_reader.Read();
        }

        public void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("View");
            string val = Data.Str("script");
            if (!string.IsNullOrEmpty(val))
                p_writer.WriteAttributeString("script", val);
            if (!ShowSearchForm)
                p_writer.WriteAttributeString("showsearchform", "False");
            if (AutoQuery)
                p_writer.WriteAttributeString("autoquery", "True");
            if (ShowColHeader)
                p_writer.WriteAttributeString("showcolheader", "True");
            if (ShowRowHeader)
                p_writer.WriteAttributeString("showrowheader", "True");
            if (ShowGridLine)
                p_writer.WriteAttributeString("showgridline", "True");

            if (!ShowMenu)
                p_writer.WriteAttributeString("showmenu", "False");
            if (ShowContextMenu)
                p_writer.WriteAttributeString("showcontextmenu", "True");
            if (ShowSelectionMenu)
                p_writer.WriteAttributeString("showselectionmenu", "True");

            if (ShowQuery)
                p_writer.WriteAttributeString("showquery", "True");
            if (!ShowExport)
                p_writer.WriteAttributeString("showexport", "False");
            if (!ShowPrint)
                p_writer.WriteAttributeString("showprint", "False");
            if (ShowColHeaderItem)
                p_writer.WriteAttributeString("colheaderitem", "True");
            if (ShowRowHeaderItem)
                p_writer.WriteAttributeString("rowheaderitem", "True");
            if (ShowGridLineItem)
                p_writer.WriteAttributeString("gridlineitem", "True");

            p_writer.WriteEndElement();
        }
    }
}
