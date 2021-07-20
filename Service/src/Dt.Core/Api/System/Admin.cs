#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 内部提供给 admin.html 页面的Api
    /// </summary>
    public class Admin
    {
        #region Admin页面
        /// <summary>
        /// Admin页面初始化时显示的内容
        /// </summary>
        /// <returns></returns>
        public List<string> GetInitInfo()
        {
            var ls = new List<string>();
            ls.Add($"{Kit.SvcName.Substring(0, 1).ToUpper()}{Kit.SvcName.Substring(1)} API目录");
            ls.Add(GetTopbarHtml());
            ls.Add(GetGroupApi("API"));
            return ls;
        }

        /// <summary>
        /// 获取分组Api的html
        /// </summary>
        /// <param name="p_group">Api分组名</param>
        /// <returns></returns>
        public string GetGroupApi(string p_group)
        {
            if (Silo.GroupMethods.TryGetValue(p_group, out List<string> ls))
                return GetApiHtml(ls);
            return "";
        }

        /// <summary>
        /// 获取指定类的Api
        /// </summary>
        /// <param name="p_clsName"></param>
        /// <returns></returns>
        public string GetClsApi(string p_clsName)
        {
            if (string.IsNullOrEmpty(p_clsName))
                return null;

            string prefix = p_clsName + ".";
            var ls = from name in Silo.Methods.Keys
                     where name.StartsWith(prefix)
                     select name;
            return GetApiHtml(ls);
        }

        /// <summary>
        /// 生成Api列表的html
        /// </summary>
        /// <param name="p_methods"></param>
        /// <returns></returns>
        string GetApiHtml(IEnumerable<string> p_methods)
        {
            int index = 0;
            int num = 0;
            string clsName = null;

            StringBuilder sb = new StringBuilder("<table>");
            foreach (var method in p_methods)
            {
                string[] info = method.Split('.');
                if (info.Length != 2)
                    continue;

                if (clsName != info[0])
                {
                    clsName = info[0];
                    sb.Append("<tr class=\"trClass\"><td colspan=\"4\">");
                    sb.AppendFormat("<a onclick=\"loadAgent('{0}')\" href=\"javascript:void(0);\" style=\"color:black;font-size:20px;text-decoration:underline;\">{0}</a>", clsName);
                    sb.Append("</td></tr>");
                    index = 0;
                }

                num = index++ % 5;
                if (num == 0)
                    sb.Append("<tr class=\"trMethod\">");
                sb.Append("<td class=\"tdMethod\">");
                if (index < 10)
                    sb.Append("&nbsp;&nbsp;");
                else if (index < 100)
                    sb.Append("&nbsp;");
                sb.Append(index);
                // 流模式Api无法调试
                if (Silo.GetMethod(method).CallMode == ApiCallMode.Unary)
                    sb.AppendFormat(".&nbsp;<a onclick=\"load('[&quot;Admin.CreateMethodCall&quot;,&quot;{0}&quot;]',true)\" href=\"javascript:void(0);\">{1}</a></td>", method, info[1]);
                else
                    sb.AppendFormat(".&nbsp;{0}</td>", info[1]);

                if (num == 4)
                    sb.Append("</tr>");
            }
            if (num != 4)
                sb.Append("</tr>");
            sb.Append("</table>");
            return sb.ToString();
        }

        /// <summary>
        /// 生成方法测试UI
        /// </summary>
        /// <param name="p_alias"></param>
        public string CreateMethodCall(string p_alias)
        {
            ApiMethod sm = Silo.GetMethod(p_alias);
            if (sm == null)
                return string.Format("<span style=\"margin: 20px;color: red;height: 40;\">不存在别名为“{0}”的方法，该方法可能已更名或被移除。</span>", p_alias);

            MethodInfo method = sm.Method;
            StringBuilder sb = new StringBuilder("<table>");
            sb.Append("<tr style=\"height: 400px;\">");

            #region 左侧
            sb.Append("<td style=\"width:300px;vertical-align:top;\">");
            sb.Append("<table>");
            sb.AppendFormat("<tr><td style=\"padding-top: 10px;\"><a onclick=\"load('[&quot;Admin.GetClsApi&quot;,&quot;{0}&quot;]',true)\" href=\"javascript:void(0);\" style=\"font-size:20px\">{0}目录</a></td></tr>", p_alias.Substring(0, p_alias.IndexOf(".")));

            sb.Append("<tr><td style=\"padding-top: 10px;font-weight: bold;\">映射方法</td></tr>");
            sb.Append("<tr><td>");
            sb.Append("<table>");
            sb.Append("<tr class=\"trLeft\"><td style=\"width: 75px;\"> 方法名称: </td><td>");
            sb.Append(method.Name);
            sb.Append("</td><tr>");
            sb.Append("<tr class=\"trLeft\"><td> 类型名称: </td><td>");
            sb.Append(method.DeclaringType.FullName);
            sb.Append("</td><tr>");
            sb.Append("<tr class=\"trLeft\"><td style=\"width: 75px;\"> 程序集: </td><td>");
            sb.Append(method.DeclaringType.Assembly.GetName().Name);
            sb.Append("</td><tr>");
            sb.Append("</table>");
            sb.Append("</td></tr>");

            sb.Append("<tr><td style=\"padding-top: 15px;font-weight: bold;\">参数列表</td></tr>");
            ParameterInfo[] pis = method.GetParameters();
            if (pis.Length == 0)
            {
                sb.Append("<tr><td style=\"padding-top: 4px;\">该方法无需参数。</td></tr>");
            }
            else
            {
                sb.Append("<tr><td style=\"padding-top: 4px;\"><table style=\"border: 1px solid #C3C3C3;\"><tr style=\"background-color: #DCDCDC;\"><td class=\"tdParam\">名称</td><td class=\"tdParam\">类型</td></tr>");
                foreach (ParameterInfo param in method.GetParameters())
                {
                    sb.AppendFormat("<tr><td class=\"tdParam\">{0}</td><td class=\"tdParam\">{1}</td></tr>", param.Name, param.ParameterType.Name);
                }
                sb.Append("</table></td></tr>");
            }

            //sb.Append("<tr><td style=\"padding-top: 15px;\"><input id=\"txtSPName\" style=\"height: 23px;width: 100%;\" /></td></tr>");
            //sb.Append("<tr><td style=\"padding-top: 6px;\"><input type=\"button\" onclick=\"onGetSPParams(txtSPName.value, true)\" value=\"SP参数查询\" style=\"height: 23px;\" /><input type=\"button\" onclick=\"onGetSPParams(txtSPName.value, false)\" value=\"模型SP参数查询\" style=\"height: 23px;margin-left: 20px\" /></td></tr>");

            sb.Append("</table>");
            sb.Append("</td>");
            #endregion

            #region 右侧
            sb.Append("<td style=\"vertical-align:top;padding-left:20px\">");
            sb.Append("<table>");
            sb.Append("<tr><td><textarea id=\"methodCall\" style=\"width:100%; height:360px;overflow: auto;\">");
            sb.Append(Silo.GetMethodCall(p_alias));
            sb.Append("</textarea></td></tr>");
            sb.Append("<tr><td><input type=\"button\" onclick=\"onTest()\" value=\"测试方法\" style=\"width: 90px;\" /><input type=\"button\" onclick=\"onCopy()\" value=\"复制结果\" style=\"width: 90px;margin-left:20px\" /></td></tr>");
            sb.Append("</table>");
            sb.Append("</td>");
            #endregion

            sb.Append("</tr>");

            #region 下侧
            sb.Append("<tr><td id=\"tdTitle\" colspan=\"2\" style=\"padding-top: 20px;font-weight: bold;\"/></tr>");
            sb.Append("<tr><td id=\"tdResult\" colspan=\"2\" style=\"word-wrap: break-word; padding: 15px;\"/></tr>");
            #endregion

            sb.Append("</table>");
            return sb.ToString();
        }

        /// <summary>
        /// 导航栏Api的html
        /// </summary>
        /// <returns></returns>
        string GetTopbarHtml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<span style=\"font-size:20px;\">{Kit.SvcName.Substring(0, 1).ToUpper()}{Kit.SvcName.Substring(1)}服务({Silo.Methods.Count})</span>");
            foreach (var item in Silo.GroupMethods.OrderBy((p) => p.Key, StringComparer.OrdinalIgnoreCase))
            {
                if (item.Value.Count > 0)
                    sb.AppendFormat("<a onclick=\"load('[&quot;Admin.GetGroupApi&quot;,&quot;{0}&quot;]',true)\" href=\"javascript:void(0);\" class=\"aTitle\">{0}</a>", item.Key);
            }
            return sb.ToString();
        }
        #endregion

        #region 生成代码
        /// <summary>
        /// 获取客户端代理类的代码
        /// </summary>
        /// <param name="p_clsName">类名</param>
        /// <returns></returns>
        public string GetAgentClass(string p_clsName)
        {
            List<string> keys = new List<string>();
            IEnumerable<XElement> comments = null;
            AgentMode mode = AgentMode.Default;
            foreach (var item in Silo.Methods)
            {
                if (!item.Key.StartsWith(p_clsName + "."))
                    continue;

                keys.Add(item.Key);
                if (comments == null)
                {
                    Type type = item.Value.Method.DeclaringType;
                    var attr = type.GetCustomAttribute<ApiAttribute>();
                    if (attr != null)
                        mode = attr.AgentMode;
                    comments = LoadComments(type);
                }
            }

            if (keys.Count == 0)
                return "未找到该类型！";

            // 不同模式的服务名
            string serviceName;
            if (mode == AgentMode.Default)
                serviceName = $"\"{Kit.SvcName}\"";
            else if (mode == AgentMode.Generic)
                serviceName = "typeof(TSvc).Name";
            else
                serviceName = "p_serviceName";

            string retTypeName;
            int paramsLength;
            StringBuilder sb = new StringBuilder();
            sb.Append("#region ");
            sb.Append(p_clsName);
            foreach (string key in keys)
            {
                ApiMethod sm = Silo.GetMethod(key);
                MethodInfo mi = sm.Method;

                // 添加注释
                sb.AppendLine();
                if (comments != null)
                    AppendComment(sm, sb, comments);

                // 自定义Agent方法代码
                var attr = mi.GetCustomAttribute<CustomAgentAttribute>(false);
                if (attr != null)
                {
                    if (!string.IsNullOrEmpty(attr.Code))
                        sb.Append(attr.Code.Replace("###", serviceName));
                    continue;
                }

                // 方法定义
                if (sm.CallMode == ApiCallMode.Unary)
                {
                    retTypeName = GetRpcTypeName(mi.ReturnType);
                    if (!string.IsNullOrEmpty(retTypeName))
                        sb.AppendFormat("public static Task<{0}> {1}(", retTypeName, mi.Name);
                    else
                        sb.AppendFormat("public static Task {0}(", mi.Name);
                }
                else
                {
                    if (sm.CallMode == ApiCallMode.ServerStream)
                        retTypeName = "ResponseReader";
                    else if (sm.CallMode == ApiCallMode.ClientStream)
                        retTypeName = "RequestWriter";
                    else
                        retTypeName = "DuplexStream";

                    sb.AppendFormat("public static Task<{0}> {1}(", retTypeName, mi.Name);
                }

                // 参数
                ParameterInfo[] infos = mi.GetParameters();
                if (infos.Length > 0)
                {
                    // 最后的RequestReader, ResponseWriter不算数
                    if (sm.CallMode == ApiCallMode.Unary)
                        paramsLength = infos.Length;
                    else if (sm.CallMode == ApiCallMode.DuplexStream)
                        paramsLength = infos.Length - 2;
                    else
                        paramsLength = infos.Length - 1;
                }
                else
                {
                    paramsLength = 0;
                }

                // 自定义服务名模式
                if (mode == AgentMode.Custom)
                {
                    sb.Append("string p_serviceName");
                    if (paramsLength > 0)
                        sb.Append(", ");
                }

                if (paramsLength > 0)
                {
                    for (int i = 0; i < paramsLength; i++)
                    {
                        var item = infos[i];

                        // 实体类型，降型Row
                        if (item.ParameterType.IsSubclassOf(typeof(Entity)))
                        {
                            sb.Append("Row ");
                            sb.Append(item.Name);
                            continue;
                        }

                        // Table<> -> Table
                        if (item.ParameterType.IsGenericType && item.ParameterType.GetGenericTypeDefinition() == typeof(Table<>))
                        {
                            sb.Append("Table ");
                            sb.Append(item.Name);
                            continue;
                        }

                        // 最后的List<object>转为params object[]，方便客户端
                        if (i == paramsLength - 1 && item.ParameterType == typeof(List<object>))
                        {
                            sb.Append("params object[] ");
                            sb.Append(item.Name);
                            continue;
                        }

                        sb.AppendFormat("{0} {1}", GetRpcTypeName(item.ParameterType), item.Name);
                        if (!DBNull.Value.Equals(item.DefaultValue))
                        {
                            if (item.DefaultValue == null)
                            {
                                if (item.ParameterType == typeof(List<object>))
                                    sb.Append(" = null");
                                else
                                    sb.Append(" = null");
                            }
                            else if (item.ParameterType == typeof(bool))
                                sb.AppendFormat(" = {0}", item.DefaultValue.ToString().ToLower());
                            else
                                sb.AppendFormat(" = {0}", item.DefaultValue);
                        }

                        if (i < paramsLength - 1)
                            sb.Append(", ");
                    }
                }
                sb.AppendLine(")");

                // 方法体
                sb.AppendLine("{");
                AppendTabSpace(sb, 1);
                if (sm.CallMode == ApiCallMode.Unary)
                    sb.AppendLine("return new UnaryRpc(");
                else if (sm.CallMode == ApiCallMode.ServerStream)
                    sb.AppendLine("return new ServerStreamRpc(");
                else if (sm.CallMode == ApiCallMode.ClientStream)
                    sb.AppendLine("return new ClientStreamRpc(");
                else
                    sb.AppendLine("return new DuplexStreamRpc(");

                // 服务名
                AppendTabSpace(sb, 2);
                sb.Append(serviceName);
                sb.AppendLine(",");

                // 完整方法名
                AppendTabSpace(sb, 2);
                sb.AppendFormat("\"{0}.{1}\"", p_clsName, mi.Name);
                if (paramsLength > 0)
                {
                    for (int i = 0; i < paramsLength; i++)
                    {
                        var item = infos[i];
                        sb.AppendLine(",");
                        AppendTabSpace(sb, 2);
                        // params object[]转为List<object>
                        if (i == paramsLength - 1 && item.ParameterType == typeof(List<object>))
                            sb.AppendFormat("({0} == null || {0}.Length == 0) ? null : {0}.ToList()", item.Name);
                        else
                            sb.Append(item.Name);
                    }
                }
                sb.AppendLine();

                // Rpc方法
                AppendTabSpace(sb, 1);
                if (sm.CallMode == ApiCallMode.Unary)
                {
                    if (!string.IsNullOrEmpty(retTypeName))
                    {
                        sb.Append(").Call<");
                        sb.Append(retTypeName);
                        sb.AppendLine(">();");
                    }
                    else
                    {
                        sb.AppendLine(").Call<object>();");
                    }
                }
                else
                {
                    sb.AppendLine(").Call();");
                }
                sb.AppendLine("}");
            }
            sb.Append("#endregion");
            return sb.ToString();
        }

        static string GetRpcTypeName(Type p_type)
        {
            string tpName = null;
            if (p_type == typeof(string))
                tpName = "string";
            else if (p_type == typeof(bool))
                tpName = "bool";
            else if (p_type == typeof(int))
                tpName = "int";
            else if (p_type == typeof(Int64))
                tpName = "long";
            else if (p_type == typeof(object))
                tpName = "object";
            else if (p_type == typeof(List<string>))
                tpName = "List<string>";
            else if (p_type == typeof(List<bool>))
                tpName = "List<bool>";
            else if (p_type == typeof(List<int>))
                tpName = "List<int>";
            else if (p_type == typeof(List<long>))
                tpName = "List<long>";
            else if (p_type == typeof(List<double>))
                tpName = "List<double>";
            else if (p_type == typeof(List<DateTime>))
                tpName = "List<DateTime>";
            else if (p_type == typeof(List<object>))
                tpName = "List<object>";
            else if (p_type == typeof(List<Table>))
                tpName = "List<Table>";
            else if (p_type == typeof(List<Dict>))
                tpName = "List<Dict>";
            else if (p_type.IsGenericType)
            {
                var name = p_type.GetGenericTypeDefinition().FullName;
                if (name == "System.Threading.Tasks.Task`1")
                    tpName = GetRpcTypeName(p_type.GetGenericArguments()[0]);
                else if (name == "Dt.Core.Table`1")
                    tpName = $"Table<{p_type.GetGenericArguments()[0].Name}>";
                else
                    tpName = p_type.Name;
            }
            else if (p_type != typeof(void) && p_type != typeof(Task))
                tpName = p_type.Name;
            return tpName;
        }

        static void AppendTabSpace(StringBuilder p_sb, int p_num)
        {
            for (int i = 0; i < p_num; i++)
            {
                p_sb.Append("    ");
            }
        }

        /// <summary>
        /// 根据类名过滤xml注释
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        static IEnumerable<XElement> LoadComments(Type p_type)
        {
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}{p_type.Assembly.GetName().Name}.xml";
            if (!File.Exists(path))
                return null;

            XDocument doc = XDocument.Load(path);
            List<XElement> results = new List<XElement>();
            XElement root = doc.Element("doc");
            if (root == null)
                return null;

            var rs = from member in root.Element("members").Elements("member")
                     where member.Attribute("name").Value.Contains(p_type.FullName)
                     select member;
            foreach (var result in rs)
            {
                results.Add(result);
            }
            return results;
        }

        /// <summary>
        /// 添加注释
        /// </summary>
        /// <param name="p_sm"></param>
        /// <param name="p_sb"></param>
        /// <param name="p_results"></param>
        static void AppendComment(ApiMethod p_sm, StringBuilder p_sb, IEnumerable<XElement> p_results)
        {
            // 获得方法在xml注释中的全名
            StringBuilder sbName = new StringBuilder();
            sbName.AppendFormat("M:{0}.{1}", p_sm.Method.DeclaringType.FullName, p_sm.Method.Name);
            ParameterInfo[] pis = p_sm.Method.GetParameters();
            if (pis.Length > 0)
            {
                sbName.Append("(");
                foreach (ParameterInfo pi in pis)
                {
                    // 只支持List<T>泛型一种情况！
                    if (pi.ParameterType.IsGenericType)
                        sbName.Append(pi.ParameterType.Namespace + ".List{" + pi.ParameterType.GenericTypeArguments[0].FullName + "},");
                    else
                        sbName.Append(pi.ParameterType.FullName + ",");
                }
                sbName.Remove(sbName.Length - 1, 1);
                sbName.Append(")");
            }
            string name = sbName.ToString();

            var member = (from result in p_results
                          where result.Attribute("name").Value == name
                          select result).FirstOrDefault();
            if (member == null)
                return;

            XElement summary = member.Element("summary");
            if (summary != null)
            {
                p_sb.AppendLine("/// <summary>");
                string[] ss = summary.Value.Trim().Split('\n');
                foreach (string str in ss)
                {
                    p_sb.AppendFormat("/// {0}\r\n", str.Trim());
                }
                p_sb.AppendLine("/// </summary>");
            }

            IEnumerable<XElement> ps = member.Elements("param");
            if (ps != null)
            {
                foreach (XElement p in ps)
                {
                    p_sb.AppendFormat("/// <param name=\"{0}\">{1}</param>\r\n", p.Attribute("name").Value, p.Value);
                }
            }

            XElement rtn = member.Element("returns");
            if (rtn != null)
            {
                string[] vals = rtn.Value.Trim().Split('\n');
                if (vals.Length == 1)
                    p_sb.AppendFormat("/// <returns>{0}</returns>\r\n", rtn.Value.Trim());
                else
                {
                    p_sb.Append("/// <returns>\r\n");
                    foreach (string val in vals)
                    {
                        p_sb.AppendFormat("/// {0}\r\n", val.Trim());
                    }
                    p_sb.Append("/// </returns>\r\n");
                }
            }
        }
        #endregion

        #region 日志
        /// <summary>
        /// 获取日志文件列表
        /// </summary>
        /// <returns></returns>
        public Task<string> GetHistoryLogFile()
        {
            return Task.Run(() =>
            {
                var dir = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "etc", "log"));
                if (dir.Exists)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var fl in dir.GetFiles().OrderByDescending(fl => fl.CreationTime))
                    {
                        sb.AppendFormat("<a onclick=\"downloadLogFile('{0}')\" href=\"javascript:void(0);\" style=\"color: white; text-decoration:underline;\">{0} 下载</a>", fl.Name);
                        sb.AppendLine();
                    }
                    return sb.ToString();
                }
                return "";
            });
        }
        #endregion
    }
}
