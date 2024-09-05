#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 数据导出到sqlite文件及sqlite文件下载的处理
    /// </summary>
    public class SqliteFileHandler
    {
        #region 成员变量
        static DirectoryInfo _path = new DirectoryInfo(System.IO.Path.Combine(AppContext.BaseDirectory, "etc/sqlite"));

        readonly ModelFileItem _model = new ModelFileItem();
        readonly Dictionary<string, SqliteFileItem> _fileItem = new Dictionary<string, SqliteFileItem>(StringComparer.OrdinalIgnoreCase);
        #endregion

        /// <summary>
        /// sqlite文件路径
        /// </summary>
        public static DirectoryInfo Path => _path;

        /// <summary>
        /// 初始化 sqlite.json 配置
        /// </summary>
        public void Init(IDictionary<string, RequestDelegate> p_handlers)
        {
            if (!File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, "etc/config/sqlite.json")))
            {
                string err = "缺少sqlite.json文件，无法正常启动";
                Log.Fatal(err);
                throw new Exception(err);
            }

            if (!_path.Exists)
                _path.Create();

            // 加载 sqlite.json 配置，需要将数据导出到sqlite文件的列表
            var cfg = new ConfigurationBuilder()
                .SetBasePath(System.IO.Path.Combine(AppContext.BaseDirectory, "etc/config"))
                .AddJsonFile("sqlite.json", false, false)
                .Build();

            // db模型的sqlite文件
            _model.Init(cfg);

            // 普通sqlite文件
            foreach (var item in cfg.GetSection("Files").GetChildren())
            {
                _fileItem[item.Key] = new SqliteFileItem(cfg, item);
            }

            // 注册请求路径处理
            p_handlers["/.sqlite"] = GetData;

            UpdateVersion();
        }

        /// <summary>
        /// 刷新所有sqlite文件
        /// </summary>
        /// <returns></returns>
        public string RefreshAll()
        {
            string start = "";

            if (!_model.IsRefreshing)
            {
                start = "model";
            }

            foreach (var item in _fileItem)
            {
                if (!item.Value.IsRefreshing)
                {
                    start += " " + item.Key;
                }
            }

            if (start != "")
            {
                start = $"开始更新{start}文件";

                Task.Run(async () =>
                {
                    if (!_model.IsRefreshing)
                        await _model.Refresh();

                    foreach (var item in _fileItem)
                    {
                        if (!item.Value.IsRefreshing)
                        {
                            await item.Value.Refresh();
                        }
                    }
                    UpdateVersion();
                });
            }
            else
            {
                start = "所有sqlite文件更新中，无需重复提交更新请求！";
            }

            return start;
        }

        /// <summary>
        /// 刷新sqlite文件
        /// </summary>
        /// <returns></returns>
        public string Refresh(string p_fileName)
        {
            string msg;
            if (string.IsNullOrEmpty(p_fileName))
            {
                msg = "待更新的文件名不可为空！";
            }
            else if ("model".Equals(p_fileName, StringComparison.OrdinalIgnoreCase))
            {
                if (_model.IsRefreshing)
                {
                    msg = "model文件更新中，无需重复提交更新请求！";
                }
                else
                {
                    msg = "开始更新model文件...";
                    Task.Run(async () =>
                    {
                        await _model.Refresh();
                        UpdateVersion();
                    });
                }
            }
            else if (_fileItem.TryGetValue(p_fileName, out var item))
            {
                if (item.IsRefreshing)
                {
                    msg = p_fileName + "文件更新中，无需重复提交更新请求！";
                }
                else
                {
                    msg = $"开始更新{p_fileName}文件...";
                    Task.Run(async () =>
                    {
                        await item.Refresh();
                        UpdateVersion();
                    });
                }
            }
            else
            {
                msg = "开始更新model文件...";
            }
            return msg;
        }

        /// <summary>
        /// 获取所有sqlite文件名
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllFile()
        {
            var ls = _fileItem.Keys.ToList();
            ls.Insert(0, "model");
            return ls;
        }

        /// <summary>
        /// 向客户端返回sqlite文件内容
        /// </summary>
        /// <param name="p_context"></param>
        /// <returns></returns>
        Task GetData(HttpContext p_context)
        {
            p_context.Response.ContentType = "application/dt";

            // 形如 /.sqlite/model
            string path = p_context.Request.Path.Value;
            int index = path.LastIndexOf('/');

            if (index < 1)
            {
                // 未传递文件名参数，无文件
                p_context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }

            byte[] data = null;
            path = path.Substring(index + 1);
            if ("model".Equals(path, StringComparison.OrdinalIgnoreCase))
            {
                data = _model.GetData();
            }
            else if (_fileItem.TryGetValue(path, out var fileItem))
            {
                data = fileItem.GetData();
            }

            if (data != null)
            {
                return p_context.Response.Body.WriteAsync(data, 0, data.Length);
            }

            // 无文件
            p_context.Response.StatusCode = 404;
            return Task.CompletedTask;
        }

        void UpdateVersion()
        {
            bool isValid = true;

            if (_model.Version == null)
            {
                isValid = false;
            }
            else
            {
                foreach (var item in _fileItem)
                {
                    if (item.Value.Version == null)
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            if (!isValid)
            {
                // 缺少sqlite文件
                SysKernel.Config.Remove("SqliteVer");
                return;
            }

            List<string> ls = new List<string>();
            ls.Add(_model.Version);
            foreach (var item in _fileItem)
            {
                ls.Add(item.Value.Version);
            }
            SysKernel.Config["SqliteVer"] = ls;
        }
    }
}
