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
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Core
{
    static class Serilogger
    {
        public static void Init()
        {
            var setting = Stub.Inst.SvcProvider.GetRequiredService<ILogSetting>();
            try
            {
                var cfg = new LoggerConfiguration();

                // 设置最小输出级别，默认Information
                cfg.MinimumLevel.Is(setting.LogLevel);
                
                if (setting.TraceEnabled)
                    cfg.WriteTo.Sink(new TraceSink());

                if (setting.ConsoleEnabled)
                    cfg.WriteTo.Sink(new ConsoleSink());

                if (setting.FileEnabled)
                    AddFileLogging(cfg);

                if (setting.TraceEnabled || setting.ConsoleEnabled || setting.FileEnabled)
                    Log.Logger = cfg.CreateLogger();
            }
            catch (Exception e)
            {
                Console.WriteLine($"创建日志对象失败：{e.Message}");
                throw;
            }
        }

        static void AddFileLogging(LoggerConfiguration p_cfg)
        {
            string fileName;
#if ANDROID
            fileName = "android-.log";
#elif IOS
            fileName = "iOS-.log";
#elif WASM
            fileName = "wasm-.log";
#else
            fileName = "win-.log";
#endif

            p_cfg.WriteTo.File(
                    new CompactJsonFormatter(),
                    Path.Combine(ApplicationData.Current.LocalFolder.Path, ".data", fileName),
                    rollingInterval: RollingInterval.Day, // 文件名末尾加日期
                    fileSizeLimitBytes: 10485760);        // 10mb
        }
    }
}