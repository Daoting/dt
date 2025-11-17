#if WASM
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-06-22 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Windows.Storage;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// wasm静态工具类
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 执行js内容，相当于eval
        /// </summary>
        /// <param name="p_js">js语句串</param>
        /// <returns>调用结果</returns>
        public static string InvokeJS(string p_js)
        {
            return Interop.InvokeJS(p_js);
        }

        /// <summary>
        /// js字符串转义
        /// </summary>
        /// <param name="p_str"></param>
        /// <returns></returns>
        public static string EscapeJs(string p_str)
        {
            if (p_str == null)
            {
                return "";
            }

            bool NeedsEscape(string s2)
            {
                for (int i = 0; i < s2.Length; i++)
                {
                    var c = s2[i];

                    if (
                        c > 255
                        || c < 32
                        || c == '\\'
                        || c == '"'
                        || c == '\r'
                        || c == '\n'
                        || c == '\t'
                    )
                    {
                        return true;
                    }
                }

                return false;
            }

            if (NeedsEscape(p_str))
            {
                var r = new StringBuilder(p_str.Length);

                foreach (var c in p_str)
                {
                    switch (c)
                    {
                        case '\\':
                            r.Append("\\\\");
                            continue;
                        case '"':
                            r.Append("\\\"");
                            continue;
                        case '\r':
                            continue;
                        case '\n':
                            r.Append("\\n");
                            continue;
                        case '\t':
                            r.Append("\\t");
                            continue;
                    }

                    if (c < 32)
                    {
                        continue; // not displayable
                    }

                    if (c <= 255)
                    {
                        r.Append(c);
                    }
                    else
                    {
                        r.Append("\\u");
                        r.Append(((ushort)c).ToString("X4"));
                    }
                }

                return r.ToString();
            }
            else
            {
                return p_str;
            }
        }

        /// <summary>
        /// 获取内容文件的web地址
        /// </summary>
        /// <param name="p_fileUrl">内容文件路径，如：ms-appx:///Assets/Html/editor.html</param>
        /// <returns></returns>
        public static async Task<string> GetLocalUrl(string p_fileUrl)
        {
            string path = null;
            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(p_fileUrl));
                int index = file.Path.IndexOf("/package_");
                if (index < 0)
                    throw new Exception("路径非法");
                path = file.Path.Substring(index);
            }
            catch {  return null; }

            // 获取浏览器中的地址
            var str = InvokeJS("location.href");
            int end = str.LastIndexOf("/index.html");
            if (end > 0)
                str = str.Substring(0, end);
            str = str.TrimEnd('/');
            
            return str + "/" + path;
        }
        
        // 支持单体服务后统一在Stub中设置
        ///// <summary>
        ///// 根据当前浏览器的url获取服务地址，如：
        ///// <para> https://localhost/fz/ui/ 的服务地址 https://localhost/fz </para>
        ///// <para>因开发调试时使用IIS Express无法通过url获得服务地址，外部直接p_debugUrl设置服务地址</para>
        ///// </summary>
        ///// <param name="p_debugUrl">开发调试时的服务地址</param>
        ///// <returns>形如："https://10.10.1.16/fz"</returns>
        //public static string GetServerUrl(string p_debugUrl)
        //{
        //    // 获取浏览器中的地址
        //    var str = InvokeJS("location.href");
        //    var match = Regex.Match(str, @"^https://[^\s/]+/[^\s/]+");
        //    if (match.Success)
        //        p_debugUrl = match.Value;

        //    Console.WriteLine($"服务器地址：{p_debugUrl}");
        //    return p_debugUrl;
        //}

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="p_url">文件完整路径</param>
        /// <param name="p_name">文件名称</param>
        public static void Download(string p_url, string p_name)
        {
            InvokeJS($"var a=document.createElement(\"a\");a.href=\"{p_url}\";a.target=\"_blank\";a.download=\"{p_name}\";a.click();");
        }
    }

    /// <summary>
    /// .NET7后的方式，它使用代码生成来创建高性能、符合 CSP 标准、线程安全的互作，并且不使用 eval
    /// 不采用 JSImport 方式，必须支持 Content Security Policy (CSP)，才能调用 eval
    /// </summary>
    internal static partial class Interop
    {
        /// <summary>
        /// uno-bootstrap.js 中的 invokeJS 函数，内部调用的 eval
        /// public static invokeJS(value: string) {
		///	return eval(value);
		///}
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [JSImport("globalThis.Uno.WebAssembly.Bootstrap.Bootstrapper.invokeJS")]
        public static partial string InvokeJS(string value);
    }
}

//internal sealed class Interop
//{
//    /// <summary>
//    /// .NET5 MonoVM and upward specific internal call.
//    /// 禁止混淆名称
//    /// </summary>
//    [Obfuscation(Feature = "renaming", Exclude = true)]
//    internal sealed class Runtime
//    {
//        /// <summary>
//        /// 升级.NET5后此方法名称由 WebAssembly.Runtime:InvokeJS 转 Interop.Runtime:InvokeJS
//        /// mono-wasm中通过mono_add_internal_call函数在启动时注册方法名称和具体逻辑的映射
//        /// c代码中注册的方法名称：Interop.Runtime:InvokeJS，所以命名空间和名称都不能修改！
//        /// c#调用js最终通过c的宏EM_ASM_INT，详细参加《搬运工客户端手册》的C#与js互相调用
//        /// </summary>
//        /// <remarks>
//        /// Matches https://github.com/dotnet/runtime/blob/54906ea87c9d8ff3df0b341f02ae255fd58820bd/src/mono/wasm/runtime/driver.c#L417
//        /// </remarks>
//        [MethodImpl(MethodImplOptions.InternalCall)]
//        [EditorBrowsable(EditorBrowsableState.Never)]
//        internal static extern string InvokeJS(string str, out int exceptional_result);
//    }
//}

//namespace WebAssembly
//{
//	internal sealed class Runtime
//	{
//		/// <summary>
//		/// 升级.net5后此方法失效，只适用于旧版mono！
//		/// mono-wasm中通过mono_add_internal_call函数在启动时注册方法名称和具体逻辑的映射
//		/// c代码中注册的方法名称：WebAssembly.Runtime:InvokeJS，所以命名空间和名称都不能修改！
//		/// c#调用js最终通过c的宏EM_ASM_INT，详细参加《搬运工客户端手册》的C#与js互相调用
//		/// </summary>
//		/// <param name="str"></param>
//		/// <param name="exceptional_result"></param>
//		/// <returns></returns>
//		[MethodImpl(MethodImplOptions.InternalCall)]
//		internal static extern string InvokeJS(string str, out int exceptional_result);
//	}
//}
#endif