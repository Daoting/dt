#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-12-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using System.Reflection;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 为 CList CPick 提供数据源描述信息，方便在xaml中设置
    /// </summary>
    [ContentProperty(Name = nameof(SqlStr))]
    public partial class Sql : DependencyObject
    {
        #region 静态内容
        public readonly static DependencyProperty SqlStrProperty = DependencyProperty.Register(
            "SqlStr",
            typeof(string),
            typeof(Sql),
            new PropertyMetadata(null));

        public readonly static DependencyProperty LocalDbProperty = DependencyProperty.Register(
            "LocalDb",
            typeof(string),
            typeof(Sql),
            new PropertyMetadata(null));

        public readonly static DependencyProperty SvcProperty = DependencyProperty.Register(
            "Svc",
            typeof(string),
            typeof(Sql),
            new PropertyMetadata(null));
        #endregion

        /// <summary>
        /// select语句，可包含变量或占位符，变量以@开头，占位符首尾添加#，它们内容格式相同，分两类：
        /// <para>1. 内部表达式取值：</para>
        /// <para>   userid：当前登录ID</para>
        /// <para>   username：当前登录名</para>
        /// <para>   input：CPick中输入的过滤串</para>
        /// <para>   [列名]：当前Fv数据源的列值</para>
        /// 
        /// <para>2. 调用外部方法取值： 外部类名.方法(参数)，如RptValueCall.GetMaxID(crud_父表)</para>
        /// <para>
        /// SELECT
        /// 	大儿名
        /// FROM
        /// 	crud_大儿
        /// WHERE
        /// 	parent_id = @[parentid]
        ///     AND name LIKE '#input#%'
        ///     AND id = @RptValueCall.GetMaxID(crud_大儿)
        ///     AND owner = @userid
        /// </para>
        /// </summary>
        public string SqlStr
        {
            get { return (string)GetValue(SqlStrProperty); }
            set { SetValue(SqlStrProperty, value); }
        }

        /// <summary>
        /// 本地sqlite库名
        /// </summary>
        public string LocalDb
        {
            get { return (string)GetValue(LocalDbProperty); }
            set { SetValue(LocalDbProperty, value); }
        }

        /// <summary>
        /// 服务名
        /// </summary>
        public string Svc
        {
            get { return (string)GetValue(SvcProperty); }
            set { SetValue(SvcProperty, value); }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="p_row">当前Fv的数据源</param>
        /// <param name="p_input">输入串</param>
        /// <returns></returns>
        internal async Task<Table> GetData(object p_row, string p_input)
        {
            var sql = SqlStr.Trim();

            // 计算sql变量值，整理参数字典
            Dict dt = null;
            var matches = Regex.Matches(SqlStr.Trim(), @"[^\s,]+[\s]*=[\s]*@[^\s,]+");
            foreach (Match match in matches)
            {
                var arr = match.Value.Split('=');
                string key;
                string exp;
                if (arr.Length == 2
                    && (key = arr[0].Trim()) != ""
                    && (exp = arr[1].Trim()) != ""
                    && exp.StartsWith('@'))
                {
                    if (dt == null)
                        dt = new Dict();

                    var str = exp.Substring(1);
                    if (exp.IndexOf('.') < 0)
                    {
                        // 无.时为内部方法
                        dt[key] = GetExpValue(str, p_row, p_input);
                    }
                    else
                    {
                        dt[key] = await ValueCall.GetValue(str);
                    }

                    sql = sql.Replace(exp, "@" + key);
                    continue;
                }

                Throw.Msg("Sql参数格式错误：" + match.Value);
            }

            // 替换占位符的值
            matches = Regex.Matches(SqlStr.Trim(), @"#[^\s#,]+#");
            foreach (Match match in matches)
            {
                var exp = match.Value;
                object val = null;
                if (exp.IndexOf('.') < 0)
                {
                    // 无.时为内部方法
                    val = GetExpValue(exp.Trim('#'), p_row, p_input);
                }
                else
                {
                    val = await ValueCall.GetValue(exp.Trim('#'));
                }

                sql = sql.Replace(exp, val == null ? "" : val.ToString());
            }

            Table tbl;
            if (!string.IsNullOrEmpty(LocalDb))
            {
                // 本地库
                var da = new AgentInfo(AccessType.Local, LocalDb).GetAccessInfo().GetDa();
                tbl = await da.Query(sql, dt);
            }
            else if (Svc == null)
            {
                // 当前服务
                tbl = await At.Query(sql, dt);
            }
            else
            {
                // 指定服务
                tbl = await Kit.Rpc<Table>(Svc, "Da.Query", sql, dt);
            }
            return tbl;
        }

        static object GetExpValue(string p_exp, object p_data, string p_input)
        {
            if (p_exp == "")
                return null;

            var exp = p_exp.ToLower();
            if (exp.StartsWith('[') && exp.EndsWith("]"))
            {
                PropertyInfo pi;
                var id = exp.Substring(1, exp.Length - 2);
                if (p_data is Row row)
                {
                    if (row.Contains(id))
                        return row[id];
                }
                else if ((pi = p_data.GetType().GetProperty(id, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)) != null)
                {
                    return pi.GetValue(p_data);
                }
            }
            else
            {
                switch (exp)
                {
                    case "userid":
                        return Kit.UserID;
                    case "username":
                        return Kit.UserName;
                    case "input":
                        return p_input;
                }
            }
            return null;
        }
    }
}