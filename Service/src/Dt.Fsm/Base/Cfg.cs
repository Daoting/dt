#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Serilog;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Fsm
{
    /// <summary>
    /// 
    /// </summary>
    public static class Cfg
    {
        const string _defaultVol = "v0";
        public const string VolumeKey = "volume";

        /// <summary>
        /// 文件服务的根目录
        /// </summary>
        public static string Root { get; private set; }

        /// <summary>
        /// 所有卷列表
        /// </summary>
        public static List<string> Volumes { get; } = new List<string>();

        /// <summary>
        /// 初始化文件服务
        /// </summary>
        public static void Init()
        {
            Root = Path.Combine(Directory.GetCurrentDirectory(), "etc", "drive");
            var dir = new DirectoryInfo(Root);
            if (!dir.Exists)
                dir.Create();

            SortedSetCache cache = new SortedSetCache(VolumeKey);
            var subs = dir.GetDirectories();
            if (subs == null || subs.Length == 0)
            {
                Volumes.Add(_defaultVol);
                Directory.CreateDirectory(Path.Combine(Root, _defaultVol));
                cache.Increment(_defaultVol, 0).Wait();
                Log.Warning("无文件存储卷，创建默认卷 " + _defaultVol);
            }
            else
            {
                string vol = "";
                foreach (var sub in subs)
                {
                    Volumes.Add(sub.Name);
                    vol += sub.Name + " ";
                    // 加入缓存
                    cache.Increment(sub.Name, 0).Wait();
                }
                Log.Information("文件存储卷 " + vol);
            }
        }
    }
}
