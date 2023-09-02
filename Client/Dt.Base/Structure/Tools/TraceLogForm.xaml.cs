#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-06-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Base.Tools
{
    public sealed partial class TraceLogForm : Tab
    {
        TraceLogData _data;

        #region 构造方法
        public TraceLogForm()
        {
            InitializeComponent();
        }
        #endregion

        public void Update(TraceLogData p_data)
        {
            _data = p_data;
            _tbInfo.Text = _data.Info;
            _tb.Text = _data.Msg;

            int index = _data.Info.IndexOf(']');
            if (index > 3)
            {
                var lev = _data.Info.Substring(index - 3, 3);
                if (lev == "DBG")
                {
                    _tbInfo.Foreground = Res.深灰1;
                }
                else if (lev == "WRN")
                {
                    _tbInfo.Foreground = Res.深黄;
                }
                else if (lev == "ERR")
                {
                    _tbInfo.Foreground = Res.RedBrush;
                }
                else if (lev == "FAL")
                {
                    _tbInfo.Foreground = Res.亮红;
                }
                else
                {
                    _tbInfo.Foreground = Res.BlackBrush;
                }
            }
        }

        void CopySql(object sender, RoutedEventArgs e)
        {
            if (_data == null || string.IsNullOrEmpty(_data.Msg))
                return;

            bool find = false;
            try
            {
                if (_data.Msg.StartsWith('[') && _data.Msg.EndsWith(']'))
                {
                    Utf8JsonReader reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(_data.Msg));
                    // [
                    reader.Read();
                    // SvcName
                    reader.Read();
                    // ApiName
                    reader.Read();
                    // 第一个参数
                    reader.Read();
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        string sql = reader.GetString().Trim();
                        if (sql.StartsWith("select", StringComparison.OrdinalIgnoreCase)
                            || sql.StartsWith("update", StringComparison.OrdinalIgnoreCase)
                            || sql.StartsWith("insert", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                reader.Read();
                                var dt = JsonRpcSerializer.Deserialize(ref reader, typeof(Dict)) as Dict;
                                sql = BuildSql(sql, dt);
                            }
                            catch { }
                            finally
                            {
                                if (!string.IsNullOrEmpty(sql))
                                {
                                    SysTrace.CopyToClipboard(sql);
                                    find = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    var det = _data.Msg.Trim();
                    if (det.StartsWith("select", StringComparison.OrdinalIgnoreCase)
                        || det.StartsWith("update", StringComparison.OrdinalIgnoreCase)
                        || det.StartsWith("insert", StringComparison.OrdinalIgnoreCase))
                    {
                        SysTrace.CopyToClipboard(det);
                        find = true;
                    }
                }
            }
            catch { }

            if (!find)
                Kit.Warn("无 sql 语句！");
        }

        /// <summary>
        /// 生成执行时候的sql语句。
        /// 注意：对于二进制编码数组的情况在实际应用时可能需要调整。
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        static string BuildSql(string p_sql, Dict p_params)
        {
            if (p_params == null)
                return p_sql;

            string str = p_sql;
            try
            {
                string prefix = str.Contains('@') ? "@" : ":";
                if (p_params is IDictionary<string, object> dt)
                {
                    // 键/值型对象
                    foreach (var item in dt)
                    {
                        str = ReplaceSql(str, prefix + item.Key, item.Value);
                    }
                }
            }
            catch { }
            return str;
        }

        /// <summary>
        /// 替换sql中的占位符
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_key">占位符</param>
        /// <param name="p_value"></param>
        /// <returns></returns>
        static string ReplaceSql(string p_sql, string p_key, object p_value)
        {
            string str = p_sql;
            int posStart = str.IndexOf(p_key, StringComparison.OrdinalIgnoreCase);
            while (posStart > -1)
            {
                string next, trueVal;
                int posEnd = posStart + p_key.Length;
                if (posEnd >= str.Length)
                    next = "";
                else
                    next = str.Substring(posEnd, 1);

                // 判断参数名的匹配是不是只匹配了一部分
                if (!string.IsNullOrEmpty(next) && _sqlPattern.IsMatch(next))
                {
                    // 匹配了一部分则继续向后查找
                    posStart = str.IndexOf(p_key, posEnd, StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                // mysql中非string类型外面加''也可正常运行！
                if (p_value == null)
                    trueVal = "null";
                else
                    trueVal = $"'{p_value}'";

                str = str.Substring(0, posStart) + trueVal + str.Substring(posStart + p_key.Length);
                posStart = str.IndexOf(p_key, posStart + trueVal.Length - 1, StringComparison.OrdinalIgnoreCase);
            }
            return str;
        }

        static readonly Regex _sqlPattern = new Regex("[0-9a-zA-Z_$]");
    }

    public class TraceLogData
    {
        /// <summary>
        /// 日志描述信息
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 日志详细内容
        /// </summary>
        public string Msg { get; set; }
    }
}
