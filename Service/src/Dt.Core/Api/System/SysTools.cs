#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-07-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Castle.Components.DictionaryAdapter.Xml;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 所有服务内部使用的工具Api
    /// </summary>
    [Api(AgentMode = AgentMode.Generic)]
    public class SysTools : DomainSvc
    {
        /// <summary>
        /// 生成实体类
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <param name="p_clsName">类名，null时按规则生成：移除前后缀，首字母大写</param>
        /// <returns></returns>
        public string GetEntityClass(string p_tblName, string p_clsName = null)
        {
            if (string.IsNullOrEmpty(p_tblName))
                return null;

            string tblName = p_tblName.ToLower();
            string clsName = string.IsNullOrEmpty(p_clsName) ? GetClsName(tblName) : p_clsName;
            var schema = _da.GetTableSchema(tblName);

            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(schema.Comments))
            {
                // 取注释中的服务名，和字段注释中的枚举类型相同
                string svc = null;
                var match = Regex.Match(schema.Comments, @"^#[^\s#]+");
                if (match.Success)
                    svc = match.Value.Trim('#');

                AppendTabSpace(sb, 1);
                sb.AppendLine("/// <summary>");
                AppendTabSpace(sb, 1);
                sb.Append("/// ");
                if (svc != null)
                    sb.AppendLine(schema.Comments.Substring(svc.Length + 2));
                else
                    sb.AppendLine(schema.Comments);
                AppendTabSpace(sb, 1);
                sb.AppendLine("/// </summary>");

                // Tbl标签
                AppendTabSpace(sb, 1);
                if (svc != null)
                    sb.Append($"[Tbl(\"{schema.Name}\", \"{svc}\")]");
                else
                    sb.Append($"[Tbl(\"{schema.Name}\")]");
            }
            else
            {
                // Tbl标签
                AppendTabSpace(sb, 1);
                sb.Append($"[Tbl(\"{schema.Name}\")]");
            }

            sb.AppendLine();
            AppendTabSpace(sb, 1);
            sb.Append($"public partial class {clsName} : EntityX<{clsName}>");
            sb.AppendLine();
            AppendTabSpace(sb, 1);
            sb.AppendLine("{");

            AppendTabSpace(sb, 2);
            sb.AppendLine("#region 构造方法");

            // 默认构造方法，私有为避免外部使用，内部在反序列化时使用
            AppendTabSpace(sb, 2);
            sb.AppendLine($"{clsName}() {{ }}");
            sb.AppendLine();

            // 构造方法，实体间类型转换时使用
            AppendTabSpace(sb, 2);
            sb.AppendLine($"public {clsName}(CellList p_cells) : base(p_cells) {{ }}");
            sb.AppendLine();

            // 构造方法
            AppendTabSpace(sb, 2);
            sb.Append("public ");
            sb.Append(clsName);
            sb.AppendLine("(");
            foreach (var col in schema.PrimaryKey)
            {
                AppendTabSpace(sb, 3);
                var colType = col.Type == typeof(byte) ? GetEnumName(col) : GetTypeName(col.Type);
                sb.Append(colType);
                sb.Append(" ");
                sb.Append(col.Name);
                sb.AppendLine(",");
            }
            foreach (var col in schema.Columns)
            {
                AppendTabSpace(sb, 3);
                var colType = col.Type == typeof(byte) ? GetEnumName(col) : GetTypeName(col.Type);
                sb.Append(colType);
                sb.Append(" ");
                sb.Append(col.Name);
                if (string.IsNullOrEmpty(col.Default))
                {
                    sb.AppendLine(" = default,");
                }
                else if (col.Type == typeof(string))
                {
                    sb.AppendLine($" = \"{col.Default}\",");
                }
                else if (col.Type == typeof(bool))
                {
                    sb.Append(" = ");
                    if (col.Default == "1")
                        sb.AppendLine("true,");
                    else
                        sb.AppendLine("false,");
                }
                else if (col.Type == typeof(byte) && colType != "byte")
                {
                    sb.AppendLine($" = ({colType}){col.Default},");
                }
                else
                {
                    sb.AppendLine($" = {col.Default},");
                }
            }
            sb.Remove(sb.Length - 3, 3);
            sb.Append(")");
            sb.AppendLine();
            AppendTabSpace(sb, 2);
            sb.AppendLine("{");
            foreach (var col in schema.PrimaryKey.Concat(schema.Columns))
            {
                AppendTabSpace(sb, 3);
                sb.Append("AddCell(\"");
                // 简化写法不需要类型
                //sb.Append(GetTypeName(col.Type));
                //sb.Append(">(\"");
                sb.Append(col.Name);
                sb.Append("\", ");
                // 内部为enum类型
                //if (IsEnumCol(col))
                //    sb.Append("(byte)");
                sb.Append(col.Name);
                sb.AppendLine(");");
            }
            AppendTabSpace(sb, 3);
            sb.AppendLine("IsAdded = true;");
            AppendTabSpace(sb, 2);
            sb.AppendLine("}");
            AppendTabSpace(sb, 2);
            sb.AppendLine("#endregion");

            // 主键属性
            bool existID = false;
            foreach (var col in schema.PrimaryKey)
            {
                if (col.Name.Equals("ID", StringComparison.OrdinalIgnoreCase))
                {
                    existID = true;
                    if (col.Type != typeof(long))
                    {
                        // 类型不同时，覆盖原ID
                        AppendColumn(col, sb, true);
                    }
                }
                else
                {
                    AppendColumn(col, sb, false);
                }
            }
            if (!existID)
            {
                // 无ID时，屏蔽ID属性
                sb.AppendLine();
                AppendTabSpace(sb, 2);
                sb.AppendLine("new public long ID { get { return -1; } }");
            }

            // 普通列
            foreach (var col in schema.Columns)
            {
                AppendColumn(col, sb, false);
            }

            AppendTabSpace(sb, 1);
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// 生成实体类的扩展部分，如 InitHook New
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <param name="p_clsName">类名，null时按规则生成：移除前后缀，首字母大写</param>
        /// <returns></returns>
        public string GetEntityClassEx(string p_tblName, string p_clsName = null)
        {
            if (string.IsNullOrEmpty(p_tblName))
                return null;

            string tblName = p_tblName.ToLower();
            string clsName = string.IsNullOrEmpty(p_clsName) ? GetClsName(tblName) : p_clsName;
            var schema = _da.GetTableSchema(tblName);

            StringBuilder sb = new StringBuilder();
            AppendTabSpace(sb, 1);
            sb.Append($"public partial class {clsName}");
            sb.AppendLine();
            AppendTabSpace(sb, 1);
            sb.AppendLine("{");

            // 只对单主键id情况生成 New 方法
            if (schema.PrimaryKey.Count == 1
                && schema.PrimaryKey[0].Name.ToLower() == "id"
                && schema.PrimaryKey[0].Type == typeof(long))
            {
                AppendTabSpace(sb, 2);
                sb.Append($"public static async Task<{clsName}> New");
                sb.AppendLine("(");

                foreach (var col in schema.Columns)
                {
                    AppendTabSpace(sb, 3);
                    var colType = col.Type == typeof(byte) ? GetEnumName(col) : GetTypeName(col.Type);
                    sb.Append(colType);
                    sb.Append(" ");
                    sb.Append(col.Name);
                    if (string.IsNullOrEmpty(col.Default))
                    {
                        sb.AppendLine(" = default,");
                    }
                    else if (col.Type == typeof(string))
                    {
                        sb.AppendLine($" = \"{col.Default}\",");
                    }
                    else if (col.Type == typeof(bool))
                    {
                        sb.Append(" = ");
                        if (col.Default == "1")
                            sb.AppendLine("true,");
                        else
                            sb.AppendLine("false,");
                    }
                    else if (col.Type == typeof(byte) && colType != "byte")
                    {
                        sb.AppendLine($" = ({colType}){col.Default},");
                    }
                    else
                    {
                        sb.AppendLine($" = {col.Default},");
                    }
                }
                sb.Remove(sb.Length - 3, 3);
                sb.AppendLine(")");
                AppendTabSpace(sb, 2);
                sb.AppendLine("{");

                AppendTabSpace(sb, 3);
                sb.AppendLine($"return new {clsName}(");
                AppendTabSpace(sb, 4);
                sb.AppendLine("ID: await NewID(),");
                foreach (var col in schema.Columns)
                {
                    AppendTabSpace(sb, 4);
                    sb.AppendLine($"{col.Name}: {col.Name},");
                }
                sb.Remove(sb.Length - 3, 3);
                sb.AppendLine(");");

                AppendTabSpace(sb, 2);
                sb.AppendLine("}");
                sb.AppendLine("");
            }

            sb.Append(_initHook);

            AppendTabSpace(sb, 1);
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// 获取最新的所有表名
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllTables()
        {
            return _da.GetAllTableNames();
        }

        /// <summary>
        /// 生成Fv格内容
        /// </summary>
        /// <param name="p_tblNames"></param>
        /// <returns></returns>
        public string GetFvCells(List<string> p_tblNames)
        {
            if (p_tblNames == null || p_tblNames.Count == 0)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (var tbl in p_tblNames)
            {
                if (string.IsNullOrEmpty(tbl))
                    continue;

                var schema = _da.GetTableSchema(tbl);
                foreach (var col in schema.Columns)
                {
                    if (sb.Length > 0)
                        sb.AppendLine();
                    AppendTabSpace(sb, 2);
                    bool isEnum = IsEnumCol(col);

                    string title = "";

                    // 字段名中文时不再需要Title
                    if (!string.IsNullOrEmpty(col.Comments)
                        && !isEnum
                        && !IsChiness(col.Name))
                    {
                        title = $" Title=\"{col.Comments}\"";
                    }

                    if (isEnum)
                    {
                        string tpName = GetEnumName(col);
                        title = col.Comments.Substring(tpName.Length + 2);
                        title = string.IsNullOrEmpty(title) ? "" : $" Title=\"{title}\"";
                        sb.Append($"<a:CList ID=\"{col.Name}\"{title} />");
                    }
                    else if (col.Type == typeof(bool))
                    {
                        sb.Append($"<a:CBool ID=\"{col.Name}\"{title} />");
                    }
                    else if (col.Type == typeof(int))
                    {
                        sb.Append($"<a:CNum ID=\"{col.Name}\"{title} IsInteger=\"True\" />");
                    }
                    else if (col.Type == typeof(long) || col.Type == typeof(double))
                    {
                        sb.Append($"<a:CNum ID=\"{col.Name}\"{title} />");
                    }
                    else if (col.Type == typeof(DateTime))
                    {
                        sb.Append($"<a:CDate ID=\"{col.Name}\"{title} />");
                    }
                    else
                    {
                        sb.Append($"<a:CText ID=\"{col.Name}\"{title} />");
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成Lv项模板
        /// </summary>
        /// <param name="p_tblNames"></param>
        /// <returns></returns>
        public string GetLvItemTemplate(List<string> p_tblNames)
        {
            if (p_tblNames == null || p_tblNames.Count == 0)
                return null;

            StringBuilder sb = new StringBuilder();
            AppendTabSpace(sb, 3);
            sb.Append("<StackPanel Padding=\"10\">");

            foreach (var tbl in p_tblNames)
            {
                if (string.IsNullOrEmpty(tbl))
                    continue;

                var schema = _da.GetTableSchema(tbl);
                foreach (var col in schema.Columns)
                {
                    sb.AppendLine();
                    AppendTabSpace(sb, 4);
                    sb.Append($"<a:Dot ID=\"{col.Name}\" />");
                }
            }
            sb.AppendLine();
            AppendTabSpace(sb, 3);
            sb.Append("</StackPanel>");
            return sb.ToString();
        }

        /// <summary>
        /// 生成Lv表格列
        /// </summary>
        /// <param name="p_tblNames"></param>
        /// <returns></returns>
        public string GetLvTableCols(List<string> p_tblNames)
        {
            if (p_tblNames == null || p_tblNames.Count == 0)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (var tbl in p_tblNames)
            {
                if (string.IsNullOrEmpty(tbl))
                    continue;

                var schema = _da.GetTableSchema(tbl);
                foreach (var col in schema.Columns)
                {
                    string title = "";

                    // 字段名中文时不再需要Title
                    if (!string.IsNullOrEmpty(col.Comments)
                        && !IsChiness(col.Name))
                    {
                        if (IsEnumCol(col))
                        {
                            string tpName = GetEnumName(col);
                            title = $" Title=\"{col.Comments.Substring(tpName.Length + 2)}\"";
                        }
                        else
                        {
                            title = $" Title=\"{col.Comments}\"";
                        }
                    }

                    if (sb.Length > 0)
                        sb.AppendLine();
                    AppendTabSpace(sb, 3);
                    sb.Append($"<a:Col ID=\"{col.Name}\"{title} />");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成模糊查询的where子句部分，可能多表(有扩展表)
        /// </summary>
        /// <param name="p_tblNames"></param>
        /// <returns></returns>
        public string GetBlurClause(List<string> p_tblNames)
        {
            if (p_tblNames == null || p_tblNames.Count == 0)
                return null;

            StringBuilder sb = new StringBuilder("where 1!=1");
            foreach (var tbl in p_tblNames)
            {
                if (string.IsNullOrEmpty(tbl))
                    continue;

                var schema = _da.GetTableSchema(tbl);
                foreach (var col in schema.Columns)
                {
                    if (col.Type == typeof(string))
                    {
                        sb.Append(" or ");
                        sb.Append(col.Name);
                        sb.Append(" like @input");
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成查询面板的FvCells
        /// </summary>
        /// <param name="p_tblNames"></param>
        /// <returns></returns>
        public string GetQueryFvCells(List<string> p_tblNames)
        {
            if (p_tblNames == null || p_tblNames.Count == 0)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (var tbl in p_tblNames)
            {
                if (string.IsNullOrEmpty(tbl))
                    continue;

                var schema = _da.GetTableSchema(tbl);
                foreach (var col in schema.Columns)
                {
                    if (sb.Length > 0)
                        sb.AppendLine();
                    AppendTabSpace(sb, 3);
                    bool isEnum = IsEnumCol(col);

                    string title = "";

                    // 字段名中文时不再需要Title
                    if (!string.IsNullOrEmpty(col.Comments)
                        && !isEnum
                        && !IsChiness(col.Name))
                    {
                        title = $" Title=\"{col.Comments}\"";
                    }

                    if (isEnum)
                    {
                        string tpName = GetEnumName(col);
                        title = col.Comments.Substring(tpName.Length + 2);
                        title = string.IsNullOrEmpty(title) ? "" : $" Title=\"{title}\"";
                        sb.Append($"<a:CList ID=\"{col.Name}\"{title} Query=\"Editable\" />");
                    }
                    else if (col.Type == typeof(bool))
                    {
                        sb.Append($"<a:CBool ID=\"{col.Name}\"{title} Query=\"Editable\" />");
                    }
                    else if (col.Type == typeof(int))
                    {
                        sb.Append($"<a:CNum ID=\"{col.Name}\"{title} IsInteger=\"True\" Query=\"Editable\" />");
                    }
                    else if (col.Type == typeof(long) || col.Type == typeof(double))
                    {
                        sb.Append($"<a:CNum ID=\"{col.Name}\"{title} Query=\"Editable\" />");
                    }
                    else if (col.Type == typeof(DateTime))
                    {
                        sb.Append($"<a:CDate ID=\"{col.Name}\"{title} Query=\"Editable\" />");
                    }
                    else
                    {
                        sb.Append($"<a:CText ID=\"{col.Name}\"{title} Query=\"Editable\" />");
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成查询面板的Row数据源
        /// </summary>
        /// <param name="p_tblNames"></param>
        /// <returns></returns>
        public string GetQueryFvData(List<string> p_tblNames)
        {
            if (p_tblNames == null || p_tblNames.Count == 0)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (var tbl in p_tblNames)
            {
                if (string.IsNullOrEmpty(tbl))
                    continue;

                var schema = _da.GetTableSchema(tbl);
                foreach (var col in schema.Columns)
                {
                    bool isEnum = IsEnumCol(col);
                    string tpName = isEnum ? GetEnumName(col) : GetTypeName(col.Type);

                    AppendTabSpace(sb, 3);
                    sb.AppendLine($"row.AddCell<{tpName}>(\"{col.Name}\");");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 表是否包含ParentID字段
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public bool ExistParentID(string p_tblName)
        {
            var schema = _da.GetTableSchema(p_tblName);
            foreach (var col in schema.Columns)
            {
                if (col.Name == "ParentID")
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 重新加载Cache.db中的sql语句
        /// </summary>
        public void UpdateSqlCache()
        {
            Silo.LoadCacheSql();
        }

        /// <summary>
        /// 获取所有微服务
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllSvcs()
        {
            return Kit.GetAllSvcs(false);
        }

        /// <summary>
        /// 获取所有微服务副本
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllSvcInsts()
        {
            return Kit.GetAllSvcs(true);
        }

        #region 内部方法
        void AppendColumn(TableCol p_col, StringBuilder p_sb, bool p_isNew)
        {
            // 枚举类型特殊
            bool isEnum = IsEnumCol(p_col);
            string tpName = isEnum ? GetEnumName(p_col) : GetTypeName(p_col.Type);

            p_sb.AppendLine();
            AppendTabSpace(p_sb, 2);
            p_sb.AppendLine("/// <summary>");
            AppendTabSpace(p_sb, 2);
            p_sb.Append("/// ");
            if (isEnum)
                p_sb.AppendLine(p_col.Comments.Substring(tpName.Length + 2));
            else
                p_sb.AppendLine(p_col.Comments);
            AppendTabSpace(p_sb, 2);
            p_sb.AppendLine("/// </summary>");
            AppendTabSpace(p_sb, 2);

            if (p_isNew)
                p_sb.Append("new ");
            p_sb.AppendLine($"public {tpName} {p_col.Name}");

            AppendTabSpace(p_sb, 2);
            p_sb.AppendLine("{");
            AppendTabSpace(p_sb, 3);
            p_sb.AppendLine($"get {{ return ({tpName})this[\"{p_col.Name}\"]; }}");
            AppendTabSpace(p_sb, 3);
            p_sb.AppendLine($"set {{ this[\"{p_col.Name}\"] = value; }}");
            AppendTabSpace(p_sb, 2);
            p_sb.AppendLine("}");
        }

        string GetClsName(string p_tblName)
        {
            string clsName;
            string[] arr = p_tblName.Split('_');
            if (arr.Length > 1)
            {
                clsName = SetFirstToUpper(arr[1]);
                if (arr.Length > 2)
                {
                    for (int i = 2; i < arr.Length; i++)
                    {
                        clsName += SetFirstToUpper(arr[i]);
                    }
                }
            }
            else
            {
                clsName = SetFirstToUpper(p_tblName);
            }
            return clsName + "X";
        }

        string GetEnumName(TableCol p_col)
        {
            if (!string.IsNullOrEmpty(p_col.Comments))
            {
                var match = Regex.Match(p_col.Comments, @"^#[^\s#]+");
                if (match.Success)
                    return match.Value.Trim('#');
            }
            return "byte";
        }

        bool IsEnumCol(TableCol p_col)
        {
            return p_col.Type == typeof(byte)
                && !string.IsNullOrEmpty(p_col.Comments)
                && Regex.IsMatch(p_col.Comments, @"^#[^\s#]+");
        }

        string GetTypeName(Type p_type)
        {
            if (p_type.IsGenericType && p_type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return GetTypeName(p_type.GetGenericArguments()[0]) + "?";

            if (p_type == typeof(string))
                return "string";
            if (p_type == typeof(bool))
                return "bool";
            if (p_type == typeof(int))
                return "int";
            if (p_type == typeof(long))
                return "long";
            if (p_type == typeof(double))
                return "double";
            if (p_type == typeof(byte))
                return "byte";
            if (p_type == typeof(sbyte))
                return "sbyte";
            return p_type.Name;
        }

        static void AppendTabSpace(StringBuilder p_sb, int p_num)
        {
            for (int i = 0; i < p_num; i++)
            {
                p_sb.Append("    ");
            }
        }

        static string SetFirstToUpper(string p_str)
        {
            char[] a = p_str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        static bool IsChiness(string p_str)
        {
            foreach (char vChar in p_str)
            {
                if ((int)vChar > 255)
                    return true;
            }
            return false;
        }

        const string _initHook =
@"        protected override void InitHook()
        {
            //OnSaving(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnSaved(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnDeleting(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnDeleted(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnChanging<string>(nameof(Name), v =>
            //{
                
            //});
        }
";
        #endregion
    }
}
