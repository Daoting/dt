#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-07-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
        /// 生成服务端实体类
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public string GetServerTblCls(string p_tblName)
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
            AppendTabSpace(sb, 1);
            sb.Append($"[Tbl(\"{tblName}\")]");
            sb.AppendLine();
            AppendTabSpace(sb, 1);
            sb.Append($"public partial class {clsName} : Root");
            sb.AppendLine();
            AppendTabSpace(sb, 1);
            sb.AppendLine("{");

            AppendTabSpace(sb, 2);
            sb.Append($"public {clsName}()");
            sb.AppendLine();
            AppendTabSpace(sb, 2);
            sb.AppendLine("{ }");
            sb.AppendLine();

            AppendTabSpace(sb, 2);
            sb.Append("public ");
            sb.Append(clsName);
            sb.Append("(");
            foreach (var col in schema.Columns)
            {
                sb.Append(col.TypeName);
                sb.Append(" p_");
                sb.Append(col.Name);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(")");
            sb.AppendLine();
            AppendTabSpace(sb, 2);
            sb.AppendLine("{");
            foreach (var col in schema.Columns)
            {
                AppendTabSpace(sb, 3);
                sb.Append(SetFirstToUpper(col.Name));
                sb.Append(" = p_");
                sb.Append(col.Name);
                sb.AppendLine(";");
            }
            AppendTabSpace(sb, 2);
            sb.AppendLine("}");

            foreach (var col in schema.Columns)
            {
                sb.AppendLine();
                AppendTabSpace(sb, 2);
                sb.AppendLine("/// <summary>");
                AppendTabSpace(sb, 2);
                sb.Append("/// ");
                sb.AppendLine(col.Comments);
                AppendTabSpace(sb, 2);
                sb.AppendLine("/// </summary>");
                AppendTabSpace(sb, 2);
                sb.Append($"public {col.TypeName}{(col.Nullable ? "?" : "")} {SetFirstToUpper(col.Name)} ");
                sb.Append("{ get; private set; }");
                sb.AppendLine();
            }
            AppendTabSpace(sb, 1);
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// 生成客户端实体类
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public string GetClientTblCls(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                return null;

            string tblName = p_tblName.ToLower();
            string clsName;
            string[] arr = tblName.Split('_');
            if (arr.Length > 1)
                clsName = SetFirstToUpper(arr[1]) + "Row";
            else
                clsName = SetFirstToUpper(tblName) + "Row";
            var schema = DbSchema.GetTableSchema(tblName);

            StringBuilder sb = new StringBuilder();
            AppendTabSpace(sb, 1);
            sb.Append($"public partial class {clsName} : Row");
            sb.AppendLine();
            AppendTabSpace(sb, 1);
            sb.Append("{");

            foreach (var col in schema.Columns)
            {
                sb.AppendLine();
                AppendTabSpace(sb, 2);
                sb.AppendLine("/// <summary>");
                AppendTabSpace(sb, 2);
                sb.Append("/// ");
                sb.AppendLine(col.Comments);
                AppendTabSpace(sb, 2);
                sb.AppendLine("/// </summary>");
                AppendTabSpace(sb, 2);
                string tpName = col.TypeName + (col.Nullable ? "?" : "");
                sb.Append($"public {tpName} {SetFirstToUpper(col.Name)} ");
                sb.AppendLine();
                AppendTabSpace(sb, 2);
                sb.AppendLine("{");
                AppendTabSpace(sb, 3);
                sb.AppendLine($"get {{ return GetVal<{col.TypeName}>(\"{col.Name}\"); }}");
                AppendTabSpace(sb, 3);
                sb.AppendLine($"set {{ _cells[\"{col.Name}\"].Val = value; }}");
                AppendTabSpace(sb, 2);
                sb.AppendLine("}");
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
