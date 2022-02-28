#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Events;
using Serilog.Formatting.Compact;
using Windows.Storage;
#endregion

namespace Dt.Core
{
    static class Serilogger
    {
        static LogSetting _setting;

        public static void Init(LogSetting p_setting)
        {
            _setting = p_setting == null ? new LogSetting() : p_setting;
            try
            {
                var cfg = new LoggerConfiguration();
                if (_setting.TraceEnabled)
                    AddTraceLogging(cfg);
                if (_setting.ConsoleEnabled)
                    AddConsoleLogging(cfg);
                if (_setting.FileEnabled)
                    AddFileLogging(cfg);
                Log.Logger = cfg.CreateLogger();
            }
            catch (Exception e)
            {
                Console.WriteLine($"创建日志对象失败：{e.Message}");
                throw;
            }
        }

        static void AddTraceLogging(LoggerConfiguration p_cfg)
        {

        }

        static void AddConsoleLogging(LoggerConfiguration p_cfg)
        {
#if ANDROID
            // 内部调用 Android.Util.Log 输出
                p_cfg.WriteTo.AndroidLog(
                    outputTemplate: "{Level:u1}/{SourceContext}: {Message:lj} {Exception}{NewLine}",
                    restrictedToMinimumLevel: _setting.LogLevel);
#elif IOS
                // 内部调用 Console.WriteLine 输出
                p_cfg.WriteTo.NSLog(
                    outputTemplate: "{Level:u1}/{SourceContext}: {Message:lj} {Exception}",
                    restrictedToMinimumLevel: _setting.LogLevel);
#else
            p_cfg.WriteTo.Console(
                    outputTemplate: "{Timestamp:MM-dd HH:mm:ss.fffzzz} {Level:u1}/{SourceContext}: {Message:lj} {Exception}{NewLine}",
                    restrictedToMinimumLevel: _setting.LogLevel);
#endif
            p_cfg.WriteTo.Debug(outputTemplate: "{Timestamp:MM-dd HH:mm:ss.fffzzz} {Level:u1}/{SourceContext}: {Message:lj} {Exception}{NewLine}");
            p_cfg.WriteTo.Debug();
        }

        static void AddFileLogging(LoggerConfiguration p_cfg)
        {
            p_cfg
                .WriteTo.File(
                    new CompactJsonFormatter(),
                    Path.Combine(ApplicationData.Current.LocalFolder.Path, ".data", "log.txt"),
                    rollingInterval: RollingInterval.Day, // 文件名末尾加日期
                    fileSizeLimitBytes: 10485760) // 10mb
#if ANDROID
                .Enrich.WithProperty("Platform", "Android");
#elif IOS
                .Enrich.WithProperty("Platform", "iOS");
#else
                .Enrich.WithProperty("Platform", "WASM");
#endif
        }
    }
}