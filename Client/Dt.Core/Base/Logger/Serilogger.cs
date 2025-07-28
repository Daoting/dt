#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Formatting.Compact;
using Windows.Storage;
#endregion

namespace Dt.Core
{
    internal static class Serilogger
    {
        public static void Init()
        {
            ApplySetting(GlobalConfig.LogSetting);
            Kit.Trace("启动日志");
        }

        public static void ApplySetting(LogSetting setting)
        {
            try
            {
                var cfg = new LoggerConfiguration();

                // 设置最小输出级别，默认Information
                cfg.MinimumLevel.Is(setting.MinimumLevel);

                if (setting.TraceEnabled)
                    cfg.WriteTo.Sink(new TraceSink(), restrictedToMinimumLevel: setting.TraceLogLevel);

                if (setting.ConsoleEnabled)
                    cfg.WriteTo.Sink(new ConsoleSink(), restrictedToMinimumLevel: setting.ConsoleLogLevel);

                if (setting.FileEnabled)
                {
                    cfg.WriteTo.File(
                        new CompactJsonFormatter(),
                        Path.Combine(ApplicationData.Current.LocalFolder.Path, ".log", "dt-.log"),
                        rollingInterval: RollingInterval.Day, // 文件名末尾加日期
                        restrictedToMinimumLevel: setting.FileLogLevel, // 设置最小输出级别，默认Information
                        rollOnFileSizeLimit: true); // 超过1G时新文件名末尾加序号
                }

                if (setting.TraceEnabled || setting.ConsoleEnabled || setting.FileEnabled)
                    Log.Logger = cfg.CreateLogger();
            }
            catch (Exception e)
            {
                Console.WriteLine($"创建日志对象失败：{e.Message}");
                throw;
            }
        }
    }
}