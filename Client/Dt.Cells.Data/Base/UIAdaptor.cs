#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.Data
{
    internal static class UIAdaptor
    {
        public static async void InvokeAsync(Action action)
        {
            if (Window.Current.Content.Dispatcher.HasThreadAccess)
                action();
            else
                await Window.Current.Content.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(action));
        }

        public static void InvokeSync(Action action)
        {
            if (Window.Current.Content.Dispatcher.HasThreadAccess)
                action();
            else
                WindowsRuntimeSystemExtensions.AsTask(Window.Current.Content.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(action))).Wait();
        }
    }
}

