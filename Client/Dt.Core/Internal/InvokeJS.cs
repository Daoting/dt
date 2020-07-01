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
using System.Runtime.CompilerServices;
using System.Text;
#endregion

namespace WebAssembly
{
    internal sealed class Runtime
	{
		/// <summary>
		/// mono-wasm中通过mono_add_internal_call函数在启动时注册方法名称和具体逻辑的映射
		/// c代码中注册的方法名称：WebAssembly.Runtime:InvokeJS，所以命名空间和名称都不能修改！
		/// c#调用js最终通过c的宏EM_ASM_INT，详细参加《搬运工客户端手册》的C#与js互相调用
		/// </summary>
		/// <param name="str"></param>
		/// <param name="exceptional_result"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string InvokeJS(string str, out int exceptional_result);
	}
}
#endif