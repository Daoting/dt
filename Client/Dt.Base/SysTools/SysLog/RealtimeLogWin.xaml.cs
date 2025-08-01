﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Windows.Storage;
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

            if (!Log.IsEnabled(LogEventLevel.Debug))
            {
                Serilogger.ApplySetting(new LogSetting
                {
                    MinimumLevel = LogEventLevel.Debug,
                    ConsoleEnabled = true,
                    FileEnabled = true,
                    TraceEnabled = true,
                    FileLogLevel = LogEventLevel.Information,
                    ConsoleLogLevel = LogEventLevel.Debug,
                    TraceLogLevel = LogEventLevel.Debug,
                });
                Log.Debug("已将最小日志级别自动调整为Debug");
            }
            _menu["Sqlite"].IsChecked = SqliteAccess.TraceLog;
        }

        void OnOutputClick(ItemClickArgs e)
        {
            var item = e.Data.To<TraceLogItem>();
            var d = new TraceLogData
            {
                Info = item.Info,
                Msg = item.Msg,
            };

            _fm.Update(d);
            NaviTo("日志内容");
        }

        void OnSqlite(Mi e)
        {
            SqliteAccess.TraceLog = !SqliteAccess.TraceLog;
            e.IsChecked = SqliteAccess.TraceLog;
        }

        void OnClear(Mi e)
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
        public static void FormatItem(Env e)
        {
            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = new GridLength(60) },
                },
            };

            var tbInfo = new TextBlock();
            grid.Children.Add(tbInfo);

            var tbMsg = new TextBlock { Style = Res.LvTextBlock, Margin = new Thickness(0, 6, 0, 6), };
            Grid.SetRow(tbMsg, 1);
            grid.Children.Add(tbMsg);
            e.UI = grid;

            e.Set += c =>
            {
                if (c.Data is not TraceLogItem r)
                    return;

                tbInfo.Text = r.Info;
                if (r.Log != null)
                {
                    switch (r.Log.Level)
                    {
                        case LogEventLevel.Debug:
                            tbInfo.Foreground = Res.深灰1;
                            break;

                        case LogEventLevel.Warning:
                            tbInfo.Foreground = Res.深黄;
                            break;

                        case LogEventLevel.Error:
                            tbInfo.Foreground = Res.RedBrush;
                            break;

                        case LogEventLevel.Fatal:
                            tbInfo.Foreground = Res.亮红;
                            break;

                        default:
                            tbInfo.Foreground = Res.BlackBrush;
                            break;
                    }
                }
                else
                {
                    tbInfo.Foreground = Res.BlackBrush;
                }
                tbMsg.Text = r.Msg;
            };
        }
    }
}
