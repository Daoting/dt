#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Microsoft.UI.Xaml.Markup;
using System.Text;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表参数定义
    /// </summary>
    public class RptParams
    {
        const string _xamlPrefix = "<a:QueryFv xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:a=\"using:Dt.Base\">";
        const string _xamlPostfix = "</a:QueryFv>";

        public RptParams(RptRoot p_root)
        {
            Root = p_root;
            Data = new Table
            {
                { "name" },
                { "type" },
                { "val" },
                { "note" },
            };
            Data.Changed += Root.OnCellValueChanged;

            XamlRow = new Row { { "xaml", typeof(string) } };
        }

        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public RptRoot Root { get; }

        /// <summary>
        /// 代码中定义的查询框，用[RptQuery]标记的类型名称
        /// </summary>
        public string QueryForm { get; set; }

        /// <summary>
        /// 参数对应的查询框xaml
        /// </summary>
        public string Xaml
        {
            get { return XamlRow.Str("xaml"); }
            set { XamlRow["xaml"] = value; }
        }

        /// <summary>
        /// 是否根据参数自动生成查询面板
        /// </summary>
        public bool AutoXaml { get; set; }

        /// <summary>
        /// 获取数据源列表
        /// </summary>
        public Table Data { get; }

        /// <summary>
        /// xaml数据源
        /// </summary>
        public Row XamlRow { get; set; }

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
        /// 是否存在查询框UI
        /// </summary>
        public bool ExistQueryForm => !string.IsNullOrEmpty(Xaml) || !string.IsNullOrEmpty(QueryForm) || AutoXaml;

        /// <summary>
        /// 根据初始参数值生成查询参数字典
        /// </summary>
        /// <returns></returns>
        public async Task<Dict> BuildInitDict()
        {
            Dict dict = new Dict();
            foreach (var row in Data)
            {
                string name = row.Str("name");
                if (name == "")
                    continue;

                string val = await GetInitVal(row);
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

                    case "long":
                        if (val != "" && long.TryParse(val, out var lval))
                        {
                            dict[name] = lval;
                        }
                        else
                        {
                            dict[name] = default(long);
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
        /// 构造查询面板
        /// </summary>
        /// <param name="p_params">初始参数值</param>
        /// <returns></returns>
        public async Task<RptQuery> CreateQueryForm(Dict p_params)
        {
            RptQuery query = null;
            Row data = await BuildInitRow(p_params);
            
            if (!string.IsNullOrEmpty(QueryForm))
            {
                var tp = Kit.GetTypeByAlias(typeof(RptQueryAttribute), QueryForm);
                if (tp != null && tp.IsSubclassOf(typeof(RptQuery)))
                {
                    query = Activator.CreateInstance(tp) as RptQuery;
                    query.LoadData(data, null);
                }
                else
                {
                    Throw.Msg($"报表缺少外部自定义查询面板类型【{QueryForm}】");
                }
            }
            else if (!string.IsNullOrEmpty(Xaml))
            {
                var fv = CreateFvByXaml();
                query = new RptQuery();
                query.LoadData(data, fv);
            }
            else if (AutoXaml)
            {
                string xaml = CreateXamlByDefine();
                var fv = XamlReader.Load(xaml) as QueryFv;
                query = new RptQuery();
                query.LoadData(data, fv);
            }
            else
            {
                Throw.Msg("报表未定义查询面板！");
            }
            return query;
        }

        /// <summary>
        /// 判断参数是否有效
        /// </summary>
        /// <returns></returns>
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
        /// 根据参数定义自动生成xaml
        /// </summary>
        /// <returns></returns>
        public string CreateXamlByDefine()
        {
            StringBuilder sb = new StringBuilder(_xamlPrefix);
            foreach (var row in Data)
            {
                string name = row.Str("name");
                if (name == "")
                    continue;

                switch (row.Str("type").ToLower())
                {
                    case "bool":
                        sb.Append($"<a:CBool ID=\"{name}\" ShowTitle=\"False\" />");
                        sb.AppendLine();
                        break;

                    case "double":
                        sb.Append($"<a:CNum ID=\"{name}\" />");
                        sb.AppendLine();
                        break;

                    case "int":
                        sb.Append($"<a:CNum ID=\"{name}\" IsInteger=\"True\" />");
                        sb.AppendLine();
                        break;

                    case "date":
                        sb.Append($"<a:CDate ID=\"{name}\" />");
                        sb.AppendLine();
                        break;

                    default:
                        sb.Append($"<a:CText ID=\"{name}\" />");
                        sb.AppendLine();
                        break;
                }
            }
            sb.Append(_xamlPostfix);
            return sb.ToString();
        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public void ReadXml(XmlReader p_reader)
        {
            if (p_reader.AttributeCount > 0)
            {
                for (int i = 0; i < p_reader.AttributeCount; i++)
                {
                    p_reader.MoveToAttribute(i);
                    if (p_reader.Name == "queryform")
                        QueryForm = p_reader.Value;
                    else if (p_reader.Name == "autoxaml")
                        AutoXaml = true;
                }
            }

            p_reader.Read();
            if (p_reader.Name == "Xaml")
            {
                p_reader.Read();
                if (p_reader.NodeType == XmlNodeType.CDATA)
                {
                    Xaml = p_reader.Value;
                    p_reader.Read();
                    p_reader.Read();
                }
            }

            if (p_reader.Name == "List")
            {
                Data.ReadXml(p_reader, null);
                // List结束
                p_reader.Read();
            }

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

            if (!string.IsNullOrEmpty(QueryForm))
                p_writer.WriteAttributeString("queryform", QueryForm);
            if (AutoXaml)
                p_writer.WriteAttributeString("autoxaml", "true");

            if (!string.IsNullOrEmpty(Xaml))
            {
                p_writer.WriteStartElement("Xaml");
                p_writer.WriteCData(Xaml);
                p_writer.WriteEndElement();
            }

            p_writer.WriteStartElement("List");
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

                p_writer.WriteEndElement();
            }
            p_writer.WriteEndElement();

            p_writer.WriteEndElement();
        }

        /// <summary>
        /// 根据xaml创建查询面板
        /// </summary>
        /// <returns></returns>
        QueryFv CreateFvByXaml()
        {
            if (string.IsNullOrEmpty(Xaml))
            {
                Throw.Msg("报表参数的xaml内容为空，无法创建查询面板！");
            }

            try
            {
                return XamlReader.Load(Xaml) as QueryFv;
            }
            catch (Exception ex)
            {
                Throw.Msg($"报表参数的xaml内容错误：{ex.Message}\n{Xaml}");
            }
            return null;
        }

        /// <summary>
        /// 根据初始参数值生成Row，常用来提供给查询面板
        /// </summary>
        /// <param name="p_params">初始参数值</param>
        /// <returns></returns>
        async Task<Row> BuildInitRow(Dict p_params)
        {
            var data = new Row();
            foreach (var row in Data)
            {
                string name = row.Str("name");
                if (name == "")
                    continue;

                string val;
                
                // 初始值：外部传入的值或缺省值
                if (p_params != null && p_params.TryGetValue(name, out var initVal))
                    val = initVal == null ? "" : initVal.ToString();
                else
                    val = await GetInitVal(row);
                
                switch (row.Str("type").ToLower())
                {
                    case "bool":
                        if (val == "")
                        {
                            data.Add<bool>(name);
                        }
                        else
                        {
                            string l = val.ToLower();
                            data.Add(name, (l == "1" || l == "true"));
                        }
                        break;

                    case "double":
                        if (val != "" && double.TryParse(val, out var v))
                        {
                            data.Add(name, v);
                        }
                        else
                        {
                            data.Add<double>(name);
                        }
                        break;

                    case "int":
                        if (val != "" && int.TryParse(val, out var i))
                        {
                            data.Add(name, i);
                        }
                        else
                        {
                            data.Add<int>(name);
                        }
                        break;

                    case "long":
                        if (val != "" && long.TryParse(val, out var lval))
                        {
                            data.Add(name, lval);
                        }
                        else
                        {
                            data.Add<long>(name);
                        }
                        break;

                    case "date":
                        if (val != "" && DateTime.TryParse(val, out var d))
                        {
                            data.Add(name, d);
                        }
                        else
                        {
                            data.Add<DateTime>(name);
                        }
                        break;

                    default:
                        data.Add(name, val);
                        break;
                }
            }
            return data;
        }

        async Task<string> GetInitVal(Row p_row)
        {
            string val = p_row.Str("val");
            if (val != "")
            {
                if (val[0] == ':')
                {
                    // 内置表达式
                    val = ValueExpression.GetValue(val.Substring(1));
                }
                else if (val[0] == '@')
                {
                    var result = await ValueCall.GetValue(val.Substring(1));
                    val = result == null ? "" : result.ToString();
                }
            }
            return val;
        }
    }
}
