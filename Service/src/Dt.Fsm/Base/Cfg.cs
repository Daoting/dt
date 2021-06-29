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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#endregion

namespace Dt.Fsm
{
    /// <summary>
    /// 配置相关
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
        /// 普通卷列表
        /// </summary>
        public static List<string> Volumes { get; } = new List<string>();

        /// <summary>
        /// 固定卷表
        /// </summary>
        public static List<string> FixedVolumes { get; } = new List<string>();

        /// <summary>
        /// 初始化文件服务
        /// </summary>
        public static void Init()
        {
            // BaseDirectory程序集所在的目录，不可用Directory.GetCurrentDirectory()！
            Root = Path.Combine(AppContext.BaseDirectory, "drive");
            var dir = new DirectoryInfo(Root);
            if (!dir.Exists)
                dir.Create();

            // 固定卷
            var fv = Kit.GetCfg<string>("FixedVolume");
            StringBuilder sb = new StringBuilder("初始化固定卷");
            if (!string.IsNullOrEmpty(fv))
            {
                var vols = fv.Split(';');
                foreach (var vol in vols)
                {
                    var v = vol.Trim().ToLower();
                    if (v != string.Empty)
                    {
                        string path = Path.Combine(Root, v);
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        FixedVolumes.Add(v);
                        sb.Append(" ");
                        sb.Append(v);
                    }
                }
            }
            sb.Append("，普通卷");

            // 普通卷
            SortedSetCache cache = new SortedSetCache(VolumeKey);
            var subs = dir.GetDirectories();
            if (subs != null && subs.Length > 0)
            {
                foreach (var sub in subs)
                {
                    // 非固定卷
                    string v = sub.Name.ToLower();
                    if (!FixedVolumes.Contains(v))
                    {
                        Volumes.Add(sub.Name);
                        // 加入缓存
                        cache.Increment(v, 0).Wait();
                        sb.Append(" ");
                        sb.Append(v);
                    }
                }
            }
            // 无普通卷时创建默认卷v0
            if (Volumes.Count == 0)
            {
                Volumes.Add(_defaultVol);
                Directory.CreateDirectory(Path.Combine(Root, _defaultVol));
                cache.Increment(_defaultVol, 0).Wait();
                sb.Append(" ");
                sb.Append(_defaultVol);
            }

            Log.Information(sb.ToString());
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
