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
        static readonly Dictionary<string, DbAccessInfo> _dbs = new Dictionary<string, DbAccessInfo>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 系统标题
        /// </summary>
        public static string Title { get; set; }

        public static string Server { get; set; }

        public static string WasmServer { get; set; }

        public static Dictionary<string, DbAccessInfo> DbInfos => _dbs;

        public static ILogSetting LogSetting { get; } = CreateLogSetting();

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
        }

        static ILogSetting CreateLogSetting()
        {
#if DEBUG
            return new CfgLogSetting
            {
                MinimumLevel = LogEventLevel.Debug,
                ConsoleEnabled = true,
                FileEnabled = true,
                TraceEnabled = true,
                FileLogLevel = LogEventLevel.Information,
                ConsoleLogLevel = LogEventLevel.Debug,
                TraceLogLevel = LogEventLevel.Debug,
            };
#else
            return new CfgLogSetting
            {
                MinimumLevel = LogEventLevel.Debug,
                ConsoleEnabled = true,
                FileEnabled = true,
                TraceEnabled = true,
                FileLogLevel = LogEventLevel.Information,
                ConsoleLogLevel = LogEventLevel.Debug,
                TraceLogLevel = LogEventLevel.Debug,
            };
#endif
        }
        
        class CfgLogSetting : ILogSetting
        {
            public LogEventLevel MinimumLevel { get; set; }
            public bool ConsoleEnabled { get; set; }
            public bool FileEnabled { get; set; }
            public bool TraceEnabled { get; set; }
            public LogEventLevel FileLogLevel { get; set; }

            public LogEventLevel ConsoleLogLevel { get; set; }
            public LogEventLevel TraceLogLevel { get; set; }
        }
    }
}