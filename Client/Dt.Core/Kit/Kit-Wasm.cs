#if WASM
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-06-22 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
            var r = Interop.Runtime.InvokeJS(p_js, out var exceptionResult);
            if (exceptionResult != 0)
            {
                Console.Error.WriteLine($"Error #{exceptionResult} \"{r}\" executing javascript: \"{p_js}\"");
            }
            //else
            //{
            //    Console.WriteLine($"InvokeJS: [{p_js}]: {r}");
            //}
            return r;
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
        /// 根据当前浏览器的url获取服务地址，如：
        /// <para> https://localhost/fz/ui/ 的服务地址 https://localhost/fz </para>
        /// <para>因开发调试时使用IIS Express无法通过url获得服务地址，外部直接p_debugUrl设置服务地址</para>
        /// </summary>
        /// <param name="p_debugUrl">开发调试时的服务地址</param>
        /// <returns>形如："https://10.10.1.16/fz"</returns>
        public static string GetServerUrl(string p_debugUrl)
        {
            // 获取浏览器中的地址
            var str = InvokeJS("location.href");
            var match = Regex.Match(str, @"^https://[^\s/]+/[^\s/]+");
            if (match.Success)
                p_debugUrl = match.Value;

            Console.WriteLine($"服务器地址：{p_debugUrl}");
            return p_debugUrl;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="p_url">文件完整路径</param>
        /// <param name="p_name">文件名称</param>
        public static void Download(string p_url, string p_name)
        {
            InvokeJS($"var a = document.createElement(\"a\");a.href = \"{p_url}\";a.download = \"{p_name}\";a.click();");
        }

        static HostOS GetHostOS()
        {
            string os = InvokeJS("navigator.userAgent.toLowerCase()");
            Console.WriteLine($"UserAgent：{os}");
            if (os.IndexOf("win") > -1)
                return HostOS.Windows;

            if (os.IndexOf("iphone") > -1)
                return HostOS.iOS;

            if (os.IndexOf("android") > -1)
                return HostOS.Android;

            if (os.IndexOf("mac") > -1)
                return HostOS.Mac;

            if (os.IndexOf("linux") > -1)
                return HostOS.Linux;

            return HostOS.Other;
        }
    }
}

internal sealed class Interop
{
    /// <summary>
    /// .NET5 MonoVM and upward specific internal call.
    /// 禁止混淆名称
    /// </summary>
    [Obfuscation(Feature = "renaming", Exclude = true)]
    internal sealed class Runtime
    {
        /// <summary>
        /// 升级.NET5后此方法名称由 WebAssembly.Runtime:InvokeJS 转 Interop.Runtime:InvokeJS
        /// mono-wasm中通过mono_add_internal_call函数在启动时注册方法名称和具体逻辑的映射
        /// c代码中注册的方法名称：Interop.Runtime:InvokeJS，所以命名空间和名称都不能修改！
        /// c#调用js最终通过c的宏EM_ASM_INT，详细参加《搬运工客户端手册》的C#与js互相调用
        /// </summary>
        /// <remarks>
        /// Matches https://github.com/dotnet/runtime/blob/54906ea87c9d8ff3df0b341f02ae255fd58820bd/src/mono/wasm/runtime/driver.c#L417
        /// </remarks>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static extern string InvokeJS(string str, out int exceptional_result);
    }
}

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