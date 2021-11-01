using Dt.Base;
using Dt.Base.Tools;
using Dt.Core;
using System;
using Windows.UI.Xaml;


namespace Dt.Sample
{
    public sealed partial class SysTraceDemo : Win
    {
        public SysTraceDemo()
        {
            InitializeComponent();
            if (!Kit.IsPhoneUI)
                _fv.FirstLoaded(() => SysTrace.ShowBox());
        }

        void OnNormal(object sender, RoutedEventArgs e)
        {
            Kit.Trace("普通信息标题", "信息内容！");
        }

        void OnRequest(object sender, RoutedEventArgs e)
        {
            Kit.Trace(TraceOutType.RpcCall, "Http请求信息", "Rpc请求内容！", "服务名称");
        }

        void OnResponse(object sender, RoutedEventArgs e)
        {
            Kit.Trace(TraceOutType.RpcRecv, "Http响应信息", "服务器端返回的内容！", "服务名称");
        }

        void OnWsRequest(object sender, RoutedEventArgs e)
        {
            Kit.Trace(TraceOutType.WsCall, "Ws请求信息", "WebSocket请求内容！");
        }

        void OnWsResponse(object sender, RoutedEventArgs e)
        {
            Kit.Trace(TraceOutType.WsRecv, "Ws响应信息", "WebSocket返回内容！");
        }

        void OnServerPush(object sender, RoutedEventArgs e)
        {
            Kit.Trace(TraceOutType.ServerPush, "服务器推送", "详细内容");
        }

        void OnRpcException(object sender, RoutedEventArgs e)
        {
            Kit.Trace(TraceOutType.RpcException, "远程调用异常信息", "异常信息详细内容");
        }

        void OnUnhandled(object sender, RoutedEventArgs e)
        {
            Kit.Trace(TraceOutType.UnhandledException, "未处理异常信息", "异常信息详细内容！");
        }

        void OnDebugExcept(object sender, RoutedEventArgs e)
        {
            throw new Exception("未处理异常信息");
        }

        void OnExcept(object sender, RoutedEventArgs e)
        {
            Throw.Msg("业务警告");
        }
    }
}
