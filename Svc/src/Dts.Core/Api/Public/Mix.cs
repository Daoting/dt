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
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Dts.Core.Db;
#endregion

namespace Dts.Core
{
    /// <summary>
    /// 所有服务的公共Api
    /// </summary>
    [Api(false, null, AgentMode.Custom)]
    public class Mix : BaseApi
    {
        /*
        /// <summary>
        /// 获取菜单项的实时提示信息，只取曾经点击过的项
        /// </summary>
        /// <param name="p_menuItems">菜单项列表</param>
        /// <returns></returns>
        public async Task<Dict> GetMenuTips(List<string> p_menuItems)
        {
            if (string.IsNullOrEmpty(UserID) || p_menuItems == null || p_menuItems.Count == 0)
                return null;

            DataAccess da = new DataAccess();
            Dict result = new Dict();
            foreach (string id in p_menuItems)
            {
                if (Glb.MenuTips.TryGetValue(id, out IMenuTip tip))
                    result[id] = await tip.GetTip(da, UserID);
            }
            return result;
        }

        /// <summary>
        /// 获取仪表盘数据源
        /// </summary>
        /// <returns></returns>
        public async Task<List<object>> GetDashboardData(List<string> p_bds)
        {
            if (string.IsNullOrEmpty(UserID) || p_bds == null || p_bds.Count == 0)
                return null;

            DataAccess da = new DataAccess();
            List<object> result = new List<object>();
            foreach (var name in p_bds)
            {
                if (Glb.Dashboards.TryGetValue(name, out IDashboard bd))
                    result.Add(await bd.GetData(da, UserID));
                else
                    result.Add(null);
            }
            return result;
        }

        /// <summary>
        /// 获取客户端后台任务的提示信息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_roles"></param>
        /// <returns></returns>
        public async Task<List<Dict>> GetBgTaskInfo(string p_userID, string p_roles)
        {
            if (string.IsNullOrEmpty(p_userID) || string.IsNullOrEmpty(p_roles))
                return null;

            var roles = p_roles.Split(',');
            List<Dict> ls = new List<Dict>();
            DataAccess da = new DataAccess();
            foreach (var bgTask in Glb.BgTasks)
            {
                Dict dt = await bgTask.GetInfo(da, p_userID, roles);
                if (dt != null)
                    ls.Add(dt);
            }
            if (ls.Count == 0)
                return null;
            return ls;
        }

        /// <summary>
        /// 获取格式化后的sql语句
        /// </summary>
        /// <param name="p_methodName"></param>
        /// <param name="p_args"></param>
        /// <returns></returns>
        public string GetFormatSql(string p_methodName, List<object> p_args)
        {
            if (string.IsNullOrEmpty(p_methodName) || p_args == null || p_args.Count < 2)
                return null;

            string sql = null;
            Dict dt = null;
            List<object> formats = null;
            switch (p_methodName)
            {
                case "Db.Query":
                    sql = Glb.Sql((string)p_args[0]);
                    dt = (Dict)p_args[1];
                    formats = (List<object>)p_args[2];
                    if (formats != null && formats.Count > 0)
                        sql = string.Format(sql, formats.ToArray());
                    break;
                case "Db.QuerySql":
                    sql = p_args[0] as string;
                    dt = (Dict)p_args[1];
                    break;
                case "Db.QueryPage":
                    int startRow = (int)p_args[0];
                    int pageSize = (int)p_args[1];
                    sql = Glb.Sql((string)p_args[2]);
                    dt = (Dict)p_args[3];
                    formats = (List<object>)p_args[4];
                    if (formats != null && formats.Count > 0)
                        sql = string.Format(sql, formats.ToArray());
                    sql = string.Format("select * from ({0}) a limit {1},{2} ", sql, startRow, pageSize);
                    break;
                case "Db.QuerySqlPage":
                    int startIdx = (int)p_args[0];
                    int pageLength = (int)p_args[1];
                    sql = p_args[2] as string;
                    dt = (Dict)p_args[3];
                    sql = string.Format("select * from ({0}) a limit {1},{2} ", sql, startIdx, pageLength);
                    break;
                case "Db.GetScalar":
                    sql = Glb.Sql((string)p_args[0]);
                    dt = (Dict)p_args[1];
                    formats = (List<object>)p_args[2];
                    if (formats != null && formats.Count > 0)
                        sql = string.Format(sql, formats.ToArray());
                    break;
                case "Db.GetSqlScalar":
                    sql = p_args[0] as string;
                    dt = (Dict)p_args[1];
                    break;
                case "Db.GetRow":
                    sql = Glb.Sql((string)p_args[0]);
                    dt = (Dict)p_args[1];
                    formats = (List<object>)p_args[2];
                    if (formats != null && formats.Count > 0)
                        sql = string.Format(sql, formats.ToArray());
                    break;
                case "Db.GetSqlRow":
                    sql = p_args[0] as string;
                    dt = (Dict)p_args[1];
                    break;
                case "Db.GetDataByKey":
                    sql = Glb.Sql((string)p_args[0]);
                    string filter = (string)p_args[1];
                    if (!string.IsNullOrEmpty(filter))
                        sql = string.Format("select * from ({0}) a where {1}", sql, filter);
                    break;
                case "Db.Exec":
                    sql = Glb.Sql((string)p_args[0]);
                    dt = (Dict)p_args[1];
                    break;
                case "Db.ExecSql":
                    sql = p_args[0] as string;
                    dt = (Dict)p_args[1];
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(sql))
                return new LoggerSql(sql, dt).GetSql();
            return null;
        }

        /// <summary>
        /// 获取单元格初始值
        /// </summary>
        /// <returns></returns>
        public async Task<Dict> GetCellInitVal(Dict p_dt)
        {
            if (p_dt == null || p_dt.Count == 0)
                return null;

            string sql = null;
            DataAccess da = new DataAccess();
            Dict resut = new Dict();
            foreach (var item in p_dt)
            {
                if (item.Value != null
                    && !string.IsNullOrEmpty(sql = item.Value.ToString()))
                {
                    object val = await da.GetSqlScalar<object>(sql);
                    if (DBNull.Value.Equals(val))
                        resut[item.Key] = null;
                    else
                        resut[item.Key] = val;
                }
            }
            return resut;
        }

        /// <summary>
        /// 查询报表数据集
        /// </summary>
        /// <param name="p_tbl">数据源描述</param>
        /// <param name="p_params">参数值</param>
        /// <param name="p_macro">宏参数名列表</param>
        /// <returns>数据集</returns>
        public async Task<Dict> GetReportDataSet(DataTable p_tbl, Dict p_params, List<string> p_macro)
        {
            if (p_tbl == null || p_tbl.Rows.Count == 0)
                return null;

            Dict dt = new Dict();
            DataAccess da = new DataAccess();
            foreach (DataRow row in p_tbl.Rows)
            {
                string sql = row.Str("cmd");
                Dict sqlDt = new Dict();

                // 按参数位置顺序整理查询参数或替换宏
                Regex reg = new Regex(@":[^\s,]+");
                MatchCollection matches = reg.Matches(row.Str("cmd"));
                foreach (Match match in matches)
                {
                    string name = match.Value.Substring(1);
                    if (p_params != null && p_params.ContainsKey(name))
                    {
                        object val = p_params[name];

                        // 名称比较忽略大小写
                        if (p_macro != null
                            && p_macro.Exists((str) => !string.IsNullOrEmpty(str) && str.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        {
                            // 替换宏
                            string str = string.Empty;
                            if (val != null)
                                str = val.ToString();
                            sql = sql.Replace(match.Value, str);
                        }
                        else
                        {
                            sqlDt[name] = val;
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("查询报表数据集时未提供参数【{0]】的值！", name));
                    }
                }
                dt[row.Str("id")] = await da.QuerySql(sql, sqlDt);
            }
            return dt;
        }

        /// <summary>
        /// 获取服务默认数据库的所有表名
        /// </summary>
        /// <returns></returns>
        public Task<DataTable> GetAllTables()
        {
            return new DataAccess().QuerySql($"select lower(table_name) name from information_schema.tables where table_schema='{DataAccess.Database}' and table_type='base table' order by name");
        }

        /// <summary>
        /// 获取指定表的列结构信息
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public DataTable GetTableSchema(string p_tblName)
        {
            TableSchema ts;
            IReadOnlyDictionary<string, TableSchema> schemas = DataAccess.GetSchema();
            if (!schemas.TryGetValue(p_tblName.ToLower(), out ts))
                throw new Exception(string.Format("未找到表{0}的结构信息！", p_tblName));

            DataTable tbl = new DataTable();
            tbl.Columns.Add("ColName", typeof(string));
            tbl.Columns.Add("DbType", typeof(string));
            tbl.Columns.Add("MaxLength", typeof(int));
            tbl.Columns.Add("Nullable", typeof(string));
            tbl.Columns.Add("Title", typeof(string));

            // 主键
            foreach (TableCol col in ts.PrimaryKey)
            {
                DataRow row = tbl.NewRow();
                row["ColName"] = col.ParamName.ToLower();
                row["DbType"] = col.DbType.ToString();
                row["MaxLength"] = col.DbType.ToLower() == "varchar" ? col.Length : 0;
                row["Nullable"] = col.Nullable ? "YES" : "NO";
                row["Title"] = col.Comments;
                tbl.Rows.Add(row);
            }

            // 非主键
            foreach (TableCol col in ts.Columns)
            {
                DataRow row = tbl.NewRow();
                row["ColName"] = col.ParamName.ToLower();
                row["DbType"] = col.DbType.ToString();
                row["MaxLength"] = col.DbType.ToLower() == "varchar" ? col.Length : 0;
                row["Nullable"] = col.Nullable ? "YES" : "NO";
                row["Title"] = col.Comments;
                tbl.Rows.Add(row);
            }

            return tbl;
        }

        /// <summary>
        /// 生成包装类
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public string GetClsByTblName(string p_tblName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            DataTable dt = GetTableSchema(p_tblName);
            if (dt != null)
            {
                string clsName = SetFirstToUpper(p_tblName);
                AppendTabSpace(sb, 1);
                sb.AppendFormat("public class {0} : DataEntity", clsName);
                sb.AppendLine();
                AppendTabSpace(sb, 1);
                sb.AppendLine("{");

                AppendTabSpace(sb, 2);
                sb.AppendLine("/// <summary>");
                AppendTabSpace(sb, 2);
                sb.AppendLine("/// 构造方法");
                AppendTabSpace(sb, 2);
                sb.AppendLine("/// </summary>");
                AppendTabSpace(sb, 2);
                sb.AppendFormat("public {0}()", clsName);
                sb.AppendLine();
                AppendTabSpace(sb, 2);
                sb.AppendLine("{ }");
                sb.AppendLine();

                AppendTabSpace(sb, 2);
                sb.AppendLine("/// <summary>");
                AppendTabSpace(sb, 2);
                sb.AppendLine("/// 构造方法");
                AppendTabSpace(sb, 2);
                sb.AppendLine("/// </summary>");
                AppendTabSpace(sb, 2);
                sb.AppendLine("/// <param name=\"p_row\">数据行</param>");
                AppendTabSpace(sb, 2);
                sb.AppendFormat("public {0}(DataRow p_row)", clsName);
                sb.AppendLine();
                AppendTabSpace(sb, 3);
                sb.AppendLine(": base(p_row)");
                AppendTabSpace(sb, 2);
                sb.AppendLine("{ }");

                foreach (DataRow row in dt.Rows)
                {
                    sb.AppendLine();
                    AppendTabSpace(sb, 2);
                    sb.AppendLine("/// <summary>");
                    AppendTabSpace(sb, 2);
                    sb.AppendLine("/// 获取设置" + row.Str("title"));
                    AppendTabSpace(sb, 2);
                    sb.AppendLine("/// </summary>");
                    AppendTabSpace(sb, 2);
                    string rowVal = "Str";
                    sb.AppendFormat("public {0} {1}", GetTypeStr(row.Str("dbtype"), out rowVal), SetFirstToUpper(row.Str("colname")));
                    sb.AppendLine();
                    AppendTabSpace(sb, 2);
                    sb.AppendLine("{");
                    AppendTabSpace(sb, 3);
                    sb.AppendFormat("get {1} return _row.{3}(\"{0}\"); {2}", row.Str("colName").ToLower(), "{", "}", rowVal);
                    sb.AppendLine();
                    AppendTabSpace(sb, 3);
                    sb.AppendFormat("set {1} _row[\"*{0}\"] = value; {2}", row.Str("colName").ToLower(), "{", "}");
                    sb.AppendLine();
                    AppendTabSpace(sb, 2);
                    sb.AppendLine("}");
                }
                sb.AppendLine();
                AppendTabSpace(sb, 2);
                sb.AppendLine("protected override string GetTblName()");
                AppendTabSpace(sb, 2);
                sb.AppendLine("{");
                AppendTabSpace(sb, 3);
                sb.AppendLine(string.Format("return \"{0}\";", p_tblName.ToLower()));
                AppendTabSpace(sb, 2);
                sb.AppendLine("}");

                AppendTabSpace(sb, 1);
                sb.AppendLine("}");
            }
            return sb.ToString();
        }
        */

        #region 内部方法
        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="p_str"></param>
        /// <returns></returns>
        static string SetFirstToUpper(string p_str)
        {
            string str = p_str.ToLower();
            if (str.Length > 1)
                return str.Substring(0, 1).ToUpper() + str.Substring(1);
            else
                return str.ToUpper();
        }

        /// <summary>
        /// 返回类型字符串
        /// </summary>
        /// <param name="p_dbTypeName"></param>
        /// <param name="p_val"></param>
        /// <returns></returns>
        static string GetTypeStr(string p_dbTypeName, out string p_val)
        {
            // 未处理地理信息类型的字段类型，由于bigint占用8个字节，对应c#中的long
            int index = p_dbTypeName.IndexOf("(");
            if (index > 0)
                p_dbTypeName = p_dbTypeName.Substring(0, index);

            switch (p_dbTypeName.ToLower())
            {
                case "char":
                case "enum":
                case "set":
                case "varchar":
                case "text":
                case "tinytext":
                case "mediumtext":
                case "longtext":
                    p_val = "Str";
                    return "string";
                case "int":
                case "smallint":
                case "integer":
                case "tinyint":
                case "year":
                case "mediumint":
                    p_val = "Int";
                    return "int";
                case "double":
                case "numeric":
                case "decimal":
                case "float":
                case "real":
                    p_val = "Double";
                    return "double";
                case "datetime":
                case "date":
                case "time":
                case "timestamp":
                    p_val = "Date";
                    return "DateTime";
                case "bit":
                case "blob":
                case "binary":
                case "varbinary":
                case "mediumblob":
                case "longblob":
                case " tinyblob":
                    p_val = "Str";
                    return "byte[]";
                case "bigint":
                    p_val = "Long";
                    return "long";
            }
            p_val = "Str";
            return "object";
        }

        static void AppendTabSpace(StringBuilder p_sb, int p_num)
        {
            for (int i = 0; i < p_num; i++)
            {
                p_sb.Append("    ");
            }
        }
        #endregion
    }
}
