using Dt.Base;
using Dt.Base.Tools;
using Dt.Core;
using System;
using Microsoft.UI.Xaml;


namespace Dt.Sample
{
    public sealed partial class LogDemo : Win
    {
        public LogDemo()
        {
            InitializeComponent();
            if (!Kit.IsPhoneUI)
                _fv.FirstLoaded(() => SysTrace.ShowBox());
        }

        void OnDebug(object sender, RoutedEventArgs e)
        {
            Log.Debug("Serilog中将日志分为六级：Verbose(冗余详细级)、Debug(内部调试级)、Information(普通信息级)、Warning(警告级)、Error(错误级)、Fatal(崩溃级)，以上为递增顺序，可以通过设置最小级别控制输出内容。");
        }

        void OnNormal(object sender, RoutedEventArgs e)
        {
            Log.Information("普通信息内容");
        }

        void OnWarn(object sender, RoutedEventArgs e)
        {
            Log.Warning(new Exception("异常信息内容"), "警告信息内容");
        }

        void OnError(object sender, RoutedEventArgs e)
        {
            Log.Error(new Exception("异常信息内容"), "出错信息内容");
        }

        void OnRequest(object sender, RoutedEventArgs e)
        {
            Log.ForContext("Rpc", "Call")
                .Debug("Http请求信息");
        }

        void OnResponse(object sender, RoutedEventArgs e)
        {
            Log.ForContext("Rpc", "Recv")
                .Debug("Http响应信息");
        }

        void OnUnhandled(object sender, RoutedEventArgs e)
        {
            throw new Exception("未处理异常信息");
        }

        void OnExcept(object sender, RoutedEventArgs e)
        {
            Throw.Msg("业务警告");
        }
    }
}
