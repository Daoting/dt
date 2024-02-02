#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-08-04
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 模拟 Dt.Core.Kit 的部分功能
    /// </summary>
    public static class ExcelKit
    {
        /// <summary>
        /// 主窗口，App启动时由外部设置
        /// </summary>
        public static Window MainWin { get; set; }

        /// <summary>
        /// 发布消息提示
        /// </summary>
        /// <param name="p_content">显示内容</param>
        /// <param name="p_delaySeconds">
        /// 几秒后自动关闭，默认3秒
        /// <para>大于0：启动定时器自动关闭，点击也关闭</para>
        /// <para>0：不自动关闭，但点击关闭</para>
        /// <para>小于0：始终不关闭，只有程序控制关闭</para>
        /// </param>
        public static void Msg(string p_content, int p_delaySeconds = 3)
        {
            var method = GetMethod("Msg");
            if (method != null)
                method.Invoke(null, new object[] { p_content, p_delaySeconds });
        }

        /// <summary>
        /// 警告提示
        /// </summary>
        /// <param name="p_content">显示内容</param>
        /// <param name="p_delaySeconds">
        /// 几秒后自动关闭，默认5秒
        /// <para>大于0：启动定时器自动关闭，点击也关闭</para>
        /// <para>0：不自动关闭，但点击关闭</para>
        /// <para>小于0：始终不关闭，只有程序控制关闭</para>
        /// </param>
        public static void Warn(string p_content, int p_delaySeconds = 5)
        {
            var method = GetMethod("Warn");
            if (method != null)
                method.Invoke(null, new object[] { p_content, p_delaySeconds });
        }

        static MethodInfo GetMethod(string p_name)
        {
            var tp = Type.GetType("Dt.Core.Kit,Dt.Core");
            if (tp != null)
                return tp.GetMethod(p_name, BindingFlags.Public | BindingFlags.Static);
            return null;
        }

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
            var dispatcher = MainWin.DispatcherQueue;
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
            var dispatcher = MainWin.DispatcherQueue;
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
            MainWin.DispatcherQueue.TryEnqueue(new DispatcherQueueHandler(p_action));
        }
        #endregion
    }
}