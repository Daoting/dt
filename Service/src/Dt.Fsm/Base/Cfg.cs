#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
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

        /// <summary>
        /// 卷状态缓存键
        /// </summary>
        public const string VolumeKey = "volume";

        /// <summary>
        /// 缩略图后缀名
        /// </summary>
        public const string ThumbPostfix = "-t.jpg";

        static readonly List<string> _imgExt = new List<string> { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".ico", ".tif" };
        static readonly List<string> _videoExt = new List<string> { ".mp4", ".wmv" };

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

        /// <summary>
        /// 是否为Image
        /// </summary>
        /// <param name="p_ext">扩展名带 .</param>
        /// <returns></returns>
        public static bool IsImage(string p_ext)
        {
            return _imgExt.Contains(p_ext);
        }

        /// <summary>
        /// 是否为视频
        /// </summary>
        /// <param name="p_ext">扩展名带 .</param>
        /// <returns></returns>
        public static bool IsVideo(string p_ext)
        {
            return _videoExt.Contains(p_ext);
        }
    }
}
