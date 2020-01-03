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
#endregion

namespace Dt.Cells.Data
{
    internal static class UIAdaptor
    {
        private static readonly CoreDispatcher _dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

        public static async void InvokeAsync(Action action)
        {
            if (_dispatcher.HasThreadAccess)
                action();
            else
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(action));
        }

        public static void InvokeSync(Action action)
        {
            if (_dispatcher.HasThreadAccess)
                action();
            else
                WindowsRuntimeSystemExtensions.AsTask(_dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(action))).Wait();
        }
    }
}

