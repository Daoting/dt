#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-07-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Serilog.Events;
using Serilog.Parsing;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Core;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 输出信息
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 输出信息，与Log不同，始终将信息输出到控制台和实时日志窗口
        /// </summary>
        /// <param name="p_msg">信息</param>
        public static void Trace(string p_msg)
        {
#if WASM || DESKTOP
            Console.WriteLine(p_msg);
#else
            System.Diagnostics.Debug.WriteLine(p_msg);
#endif

            var item = new TraceLogItem
            {
                Info = $"[{DateTime.Now:HH:mm:ss.fff} Trace]",
                Msg = p_msg
            };
            TraceLogs.AddItem(item);
        }
        
    }
}