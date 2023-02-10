using Dt.Base;
using Dt.Base.Tools;
using Dt.Core;
using System;
using Microsoft.UI.Xaml;


namespace Dt.UIDemo
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

        void OnRpc(object sender, RoutedEventArgs e)
        {
            Log.ForContext("SourceContext", "Rpc")
                .Debug("Http请求响应信息");
        }

        void OnSqlite(object sender, RoutedEventArgs e)
        {
            Log.ForContext("SourceContext", "Sqlite")
                .Debug("Sqlite访问信息");
        }

        void OnPush(object sender, RoutedEventArgs e)
        {
            Log.ForContext("SourceContext", "Push")
                .Debug("Push推送信息");
        }

        void OnSrcDebug(object sender, RoutedEventArgs e)
        {
            // SourceContext：Dt.UIDemo.LogDemo
            Log.ForContext<LogDemo>()
                .Debug("Serilog中将日志分为六级：Verbose(冗余详细级)、Debug(内部调试级)、Information(普通信息级)、Warning(警告级)、Error(错误级)、Fatal(崩溃级)，以上为递增顺序，可以通过设置最小级别控制输出内容。");
        }

        void OnSrcNormal(object sender, RoutedEventArgs e)
        {
            Log.ForContext<LogDemo>()
                .Information("普通信息内容");
        }

        void OnSrcWarn(object sender, RoutedEventArgs e)
        {
            Log.ForContext<LogDemo>()
                .Warning(new Exception("异常信息内容"), "警告信息内容");
        }

        void OnSrcError(object sender, RoutedEventArgs e)
        {
            Log.ForContext<LogDemo>()
                .Error(new Exception("异常信息内容"), "出错信息内容");
        }

        void OnDetail(object sender, RoutedEventArgs e)
        {
            Log.ForContext("Detail", "详细内容描述")
                .Information("点击右上菜单，可以查看或复制详细内容");
        }

        void OnFormat(object sender, RoutedEventArgs e)
        {
            // 推荐
            Log.Debug("PhoneUI模式：{Mode}，操作系统：{OS}", Kit.IsPhoneUI, Kit.HostOS);

            // 不推荐
            //Log.Debug($"PhoneUI模式：{Kit.IsPhoneUI}，操作系统：{Kit.HostOS}");
        }

        void OnAddContext(object sender, RoutedEventArgs e)
        {
            // 简单写法
            //Log.ForContext("OS", Kit.HostOS)
            //    .Debug("临时附加属性");

            // log输出的日志都包含两个附加属性
            var log = Log.ForContext("IsPhoneUI", Kit.IsPhoneUI)
                         .ForContext("OS", Kit.HostOS);

            log.Debug("信息1");
            log.Information("信息2");
        }

        void OnDsDebug(object sender, RoutedEventArgs e)
        {
            LogDemoDs.Me.LogDebug();
        }

        void OnDsNormal(object sender, RoutedEventArgs e)
        {
            LogDemoDs.Me.LogInfo();
        }

        void OnDsWarn(object sender, RoutedEventArgs e)
        {
            LogDemoDs.Me.LogWarn();
        }

        void OnDsError(object sender, RoutedEventArgs e)
        {
            LogDemoDs.Me.LogError();
        }
    }

    class LogDemoDs : DomainSvc<LogDemoDs, AtState.Info>
    {
        LogDemoDs() { }

        public void LogDebug()
        {
            _log.ForContext("Detail", "详细内容")
                .Debug("内部调试级信息，含详细内容");
        }

        public void LogInfo()
        {
            _log.Information("普通信息内容");
        }

        public void LogWarn()
        {
            _log.Warning(new Exception("异常信息内容"), "警告信息内容");
        }

        public void LogError()
        {
            _log.Error(new Exception("异常信息内容"), "出错信息内容");
        }
    }
}
