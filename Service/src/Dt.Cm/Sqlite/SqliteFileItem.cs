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
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// Sqlite文件项
    /// </summary>
    class SqliteFileItem
    {
        byte[] _data;

        public SqliteFileItem(IConfigurationRoot p_cfg, IConfigurationSection p_item)
        {

        }

        /// <summary>
        /// 是否正在刷新中
        /// </summary>
        public bool IsRefreshing { get; set; }

        /// <summary>
        /// 获取设置模型文件版本号
        /// </summary>
        public string Version { get; internal set; }

        public byte[] GetData()
        {
            if (_data == null)
            {
                LoadFile();
            }
            return _data;
        }

        public Task Refresh()
        {
            return Task.CompletedTask;
        }

        void LoadFile()
        {
            string gzFile = Path.Combine(SqliteFileHandler.Path.FullName, Version);
            using (FileStream fs = new FileStream(gzFile, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    _data = new byte[fs.Length];
                    reader.Read(_data, 0, (int)fs.Length);
                }
            }
        }
    }
}
