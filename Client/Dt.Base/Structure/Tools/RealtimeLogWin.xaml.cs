#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Serilog.Events;
#endregion

namespace Dt.Base.Tools
{
    /// <summary>
    /// 实时日志
    /// </summary>
    public sealed partial class RealtimeLogWin : Win
    {
        public RealtimeLogWin()
        {
            InitializeComponent();

            // 数据源是静态的，即使关闭仍做绑定处理
            _lv.Loaded += (s, e) => _lv.Data = TraceLogs.Data;
            Closed += (s, e) => _lv.Data = null;

            _menu["Rpc"].SetBinding(Mi.IsCheckedProperty, new Binding { Path = new PropertyPath("ShowRpcLog"), Source = TraceLogs.Filter, Mode = BindingMode.TwoWay });
            _menu["Sqlite"].SetBinding(Mi.IsCheckedProperty, new Binding { Path = new PropertyPath("ShowSqliteLog"), Source = TraceLogs.Filter, Mode = BindingMode.TwoWay });
        }

        void OnOutputClick(object sender, ItemClickArgs e)
        {
            var item = e.Data.To<TraceLogItem>();
            var d = new TraceLogData
            {
                Time = item.Time,
                Level = item.Log.Level.ToString(),
                Detial = item.Detial,
            };

            if (item.Log.Properties.TryGetValue("SourceContext", out var val))
            {
                d.Source = val.ToString("l", null);
            }
            else
            {
                d.Source = "未知";
            }

            if (item.Log.Properties.TryGetValue("Title", out var vtitle)
                && vtitle.ToString("l", null) is string msg)
            {
                d.Title = msg;
            }

            _fm.Update(d);
        }

        void OnClear(object sender, Mi e)
        {
            TraceLogs.Clear();
        }

        public void ClearData()
        {
            _lv.Data = null;
        }
    }

    [LvCall]
    public class LogCellUI
    {
        public static void TitleBrush(Env e)
        {
            e.Set += c =>
            {
                var item = (TraceLogItem)c.Data;
                if (item.Log.Level == LogEventLevel.Error || item.Log.Level == LogEventLevel.Fatal)
                {
                    e.Dot.Foreground = Res.WhiteBrush;
                    e.Dot.Background = Res.RedBrush;
                }
                else if (item.Log.Level == LogEventLevel.Warning)
                {
                    e.Dot.Foreground = Res.WhiteBrush;
                    e.Dot.Background = Res.深黄;
                }
                else
                {
                    e.Dot.Foreground = Res.深灰1;
                    e.Dot.Background = Res.WhiteBrush;
                }
            };
        }
    }
}
