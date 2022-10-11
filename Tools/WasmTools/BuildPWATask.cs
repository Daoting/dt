using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WasmTools
{
    public partial class BuildPWATask : Microsoft.Build.Utilities.Task
    {
        string _pkgDir;

        public string OutDir { get; set; }

        public override bool Execute()
        {
            try
            {
                //Debugger.Launch();

                if (!IsValidate())
                    return false;

                // 删除无用的文件，如 *.pdb
                DelUnusedFiles();
                // 生成 service-worker.js 文件并压缩，动态创建要缓存的文件
                WriteServiceWorker();
                // 替换 uno-config.js 中的 config.offline_files
                ReplaceUnoConfig();

                return true;
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString(), false, true, null);
                return false;
            }
        }

        bool IsValidate()
        {
            if (string.IsNullOrEmpty(OutDir))
            {
                Log.LogError("OutDir不可为空！");
                return false;
            }

            if (!Directory.Exists(OutDir))
            {
                Log.LogError($"OutDir路径不存在：{OutDir}");
                return false;
            }

            OutDir = Path.GetFullPath(OutDir);
            var pkg = Directory.GetDirectories(OutDir, "package_*", SearchOption.TopDirectoryOnly);
            if (pkg == null || pkg.Length == 0)
            {
                Log.LogError($"未找到package_xxx路径！");
                return false;
            }

            _pkgDir = pkg[0];
            return true;
        }

        void DelUnusedFiles()
        {
            foreach (var file in Directory.EnumerateFiles(OutDir))
            {
                if (!file.EndsWith("\\index.html"))
                {
                    DelFile(file);
                }
            }

            foreach (var file in Directory.EnumerateFiles(Path.Combine(_pkgDir, "managed"), "*.pdb"))
            {
                DelFile(file);
            }

            // 不可删除，加载嵌入的资源时用到
            //DelFile(Path.Combine(_pkgDir, "uno-assets.txt"));
        }

        void WriteServiceWorker()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[\"./\", \"./service-worker.js\"");

            // .config文件默认被iis拒绝，无法缓存！
            var ls = from file in Directory.EnumerateFiles(_pkgDir, "*.*", SearchOption.AllDirectories)
                     where !file.EndsWith(".br", StringComparison.OrdinalIgnoreCase) && !file.EndsWith(".config", StringComparison.OrdinalIgnoreCase)
                     select file.Replace(OutDir, ".").Replace("\\", "/");
            foreach (var file in ls)
            {
                sb.Append(", \"");
                sb.Append(file);
                sb.Append("\"");
            }
            sb.Append("]");

            string js;
            using (Stream stream = typeof(BuildPWATask).Assembly.GetManifestResourceStream("Dt.WasmTools.Res.service-worker.js"))
            using (StreamReader sr = new StreamReader(stream))
            {
                js = sr.ReadToEnd();
            }

            var cacheKey = Path.GetFileName(_pkgDir);
            js = js.Replace("$(CACHE_KEY)", cacheKey).Replace("$(CACHE_FILES)", sb.ToString());
            var swFile = Path.Combine(OutDir, "service-worker.js");
            File.WriteAllText(swFile, js);
            BrotliCompress(swFile, swFile + ".br");
        }

        void ReplaceUnoConfig()
        {
            string path = Path.Combine(_pkgDir, "uno-config.js");
            if (!File.Exists(path))
                return;

            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("config.offline_files ="))
                {
                    lines[i] = "config.offline_files = ['./'];";
                    continue;
                }
            }
            File.WriteAllLines(path, lines);

            var br = path + ".br";
            if (File.Exists(br))
                File.Delete(br);
            BrotliCompress(path, br);
        }

        void BrotliCompress(string source, string destination)
        {
            using var input = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var output = File.Create(destination);
            using var bs = new BrotliSharpLib.BrotliStream(output, CompressionMode.Compress);

            // 原来引用包：BrotliSharpLib
            // By default, BrotliSharpLib uses a quality value of 1 and window size of 22 if the methods are not called.
            bs.SetQuality(7);
            /** bs.SetWindow(windowSize); **/
            /** bs.SetCustomDictionary(customDict); **/
            input.CopyTo(bs);

            /* IMPORTANT: Only use the destination stream after closing/disposing the BrotliStream
			   as the BrotliStream must be closed in order to signal that no more blocks are being written
			   for the final block to be flushed out 
			*/
            bs.Dispose();
        }

        void DelFile(string p_filePath)
        {
            try
            {
                if (File.Exists(p_filePath))
                {
                    File.Delete(p_filePath);
                }
            }
            catch { }
        }
    }
}
