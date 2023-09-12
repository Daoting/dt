﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 基础功能杂项
    /// </summary>
    public partial class Kit
    {
        #region UI线程调用
        /**********************************************************************************************************************************************************/
        // 升级WinUI后的问题：
        //
        // 1. WinUI 中 Window.Dispatcher 和 DependencyObject.Dispatcher 始终null，只能使用 DispatcherQueue！
        //
        // 2. uno 中 Window.DispatcherQueue 未实现，RootGrid.DispatcherQueue 在 Task 中访问为 null，只能使用 UWP 时的方式！ (uno4.4 已实现)
        //
        /***********************************************************************************************************************************************************/

        /// <summary>
        /// 确保在UI线程调用给定方法
        /// </summary>
        /// <param name="p_action"></param>
        public static void RunAsync(Action p_action)
        {
            // uno4.4 已实现 Window.DispatcherQueue
            var dispatcher = UITree.MainWin.DispatcherQueue;
            if (dispatcher.HasThreadAccess)
            {
                p_action();
            }
            else
            {
                dispatcher.TryEnqueue(new DispatcherQueueHandler(p_action));
            }
        }

        /// <summary>
        /// 确保在UI线程同步调用给定方法
        /// </summary>
        /// <param name="p_action"></param>
        public static void RunSync(Action p_action)
        {
            var dispatcher = UITree.MainWin.DispatcherQueue;
            if (dispatcher.HasThreadAccess)
            {
                p_action();
                return;
            }

            var taskSrc = new TaskCompletionSource<bool>();
            dispatcher.TryEnqueue(() =>
            {
                p_action();
                taskSrc.TrySetResult(true);
            });
            taskSrc.Task.Wait();
        }

        /// <summary>
        /// 始终在UI线程的DispatcherQueue中调用给定方法，和RunAsync方法不同
        /// <para>RunAsync根据UI线程的访问情况直接调用 或 在DispatcherQueue中调用给定方法</para>
        /// </summary>
        /// <param name="p_action"></param>
        public static void RunInQueue(Action p_action)
        {
            // uno4.4 已实现 Window.DispatcherQueue
            UITree.MainWin.DispatcherQueue.TryEnqueue(new DispatcherQueueHandler(p_action));
        }
        #endregion

        #region 输出调试信息
        /// <summary>
        /// 只在调试时向输出窗口或控制台输出信息(内部用)，不同于Log
        /// </summary>
        /// <param name="p_msg">信息</param>
        public static void Debug(string p_msg)
        {
#if DEBUG
#if WASM
            Console.WriteLine(p_msg);
#else
            System.Diagnostics.Debug.WriteLine(p_msg);
#endif
#endif
        }
        #endregion

        #region 可视区域
        /// <summary>
        /// 主窗口
        /// </summary>
        public static Window MainWin => UITree.MainWin;

        /// <summary>
        /// Window.Content内容，根Grid
        /// </summary>
        public static Grid RootGrid => UITree.RootGrid;

        /// <summary>
        /// 桌面层/页面层的内容元素
        /// </summary>
        public static UIElement RootContent => UITree.RootContent;

        /// <summary>
        /// 可视区域宽度
        /// 手机：页面宽度
        /// PC上：除标题栏和外框的窗口内部宽度
        /// </summary>
        public static double ViewWidth
        {
            get
            {
#if IOS
                // ios首页未显示前uno中的Bounds为0x0
                var w = UITree.MainWin.Bounds.Width;
                if (w > 0)
                    return w;

                var info = Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfo;
                return info.Width / info.Density;
#else
                return UITree.MainWin.Bounds.Width;
#endif
            }
        }

        /// <summary>
        /// 可视区域高度
        /// 手机：不包括状态栏的高度
        /// PC上：除标题栏和外框的窗口内部高度
        /// </summary>
        public static double ViewHeight
        {
            get
            {
#if IOS
                // ios首页未显示前uno中的Bounds为0x0
                var h = UITree.MainWin.Bounds.Height;
                if (h <= 0)
                {
                    var info = Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfo;
                    h = info.Height / info.Density;
                }
                return h - UITree.StatusBarHeight;
#else
                return UITree.MainWin.Bounds.Height - UITree.StatusBarHeight;
#endif
            }
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 获取新Guid，小写无连字符'-'
        /// </summary>
        public static string NewGuid => Guid.NewGuid().ToString("N");

        /// <summary>
        /// 将文本复制到剪贴板
        /// </summary>
        /// <param name="p_text"></param>
        /// <param name="p_showText">是否显示要复制的内容</param>
        public static void CopyToClipboard(string p_text, bool p_showText = false)
        {
            DataPackage data = new DataPackage();
            data.SetText(p_text);
            Clipboard.SetContent(data);
            if (p_showText)
                Msg("已复制到剪切板：\r\n" + p_text);
            else
                Msg("已复制到剪切板！");
        }

        /// <summary>
        /// 将字节长度转成描述信息
        /// </summary>
        /// <param name="p_size"></param>
        /// <returns></returns>
        public static string GetFileSizeDesc(ulong p_size)
        {
            if (p_size < KB)
                return string.Format("{0} 字节", p_size);
            if (p_size < MB)
                return string.Format("{0} KB", Math.Round(p_size / (float)KB, 2));
            if (p_size < GB)
                return string.Format("{0} MB", Math.Round(p_size / (float)MB, 2));
            return string.Format("{0} GB", Math.Round(p_size / (float)GB, 2));
        }

        /// <summary>
        /// 获取字符串按utf8编码的字节长度
        /// </summary>
        /// <param name="p_content">要计算的字符串 </param>
        /// <returns></returns>
        public static int GetUtf8Length(string p_content)
        {
            if (string.IsNullOrEmpty(p_content))
                return 0;
            return Encoding.UTF8.GetBytes(p_content).Length;
        }

        static Encoding _gbk;
        /// <summary>
        /// 获取字符串按GBK编码的字节长度
        /// </summary>
        /// <param name="p_content"></param>
        /// <returns></returns>
        public static int GetGbkLength(string p_content)
        {
            if (string.IsNullOrEmpty(p_content))
                return 0;

            if (_gbk == null)
            {
                _gbk = Encoding.GetEncoding("GBK");
            }
            return _gbk.GetBytes(p_content).Length;
        }

        /// <summary>
        /// 获取字符串按Unicode编码的字节长度
        /// </summary>
        /// <param name="p_content"></param>
        /// <returns></returns>
        public static int GetUnicodeLength(string p_content)
        {
            if (string.IsNullOrEmpty(p_content))
                return 0;
            return Encoding.Unicode.GetBytes(p_content).Length;
        }

        /// <summary>
        /// 转换特殊字符 导出Xaml时用
        /// </summary>
        /// <param name="p_txt"></param>
        /// <returns></returns>
        public static string ConvertSpecialStr(string p_txt)
        {
            p_txt = Regex.Replace(p_txt, "&", "&amp;");
            p_txt = Regex.Replace(p_txt, ">", "&gt;");
            p_txt = Regex.Replace(p_txt, "<", "&lt;");
            p_txt = Regex.Replace(p_txt, "\"", "&quot;");
            p_txt = Regex.Replace(p_txt, "'", "&apos;");
            p_txt = Regex.Replace(p_txt, "{", "{}{");
            return p_txt;
        }

        /// <summary>
        /// 转换对象的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_val"></param>
        /// <returns></returns>
        public static T ConvertType<T>(object p_val)
        {
            if (p_val != null)
            {
                if (typeof(T) == p_val.GetType())
                {
                    // 结果对象与给定对象类型相同时
                    return (T)p_val;
                }

                object val = null;
                try
                {
                    val = Convert.ChangeType(p_val, typeof(T));
                }
                catch
                {
                    throw new Exception(string.Format("无法将【{0}】转换到【{1}】类型！", val, typeof(T)));
                }
                return (T)val;
            }
            return default(T);
        }

        /// <summary>
        /// 获得给定字符串的字节个数
        /// </summary>
        /// <param name="p_txt"></param>
        /// <returns></returns>
        public static int GetByteCount(string p_txt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(p_txt);
            int count = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i % 2 == 0)
                    count++;
                else if (bytes[i] > 0)
                    count++;
            }
            return count;
        }
        #endregion

        #region 常量
        /// <summary>
        /// 1GB
        /// </summary>
        public const int GB = 1024 * 1024 * 1024;

        /// <summary>
        /// 1MB
        /// </summary>
        public const int MB = 1024 * 1024;

        /// <summary>
        /// 1KB
        /// </summary>
        public const int KB = 1024;
        #endregion
    }
}