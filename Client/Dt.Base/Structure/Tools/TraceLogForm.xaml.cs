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
using System.Text.Json;
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

        void CopySource(object sender, RoutedEventArgs e)
        {
            if (_data != null && !string.IsNullOrEmpty(_data.Source))
                SysTrace.CopyToClipboard(_data.Source, true);
        }
    }

    public class TraceLogData
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 日志源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Title属性
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 日志项的详细内容
        /// </summary>
        public string Detial { get; set; }
    }
}
