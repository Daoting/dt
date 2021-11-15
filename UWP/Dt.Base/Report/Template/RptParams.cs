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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表参数定义
    /// </summary>
    internal class RptParams
    {
        const string _xamlPrefix = "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:a=\"using:Dt.Base\" ";

        public RptParams(RptRoot p_root)
        {
            Root = p_root;
            Data = new Table
            {
                { "name" },
                { "type" },
                { "val" },
                { "note" },
                { "xaml" },
            };
            Data.Changed += Root.OnCellValueChanged;
        }

        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public RptRoot Root { get; }

        /// <summary>
        /// 获取数据源列表
        /// </summary>
        public Table Data { get; }

        /// <summary>
        /// 根据参数名获取参数定义Row
        /// </summary>
        /// <param name="p_col"></param>
        /// <returns></returns>
        public Row this[string p_col]
        {
            get
            {
                return (from row in Data
                        where row.Str("name") == p_col
                        select row).FirstOrDefault();
            }
        }

        /// <summary>
        /// 是否存在含xaml的参数
        /// </summary>
        public bool ExistXaml
        {
            get
            {
                return (from row in Data
                        where row.Str("xaml") != ""
                        select row).Any();
            }
        }

        /// <summary>
        /// 根据初始参数值生成Row，常用来提供给查询面板
        /// </summary>
        /// <returns></returns>
        public Row BuildInitRow()
        {
            var data = new Row();
            foreach (var row in Data)
            {
                string name = row.Str("name");
                if (name == "")
                    continue;

                string val = row.Str("val");
                if (val != "" && val[0] == ':')
                    val = ValueExpression.GetValue(val.Substring(1));
                switch (row.Str("type").ToLower())
                {
                    case "bool":
                        if (val == "")
                        {
                            data.AddCell<bool>(name);
                        }
                        else
                        {
                            string l = val.ToLower();
                            data.AddCell(name, (l == "1" || l == "true"));
                        }
                        break;

                    case "double":
                        if (val != "" && double.TryParse(val, out var v))
                        {
                            data.AddCell(name, v);
                        }
                        else
                        {
                            data.AddCell<double>(name);
                        }
                        break;

                    case "int":
                        if (val != "" && int.TryParse(val, out var i))
                        {
                            data.AddCell(name, i);
                        }
                        else
                        {
                            data.AddCell<int>(name);
                        }
                        break;

                    case "date":
                        if (val != "" && DateTime.TryParse(val, out var d))
                        {
                            data.AddCell(name, d);
                        }
                        else
                        {
                            data.AddCell<DateTime>(name);
                        }
                        break;

                    default:
                        data.AddCell(name, val);
                        break;
                }
            }
            return data;
        }

        /// <summary>
        /// 根据初始参数值生成查询参数字典
        /// </summary>
        /// <returns></returns>
        public Dict BuildInitDict()
        {
            Dict dict = new Dict();
            foreach (var row in Data)
            {
                string name = row.Str("name");
                if (name == "")
                    continue;

                string val = row.Str("val");
                if (val != "" && val[0] == ':')
                    val = ValueExpression.GetValue(val.Substring(1));
                switch (row.Str("type").ToLower())
                {
                    case "bool":
                        if (val == "")
                        {
                            dict[name] = false;
                        }
                        else
                        {
                            string l = val.ToLower();
                            dict[name] = (l == "1" || l == "true");
                        }
                        break;

                    case "double":
                        if (val != "" && double.TryParse(val, out var v))
                        {
                            dict[name] = v;
                        }
                        else
                        {
                            dict[name] = default(double);
                        }
                        break;

                    case "int":
                        if (val != "" && int.TryParse(val, out var i))
                        {
                            dict[name] = i;
                        }
                        else
                        {
                            dict[name] = default(int);
                        }
                        break;

                    case "date":
                        if (val != "" && DateTime.TryParse(val, out var d))
                        {
                            dict[name] = d;
                        }
                        else
                        {
                            dict[name] = default(DateTime);
                        }
                        break;

                    default:
                        dict[name] = val;
                        break;
                }
            }
            return dict;
        }

        /// <summary>
        /// 构造查询面板的单元格
        /// </summary>
        /// <param name="p_fv"></param>
        public void LoadFvCells(Fv p_fv)
        {
            foreach (var row in Data)
            {
                string name = row.Str("name");
                if (name == "")
                    continue;

                // 由xaml生成格
                string xaml = row.Str("xaml").Trim();
                if (xaml != "")
                {
                    try
                    {
                        int index = xaml.IndexOf(' ') + 1;
                        var cell = XamlReader.Load(xaml.Insert(index, _xamlPrefix)) as FvCell;
                        if (cell != null)
                        {
                            cell.ID = name;
                            p_fv.Items.Add(cell);
                        }
                    }
                    catch (Exception ex)
                    {
                        Kit.Warn($"报表参数【{name}】的xaml内容错误：{ex.Message}");
                    }
                }
            }
            p_fv.Data = BuildInitRow();
        }

        public bool IsValid()
        {
            bool fail = (from row in Data
                         where row.Str("name") == ""
                         select row).Any();
            if (fail)
            {
                Kit.Warn("参数标识不可为空！");
                return false;
            }

            fail = Data.GroupBy(r => r.Str("name")).Where(g => g.Count() > 1).Any();
            if (fail)
            {
                Kit.Warn("参数标识不可重复！");
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
            Data.ReadXml(p_reader, "xaml");

            // 默认类型
            var ls = from row in Data
                     where row.Str("type") == ""
                     select row;
            foreach (var row in ls)
            {
                row.Cells["type"].InitVal("string");
            }
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        public void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Params");
            foreach (Row row in Data)
            {
                p_writer.WriteStartElement("Param");

                p_writer.WriteAttributeString("name", row.Str("name"));

                string val = row.Str("type");
                if (val != "" && val != "string")
                    p_writer.WriteAttributeString("type", val);

                val = row.Str("val");
                if (val != "")
                    p_writer.WriteAttributeString("val", val);

                val = row.Str("note");
                if (val != "")
                    p_writer.WriteAttributeString("note", val);

                val = row.Str("xaml");
                if (val != "")
                    p_writer.WriteCData(val);
                p_writer.WriteEndElement();
            }
            p_writer.WriteEndElement();
        }
    }
}
