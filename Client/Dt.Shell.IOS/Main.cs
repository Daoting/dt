using CoreFoundation;
using Dt.Core;
using System;
using UIKit;

namespace Dt.Shell.iOS
{
    public class Application
    {
        static void Main(string[] args)
        {
            try
            {
                UIApplication.Main(args, null, typeof(Dt.Shell.App));
            }
            catch (Exception ex)
            {
                Kit.OnUnhandledException(ex);
                RunLoop();
            }
        }

        /// <summary>
        /// 原创方法，防止异常时闪退，碰巧好使
        /// 网上未找到处理方法，已测试的方法有：
        /// ObjCRuntime.Runtime.MarshalManagedException += OnIOSUnhandledException;
        /// AppDomain.CurrentDomain.UnhandledException
        /// NSSetUncaughtExceptionHandler signal
        /// Mono.Runtime.RemoveSignalHandlers
        /// </summary>
        static void RunLoop()
        {
            bool hasException = false;
            var loop = CFRunLoop.Current;
            while (true)
            {
                try
                {
                    loop.RunInMode(CFRunLoop.ModeDefault, 0.001, false);
                }
                catch (Exception ex)
                {
                    hasException = true;
                    Kit.OnUnhandledException(ex);
                    break;
                }
            }

            if (hasException)
                RunLoop();
        }
    }
}