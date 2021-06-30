#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace Dt.Core.Sqlite
{
    /// <summary>
    /// Sqlite模型文件处理
    /// </summary>
    public class SqliteModelHandler
    {
        internal const string Warning = "当前服务不支持模型文件！";
        bool _isInited;
        string _version;
        byte[] _data;

        /// <summary>
        /// 是否正在刷新中
        /// </summary>
        public static bool Refreshing { get; set; }

        /// <summary>
        /// 模型文件路径
        /// </summary>
        public static string ModelPath => System.IO.Path.Combine(AppContext.BaseDirectory, "etc/model");

        /// <summary>
        /// 初始化SQLite模型文件
        /// </summary>
        public void Init(IDictionary<string, RequestDelegate> p_handlers)
        {
            // 注册请求路径处理
            p_handlers["/.model"] = (p_context) =>
            {
                p_context.Response.ContentType = "application/dt";
                return p_context.Response.Body.WriteAsync(_data, 0, _data.Length);
            };

            DirectoryInfo dir = new DirectoryInfo(ModelPath);
            if (!dir.Exists)
                dir.Create();

            FileInfo fi = dir.EnumerateFiles("*.gz", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (fi != null)
            {
                _version = fi.Name.Substring(0, fi.Name.Length - 3);
                LoadModelFile();
                Log.Information("缓存模型文件成功");
            }
            else
            {
                Log.Error("模型文件不存在，请[更新模型]创建模型文件");
            }
            _isInited = true;
        }

        /// <summary>
        /// 获取模型文件版本号
        /// </summary>
        public string GetVersion()
        {
            Throw.If(!_isInited, Warning);
            return _version;
        }

        /// <summary>
        /// 刷新模型版本
        /// </summary>
        /// <returns></returns>
        public bool Refresh()
        {
            Throw.If(!_isInited, Warning);
            if (Refreshing)
                return false;

            // 远程事件通知刷新，服务可能存在多个副本！
            var ed = new ModelRefreshEvent { Version = Guid.NewGuid().ToString().Substring(0, 8) };
            Kit.RemoteMulticast(ed, Kit.SvcName);
            return true;
        }

        /// <summary>
        /// 读取模型文件到缓存
        /// </summary>
        internal void LoadModelFile()
        {
            string gzFile = Path.Combine(ModelPath, _version + ".gz");
            using (FileStream fs = new FileStream(gzFile, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    _data = new byte[fs.Length];
                    reader.Read(_data, 0, (int)fs.Length);
                }
            }
        }

        /// <summary>
        /// 刷新后重置版本
        /// </summary>
        /// <param name="p_ver"></param>
        internal void SetVersion(string p_ver)
        {
            _version = p_ver;
        }
    }

    /// <summary>
    /// 模型文件刷新事件参数
    /// </summary>
    public class ModelRefreshEvent : IEvent
    {
        /// <summary>
        /// 模型文件版本号
        /// </summary>
        public string Version { get; set; }
    }
}
