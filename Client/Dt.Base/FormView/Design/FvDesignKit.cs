#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using System.Reflection;
using System.Text;
using System.Xml;
#endregion

namespace Dt.Base
{
    public static class FvDesignKit
    {
        /// <summary>
        /// 加载单元格属性面板
        /// </summary>
        /// <param name="p_tgtCell">目标单元格</param>
        /// <param name="p_fv">属性面板</param>
        public static void LoadCellProps(object p_tgtCell, Fv p_fv)
        {
            if (p_tgtCell == null || p_fv == null)
                return;

            if (p_fv.Data == p_tgtCell)
                return;

            // 同类型直接切换数据源
            if (p_fv.Data != null && p_fv.Data.GetType() == p_tgtCell.GetType())
            {
                p_fv.Data = p_tgtCell;
                return;
            }

            var items = p_fv.Items;
            using (items.Defer())
            {
                items.Clear();

                if (p_tgtCell is FvCell fc)
                {
                    FvCell cell = new CTip();
                    cell.ID = "ID";
                    items.Add(cell);

                    cell = new CText();
                    cell.ID = "Title";
                    cell.Title = "标题";
                    items.Add(cell);

                    int sumFront = 2;
                    int sumMiddle = 0;
                    int sumBar = 0;

                    // 按标题宽度排序显示，最后放选择框
                    foreach (var info in GetCellProps(fc.GetType()))
                    {
                        cell = fc.CreateDesignCell(info);
                        if (info.Info.PropertyType == typeof(bool))
                        {
                            items.Add(cell);
                        }
                        else if (!cell.ShowTitle)
                        {
                            items.Insert(sumFront + sumMiddle + sumBar, new CBar { Title = info.Title });
                            sumBar++;
                            items.Insert(sumFront + sumMiddle + sumBar, cell);
                            sumBar++;
                        }
                        else if (cell.TitleWidth > 120)
                        {
                            items.Insert(sumFront + sumMiddle, cell);
                            sumMiddle++;
                        }
                        else
                        {
                            items.Insert(sumFront, cell);
                            sumFront++;
                        }
                    }

                    if (fc.Owner != null && fc.Owner.IsDesignMode)
                        fc.AddCustomDesignCells(items);
                }
                else if (p_tgtCell is CBar bar)
                {
                    FvCell cell = new CText();
                    cell.ID = "Title";
                    cell.Title = "标题";
                    items.Add(cell);

                    cell = new CNum { IsInteger = true };
                    cell.ID = "RowSpan";
                    cell.Title = "占用行数";
                    items.Add(cell);

                    cell = new CNum();
                    cell.ID = "ColSpan";
                    cell.Title = "列宽占比0~1";
                    items.Add(cell);

                    if (bar.Owner != null && bar.Owner.IsDesignMode)
                        bar.AddCustomDesignCells(items);
                }
            }
            p_fv.Data = p_tgtCell;
        }

        public static IEnumerable<CellPropertyInfo> GetCellProps(Type p_type)
        {
            if (p_type == null)
                return null;

            if (!_cellProps.TryGetValue(p_type, out List<CellPropertyInfo> props))
            {
                props = new List<CellPropertyInfo>();
                var obj = Activator.CreateInstance(p_type);
                foreach (var info in p_type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    var attr = (CellParamAttribute)info.GetCustomAttribute(typeof(CellParamAttribute), false);
                    if (attr != null)
                        props.Add(new CellPropertyInfo(info, info.GetValue(obj), attr.Title));
                }
                _cellProps[p_type] = props;
            }
            return props.Concat(_baseProps);
        }

        public static void CopyXml(XmlWriter p_xw, string p_xml)
        {
            int index;
            if (string.IsNullOrEmpty(p_xml) || (index = p_xml.IndexOf('>')) < 0)
                return;

            // 未包含命名空间，补充，否则节点含a: x:前缀时无法解析
            string xml = p_xml;
            if (xml.IndexOf(" xmlns:a=", 0, index) == -1)
            {
                if (xml[index - 1] == '/')
                    index--;
                xml = xml.Insert(index, " xmlns:x=\"xaml\" xmlns:a=\"dt\"");
            }

            using (var stringReader = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(stringReader, new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true, IgnoreProcessingInstructions = true }))
            {
                while (reader.Read()) // 逐节点读取
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            // 处理元素及属性
                            p_xw.WriteStartElement(reader.Prefix, reader.LocalName, null);

                            // 复制所有属性
                            if (reader.HasAttributes)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    // 忽略xmlns前缀的属性
                                    if (reader.Prefix != "xmlns")
                                        p_xw.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                                }
                                reader.MoveToElement(); // 移回元素
                            }

                            // 如果是空元素则立即闭合
                            if (reader.IsEmptyElement)
                            {
                                p_xw.WriteEndElement();
                            }
                            break;

                        case XmlNodeType.Text:
                            p_xw.WriteString(reader.Value);
                            break;

                        case XmlNodeType.CDATA:
                            p_xw.WriteCData(reader.Value);
                            break;

                        case XmlNodeType.EndElement:
                            p_xw.WriteEndElement();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 未包含命名空间，补充，否则节点含a: x:前缀时无法解析
        /// </summary>
        /// <param name="p_xml"></param>
        /// <returns></returns>
        public static string AddXmlns(string p_xml)
        {
            int index;
            if (string.IsNullOrEmpty(p_xml) || (index = p_xml.IndexOf('>')) < 0)
                return p_xml;

            if (p_xml.IndexOf(" xmlns:a=", 0, index) == -1)
            {
                if (p_xml[index - 1] == '/')
                    index--;
                return p_xml.Insert(index, " xmlns:x=\"xaml\" xmlns:a=\"dt\"");
            }
            return p_xml;
        }

        /// <summary>
        /// 获取节点的xml
        /// </summary>
        /// <param name="p_node"></param>
        /// <returns></returns>
        public static string GetNodeXml(XmlNode p_node)
        {
            var sb = new StringBuilder();
            using (XmlWriter xw = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true }))
            {
                p_node.WriteTo(xw);
                xw.Flush();
            }
            return sb.Replace(" xmlns:a=\"dt\"", "").Replace(" xmlns:x=\"xaml\"", "").ToString();
        }

        /// <summary>
        /// 添加xaml到CText
        /// </summary>
        /// <param name="p_ct"></param>
        /// <param name="p_txt"></param>
        public static void AddXamlToCText(CText p_ct, string p_txt)
        {
            var tb = p_ct.TextBox;
            var prefix = tb.Text.Substring(0, tb.SelectionStart).TrimEnd('\r');
            if (prefix != "")
                prefix += "\r";

            string postfix = "";
            if (tb.Text.Length > tb.SelectionStart)
                postfix = "\r" + tb.Text.Substring(tb.SelectionStart).Trim('\r');
            p_ct.Val = prefix + p_txt + postfix;
        }
        
        static FvDesignKit()
        {
            var normal = new FvCell();
            foreach (var info in typeof(FvCell).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var attr = (CellParamAttribute)info.GetCustomAttribute(typeof(CellParamAttribute), false);
                if (attr != null)
                    _baseProps.Add(new CellPropertyInfo(info, info.GetValue(normal), attr.Title));
            }
        }

        static readonly Dictionary<Type, List<CellPropertyInfo>> _cellProps = new Dictionary<Type, List<CellPropertyInfo>>();
        static readonly List<CellPropertyInfo> _baseProps = new List<CellPropertyInfo>();
    }

    public class CellPropertyInfo
    {
        public CellPropertyInfo(PropertyInfo p_info, object p_defVal, string p_title)
        {
            Info = p_info;
            DefaultValue = p_defVal;
            Title = p_title;
        }

        public PropertyInfo Info { get; }

        public object DefaultValue { get; }

        public string Title { get; }
    }
}