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
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// wasm静态工具类
    /// </summary>
    public static class AtWasm
    {
        const string _jsDownload = "var a = document.createElement(\"a\");a.href = \"{0}\";a.download = \"{1}\";a.click();";

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
            InvokeJS(string.Format(_jsDownload, p_url, p_name));
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