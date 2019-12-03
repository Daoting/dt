#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-07-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 所有服务内部使用的工具Api
    /// </summary>
    [Api(GroupName = "系统工具", AgentMode = AgentMode.Generic)]
    public class SysTools : BaseApi
    {
        /// <summary>
        /// 重新加载Cache.db中的sql语句
        /// </summary>
        public void 刷新sql缓存()
        {
            Silo.LoadCacheSql();
        }

        /// <summary>
        /// 生成实体类
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public string GetEntityCls(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                return null;

            string tblName = p_tblName.ToLower();
            string clsName;
            string[] arr = tblName.Split('_');
            if (arr.Length > 1)
                clsName = SetFirstToUpper(arr[1]);
            else
                clsName = SetFirstToUpper(tblName);
            var schema = DbSchema.GetTableSchema(tblName);

            StringBuilder sb = new StringBuilder();

            // Tbl标签
            AppendTabSpace(sb, 1);
            sb.Append($"[Tbl(\"{tblName}\", \"{Glb.SvcName}\")]");

            sb.AppendLine();
            AppendTabSpace(sb, 1);
            sb.Append($"public partial class {clsName} : Entity");
            sb.AppendLine();
            AppendTabSpace(sb, 1);
            sb.AppendLine("{");

            // 默认构造方法
            AppendTabSpace(sb, 2);
            sb.Append($"public {clsName}()");
            sb.AppendLine();
            AppendTabSpace(sb, 2);
            sb.AppendLine("{ }");
            sb.AppendLine();

            // 构造方法
            AppendTabSpace(sb, 2);
            sb.Append("public ");
            sb.Append(clsName);
            sb.AppendLine("(");
            foreach (var col in schema.PrimaryKey)
            {
                AppendTabSpace(sb, 3);
                sb.Append(GetTypeName(col.Type));
                sb.Append(" ");
                sb.Append(col.Name);
                sb.AppendLine(",");
            }
            foreach (var col in schema.Columns)
            {
                AppendTabSpace(sb, 3);
                sb.Append(GetTypeName(col.Type));
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
                sb.Append("AddCell<");
                sb.Append(GetTypeName(col.Type));
                sb.Append(">(\"");
                sb.Append(col.Name);
                sb.Append("\", ");
                sb.Append(col.Name);
                sb.AppendLine(");");
            }
            AppendTabSpace(sb, 3);
            sb.AppendLine("IsAdded = true;");
            AppendTabSpace(sb, 2);
            sb.AppendLine("}");

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

        public Task<List<string>> 所有微服务()
        {
            return Glb.GetCurrentSvcs(false);
        }

        public Task<List<string>> 所有微服务副本()
        {
            return Glb.GetCurrentSvcs(true);
        }

        void AppendColumn(TableCol p_col, StringBuilder p_sb, bool p_isNew)
        {
            p_sb.AppendLine();
            AppendTabSpace(p_sb, 2);
            p_sb.AppendLine("/// <summary>");
            AppendTabSpace(p_sb, 2);
            p_sb.Append("/// ");
            p_sb.AppendLine(p_col.Comments);
            AppendTabSpace(p_sb, 2);
            p_sb.AppendLine("/// </summary>");
            AppendTabSpace(p_sb, 2);

            string tpName = GetTypeName(p_col.Type);
            if (p_isNew)
                p_sb.Append("new ");
            p_sb.AppendLine($"public {tpName} {p_col.Name}");

            AppendTabSpace(p_sb, 2);
            p_sb.AppendLine("{");
            AppendTabSpace(p_sb, 3);
            p_sb.AppendLine($"get {{ return ({tpName})_cells[\"{p_col.Name}\"].Val; }}");
            AppendTabSpace(p_sb, 3);
            // 私有，确保外部只能通过统一方法设置，DDD规范
            p_sb.AppendLine($"private set {{ _cells[\"{p_col.Name}\"].Val = value; }}");
            AppendTabSpace(p_sb, 2);
            p_sb.AppendLine("}");
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
    }
}
