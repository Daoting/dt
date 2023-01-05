#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-07-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Castle.Components.DictionaryAdapter.Xml;
using System;
using System.Collections.Generic;
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
    public class SysTools : BaseApi
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
            var schema = DbSchema.GetTableSchema(tblName);

            StringBuilder sb = new StringBuilder();

            // Tbl标签
            AppendTabSpace(sb, 1);
            sb.Append($"[Tbl(\"{tblName}\")]");

            sb.AppendLine();
            AppendTabSpace(sb, 1);
            sb.Append($"public partial class {clsName} : Entity");
            sb.AppendLine();
            AppendTabSpace(sb, 1);
            sb.AppendLine("{");

            AppendTabSpace(sb, 2);
            sb.AppendLine("#region 构造方法");

            // 默认构造方法，私有为避免外部使用，内部在反序列化时使用
            AppendTabSpace(sb, 2);
            sb.AppendLine($"{clsName}() {{ }}");
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
            sb.AppendLine("}");

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
            var schema = DbSchema.GetTableSchema(tblName);

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
                int index = p_tblName.IndexOf("_");
                string svc = index == -1 ? "cm" : p_tblName.Substring(0, index).ToLower();
                sb.AppendLine($"long id = await NewID(\"{svc}\");");

                AppendTabSpace(sb, 3);
                sb.Append($"return new {clsName}(id");
                foreach (var col in schema.Columns)
                {
                    sb.Append($", {col.Name}");
                }
                sb.AppendLine(");");

                AppendTabSpace(sb, 2);
                sb.AppendLine("}");
                sb.AppendLine("");
            }

            AppendTabSpace(sb, 2);
            sb.AppendLine("protected override void InitHook()");
            AppendTabSpace(sb, 2);
            sb.AppendLine("{");
            AppendTabSpace(sb, 2);
            sb.AppendLine("}");

            AppendTabSpace(sb, 1);
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// 更新表结构缓存
        /// </summary>
        /// <returns></returns>
        public string UpdateDbSchema()
        {
            return DbSchema.LoadSchema();
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

        /// <summary>
        /// 获取所有表名
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllTables()
        {
            return DbSchema.Schema.Keys.ToList();
        }

        /// <summary>
        /// 生成Fv格内容
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public string GetFvCells(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                return null;

            string tblName = p_tblName.ToLower();
            var schema = DbSchema.GetTableSchema(tblName);
            StringBuilder sb = new StringBuilder();

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
                    sb.Append($"<a:CList ID=\"{col.Name}\"{title} Enum=\"$namespace$.{tpName},$rootnamespace$.Client\" />");
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
            return sb.ToString();
        }

        /// <summary>
        /// 生成Lv项模板
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public string GetLvItemTemplate(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                return null;

            string tblName = p_tblName.ToLower();
            var schema = DbSchema.GetTableSchema(tblName);
            StringBuilder sb = new StringBuilder();
            AppendTabSpace(sb, 3);
            sb.Append("<StackPanel Padding=\"10\">");
            foreach (var col in schema.Columns)
            {
                sb.AppendLine();
                AppendTabSpace(sb, 4);
                sb.Append($"<a:Dot ID=\"{col.Name}\" />");
            }
            sb.AppendLine();
            AppendTabSpace(sb, 3);
            sb.Append("</StackPanel>");
            return sb.ToString();
        }

        /// <summary>
        /// 生成Lv表格列
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public string GetLvTableCols(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                return null;

            string tblName = p_tblName.ToLower();
            var schema = DbSchema.GetTableSchema(tblName);
            StringBuilder sb = new StringBuilder();
            AppendTabSpace(sb, 2);
            sb.Append("<a:Cols>");
            foreach (var col in schema.Columns)
            {
                sb.AppendLine();
                AppendTabSpace(sb, 3);

                string title = "";

                // 字段名中文时不再需要Title
                if (!string.IsNullOrEmpty(col.Comments)
                    && !IsChiness(col.Name))
                {
                    title = $" Title=\"{col.Comments}\"";
                }

                sb.Append($"<a:Col ID=\"{col.Name}\"{title} />");
            }
            sb.AppendLine();
            AppendTabSpace(sb, 2);
            sb.Append("</a:Cols>");
            return sb.ToString();
        }

        /// <summary>
        /// 生成单表框架用到的sql
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <param name="p_title"></param>
        /// <param name="p_blurQuery"></param>
        /// <returns></returns>
        public async Task<string> GetSingleTblSql(string p_tblName, string p_title, bool p_blurQuery)
        {
            if (!DbSchema.Schema.ContainsKey("lob_sql"))
                return "lob_sql表不存在，无法生成框架sql";

            if (string.IsNullOrEmpty(p_tblName) || string.IsNullOrEmpty(p_title))
                return "表名和标题不可为空！";

            return await CreateTblSql(p_tblName, p_title, p_blurQuery);
        }

        /// <summary>
        /// 生成一对多框架用到的sql
        /// </summary>
        /// <param name="p_parentTbl"></param>
        /// <param name="p_parentTitle"></param>
        /// <param name="p_childTbls"></param>
        /// <param name="p_childTitles"></param>
        /// <param name="p_blurQuery"></param>
        /// <returns></returns>
        public async Task<string> GetOneToManySql(string p_parentTbl, string p_parentTitle, List<string> p_childTbls, List<string> p_childTitles, bool p_blurQuery)
        {
            if (!DbSchema.Schema.ContainsKey("lob_sql"))
                return "lob_sql表不存在，无法生成框架sql";

            if (string.IsNullOrEmpty(p_parentTbl) || string.IsNullOrEmpty(p_parentTitle))
                return "父表表名和标题不可为空，未生成sql！";

            string msg = await CreateTblSql(p_parentTbl, p_parentTitle, p_blurQuery);

            if (p_childTbls != null
                && p_childTitles != null
                && p_childTbls.Count == p_childTitles.Count)
            {
                for (int i = 0; i < p_childTbls.Count; i++)
                {
                    msg += await CreateSql($"{p_parentTitle}-关联{p_childTitles[i]}", $"select * from {p_childTbls[i]} where ParentID=@ParentID");
                    msg += await CreateSql(p_childTitles[i] + "-编辑", $"select * from {p_childTbls[i]} where id=@id");
                }
            }
            else
            {
                msg += "未生成子表sql\r\n";
            }
            return msg;
        }

        /// <summary>
        /// 生成多对多框架用到的sql
        /// </summary>
        /// <param name="p_mainTbl"></param>
        /// <param name="p_mainTitle"></param>
        /// <param name="p_blurQuery"></param>
        /// <returns></returns>
        public async Task<string> GetManyToManySql(string p_mainTbl, string p_mainTitle, bool p_blurQuery)
        {
            if (!DbSchema.Schema.ContainsKey("lob_sql"))
                return "lob_sql表不存在，无法生成框架sql";

            if (string.IsNullOrEmpty(p_mainTbl) || string.IsNullOrEmpty(p_mainTitle))
                return "主表表名和标题不可为空，未生成sql！";

            string msg = await CreateTblSql(p_mainTbl, p_mainTitle, p_blurQuery);
            return msg;
        }

        /// <summary>
        /// 表是否包含ParentID字段
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public bool ExistParentID(string p_tblName)
        {
            var schema = DbSchema.GetTableSchema(p_tblName);
            foreach (var col in schema.Columns)
            {
                if (col.Name == "ParentID")
                    return true;
            }
            return false;
        }

        const string _sqlInsert = "insert into lob_sql (id, `sql`) values (@id, @sql)";
        const string _sqlSelect = "select count(*) from lob_sql where id=@id";

        async Task<string> CreateTblSql(string p_tblName, string p_title, bool p_blurQuery)
        {
            string msg = await CreateSql(p_title + "-全部", $"select * from {p_tblName}");
            msg += await CreateSql(p_title + "-编辑", $"select * from {p_tblName} where id=@id");

            if (p_blurQuery)
            {
                var schema = DbSchema.GetTableSchema(p_tblName);
                StringBuilder sb = new StringBuilder();
                foreach (var col in schema.Columns)
                {
                    if (col.Type == typeof(string))
                    {
                        if (sb.Length > 0)
                            sb.Append(" OR ");
                        sb.Append(col.Name);
                        sb.AppendLine(" LIKE @input");
                    }
                }

                msg += await CreateSql(p_title + "-模糊查询", $"select * from {p_tblName} where \r\n {sb.ToString()}");
            }

            return msg;
        }

        async Task<string> CreateSql(string p_key, string p_sql)
        {
            string msg;
            int cnt = await Dp.GetScalar<int>(_sqlSelect, new { id = p_key });
            if (cnt == 0)
            {
                cnt = await Dp.Exec(_sqlInsert, new { id = p_key, sql = p_sql });
                msg = cnt > 0 ? $"[{p_key}] sql生成成功\r\n" : $"[{p_key}] sql生成失败\r\n";
            }
            else
            {
                msg = $"[{p_key}] sql已存在\r\n";
            }
            return msg;
        }

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
            return clsName + "Obj";
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
                if ((int)vChar < 256)
                    return false;
            }
            return true;
        }
    }
}
