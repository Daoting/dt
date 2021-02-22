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
        /// 获取服务器地址，形如："https://10.10.1.16/fz"
        /// </summary>
        /// <returns></returns>
        public static string GetServerUrl()
        {
            // 获取浏览器中的地址
            var str = InvokeJS("location.href");
            var match = Regex.Match(str, @"^https://[^\s/]+/[^\s/]+");
            if (match.Success)
                return match.Value;

            Console.Error.WriteLine("服务器地址格式错误！");
            return "";
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