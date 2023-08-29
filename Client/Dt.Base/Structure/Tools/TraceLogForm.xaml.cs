﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-06-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
            _fv.Data = _data;
            _tb.Text = _data.Detial;
        }

        void CopySql(object sender, RoutedEventArgs e)
        {
            if (_data == null || string.IsNullOrEmpty(_data.Detial))
                return;

            bool find = false;
            try
            {
                if (_data.Detial.StartsWith('[') && _data.Detial.EndsWith(']'))
                {
                    var elem = JsonSerializer.Deserialize<JsonElement>(_data.Detial);
                    if (elem.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var it in elem.EnumerateArray())
                        {
                            if (it.ValueKind == JsonValueKind.String)
                            {
                                var val = it.GetString().Trim();
                                if (val.StartsWith("select", StringComparison.OrdinalIgnoreCase)
                                    || val.StartsWith("update", StringComparison.OrdinalIgnoreCase)
                                    || val.StartsWith("insert", StringComparison.OrdinalIgnoreCase))
                                {
                                    SysTrace.CopyToClipboard(val);
                                    find = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    var det = _data.Detial.Trim();
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
        static string BuildSql(string p_sql, object p_params)
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
                else
                {
                    // 普通对象
                    Type type = p_params.GetType();
                    foreach (var prop in type.GetProperties())
                    {
                        str = ReplaceSql(str, prefix + prop.Name, prop.GetValue(p_params));
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

        void CopySource(object sender, RoutedEventArgs e)
        {
            if (_data != null && !string.IsNullOrEmpty(_data.Source))
                SysTrace.CopyToClipboard(_data.Source, true);
        }
    }

    public class TraceLogData
    {
        /// <summary>
        /// 时间及级别
        /// </summary>
        public string TimeLevel { get; set; }

        /// <summary>
        /// 日志源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 日志项的详细内容
        /// </summary>
        public string Detial { get; set; }
    }
}
