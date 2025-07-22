#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Events;
using System.Text;
using System.Text.Json;
using Windows.Storage;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 全局配置
    /// </summary>
    internal class GlobalConfig
    {
        public static readonly Dictionary<string, DbAccessInfo> _dbs = new Dictionary<string, DbAccessInfo>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 系统标题
        /// </summary>
        public static string Title { get; set; }

        public static string Server { get; set; }

        public static string WasmServer { get; set; }

        public static LogSetting LogSetting { get; } = CreateLogSetting();

        public static async Task Load()
        {
            try
            {
                // 采用统一方式读取Config.json文件内容，wasm不支持Task.Wait()
                string config;
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Config.json"));
                using (var stream = await file.OpenStreamForReadAsync())
                using (var reader = new StreamReader(stream))
                {
                    config = reader.ReadToEnd();
                }

                var r = new Utf8JsonReader(Encoding.UTF8.GetBytes(config), new JsonReaderOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true, });
                // {
                r.Read();
                while (r.Read())
                {
                    if (r.TokenType != JsonTokenType.PropertyName)
                        break;

                    string key = r.GetString().ToLower();
                    if (key == "title")
                    {
                        Title = r.ReadAsString();
                        UITree.MainWin.Title = Title;
                    }
                    else if (key == "wasmserver")
                    {
                        WasmServer = r.ReadAsString();
                    }
                    else if (key == "server")
                    {
                        Server = r.ReadAsString();
                    }
                    else if (key == "database")
                    {
                        // {
                        r.Read();

                        while (r.Read() && r.TokenType != JsonTokenType.EndObject)
                        {
                            string dbName = r.GetString();
                            // {
                            r.Read();

                            string connStr = null;
                            DatabaseType? tp = null;
                            while (r.Read() && r.TokenType != JsonTokenType.EndObject)
                            {
                                var name = r.GetString().ToLower();
                                if (name == "connstr")
                                {
                                    connStr = r.ReadAsString();
                                }
                                else if (name == "dbtype")
                                {
                                    var dbType = r.ReadAsString().ToLower();
                                    if (dbType == "mysql")
                                        tp = DatabaseType.MySql;
                                    else if (dbType == "oracle")
                                        tp = DatabaseType.Oracle;
                                    else if (dbType == "sqlserver")
                                        tp = DatabaseType.SqlServer;
                                    else if (dbType == "postgresql")
                                        tp = DatabaseType.PostgreSql;
                                }
                            }
                            if (!string.IsNullOrEmpty(connStr) && tp != null)
                            {
                                _dbs[dbName] = new DbAccessInfo(dbName, connStr, tp.Value);
                            }
                        }
                    }
                    else if (key == "logsetting")
                    {
                        // {
                        r.Read();

                        while (r.Read() && r.TokenType != JsonTokenType.EndObject)
                        {
                            switch (r.GetString().ToLower())
                            {
                                case "minimumlevel":
                                    LogSetting.MinimumLevel = Enum.Parse<LogEventLevel>(r.ReadAsString());
                                    break;
                                case "consoleenabled":
                                    LogSetting.ConsoleEnabled = r.ReadAsBool();
                                    break;
                                case "consoleloglevel":
                                    LogSetting.ConsoleLogLevel = Enum.Parse<LogEventLevel>(r.ReadAsString());
                                    break;
                                case "fileenabled":
                                    LogSetting.FileEnabled = r.ReadAsBool();
                                    break;
                                case "fileloglevel":
                                    LogSetting.FileLogLevel = Enum.Parse<LogEventLevel>(r.ReadAsString());
                                    break;
                                case "traceenabled":
                                    LogSetting.TraceEnabled = r.ReadAsBool();
                                    break;
                                case "traceloglevel":
                                    LogSetting.TraceLogLevel = Enum.Parse<LogEventLevel>(r.ReadAsString());
                                    break;
                                default:
                                    r.Read();
                                    r.TrySkip();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        r.Read();
                        r.TrySkip();
                    }
                }
            }
            catch (Exception ex)
            {
                // throw时无提示信息
                throw new Exception("读取 Config.json 时出错！" + ex.Message);
            }
            Kit.Debug("加载全局配置");
        }

        static LogSetting CreateLogSetting()
        {
            // 默认日志设置
#if DEBUG
            return new LogSetting
            {
                MinimumLevel = LogEventLevel.Debug,
                ConsoleEnabled = true,
                ConsoleLogLevel = LogEventLevel.Debug,
                FileEnabled = true,
                FileLogLevel = LogEventLevel.Information,
                TraceEnabled = true,
                TraceLogLevel = LogEventLevel.Debug,
            };
#else
            return new LogSetting
            {
                MinimumLevel = LogEventLevel.Warning,
                ConsoleEnabled = false,
                ConsoleLogLevel = LogEventLevel.Debug,
                FileEnabled = true,
                FileLogLevel = LogEventLevel.Warning,
                TraceEnabled = false,
                TraceLogLevel = LogEventLevel.Debug,
            };
#endif
        }
    }
}